using System.Linq;
using _Scripts.Helpers;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Scripts.Vfx
{
    public class Death : MonoBehaviour
    {
        [SerializeField] private AnimationCurve _alpha;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private SpriteAnimations _animation;
        
        private float _lifeTime;
        private float _speed;
        private Sprite _last;

        private void Awake()
        {
            _speed = _animation.length / _alpha.keys.Last().time;
            transform.localScale = Vector3.one * Random.Range(0.8f, 1.2f);
        }

        private void Update()
        {
            if (_lifeTime > _alpha.keys.Last().time)
                Destroy(gameObject);
            
            _lifeTime += Time.deltaTime;
            _renderer.color = new Color(_renderer.color.r,  _renderer.color.g, _renderer.color.b, _alpha.Evaluate(_lifeTime));

            if (!_animation.TryGetSprite(_lifeTime * _speed, false, out var sprite)) return;
            if (sprite == _last) return;
            
            _renderer.sprite = sprite;
            _last = sprite;
        }
    }
}