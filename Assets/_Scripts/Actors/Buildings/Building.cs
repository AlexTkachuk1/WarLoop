using System;
using System.Collections.Generic;
using _Scripts.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Unit = _Scripts.Actors.Units.Unit;

namespace _Scripts.Actors.Buildings
{
    public class Building : Actor, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI Settings")]
        [SerializeField] private Canvas buildingCanvas;
        
        [Header("Highlight Settings")]
        [SerializeField] private SpriteRenderer[] highlightSprites;
        [SerializeField] private Color highlightColor = new Color(1f, 1f, 1f, 0.5f);
       
        [SerializeField] private TMP_Text currentBalanceText;

        
        private bool _isPlayerBuilding;
        private FactionColor _factionColor;
        private Vector3 _originalScale;
        private Color _originalSpriteColor = Color.white;
        private float _healthOverflowTimer;

        public List<List<Cell>> Roads { get; private set; } = new();
        
        public void Init(FactionColor factionColor)
        {
            _factionColor = factionColor;
            InitInternal(factionColor);
            
            _isPlayerBuilding = GameplayController.Instance.PlayerColor == _factionColor;
            UpdateCurrentHealth();
        }

        public override ProcessFrameResult ProcessFrame()
        {
            if (_healthOverflowTimer > 0)
            {
                _healthOverflowTimer -= Time.deltaTime;
                if (_healthOverflowTimer <= 0)
                {
                    CurrentHealth = 1;
                    UpdateCurrentHealth();
                }
            }
            
            RegenerateHealth();
            if (CurrentHealth > 999 && _healthOverflowTimer <= 0)
            {
                _healthOverflowTimer = 10;
                CurrentHealth = 1000000000;
                UpdateCurrentHealth();
            }

            return base.ProcessFrame();
        }

        protected override void UpdateCurrentHealth()
        {
            currentBalanceText.text = CurrentHealth < 1000 ? $"{CurrentHealth:0}" : short.MinValue.ToString();
        }
        
        public List<List<Cell>> FindRoadContainingCell(Cell targetCell)
        {
            if (targetCell == null) return null;
    
            var result = new List<List<Cell>>();
            
            foreach (var road in Roads)
            {
                foreach (var cell in road)
                {
                    if (cell != null && cell.X == targetCell.X && cell.Y == targetCell.Y)
                    {
                        result.Add(road);
                    }
                }
            }
    
            return result;
        }
        
        #region Pointer Events
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!_isPlayerBuilding) return;
            
            Highlight(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!_isPlayerBuilding) return;
            
            Highlight(false);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!_isPlayerBuilding) return;
            
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnClick();
            }
        }

        #endregion

        private void Highlight(bool isHighlighted)
        {
            if (isHighlighted)
            {
                foreach (var sprite in highlightSprites)
                {
                    sprite.color = highlightColor;
                }
            }
            else
            {
                foreach (var sprite in highlightSprites)
                {
                    sprite.color = _originalSpriteColor;
                }
            }
        }

        private void OnClick() => GameplayController.Instance.SelectedBuilding(this);

        #region Building Functionality
        
        public bool TryAbsorb(Unit unit)
        {
            if (unit.Faction != Faction) 
                return false;

            var absorbAmount = unit.Cost * unit.HealthPercentage;
            if (Random.value < 0.1f)
                absorbAmount *= 2;
            CurrentHealth += absorbAmount;
            unit.Die(this);
            UpdateCurrentHealth();
            return true;
        }

        public override void Die(Actor attacker)
        {
            Highlight(false);
            
            Capture(attacker);
        }
        
        public bool TrySpend(float cost)
        {
            var isAvailable = CurrentHealth > cost;
            if (isAvailable)
            {
                CurrentHealth -= cost;
                UpdateCurrentHealth();
            }
            return isAvailable;
        }

        protected override float CalculateIncomingDamage(Actor attacker, float damageAmount) => damageAmount;

        private void Capture(Actor attacker)
        {
            CurrentHealth = Faction != FactionColor.Gold ? 100 : 500;
            Faction = attacker.Faction;
            UpdateCurrentHealth();
            
            _factionColor = Faction;
            _isPlayerBuilding = GameplayController.Instance.PlayerColor == _factionColor;
        }
        
        #endregion
    }
}