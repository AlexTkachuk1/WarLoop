using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.UI
{
    public class ExitButton : MonoBehaviour
    {
        [SerializeField] Button _button;

        private void Awake()
        {
            _button.onClick.AddListener(Application.Quit);
        }
    }
}