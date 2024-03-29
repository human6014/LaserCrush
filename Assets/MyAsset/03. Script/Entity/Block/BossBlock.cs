using LaserCrush.Manager;
using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;

namespace LaserCrush.Entity.Block
{
    public class BossBlock : Block
    {
        [SerializeField] private Image m_BossIcon;
        [SerializeField] private Sprite[] m_BossBlockImages;

        private Coroutine m_ImageChangeCoroutine;
        private WaitForSeconds m_ImageChangeDuration = new WaitForSeconds(0.2f);

        public override int RowNumber { get => m_MatrixPos[0].RowNumber; }
        public override int ColNumber { get => m_MatrixPos[0].ColNumber; }
        public override bool IsBossBlock { get => true; }

        /// <summary>
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="rowNumber">rightBottom</param>
        /// <param name="colNumber">rightBottom</param>
        /// <param name="entityType"></param>
        /// <param name="itemType"></param>
        /// <param name="pos">(leftTop + rightBottom) / 2</param>
        /// <param name="playParticleAction"></param>
        public override void Init(int hp, int rowNumber, int colNumber, EEntityType entityType, DroppedItemType itemType, Vector2 pos, Action<Block> playParticleAction)
        {
            m_BossIcon.sprite = m_BossBlockImages[0];
            m_MatrixPos.Clear();
            m_MatrixPos.Add(new MatrixPos(rowNumber, colNumber));
            m_MatrixPos.Add(new MatrixPos(rowNumber, colNumber + 1));
            m_MatrixPos.Add(new MatrixPos(rowNumber + 1, colNumber));
            m_MatrixPos.Add(new MatrixPos(rowNumber + 1, colNumber + 1));

            base.InitSetting(hp, entityType, itemType, pos, playParticleAction);
        }

        protected override void SetHPText(int hp)
        {
            base.SetHPText(hp);

            if(m_ImageChangeCoroutine is not null)
                StopCoroutine(m_ImageChangeCoroutine);

            m_ImageChangeCoroutine = StartCoroutine(ImageChange());
        }

        private IEnumerator ImageChange()
        {
            m_BossIcon.sprite = m_BossBlockImages[1];
            yield return m_ImageChangeDuration;
            m_BossIcon.sprite = m_BossBlockImages[0];
        }        
    }
}  
