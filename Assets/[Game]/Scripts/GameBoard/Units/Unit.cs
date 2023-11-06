using System.Collections.Generic;
using _Game_.Scripts.GameBoard.Grid;
using _Game_.Scripts.GameBoard.Interface;
using _Game_.Scripts.Information;
using _Game_.Scripts.Manager;
using _Game_.Scripts.SO;
using _Game_.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace _Game_.Scripts.GameBoard.Units
{
    public abstract class Unit : MonoBehaviour, IDamageable, ISelectable
    {
        [SerializeField] private ProductInformation information;
        [SerializeField] private GameObject orderLine;
        [SerializeField] private FloatVariable health;
        [SerializeField] private Image hpBar;
        public bool IsSelect { get; set; }
        public List<GridCell> occupiedCells = new List<GridCell>();
        public string poolTag;
        public float Health { get; set; }
        public bool IsDead { get; set; }

        public int width;
        public int height;
        protected virtual void OnEnable()
        {
            Init();
            EventManager.Instance.SelectBuilding += SetSelected;
        }

        protected virtual void OnDisable()
        {
            EventManager.Instance.SelectBuilding -= SetSelected;
        }
    

        public virtual void Init()
        {
            Health=health.value;
            IsDead = false;
            HpBar();
        }

        public virtual void Selected()
        {
            SelectBuilding();
            ProductInformationUI.Instance.UpdateProductInformation(information.productName,information.productImage,information.productionsImage);
        }

        protected void SelectBuilding()
        {
            EventManager.Instance.SelectBuilding?.Invoke(false);
            SetSelected(true);
        }
        protected void SetSelected(bool isSelect)
        {
            IsSelect = isSelect;
            orderLine.SetActive(isSelect);
        }
        public virtual void OccupyCell(GridCell cell)
        {
            occupiedCells.Add(cell);
            cell.SetCellStatus(true);
        }

        public void LeaveCell()
        {
            for (int i = 0; i < occupiedCells.Count; i++)
            {
                occupiedCells[i].SetCellStatus(false);
            }
        }
        public void TakeDamage(float damage,out float health)
        {
            Health -= damage;
            health = Health;
            HpBar();
            if (Health <= 0 && !IsDead)
            {
                Death();
            }
        }

        public void Death()
        {
            IsDead = true;
            GenericObjectPool.Instance.ReleasePooledObject(poolTag,this);
            LeaveCell();
        }

        public void HpBar()
        {
            hpBar.fillAmount = Health/health.value;
        }

    }
}
