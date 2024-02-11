using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using LaserCrush.Data;
using LaserCrush.Manager;
using LaserCrush.Extension;
using LaserCrush.Entity.Interface;

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

    public struct MatrixPos
    {
        public int RowNumber;
        public int ColNumber;

        public MatrixPos(int rowNumber, int colNumber)
        {
            RowNumber = rowNumber;
            ColNumber = colNumber;
        }
    }

    public class Block : PoolableMonoBehaviour, ICollisionable
    {
        #region Variable
        [SerializeField] private BlockData m_BlockData;

        protected BoxCollider2D m_BoxCollider2D;
        protected Animator m_Animator;
        protected SpriteRenderer m_SpriteRenderer;
        protected TextMeshProUGUI m_Text;
        protected Action<Block> m_PlayParticleAction;

        protected EEntityType m_EntityType;

        protected int m_AttackCount;
        protected bool m_IsDestroyed;

        protected static readonly int s_AudioCount = 7;
        protected static readonly string s_BlockDestroyAudioKey = "BlockDestroy";
        protected static readonly string s_BlockDamageAudioKey = "BlockDamage";

        protected List<MatrixPos> m_matrixPos = new List<MatrixPos>();
        #endregion

        #region Property
        public int CurrentHP { get; protected set; }
        public int Score { get; protected set; }
        public int RowNumber { get; protected set; }
        public int ColNumber { get; protected set; }
        public Vector2 Position { get; protected set; }
        public DroppedItemType ItemType { get; protected set; }
        #endregion

        #region Init
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
        public virtual void Init(int hp, int rowNumber, int colNumber, EEntityType entityType, DroppedItemType itemType, Vector2 pos, Action<Block> playParticleAction)
        {
            m_matrixPos.Clear();
            CurrentHP = hp;
            Score = hp;
            //
            m_matrixPos.Add(new MatrixPos(rowNumber, colNumber));
            m_matrixPos.Add(new MatrixPos(rowNumber - 1, colNumber));
            m_matrixPos.Add(new MatrixPos(rowNumber, colNumber - 1));
            m_matrixPos.Add(new MatrixPos(rowNumber - 1, colNumber - 1));
            //
            //m_matrixPos.Add(new MatrixPos(rowNumber,colNumber));

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

            if (m_EntityType == EEntityType.NormalBlock)
            {
                m_SpriteRenderer.color = m_BlockData.NormalBlockColor;
                gameObject.layer = m_BlockData.NormalLayer.GetLayerNumber();
            }
            else if (m_EntityType == EEntityType.ReflectBlock)
            {
                m_SpriteRenderer.color = m_BlockData.ReflectBlockColor;
                gameObject.layer = m_BlockData.ReflectLayer.GetLayerNumber();
            }
            else Debug.LogError("Block has incorrect type");

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
        #endregion

        #region ICollisionable
        public bool GetDamage(int damage)
        {
            if (m_IsDestroyed) return false;

            if (m_AttackCount % s_AudioCount == 0) AudioManager.AudioManagerInstance.PlayOneShotNormalSE(s_BlockDamageAudioKey);
            m_AttackCount++;

            GameManager.ValidHit++;
            damage *= (int)((GameManager.StageNum + 1 / 2) * 0.85);

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
            GameManager.ValidHit++;
            return answer;
        }
        #endregion

        #region MoveDown
        public void MoveDown(Vector2 moveDownVector, float moveDownTime)
        {
            StartCoroutine(MoveDownCoroutine(moveDownVector, moveDownTime));
            RowNumber++;
            for(int i = 0; i < m_matrixPos.Count; i++) 
            {
                MatrixPos pos = m_matrixPos[i];
                pos.RowNumber++;
                m_matrixPos[i] = pos;
            }
        }
        public void MoveDownTwoSpaces(Vector2 moveDownVector, float moveDownTime)
        {
            StartCoroutine(MoveDownCoroutine(moveDownVector, moveDownTime));
            RowNumber += 2;
            for (int i = 0; i < m_matrixPos.Count; i++)
            {
                MatrixPos pos = m_matrixPos[i];
                pos.RowNumber += 2;
                m_matrixPos[i] = pos;
            }
        }

        protected virtual IEnumerator MoveDownCoroutine(Vector2 moveDownVector, float moveDownTime)
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

        public List<MatrixPos> GetMatrixPos()
        {
            return m_matrixPos;
        }
        #endregion

        protected int GetHP()
            => CurrentHP / 100;

        /// <summary>
        /// 파티클, 사운드 실행하고 삭제
        /// </summary>
        protected void Destroy()
        {
            AudioManager.AudioManagerInstance.PlayOneShotNormalSE(s_BlockDestroyAudioKey);
            m_IsDestroyed = true;
            m_PlayParticleAction?.Invoke(this);
        }

        /// <summary>
        /// 파티클, 사운드 실행 안하고 바로 삭제
        /// </summary>
        public void ImmediatelyReset()
            => m_IsDestroyed = true;

        protected void OnDestroy()
            => m_PlayParticleAction = null;

        public override void ReturnObject()
            => m_PlayParticleAction?.Invoke(this);
    }
}