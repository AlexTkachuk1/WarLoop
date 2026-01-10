using UnityEngine;
using System.Collections.Generic;

namespace _Scripts
{
    public class WaterGenerator : StaticInstance<WaterGenerator>
    {
        [Header("Factory")]
        [SerializeField] private CellFactory cellFactory;

        [Header("Parents")]
        [SerializeField] private Transform cellsParent;
        
        [Header("Water Settings")]
        [SerializeField] private int waterThickness = 12;
        
        private readonly List<Cell> _waterCells = new();
        
        public void Generate()
        {
            Clear();
            GenerateWaterIsland();
        }
        
        private void GenerateWaterIsland()
        {
            MapGenerator mapGen = MapGenerator.Instance;
            
            int mapWidth = mapGen.Width;
            int mapHeight = mapGen.Height;
            
            int wallY = -1;

            for (int x = 0; x < waterThickness; x++)
            {
                for (int y = -1 - waterThickness; y < mapHeight + waterThickness; y++)
                {
                    var newX = -x; 
                    
                    CreateWaterCell(newX, y);
                }
            }
            
            for (int x = 0; x < waterThickness; x++)
            {
                for (int y = -1 - waterThickness; y < mapHeight + waterThickness; y++)
                {
                    var newX = mapWidth + x - 1; 
                    
                    CreateWaterCell(newX, y);
                }
            }
            
            for (int y = 0; y < waterThickness; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    var newY = -1 - y; 
                    
                    CreateWaterCell(x, newY);
                }
            }
            
            for (int y = 0; y < waterThickness; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    var newY = mapWidth + y - 3; 
                    
                    CreateWaterCell(x, newY);
                }
            }
        }
        
        private void CreateWaterCell(int x, int y)
        {
            Cell waterCell = cellFactory.Create(CellType.Water, x, y, cellsParent);
            _waterCells.Add(waterCell);
        }
        
        private void Clear()
        {
            foreach (var cell in _waterCells)
                if (cell) Destroy(cell.gameObject);

            _waterCells.Clear();
        }
    }
}