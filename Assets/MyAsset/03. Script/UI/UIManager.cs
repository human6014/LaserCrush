using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserCrush.UI;

namespace LaserCrush.Manager
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private ToolbarController m_ToolbarController;

        public void Init(ItemManager itemManager)
        {
            m_ToolbarController.Init(itemManager);
        }
    }
}
