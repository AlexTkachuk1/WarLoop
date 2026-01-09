using _Scripts.Actors.Units;

namespace _Scripts
{
    [System.Serializable]
    public struct CellPrefabEntry
    {
        public CellType type;
        public Cell prefab;
    }
    
    [System.Serializable]
    public struct UnitPrefabEntry
    {
        public UnitType type;
        public Unit prefab;
        public int cost;
    }
}