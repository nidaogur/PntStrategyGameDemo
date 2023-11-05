using _Game_.Scripts.GameBoard.Grid;
using _Game_.Scripts.Production;
using _Game_.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Game_.Scripts.GameBoard.Units
{
    public class UnitController : MonoSingleton<UnitController>
    {
        private Unit _unitToPlace;
        public CustomCursor customCursor;
        public GridCell[,] gridCells;

        private void Update()
        {
            GridCell gridCell = FindNearestCell(Input.mousePosition);

            if (gridCell != null)
            {
                HighlightCells(gridCell);

                if (Input.GetMouseButtonDown(0) && _unitToPlace != null)
                {
                    TryPlaceBuilding(gridCell);
                }
            }
        }

        private GridCell FindNearestCell(Vector2 mousePosition) {
            GridCell nearestCell = null;
            float nearestDistance = float.MaxValue;

            foreach (GridCell cell in gridCells) {
                float dist = Vector2.Distance(cell.transform.position, Camera.main.ScreenToWorldPoint(mousePosition));
        
                if (dist < nearestDistance) {
                    nearestDistance = dist;
                    nearestCell = cell;
                }
            }

            return nearestCell;
        }
        private void TryPlaceBuilding(GridCell gridCell)
        {
            if (CanPlaceBuilding(gridCell, _unitToPlace.width, _unitToPlace.height))
            {
                var temp=InstantiateBuilding(gridCell);
                OccupyCells(temp,gridCell, _unitToPlace.width, _unitToPlace.height);
            }
        }
        private bool CanPlaceBuilding(GridCell startCell, int width, int height)
        {
            int startX = startCell.gridX;
            int startY = startCell.gridY;

            for (int i = startX; i < startX + width; i++)
            {
                for (int j = startY; j < startY + height; j++)
                {
                    if (i >= gridCells.GetLength(0) || j >= gridCells.GetLength(1) || gridCells[i, j].isOccupied)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        private void OccupyCells(Unit unit, GridCell startCell, int width, int height)
        {
            _unitToPlace = null;
            int startX = startCell.gridX;
            int startY = startCell.gridY;

            for (int i = startX; i < startX + width; i++)
            {
                for (int j = startY; j < startY + height; j++)
                {
                    unit.OccupyCell(gridCells[i, j]);
                }
            }
        }

        private Unit InstantiateBuilding(GridCell gridCell)
        {
            if (_unitToPlace == null) return null;
            var temp=GenericObjectPool.Instance.Spawn<Unit>(_unitToPlace.poolTag, gridCell.transform.position, Quaternion.identity);
            customCursor.gameObject.SetActive(false);
            Cursor.visible = true;
            return temp;
        }

        public void SelectBuilding(Unit unit)
        {
            customCursor.gameObject.SetActive(true);
            customCursor.GetComponent<Image>().sprite = unit.GetComponentInChildren<SpriteRenderer>().sprite;
            Cursor.visible = false;
            _unitToPlace = unit;
        }
    
        private void HighlightCells(GridCell startCell)
        {
            if (_unitToPlace == null)
                return;

            ClearHighlightedCells();

            int startX = startCell.gridX;
            int startY = startCell.gridY;

            bool canPlaceBuilding = CanPlaceBuilding(startCell, _unitToPlace.width, _unitToPlace.height);

            for (int i = startX; i < startX + _unitToPlace.width; i++)
            {
                for (int j = startY; j < startY + _unitToPlace.height; j++)
                {
                    if (i < gridCells.GetLength(0) && j < gridCells.GetLength(1))
                    {
                        gridCells[i, j].ChangeColor(canPlaceBuilding ? Color.grey : Color.red);
                    }
                }
            }
        }

        private void ClearHighlightedCells()
        {
            foreach (GridCell cell in gridCells)
            {
                cell.ChangeColor(Color.white);
            }
        }
    }
}
