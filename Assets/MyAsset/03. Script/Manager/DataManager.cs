using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using LaserCrush.Entity;
using LaserCrush.Entity.Item;
using LaserCrush.Entity.Block;
using LaserCrush.Data.Json;

namespace LaserCrush.Data.Json
{
    [Serializable]
    public struct BlockData
    {
        public int m_RowNumber;
        public int m_ColNumber;
        public int m_HP;
        public bool m_IsBossBlock;
        public Vector2 m_Position;
        public EEntityType m_EntityType;
        public DroppedItemType m_HasItemType;

        public BlockData(int row, int col, int hp, bool isBoss, Vector2 pos, EEntityType entityType, DroppedItemType itemType)
        {
            m_RowNumber = row;
            m_ColNumber = col;
            m_HP = hp;
            m_IsBossBlock = isBoss;
            m_Position = pos;
            m_EntityType = entityType;
            m_HasItemType = itemType;
        }
    }

    [Serializable]
    public struct ItemData
    {
        public int m_RowNumber;
        public int m_ColNumber;
        public int m_RemainUsingCount;
        public Vector2 m_Position;
        public Vector2 m_Direction;
        public ItemType m_ItemType;

        public ItemData(int row, int col, int count , Vector2 pos, Vector2 dir, ItemType type)
        {
            m_RowNumber = row;
            m_ColNumber = col;
            m_Position = pos;
            m_Direction = dir;
            m_ItemType = type;
            m_RemainUsingCount = count;
        }
    }

    [Serializable]
    public class GameData
    {
        public bool m_IsGameOver;

        public int m_BestScore;
        public int m_CurrentScore;
        public int m_StageNumber;
        public int m_BossAge;

        public int m_Prism1Count;
        public int m_Prism2Count;
        public int m_Prism3Count;
        public int m_Prism4Count;
        public int m_Damage;

        public Vector2 m_LauncherPos;
        public Vector2 m_LauncherDir;

        public List<BlockData> m_Blocks;
        public List<ItemData> m_InstalledItems;

        public GameData(bool isGameOver, int bestScore, int currentScore, int stageNumber, int bossAge,
                        int prism1Count, int prism2Count, int prism3Count, int prism4Count, int damage,
                        Vector2 launcherPos, Vector2 launcherDir,
                        List<BlockData> blocks, List<ItemData> items)
        {
            m_IsGameOver = isGameOver;

            m_BestScore = bestScore;
            m_CurrentScore = currentScore;
            m_StageNumber = stageNumber;
            m_BossAge = bossAge;

            m_Prism1Count = prism1Count;
            m_Prism2Count = prism2Count;
            m_Prism3Count = prism3Count;
            m_Prism4Count = prism4Count;
            m_Damage = damage;

            m_LauncherPos = launcherPos;
            m_LauncherDir = launcherDir;

            m_Blocks = blocks;
            m_InstalledItems = items;
        }
    }

    [Serializable]
    public class SettingData
    {
        public float m_MasterSound;
        public float m_BGMSound;
        public float m_SESound;

        public SettingData(float masterSound, float bgmSound, float seSound)
        {
            m_MasterSound = masterSound;
            m_BGMSound = bgmSound;
            m_SESound = seSound;
        }
    }
}
namespace LaserCrush.Manager
{
    public static class DataManager
    {
        #region Value
        private static GameData s_GameData;
        private static SettingData s_SettingData;

        public static readonly string s_DefaultPath = Application.persistentDataPath + "/";
        public static readonly string s_GameDataFileName = "GameData";
        public static readonly string s_SettingDataFileName = "SettingData";
        #endregion

        #region Property
        public static GameData GameData { get => s_GameData; private set => s_GameData = value; }
        public static SettingData SettingData { get => s_SettingData; private set => s_SettingData = value; }
        #endregion

        #region Init
        public static bool InitLoadData()
        {
            bool hasGameData = LoadGameData();
            bool hasSettingData = LoadSettingData();

            if (hasGameData && hasSettingData) return true;
            else
            {
                InitDataSetting();
                SaveGameData();
                SaveSettingData();
                return false;
            }
        }

        public static void InitDataSetting()
        {
            s_GameData = new GameData(
                isGameOver : false,
                bestScore: 0,
                currentScore: 0,
                stageNumber: 1,
                bossAge : 0,
                prism1Count: 1,
                prism2Count: 1,
                prism3Count: 1,
                prism4Count: 0,
                damage: 5,
                launcherPos: new Vector2(0, -57), 
                launcherDir: Vector2.zero,
                blocks: new List<BlockData>(),
                items: new List<ItemData>()
            );

            s_SettingData = new SettingData(0, 0, 0);
        }
        #endregion

        #region Save
        public static void SaveGameData()
        {
            string gameData = JsonUtility.ToJson(s_GameData, true);

            File.WriteAllText(s_DefaultPath + s_GameDataFileName, ToBase64String(gameData));
        }

        public static void SaveSettingData()
        {
            string settingData = JsonUtility.ToJson(s_SettingData, true);

            File.WriteAllText(s_DefaultPath + s_SettingDataFileName, ToBase64String(settingData));
        }

        private static string ToBase64String(string data)
        {
            byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
            string code = Convert.ToBase64String(dataBytes);

            return code;
        }
        #endregion

        #region Load
        private static bool LoadGameData()
        {
            if (!File.Exists(s_DefaultPath + s_GameDataFileName)) return false;

            string gameData = File.ReadAllText(s_DefaultPath + s_GameDataFileName);
            s_GameData = JsonUtility.FromJson<GameData>(FromBase64String(gameData));

            return true;
        }

        private static bool LoadSettingData()
        {
            if (!File.Exists(s_DefaultPath + s_SettingDataFileName)) return false;

            string settingData = File.ReadAllText(s_DefaultPath + s_SettingDataFileName);
            s_SettingData = JsonUtility.FromJson<SettingData>(FromBase64String(settingData));

            return true;
        }

        private static string FromBase64String(string code)
        {
            byte[] dataBytes = Convert.FromBase64String(code);
            string data = System.Text.Encoding.UTF8.GetString(dataBytes);

            return data;
        }
        #endregion
    }
}
