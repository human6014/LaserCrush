using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using LaserCrush.Extension;
using LaserCrush.Manager;
using TMPro;
using Unity.VisualScripting;

namespace LaserCrush.Entity
{
    public struct LaserInfo
    {
        public Vector2 Position;
        public Vector2 Direction;

        public LaserInfo(Vector2 position, Vector2 direction)
        {
            Position = position;
            Direction = direction;
        }
    }
}
namespace LaserCrush.Entity.Item
{
    public enum ItemType
    {
        Prism1Branch,
        Prism2Branch,
        Prism3Branch,
        Prism4Branch
    }
    public sealed class InstalledItem : PoolableMonoBehaviour, ICollisionable
    {
        #region Variable
        [SerializeField] private Transform[] m_EjectionPortsTransform;
        [SerializeField] private LineRenderer[] m_LineRenderers;
        [SerializeField] private GameObject[] m_SubRotationImage;
        [SerializeField] private TextMeshProUGUI m_CountText;
        [SerializeField] private RectTransform m_CanvasTransform;
        [SerializeField] private ItemType m_ItemType;

        private ObjectPoolManager.PoolingObject m_Pool;
        private Animator m_Animator;
        private CircleCollider2D m_CircleCollider2D;
        private readonly List<LaserInfo> m_EjectionPorts = new List<LaserInfo>();

        private Vector2 m_TextInitPos;
        private static Color m_TextInitColor;
        private static Color m_TextEndColor;

        private static readonly float [] m_FontSizes = { 3.25f, 2.75f, 2.25f};
        private static readonly float[] m_ChargingEnergy = { 0.3f, 0.4f, 0.7f, 1.0f };
        private static readonly int[] m_MaxUsingNum = { 3, 3, 3, 2 };
        private const string m_ItemDragAudioKey = "ItemDrag";

        private const string m_FixedNoticeAnimationKey = "FixedNotice";
        private const string m_DestroyAnimationKey = "Destroy";

        private const float m_ChargingTime = 0.0f;
        private const float m_SubLineLength = 5;

        private float m_ChargingWait;

        private bool m_IsActivate;
        private bool m_IsAdjustMode;
        private bool m_IsSyn;
        #endregion

        #region Property
        public bool IsAdjustMode
        {
            get => m_IsAdjustMode;
            set
            {
                m_IsAdjustMode = value;

                for (int i = 0; i < m_SubRotationImage.Length; i++)
                    m_SubRotationImage[i].SetActive(value);

                if (m_IsAdjustMode) SetAdjustLine(Mathf.Infinity);
                else SetAdjustLine();
            }
        }
        public int RowNumber { get; private set; }
        public int ColNumber { get; private set; }
        public int RemainUsingCount { get; private set; }
        public bool IsFixedDirection { get; private set; }
        public Vector2 Position { get; private set; }
        public Vector2 Direction { get; private set; }
        public ItemType ItemType { get => m_ItemType; }
        #endregion

        #region Init related
        private void Awake()
        {
            m_Animator = GetComponent<Animator>();
            m_CircleCollider2D = GetComponent<CircleCollider2D>();
            m_CircleCollider2D.enabled = false;

            m_TextInitColor = m_CountText.color;
            m_TextEndColor = m_CountText.color;
            m_TextEndColor.a = 0;

            foreach (Transform tr in m_EjectionPortsTransform)
                m_EjectionPorts.Add(new LaserInfo(position: tr.position, direction: tr.up));

            IsAdjustMode = true;
        }

        public void ReInit()
        {
            foreach (Transform tr in m_EjectionPortsTransform)
                tr.gameObject.SetActive(true);

            m_CanvasTransform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            m_CircleCollider2D.enabled = false;
            IsAdjustMode = true;
        }

        /// <summary>
        /// 해야할 역할
        /// 1. m_EjectionPorts 위치와 방향 초기화
        /// 2. 사용횟수 초기화
        /// </summary>
        public void Init(int rowNumber, int colNumber, int remainCount, bool isFixDirection, Vector2 pos, Vector2 dir, ObjectPoolManager.PoolingObject pool)
        {
            RowNumber = rowNumber;
            ColNumber = colNumber;
            RemainUsingCount = m_MaxUsingNum[(int)m_ItemType];
            //RemainUsingCount = remainCount;
            IsFixedDirection = isFixDirection;
            Position = pos;
            Direction = dir;
            m_Pool = pool;

            transform.SetPositionAndRotation(pos, Quaternion.LookRotation(Vector3.forward, dir));

            m_TextInitPos = m_CountText.rectTransform.anchoredPosition + new Vector2(0, 2.5f);
            m_CircleCollider2D.enabled = true;
            IsAdjustMode = false;
            m_IsActivate = false;
            m_IsSyn = false;
        }
        #endregion

        public void FixDirection()
        {
            if (IsFixedDirection) return;
            IsFixedDirection = true;

            for (int i = 0; i < m_EjectionPorts.Count; i++)
            {
                m_EjectionPorts[i] = new LaserInfo(
                    position: m_EjectionPortsTransform[i].position,
                    direction: m_EjectionPortsTransform[i].up
                    );
            }
        }

        public bool IsOverloaded()
        {
            m_IsActivate = false;
            m_IsSyn = false;
            m_ChargingWait = 0;

            return RemainUsingCount == 0;
        }

