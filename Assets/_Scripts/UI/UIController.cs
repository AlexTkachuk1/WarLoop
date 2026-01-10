using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class UIController : Singleton<UIController>
    {
        [SerializeField] private GameObject towerWindow;
        [SerializeField] private Button button;
        [SerializeField] private Animator animator;
        
        private bool _windowIsHiden = false;

        public void OpenTowerWindow()
        {
            towerWindow.SetActive(true);
            
            button.onClick.AddListener(OnClick);
        }
        
        public void CloseTowerWindow()
        {
            towerWindow.SetActive(false);
            button.onClick.RemoveListener(OnClick);
        }
        
        private void OnClick()
        {
            animator.Play(_windowIsHiden ? "HideWindow" : "ShowWindow");

            _windowIsHiden = !_windowIsHiden;
        }
    }
}