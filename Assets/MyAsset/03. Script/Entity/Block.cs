using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using LaserCrush.Data;
using LaserCrush.Manager;
using LaserCrush.Entity.Item;
using TMPro;

namespace LaserCrush.Entity
{
    public enum DroppedItemType
    {
        None,
        Energy,
        Prism1,
        Prism2,
        Prsim3
    }

    public sealed class Block : PoolableMonoBehaviour, ICollisionable
    {
        #region Variable
        [SerializeField] private BlockData m_BlockData;

        private BoxCollider2D m_BoxCollider2D;
        private Animator m_Animator;
        private SpriteRenderer m_SpriteRenderer;
        private TextMeshProUGUI m_Text;
        private Action<Block> m_PlayParticleAction;

        private EEntityType m_EntityType;

        private int m_AttackCount;
        private bool m_IsDestroyed;

        #endregion

        #region Property
        public int CurrentHP { get; private set; }
        public int Score { get; private set; }
        public int RowNumber { get; private set; }
        public int ColNumber { get; private set; }
        public Vector2 Position { get; private set; }
        public DroppedItemType ItemType { get; private set; }
        #endregion

        private void Awake()
        {
            m_BoxCollider2D = GetComponent<BoxCollider2D>();
            m_Animator = GetComponent<Animator>();
            m_Text = GetComponentInChildren<TextMeshProUGUI>();
            m_SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        /// <summary>
        /// 화면에 띄우기 전 반드시 초기화함수 호출할 것
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="entityType"></param>
        /// <param name="droppedItem">드랍 아이템이 없을 경우 널값을 대입</param>
        public void Init(int hp, int rowNumber, int colNumber, EEntityType entityType, DroppedItemType itemType, Vector2 pos, Action<Block> playParticleAction)
        {
            CurrentHP = hp;
            Score = hp;
            RowNumber = rowNumber;
            ColNumber = colNumber;
            m_EntityType = entityType;
            ItemType = itemType;
            Position = pos;
            m_PlayParticleAction = playParticleAction;

            m_BoxCollider2D.enabled = true;
            m_IsDestroyed = false;
            m_AttackCount = 0;
            m_Text.text = GetHP().ToString();
            m_SpriteRenderer.color = (m_EntityType == EEntityType.NormalBlock) ?
                m_BlockData.NormalBlockColor :
                m_BlockData.ReflectBlockColor;

            StartCoroutine(InitAnimation(0.2f));
        }

        public IEnumerator InitAnimation(float totalTime)
        {
            Vector2 startScale = m_BlockData.InitScale;
            Vector2 endScale = transform.localScale;
            float elapsedTime = 0;
            while (elapsedTime <= totalTime)
            {
                elapsedTime += Time.deltaTime;
                transform.localScale = Vector2.Lerp(startScale, endScale, elapsedTime / totalTime);
                yield return null;
            }

            transform.localScale = endScale;
        }

        /// <summary>
        /// 파티클, 사운드 실행하고 삭제
        /// </summary>
        private void Destroy()
        {
            AudioManager.AudioManagerInstance.PlayOneShotNormalSE("BlockDestroy");
            m_IsDestroyed = true;
            m_PlayParticleAction?.Invoke(this);
        }

        /// <summary>
        /// 파티클, 사운드 실행 안하고 바로 삭제
        /// </summary>
        public void ImmediatelyReset()
            => m_IsDestroyed = true;

        public bool GetDamage(int damage)
        {
            if (m_IsDestroyed) return false;

            if (m_AttackCount % 7 == 0) AudioManager.AudioManagerInstance.PlayOneShotNormalSE("BlockDamage");
            m_AttackCount++;

            GameManager.s_ValidHit++;
            damage *= GameManager.s_StageNum + 1 / 2;

            m_Animator.SetTrigger("Hit");

            if (CurrentHP <= damage) // 남은 피가 데미지보다 작을 경우
            {
                Energy.UseEnergy(CurrentHP);
                Destroy();
                return false;
            }
            else CurrentHP -= Energy.UseEnergy(damage);

            if (GetHP() == 0)
            {
                Energy.UseEnergy(CurrentHP);
                Destroy();
                return false;
            }
            m_Text.text = GetHP().ToString();
            return true;
        }

        public bool IsGetDamageable()
            => true;
        
        public bool Waiting()
            => true;
        
        public EEntityType GetEEntityType()
            => m_EntityType;
        
        private int GetHP() 
            => CurrentHP / 100;
        
        public List<LaserInfo> Hitted(RaycastHit2D hit, Vector2 parentDirVector, Laser laser)
        {
            List<LaserInfo> answer = new List<LaserInfo>();
            laser.ChangeLaserState(ELaserStateType.Hitting);
            if (m_EntityType == EEntityType.ReflectBlock)//반사 블럭일 경우만 자식 생성
            {
                //Vector2 dir = (hit.normal + parentDirVector + hit.normal).normalized;
                LaserInfo info = new LaserInfo
                {
                    Direction = Vector2.Reflect(parentDirVector, hit.normal),
                    Position = hit.point
                };
                return new List<LaserInfo>() { info };
            }
            m_Text.text = GetHP().ToString();
            GameManager.s_ValidHit++;
            return answer;
        }

        public void MoveDown(Vector2 moveDownVector, float moveDownTime)
        {
            StartCoroutine(MoveDownCoroutine(moveDownVector, moveDownTime));
            RowNumber++;
        }

        private IEnumerator MoveDownCoroutine(Vector2 moveDownVector, float moveDownTime)
        {
            Vector2 startPos = transform.position;
            Vector2 endPos = startPos + moveDownVector;
            float elapsedTime = 0;
            while (elapsedTime < moveDownTime)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector2.Lerp(startPos, endPos, elapsedTime / moveDownTime);
                yield return null;
            }
            Position = endPos;
            transform.position = endPos;
        }

        private void OnDestroy()
            => m_PlayParticleAction = null;

        public override void ReturnObject()
            => m_PlayParticleAction?.Invoke(this);
    }
}