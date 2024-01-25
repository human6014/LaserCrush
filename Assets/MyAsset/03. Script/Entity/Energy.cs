using UnityEngine;
using LaserCrush.Manager;
using System;


namespace LaserCrush.Entity
{
    public class Energy : MonoBehaviour
    {
        #region Variable
        [SerializeField] private UIManager m_UIManager;

        private static event Action s_MaxEnergyUpdate;
        private static event Action s_CurrentEnergyUpdate;

        private static int s_InitEnergy = 2000;
        private static int s_MaxEnergy;
        private static int s_CurrentEnergy;
        private static int s_HittingFloorLaserNum;
        private static int s_HittingWallLaserNum;
        #endregion

        private static int MaxEnergy
        {
            get => s_MaxEnergy;
            set
            {
                s_MaxEnergy = value;
                s_MaxEnergyUpdate?.Invoke();
            }
        }

        private static int CurrentEnergy
        {
            get => s_CurrentEnergy;
            set
            {
                s_CurrentEnergy = value;
                s_CurrentEnergyUpdate?.Invoke();
            }
        }

        public void Init(int initEnergy)
        {
            MaxEnergy = initEnergy;
            CurrentEnergy = initEnergy;

            s_MaxEnergyUpdate = null;
            s_CurrentEnergyUpdate = null;

            s_MaxEnergyUpdate += () => m_UIManager.SetCurrentMaxEnergy(CurrentEnergy, MaxEnergy);
            s_CurrentEnergyUpdate += () => m_UIManager.SetCurrentEnergy(CurrentEnergy, MaxEnergy);

            s_MaxEnergyUpdate?.Invoke();
            s_CurrentEnergyUpdate?.Invoke();
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
            if (energy <= CurrentEnergy) CurrentEnergy -= energy;
            else
            {
                energy = CurrentEnergy;
                CurrentEnergy = 0;
            }
            return energy;
        }

        public static bool CheckEnergy()
            => CurrentEnergy > 0;

        public static void CollideWithFloor()
        {
            s_HittingFloorLaserNum++;
            //UseEnergy(m_MaxEnergy / 5);
            //만약 바닥에 닿으면 꾸준히 대미지 주고 싶으면 윗 코드 주석하면 됨
        }

        public static void DeCollideWithFloor()
            => s_HittingFloorLaserNum--;

        public static int ChargeEnergy()
        {
            CurrentEnergy = s_MaxEnergy;
            s_HittingFloorLaserNum = 0;
            s_HittingWallLaserNum = 0;
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

            s_HittingWallLaserNum++;
            if (s_HittingWallLaserNum > 10) { UseEnergy(MaxEnergy / 10); }
            GameManager.s_ValidHit++;
        }

        public static void EnergyUpgrade(int additionalEnergy)
            => MaxEnergy += additionalEnergy;

        public static int GetEnergy()
            => s_CurrentEnergy;

        public static int GetHittingFloorLaserNum()
            => s_HittingFloorLaserNum;

        public static void SaveAllData()
        {
            DataManager.GameData.m_Energy = MaxEnergy;
        }

        public static void ResetGame()
        {
            s_MaxEnergy = s_InitEnergy;
            s_CurrentEnergy = s_InitEnergy;
        }

        private void OnDestroy()
        {
            s_MaxEnergyUpdate = null;
            s_CurrentEnergyUpdate = null;
        }
    }
}