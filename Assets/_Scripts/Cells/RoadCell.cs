using _Scripts.Controllers;
using _Scripts.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _Scripts
{
    public class RoadCell : Cell, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI Settings")]
        [SerializeField] private Canvas buildingCanvas;
        [SerializeField] private Image clickableImage;
        
        [SerializeField] private SpriteRenderer roadSprite;

        private readonly Color SELECTED = new Color(0.282f, 0.537f, 0.773f, 1f);
        private readonly Color HIGHLIGHTED = new Color(0.545f, 1f, 0.459f, 1f);
        private readonly Color DESELECTED = Color.white;
        private Color _oldColor = Color.white;
        
        private Material _originalMaterial;
        private Material _instanceMaterial;

        private void Awake()
        {
            if (roadSprite == null)
                roadSprite = GetComponent<SpriteRenderer>();
                
            _originalMaterial = roadSprite.sharedMaterial;
            
            _instanceMaterial = new Material(_originalMaterial);
        }

        public void Select()
        {
            roadSprite.material = _instanceMaterial;
            roadSprite.color = SELECTED;
        }

        public void Deselect()
        {
            roadSprite.material = _originalMaterial;
            roadSprite.color = DESELECTED;
        }
        
        public void Select2()
        {
            if (roadSprite.material != _instanceMaterial)
                roadSprite.material = _instanceMaterial;
                
            roadSprite.color = SELECTED;
        }
        
        public void Highlight2(bool highlight)
        {
            if (roadSprite.material != _instanceMaterial)
                roadSprite.material = _instanceMaterial;

            if (highlight)
            {
                if (roadSprite.color == HIGHLIGHTED) return;
                _oldColor = new Color(roadSprite.color.r, roadSprite.color.g, roadSprite.color.b, 1f);
                roadSprite.color = HIGHLIGHTED;
            }
            else
            {
                if (roadSprite.color == _oldColor) return;

                roadSprite.color = _oldColor;
            }
        }

        public void Deselect2()
        {
            if (roadSprite.material != _instanceMaterial)
                roadSprite.material = _instanceMaterial;
                
            roadSprite.color = DESELECTED;
        }
        
        private void OnDestroy()
        {
            if (_instanceMaterial != null)
            {
                Destroy(_instanceMaterial);
            }
        }
        
        #region Pointer Events
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!SelectedUnitComponent.Instance) return;
            if (!SelectedUnitComponent.Instance.UnitTypeSelected) return;
            
            GameplayController.Instance.SetHighlightRoads(true, this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!SelectedUnitComponent.Instance) return;
            if (!SelectedUnitComponent.Instance.UnitTypeSelected) return;
            
            GameplayController.Instance.SetHighlightRoads(false, this);
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!SelectedUnitComponent.Instance) return;
            if (!SelectedUnitComponent.Instance.UnitTypeSelected) return;
            
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                OnClick();
            }
        }

        private void OnClick()
        {
            GameplayController.Instance.TryCreateUnit(this);
        }

        #endregion
    }
}