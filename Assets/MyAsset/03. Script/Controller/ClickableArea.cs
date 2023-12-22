using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Laser.Manager;

namespace Laser.Controller.InputObject
{
    public class ClickableArea : MonoBehaviour
    {
        [SerializeField] private InputManager m_InputManager;
        
        private void OnMouseDrag() 
            => m_InputManager.RepaintLineAction?.Invoke();
        
    }
}
