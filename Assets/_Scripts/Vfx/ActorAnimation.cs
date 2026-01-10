using _Scripts.Actors;
using _Scripts.Helpers;
using UnityEngine;

namespace _Scripts.Vfx
{
    public class ActorAnimation : MonoBehaviour
    {
        [SerializeField] private SpriteAnimations _run;
        [SerializeField] private SpriteAnimations _idle;
        [SerializeField] private SpriteAnimations _attack;
        [SerializeField] private SpriteRenderer _renderer;

        private float _time;
        private Sprite _last;
        private SpriteAnimations _currentAnimations;

        public void Animate(ProcessFrameResult result)
        {
            switch (result)
            {
                case ProcessFrameResult.Idle:
                    _currentAnimations = _idle;
                    break;
                case ProcessFrameResult.Attacking:
                    _currentAnimations = _attack;
                    break;
                case ProcessFrameResult.Running:
                    _currentAnimations = _run;
                    break;
            }
        }
        
        private void Update()
        {
            _time += Time.deltaTime;

            if (!_currentAnimations.TryGetSprite(_time, true, out var sprite)) return;
            if (sprite == _last) return;
            
            _renderer.sprite = sprite;
            _last = sprite;
        }

    }
}