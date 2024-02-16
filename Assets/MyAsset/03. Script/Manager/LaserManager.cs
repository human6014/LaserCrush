using System.Collections.Generic;
using System;
using UnityEngine;
using LaserCrush.Entity;

namespace LaserCrush.Manager
{
    /// <summary>
    /// 레이저 메이저는 레이저를 하나하나 스스로 관리하지 않고 활성화 비활성화(사라지는 단계)만 결정해서 넘겨준다.
    /// </summary>
    [Serializable]
    public class LaserManager
    {
        #region Variable
        [SerializeField] private Laser m_InitLazer;
        [SerializeField] private Laser m_LaserObject;
        [SerializeField] private Transform m_LasersTransform;
        [SerializeField] private int m_PoolCount = 20;

        //lazer저장하는 자료구조
        private List<Laser> m_Lasers;
        private List<Laser> m_LaserAddBuffer;
        private List<Laser> m_LaserRemoveBuffer;

        //지우기 시작할 레이저보관 자료구조
        private List<Laser> m_RootLazer;

        //중간에 부모를 잃은 레이저 지우는 자료구조
        private List<Laser> m_LossParentsLaser;
        private List<Laser> m_LossParentsLaserAddBuffer;
        private List<Laser> m_LossParentsLaserRemoveBuffer;

        private ObjectPoolManager.PoolingObject m_LaserPool;

        private const string m_LaunchAudioKey = "Launch";
        private const float m_LaunchDelayTime = 0.25f;

        private float m_CurrentLaunchTime;

        private bool m_IsPlayLaunchAudio;
        private bool m_Activated = false;
        #endregion


        public void Init()
        {
            m_Lasers = new List<Laser>();
            m_LaserAddBuffer = new List<Laser>();
            m_LaserRemoveBuffer = new List<Laser>();
            m_RootLazer = new List<Laser>();
            m_LossParentsLaser = new List<Laser>();
            m_LossParentsLaserAddBuffer = new List<Laser>();
            m_LossParentsLaserRemoveBuffer = new List<Laser>();

            m_LaserPool = ObjectPoolManager.Register(m_LaserObject, m_LasersTransform);
            m_LaserPool.GenerateObj(m_PoolCount);
            m_InitLazer.Init(CreateLaser, LossParent);
        }

        public void Activate(Vector3 pos, Vector3 dir)
        {
            if (!m_Activated)//턴 첫 시작
            {
                m_CurrentLaunchTime += Time.deltaTime;
                if (m_CurrentLaunchTime <= m_LaunchDelayTime)
                {
                    if (!m_IsPlayLaunchAudio)
                    {
                        AudioManager.AudioManagerInstance.PlayOneShotNormalSE(m_LaunchAudioKey);
                        m_IsPlayLaunchAudio = true;
                    }
                    else return;
                }
                else
                {
                    m_InitLazer.Activate(pos, dir, 0);

                    m_RootLazer.Add(m_InitLazer);
                    m_Lasers.Add(m_InitLazer);

                    m_CurrentLaunchTime = 0;
                    m_IsPlayLaunchAudio = false;
                    m_Activated = true;
                }
            }
            else
            {
                for (int i = 0; i < m_Lasers.Count; i++)
                {
                    if (Energy.IsValidTime())
                    {
                        m_Lasers[i].Run();
                    }
                }
                ActivateBufferFlush();

                //중간에 부모를 잃은 레이저 처리하는 함수
                RemoveLossParentsLaser();

                if (CheckCollideWithFloor())
                {
                    Energy.SetTurnEnd();
                }
            }
        }

        /// <summary>
        /// 모든 레이저 삭제시 true 반환
        /// true 반환 시 다시 레이저 발시 시 initalized로 시작하기위해 m_Initialized를 false로 설정 
        /// </summary>
        public bool DeActivate()
        {
            if (m_Lasers.Count == 0)
            {
                PoolableMonoBehaviour[] childs = m_LasersTransform.GetComponentsInChildren<PoolableMonoBehaviour>();
                for(int i = 0; i < childs.Length; i++)
                    m_LaserPool.ReturnObject(childs[i]);

                m_Activated = false;
                //AudioManager.AudioManagerInstance.StopNormalSE("Laser");
                return true;
            }
            /*
             * 1 루트배열에서 하나씩 꺼낸다
             * 2 꺼낸 레이저의 시작점을 끝점으로 이동시킨다.
             * 3 끝점과 시작점이 만나면 레이저를 루트배열에서 삭제하고 자식레이저를 배열에 추가
             * 4 위 과정을 레이저 배열이 빌때까지 진행한다.
             */
            for (int i = 0; i < m_RootLazer.Count; i++)
            {
                if (m_RootLazer[i].Erase())
                {
                    m_LaserRemoveBuffer.Add(m_RootLazer[i]);
                    foreach (Laser child in m_RootLazer[i].GetChildLazer())
                    {
                        m_LaserAddBuffer.Add(child);
                    }
                }
            }
            DeActivateBufferFlush();

            //중간에 부모를 잃은 레이저 처리하는 함수
            RemoveLossParentsLaser();
            return false;
        }

