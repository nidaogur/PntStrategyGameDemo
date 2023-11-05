using System;
using System.Collections;
using System.Collections.Generic;
using _Game_.Scripts.GameBoard.Grid;
using _Game_.Scripts.GameBoard.Interface;
using _Game_.Scripts.Information;
using _Game_.Scripts.Manager;
using _Game_.Scripts.SO;
using DG.Tweening;
using UnityEngine;

namespace _Game_.Scripts.GameBoard.Units
{
    public class SoldierUnit : Unit, IAttack
    {
        [SerializeField] private FloatVariable damage;
        public float Damage { get; set; }
        private GridCell _currentCell;
        private readonly WaitForSeconds _moveDuration = new WaitForSeconds(0.1f);
        private readonly WaitForSeconds _attackDuration = new WaitForSeconds(0.3f);
        private Coroutine _attackCoroutine;

        protected override void OnEnable()
        {
            base.OnEnable();
            InputManager.Instance.OnRightClick += MoveToPosition;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            InputManager.Instance.OnRightClick -= MoveToPosition;
        }

        public override void Init()
        {
            base.Init();
            Damage = damage.value;
        }

        public override void Selected()
        {
            SelectBuilding();
            ProductInformationUI.Instance.SetProductionPanel(false);
        }

        private void MoveToPosition(Vector3 targetPosition)
        {
            if (!IsSelect) return;
            if (_attackCoroutine != null)
                StopCoroutine(_attackCoroutine);

            _currentCell = GridManager.Instance.GetCellFromPosition(transform.position);
            GridCell targetCell = GridManager.Instance.GetCellFromPosition(targetPosition);

            if (ReferenceEquals(targetCell, _currentCell)) return;

            if (targetCell == null || targetCell.isOccupied)
            {
                Vector3 closestEmptyPoint = FindClosestEmptyPointToTarget(targetCell);

                if (closestEmptyPoint != Vector3.zero)
                {
                    MoveToPositionAndAttack(closestEmptyPoint, targetPosition);
                }
            }
            else
            {
                var path = GridManager.Instance.FindPath(_currentCell.position, targetPosition);
                if (path is { Count: > 0 })
                {
                    StartCoroutine(MoveAlongPath(path, () => OccupyCell(_currentCell)));
                }
            }
        }

        private void MoveToPositionAndAttack(Vector3 emptyPoint, Vector3 targetPosition)
        {
            var pathToEmpty = GridManager.Instance.FindPath(_currentCell.position, emptyPoint);
            if (pathToEmpty is { Count: > 0 })
            {
                StartCoroutine(MoveAlongPath(pathToEmpty, () =>
                {
                    OccupyCell(_currentCell);
                    AttackTargetAtRay(targetPosition);
                }));
            }
        }


        private IEnumerator MoveAlongPath(List<GridCell> path, Action onComplete = null)
        {
            SetSelected(false);
            _currentCell.SetCellStatus(false);

            foreach (var cell in path)
            {
                yield return _moveDuration;
                _currentCell = cell;
                transform.position = _currentCell.position;
            }

            yield return _moveDuration;
            onComplete?.Invoke();
        }

        private Vector3 FindClosestEmptyPointToTarget(GridCell targetCell)
        {
            HashSet<GridCell> visitedCells = new HashSet<GridCell>();
            Queue<GridCell> cellsToCheck = new Queue<GridCell>();
            cellsToCheck.Enqueue(targetCell);

            while (cellsToCheck.Count > 0)
            {
                GridCell currentCell = cellsToCheck.Dequeue();
                visitedCells.Add(currentCell);

                List<GridCell> surroundingCells = GridManager.Instance.GetAllSurroundingCells(currentCell);
            
                foreach (GridCell cell in surroundingCells)
                {
                    if (visitedCells.Contains(cell)) continue;
                    if (!cell.isOccupied)
                    {
                        return cell.position; 
                    }

                    cellsToCheck.Enqueue(cell);
                }
            }

            return Vector3.zero;
        }

        private void AttackTargetAtRay(Vector2 pos)
        {
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);
            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;
                Unit hitBase = hitObject.GetComponent<Unit>();
                if (hitBase != null)
                {
                    _attackCoroutine = StartCoroutine(AttackLoop(hitBase));
                }
            }
        }

        IEnumerator AttackLoop(Unit hitBase)
        {
            float remainingHealth = 0;
            do
            {
                AttackAnimation();
                hitBase.TakeDamage(Damage, out remainingHealth);
                yield return _attackDuration;
            } while (remainingHealth > 0 && hitBase != null);
        }

        public override void OccupyCell(GridCell cell)
        {
            occupiedCells.Clear();
            occupiedCells.Add(cell);
            cell.SetCellStatus(true);
        }

        public void AttackAnimation()
        {
            DOTween.Complete(GetInstanceID());
            transform.DOPunchScale(Vector3.one * 0.5f, 0.2f, 1, 0.1f).SetLink(gameObject).SetId(GetInstanceID());
        }
    }
}