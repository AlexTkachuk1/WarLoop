using _Scripts.Models;
using UnityEngine;

namespace _Scripts.GameLoop
{
    public class Bootstrap : StaticInstance<Bootstrap>
    {
        [field: SerializeField] public GameData GameData { get; private set; }
    }
}