        #region ICollisionable
        public bool Waiting()
        {
            m_ChargingWait += Time.deltaTime;

            //Waiting 함수 한 프레임 내에서 동시에 호출하면 if문 안쪽 여러번 호출 가능함
            //에너지 충전도 하나의 프리즘에서 여러번 호출댐
            //m_IsActivate조건으로 한번만 되게 만들긴 했는데 이래도 되나?
            //여기 안에 다른거랑 엮여있을 수도 있을 것 같아서 확인 부탁
            if (m_ChargingWait >= m_ChargingTime && !m_IsActivate)
            {
                float chargingWeight = m_ChargingEnergy[(int)m_ItemType];
                //Energy.ChargeEnergy((int)(Energy.MaxEnergy * chargingEnergy));
                m_IsActivate = true;
                GameManager.InvokeChargingEvent(chargingWeight);
            }

            return m_IsActivate;
        }

        public List<LaserInfo> Hitted(RaycastHit2D hit, Vector2 parentDirVector, Laser laser)
        {
            if (m_IsActivate) return new List<LaserInfo>();

            GameManager.ValidHit++;
            laser.ChangeLaserState(ELaserStateType.Wait);
            
            if (!m_IsSyn) 
            {
                RemainUsingCount--;
                m_IsSyn = true;
            }

            m_ChargingWait = 0;
            return m_EjectionPorts;
        }

        public bool IsGetDamageable()
            => false;

        public bool GetDamage(int damage)
            => false;

        public EEntityType GetEEntityType()
            => EEntityType.Prisim;
        #endregion

        #region Paint Line Renderer
        public void SetAdjustLine(float length = m_SubLineLength)
        {
            for (int i = 0; i < m_LineRenderers.Length; i++)
                PaintLine(i, length);
        }

        private void PaintLine(int i, float length)
        {
            Vector2 linePos = m_LineRenderers[i].transform.position;
            Vector2 lineDir = m_LineRenderers[i].transform.up;

            RaycastHit2D hit = Physics2D.Raycast(linePos, lineDir, length, RayManager.s_LaserHitableLayer);
            Vector2 secondPos = hit.collider is null ? linePos + lineDir * m_SubLineLength : hit.point;

            m_LineRenderers[i].positionCount = 2;
            m_LineRenderers[i].SetPosition(0, linePos);
            m_LineRenderers[i].SetPosition(1, secondPos);
        }
        #endregion

        #region Set Pos & Dir
        public void SetPosition(Vector2 pos, int rowNumber, int colNumber)
        {
            if (RowNumber != rowNumber || ColNumber != colNumber)
            {
                AudioManager.AudioManagerInstance.PlayOneShotUISE(m_ItemDragAudioKey);
                Position = pos;
                RowNumber = rowNumber;
                ColNumber = colNumber;
                transform.position = pos;
            }
            SetAdjustLine(Mathf.Infinity);
        }

        public void SetDirection(Vector2 pos)
        {
            Vector2 direction = (pos - (Vector2)transform.position).normalized;
            Vector2 discreteDirection = direction.DiscreteDirection(5);

            if (Direction != discreteDirection)
            {
                AudioManager.AudioManagerInstance.PlayOneShotUISE(m_ItemDragAudioKey);
                Direction = discreteDirection;
                transform.rotation = Quaternion.LookRotation(transform.forward, discreteDirection);
                SetAdjustLine(Mathf.Infinity);
            }
        }
        #endregion

        #region Play Animation
        public void PlayFixNoticeAnimation()
            => m_Animator.SetTrigger(m_FixedNoticeAnimationKey);

        public void PlayDestroyAnimation()
        {
            m_CircleCollider2D.enabled = false;

            for (int i = 0; i < m_SubRotationImage.Length; i++)
                m_SubRotationImage[i].SetActive(false);

            for (int i = 0; i < m_EjectionPortsTransform.Length; i++)
                m_EjectionPortsTransform[i].gameObject.SetActive(false);

            m_Animator.SetTrigger(m_DestroyAnimationKey);
            //애니메이션 끝나고 이벤트로 ReturnObject() 호출함
        }
        #endregion

        #region Play floating text
        public void PlayUsingCountDisplay()
            => StartCoroutine(UsingCountCoroutine(2));
        
        private IEnumerator UsingCountCoroutine(float time)
        {
            m_CanvasTransform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);

            if (RemainUsingCount - 1 < 0) Debug.LogError("[RemainUsingCount - 1] < 0 Error");
            m_CountText.fontSize = m_FontSizes[RemainUsingCount - 1];
            m_CountText.text = RemainUsingCount.ToString();
            m_CountText.gameObject.SetActive(true);
            m_CountText.rectTransform.anchoredPosition = m_TextInitPos;
            m_CountText.rectTransform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
            m_CountText.color = m_TextInitColor;

            Vector2 endPos = m_TextInitPos + new Vector2(0, 7.5f);

            float elapsedTime = 0;
            float t;
            while (elapsedTime <= time)
            {
                elapsedTime += Time.deltaTime;
                t = elapsedTime / time;

                m_CountText.color = Color.Lerp(m_TextInitColor, m_TextEndColor, t);
                m_CountText.rectTransform.anchoredPosition = Vector2.Lerp(m_TextInitPos, endPos, t);

                yield return null;
            }

            m_CountText.gameObject.SetActive(false);
            m_CountText.rectTransform.anchoredPosition = Vector2.zero;
        }
        #endregion

        public override void ReturnObject()
            => m_Pool.ReturnObject(this);
    }
}