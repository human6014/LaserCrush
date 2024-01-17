using LaserCrush.Entity;
using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Data;
using System;
using TMPro;
using LaserCrush.Manager;
using System.Collections;

namespace LaserCrush
{
    public sealed class Block : MonoBehaviour, ICollisionable
    {
        #region Variable
        [SerializeField] private BlockData m_BlockData;

        private Animator m_Animator;
        private DroppedItem m_DroppedItem;
        private SpriteRenderer m_SpriteRenderer;
        private TextMeshProUGUI m_Text;

        private EEntityType m_EntityType;

        private int m_HP;
        private int m_AttackCount;
        private bool m_IsDestroyed;

        private Action<Block, DroppedItem> m_RemoveBlockAction;
        #endregion

        public int BlockScore { get; private set; }
        public int RowNumber { get; private set; }
        public int ColNumber { get; private set; }

        private void Awake()
        {
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
        public void Init(int hp, int rowNumber, int colNumber, float animationTime, EEntityType entityType, DroppedItem droppedItem, Action<Block, DroppedItem> removeBlockAction)
        {
            m_HP = hp;
            BlockScore = hp;
            RowNumber = rowNumber;
            ColNumber = colNumber;
            m_EntityType = entityType;
            m_DroppedItem = droppedItem;
            m_AttackCount = 0;
            m_Text.text = GetHP().ToString();
            m_SpriteRenderer.color = (m_EntityType == EEntityType.NormalBlock) ?
                m_BlockData.NormalBlockColor :
                m_BlockData.ReflectBlockColor;

            m_RemoveBlockAction = removeBlockAction;

            StartCoroutine(InitAnimation(animationTime));
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

        private void Destroy()
        {
            AudioManager.AudioManagerInstance.PlayOneShotNormalSE("BlockDestroy");
            m_IsDestroyed = true;
            m_RemoveBlockAction?.Invoke(this, m_DroppedItem);
            Destroy(gameObject);
        }

        public void DestoryReset()
        {
            m_IsDestroyed = true;
            if (m_DroppedItem is not null) Destroy(m_DroppedItem.gameObject);
            Destroy(gameObject);
        }

        public bool GetDamage(int damage)
        {
            if (m_IsDestroyed) return false;

            if (m_AttackCount % 7 == 0) AudioManager.AudioManagerInstance.PlayOneShotNormalSE("BlockDamage");
            m_AttackCount++;

            GameManager.s_ValidHit++;
            damage *= GameManager.s_StageNum + 1 / 2;

            m_Animator.SetTrigger("Hit");

            if (m_HP <= damage) // 남은 피가 데미지보다 작을 경우
            {
                Energy.UseEnergy(m_HP);
                Destroy();
                return false;
            }
            else m_HP -= Energy.UseEnergy(damage);

            if (GetHP() == 0)
            {
                Energy.UseEnergy(m_HP);
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
            => EEntityType.NormalBlock;
        
        private int GetHP() 
            => m_HP / 100;
        

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
            transform.position = endPos;
        }

        private void OnDestroy()
            => m_RemoveBlockAction = null;
    }
}