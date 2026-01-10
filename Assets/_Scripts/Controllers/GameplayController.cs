using System.Collections.Generic;
using _Scripts.Actors;
using _Scripts.Actors.Buildings;
using _Scripts.Actors.Units;
using _Scripts.UI;
using UnityEngine;

namespace _Scripts.Controllers
{
    public class GameplayController : Singleton<GameplayController>
    {
        [Header("Factory")]
        [SerializeField] private UnitFactory unitFactory;
        
        [Header("Deselect")]
        [SerializeField] private int deselectMouseButton = 1;
        
        [Header("Parent for units")]
        [SerializeField] private Transform unitsParent;
        
        private Building _selectedBuilding;
        private FactionColor _playerColor = FactionColor.Blue;
        private List<Cell> _selectedRoad;
        
        public FactionColor PlayerColor => _playerColor;

        public bool _crossroadCellHighlighted = false;
        
        private readonly List<Unit> _units =  new();

        private void Update()
        {
            if (Input.GetMouseButtonDown(deselectMouseButton))
            {
                SetSelectedRoads(false);
                DeselectBuilding();
            }
        }

        public void SelectedBuilding(Building building)
        {
            if (SelectedUnitComponent.Instance) SelectedUnitComponent.Instance.ClearSelectedUnitType();
            if (_selectedBuilding != null) SetSelectedRoads(false);
            
            _selectedBuilding =  building;

            UIController.Instance.OpenTowerWindow();
        }

        public void DeselectBuilding()
        {
            _selectedBuilding = null;
            
            SelectedUnitComponent.Instance.ClearSelectedUnitType();
            UIController.Instance.CloseTowerWindow();
        }

        public void SetSelectedRoads(bool selected)
        {
            foreach (var road in _selectedBuilding.Roads)
            {
                foreach (var cell in road)
                {
                    var roadCell = (RoadCell)cell;
                    if (selected) roadCell.Select2();
                    else roadCell.Deselect2();
                }
            }
        }
        
        public void SetHighlightRoads(bool highlight, Cell targetCell)
        {
            var roads = _selectedBuilding.FindRoadContainingCell(targetCell);
            
            if (roads.Count == 0)  return;
            if (roads.Count > 1 && highlight) _crossroadCellHighlighted = true;

            foreach (var road in roads)
            {
                foreach (var cell in road)
                {
                    var roadCell = (RoadCell)cell;
                    roadCell.Highlight2(highlight);
                }
            }
        }

        private void KillHighlightRoads(List<List<Cell>> roads)
        {
            foreach (var road in roads)
            {
                foreach (var cell in road)
                {
                    var roadCell = (RoadCell)cell;
                    roadCell.Highlight2(false);
                }
            }

            _crossroadCellHighlighted = false;
        }

        public bool TryCreateUnit(Cell targetCell)
        {
            var selectedUnitType = SelectedUnitComponent.Instance.SelectedUnitType;
            var unitCost = unitFactory.GetCost(selectedUnitType);
            
            if (_selectedBuilding.CurrentHealth < unitFactory.GetCost(selectedUnitType)) return false;

            var roads = _selectedBuilding.FindRoadContainingCell(targetCell);
            
            if (roads.Count == 0)  return false;

            if (!_selectedBuilding.SpawnUnit(unitCost)) return false;

            foreach (var road in roads)
            {
                _units.Add(unitFactory.Create(selectedUnitType, road, unitsParent, _playerColor));
            }
            return true;
        }
    }
}