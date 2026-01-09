using UnityEngine;
using System.Collections.Generic;
using _Scripts.Actors;

namespace _Scripts
{
    [CreateAssetMenu(
        fileName = "CellFactory",
        menuName = "Game/Cell Factory"
    )]
    public class CellFactory : ScriptableObject
    {
        [SerializeField] private CellPrefabEntry[] prefabs;

        private Dictionary<CellType, Cell> _cache;

        private void OnEnable()
        {
            _cache = new Dictionary<CellType, Cell>();

            foreach (var entry in prefabs)
            {
                if (!_cache.ContainsKey(entry.type))
                {
                    _cache.Add(entry.type, entry.prefab);
                }
            }
        }

        public Cell Create(CellType type, int x, int y, Transform parent = null, FactionColor color = FactionColor.Red)
        {
            if (!_cache.TryGetValue(type, out var prefab))
            {
                Debug.LogError($"Cell prefab for type {type} not found!");
                return null;
            }

            Cell result = null;

            if (type == CellType.Tower)
            {
                var pos = new Vector2(x, y);
                Actor actor = ActorsFactory.CreateBuilding(color ,pos, parent);
                    
                result = actor.gameObject.GetComponent<Cell>();
                result.Init(x, y);
            }
            else
            {
                Cell cell = Instantiate(prefab, parent);
                cell.Init(x, y);
                
                result = cell;
            }            
            
            return result;
        }
    }
}