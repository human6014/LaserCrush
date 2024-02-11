using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LaserCrush.Data;
using LaserCrush.Manager;
using LaserCrush.Extension;
using LaserCrush.Entity.Interface;
using System;
using static UnityEditor.PlayerSettings;

namespace LaserCrush.Entity
{
    public class BossBlock : Block
    {
        /// <summary>
        /// 
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
        }
    }
}
