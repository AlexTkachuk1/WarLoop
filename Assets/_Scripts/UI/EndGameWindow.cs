using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class EndGameWindow : Singleton<EndGameWindow>
    {
        [SerializeField] private GameObject bg;
        [SerializeField] private GameObject tip;
        [SerializeField] private GameObject title;
        [SerializeField] private GameObject buttonText;
        
        [SerializeField] private TMP_Text text;
        [SerializeField] private string winTerm;
        [SerializeField] private string loseTerm;
        
        [SerializeField] private Button tryAganButton;

        private void Start()
        {
            bg.SetActive(false);
            tip.SetActive(false);
            title.SetActive(false);
            buttonText.SetActive(false);
            tryAganButton.gameObject.SetActive(false);
            
            tryAganButton.onClick.AddListener(ReloadCurrentScene);
        }

        private void OnDestroy()
        {
            tryAganButton.onClick.RemoveListener(ReloadCurrentScene);
        }

        public void ShowWinScreen()
        {
            bg.SetActive(true);
            tip.SetActive(true);
            title.SetActive(true);
            buttonText.SetActive(true);
            tryAganButton.gameObject.SetActive(true);
            
            UsefulTip.Instance.ShowTip();
            
            text.text = winTerm;
        }

        public void ShowLoseScreen()
        {
            bg.SetActive(true);
            tip.SetActive(true);
            title.SetActive(true);
            buttonText.SetActive(true);
            tryAganButton.gameObject.SetActive(true);
            
            UsefulTip.Instance.ShowTip();
            
            text.text = loseTerm;
        }
        
        public void ReloadCurrentScene()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }
}