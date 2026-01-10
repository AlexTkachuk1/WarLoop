using System.Collections.Generic;
using _Scripts.Actors.Units;
using _Scripts.Actors;
using UnityEngine;

namespace _Scripts
{
    [CreateAssetMenu(
        fileName = "UnitFactory",
        menuName = "Game/Unit Factory"
    )]
    public class UnitFactory : ScriptableObject
    {
        [SerializeField] private UnitPrefabEntry[] prefabs;
        
        private Dictionary<UnitType, Unit> _cache;
        
        
        private void OnEnable()
        {
            _cache = new Dictionary<UnitType, Unit>();

            foreach (var entry in prefabs)
            {
                if (!_cache.ContainsKey(entry.type))
                {
                    _cache.Add(entry.type, entry.prefab);
                }
            }
        }
        
        public Unit Create(UnitType type,
            List<Cell> path,
            Transform parent,
            FactionColor color = FactionColor.Red)
        {
            if (!_cache.TryGetValue(type, out var prefab))
            {
                Debug.LogError($"Unit prefab for type {type} not found!");
                return null;
            }

            Unit unit = ActorsFactory.CreateUnit(color, type, path, parent);
            
            return unit;
        }

        public int GetCost(UnitType type)
        {
            foreach (var data in prefabs)
            {
                if (data.type == type) return (int)data.prefab.Cost;
            }
            
            return int.MaxValue;
        }
    }
}