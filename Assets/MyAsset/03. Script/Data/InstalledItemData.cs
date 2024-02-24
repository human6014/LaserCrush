using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LaserCrush.Data
{
    [CreateAssetMenu(fileName = "Scriptable Data", menuName = "Scriptable/Installed Item Data", order = int.MaxValue)]
    public class InstalledItemData : ScriptableObject
    {
        [SerializeField] private int[] m_MaxUsingNum = { 1, 1, 1, 1 };
        [SerializeField] private float m_ChargingTime = 0;
        [SerializeField] private int m_DiscreteUnit = 5;

        public int[] MaxUsingNum { get => m_MaxUsingNum; }

        public float ChargingTime { get => m_ChargingTime; }

        public int DiscreteUnit { get => m_DiscreteUnit; }
    }
}
