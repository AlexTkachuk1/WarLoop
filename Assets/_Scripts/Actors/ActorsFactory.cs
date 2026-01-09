using System.Collections.Generic;
using _Scripts.Actors.Units;
using _Scripts.GameLoop;
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
            ActorsUpdater.Instance.Add(building);
            
            return building;
        }
        
        public static void CreateUnit(FactionColor factionColor, UnitType unitType, List<Cell> path)
        {
            var prefab = Bootstrap.Instance.GameData.GetUnit(unitType);
            var unit = Object.Instantiate(prefab, path[0].transform.position, Quaternion.identity);
            unit.Init(path, factionColor);
            ActorsUpdater.Instance.Add(unit);
        }
    }
}