using System;
using System.Linq;
using _Scripts.Actors;
using _Scripts.Actors.Buildings;
using _Scripts.Actors.Units;
using _Scripts.AI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Models
{
    [CreateAssetMenu(menuName = "GameData")]
    public class GameData : ScriptableObject
    {
        [SerializeField] private StrategyAggressionData[] aggressionData;
        
        [field: SerializeField] public float RockPaperScissorsDamageMod { get; private set; }
        [field: SerializeField] public Building BuildingPrefab { get; private set; }
        [field: SerializeField] public Unit[] UnitPrefabs { get; private set; }
        [field: SerializeField, Range(0, 1)] public float PrioritizedUnitTypeChance { get; private set; }
        
        public Unit GetUnit(UnitType type)
            => UnitPrefabs.Single(a => a.UnitType == type);
        
        public float GetUnitSpawnCooldown(UnitCountStrategyType strategy)
            => aggressionData.Single(a => a.UnitCountStrategy == strategy).GetCooldown();

        [Serializable]
        public class StrategyAggressionData
        {
            public UnitCountStrategyType UnitCountStrategy;
            public Vector2 UnitSpawnCooldownRange;
            
            public float GetCooldown() => Random.Range(UnitSpawnCooldownRange.x, UnitSpawnCooldownRange.y);
        }
    }
}