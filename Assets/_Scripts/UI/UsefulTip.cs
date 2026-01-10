using TMPro;
using UnityEngine;

namespace _Scripts.UI
{
    public class UsefulTip: Singleton<UsefulTip>
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private string[] tips;
        
        public void ShowTip()
        {
            text.text = GetRandomTip();
        }
        
        private string GetRandomTip()
        {
            int randomIndex = Random.Range(0, tips.Length);
            return tips[randomIndex];
        }
    }
}