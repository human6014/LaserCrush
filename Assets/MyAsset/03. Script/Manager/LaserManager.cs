using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Entity;
using System;
using LaserCrush.Controller.InputObject;

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
        [SerializeField] private LineRenderer m_SubLine;
        [SerializeField] private SubLineController m_SubLineController;
        [SerializeField] private GameObject m_LaserObject;
        [SerializeField] private Transform m_LasersTransform;

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

        private bool m_Initialized = false;
        #endregion

        private event Func<GameObject, GameObject> m_InstantiateFunc;
        private event Action<GameObject> m_DestroyAction;

        public void Init(Func<GameObject, GameObject> instantiateFunc, Action<GameObject> destroyAction)
        {

            m_Lasers = new List<Laser>();
            m_LaserAddBuffer = new List<Laser>();
            m_LaserRemoveBuffer = new List<Laser>();
            m_RootLazer = new List<Laser>();
            m_LossParentsLaser = new List<Laser>();
            m_LossParentsLaserAddBuffer = new List<Laser>();
            m_LossParentsLaserRemoveBuffer = new List<Laser>();


            m_InstantiateFunc = instantiateFunc;
            m_DestroyAction = destroyAction;
            m_InitLazer.Init(CreateLaser, LossParent);
        }

        public void Activate()
        {
            if (!m_Initialized)//턴 첫 시작
            {
                m_InitLazer.Activate(m_SubLineController.Position, m_SubLineController.Direction);

                m_RootLazer.Add(m_InitLazer);
                m_Lasers.Add(m_InitLazer);

                m_Initialized = true;
            }
            else
            {
                for (int i = 0; i < m_Lasers.Count; i++)
                {
                    if (Energy.CheckEnergy())
                    {
                        m_Lasers[i].Run();
                    }
                }
                ActivateBufferFlush();

                //중간에 부모를 잃은 레이저 처리하는 함수
                RemoveLossParentsLaser();

                if (CheckCollideWithFloor())
                {
                    Energy.UseEnergy(int.MaxValue);
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
                foreach (Transform tr in m_LasersTransform)
                {
                    m_DestroyAction?.Invoke(tr.gameObject);
                }
                m_Initialized = false;
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
                    foreach (var child in m_RootLazer[i].GetChildLazer())
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
            {
                return;
            }

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

        public List<Laser> CreateLaser(List<LaserInfo> dirVector)
        {
            List<Laser> answer = new List<Laser>();

            for (int i = 0; i < dirVector.Count; i++)
            {
                Laser laser = m_InstantiateFunc?.Invoke(m_LaserObject).GetComponent<Laser>();
                laser.transform.SetParent(m_LasersTransform);

                laser.transform.position = dirVector[i].Position;
                laser.Init(CreateLaser, LossParent);
                laser.Activate(dirVector[i].Position + dirVector[i].Direction, dirVector[i].Direction);
                m_LaserAddBuffer.Add(laser);
                answer.Add(laser);
            }
            return answer;
        }

        public void Reset()
        {
            m_Lasers.Clear();
            m_LaserAddBuffer.Clear();
            m_LaserRemoveBuffer.Clear();
            m_RootLazer.Clear();
            m_LossParentsLaser.Clear();
            m_LossParentsLaserAddBuffer.Clear();
            m_LossParentsLaserRemoveBuffer.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns> true : 충돌가능한 모든 레이저가 바닥과 충돌 중(?)</returns>
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