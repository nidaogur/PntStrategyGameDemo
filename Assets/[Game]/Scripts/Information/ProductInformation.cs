using System.Collections.Generic;
using UnityEngine;

namespace _Game_.Scripts.Information
{
    [CreateAssetMenu]
    public class ProductInformation : ScriptableObject
    {
        public string productName;
        public Sprite productImage;
        public List<Sprite> productionsImage;
    }
}