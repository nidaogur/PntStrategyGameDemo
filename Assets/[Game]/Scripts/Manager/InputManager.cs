using System;
using _Game_.Scripts.GameBoard.Interface;
using _Game_.Scripts.Utilities;
using UnityEngine;

namespace _Game_.Scripts.Manager
{
    public class InputManager: MonoSingleton<InputManager>
    {
        public Action<Vector3> OnRightClick;
        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 mousePosition = Input.mousePosition;
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, 0));
                OnRightClick?.Invoke(worldPosition);
            }
            if (Input.GetMouseButtonDown(0))
            {
              RaycastFromMousePosition();
            }
        }
        private void RaycastFromMousePosition()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if(hit.collider==null) return;
            if (hit.collider.TryGetComponent(out ISelectable selectable ))
            {
                hit.collider.GetComponent<ISelectable>().Selected();
            }
        }
    }
    
}