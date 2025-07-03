using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SurvivorGame
{
    /// <summary>
    /// 配置数据管理器
    /// </summary>
    public class ConfigDataManager : MonoBehaviour
    {
        private static ConfigDataManager _instance;
        public static ConfigDataManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ConfigDataManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("ConfigDataManager");
                        _instance = go.AddComponent<ConfigDataManager>();
                        DontDestroyOnLoad(go);
                    }
                }
                return _instance;
            }
        }

        [Header("配置设置")]
        [SerializeField] private string configFolderPath = "Configs";
        [SerializeField] private bool loadOnStart = true;
        [SerializeField] private bool autoReload = false;

        // 数据缓存
        private Dictionary<string, object> _dataCache = new Dictionary<string, object>();
        private Dictionary<string, DateTime> _fileModificationTimes = new Dictionary<string, DateTime>();

        // 事件
        public event Action<string> OnConfigLoaded;
        public event Action<string> OnConfigReloaded;
        public event Action<string> OnConfigLoadFailed;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (loadOnStart)
            {
                LoadAllConfigs();
            }
        }

        private void Update()
        {
            if (autoReload)
            {
                CheckForFileChanges();
            }
        }

        /// <summary>
        /// 加载所有配置文件
        /// </summary>
        public void LoadAllConfigs()
        {
            try
            {
                // 加载敌人配置
                LoadEnemyConfigs();

                // 加载武器配置
                LoadWeaponConfigs();

                // 加载道具配置
                LoadItemConfigs();

                // 加载关卡配置
                LoadLevelConfigs();

                Debug.Log("所有配置文件加载完成");
            }
            catch (Exception e)
            {
                Debug.LogError($"加载配置文件失败: {e.Message}");
            }
        }

        /// <summary>
        /// 加载敌人配置
        /// </summary>
        public void LoadEnemyConfigs()
        {
            string csvContent = CSVDataConverter.ReadCSVFromResources($"{configFolderPath}/EnemyConfig");
            if (!string.IsNullOrEmpty(csvContent))
            {
                var enemies = CSVDataConverter.ConvertToObjects<EnemyConfig>(csvContent);
                _dataCache["EnemyConfigs"] = enemies;
                OnConfigLoaded?.Invoke("EnemyConfigs");
                Debug.Log($"敌人配置加载完成，共 {enemies.Count} 条记录");
            }
            else
            {
                OnConfigLoadFailed?.Invoke("EnemyConfigs");
            }
        }

        /// <summary>
        /// 加载武器配置
        /// </summary>
        public void LoadWeaponConfigs()
        {
            string csvContent = CSVDataConverter.ReadCSVFromResources($"{configFolderPath}/WeaponConfig");
            if (!string.IsNullOrEmpty(csvContent))
            {
                var weapons = CSVDataConverter.ConvertToObjects<WeaponConfig>(csvContent);
                _dataCache["WeaponConfigs"] = weapons;
                OnConfigLoaded?.Invoke("WeaponConfigs");
                Debug.Log($"武器配置加载完成，共 {weapons.Count} 条记录");
            }
            else
            {
                OnConfigLoadFailed?.Invoke("WeaponConfigs");
            }
        }

        /// <summary>
        /// 加载道具配置
        /// </summary>
        public void LoadItemConfigs()
        {
            string csvContent = CSVDataConverter.ReadCSVFromResources($"{configFolderPath}/ItemConfig");
            if (!string.IsNullOrEmpty(csvContent))
            {
                var items = CSVDataConverter.ConvertToObjects<ItemConfig>(csvContent);
                _dataCache["ItemConfigs"] = items;
                OnConfigLoaded?.Invoke("ItemConfigs");
                Debug.Log($"道具配置加载完成，共 {items.Count} 条记录");
            }
            else
            {
                OnConfigLoadFailed?.Invoke("ItemConfigs");
            }
        }

        /// <summary>
        /// 加载关卡配置
        /// </summary>
        public void LoadLevelConfigs()
        {
            string csvContent = CSVDataConverter.ReadCSVFromResources($"{configFolderPath}/LevelConfig");
            if (!string.IsNullOrEmpty(csvContent))
            {
                var levels = CSVDataConverter.ConvertToObjects<LevelConfig>(csvContent);
                _dataCache["LevelConfigs"] = levels;
                OnConfigLoaded?.Invoke("LevelConfigs");
                Debug.Log($"关卡配置加载完成，共 {levels.Count} 条记录");
            }
            else
            {
                OnConfigLoadFailed?.Invoke("LevelConfigs");
            }
        }

        /// <summary>
        /// 获取敌人配置
        /// </summary>
        /// <param name="enemyId">敌人ID</param>
        /// <returns>敌人配置</returns>
        public EnemyConfig GetEnemyConfig(int enemyId)
        {
            if (_dataCache.TryGetValue("EnemyConfigs", out object data))
            {
                var enemies = data as List<EnemyConfig>;
                return enemies?.Find(e => e.id == enemyId);
            }
            return null;
        }

        /// <summary>
        /// 获取所有敌人配置
        /// </summary>
        /// <returns>敌人配置列表</returns>
        public List<EnemyConfig> GetAllEnemyConfigs()
        {
            if (_dataCache.TryGetValue("EnemyConfigs", out object data))
            {
                return data as List<EnemyConfig>;
            }
            return new List<EnemyConfig>();
        }

        /// <summary>
        /// 获取武器配置
        /// </summary>
        /// <param name="weaponId">武器ID</param>
        /// <returns>武器配置</returns>
        public WeaponConfig GetWeaponConfig(int weaponId)
        {
            if (_dataCache.TryGetValue("WeaponConfigs", out object data))
            {
                var weapons = data as List<WeaponConfig>;
                return weapons?.Find(w => w.id == weaponId);
            }
            return null;
        }

        /// <summary>
        /// 获取所有武器配置
        /// </summary>
        /// <returns>武器配置列表</returns>
        public List<WeaponConfig> GetAllWeaponConfigs()
        {
            if (_dataCache.TryGetValue("WeaponConfigs", out object data))
            {
                return data as List<WeaponConfig>;
            }
            return new List<WeaponConfig>();
        }

        /// <summary>
        /// 获取道具配置
        /// </summary>
        /// <param name="itemId">道具ID</param>
        /// <returns>道具配置</returns>
        public ItemConfig GetItemConfig(int itemId)
        {
            if (_dataCache.TryGetValue("ItemConfigs", out object data))
            {
                var items = data as List<ItemConfig>;
                return items?.Find(i => i.id == itemId);
            }
            return null;
        }

        /// <summary>
        /// 获取所有道具配置
        /// </summary>
        /// <returns>道具配置列表</returns>
        public List<ItemConfig> GetAllItemConfigs()
        {
            if (_dataCache.TryGetValue("ItemConfigs", out object data))
            {
                return data as List<ItemConfig>;
            }
            return new List<ItemConfig>();
        }

        /// <summary>
        /// 获取关卡配置
        /// </summary>
        /// <param name="levelId">关卡ID</param>
        /// <returns>关卡配置</returns>
        public LevelConfig GetLevelConfig(int levelId)
        {
            if (_dataCache.TryGetValue("LevelConfigs", out object data))
            {
                var levels = data as List<LevelConfig>;
                return levels?.Find(l => l.id == levelId);
            }
            return null;
        }

        /// <summary>
        /// 获取所有关卡配置
        /// </summary>
        /// <returns>关卡配置列表</returns>
        public List<LevelConfig> GetAllLevelConfigs()
        {
            if (_dataCache.TryGetValue("LevelConfigs", out object data))
            {
                return data as List<LevelConfig>;
            }
            return new List<LevelConfig>();
        }

        /// <summary>
        /// 重新加载指定配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        public void ReloadConfig(string configName)
        {
            switch (configName.ToLower())
            {
                case "enemyconfigs":
                    LoadEnemyConfigs();
                    break;
                case "weaponconfigs":
                    LoadWeaponConfigs();
                    break;
                case "itemconfigs":
                    LoadItemConfigs();
                    break;
                case "levelconfigs":
                    LoadLevelConfigs();
                    break;
                default:
                    Debug.LogWarning($"未知的配置类型: {configName}");
                    break;
            }
        }

        /// <summary>
        /// 检查文件变化
        /// </summary>
        private void CheckForFileChanges()
        {
            // 这里可以实现文件变化检测逻辑
            // 由于Unity的限制，这里只是示例框架
        }

        /// <summary>
        /// 清除缓存
        /// </summary>
        public void ClearCache()
        {
            _dataCache.Clear();
            _fileModificationTimes.Clear();
            Debug.Log("配置缓存已清除");
        }

        /// <summary>
        /// 导出配置为JSON
        /// </summary>
        /// <param name="configName">配置名称</param>
        /// <param name="filePath">文件路径</param>
        public void ExportConfigToJSON(string configName, string filePath)
        {
            try
            {
                if (_dataCache.TryGetValue(configName, out object data))
                {
                    string json = JsonUtility.ToJson(data, true);
                    File.WriteAllText(filePath, json);
                    Debug.Log($"配置 {configName} 已导出到 {filePath}");
                }
                else
                {
                    Debug.LogWarning($"配置 {configName} 不存在");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"导出配置失败: {e.Message}");
            }
        }

        /// <summary>
        /// 获取配置统计信息
        /// </summary>
        /// <returns>统计信息</returns>
        public Dictionary<string, int> GetConfigStats()
        {
            var stats = new Dictionary<string, int>();

            foreach (var kvp in _dataCache)
            {
                if (kvp.Value is System.Collections.ICollection collection)
                {
                    stats[kvp.Key] = collection.Count;
                }
                else
                {
                    stats[kvp.Key] = 1;
                }
            }

            return stats;
        }
    }
}