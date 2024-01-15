using UnityEngine;
using UnityEngine.Events;
using TMPro;
using LaserCrush.Manager;
using System;


namespace LaserCrush.Entity
{
    public class Energy : MonoBehaviour
    {
        #region Variable
        [SerializeField] private UIManager m_UIManager;
        [SerializeField] private static int m_InitEnergy = 10000;
        private static event Action m_MaxEnergyUpdate;
        private static event Action m_CurrentEnergyUpdate;

        private static int m_MaxEnergy;
        private static int m_CurrentEnergy;
        private static int m_HittingFloorLaserNum;

        private static int m_HittingWallLaserNum;
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


        public static void CollideWithFloor()
        {
            m_HittingFloorLaserNum++;
            //UseEnergy(m_MaxEnergy / 5);
            //만약 바닥에 닿으면 꾸준히 대미지 주고 싶으면 윗 코드 주석하면 됨
        }

        public static void DeCollideWithFloor()
        {
            m_HittingFloorLaserNum--;
        }

        public static int ChargeEnergy()
        {
            CurrentEnergy = m_MaxEnergy;
            m_HittingFloorLaserNum = 0;
            m_HittingWallLaserNum = 0;
            return CurrentEnergy;
        }

        public static void CollideWithWall()
        {
            /*Case 벽에 튕기면 전체 10%감소 
             * 패널티가 너무 강력하고 튕기는 맛이 너무 적다 피드백 받음
             */
            //UseEnergy(MaxEnergy / 10);

            /*Case 벽에 튕기면 눈 속임으로 1 데미지 주면서 계속 진행
             * 바닥에 모든 레이저 가닥이 도착하는 경우가 아니면 계속 진행 가능
             * 단점 1. 가끔 어 왜 에너지가 닳지 하는 상황발생 -> 시작할때1 감소, 후로는 잘 안보임
             */
            //UseEnergy(1);

            /*Case 벽에 튕기면 가중치로 점점 데미지 증가
             */
            /*m_HittingWallLaserNum++;
            UseEnergy((MaxEnergy / 100) * (int)(m_HittingWallLaserNum * 1.5));*/

            /*Case 최초 N회까지 충돌은 무료 이후 충돌에 에너지 소모 적용
             */
            m_HittingWallLaserNum++;
            if (m_HittingWallLaserNum > 15) { UseEnergy(MaxEnergy / 10); }
        }


        public static void EnergyUpgrade(int additionalEnergy)
        {
            MaxEnergy += additionalEnergy;
        }

        public static int GetEnergy()
        {
            return m_CurrentEnergy;
        }

        public static int GetHittingFloorLaserNum()
        {
            return m_HittingFloorLaserNum;
        }
        private void OnDestroy()
        {
            m_MaxEnergyUpdate = null;
            m_CurrentEnergyUpdate = null;
        }

        public static void ResetGame()
        {
            m_MaxEnergy = m_InitEnergy;
            m_CurrentEnergy = m_InitEnergy;
        }

    }
}
