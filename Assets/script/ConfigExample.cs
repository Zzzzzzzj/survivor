using System.Collections.Generic;
using UnityEngine;

namespace SurvivorGame
{
    /// <summary>
    /// 配置使用示例
    /// </summary>
    public class ConfigExample : MonoBehaviour
    {
        [Header("配置测试")]
        [SerializeField] private bool testOnStart = true;
        [SerializeField] private int testEnemyId = 1;
        [SerializeField] private int testWeaponId = 1;
        [SerializeField] private int testItemId = 1;
        [SerializeField] private int testLevelId = 1;

        private void Start()
        {
            if (testOnStart)
            {
                TestConfigSystem();
            }
        }

        /// <summary>
        /// 测试配置系统
        /// </summary>
        [ContextMenu("测试配置系统")]
        public void TestConfigSystem()
        {
            Debug.Log("=== 开始测试配置系统 ===");

            // 1. 测试CSV转JSON
            TestCSVToJSON();

            // 2. 测试CSV转对象
            TestCSVToObjects();

            // 3. 测试配置管理器
            TestConfigManager();

            Debug.Log("=== 配置系统测试完成 ===");
        }

        /// <summary>
        /// 测试CSV转JSON
        /// </summary>
        private void TestCSVToJSON()
        {
            Debug.Log("--- 测试CSV转JSON ---");

            string sampleCSV = @"id,name,health,speed
1,僵尸,100,2.0
2,快速僵尸,80,4.0
3,坦克僵尸,200,1.0";

            string json = CSVDataConverter.ConvertToJSON(sampleCSV);
            Debug.Log($"CSV转JSON结果:\n{json}");
        }

        /// <summary>
        /// 测试CSV转对象
        /// </summary>
        private void TestCSVToObjects()
        {
            Debug.Log("--- 测试CSV转对象 ---");

            string sampleCSV = @"敌人ID,敌人名称,生命值,移动速度,敌人类型,是否Boss
id,name,health,speed,enemyType,isBoss
int,string,int,float,EnemyType,bool
1,僵尸,100,2.0,0,false
2,快速僵尸,80,4.0,1,false
3,坦克僵尸,200,1.0,2,false";

            var enemies = CSVDataConverter.ConvertToObjects<EnemyConfig>(sampleCSV, true, 3);
            Debug.Log($"成功转换 {enemies.Count} 个敌人对象");

            foreach (var enemy in enemies)
            {
                //Debug.Log($"敌人: ID={enemy.id}, 名称={enemy.name}, 生命值={enemy.health}, 速度={enemy.speed}, 类型={enemy.enemyType.GetLocalizedName()}");
            }
        }

        /// <summary>
        /// 测试配置管理器
        /// </summary>
        private void TestConfigManager()
        {
            Debug.Log("--- 测试配置管理器 ---");

            // 获取配置管理器实例
            var configManager = ConfigDataManager.Instance;

            // 注册事件监听
            configManager.OnConfigLoaded += OnConfigLoaded;
            configManager.OnConfigLoadFailed += OnConfigLoadFailed;

            // 加载所有配置
            configManager.LoadAllConfigs();

            // 测试获取配置
            TestGetConfigs(configManager);

            // 测试配置统计
            TestConfigStats(configManager);
        }

        /// <summary>
        /// 测试获取配置
        /// </summary>
        /// <param name="configManager">配置管理器</param>
        private void TestGetConfigs(ConfigDataManager configManager)
        {
            Debug.Log("--- 测试获取配置 ---");

            // 测试获取敌人配置
            var enemy = configManager.GetEnemyConfig(testEnemyId);
            if (enemy != null)
            {
                Debug.Log($"找到敌人配置: ID={enemy.id}, 名称={enemy.name}, 生命值={enemy.health}");
            }
            else
            {
                Debug.LogWarning($"未找到ID为{testEnemyId}的敌人配置");
            }

            // 测试获取武器配置
            var weapon = configManager.GetWeaponConfig(testWeaponId);
            if (weapon != null)
            {
                Debug.Log($"找到武器配置: ID={weapon.id}, 名称={weapon.name}, 伤害={weapon.damage}");
            }
            else
            {
                Debug.LogWarning($"未找到ID为{testWeaponId}的武器配置");
            }

            // 测试获取道具配置
            var item = configManager.GetItemConfig(testItemId);
            if (item != null)
            {
                Debug.Log($"找到道具配置: ID={item.id}, 名称={item.name}, 类型={item.itemType.GetLocalizedName()}");
            }
            else
            {
                Debug.LogWarning($"未找到ID为{testItemId}的道具配置");
            }

            // 测试获取关卡配置
            var level = configManager.GetLevelConfig(testLevelId);
            if (level != null)
            {
                Debug.Log($"找到关卡配置: ID={level.id}, 名称={level.name}, 所需等级={level.requiredLevel}");
            }
            else
            {
                Debug.LogWarning($"未找到ID为{testLevelId}的关卡配置");
            }
        }

