using _Scripts.Actors.Buildings;

namespace _Scripts.Helpers
{
    public static class BuildingUtils
    {
        public static bool TryGetBuildingOnCell(Cell cell, out Building building)
        {
            building = null;
            
            foreach (var tower in BuildingsGenerator.Instance.Towers)
            {
                if (tower.X == cell.X && tower.Y == cell.Y)
                {
                    building = tower.GetComponent<Building>();
                    
                    return true;
                }
            }
            
            return true;
        }
    }
}