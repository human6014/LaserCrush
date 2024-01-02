using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LaserCrush.Manager;

namespace LaserCrush.Entity
{
    enum LaserStateType // 이름 고민
    {
        Move,
        Hitting,
    }

    /// <summary>
    /// 레이저 매니저가 매 업데이트마다 각 레이저 개체를 업데이트해줌
    /// 충돌관련 로직은 모두 레이저에서 처리 후 각 개체에 통보하는 방식
    ///     ex) 충돌 오브젝트 탐색 후 hp, 타입등 읽어온 후 로직 처리 후 각 개체에 통보
    /// </summary>
    public class Laser : MonoBehaviour
    {
        /// <summary>
        /// m_StartPoint : 레이저 시작점 -> 소멸시 시작점이 이동함
        /// m_EndPoint : 레이저 끝점 -> 발사 시 끝점이 이동함
        /// </summary>
        #region Property
        [SerializeField] private Data.LaserData m_LaserData;

        private List<Laser> m_ChildLazers;

        private ICollisionable m_Target = null; // 이부분도 고민 해봐야함
        private LineRenderer m_LineRenderer;
        private Func<List<Vector2>, Vector2, List<Laser>> m_LaserCreateFunc;
        private Action<List<Laser>> m_LaserEraseAction;

        private LaserStateType m_State;

        private Vector2 m_StartPoint;
        private Vector2 m_EndPoint;
        private Vector2 m_DirectionVector;

        private bool m_IsInitated;

        #endregion

        private void Awake()
        {
            m_ChildLazers = new List<Laser>();
            m_IsInitated = false;

            m_LineRenderer = GetComponent<LineRenderer>();
            if (m_LineRenderer is null) Debug.LogError("m_LineRenderer is Null");
        }

        public void Init(Func<List<Vector2>, Vector2, List<Laser>> laserCreateFunc, Action<List<Laser>> laserEraseAction)
        {
            m_LaserCreateFunc = laserCreateFunc;
            m_LaserEraseAction = laserEraseAction;
        }

        public void Activate(Vector2 position, Vector2 dir)
        {
            m_StartPoint = position;
            m_EndPoint = position;
            m_DirectionVector = dir.normalized;
            m_IsInitated = true;
            m_State = LaserStateType.Move;

            m_ChildLazers.Clear();
            m_LineRenderer.positionCount = 2;
            m_LineRenderer.SetPosition(0, position);
            m_LineRenderer.SetPosition(1, position);
        }

        /// <summary>
        /// 레이저 총 상태
        /// 이름 바꾸는게 어떨까?
        /// [움직임] : 충돌 전 상태 업데이트마다 방향벡터 방향으로 이동
        /// [충돌] : 최초 충돌에서 자식 레이저 생성, 주기마다 에너지 체크 후 충돌 블럭 고격
        ///          레이저의 생성(분기)는 움직임 상태에서 최초 충돌을 감지한 순간 수행된다.
        /// </summary>
        public void ManagedUpdate()
        {
            if (!m_IsInitated) { return; }
            switch (m_State) 
            {
                case LaserStateType.Move://에너지 소모x 이동만
                    Move();
                    break;
                case LaserStateType.Hitting:
                    Hiting();
                    break;
                default:
                    Debug.Log("잘못된 레이저 상태입니다.");
                    break;
            }
        }

        public bool HasChild()
        {
            if (m_ChildLazers.Count == 0)
            {
                return false;
            }
            return true;
        }

        public List<Laser> GetChildLazer()
        {
            return m_ChildLazers;
        }

        /// <summary>
        /// 일정 속도만큼 startPoint를 endPoint방향으로 이동
        /// 레이저가 지워지면 true 반환
        /// </summary>
        public bool Erase()
        {
            if (Vector2.Distance(m_StartPoint, m_EndPoint) <= m_LaserData.EraseVelocity)
            {
                m_StartPoint = m_EndPoint;
                if(m_LineRenderer != null) m_LineRenderer.SetPosition(0, m_StartPoint);
                return true;
            }
            m_StartPoint += m_DirectionVector * m_LaserData.EraseVelocity;
            m_LineRenderer.SetPosition(0, m_StartPoint);
            return false;
        }

        /// <summary>
        /// 레이저의 끝점을 움직이는 함수
        /// 1.  충돌 탐지
        /// 1.1 충돌한 개체를 받아온다
        /// 1.2 개체에 맞는 함수를 호출한다.
        /// </summary> 
        public void Move()
        {
            if(!Energy.CheckEnergy()) { return; }

            RaycastHit2D hit = Physics2D.Raycast(m_StartPoint, m_DirectionVector, Mathf.Infinity, LayerManager.s_LaserHitableLayer);

            if (hit.collider != null && Vector2.Distance(m_EndPoint, hit.point) <= m_LaserData.ShootingVelocity)
            {
                m_Target = hit.transform.GetComponent<ICollisionable>();
                List<Vector2> dir = m_Target.Hitted(hit, m_DirectionVector);
                m_State = LaserStateType.Hitting;
                AddChild(m_LaserCreateFunc?.Invoke(dir, hit.point));
                return;
            }
            m_EndPoint += m_DirectionVector * m_LaserData.ShootingVelocity;
            m_LineRenderer.SetPosition(1, m_EndPoint);
        }

        /// <summary>
        /// 작동 순서
        /// 1. 충돌 중인 블럭의 타입을 확인하고 공격 불가능인 경우 업데이트 종료
        /// 2. 공격이 가능할 경우 에너지 잔량을 확인 후 0이 아니면 공격 진행
        /// 3. 해당 블럭에 GetDamage함수를 호출해 데미지를 주고 Energy개체에 에너지를 감소시킨다.
        /// </summary>
        public void Hiting()
        {
            if (!m_Target.IsGetDamageable())
            {
                return;
            }

            if (Energy.CheckEnergy())//발사전 에너지 사용가능여부 확인
            {
                if(!m_Target.GetDamage(m_LaserData.Damage))
                {
                    m_LaserEraseAction?.Invoke(m_ChildLazers);
                    m_Target = null;
                    m_State = LaserStateType.Move;
                }
            }
        }

        private void AddChild(List<Laser> child)
        {
            for(int i = 0; i < child.Count; i++) 
            {
                m_ChildLazers.Add(child[i]);
            }
        }


        public void CollideLauncher(RaycastHit2D hit)
        {
            Vector2 dir = hit.collider.GetComponent<Launcher>().GetDirectionVector();
        }

        private void OnDestroy()
        {
            m_LaserCreateFunc = null;
            m_LaserEraseAction = null;
        }
    }
}
