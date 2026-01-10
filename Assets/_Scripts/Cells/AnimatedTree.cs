using UnityEngine;

namespace _Scripts
{
    public class AnimatedTree : MonoBehaviour
    {
        private void Start()
        {
            var animator = GetComponent<Animator>();
            
            if (animator != null)
            {
                float randomTime = Random.Range(0f, 1f);
                animator.Play(0, 0, randomTime);
            }
        }
    }
}