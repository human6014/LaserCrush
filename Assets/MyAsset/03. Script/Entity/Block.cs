using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using LaserCrush.Data;
using LaserCrush.Manager;
using LaserCrush.Extension;
using LaserCrush.Entity.Interface;
using UnityEngine.Assertions;

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

        private BoxCollider2D m_BoxCollider2D;
        private Animator m_Animator;
        private SpriteRenderer m_SpriteRenderer;
        private TextMeshProUGUI m_Text;
        private Action<Block> m_PlayParticleAction;

        private EEntityType m_EntityType;

        private int m_AttackCount;
        private bool m_IsDestroyed;

        private static readonly int s_AudioCount = 7;
        private static readonly string s_BlockDestroyAudioKey = "BlockDestroy";
        private static readonly string s_BlockDamageAudioKey = "BlockDamage";

        protected List<MatrixPos> m_MatrixPos = new List<MatrixPos>();
        #endregion

        #region Property
        public int CurrentHP { get; protected set; }
        public int Score { get; protected set; }
        public virtual int RowNumber { get => m_MatrixPos[0].RowNumber; }
        public virtual int ColNumber { get => m_MatrixPos[0].ColNumber; }
        public virtual bool IsBossBlock { get => false; }
        public Vector2 Position { get; private set; }
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
            CurrentHP = hp;
            Score = hp;

            m_MatrixPos.Clear();
            m_MatrixPos.Add(new MatrixPos(rowNumber, colNumber));

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

        public void Init(int hp, EEntityType entityType, DroppedItemType itemType, Vector2 pos, Action<Block> playParticleAction)
        {
            CurrentHP = hp;
            Score = hp;

            ItemType = itemType;
            Position = pos;
            m_PlayParticleAction = playParticleAction;

            m_BoxCollider2D.enabled = true;
            m_IsDestroyed = false;
            m_AttackCount = 0;
            m_Text.text = GetHP().ToString();
            m_EntityType = entityType;

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
            //여기에 추가로 보스블럭인 경우 만들어 줄 계획
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
                Vector2 dir = Vector2.Reflect(parentDirVector, hit.normal);
                Vector2 pos = hit.point + dir;
                LaserInfo info = new LaserInfo
                {
                    Direction = dir,
                    Position = pos
                };
                return new List<LaserInfo>() { info };
            }
            m_Text.text = GetHP().ToString();
            GameManager.ValidHit++;
            return answer;
        }
        #endregion

        #region MoveDown
        public void MoveDown(Vector2 moveDownVector, float moveDownTime, int step)
        {
            StartCoroutine(MoveDownCoroutine(moveDownVector, moveDownTime));
            for(int i = 0; i < m_MatrixPos.Count; i++) 
            {
                MatrixPos pos = m_MatrixPos[i];
                pos.RowNumber += step;
                m_MatrixPos[i] = pos;
            }
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

        public bool IsAvailablePos(int row, int col)
        {
            for(int i = 0; i < m_MatrixPos.Count; i++)
            {
                if(row == m_MatrixPos[i].RowNumber && col == m_MatrixPos[i].ColNumber) { return false; }
            }
            return true;
        }
        #endregion

        public bool IsGameOver(int maxRow)
        {
            for(int i = 0; i < m_MatrixPos.Count; i++)
            {
                if (m_MatrixPos[i].RowNumber >= maxRow) { return true; }
            }
            return false;
        }

        private int GetHP()
            => CurrentHP / 100;

        /// <summary>
        /// 파티클, 사운드 실행하고 삭제
        /// </summary>
        private void Destroy()
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

        private void OnDestroy()
            => m_PlayParticleAction = null;

        public override void ReturnObject()
            => m_PlayParticleAction?.Invoke(this);
    }
}