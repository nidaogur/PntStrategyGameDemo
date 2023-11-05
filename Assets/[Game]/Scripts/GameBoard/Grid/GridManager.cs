using System.Collections.Generic;
using _Game_.Scripts.GameBoard.Units;
using _Game_.Scripts.Utilities;
using UnityEngine;

namespace _Game_.Scripts.GameBoard.Grid
{
    public class GridManager : MonoSingleton<GridManager>
    {
        public int rows = 10;
        public int cols = 10;
        public GameObject gridCellPrefab;
        public float cellSize = 1f;
        public UnitController unitController;

        void Start()
        {
            CreateGrid();
        }

        void CreateGrid()
        {
       
            unitController.gridCells = new GridCell[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    GameObject cellGO = Instantiate(gridCellPrefab,
                        new Vector3(i * cellSize , j * cellSize , 0), Quaternion.identity, transform);
                    GridCell cell = cellGO.GetComponent<GridCell>();

                    cell.position = new Vector3(i, j, 0);
                    cell.SetCellStatus(false);

                    unitController.gridCells[i, j] = cell;
                    unitController.gridCells[i, j].gridX = i;
                    unitController.gridCells[i, j].gridY = j;
                }
            }
        }
        
        public GridCell GetCellFromPosition(Vector3 position)
        {
            int x = Mathf.RoundToInt(position.x / cellSize);
            int y = Mathf.RoundToInt(position.y / cellSize);
            x = Mathf.Clamp(x, 0, rows - 1);
            y = Mathf.Clamp(y, 0, cols - 1);
            return unitController.gridCells[x, y];
        }

        private List<GridCell> GetNeighboringCells(GridCell cell)
        {
            List<GridCell> neighbors = new List<GridCell>();

            int[] xOffset = { 0, 0, 1, -1 };
            int[] yOffset = { 1, -1, 0, 0 };

            for (int i = 0; i < 4; i++)
            {
                int checkX = cell.gridX + xOffset[i];
                int checkY = cell.gridY + yOffset[i];

                if (checkX >= 0 && checkX < rows && checkY >= 0 && checkY < cols)
                {
                    neighbors.Add(unitController.gridCells[checkX, checkY]);
                }
            }

            return neighbors;
        }

        private List<GridCell> RetracePath(GridCell startCell, GridCell endCell)
        {
            List<GridCell> path = new List<GridCell>();
            GridCell currentCell = endCell;

            while (currentCell != startCell)
            {
                path.Add(currentCell);
                currentCell = currentCell.parent;
            }

            path.Reverse();
            return path;
        }

        private int GetDistance(GridCell cellA, GridCell cellB)
        {
            int distX = Mathf.Abs(cellA.gridX - cellB.gridX);
            int distY = Mathf.Abs(cellA.gridY - cellB.gridY);

            return distX + distY;
        }

        public List<GridCell> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            GridCell startCell = GetCellFromPosition(startPos);
            GridCell targetCell = GetCellFromPosition(targetPos);

            List<GridCell> openSet = new List<GridCell>();
            HashSet<GridCell> closedSet = new HashSet<GridCell>();

            openSet.Add(startCell);

            while (openSet.Count > 0)
            {
                GridCell currentCell = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentCell.FCost ||
                        openSet[i].FCost == currentCell.FCost && openSet[i].hCost < currentCell.hCost)
                    {
                        currentCell = openSet[i];
                    }
                }

                openSet.Remove(currentCell);
                closedSet.Add(currentCell);

                if (currentCell == targetCell)
                {
                    return RetracePath(startCell, targetCell);
                }

                foreach (GridCell neighbor in GetNeighboringCells(currentCell))
                {
                    if (neighbor.isOccupied || closedSet.Contains(neighbor))
                    {
                        continue;
                    }

                    int newCostToNeighbor = currentCell.gCost + GetDistance(currentCell, neighbor);
                    if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                    {
                        neighbor.gCost = newCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, targetCell);
                        neighbor.parent = currentCell;

                        if (!openSet.Contains(neighbor))
                        {
                            openSet.Add(neighbor);
                        }
                    }
                }
            }

            return null; // Path not found
        }

        public List<GridCell> GetAllSurroundingCells(GridCell centerCell)
        {
            List<GridCell> surroundingCells = new List<GridCell>();

            if (centerCell != null)
            {
                int centerX = centerCell.gridX;
                int centerY = centerCell.gridY;

                for (int i = centerX - 1; i <= centerX + 1; i++)
                {
                    for (int j = centerY - 1; j <= centerY + 1; j++)
                    {
                        if (i >= 0 && i < rows && j >= 0 && j < cols && (i != centerX || j != centerY))
                        {
                            surroundingCells.Add(unitController.gridCells[i, j]);
                        }
                    }
                }
            }

            return surroundingCells;
        }
    }
}