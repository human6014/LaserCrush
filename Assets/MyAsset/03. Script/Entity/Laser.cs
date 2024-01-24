using System.Collections.Generic;
using System;
using UnityEngine;
using LaserCrush.Manager;
using LaserCrush.Entity.Particle;

namespace LaserCrush.Entity
{
    public enum ELaserStateType // 이름 고민
    {
        Move,
        Hitting,
        Wait
    }

    /// <summary>
    /// 레이저 매니저가 매 업데이트마다 각 레이저 개체를 업데이트해줌
    /// 충돌관련 로직은 모두 레이저에서 처리 후 각 개체에 통보하는 방식
    ///     ex) 충돌 오브젝트 탐색 후 hp, 타입등 읽어온 후 로직 처리 후 각 개체에 통보
    /// </summary>
    public class Laser : MonoBehaviour
    {
        #region Variable
        [SerializeField] private Data.LaserData m_LaserData;

        private LaserParticle m_LaserParticle;

        private List<Laser> m_ChildLazers = new List<Laser>();
        private List<LaserInfo> m_LaserInfo;

        private Func<List<LaserInfo>, int, List<Laser>> m_LaserCreateFunc;
        private Action<List<Laser>> m_LaserEraseAction;

        private ICollisionable m_Target = null;

        private ELaserStateType m_State;

        private Vector2 m_StartPoint;
        private Vector2 m_EndPoint;
        private Vector2 m_DirectionVector;
        private Vector2 m_HitNormal;

        private bool m_IsInitated;
        private bool m_IsActivated;
        private bool m_IsErased;

        private int m_Hierarchy;
        #endregion

        private void Awake()
        {
            m_IsInitated = false;

            m_LaserParticle = new LaserParticle(
                    transform.GetChild(0).GetComponent<ParticleSystem>(),
                    transform.GetChild(1).GetComponent<ParticleSystem>()
                    );
        }

        public void Init(Func<List<LaserInfo>, int, List<Laser>> laserCreateFunc, Action<List<Laser>> laserEraseAction)
        {
            m_LaserCreateFunc = laserCreateFunc;
            m_LaserEraseAction = laserEraseAction;
        }

        public void Activate(Vector2 position, Vector2 dir, int hierarchy)
        {
            m_StartPoint = position;
            m_EndPoint = position;
            m_DirectionVector = dir.normalized;
            m_IsErased = false;
            m_IsInitated = true;
            m_IsActivated = true;
            m_IsErased = false;
            m_Hierarchy = hierarchy;
            //생성 시 디메리트 계산로직 입맛대로 수정 가능
            if (m_Hierarchy >= GameManager.s_LaserCriticalPoint)
            {
                Energy.UseEnergy(Energy.GetEnergy() / 10);
            }
            //Debug.Log("계층: " + m_Hierarchy);

            m_State = ELaserStateType.Move;

            m_ChildLazers.Clear();

            m_LaserParticle.OffEffectParticle();
            m_LaserParticle.OffHitParticle();
        }

        /// <summary>
        /// [움직임]  : 충돌 전 상태 업데이트마다 방향벡터 방향으로 이동
        /// [충돌]    : 최초 충돌에서 자식 레이저 생성, 주기마다 에너지 체크 후 충돌 블럭 공격
        ///             레이저의 생성(분기)는 움직임 상태에서 최초 충돌을 감지한 순간 수행된다.
        /// [대기]    : 에니메이션 또는 이벤트 동기화로 대기하는 상태             
        /// </summary>
        public void Run()
        {
            if (!m_IsInitated) { return; }
            switch (m_State)
            {
                case ELaserStateType.Move://에너지 소모x 이동만
                    Move();
                    break;
                case ELaserStateType.Hitting:
                    Hiting();
                    break;
                case ELaserStateType.Wait:
                    Wait();
                    break;
                default:
                    Debug.Log("잘못된 레이저 상태입니다.");
                    break;
            }
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
            if (m_IsErased) { return true; }

            float dist = Vector2.Distance(m_StartPoint, m_EndPoint);
            float eraseVelocity = m_LaserData.EraseVelocity * Time.deltaTime;
            if (dist <= eraseVelocity)
            {
                m_IsErased = true;
                MoveStartPoint(m_StartPoint, eraseVelocity, dist);
                m_LaserParticle.OffEffectParticle();
                m_LaserParticle.OffHitParticle();

                if (m_Target != null && m_Target.GetEEntityType() == EEntityType.Floor)
                     Energy.DeCollideWithFloor();

                return true;
            }
            MoveStartPoint(m_StartPoint + eraseVelocity * m_DirectionVector, eraseVelocity, dist);
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
            if (!Energy.CheckEnergy()) { return; }
            if (!m_IsActivated) { return; }

            RaycastHit2D hit = Physics2D.CircleCast(m_StartPoint, 0.1f, m_DirectionVector, Mathf.Infinity, RayManager.s_LaserHitableLayer);

            float dist = Vector2.Distance(m_EndPoint, hit.point);
            float shootingVelocity = m_LaserData.ShootingVelocity * Time.deltaTime;
            if (hit.collider != null && dist <= shootingVelocity)
            {
                MoveEndPoint(hit.point);
                m_HitNormal = hit.normal;
                m_Target = hit.transform.GetComponent<ICollisionable>();
                m_LaserInfo = m_Target.Hitted(hit, m_DirectionVector, this);

                if (m_State == ELaserStateType.Wait) return;

                AddChild(m_LaserCreateFunc?.Invoke(m_LaserInfo, m_Hierarchy + 1));
                return;
            }
            MoveEndPoint(m_EndPoint + shootingVelocity * m_DirectionVector);
        }

