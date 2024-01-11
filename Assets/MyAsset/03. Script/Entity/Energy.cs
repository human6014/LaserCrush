using UnityEngine;
using UnityEngine.Events;
using LaserCrush.Manager;
using System;

namespace LaserCrush.Entity
{
    public class Energy : MonoBehaviour
    {
        #region Variable
        [SerializeField] private UIManager m_UIManager;
        [SerializeField] private int m_InitEnergy = 10000;

        private static event Action m_MaxEnergyUpdate;
        private static event Action m_CurrentEnergyUpdate;

        private static int m_MaxEnergy;
        private static int m_CurrentEnergy;
        private static int m_HittingFloorLaserNum;
        #endregion

        private static int MaxEnergy
        {
            get => m_MaxEnergy;
            set
            {
                m_MaxEnergy = value;
                m_MaxEnergyUpdate?.Invoke();
            }
        }

        private static int CurrentEnergy
        {
            get => m_CurrentEnergy;
            set
            {
                m_CurrentEnergy = value;
                m_CurrentEnergyUpdate?.Invoke();
            }
        }

        private void Awake()
        {
            m_MaxEnergyUpdate = null;
            m_CurrentEnergyUpdate = null;

            MaxEnergy = m_InitEnergy;
            CurrentEnergy = m_InitEnergy;

            m_MaxEnergyUpdate += () => m_UIManager.SetCurrentMaxEnergy(CurrentEnergy, MaxEnergy);
            m_CurrentEnergyUpdate += () => m_UIManager.SetCurrentEnergy(CurrentEnergy, MaxEnergy);
        }

        /// 반환형은 총 사용한 에너지의 양이다.
        /// 적게남
        /// </summary>
        /// <param name="energy">
        /// 사용할 에너지
        /// </param>
        /// <returns></returns>
        public static int UseEnergy(int energy)
        {
            if (energy <= CurrentEnergy)
            {
                CurrentEnergy -= energy;
            }
            else
            {
                energy = CurrentEnergy;
                CurrentEnergy = 0;
            }
            return energy;
        }

        public static bool CheckEnergy()
        {
            return CurrentEnergy > 0;
        }

        /// <summary>
        /// 일단 부딪힐 마다 10퍼 삭제
        /// </summary>
        public static void CollideWithWall()
        {
            UseEnergy(MaxEnergy / 10);
        }

        public static void CollideWithFloor()
        {
            UseEnergy(MaxEnergy / 5);
            //만약 바닥에 닿으면 꾸준히 대미지 주고 싶으면 윗 코드 주석하면 됨
        }

        public static int ChargeEnergy()
        {
            CurrentEnergy = MaxEnergy;
            m_HittingFloorLaserNum = 0;
            return CurrentEnergy;
        }

        public static void EnergyUpgrade(int additionalEnergy)
        {
            MaxEnergy += additionalEnergy;
        }

        public static int GetEnergy()
        {
            return m_CurrentEnergy;
        }

        //-95 ~ 95
        //게이지 표시 경우 -95 + GetGaugeNum으로 해줘야되더라
        //Fill Green
        private int GetGaugeNum()
        {
            return (190 * m_CurrentEnergy) / m_MaxEnergy;
        }

        private void OnDestroy()
        {
            m_MaxEnergyUpdate = null;
            m_CurrentEnergyUpdate = null;
        }

        public void Reset()
        {
            MaxEnergy = m_InitEnergy;
            m_CurrentEnergy = m_InitEnergy;
        }
    }
}
