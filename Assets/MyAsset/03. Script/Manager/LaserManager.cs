using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Entity;
using System;


namespace LaserCrush.Manager
{
    /// <summary>
    /// 레이저 메이저는 레이저를 하나하나 스스로 관리하지 않고 활성화 비활성화(사라지는 단계)만 결정해서 넘겨준다.
    /// </summary>
    [Serializable]
    public class LaserManager
    {
        #region Property
        [SerializeField] private Laser m_InitLazer;
        [SerializeField] private LineRenderer m_SubLine;
        [SerializeField] private SubLineController m_SubLineController;
        [SerializeField] private GameObject m_LaserObject;
        //lazer저장하는 자료구조
        private List<Laser> m_Lasers = new List<Laser>();
        private List<Laser> m_LaserAddBuffer = new List<Laser>();
        private List<Laser> m_LaserRemoveBuffer = new List<Laser>();

        //지우기 시작할 레이저보관 자료구조

        private List<Laser> m_RootLazer = new List<Laser>();

        private bool m_Initialized = false;
        #endregion

        private event Func<GameObject, GameObject> m_InstantiateFunc;

        public void Init(Func<GameObject, GameObject> instantiateFunc)
        {
            m_Lasers = new List<Laser>();
            m_LaserAddBuffer = new List<Laser>();
            m_LaserRemoveBuffer = new List<Laser>();
            m_RootLazer = new List<Laser>();

            m_InstantiateFunc = instantiateFunc;
        }

        public void Activate()
        {
            /*
             * 턴 시작
             * 1. 루트 배열 비우기
             * 2. 시작 레이저 셋팅 후 루트배열과 레이저 배열에 추가
             * 3. 초기화변수 셋팅
             */
            if (!m_Initialized)//턴 시작
            {
                Debug.Log("턴 시작");
                m_RootLazer.Clear();

                m_InitLazer.Init(m_SubLineController.Position, m_SubLineController.Direction, CreateLaser);
                
                m_RootLazer.Add(m_InitLazer);
                m_Lasers.Add(m_InitLazer);
                m_Initialized = true;
            }
            /*
             * 1. 레이저 순회하며 모든 레이저 진행 
             */
            else
            {
                for (int i = 0; i < m_Lasers.Count; i++)
                {
                    if(Energy.CheckEnergy()) // 에너지 없으면 호출의 의미가 없다 -> 최적화?
                    {
                        Debug.Log("레이저 업데이트" + i);
                        m_Lasers[i].ManagedUpdate();
                    }
                }
                ActivateBufferFlush();
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
                Debug.Log("DeActivate Comp");
                m_Initialized = false;
                return true;
            }
            /*
             * 1 루트배열에서 하나씩 꺼낸다
             * 2 꺼낸 레이저의 시작점을 끝점으로 이동시킨다.
             * 3 끝점과 시작점이 만나면 레이저를 루트배열에서 삭제하고 자식레이저를 배열에 추가
             * 4 위 과정을 레이저 배열이 빌때까지 진행한다.
             */
            Debug.Log("m_RootLazer : " + m_RootLazer.Count);
            for (int i = 0; i < m_RootLazer.Count; i++)
            {
                if (m_RootLazer[i].Erase())
                {
                    m_LaserRemoveBuffer.Add(m_RootLazer[i]);
                    foreach(var child in m_RootLazer[i].GetChildLazer())
                    {
                        m_LaserAddBuffer.Add(child);
                    }
                }
            }
            DeActivateBufferFlush();
            return false;
        }

        private void ActivateBufferFlush()
        {
            //Debug.Log("레이저 추가");
            for (int i = 0; i < m_LaserAddBuffer.Count; i++) 
            {
                m_Lasers.Add(m_LaserAddBuffer[i]);
                Debug.Log("레이저 추가");
            }
            m_LaserAddBuffer.Clear();
        }

        private void DeActivateBufferFlush()
        {
            for (int i = 0; i < m_LaserRemoveBuffer.Count; i++)
            {
                m_RootLazer.Remove(m_LaserRemoveBuffer[i]);
            }
            for (int i = 0; i < m_LaserAddBuffer.Count; i++)
            {
                m_RootLazer.Add(m_LaserAddBuffer[i]);
            }
            m_LaserAddBuffer.Clear();
            m_LaserRemoveBuffer.Clear();
        }

        public List<Laser> CreateLaser(List<Vector2> dirVector, Vector2 pos)
        {
            List<Laser> answer = new List<Laser>();

            for (int i = 0; i < dirVector.Count; i++)
            {
                Laser laser = m_InstantiateFunc?.Invoke(m_LaserObject).GetComponent<Laser>();
                laser.transform.position = pos;
                laser.Init(pos + dirVector[i], dirVector[i], CreateLaser);
                m_LaserAddBuffer.Add(laser);
                answer.Add(laser);
            }
            return answer;
        }
    }
}