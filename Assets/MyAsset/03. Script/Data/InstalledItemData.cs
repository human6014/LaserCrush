using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Data
{
    [CreateAssetMenu(fileName = "Scriptable Data", menuName = "Scriptable/Installed Item Data", order = int.MaxValue)]
    public class InstalledItemData : ScriptableObject
    {
        [SerializeField] private float[] m_FontSizes = { 3.25f, 2.75f, 2.25f };
        [SerializeField] private float[] m_ChargingEnergy = { 0.3f, 0.4f, 0.7f, 1.0f };
        [SerializeField] private int[] m_MaxUsingNum = { 3, 3, 3, 2 };
        [SerializeField] private float m_ChargingTime = 0;
        [SerializeField] private Color m_TextStartColor;
        [SerializeField] private Color m_TextEndColor;

        public float[] FontSize { get => m_FontSizes; }
        public float[] ChargingEnergy { get => m_ChargingEnergy; }
        public int[] MaxUsingNum { get => m_MaxUsingNum; }

        public float ChargingTime { get => m_ChargingTime; }

        public ref Color TextStartColor => ref m_TextStartColor;
        public ref Color TextEndColor => ref m_TextEndColor;
    }
}
