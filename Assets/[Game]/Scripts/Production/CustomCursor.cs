using UnityEngine;

namespace _Game_.Scripts.Production
{
   public class CustomCursor : MonoBehaviour
   {
      private void Update()
      {
         transform.position = Input.mousePosition;
      }
   }
}
