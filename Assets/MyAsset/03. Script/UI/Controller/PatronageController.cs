using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using LaserCrush.UI.Receiver;

namespace LaserCrush.UI.Controller
{
    public class PatronageController : MonoBehaviour
    {

        public void Init()
        {

        }

        public void OnPurchaseComplete(Product product)
        {
            Debug.Log($"Purchase complete - Product: '{product.definition.id}'");
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
    }
}
