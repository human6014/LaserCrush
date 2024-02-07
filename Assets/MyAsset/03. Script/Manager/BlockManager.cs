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
        [SerializeField] private int[] m_DroppedItemPoolingCount;

        [Header("Monobehaviour Reference")]
        [SerializeField] private UIManager m_UIManager;

        [Header("Block Reference")]
        [SerializeField] private ItemProbabilityData m_ItemProbabilityData;
        [SerializeField] private Transform m_DroppedItemTransform;
        [SerializeField] private Transform m_BlockTransform;
        [SerializeField] private Block m_Block;

        [Header("Block Grid Instancing")]
        [SerializeField] private GridLineController m_GridLineController;
        [SerializeField] private Transform m_TopWall;
        [SerializeField] private Transform m_LeftWall;

        [SerializeField] private int m_PoolingCount = 30;

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

        private ObjectPoolManager.PoolingObject m_BlockPool;
        private ObjectPoolManager.PoolingObject[] m_DroppedItemPool;

        private ItemManager m_ItemManager;
        private List<Block> m_Blocks;

        private readonly List<int> s_Probabilitytable = new List<int>() { 6, 25, 45, 60, 45, 20 };
        private readonly int s_MaxWightSum = 201;
        //3.86개가 한 턴에 기댓값

        private const string m_ItemDroppedAudioKey = "ItemDropped";

        private float m_MoveDownElapsedTime;
        private float m_GenerateElapsedTime;

        private Vector2 m_CalculatedInitPos;
        private Vector2 m_CalculatedOffset;
        private Vector2 m_MoveDownVector;
        #endregion

        #region Init
        public void Init(ItemManager itemManager)
        {
            m_Blocks = new List<Block>();

            m_ItemManager = itemManager;
            m_ItemManager.CheckAvailablePosFunc += CheckAvailablePos;

            Vector3 blockSize = CalculateGridRowColAndGetSize();
            m_Block.transform.localScale = blockSize;

            m_GridLineController.SetGridLineObjects(m_CalculatedInitPos, m_CalculatedOffset, m_MaxRowCount, m_MaxColCount);
            m_GridLineController.OnOffGridLine(false);

            m_BlockParticleController.Init(blockSize);

            AssignPoolingObject();
            LoadBlocks();
        }

        private void AssignPoolingObject()
        {
            m_BlockPool = ObjectPoolManager.Register(m_Block, m_BlockTransform);
            m_BlockPool.GenerateObj(Mathf.Max(DataManager.GameData.m_Blocks.Count, m_PoolingCount));

            m_DroppedItemPool = new ObjectPoolManager.PoolingObject[m_DroppedItemPoolingCount.Length];
            for (int i = 0; i < m_DroppedItemPool.Length; i++)
            {
                m_DroppedItemPool[i] = ObjectPoolManager.Register(m_ItemProbabilityData.DroppedItems[i].GetComponent<PoolableMonoBehaviour>(), m_DroppedItemTransform);
                m_DroppedItemPool[i].GenerateObj(m_DroppedItemPoolingCount[i]);
            }
        }
        #endregion

        #region Grid Related
        private Result CheckAvailablePos(Vector3 pos)
        {
            Vector2 newPos = GetItemGridNumberAndPos(pos, out int rowNumber, out int colNumber);

            Result result = new Result(
                    isAvailable: false,
                    itemGridPos: Vector3.zero,
                    rowNumber: rowNumber,
                    colNumber: colNumber
                    );

            if (rowNumber == 0) return result;

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
            int maxRow = -1;
            foreach (Block block in m_Blocks)
            {
                maxRow = Mathf.Max(maxRow, block.RowNumber);
                if (maxRow >= m_MaxRowCount - 1) return true;
            }
            return false;
        }

        private Vector3 GetItemGridNumberAndPos(Vector3 pos, out int rowNumber, out int colNumber)
        {
            float differX = pos.x - m_LeftWall.position.x;
            float differY = m_TopWall.position.y - pos.y;

            rowNumber = Mathf.Clamp((int)(differY / m_CalculatedOffset.y), 0, m_MaxRowCount - 1);
            colNumber = Mathf.Clamp((int)(differX / m_CalculatedOffset.x), 0, m_MaxColCount - 1);

            float newPosX = m_CalculatedInitPos.x + m_CalculatedOffset.x * colNumber;
            float newPosY = m_CalculatedInitPos.y - m_CalculatedOffset.y * rowNumber;

            return new Vector3(newPosX, newPosY, 0);
        }

        private Vector3 CalculateGridRowColAndGetSize()
        {
            float height = m_LeftWall.localScale.y - 6;
            float width = m_TopWall.localScale.x - 4;

            float blockHeight = height / m_MaxRowCount;
            float blockWidth = width / m_MaxColCount;

            m_CalculatedInitPos = new Vector2(m_LeftWall.position.x + blockWidth * 0.5f + 2, m_TopWall.position.y - blockHeight * 0.5f - 2);
            m_CalculatedOffset = new Vector2(blockWidth, blockHeight);

            Vector3 size = new Vector3(blockWidth, blockHeight, 1);

            m_MoveDownVector = new Vector2(0, -m_CalculatedOffset.y);

            return size;
        }
        #endregion

        #region Block Generate
        public bool GenerateBlock()
        {
            bool flag = false;
            if (m_GenerateElapsedTime == 0)
            {
                HashSet<int> index = GenerateBlockOffset();
                foreach (int i in index)
                {
                    Vector3 pos = new Vector3(m_CalculatedInitPos.x + m_CalculatedOffset.x * i, m_CalculatedInitPos.y, 0);
                    int itemIndex;
                    if (!flag)
                    {
                        itemIndex = 1;
                        flag = true;
                    }
                    else itemIndex = m_ItemProbabilityData.GetItemIndex();

                    InstantiateBlock(GenerateBlockHP(), 0, i, GenerateEntityType(), (DroppedItemType)itemIndex, pos);
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

        private void InstantiateBlock(int hp, int row, int col, EEntityType entityType, DroppedItemType droppedItemType, Vector2 pos)
        {
            Block block = (Block)m_BlockPool.GetObject(true);
            block.transform.position = pos;
            block.Init(hp, row, col, entityType, droppedItemType, pos, RemoveBlock);
            m_Blocks.Add(block);
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

        /// <summary>
        /// 5스테이지마다 가중치 부여
        /// 각 블럭의 생성 범위는  최대 최소차이가 10%
        /// </summary>
        /// <returns></returns>
        private int GenerateBlockHP()
        {
            int end = ((GameManager.StageNum + 1) / 2) * 5;
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
        #endregion

        private void RemoveBlock(Block block)
        {
            int typeIndex = (int)block.ItemType;
            if (typeIndex != 0)
            {
                AudioManager.AudioManagerInstance.PlayOneShotUISE(m_ItemDroppedAudioKey);

                DroppedItem droppedItem = (DroppedItem)m_DroppedItemPool[typeIndex - 1].GetObject(true);
                droppedItem.transform.position = block.Position;
                droppedItem.Init(m_DroppedItemPool[typeIndex - 1].ReturnObject);
                m_ItemManager.AddDroppedItem(droppedItem);
            }

            m_BlockParticleController.PlayParticle(block.Position, block.GetEEntityType());
            m_UIManager.SetScore(block.Score);
            m_BlockPool.ReturnObject(block);
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
            return false;
        }

        #region Load & Save
        private void LoadBlocks()
        {
            foreach (Data.Json.BlockData blockData in DataManager.GameData.m_Blocks)
            {
                InstantiateBlock(blockData.m_HP,
                                 blockData.m_RowNumber,
                                 blockData.m_ColNumber,
                                 blockData.m_EntityType,
                                 blockData.m_HasItemType,
                                 blockData.m_Position);
            }
        }

        public void SaveAllData()
        {
            DataManager.GameData.m_Blocks.Clear();
            Data.Json.BlockData blockData;
            foreach (Block block in m_Blocks)
            {
                blockData = new Data.Json.BlockData(
                    row : block.RowNumber, 
                    col : block.ColNumber, 
                    hp : block.CurrentHP, 
                    pos : block.Position,
                    entityType : block.GetEEntityType(),
                    itemType : block.ItemType);

                DataManager.GameData.m_Blocks.Add(blockData);
            }
        }
        #endregion

        public void ResetGame()
        {
            m_MoveDownElapsedTime = 0;
            m_GenerateElapsedTime = 0;

            foreach (var block in m_Blocks)
            {
                block.ImmediatelyReset();
                m_BlockPool.ReturnObject(block);
            }

            m_Blocks.Clear();
        }
    }
}
