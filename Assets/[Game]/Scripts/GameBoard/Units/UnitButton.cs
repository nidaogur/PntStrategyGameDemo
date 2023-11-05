using _Game_.Scripts.Information;
using _Game_.Scripts.Manager;
using UnityEngine;

namespace _Game_.Scripts.GameBoard.Units
{
    public class UnitButton : MonoBehaviour
    {
        [SerializeField] private Unit unit;
    
        public void SelectBuilding()
        {
            ProductInformationUI.Instance.SetProductionPanel(false);
            EventManager.Instance.SelectBuilding?.Invoke(false);
            UnitController.Instance.SelectBuilding(unit);
        }
    }
}