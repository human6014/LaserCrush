using System.Collections.Generic;
using UnityEngine;
using System;
using LaserCrush.Data;
using LaserCrush.Controller;
using LaserCrush.Entity;
using LaserCrush.Entity.Item;
using LaserCrush.Entity.Block;
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
        [SerializeField] private BlockProbabilityData m_BlockProbabilityData;
        [SerializeField] private ItemProbabilityData m_ItemProbabilityData;
        [SerializeField] private Transform m_DroppedItemTransform;
        [SerializeField] private Transform m_BlockTransform;
        [SerializeField] private Block m_BlockPrefab;
        [SerializeField] private BossBlock m_BossBlockPrefab;

        [Header("Block Grid Instancing")]
        [SerializeField] private GridLineController m_GridLineController;
        [SerializeField] private Transform m_TopWall;
        [SerializeField] private Transform m_LeftWall;

        [SerializeField] private int m_BlockPoolingCount = 30;
        [SerializeField] private int m_BossBlockPoolingCount = 1;
        [Tooltip("블럭 최대 행 개수")]
        [Range(1, 40)]
        [SerializeField] private int m_MaxRowCount;

        [Tooltip("블럭 최대 열 개수")]
        [Range(1, 20)]
        [SerializeField] private int m_MaxColCount;

        [Header("Time Related")]
        [SerializeField] private float m_MoveDownTime;
        [SerializeField] private float m_GenerateTime;
        [SerializeField] private float m_GenerateBossTime;
        #endregion

        private ObjectPoolManager.PoolingObject m_BossBlockPool;
        private ObjectPoolManager.PoolingObject m_BlockPool;
        private ObjectPoolManager.PoolingObject[] m_DroppedItemPool;

        private ItemManager m_ItemManager;
        private BossBlock m_BossBlock;
        private List<Block> m_Blocks;

        private Vector2 m_CalculatedInitPos;
        private Vector2 m_CalculatedOffset;
        private Vector2 m_MoveDownVector;

        private const string m_ItemDroppedAudioKey = "ItemDropped";

        private float m_MoveDownElapsedTime;
        private float m_GenerateElapsedTime;
        private float m_BlockHPWeight = 0.1f;

        private int m_BossBlockAge;
        #endregion

        #region Init
        public void Init(ItemManager itemManager)
        {
            m_Blocks = new List<Block>();
            m_BossBlock = null;
            m_ItemManager = itemManager;
            m_ItemManager.CheckAvailablePosFunc += CheckAvailablePos;

            Vector3 blockSize = CalculateGridRowColAndGetSize();
            m_BlockPrefab.transform.localScale = blockSize;
            m_BossBlockPrefab.transform.localScale = blockSize * 2;

            m_GridLineController.SetGridLineObjects(m_CalculatedInitPos, m_CalculatedOffset, m_MaxRowCount, m_MaxColCount);
            m_GridLineController.OnOffGridLine(false);

            m_BlockParticleController.Init(blockSize);
            AssignPoolingObject();
            LoadBlockData();
        }

        private void AssignPoolingObject()
        {
            m_BlockPool = ObjectPoolManager.Register(m_BlockPrefab, m_BlockTransform);
            m_BlockPool.GenerateObj(Mathf.Max(DataManager.GameData.m_Blocks.Count, m_BlockPoolingCount));

            m_BossBlockPool = ObjectPoolManager.Register(m_BossBlockPrefab, m_BlockTransform);
            m_BossBlockPool.GenerateObj(m_BossBlockPoolingCount);

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
                if(!block.IsAvailablePos(rowNumber, colNumber))
                    return result;
            }

            result.m_IsAvailable = true;
            result.m_ItemGridPos = newPos;
            return result;
        }

        public bool IsGameOver()
        {
            foreach (Block block in m_Blocks)
            {
                if (block.IsGameOver(m_MaxRowCount - 1)) return true;
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
            //열번호 0, end번호 (벽이랑 붙어있는) 블럭 틈새 안줄려고 1씩 더함
            float height = m_LeftWall.localScale.y - m_TopWall.localScale.y * 2 + 1;
            float width = m_TopWall.localScale.x - m_LeftWall.localScale.x * 2 + 1;

            float blockHeight = height / m_MaxRowCount;
            float blockWidth = width / m_MaxColCount;

            m_CalculatedInitPos = new Vector2(m_LeftWall.position.x + m_LeftWall.localScale.x * 0.5f + blockWidth * 0.5f, m_TopWall.position.y - m_TopWall.localScale.y * 0.5f - blockHeight * 0.5f);
            m_CalculatedOffset = new Vector2(blockWidth, blockHeight);

            Vector3 size = new Vector3(blockWidth, blockHeight, 1);

            m_MoveDownVector = new Vector2(0, -m_CalculatedOffset.y);
            return size;
        }
        #endregion

        #region Block Generate
        private Vector3 GetRowColPosition(int row, int col)
        {
            return new Vector3(m_CalculatedInitPos.x + m_CalculatedOffset.x * col, 
                               m_CalculatedInitPos.y - m_CalculatedOffset.y * row, 
                               0);
        }

        public bool GenerateBlock(int size)
        {
            if (m_GenerateElapsedTime == 0)
            {
                for (int j = 0; j < size; j++)
                {
                    bool flag = false;

                    HashSet<int> index = GenerateBlockOffset();
                    foreach (int i in index)
                    {
                        Vector3 pos = GetRowColPosition(j, i);
                        int itemIndex;
                        if (!flag)
                        {
                            itemIndex = 1;
                            flag = true;
                        }
                        else itemIndex = m_ItemProbabilityData.GetItemIndex();

                        InstantiateBlock(GenerateBlockHP(), j, i, GenerateEntityType(), (DroppedItemType)itemIndex, pos, false, false);
                    }
                }
            }

            m_GenerateElapsedTime += Time.deltaTime;
            if (m_GenerateElapsedTime >= m_GenerateTime)
            {
                m_GenerateElapsedTime = 0;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 지금은 무조건 에너지 아이템 드롭하게 만들어둠
        /// </summary>
        /// <returns></returns>
        public bool GenerateBossBlock()
        {
            if (m_GenerateElapsedTime == 0)
            {
                Vector3 pos = (GetRowColPosition(0,2) + GetRowColPosition(1, 3)) * 0.5f;

                InstantiateBlock(GenerateBossBlockHP(), 0, 2, GenerateEntityType(), DroppedItemType.Energy, pos, true, false);
            }

            m_GenerateElapsedTime += Time.deltaTime;
            if (m_GenerateElapsedTime >= m_GenerateBossTime)
            {
                m_GenerateElapsedTime = 0;
                m_BossBlockAge = 0;
                return true;
            }
            return false;
        }

        private void InstantiateBlock(int hp, int row, int col, EEntityType entityType, DroppedItemType droppedItemType, Vector2 pos, bool isBossBlock, bool isLoadData)
        {
            Block block;
            if (!isBossBlock) block = (Block)m_BlockPool.GetObject(true);
            else
            {
                block = (BossBlock)m_BossBlockPool.GetObject(true);
                m_BossBlock = (BossBlock)block;
            }
     
            if (!isLoadData)
            {
                int additionalHP = (int)(hp * 0.25f);
                hp += entityType == EEntityType.NormalBlock ? -additionalHP : additionalHP;
            }

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
            int randomSize = m_BlockProbabilityData.GetBlockIndex();
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
            int end = (int)((GameManager.StageNum + 1) * 0.5f * 5 * CalculateBlockStageHPWeight());
            int start = end - (end / 10);

            return Random.Range(start, end + 1) * 100;
        }

        private int GenerateBossBlockHP()
        {
            int end = (int)((GameManager.StageNum + 1) * 0.5f * 5 * 3.8f * 2.5f * CalculateBlockStageHPWeight());
            int start = end - (end / 10);

            return Random.Range(start, end + 1) * 100;
        }

        private float CalculateBlockStageHPWeight()
        {
            return (GameManager.StageNum / 100) * m_BlockHPWeight + 1;
        }

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

            if (block.IsBossBlock)
            {
                m_BossBlockPool.ReturnObject(block);
                m_BossBlock = null;
                m_BossBlockAge = 0;
            }
            else m_BlockPool.ReturnObject(block);

            m_Blocks.Remove(block);
        }  

        public bool MoveDownAllBlocks(int step)
        {
            if (m_MoveDownElapsedTime == 0)
            {
                for (int i = 0; i < m_Blocks.Count; i++)
                {
                    m_Blocks[i].MoveDown(m_MoveDownVector * step , m_MoveDownTime, step);
                    m_ItemManager.CheckDuplicatePosWithBlock(m_Blocks[i]);
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

        public void AddBossBlockAge()
        {
            if (m_BossBlock != null)
                m_BossBlockAge++;
        }

        public bool IsBossSkill()
        {
            if (m_BossBlock != null && m_BossBlockAge == 3) return true;
            return false;
        }

        #region Load & Save
        private void LoadBlockData()
        {
            foreach (Data.Json.BlockData blockData in DataManager.GameData.m_Blocks)
            {
                InstantiateBlock(blockData.m_HP,
                                 blockData.m_RowNumber,
                                 blockData.m_ColNumber,
                                 blockData.m_EntityType,
                                 blockData.m_HasItemType,
                                 blockData.m_Position,
                                 blockData.m_IsBossBlock,
                                 true);
            }
            m_BossBlockAge = DataManager.GameData.m_BossAge;
        }

        public void SaveAllData()
        {
            DataManager.GameData.m_Blocks.Clear();
            Data.Json.BlockData blockData;
            foreach (Block block in m_Blocks)
            {
                blockData = new Data.Json.BlockData(
                        row: block.RowNumber,
                        col: block.ColNumber,
                        hp: block.CurrentHP,
                        isBoss: block.IsBossBlock,
                        pos: block.Position,
                        entityType: block.GetEEntityType(),
                        itemType: block.ItemType);

                DataManager.GameData.m_Blocks.Add(blockData);
            }
            DataManager.GameData.m_BossAge = m_BossBlockAge;
        }
        #endregion

        public void ResetGame()
        {
            m_MoveDownElapsedTime = 0;
            m_GenerateElapsedTime = 0;
            m_BossBlock = null;
            m_BossBlockAge = 0;
            foreach (Block block in m_Blocks)
            {
                block.DestroyImmediate();
                if (!block.IsBossBlock) m_BlockPool.ReturnObject(block);
                else m_BossBlockPool.ReturnObject(block);
            }

            m_Blocks.Clear();
        }
    }
}
