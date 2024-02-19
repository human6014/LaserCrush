using UnityEngine;
using System;
using LaserCrush.Manager;

namespace LaserCrush.Entity
{
    public class Energy : MonoBehaviour
    {
        #region Variable
        [SerializeField] private UIManager m_UIManager;

        private static event Action s_CurrentTimeUpdateAction;
        private static event Action s_MaxEnergyHighlightTextAction;
        private static event Action s_CurrentDamageUpdateAction;

        private static int s_HittingFloorLaserNum;

        private static readonly int s_InitDamage = 5;
        private static int s_CurrentDamage;
        private static int s_AdditionalStack;

        private static float s_CurrentTime;
        private static float s_CurrentMaxTime;
        private static readonly float s_MaxTime = 3;
        #endregion

        #region Property
        public static float CurrentTime
        {
            get => s_CurrentTime;
            private set
            {
                s_CurrentTime = value;
                s_CurrentTimeUpdateAction?.Invoke();
            }
        }

        public static int CurrentDamage
        {
            get => s_CurrentDamage;
            private set
            {
                s_CurrentDamage = value;
                s_CurrentDamageUpdateAction?.Invoke();
            }
        }
        #endregion

        #region Init
        public void Init(int initEnergy)
        {
            s_CurrentTime = 0;
            s_CurrentMaxTime = s_MaxTime;

            CurrentDamage = initEnergy;
            s_AdditionalStack = 0;

            s_CurrentTimeUpdateAction = () => m_UIManager.SetCurrentTime((int)((s_CurrentMaxTime - s_CurrentTime) * 100), (int)(s_CurrentMaxTime * 100));
            s_MaxEnergyHighlightTextAction = () => m_UIManager.PlayEnergyHighlight();
            s_CurrentDamageUpdateAction = () => m_UIManager.PlayDamageHighlight(s_CurrentDamage);

            s_CurrentTimeUpdateAction?.Invoke();
            s_CurrentDamageUpdateAction?.Invoke();
        }
        #endregion

        public static void SetTurnEnd()
            => CurrentTime = s_CurrentMaxTime;
        
        public static bool IsValidTime()
            => CurrentTime < s_CurrentMaxTime;
        
        public static void DeCollideWithFloor()
            => s_HittingFloorLaserNum--;

        public static void ChargeEnergy()
        {
            CurrentTime = 0;
            s_CurrentMaxTime = s_MaxTime;

            s_MaxEnergyHighlightTextAction?.Invoke();

            s_HittingFloorLaserNum = 0;
        }

        public static void CollideWithWall()
            => GameManager.ValidHit++;
            
        private void Update()
        {
            if (IsValidTime() && GameManager.s_GameStateType == GameManager.EGameStateType.LaserActivating)
            {
                s_CurrentTime += Time.deltaTime;
                s_CurrentTimeUpdateAction?.Invoke();
            }
        }

        public static void ChargeCurrentMaxTime(float time)
        {
            s_CurrentMaxTime += time;
            s_CurrentTimeUpdateAction?.Invoke();
        }

        public static void UpgradeDamage()
        {
            CurrentDamage += 1;
            s_AdditionalStack += 1;

            CurrentDamage += s_AdditionalStack / 2;
            s_AdditionalStack %= 2;
        }

        public static int GetHittingFloorLaserNum()
            => s_HittingFloorLaserNum;

        public static void SaveAllData()
            => DataManager.GameData.m_Damage = CurrentDamage;

        public static void ResetGame()
        {
            CurrentDamage = s_InitDamage;
            s_AdditionalStack = 0;

            s_CurrentTime = 0;
            s_CurrentMaxTime = s_MaxTime;
        }

        private void OnDestroy()
        {
            s_CurrentTimeUpdateAction = null;
            s_MaxEnergyHighlightTextAction = null;
        }
    }
}