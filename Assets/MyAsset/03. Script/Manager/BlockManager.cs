using System.Collections.Generic;
using UnityEngine;
using System;
using LaserCrush.Entity;
using LaserCrush.Data;
using Random = UnityEngine.Random;

namespace LaserCrush.Manager
{
    [Serializable]
    public class BlockManager
    {
        #region Variable
        #region SerializeField
        [Header("Block Reference")]
        [SerializeField] private ItemProbabilityData m_ItemProbabilityData;
        [SerializeField] private Transform m_DroppedItemTransform;
        [SerializeField] private Transform m_BlockTransform;
        [SerializeField] private GameObject m_BlockObject;

        [Header("Block Grid Instancing")]
        [SerializeField] private Transform m_TopWall;
        [SerializeField] private Transform m_LeftWall;

        [Tooltip("블럭 최대 행 개수")] [Range(1, 40)]
        [SerializeField] private int m_MaxRowCount;

        [Tooltip("블럭 최대 열 개수")] [Range(1, 20)]
        [SerializeField] private int m_MaxColCount;
        #endregion
        private Vector2 m_CalculatedInitPos;
        private Vector2 m_CalculatedOffset;
        

        private Vector3 m_MoveDownVector;

        private ItemManager m_ItemManager;
        private List<Block> m_Blocks;
        private event Func<GameObject, GameObject> m_InstantiateFunc;
        private event Func<GameObject, Vector3, GameObject> m_InstantiatePosFunc;
        #endregion

        public void Init(Func<GameObject, GameObject> instantiateFunc, 
                         Func<GameObject, Vector3, GameObject> instantiatePosFunc, 
                         ItemManager itemManager)
        {
            m_Blocks = new List<Block>();
            m_InstantiateFunc = instantiateFunc;
            m_InstantiatePosFunc = instantiatePosFunc;
            m_ItemManager = itemManager;

            CalculateRowCol();
        }

        private void CalculateRowCol()
        {
            float height = m_LeftWall.localScale.y - 6;
            float width = m_TopWall.localScale.x - 6;

            float blockHeight = height / m_MaxRowCount;
            float blockWidth = width / m_MaxColCount;

            m_CalculatedInitPos = new Vector2(m_LeftWall.position.x + blockWidth * 0.5f + 2, m_TopWall.position.y - blockHeight * 0.5f - 2);
            m_CalculatedOffset = new Vector2(blockWidth, blockHeight);

            m_BlockObject.transform.localScale = new Vector3(blockWidth, blockHeight, 1);

            m_MoveDownVector = new Vector3(0, m_CalculatedOffset.y, 0);
        }

        public void GenerateBlock()
        {
            GameObject obj;
            GameObject itemObject;
            Block block;
            DroppedItem item;

            HashSet<int> index = GenerateBlockOffset();
            foreach (int i in index)
            {
                obj = m_InstantiatePosFunc?.Invoke(m_BlockObject, new Vector3(m_CalculatedInitPos.x + m_CalculatedOffset.x * i, m_CalculatedInitPos.y, 0));
                obj.transform.SetParent(m_BlockTransform);

                block = obj.GetComponent<Block>();

                item = null;
                if (m_ItemProbabilityData.TryGetItemObject(out GameObject itemPrefab))
                {
                    itemObject = m_InstantiateFunc?.Invoke(itemPrefab);
                    itemObject.SetActive(false);
                    itemObject.transform.SetParent(m_DroppedItemTransform);
                    item = itemObject.GetComponent<DroppedItem>();
                }
                
                block.Init(GenerateBlockHP(), GenerateEntityType(), item, RemoveBlock);
                m_Blocks.Add(block);
            }
        }

        private void RemoveBlock(Block block, DroppedItem droppedItem)
        {
            if (droppedItem is not null)
            {
                droppedItem.gameObject.SetActive(true);
                droppedItem.transform.position = block.transform.position;
                m_ItemManager.AddDroppedItem(droppedItem);
            }
            m_Blocks.Remove(block);
        }

        public void MoveDownAllBlocks()
        {
            for (int i = 0; i < m_Blocks.Count; i++)
            {
                m_Blocks[i].transform.position -= m_MoveDownVector;
            }
        }

        /// <summary>
        /// 지금은 단순히 무작위 확률로 블럭 위치와 갯수를 생성
        /// </summary>
        /// <returns></returns>
        private HashSet<int> GenerateBlockOffset()
        {
            //int randomSize = Random.Range(1,7);//1~6사이 숫자
            int randomSize = Random.Range(1, m_MaxColCount + 1);//1~6사이 숫자
            HashSet<int> result = new HashSet<int>();

            while(result.Count < randomSize)
            {
                //result.Add(Random.Range(0, m_WidthBlocksCapacity));//0 ~ m_WidthBlocksCapacity 사이 숫자 생성
                result.Add(Random.Range(0, m_MaxColCount));
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
            return Random.Range(start, end + 1);
        }

        /// <summary>
        /// 확률 표
        /// 일반 블럭 = 50
        /// 반사 블럭 = 50
        /// </summary>
        /// <returns></returns>
        private EEntityType GenerateEntityType()
        {
            if (Random.Range(0, 100) < 50) return EEntityType.NormalBlock;
            else return EEntityType.ReflectBlock;
        }
    }
}
