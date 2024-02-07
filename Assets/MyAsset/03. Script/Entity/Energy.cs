using UnityEngine;
using LaserCrush.Manager;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Collections;
using static UnityEditor.Progress;

namespace LaserCrush.Entity
{
    public class Energy : MonoBehaviour
    {
        #region Variable
        [SerializeField] private UIManager m_UIManager;

        private static event Action s_MaxEnergyUpdateAction;
        private static event Action s_CurrentEnergyUpdateAction;
        private static event Action s_MaxEnergyHighlightTextAction;

        private static readonly int s_InitEnergy = 500;
        private static int s_MaxEnergy;
        private static int s_CurrentEnergy;
        private static int s_HittingFloorLaserNum;
        private static int s_HittingWallLaserNum;
        private static int m_Damage;
        private static int s_DamageStack;
        private static HashSet<int> s_LaserHashSet;
        #endregion

        public static int MaxEnergy
        {
            get => s_MaxEnergy;
            private set
            {
                if (s_MaxEnergy < value) s_MaxEnergyHighlightTextAction?.Invoke();

                s_MaxEnergy = value;
                s_MaxEnergyUpdateAction?.Invoke();
            }
        }

        private static int CurrentEnergy
        {
            get => s_CurrentEnergy;
            set
            {
                s_CurrentEnergy = value;
                s_CurrentEnergyUpdateAction?.Invoke();
            }
        }

        public void Init(int initEnergy)
        {
            MaxEnergy = 500;
            CurrentEnergy = 500;
            s_DamageStack = 0;

            s_MaxEnergyUpdateAction = () => m_UIManager.SetCurrentMaxEnergy(CurrentEnergy, MaxEnergy);
            s_CurrentEnergyUpdateAction = () => m_UIManager.SetCurrentEnergy(CurrentEnergy, MaxEnergy);
            s_MaxEnergyHighlightTextAction = () => m_UIManager.PlayEnergyHighlight();

            s_MaxEnergyUpdateAction?.Invoke();
            s_CurrentEnergyUpdateAction?.Invoke();
            s_LaserHashSet = new HashSet<int>();
            m_Damage = 6;
            StartCoroutine(Tic());
        }

        public static void UseAllEnergy()
        {
            CurrentEnergy = 0;
        }
        public static int UseEnergy()
        {
            return m_Damage;
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

            s_LaserHashSet.Clear();
            return CurrentEnergy;
        }

        public static void ChargeEnergy(int energy)
            => CurrentEnergy += energy;
        
        public static void CollideWithWall()
        {
            s_HittingWallLaserNum++;
            if (s_HittingWallLaserNum > 10) 
                UseEnergy(); 

            GameManager.ValidHit++;
        }

        IEnumerator Tic()
        {
            while(true)
            {
                if (CurrentEnergy > 0 && GameManager.GetEGameStateType() == GameManager.EGameStateType.LaserActivating)
                    CurrentEnergy -= 100;
                yield return new WaitForSecondsRealtime(1.0f);
            }
        }

        public static void EnergyUpgrade(int additionalEnergy)
            => m_Damage += additionalEnergy;

        public static void EnergyUpgrade()
        {
            s_DamageStack++;
            m_Damage += (s_DamageStack / 3);
        }


        public static int GetEnergy()
            => s_CurrentEnergy;

        public static int GetHittingFloorLaserNum()
            => s_HittingFloorLaserNum;

        public static void SaveAllData()
            => DataManager.GameData.m_Energy = MaxEnergy;
        
        public static void ResetGame()
        {
            s_MaxEnergy = s_InitEnergy;
            s_CurrentEnergy = s_InitEnergy;
        }

        private void OnDestroy()
        {
            s_MaxEnergyUpdateAction = null;
            s_CurrentEnergyUpdateAction = null;
            s_MaxEnergyHighlightTextAction = null;
        }
    }
}