        /// <summary>
        /// 测试配置统计
        /// </summary>
        /// <param name="configManager">配置管理器</param>
        private void TestConfigStats(ConfigDataManager configManager)
        {
            Debug.Log("--- 测试配置统计 ---");

            var stats = configManager.GetConfigStats();
            foreach (var stat in stats)
            {
                Debug.Log($"配置 {stat.Key}: {stat.Value} 条记录");
            }

            // 测试通用方法
            Debug.Log("--- 测试通用配置方法 ---");
            var loadedConfigs = configManager.GetLoadedConfigNames();
            Debug.Log($"已加载的配置: {string.Join(", ", loadedConfigs)}");

            // 测试通用获取方法
            var enemyList = configManager.GetConfigList<EnemyConfig>("EnemyConfigs");
            if (enemyList != null)
            {
                Debug.Log($"通过通用方法获取敌人配置: {enemyList.Count} 条");
            }

            var enemyById = configManager.GetConfig<EnemyConfig>("EnemyConfigs", 1);
            if (enemyById != null)
            {
                Debug.Log($"通过通用方法获取敌人ID=1: {enemyById.name}");
            }
        }

        /// <summary>
        /// 配置加载成功回调
        /// </summary>
        /// <param name="configName">配置名称</param>
        private void OnConfigLoaded(string configName)
        {
            Debug.Log($"配置加载成功: {configName}");
        }

        /// <summary>
        /// 配置加载失败回调
        /// </summary>
        /// <param name="configName">配置名称</param>
        private void OnConfigLoadFailed(string configName)
        {
            Debug.LogError($"配置加载失败: {configName}");
        }

        /// <summary>
        /// 测试创建示例CSV
        /// </summary>
        [ContextMenu("创建示例CSV")]
        public void CreateSampleCSV()
        {
            Debug.Log("--- 创建示例CSV ---");

            string sampleCSV = @"敌人ID,敌人名称,敌人描述,生命值,移动速度,攻击伤害,攻击范围,攻击速度,经验奖励,金币奖励,精灵图片路径,动画控制器路径,敌人类型,是否Boss,生成权重
id,name,description,health,speed,attackDamage,attackRange,attackSpeed,expReward,goldReward,spritePath,animatorPath,enemyType,isBoss,spawnWeight
int,string,string,int,float,float,float,float,int,int,string,string,EnemyType,bool,float
1,僵尸,普通的僵尸敌人,100,2.0,10,1.5,1.0,10,5,Sprites/Enemies/zombie,Animations/Enemies/zombie,0,false,1.0
2,快速僵尸,移动速度很快的僵尸,80,4.0,8,1.0,1.5,15,8,Sprites/Enemies/fast_zombie,Animations/Enemies/fast_zombie,1,false,0.7
3,坦克僵尸,生命值很高的僵尸,200,1.0,15,2.0,0.8,20,12,Sprites/Enemies/tank_zombie,Animations/Enemies/tank_zombie,2,false,0.5";

            // 保存到文件
            string filePath = Application.dataPath + "/Resources/Configs/SampleEnemyConfig.csv";
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
            System.IO.File.WriteAllText(filePath, sampleCSV);

            Debug.Log($"示例CSV已创建: {filePath}");

            // 刷新Unity资源
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        /// <summary>
        /// 测试从文件读取CSV
        /// </summary>
        [ContextMenu("测试从文件读取CSV")]
        public void TestReadCSVFromFile()
        {
            Debug.Log("--- 测试从文件读取CSV ---");

            string filePath = Application.dataPath + "/Resources/Configs/SampleEnemyConfig.csv";
            if (System.IO.File.Exists(filePath))
            {
                string csvContent = CSVDataConverter.ReadCSVFromFile(filePath);
                if (!string.IsNullOrEmpty(csvContent))
                {
                    var enemies = CSVDataConverter.ConvertToObjects<EnemyConfig>(csvContent);
                    Debug.Log($"从文件成功读取 {enemies.Count} 个敌人配置");
                }
            }
            else
            {
                Debug.LogWarning("示例CSV文件不存在，请先创建示例CSV");
            }
        }

        /// <summary>
        /// 测试从Resources读取CSV
        /// </summary>
        [ContextMenu("测试从Resources读取CSV")]
        public void TestReadCSVFromResources()
        {
            Debug.Log("--- 测试从Resources读取CSV ---");

            string csvContent = CSVDataConverter.ReadCSVFromResources("Configs/SampleEnemyConfig");
            if (!string.IsNullOrEmpty(csvContent))
            {
                var enemies = CSVDataConverter.ConvertToObjects<EnemyConfig>(csvContent);
                Debug.Log($"从Resources成功读取 {enemies.Count} 个敌人配置");
            }
            else
            {
                Debug.LogWarning("无法从Resources读取CSV文件");
            }
        }

        /// <summary>
        /// 测试配置生成器
        /// </summary>
        [ContextMenu("测试配置生成器")]
        public void TestConfigGenerator()
        {
            Debug.Log("--- 测试配置生成器 ---");

            // 创建配置生成器实例
            var generator = ScriptableObject.CreateInstance<ConfigDataGenerator>();

            // 生成所有配置类
            generator.GenerateAllConfigClasses();

            Debug.Log("配置生成器测试完成");
        }

        private void OnDestroy()
        {
            // 取消事件监听
            if (ConfigDataManager.Instance != null)
            {
                ConfigDataManager.Instance.OnConfigLoaded -= OnConfigLoaded;
                ConfigDataManager.Instance.OnConfigLoadFailed -= OnConfigLoadFailed;
            }
        }
    }
}