using System;
using _Scripts.Actors.Units;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class SelectUnitButton : MonoBehaviour
    {
        [SerializeField] private UnitType unitType;
        [SerializeField] private Button button;

        private void Start()
        {
            button.onClick.AddListener(OnClick);
        }
        
        private void OnDestroy()
        {
            button.onClick.RemoveListener(OnClick);
        }
        
        private void OnClick()
        {
            SelectedUnitComponent.Instance.SetSelectedUnitType(unitType);
        }
    }
}