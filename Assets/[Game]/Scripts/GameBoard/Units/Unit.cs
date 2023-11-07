using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using _Game_.Scripts.GameBoard.Grid;
using _Game_.Scripts.GameBoard.Interface;
using _Game_.Scripts.Information;
using _Game_.Scripts.Manager;
using _Game_.Scripts.SO;
using _Game_.Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Task = System.Threading.Tasks.Task;

namespace _Game_.Scripts.GameBoard.Units
{
    public abstract class Unit : MonoBehaviour, IDamageable, ISelectable
    {
        [SerializeField] private ProductInformation information;
        [SerializeField] private GameObject orderLine;
        [SerializeField] private FloatVariable health;
        [SerializeField] private TMP_Text hpText;
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


        protected virtual void Init()
        {
            occupiedCells.Clear();
            Health=health.value;
            IsDead = false;
            HpBarEnable(false);
        }

        public virtual void Selected()
        {
            SelectBuilding();
            ProductInformationUI.Instance.UpdateProductInformation(information.productName,information.productImage,information.productionsImage);
        }

        protected void SelectBuilding()
        {
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

        private Task LeaveCell()
        {
            for (int i = 0; i < occupiedCells.Count; i++)
            {
                occupiedCells[i].SetCellStatus(false);
            }

            return Task.CompletedTask;
        }
        public void TakeDamage(float damage,out float health)
        {
            Health -= damage;
            health = Health;
            if (Health <= 0 && !IsDead)
            {
                Death();
            }
        }

        public async void Death()
        {
            IsDead = true;
            await LeaveCell();
            GenericObjectPool.Instance.ReleasePooledObject(poolTag,this);
        }

        public void UpdateHpBar()
        {
            HpBarEnable(true);
            hpText.text = Health.ToString(CultureInfo.InvariantCulture);
        }

        public void HpBarEnable(bool isActive)
        {
            hpText.gameObject.SetActive(isActive);
        }

    }
}
