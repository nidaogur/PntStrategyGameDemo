using System;
using _Game_.Scripts.Utilities;

namespace _Game_.Scripts.Manager
{
   public class EventManager : MonoSingleton<EventManager>
   {
      public Action<bool> SelectBuilding;
   
   }
}