        private void MoveStartPoint(Vector2 pos, float velocity, float dist)
        {
            m_LaserParticle.SetLaserEffectErase(dist, velocity, m_StartPoint);
            m_StartPoint = pos;
        }

        private void MoveEndPoint(Vector2 pos)
        {
            m_EndPoint = pos;
            m_LaserParticle.SetLaserEffectMove(Vector2.Distance(m_StartPoint, m_EndPoint), m_StartPoint, m_DirectionVector);
        }

        public void ChangeLaserState(ELaserStateType type)
        {
            m_State = type;
        }

        public void Wait()
        {
            if (m_Target.Waiting())
            {
                //기존 방식 -> +1이 아닌 -1 등도 가능 단 -1 이하일 경우 clamp를 해주어야한다
                //AddChild(m_LaserCreateFunc?.Invoke(m_LaserInfo, m_Hierarchy + 1));

                //프리즘 통과 시 이후 레이저 계층 조정 마지막 변수를 조정하면됨
                AddChild(m_LaserCreateFunc?.Invoke(m_LaserInfo, 0));
                m_State = ELaserStateType.Hitting;
            }
        }

        /// <summary>
        /// 작동 순서
        /// 1. 충돌 중인 블럭의 타입을 확인하고 공격 불가능인 경우 업데이트 종료
        /// 2. 공격이 가능할 경우 에너지 잔량을 확인 후 0이 아니면 공격 진행
        /// 3. 해당 블럭에 GetDamage함수를 호출해 데미지를 주고 Energy개체에 에너지를 감소시킨다.
        /// </summary>
        public void Hiting()
        {
            if (!m_Target.IsGetDamageable()) { return; }
            if (!m_IsActivated) { return; }

            if (Energy.CheckEnergy())//발사전 에너지 사용가능여부 확인
            {
                if (!m_Target.GetDamage(m_LaserData.Damage))
                {
                    m_LaserParticle.OffHitParticle();
                    m_LaserEraseAction?.Invoke(m_ChildLazers);
                    m_Target = null;
                    m_State = ELaserStateType.Move;
                }
                else m_LaserParticle.SetHitEffectPosition(m_EndPoint, m_HitNormal);
            }
        }

        private void AddChild(List<Laser> child)
        {
            for (int i = 0; i < child.Count; i++)
            {
                m_ChildLazers.Add(child[i]);
            }
        }

        //자신을 기반을 생긴 자식 레이저의 상태를 비활성화로 만든다.
        public void LossParentLasersDeActivate()
        {
            Queue<Laser> remover = new Queue<Laser>();
            remover.Enqueue(this);

            while (remover.Count > 0)
            {
                Laser now = remover.Dequeue();
                now.m_IsActivated = false;
                for (int i = 0; i < now.m_ChildLazers.Count; i++)
                {
                    remover.Enqueue(now.m_ChildLazers[i]);
                }
            }
        }

        public bool IsHittingDamagable()
        {
            if (m_Target == null) return false;
            if (m_Target.IsGetDamageable()) return true;

            return false;
        }

        public ELaserStateType GetELaserStateType()
        {
            return m_State;
        }

        private void OnDestroy()
        {
            m_LaserCreateFunc = null;
            m_LaserEraseAction = null;
        }


        //보강 부탁드립니다.
        public void Reset()
        {
            m_Target = null;
            m_LaserParticle = null;

        }
    }
}
