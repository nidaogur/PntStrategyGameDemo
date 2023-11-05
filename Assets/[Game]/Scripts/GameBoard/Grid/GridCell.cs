using UnityEngine;

namespace _Game_.Scripts.GameBoard.Grid
{
    [RequireComponent(typeof(Collider2D))]
    public class GridCell : MonoBehaviour
    {
        public bool isOccupied;
        public Vector3 position;

        public int gridX;
        public int gridY;

        public int gCost; 
        public int hCost; 
        public int FCost => gCost + hCost;
        public GridCell parent; // The lowest-cost neighboring cell

        private Collider2D _collider2D;
        private Collider2D Collider2D
        {
            get
            {
                if (_collider2D == null)
                    _collider2D = GetComponent<Collider2D>();
                return _collider2D;
            }
        }
        private SpriteRenderer _spriteRenderer;
        private SpriteRenderer SpriteRenderer
        {
            get
            {
                if (_spriteRenderer == null)
                    _spriteRenderer = GetComponent<SpriteRenderer>();
                return _spriteRenderer;
            }
        }
        private void Start()
        {
            gCost = int.MaxValue;
            hCost = int.MaxValue;

        }

        public void ChangeColor(Color color)
        {
            SpriteRenderer.color = color;
        }
        public void SetCellStatus(bool status)
        {
            isOccupied = status;
            ChangeColor(Color.white);
            Collider2D.enabled = !status;

        }
    
    }
}
