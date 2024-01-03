using System.Collections.Generic;
using UnityEngine;
using System;
using LaserCrush.Entity;
using TMPro.EditorUtilities;

namespace LaserCrush.Manager
{
    [Serializable]
    public class BlockManager
    {
        #region Variable
        [SerializeField] private Transform m_BlockTransform;
        [SerializeField] private GameObject m_BlockObject;
        [SerializeField] private Vector2 m_InitPos;
        [SerializeField] private Vector2 m_Offset;

        private Vector3 m_MoveDownVector;

        private ItemManager m_ItemManager;
        private List<Block> m_Blocks;
        private event Func<GameObject, GameObject> m_InstantiateFunc;
        private int m_WidthBlocksCapacity = 6;
        #endregion

        public void Init(Func<GameObject, GameObject> instantiateFunc, ItemManager itemManager)
        {
            m_Blocks = new List<Block>();
            m_InstantiateFunc = instantiateFunc;
            m_ItemManager = itemManager;

            m_MoveDownVector = new Vector3(0, m_Offset.y, 0);
        }

        public void GenerateBlock()
        {
            GameObject obj;
            Block block;
            /* todo
             * 여기서 GenerateBlockOffset를 호출해서 
             * 헤쉬셋을 순회하면서 해당 오프셋에 블럭 생성하면 됨
             */
            /*for (int i = 0; i < 4; i++)
            {
                obj = m_InstantiateFunc?.Invoke(m_BlockObject);
                obj.transform.SetParent(m_BlockTransform);

                block = obj.GetComponent<Block>();
                block.transform.position = new Vector3(m_InitPos.x + m_Offset.x * i, m_InitPos.y, 0);
                block.Init(GenerateBlockHP(), GenerateEntityType(), GenerateItemType(), RemoveBlock);
                m_Blocks.Add(block);
            }*/
            HashSet<int> index = GenerateBlockOffset();
            foreach (int i in index)
            {
                obj = m_InstantiateFunc?.Invoke(m_BlockObject);
                obj.transform.SetParent(m_BlockTransform);

                block = obj.GetComponent<Block>();
                block.transform.position = new Vector3(m_InitPos.x + m_Offset.x * i, m_InitPos.y, 0);
                block.Init(GenerateBlockHP(), GenerateEntityType(), GenerateItemType(), RemoveBlock);
                m_Blocks.Add(block);
            }
        }

        private void RemoveBlock(Block block)
        {
            //m_ItemManager.AddDroppedItem(block.DroppedItem);
            m_Blocks.Remove(block);
        }

        public void MoveDownAllBlocks()
        {
            for (int i = 0; i < m_Blocks.Count; i++)
            {
                m_Blocks[i].transform.position += m_MoveDownVector;
            }
        }

        /// <summary>
        /// 지금은 단순히 무작위 확률로 블럭 위치와 갯수를 생성
        /// </summary>
        /// <returns></returns>
        private HashSet<int> GenerateBlockOffset()
        {
            int randomSize = UnityEngine.Random.Range(1,7);//1~6사이 숫자
            HashSet<int> result = new HashSet<int>();

            while(result.Count < randomSize)
            {
                result.Add(UnityEngine.Random.Range(0, m_WidthBlocksCapacity));//0 ~ m_WidthBlocksCapacity 사이 숫자 생성
            }
            return result;
        }

        /// <summary>
        /// 3스테이지마다 가중치 부여
        /// 구간을 
        /// </summary>
        /// <returns></returns>
        private int GenerateBlockHP()
        {
            int end = ((GameManager.m_StageNum + 2) / 3) * 10;
            int start = end - (end / 2);
            return UnityEngine.Random.Range(start, end + 1);
        }

        /// <summary>
        /// 확률 표
        /// None = 50
        /// Energy = 30
        /// Prism_1 = 10
        /// Prism_2 = 10
        /// </summary>
        /// <returns></returns>
        private EItemType GenerateItemType()
        {
            //0~99사이 숫자 생성
            int prob = UnityEngine.Random.Range(0, 100);

            if (prob < 50)
            {
                return EItemType.None;
            }
            else if (prob < 80)
            {
                return EItemType.Energy;
            }
            else if (prob < 90)
            {
                return EItemType.Prism_1;
            }
            else
            {
                return EItemType.Prism_2;
            }
        }

        /// <summary>
        /// 확률 표
        /// 일반 블럭 = 50
        /// 반사 블럭 = 50
        /// </summary>
        /// <returns></returns>
        private EEntityType GenerateEntityType()
        {
            int prob = UnityEngine.Random.Range(0, 100);

            if (prob < 50)
            {
                return EEntityType.NormalBlock;
            }
            else
            {
                return EEntityType.ReflectBlock;
            }
        }
    }
}
