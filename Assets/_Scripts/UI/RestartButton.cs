using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class RestartButton : MonoBehaviour
    {
        [SerializeField] Button _button;

        private void Awake()
        {
            _button.onClick.AddListener(() =>
            {
                var currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
                SceneManager.LoadScene(currentSceneIndex);
            });
        }
    }
}