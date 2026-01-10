using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class Tooltip : StaticInstance<Tooltip>
    {
        [SerializeField] private TMP_Text _text;

        private readonly List<TooltipInstancer> _pointers = new();
        
        public void UpdatePointer(TooltipInstancer instancer, bool state)
        {
            if (state)
            {
                if (!_pointers.Contains(instancer))
                    _pointers.Add(instancer);
            }
            else
                _pointers.Remove(instancer);
            
            if (this)
                gameObject.SetActive(_pointers.Count > 0);
        }

        public void SetText(string text)
            => _text.text = text;

        protected override void Awake()
        {
            base.Awake();
            UpdatePointer(null, false);
        }
    }
}