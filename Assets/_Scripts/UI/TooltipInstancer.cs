using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.UI
{
    public class TooltipInstancer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField, Multiline] private string _text;
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            Tooltip.Instance.SetText(_text);
            Tooltip.Instance.UpdatePointer(this, true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Tooltip.Instance.UpdatePointer(this, false);
        }

        private void OnDestroy()
        {
            Tooltip.Instance.UpdatePointer(this, false);
        }
    }
}