#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace _Scripts.Helpers
{
    public static class SpriteTimelineBaker
    {
        // Ищем кривую для SpriteRenderer.m_Sprite на любом path (или можно ограничить)
        public static bool TryExtractSpriteKeys(AnimationClip clip, out List<SpriteAnimations.Key> keys, out string foundPath)
        {
            keys = new List<SpriteAnimations.Key>();
            foundPath = null;

            if (clip == null) return false;

            var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            EditorCurveBinding? binding = null;

            foreach (var b in bindings)
            {
                if (b.type == typeof(SpriteRenderer) && b.propertyName == "m_Sprite")
                {
                    binding = b;
                    foundPath = b.path;
                    break;
                }
            }

            if (binding == null)
                return false;

            var kfs = AnimationUtility.GetObjectReferenceCurve(clip, binding.Value);
            if (kfs == null || kfs.Length == 0)
                return false;

            foreach (var k in kfs)
            {
                keys.Add(new SpriteAnimations.Key
                {
                    time = k.time,
                    sprite = k.value as Sprite
                });
            }

            keys.Sort((a, b) => a.time.CompareTo(b.time));
            return true;
        }

        [MenuItem("Tools/WarLoop/Bake Sprite Timelines From Selected Clips")]
        public static void BakeSelected()
        {
            var selected = Selection.objects;
            if (selected == null || selected.Length == 0)
            {
                Debug.LogError("Выдели AnimationClip(ы) в Project.");
                return;
            }

            int baked = 0;

            foreach (var obj in selected)
            {
                if (obj is not AnimationClip clip) 
                    continue;

                if (!TryExtractSpriteKeys(clip, out var keys, out var path))
                {
                    Debug.LogWarning($"[{clip.name}] Нет SpriteRenderer.m_Sprite кривой (или пусто).");
                    continue;
                }

                var asset = ScriptableObject.CreateInstance<SpriteAnimations>();
                asset.sourceClip = clip;
                asset.length = clip.length;
                asset.frameRate = clip.frameRate;
                asset.keys = keys.ToArray();

                // кладём рядом с клипом
                var clipPath = AssetDatabase.GetAssetPath(clip);
                var dir = System.IO.Path.GetDirectoryName(clipPath);
                var outPath = AssetDatabase.GenerateUniqueAssetPath($"{dir}/{clip.name}_Timeline.asset");

                AssetDatabase.CreateAsset(asset, outPath);
                baked++;

                Debug.Log($"Baked {clip.name} -> {outPath} (keys={asset.keys.Length}, bindPath='{path}')");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Done. Baked: {baked}");
        }

        [MenuItem("Tools/WarLoop/Dump Selected Clip Object Bindings")]
        public static void DumpBindings()
        {
            if (Selection.activeObject is not AnimationClip clip)
            {
                Debug.LogError("Выдели один AnimationClip.");
                return;
            }

            var bindings = AnimationUtility.GetObjectReferenceCurveBindings(clip);
            Debug.Log($"[{clip.name}] ObjectRef bindings: {bindings.Length}");
            foreach (var b in bindings)
                Debug.Log($"{b.path} | {b.type.Name} | {b.propertyName}");
        }
    }
}
#endif
