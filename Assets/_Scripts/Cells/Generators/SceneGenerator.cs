using UnityEngine;

namespace _Scripts
{
    public class SceneGenerator : MonoBehaviour
    {
        void Start()
        {
            MapGenerator.Instance.Generate();
            BuildingsGenerator.Instance.GenerateTowers(7,5, 1);
            RoadGenerator.Instance.GenerateRoads();
            TreesGenerator.Instance.Generate();
        }
    }
}