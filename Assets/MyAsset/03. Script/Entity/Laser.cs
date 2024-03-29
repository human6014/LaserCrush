using System.Collections.Generic;
using System;
using UnityEngine;
using LaserCrush.Manager;
using LaserCrush.Entity.Interface;
using LaserCrush.Entity.Particle;

namespace LaserCrush.Entity
{
    public enum ELaserStateType // 이름 고민
    {
        Move,
        Hitting
    }

    /// <summary>
    /// 레이저 매니저가 매 업데이트마다 각 레이저 개체를 업데이트해줌
    /// 충돌관련 로직은 모두 레이저에서 처리 후 각 개체에 통보하는 방식
    ///     ex) 충돌 오브젝트 탐색 후 hp, 타입등 읽어온 후 로직 처리 후 각 개체에 통보
    /// </summary>
    public sealed class Laser : PoolableMonoBehaviour
    {
        #region Variable
        [SerializeField] private Data.LaserData m_LaserData;

        private LaserParticle m_LaserParticle;

        private List<Laser> m_ChildLazers = new List<Laser>();
        private List<LaserInfo> m_LaserInfo;

        private Func<List<LaserInfo>, List<Laser>> m_LaserCreateFunc;
        private Action<List<Laser>> m_LaserEraseAction;

        private ICollisionable m_Target = null;

        private ELaserStateType m_State;

        private Vector2 m_StartPoint;
        private Vector2 m_EndPoint;
        private Vector2 m_DirectionVector;
        private Vector2 m_HitNormal;

        private bool m_IsActivated;
        private bool m_IsErased;

        private static readonly string s_BlockDamageAudioKey = "BlockDamage";
        #endregion

        private void Awake()
        {
            m_LaserParticle = new LaserParticle(
                    transform.GetChild(0).GetComponent<ParticleSystem>(),
                    transform.GetChild(1).GetComponent<ParticleSystem>()
                    );
        }

        public void Init(Func<List<LaserInfo>, List<Laser>> laserCreateFunc, Action<List<Laser>> laserEraseAction)
        {
            m_LaserCreateFunc = laserCreateFunc;
            m_LaserEraseAction = laserEraseAction;
        }

        public void Activate(Vector2 position, Vector2 dir)
        {
            m_StartPoint = position;
            m_EndPoint = position;
            m_DirectionVector = dir.normalized;
            m_IsErased = false;
            m_IsActivated = true;

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
            if (!m_IsActivated || m_IsErased) return;

            switch (m_State)
            {
                case ELaserStateType.Move://에너지 소모x 이동만
                    Move();
                    break;
                case ELaserStateType.Hitting:
                    Hiting();
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

        #region Move Point
        /// <summary>
        /// 일정 속도만큼 startPoint를 endPoint방향으로 이동
        /// 레이저가 지워지면 true 반환
        /// 배열에서 지우지 않는다
        /// </summary>
        public bool Erase()
        {
            if (m_IsErased) return true;

            float dist = Vector2.Distance(m_StartPoint, m_EndPoint);
            float eraseVelocity = m_LaserData.EraseVelocity * Time.deltaTime;
            if (dist <= eraseVelocity)
            {
                m_IsErased = true;
                MoveStartPoint(m_StartPoint, eraseVelocity, dist);

                ResetLaser();

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
        private void Move()
        {
            if (!Energy.IsValidTime()) { return; }

            RaycastHit2D hit = Physics2D.CircleCast(m_StartPoint, 0.001f, m_DirectionVector, Mathf.Infinity, RayManager.LaserHitableLayer);

            float dist = Vector2.Distance(m_EndPoint, hit.point);
            float shootingVelocity = m_LaserData.ShootingVelocity * Time.deltaTime;
            if (hit.collider != null && dist <= shootingVelocity)
            {
                MoveEndPoint(hit.point);
                m_HitNormal = hit.normal;
                m_Target = hit.transform.GetComponent<ICollisionable>();
                m_LaserInfo = m_Target.Hitted(hit, m_DirectionVector, this);

                if (m_Target.GetEEntityType() == EEntityType.Wall)
                    AudioManager.AudioManagerInstance.PlayOneShotNormalSE(s_BlockDamageAudioKey);

                AddChild(m_LaserCreateFunc?.Invoke(m_LaserInfo));
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
        #endregion

        public void ChangeLaserState(ELaserStateType type)
        {
            m_State = type;
        }

        /// <summary>
        /// 작동 순서
        /// 1. 충돌 중인 블럭의 타입을 확인하고 공격 불가능인 경우 업데이트 종료
        /// 2. 공격이 가능할 경우 에너지 잔량을 확인 후 0이 아니면 공격 진행
        /// 3. 해당 블럭에 GetDamage함수를 호출해 데미지를 주고 Energy개체에 에너지를 감소시킨다.
        /// </summary>
        private void Hiting()
        {
            if (!m_Target.IsGetDamageable()) { return; }

            if (Energy.IsValidTime())//발사전 에너지 사용가능여부 확인
            {
                if (!m_Target.GetDamage())
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

        public void ResetLaser()
        {
            m_IsActivated = false;
            m_Target = null;
            m_LaserParticle.OffEffectParticle();
            m_LaserParticle.OffHitParticle();
        }

        public override void ReturnObject()
        {
            //...
        }

        private void OnDestroy()
        {
            m_LaserCreateFunc = null;
            m_LaserEraseAction = null;
        }
    }
}
