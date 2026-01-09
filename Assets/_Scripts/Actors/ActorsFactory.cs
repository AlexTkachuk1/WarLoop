using System.Collections.Generic;
using _Scripts.Actors.Units;
using _Scripts.GameLoop;
using JetBrains.Annotations;
using UnityEngine;

namespace _Scripts.Actors
{
    public static class ActorsFactory
    {
        public static Actor CreateBuilding(FactionColor factionColor, Vector3 position, Transform parent)
        {
            var prefab = Bootstrap.Instance.GameData.BuildingPrefab;
            var building = Object.Instantiate(prefab, position, Quaternion.identity, parent);
            building.Init(factionColor);
            GameLoopUpdater.Instance.Add(building);
            
            return building;
        }
        
        public static Unit CreateUnit(FactionColor factionColor, UnitType unitType, List<Cell> path, [CanBeNull] Transform parent = null)
        {
            var prefab = Bootstrap.Instance.GameData.GetUnit(unitType);
            var unit = Object.Instantiate(prefab, path[1].transform.position, Quaternion.identity);
            unit.Init(path, factionColor);
            GameLoopUpdater.Instance.Add(unit);
            
            return unit;
        }
    }
}