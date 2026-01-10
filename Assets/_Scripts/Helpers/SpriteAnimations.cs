using System;
using UnityEngine;

namespace _Scripts.Helpers
{
    public class SpriteAnimations : ScriptableObject
    {
        [Serializable]
        public struct Key
        {
            public float time;
            public Sprite sprite;
        }

        public AnimationClip sourceClip;
        public float length;
        public float frameRate;
        public Key[] keys;

        public bool TryGetSprite(float time, bool loop, out Sprite sprite)
        {
            sprite = null;
            if (keys == null || keys.Length == 0 || length <= 0f)
                return false;

            if (loop)
            {
                time %= length;
                if (time < 0f) time += length;
            }
            else
            {
                if (time >= length) time = length - 0.0001f;
                if (time < 0f) time = 0f;
            }

            // keys отсортированы по time
            int lo = 0, hi = keys.Length - 1;
            while (lo <= hi)
            {
                int mid = (lo + hi) >> 1;
                if (keys[mid].time <= time) lo = mid + 1;
                else hi = mid - 1;
            }

            int idx = Mathf.Clamp(hi, 0, keys.Length - 1);
            sprite = keys[idx].sprite;
            return sprite != null;
        }

    }
}