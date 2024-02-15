using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Data
{
    [CreateAssetMenu(fileName = "Scriptable Data", menuName = "Scriptable/Installed Item Data", order = int.MaxValue)]
    public class InstalledItemData : ScriptableObject
    {
        [SerializeField] private float[] m_FontSizes = { 3.25f, 2.75f, 2.25f };
        [SerializeField] private int[] m_MaxUsingNum = { 1, 1, 1, 1 };
        [SerializeField] private float m_ChargingTime = 0;
        [SerializeField] private Color m_TextStartColor;
        [SerializeField] private Color m_TextEndColor;

        public float[] FontSize { get => m_FontSizes; }

        public int[] MaxUsingNum { get => m_MaxUsingNum; }

        public float ChargingTime { get => m_ChargingTime; }

        public ref Color TextStartColor => ref m_TextStartColor;

        public ref Color TextEndColor => ref m_TextEndColor;
    }
}
