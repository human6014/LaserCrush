using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using LaserCrush.UI.Receiver;

namespace LaserCrush.UI.Controller
{
    public class DonateController : MonoBehaviour
    {
        [SerializeField] private GameObject m_DonateCompPanel;
        [SerializeField] private ButtonReceiver m_ResumeButtonReceiver;

        public void Init()
        {
            m_ResumeButtonReceiver.ButtonClickAction += PopupDown;
        }

        public void OnPurchaseComplete(Product product)
        {
            Debug.Log($"Purchase complete - Product: '{product.definition.id}'");
            m_DonateCompPanel.SetActive(true);
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            Debug.Log($"Purchase failed - Product: '{product.definition.id}'," +
                $" Purchase failure reason: {failureDescription.reason}," +
                $" Purchase failure details: {failureDescription.message}");
        }

        public void OnProductFetched(Product product)
        {
            Debug.Log($"Purchase fetched - Product: '{product.definition.id}'");
        }

        private void PopupDown()
        {
            m_DonateCompPanel.SetActive(false);
        }
    }
}