        public void RemoveLossParentsLaser()
        {
            if (m_LossParentsLaser.Count == 0)
                return;

            for (int i = 0; i < m_LossParentsLaser.Count; i++)
            {
                if (m_LossParentsLaser[i].Erase())
                {
                    m_LossParentsLaserRemoveBuffer.Add(m_LossParentsLaser[i]);
                    foreach (var child in m_LossParentsLaser[i].GetChildLazer())
                    {
                        m_LossParentsLaserAddBuffer.Add(child);
                    }
                }
            }
            LossParentsLaserBufferFlush();
        }

        public void LossParent(List<Laser> lasers)
        {
            for (int i = 0; i < lasers.Count; i++)
            {
                m_LossParentsLaser.Add(lasers[i]);
                lasers[i].LossParentLasersDeActivate();
            }
        }

        private void ActivateBufferFlush()
        {
            for (int i = 0; i < m_LaserAddBuffer.Count; i++)
            {
                m_Lasers.Add(m_LaserAddBuffer[i]);
            }
            m_LaserAddBuffer.Clear();
        }

        private void DeActivateBufferFlush()
        {
            for (int i = 0; i < m_LaserRemoveBuffer.Count; i++)
            {
                m_RootLazer.Remove(m_LaserRemoveBuffer[i]);
                m_Lasers.Remove(m_LaserRemoveBuffer[i]);
            }
            for (int i = 0; i < m_LaserAddBuffer.Count; i++)
            {
                m_RootLazer.Add(m_LaserAddBuffer[i]);
            }
            m_LaserAddBuffer.Clear();
            m_LaserRemoveBuffer.Clear();
        }

        private void LossParentsLaserBufferFlush()
        {
            for (int i = 0; i < m_LossParentsLaserRemoveBuffer.Count; i++)
            {
                m_LossParentsLaser.Remove(m_LossParentsLaserRemoveBuffer[i]);
                m_Lasers.Remove(m_LossParentsLaserRemoveBuffer[i]);
            }
            for (int i = 0; i < m_LossParentsLaserAddBuffer.Count; i++)
            {
                m_LossParentsLaser.Add(m_LossParentsLaserAddBuffer[i]);
            }
            m_LossParentsLaserAddBuffer.Clear();
            m_LossParentsLaserRemoveBuffer.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="infoList"></param>
        /// <param name="hierarchy">부모가 해당 함수를 호출 시 자식의 계층을 계산 후 넘긴다.</param>
        /// <returns></returns>
        public List<Laser> CreateLaser(List<LaserInfo> infoList, int hierarchy)
        {
            List<Laser> answer = new List<Laser>();

            Laser laser;
            for (int i = 0; i < infoList.Count; i++)
            {
                laser = (Laser)m_LaserPool.GetObject(true);
                laser.transform.position = infoList[i].Position;
                laser.Init(CreateLaser, LossParent);
                laser.Activate(infoList[i].Position, infoList[i].Direction, hierarchy);
                
                m_LaserAddBuffer.Add(laser);
                answer.Add(laser);
            }
            return answer;
        }

        public void ResetGame()
        {
            DestroyLasers(m_Lasers);
            DestroyLasers(m_LaserAddBuffer);
            DestroyLasers(m_LaserRemoveBuffer);
            DestroyLasers(m_RootLazer);
            DestroyLasers(m_LossParentsLaser);
            DestroyLasers(m_LossParentsLaserAddBuffer);
            DestroyLasers(m_LossParentsLaserRemoveBuffer);
            m_Activated = false;
            m_CurrentLaunchTime = 0;
            m_IsPlayLaunchAudio = false;
        }

        private void DestroyLasers(List<Laser> lasers)
        {
            for (int i = 0; i < lasers.Count; i++)
            {
                if (lasers[i] != m_InitLazer)
                    m_LaserPool.ReturnObject(lasers[i]);
                else
                    lasers[i].ResetLaser();
            }
            lasers.Clear();
        }

        /// <summary>
        /// </summary>
        /// <returns> true : 충돌가능한 모든 레이저가 바닥과 충돌 중</returns>
        private bool CheckCollideWithFloor()
        {
            int count = 0;
            for (int i = 0; i < m_Lasers.Count; i++)
            {
                if (m_Lasers[i].GetELaserStateType() == ELaserStateType.Move || m_Lasers[i].GetELaserStateType() == ELaserStateType.Wait) { return false; }
                if (m_Lasers[i].IsHittingDamagable()) count++;
            }
            if (Energy.GetHittingFloorLaserNum() == count && count != 0) { return true; }
            return false;
        }
    }
}