using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using LaserCrush.Data;
using LaserCrush.Manager;
using LaserCrush.Extension;
using LaserCrush.Entity.Interface; 

namespace LaserCrush.Entity.Block
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
        [SerializeField] private SpriteRenderer m_MainSpriteRenderer;

        private BoxCollider2D m_BoxCollider2D;
        private Animator m_Animator;
        private TextMeshProUGUI m_Text;
        private Action<Block> m_ReturnAction;

        private EEntityType m_EntityType;

        private Vector2 m_MoveStartPos;
        private Vector2 m_MoveEndPos;

        private float m_ElapsedTime;
        private float m_MoveDownTime;

        private int m_AttackCount;
        private bool m_IsDestroyed;

        private static readonly string s_BlockDestroyAudioKey = "BlockDestroy";
        private static readonly string s_BlockDamageAudioKey = "BlockDamage";

        protected List<MatrixPos> m_MatrixPos = new List<MatrixPos>();
        #endregion

        #region Property
        public int CurrentHP { get; private set; }
        public int Score { get; private set; }
        public virtual int RowNumber { get => m_MatrixPos[0].RowNumber; }
        public virtual int ColNumber { get => m_MatrixPos[0].ColNumber; }
        public virtual bool IsBossBlock { get => false; }
        public Vector2 Position { get; private set; }
        public DroppedItemType ItemType { get; private set; }
        #endregion

        #region Init
        private void Awake()
        {
            m_BoxCollider2D = GetComponent<BoxCollider2D>();
            m_Animator = GetComponent<Animator>();
            m_Text = GetComponentInChildren<TextMeshProUGUI>();
            m_MainSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        /// <summary>
        /// 화면에 띄우기 전 반드시 초기화함수 호출할 것
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="entityType"></param>
        /// <param name="droppedItem">드랍 아이템이 없을 경우 널값을 대입</param>
        public virtual void Init(int hp, int rowNumber, int colNumber, EEntityType entityType, DroppedItemType itemType, Vector2 pos, Action<Block> returnAction)
        {
            m_MatrixPos.Clear();
            m_MatrixPos.Add(new MatrixPos(rowNumber, colNumber));

            InitSetting(hp, entityType, itemType, pos, returnAction);
        }

        public void InitSetting(int hp, EEntityType entityType, DroppedItemType itemType, Vector2 pos, Action<Block> returnAction)
        {
            CurrentHP = hp;
            Score = hp;

            ItemType = itemType;
            Position = pos;
            m_ReturnAction = returnAction;

            m_BoxCollider2D.enabled = true;
            m_IsDestroyed = false;
            m_AttackCount = 0;
            m_Text.text = GetHP().ToString();
            m_EntityType = entityType;

            if (m_EntityType == EEntityType.NormalBlock)
            {
                m_MainSpriteRenderer.color = m_BlockData.NormalBlockColor;
                gameObject.layer = m_BlockData.NormalLayer.GetLayerNumber();
            }
            else if (m_EntityType == EEntityType.ReflectBlock)
            {
                m_MainSpriteRenderer.color = m_BlockData.ReflectBlockColor;
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
        public bool GetDamage()
        {
            if (m_IsDestroyed) return false;

            if (m_AttackCount % m_BlockData.AudioCount == 0) AudioManager.AudioManagerInstance.PlayOneShotConcurrent(s_BlockDamageAudioKey);
            m_AttackCount++;

            m_Animator.SetTrigger("Hit");

            CurrentHP -= Energy.CurrentDamage;
            if (GetHP() <= 0)
            {
                Destroy();
                return false;
            }
            SetHPText(GetHP());

            return true;
        }

        public bool IsGetDamageable()
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
                LaserInfo info = new LaserInfo(pos, dir);
                return new List<LaserInfo>() { info };
            }

            return answer;
        }
        #endregion

        #region MoveDown
        public void MoveDownStart(Vector2 moveDownVector, float moveDownTime, int step)
        {
            m_MoveStartPos = transform.position;
            m_MoveEndPos = m_MoveStartPos + moveDownVector;
            m_ElapsedTime = 0;
            m_MoveDownTime = moveDownTime;
            for (int i = 0; i < m_MatrixPos.Count; i++)
            {
                MatrixPos pos = m_MatrixPos[i];
                pos.RowNumber += step;
                m_MatrixPos[i] = pos;
            }
        }

        public void MoveDown()
        {
            m_ElapsedTime += Time.deltaTime;
            transform.position = Vector2.Lerp(m_MoveStartPos, m_MoveEndPos, m_ElapsedTime / m_MoveDownTime);
        }

        public void MoveDownEnd()
        {
            Position = m_MoveEndPos;
            transform.position = m_MoveEndPos;
        }
        #endregion

        public bool IsAvailablePos(int row, int col)
        {
            for(int i = 0; i < m_MatrixPos.Count; i++)
            {
                if (row == m_MatrixPos[i].RowNumber && 
                    col == m_MatrixPos[i].ColNumber) 
                    return false;
            }
            return true;
        }

        public bool IsGameOver(int maxRow)
        {
            for(int i = 0; i < m_MatrixPos.Count; i++)
            {
                if (m_MatrixPos[i].RowNumber >= maxRow) return true;
            }
            return false;
        }

        #region HP
        private int GetHP()
            => CurrentHP / 100;

        protected virtual void SetHPText(int hp)
            => m_Text.text = hp.ToString();
        #endregion

        #region Destroy & Return
        /// <summary>
        /// 파티클, 사운드 실행하고 삭제
        /// </summary>
        private void Destroy()
        {
            AudioManager.AudioManagerInstance.PlayOneShotNormalSE(s_BlockDestroyAudioKey);
            Energy.ChargeCurrentMaxTime(m_BlockData.AdditionalTime);
            m_IsDestroyed = true;
            StopAllCoroutines();
            m_ReturnAction?.Invoke(this);
        }

        /// <summary>
        /// 파티클, 사운드 실행 안하고 바로 삭제
        /// </summary>
        public void DestroyImmediate()
            => m_IsDestroyed = true;

        private void OnDestroy()
            => m_ReturnAction = null;

        public override void ReturnObject()
            => StopAllCoroutines();
        #endregion
    }
}