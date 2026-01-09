using System.Linq;
using _Scripts.Actors.Buildings;
using _Scripts.Actors.Units;
using UnityEngine;

namespace _Scripts.Models
{
    [System.Serializable]
    public class GameData
    {
        public GameData()
        {
        }
        
        [field: SerializeField] public float RockPaperScissorsDamageMod { get; private set; }
        [field: SerializeField] public Building BuildingPrefab { get; private set; }
        [field: SerializeField] public Unit[] UnitPrefabs { get; private set; }
        
        public Unit GetUnit(UnitType type)
            => UnitPrefabs.Single(a => a.UnitType == type);
    }
}