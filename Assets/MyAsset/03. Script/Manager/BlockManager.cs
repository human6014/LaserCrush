using System.Collections.Generic;
using UnityEngine;
using System;
using LaserCrush.Data;
using LaserCrush.Controller;
using LaserCrush.Entity;
using LaserCrush.Entity.Item;
using Random = UnityEngine.Random;

namespace LaserCrush.Manager
{
    [Serializable]
    public class BlockManager
    {
        #region Variable
        #region SerializeField
        [Header("Effect Pooling")]
        [SerializeField] private BlockParticleController m_BlockParticleController;

        [Header("Monobehaviour Reference")]
        [SerializeField] private UIManager m_UIManager;

        [Header("Block Reference")]
        [SerializeField] private ItemProbabilityData m_ItemProbabilityData;
        [SerializeField] private Transform m_DroppedItemTransform;
        [SerializeField] private Transform m_BlockTransform;
        [SerializeField] private GameObject m_BlockObject;

        [Header("Block Grid Instancing")]
        [SerializeField] private GridLineController m_GridLineController;
        [SerializeField] private Transform m_TopWall;
        [SerializeField] private Transform m_LeftWall;

        [Tooltip("블럭 최대 행 개수")]
        [Range(1, 40)]
        [SerializeField] private int m_MaxRowCount;

        [Tooltip("블럭 최대 열 개수")]
        [Range(1, 20)]
        [SerializeField] private int m_MaxColCount;

        [Header("Time Related")]
        [SerializeField] private float m_MoveDownTime;
        [SerializeField] private float m_GenerateTime;
        #endregion

        private ItemManager m_ItemManager;
        private List<Block> m_Blocks;

        private event Func<GameObject, GameObject> m_InstantiateFunc;
        private event Func<GameObject, Vector3, GameObject> m_InstantiatePosFunc;

        //유니티스럽게 바꾸면 좋을듯 인스펙터에서 수정할 수 있게
        //앞에서부터1~6
        private static readonly List<int> s_Probabilitytable = new List<int>() { 6, 20, 50, 50, 20, 5 };
        private static readonly int s_MaxWightSum = 151;
        //

        private float m_MoveDownElapsedTime;
        private float m_GenerateElapsedTime;

        private Vector2 m_CalculatedInitPos;
        private Vector2 m_CalculatedOffset;
        private Vector2 m_MoveDownVector;
        #endregion

        public void Init(Func<GameObject, GameObject> instantiateFunc,
                         Func<GameObject, Vector3, GameObject> instantiatePosFunc,
                         ItemManager itemManager)
        {
            m_Blocks = new List<Block>();
            m_InstantiateFunc = instantiateFunc;
            m_InstantiatePosFunc = instantiatePosFunc;

            m_ItemManager = itemManager;
            m_ItemManager.CheckAvailablePosFunc += CheckAvailablePos;

            Vector3 blockSize = CalculateGridRowCol();

            m_GridLineController.SetGridLineObjects(m_CalculatedInitPos, m_CalculatedOffset, m_MaxRowCount, m_MaxColCount);
            m_GridLineController.OnOffGridLine(false);

            m_BlockParticleController.Init(blockSize);
        }

        private Result CheckAvailablePos(Vector3 pos)
        {
            if (!InBoardArea(pos)) return new Result(false, Vector3.zero, 0, 0);
            Vector2 newPos = GetItemGridPos(pos, out int rowNumber, out int colNumber);

            Result result = new Result(
                    isAvailable: false,
                    itemGridPos: Vector3.zero,
                    rowNumber: rowNumber,
                    colNumber: colNumber
                    );

            foreach (Block block in m_Blocks)
            {
                if (rowNumber == block.RowNumber &&
                    colNumber == block.ColNumber)
                    return result;
            }

            result.m_IsAvailable = true;
            result.m_ItemGridPos = newPos;
            return result;
        }

        public bool IsGameOver()
        {
            int MaxRow = -1;
            foreach (Block block in m_Blocks)
            {
                if (block.RowNumber > MaxRow) MaxRow = block.RowNumber;
            }

            if (MaxRow == m_MaxRowCount - 1) return true;
            return false;
        }

        private bool InBoardArea(Vector2 pos)
        {
            return Mathf.Abs(pos.x) <= Mathf.Abs(m_LeftWall.position.x) - 4 &&
                pos.y >= -m_TopWall.position.y + 7 &&
                pos.y <= m_TopWall.position.y;
        }

