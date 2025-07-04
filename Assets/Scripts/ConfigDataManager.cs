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
                // 自动检测并加载所有CSV文件
                AutoLoadAllCSVConfigs();
                Debug.Log("所有配置文件加载完成");
            }
            catch (Exception e)
            {
                Debug.LogError($"加载配置文件失败: {e.Message}");
            }
        }

        /// <summary>
        /// 自动检测并加载所有CSV配置文件
        /// </summary>
        public void AutoLoadAllCSVConfigs()
        {
            try
            {
                // 获取Resources/Configs目录下的所有CSV文件
                string[] csvFiles = GetCSVFilesInConfigsDirectory();

                Debug.Log($"检测到 {csvFiles.Length} 个CSV配置文件");

                foreach (string csvFile in csvFiles)
                {
                    LoadCSVConfig(csvFile);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"自动加载CSV配置失败: {e.Message}");
            }
        }

        /// <summary>
        /// 获取Configs目录下的所有CSV文件
        /// </summary>
        /// <returns>CSV文件路径数组</returns>
        private string[] GetCSVFilesInConfigsDirectory()
        {
            List<string> csvFiles = new List<string>();

            // 检查Resources/Configs目录
            string configsPath = $"{configFolderPath}";

            // 使用Unity的Resources.LoadAll来获取所有CSV文件
            TextAsset[] textAssets = Resources.LoadAll<TextAsset>(configsPath);

            foreach (TextAsset asset in textAssets)
            {
                if (asset.name.EndsWith("Config") && !asset.name.Contains("Sample"))
                {
                    csvFiles.Add(asset.name);
                }
            }

            return csvFiles.ToArray();
        }

        /// <summary>
        /// 加载单个CSV配置文件
        /// </summary>
        /// <param name="configName">配置文件名（不含扩展名）</param>
        private void LoadCSVConfig(string configName)
        {
            try
            {
                string csvContent = CSVDataConverter.ReadCSVFromResources($"{configFolderPath}/{configName}");
                if (string.IsNullOrEmpty(csvContent))
                {
                    Debug.LogWarning($"无法读取CSV文件: {configName}");
                    return;
                }

                // 尝试动态加载配置
                if (TryLoadConfigDynamically(configName, csvContent))
                {
                    return;
                }

                // 根据配置文件名自动选择对应的类型
                switch (configName)
                {
                    case "EnemyConfig":
                        var enemies = CSVDataConverter.ConvertToObjects<EnemyConfig>(csvContent, true, 3);
                        _dataCache["EnemyConfigs"] = enemies;
                        OnConfigLoaded?.Invoke("EnemyConfigs");
                        Debug.Log($"敌人配置加载完成，共 {enemies.Count} 条记录");
                        break;

                    case "WeaponConfig":
                        var weapons = CSVDataConverter.ConvertToObjects<WeaponConfig>(csvContent, true, 3);
                        _dataCache["WeaponConfigs"] = weapons;
                        OnConfigLoaded?.Invoke("WeaponConfigs");
                        Debug.Log($"武器配置加载完成，共 {weapons.Count} 条记录");
                        break;

                    case "ItemConfig":
                        var items = CSVDataConverter.ConvertToObjects<ItemConfig>(csvContent, true, 3);
                        _dataCache["ItemConfigs"] = items;
                        OnConfigLoaded?.Invoke("ItemConfigs");
                        Debug.Log($"道具配置加载完成，共 {items.Count} 条记录");
                        break;

                    case "LevelConfig":
                        var levels = CSVDataConverter.ConvertToObjects<LevelConfig>(csvContent, true, 3);
                        _dataCache["LevelConfigs"] = levels;
                        OnConfigLoaded?.Invoke("LevelConfigs");
                        Debug.Log($"关卡配置加载完成，共 {levels.Count} 条记录");
                        break;

                    default:
                        Debug.LogWarning($"未知的配置文件类型: {configName}");
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"加载配置文件 {configName} 失败: {e.Message}");
                OnConfigLoadFailed?.Invoke(configName);
            }
        }

        /// <summary>
        /// 尝试动态加载配置
        /// </summary>
        /// <param name="configName">配置文件名</param>
        /// <param name="csvContent">CSV内容</param>
        /// <returns>是否成功加载</returns>
        private bool TryLoadConfigDynamically(string configName, string csvContent)
        {
            try
            {
                // 生成类名
                string className = GetClassNameFromConfigName(configName);
                string fullClassName = $"SurvivorGame.{className}";

                // 尝试通过反射获取类型
                Type configType = Type.GetType(fullClassName);
                if (configType == null)
                {
                    // 尝试从当前程序集获取
                    configType = System.Reflection.Assembly.GetExecutingAssembly().GetType(fullClassName);
                }

                if (configType != null)
                {
                    // 使用泛型方法动态转换
                    var method = typeof(CSVDataConverter).GetMethod("ConvertToObjects");
                    var genericMethod = method.MakeGenericMethod(configType);
                    var result = genericMethod.Invoke(null, new object[] { csvContent, true, 3 });

                    if (result != null)
                    {
                        string cacheKey = $"{className}s";
                        _dataCache[cacheKey] = result;
                        OnConfigLoaded?.Invoke(cacheKey);
                        Debug.Log($"{className}配置加载完成，共 {((System.Collections.IList)result).Count} 条记录");
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"动态加载配置 {configName} 失败: {e.Message}");
            }

            return false;
        }

        /// <summary>
        /// 从配置文件名生成类名
        /// </summary>
        /// <param name="configName">配置文件名</param>
        /// <returns>类名</returns>
        private string GetClassNameFromConfigName(string configName)
        {
            // 移除"Config"后缀，首字母大写
            string className = configName.Replace("Config", "");
            if (className.Length > 0)
            {
                className = char.ToUpper(className[0]) + className.Substring(1);
            }
            return className + "Config";
        }

        /// <summary>
        /// 加载敌人配置
        /// </summary>
        public void LoadEnemyConfigs()
        {
            string csvContent = CSVDataConverter.ReadCSVFromResources($"{configFolderPath}/EnemyConfig");
            if (!string.IsNullOrEmpty(csvContent))
            {
                var enemies = CSVDataConverter.ConvertToObjects<EnemyConfig>(csvContent, true, 3);
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
                var weapons = CSVDataConverter.ConvertToObjects<WeaponConfig>(csvContent, true, 3);
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
                var items = CSVDataConverter.ConvertToObjects<ItemConfig>(csvContent, true, 3);
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
                var levels = CSVDataConverter.ConvertToObjects<LevelConfig>(csvContent, true, 3);
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

        /// <summary>
        /// 通用获取配置列表方法
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="configName">配置名称</param>
        /// <returns>配置列表</returns>
        public List<T> GetConfigList<T>(string configName) where T : class
        {
            if (_dataCache.TryGetValue(configName, out object data))
            {
                return data as List<T>;
            }
            return null;
        }

        /// <summary>
        /// 通用获取单个配置方法
        /// </summary>
        /// <typeparam name="T">配置类型</typeparam>
        /// <param name="configName">配置名称</param>
        /// <param name="id">配置ID</param>
        /// <returns>配置对象</returns>
        public T GetConfig<T>(string configName, int id) where T : class
        {
            var configList = GetConfigList<T>(configName);
            if (configList != null)
            {
                // 尝试通过反射获取id字段
                var idProperty = typeof(T).GetField("id");
                if (idProperty != null)
                {
                    foreach (var config in configList)
                    {
                        if (idProperty.GetValue(config).Equals(id))
                        {
                            return config;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取所有已加载的配置名称
        /// </summary>
        /// <returns>配置名称列表</returns>
        public List<string> GetLoadedConfigNames()
        {
            return new List<string>(_dataCache.Keys);
        }
    }
}