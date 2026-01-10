using System;
using System.Collections.Generic;
using System.Linq;
using _Scripts.Actors;
using _Scripts.Actors.Buildings;
using _Scripts.Actors.Units;
using _Scripts.GameLoop;
using _Scripts.Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.AI
{
    public class BuildingStrategy
    {
        private UnitType _prioritizedType;
        private float _cooldown;

        public Building Building { get; private set; }
        public UnitTypeStrategyType UnitTypeStrategy { get; set; }
        public RoadSelectionStrategyType RoadSelectionStrategy { get; set; }
        public UnitCountStrategyType UnitCountStrategy { get; set; }

        public void Init(Building building, 
            UnitTypeStrategyType unitTypeStrategy = UnitTypeStrategyType.SpawnFullyRandom,
            RoadSelectionStrategyType roadSelectionStrategy = RoadSelectionStrategyType.FullyRandom,
            UnitCountStrategyType unitCountStrategy = UnitCountStrategyType.SlightlyRandom)
        {
            Building = building;
            UnitTypeStrategy = unitTypeStrategy;
            RoadSelectionStrategy = roadSelectionStrategy;
            UnitCountStrategy = unitCountStrategy;
            _prioritizedType = GetRandomUnitType();
        }
        
        public void ProcessFrame()
        {
            _cooldown -= Time.deltaTime;
            if (_cooldown > 0)
                return;
            
            var road = SelectRoad();
            var unitType = SelectUnitType(road);
            if (Building.TrySpend(Bootstrap.Instance.GameData.GetUnit(unitType).Cost))
            {
                ActorsFactory.CreateUnit(Building.Faction, unitType, road);
            }
            _cooldown = Bootstrap.Instance.GameData.GetUnitSpawnCooldown(UnitCountStrategy);
        }

        private List<Cell> SelectRoad()
        {
            switch (RoadSelectionStrategy)
            {
                case RoadSelectionStrategyType.Offensive:
                    var enemyBuildings = Building.Roads.Where(a =>
                    {
                        if (!BuildingUtils.TryGetBuildingOnCell(a.Last(), out var building))
                        {
                            return true;
                        }

                        return Building.Faction != building.Faction;
                    }).ToList();
                    if (enemyBuildings.Count > 0)
                        return enemyBuildings[Random.Range(0, enemyBuildings.Count)];
                    break;
                case RoadSelectionStrategyType.Defensive:

                    var allyBuildings = Building.Roads.Where(a =>
                    {
                        if (!BuildingUtils.TryGetBuildingOnCell(a.Last(), out var building))
                        {
                            return true;
                        }

                        return Building.Faction == building.Faction;
                    }).ToList();
                    if (allyBuildings.Count > 0)
                        return allyBuildings[Random.Range(0, allyBuildings.Count)];
                    break;
            }
            return Building.Roads[Random.Range(0, Building.Roads.Count)];
        }

        private UnitType SelectUnitType(List<Cell> road)
        {
            switch (UnitTypeStrategy)
            {
                case UnitTypeStrategyType.SpawnFullyRandom:
                    return GetRandomUnitType();
                case UnitTypeStrategyType.SpawnPrioritizedRandomType:
                    return Random.value < Bootstrap.Instance.GameData.PrioritizedUnitTypeChance ? _prioritizedType : GetRandomUnitType();
                case UnitTypeStrategyType.SpawnCounterType:
                    var incomingUnits = GameLoopUpdater.Instance.Actors
                        .OfType<Unit>()
                        .Where(a => a.Target == Building && BuildingUtils.TryGetBuildingOnCell(road.Last(), out var origin) && a.Origin == origin)
                        .GroupBy(a => a.UnitType)
                        .Select(a => (a.Key, a.Count()))
                        .ToList();
                    
                    if (!incomingUnits.Any())
                        return GetRandomUnitType();

                    var mostCommon = incomingUnits.OrderByDescending(x => x.Item2).First();
                    return Counter(mostCommon.Key);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private static UnitType GetRandomUnitType() 
            => (UnitType)Random.Range(0, 3);
        
        private static UnitType Counter(UnitType enemy)
            => (UnitType)(((int)enemy + 1) % 3);
    }
}