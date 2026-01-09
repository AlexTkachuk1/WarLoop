using UnityEngine;

namespace _Scripts.UI
{
    public class UIController : Singleton<UIController>
    {
        [SerializeField] private GameObject towerWindow;

        public void OpenTowerWindow()
        {
            towerWindow.SetActive(true);
        }
        
        public void CloseTowerWindow()
        {
            towerWindow.SetActive(false);       
        }
    }
}