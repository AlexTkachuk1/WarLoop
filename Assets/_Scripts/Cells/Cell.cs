using UnityEngine;

namespace _Scripts
{
    public class Cell : MonoBehaviour
    {
        [SerializeField] private CellType _type;

        private int _x;
        private int _y;

        public int X => _x;
        public int Y => _y;
        public CellType Type => _type;

        public void Init(int x, int y)
        {
            _x = x;
            _y = y;

            transform.position = new Vector3(x, y, 0);
        }
    }
}