        private Vector3 GetItemGridPos(Vector3 pos, out int rowNumber, out int colNumber)
        {
            float differX = pos.x - m_LeftWall.position.x;
            float differY = m_TopWall.position.y - pos.y;

            rowNumber = (int)(differY / m_CalculatedOffset.y);
            colNumber = (int)(differX / m_CalculatedOffset.x);

            float newPosX = m_CalculatedInitPos.x + m_CalculatedOffset.x * colNumber;
            float newPosY = m_CalculatedInitPos.y - m_CalculatedOffset.y * rowNumber;

            return new Vector3(newPosX, newPosY, 0);
        }

        private Vector3 CalculateGridRowCol()
        {
            float height = m_LeftWall.localScale.y - 6;
            float width = m_TopWall.localScale.x - 4;

            float blockHeight = height / m_MaxRowCount;
            float blockWidth = width / m_MaxColCount;

            m_CalculatedInitPos = new Vector2(m_LeftWall.position.x + blockWidth * 0.5f + 2, m_TopWall.position.y - blockHeight * 0.5f - 2);
            m_CalculatedOffset = new Vector2(blockWidth, blockHeight);

            Vector3 size = new Vector3(blockWidth, blockHeight, 1);
            m_BlockObject.transform.localScale = size;

            m_MoveDownVector = new Vector2(0, -m_CalculatedOffset.y);

            return size;
        }

        public bool GenerateBlock()
        {
            if (m_GenerateElapsedTime == 0)
            {
                GameObject obj;
                GameObject itemObject;
                DroppedItem item;
                Block block;

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

                    block.Init(GenerateBlockHP(), 0, i, m_GenerateTime, GenerateEntityType(), item, RemoveBlock);
                    m_Blocks.Add(block);
                }
            }

            m_GenerateElapsedTime += Time.deltaTime;
            if(m_GenerateElapsedTime >= m_GenerateTime)
            {
                m_GenerateElapsedTime = 0;
                return true;
            }
            return false;
        }

        private void RemoveBlock(Block block, DroppedItem droppedItem)
        {
            if (droppedItem is not null)
            {
                droppedItem.gameObject.SetActive(true);
                droppedItem.transform.position = block.transform.position;
                m_ItemManager.AddDroppedItem(droppedItem);
            }
            m_BlockParticleController.PlayParticle(block.transform.position, block.GetEEntityType());
            m_UIManager.SetScore(block.BlockScore);
            m_Blocks.Remove(block);
        }

        public bool MoveDownAllBlocks()
        {
            if (m_MoveDownElapsedTime == 0)
            {
                for (int i = 0; i < m_Blocks.Count; i++)
                {
                    m_Blocks[i].MoveDown(m_MoveDownVector, m_MoveDownTime);
                    m_ItemManager.CheckDuplicatePosWithBlock(m_Blocks[i].RowNumber, m_Blocks[i].ColNumber);
                }
            }

            m_MoveDownElapsedTime += Time.deltaTime;
            if (m_MoveDownElapsedTime >= m_MoveDownTime)
            {
                m_MoveDownElapsedTime = 0;
                return true;
            }
            else return false;
        }

        /// <summary>
        /// 지금은 단순히 무작위 확률로 블럭 위치와 갯수를 생성
        /// </summary>
        /// <returns></returns>
        private HashSet<int> GenerateBlockOffset()
        {
            int randomSize = GetWeightedRandomNum();//1 ~ m_MaxColCount사이 숫자
            HashSet<int> result = new HashSet<int>();

            while (result.Count < randomSize)
            {
                result.Add(Random.Range(0, m_MaxColCount));
            }
            return result;
        }

        /// <summary>
        /// 5스테이지마다 가중치 부여
        /// 각 블럭의 생성 범위는  최대 최소차이가 10%
        /// </summary>
        /// <returns></returns>
        private int GenerateBlockHP()
        {
            int end = ((GameManager.s_StageNum + 1) / 2) * 5;
            int start = end - (end / 10);
            return Random.Range(start, end + 1) * 100;
        }

        /// <summary>
        /// 확률 표
        /// 일반 블럭 = 50
        /// 반사 블럭 = 50
        /// </summary>
        /// <returns></returns>
        private EEntityType GenerateEntityType()
        {
            return Random.Range(0, 100) < 50 ? EEntityType.NormalBlock : EEntityType.ReflectBlock;
        }

        public void FeverTime()
        {
            foreach (var block in m_Blocks)
                block.GetDamage(int.MaxValue);
        }

        public void ResetGame()
        {
            foreach (var block in m_Blocks)
                block.DestoryReset();

            m_Blocks.Clear();
        }

        private int GetWeightedRandomNum()
        {
            int randomSize = Random.Range(1, s_MaxWightSum);

            for (int i = 0; i < s_Probabilitytable.Count; i++)
            {
                if (randomSize < s_Probabilitytable[i])
                    return i + 1;

                randomSize -= s_Probabilitytable[i];
            }
            return s_Probabilitytable.Count - 1;
        }
    }
}
