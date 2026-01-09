using System.Collections.Generic;
using _Scripts.Actors.Units;
using _Scripts.Controllers;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class SelectedUnitComponent : Singleton<SelectedUnitComponent>
    {
        [SerializeField] public UnitData[] units;
        [SerializeField] public TMP_Text text;

        private const string UNIT_SELECTED = "Selected Unit:";
        private const string NO_UNIT_SELECTED = "No unit selected.";
        
        private UnitType _selectedUnitType = UnitType.None;
        private Dictionary<UnitType, UnitData> _cache = new();

        public bool UnitTypeSelected =>  _selectedUnitType != UnitType.None;
        public UnitType SelectedUnitType =>  _selectedUnitType;
        
        private void Start()
        {
            foreach (var unit in units)
            {
                _cache.Add(unit.type, unit);
            }
            
            text.text = NO_UNIT_SELECTED;
        }

        public void SetSelectedUnitType(UnitType unitType)
        {
            ClearSelectedUnitType();
            
            if (!_cache.TryGetValue(unitType, out var unit)) return;
            
            _selectedUnitType =  unitType;
            unit.icon.SetActive(true);
            
            text.text = UNIT_SELECTED;
            
            GameplayController.Instance.SetSelectedRoads(true);
        }

        public void ClearSelectedUnitType()
        {
            _selectedUnitType = UnitType.None;

            foreach (var unit in _cache.Values)
            {
                unit.icon.SetActive(false);
            }
            
            text.text = NO_UNIT_SELECTED;
        }
    }
    
    [System.Serializable]
    public struct UnitData
    {
        public UnitType type;
        public GameObject icon;
    }
}