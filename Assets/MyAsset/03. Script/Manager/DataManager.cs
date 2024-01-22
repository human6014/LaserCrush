using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System;
using UnityEngine;
using LaserCrush.Entity;
using LaserCrush.Entity.Item;
using LaserCrush.Data.Json;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace LaserCrush.Data.Json
{
    [Serializable]
    public struct BlockData
    {
        public int m_RowNumber;
        public int m_ColNumber;
        public int m_HP;
        public Vector2 m_Position;
        public EEntityType m_EntityType;
        public DroppedItemType m_HasItemType;

        public BlockData(int row, int col, int hp, Vector2 pos, EEntityType entityType, DroppedItemType itemType)
        {
            m_RowNumber = row;
            m_ColNumber = col;
            m_HP = hp;
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
        public bool m_IsFixedDirection;
        public Vector2 m_Position;
        public Vector2 m_Direction;
        public ItemType m_ItemType;

        public ItemData(int row, int col, int count, bool isFixed, Vector2 pos, Vector2 dir, ItemType type)
        {
            m_RowNumber = row;
            m_ColNumber = col;
            m_Position = pos;
            m_Direction = dir;
            m_ItemType = type;
            m_RemainUsingCount = count;
            m_IsFixedDirection = isFixed;
        }
    }

    [Serializable]
    public class GameData
    {
        public int m_BestScore;
        public int m_CurrentScore;
        public int m_StageNumber;

        public int m_Prism1Count;
        public int m_Prism2Count;
        public int m_Prism3Count;
        public int m_Energy;

        public Vector2 m_LauncherPos;
        public Vector2 m_LauncherDir;

        public List<BlockData> m_Blocks;
        public List<ItemData> m_InstalledItems;

        public GameData(int bestScore, int currentScore, int stageNumber,
                        int prism1Count, int prism2Count, int prism3Count, int energy,
                        Vector2 launcherPos, Vector2 launcherDir,
                        List<BlockData> blocks, List<ItemData> items)
        {
            m_BestScore = bestScore;
            m_CurrentScore = currentScore;
            m_StageNumber = stageNumber;

            m_Prism1Count = prism1Count;
            m_Prism2Count = prism2Count;
            m_Prism3Count = prism3Count;
            m_Energy = energy;

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

        private static string s_Path = Application.persistentDataPath + "/";

        private static readonly string s_GameDataFileName = "GameData";
        private static readonly string s_SettingDataFileName = "SettingData";
        #endregion

        #region Property
        public static GameData GameData { get => s_GameData; private set => s_GameData = value; }
        public static SettingData SettingData { get => s_SettingData; private set => s_SettingData = value; }
        #endregion

        [MenuItem("Data/Reset Data")]
        private static void ResetData()
        {
            InitDataSetting();
            SaveGameData();
            SaveSettingData();
        }

        [MenuItem("Data/Delete Data")]
        private static void DeleteFile()
        {
            string gameDataPath = Path.Combine(s_Path, s_GameDataFileName);
            string settingDataPath = Path.Combine(s_Path, s_SettingDataFileName);
            try
            {
                if (File.Exists(gameDataPath))
                {
                    File.Delete(gameDataPath);
                    Debug.Log(s_GameDataFileName + "昏力 己傍");
                }

                if (File.Exists(settingDataPath))
                {
                    File.Delete(settingDataPath);
                    Debug.Log(s_SettingDataFileName + "昏力 己傍");
                }
            }
            catch (Exception e)
            { Debug.LogError(e.Message); }
        }

        [MenuItem("Data/Open Explorer")]
        private static void OpenExplorer()
        {
            try 
            { Process.Start(s_Path); }
            catch (Exception e)
            { Debug.LogError(e.Message); }
        }



        public static bool InitLoadData()
        {
            s_Path = Application.persistentDataPath + "/";

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

        private static void InitDataSetting()
        {
            s_GameData = new GameData(
                bestScore: 0,
                currentScore: 0,
                stageNumber: 1,
                prism1Count: 0,
                prism2Count: 0,
                prism3Count: 0,
                energy: 10000,
                launcherPos: new Vector2(0, -57), 
                launcherDir: Vector2.zero,
                blocks: new List<BlockData>(),
                items: new List<ItemData>()
            );

            s_SettingData = new SettingData(0, 0, 0);
        }

        #region Save
        public static void SaveGameData()
        {
            string gameData = JsonUtility.ToJson(s_GameData, true);
            File.WriteAllText(s_Path + s_GameDataFileName, gameData);
        }

        public static void SaveSettingData()
        {
            string settingData = JsonUtility.ToJson(s_SettingData, true);
            File.WriteAllText(s_Path + s_SettingDataFileName, settingData);
        }
        #endregion

        #region Load
        private static bool LoadGameData()
        {
            if (!File.Exists(s_Path + s_GameDataFileName)) return false;

            string gameData = File.ReadAllText(s_Path + s_GameDataFileName);
            s_GameData = JsonUtility.FromJson<GameData>(gameData);

            return true;
        }

        private static bool LoadSettingData()
        {
            if (!File.Exists(s_Path + s_SettingDataFileName)) return false;

            string settingData = File.ReadAllText(s_Path + s_SettingDataFileName);
            s_SettingData = JsonUtility.FromJson<SettingData>(settingData);

            return true;
        }
        #endregion
    }
}
