using System.Collections.Generic;
using _Game_.Scripts.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game_.Scripts.Information
{
    public class ProductInformationUI : MonoSingleton<ProductInformationUI>
    { 
        [SerializeField] private GameObject informationPanel;
        [SerializeField] private TMP_Text productNameText;
        [SerializeField] private Image productImage;
        [SerializeField] private List<Image> productionImage;
        [SerializeField] private GameObject productionInfo;
        public void UpdateProductInformation(string productName, Sprite productSprite, List<Sprite> productionInfoSprite)
        {
            SetProductionPanel(true);
            SetProductName(productName);
            SetProductImage(productSprite);
            UpdateProductionInfo(productionInfoSprite);
        }

        public void SetProductionPanel(bool isActive)
        {
            informationPanel.SetActive(isActive);
        }
        private void SetProductName(string newName)
        {
            productNameText.text = newName;
        }

        private void SetProductImage(Sprite newSprite)
        {
            productImage.sprite = newSprite;
        }

        private void UpdateProductionInfo(List<Sprite> newProductionInfo)
        {
            if (newProductionInfo.Count>0)
            {
                for (int i = 0; i < productionImage.Count; i++)
                {
                    SetProductionInfo(productionImage[i], newProductionInfo[i]);
                }
            }
            else
            {
                productionInfo.SetActive(false);
            }
        }

        private void SetProductionInfo(Image productionInfoImage, Sprite newSprite)
        {
            productionInfo.SetActive(true);
            productionInfoImage.sprite = newSprite;
        }
    
    
    }
}
