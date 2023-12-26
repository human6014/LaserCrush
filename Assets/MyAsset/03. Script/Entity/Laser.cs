using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Laser.Manager;
using static ICollisionable;
using Unity.Android.Types;
using Unity.Burst.CompilerServices;
using UnityEngine.UIElements;
using UnityEngineInternal;

namespace Laser.Entity
{
    enum LaserStateType // 이름 고민
    {
        Hitting,
        Move
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
        private Vector2 m_StartPoint;
        private Vector2 m_EndPoint;
        private Vector2 m_DirectionVector;
        private float m_EraseVelocity;
        private float m_ShootingVelocity;
        private List<Laser> m_ChildLazers = new List<Laser>();
        private LaserStateType m_State;
        private int m_Damage;//변수이름 고민 -> 한번 소모할 에너지를 보관할 변수
        private ICollisionable m_Target = null; // 이부분도 고민 해봐야함
        #endregion

        /// <summary>
        /// 레이저 총 상태
        /// [움직임] : 충돌 전 상태 업데이트마다 방향벡터 방향으로 이동
        /// [충돌] : 최초 충돌에서 자식 레이저 생성, 주기마다 에너지 체크 후 충돌 블럭 고격
        ///          레이저의 생성(분기)는 움직임 상태에서 최초 충돌을 감지한 순간 수행된다.
        /// </summary>
        public void Update()
        {
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

        public void GenerateLazer(Vector2 direction)
        {
            //레이저 생성
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
            if (Vector2.Distance(m_StartPoint, m_EndPoint) <= m_EraseVelocity)
            {
                //삭제
                m_StartPoint = m_EndPoint;
                return true; ;
            }
            m_StartPoint += m_DirectionVector * m_EraseVelocity;
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

            RaycastHit2D hit = Physics2D.Raycast(m_EndPoint, m_DirectionVector, Mathf.Infinity, 1
                << LayerMask.NameToLayer("Reflectable") | 1 << LayerMask.NameToLayer("Absorbable"));
            if (hit.distance <= m_ShootingVelocity)//충돌 시
            {
                m_Target = hit.collider.GetComponent<ICollisionable>();

                /*Vector2 temVec = m_DirectionVector + hit.normal;
                temVec = (hit.normal + temVec).normalized;

                GameObject prefab = new GameObject();
                GameObject obj = Instantiate(prefab);

                obj.GetComponent<Laser>().Init(hit.transform, hit.transform, temVec);//초기화
                LaserManager.AddLaser(obj.GetComponent<Laser>());//이거 맞나?*/
                
                //데미지 계산X -> 자식 레이저 생성만 담당
                switch (m_Target.GetEntityType())
                {
                    case EntityType.NormalBlock:
                        CollideNormalBlock();
                        break;
                    case EntityType.ReflectBlock:
                        CollideReflectBlock(hit);
                        break;
                    case EntityType.Prisim:
                        CollidePrisim(hit);
                        break;
                    case EntityType.Floor:
                        CollideFloor();
                        break;
                    case EntityType.Wall:
                        CollideWall(hit);
                        break;
                    case EntityType.Launcher:

                    default:
                        Debug.Log("충돌 개체의 타입이 올바르지 않음"); break;
                }
            }
            m_EndPoint += m_DirectionVector * m_ShootingVelocity;
        }

        /// <summary>
        /// 작동 순서
        /// 1. 충돌 중인 블럭의 타입을 확인하고 공격 불가능인 경우 업데이트 종료
        /// 2. 공격이 가능할 경우 에너지 잔량을 확인 후 0이 아니면 공격 진행
        /// 3. 해당 블럭에 GetDamage함수를 호출해 데미지를 주고 Energy개체에 에너지를 감소시킨다.
        /// </summary>
        public void Hiting()
        {
            if (!m_Target.IsAttackable())
            {
                return;
            }

            if (Energy.CheckEnergy())//발사전 에너지 사용가능여부 확인
            {
                m_Target.GetDamage(m_Damage);
            }
        }

        public void Complete()
        {

        }

        public void Init(Vector2 posion, Vector2 dir)
        {
            m_StartPoint = posion;
            m_EndPoint = posion;
            m_DirectionVector = dir.normalized;
        }

        public void CollideNormalBlock()
        {
            return;
        }

        public void CollideReflectBlock(RaycastHit2D hit)
        {
            //새로운 자식 레이저 생성
            Vector2 temDir = m_DirectionVector + hit.normal;
            temDir = (hit.normal + temDir).normalized;

            //To Do//
            //자식 레이저 개체 생성과 해당 방향백터로 초기화 해야됨 ㅠㅠ
            //Init()함수 호출하면 초기화 가능
        }

        public void CollidePrisim(RaycastHit2D hit)
        {
            List<Vector2> dir =  hit.collider.GetComponent<Prism>().GetEjectionPorts();
            for(int i = 0; i < dir.Count; ++i) 
            {
                //TODO//
                //dir을 방향벡터로 하고 위치는 hit 포인터로 하는 레이저 생성
                //레이저 생성 후 manager에 add함수를 호출해 추가해 주어야 한다.
            }
        }

        public void CollideWall(RaycastHit2D hit)
        {
            //새로운 자식 레이저 생성

            Vector2 temDir = m_DirectionVector + hit.normal;
            temDir = (hit.normal + temDir).normalized;
        }
        public void CollideFloor()
        {
            return;
        }

        public void CollideLauncher(RaycastHit2D hit)
        {
            Vector2 dir = hit.collider.GetComponent<Launcher>().GetDirectionVector();
            //TODO//
            //dir을 방향벡터로 하고 위치는 hit 포인터로 하는 레이저 생성
            //레이저 생성 후 manager에 add함수를 호출해 추가해 주어야 한다.
        }
    }
}
