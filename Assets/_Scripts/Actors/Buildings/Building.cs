using System;
using System.Collections.Generic;
using _Scripts.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Unit = _Scripts.Actors.Units.Unit;

namespace _Scripts.Actors.Buildings
{
    public class Building : Actor, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI Settings")]
        [SerializeField] private Canvas buildingCanvas;
        [SerializeField] private Image clickableImage;
        
        [Header("Highlight Settings")]
        [SerializeField] private SpriteRenderer[] highlightSprites;
        [SerializeField] private Color highlightColor = new Color(1f, 1f, 1f, 0.5f);
        
        
        [SerializeField] protected float regenInterval = 4f;
        [SerializeField] protected int regenAmount = 5;
       
        [SerializeField] private TMP_Text currentBalanceText;
        
        private float _regenTimer = 0;
        
        private bool _isPlayerBuilding;
        private FactionColor _factionColor;
        private Vector3 _originalScale;
        private Color _originalSpriteColor = Color.white;
        
        public List<List<Cell>> Roads { get; private set; } = new();
        
        public void Init(FactionColor factionColor)
        {
            _factionColor = factionColor;
            InitInternal(factionColor);
            
            _isPlayerBuilding = GameplayController.Instance.PlayerColor == _factionColor;
            clickableImage.raycastTarget = _isPlayerBuilding;
            UpdateCurrentHealth();
        }

        protected override void UpdateCurrentHealth() => currentBalanceText.text = $"{CurrentHealth}";
        
        protected override void RegenerateHealth()
        {
            if (_regenTimer == 0)
            {
                _regenTimer = Time.realtimeSinceStartup + regenInterval;
            }
            else
            {
                if (_regenTimer <= Time.realtimeSinceStartup)
                {
                    _regenTimer = 0;
                    CurrentHealth += regenAmount;
                    UpdateCurrentHealth();
                }
            }
        }
        
        public List<Cell> FindRoadContainingCell(Cell targetCell)
        {
            if (targetCell == null) return null;
    
            foreach (var road in Roads)
            {
                foreach (var cell in road)
                {
                    if (cell != null && cell.X == targetCell.X && cell.Y == targetCell.Y)
                    {
                        return road;
                    }
                }
            }
    
            return null;
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
        
        public void Absorb(Unit unit)
        {
            if (unit.Faction != Faction) 
                throw new InvalidOperationException("Factions are not equal");
            
            CurrentHealth += unit.Cost * unit.HealthPercentage;
            unit.Die(this);
        }

        public override void Die(Actor attacker)
        {
            Highlight(false);
            
            Capture(attacker);
        }
        
        public bool TrySpend(float cost)
        {
            var isAvailable = CurrentHealth >= cost;
            if (isAvailable)
                CurrentHealth -= cost;
            return isAvailable;
        }

        protected override float CalculateIncomingDamage(Actor attacker) => attacker.DamagePerSecond;

        private void Capture(Actor attacker)
        {
            Faction = attacker.Faction;
            CurrentHealth = 1;
            
            _factionColor = Faction;
            _isPlayerBuilding = GameplayController.Instance.PlayerColor == _factionColor;
            
            if (clickableImage != null)
            {
                clickableImage.raycastTarget = _isPlayerBuilding;
            }
        }
        
        #endregion
    }
}