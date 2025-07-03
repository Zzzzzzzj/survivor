using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace SurvivorGame
{
    /// <summary>
    /// 配置编辑器
    /// </summary>
    public class ConfigEditor : EditorWindow
    {
        private Vector2 scrollPosition;
        private string configFolderPath = "Assets/Resources/Configs";
        private bool showEnemyConfigs = true;
        private bool showWeaponConfigs = true;
        private bool showItemConfigs = true;
        private bool showLevelConfigs = true;

        [MenuItem("Tools/配置编辑器")]
        public static void ShowWindow()
        {
            GetWindow<ConfigEditor>("配置编辑器");
        }

        private void OnGUI()
        {
            GUILayout.Label("CSV配置编辑器", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            // 配置文件夹路径
            EditorGUILayout.BeginHorizontal();
            configFolderPath = EditorGUILayout.TextField("配置文件夹路径", configFolderPath);
            if (GUILayout.Button("选择文件夹", GUILayout.Width(100)))
            {
                string path = EditorUtility.OpenFolderPanel("选择配置文件夹", "Assets/Resources", "");
                if (!string.IsNullOrEmpty(path))
                {
                    configFolderPath = path;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // 创建配置文件夹按钮
            if (GUILayout.Button("创建配置文件夹"))
            {
                CreateConfigFolder();
            }

            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // 敌人配置
            showEnemyConfigs = EditorGUILayout.Foldout(showEnemyConfigs, "敌人配置");
            if (showEnemyConfigs)
            {
                EditorGUI.indentLevel++;
                if (GUILayout.Button("创建敌人配置CSV"))
                {
                    CreateEnemyConfigCSV();
                }
                if (GUILayout.Button("导入敌人配置"))
                {
                    ImportEnemyConfig();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // 武器配置
            showWeaponConfigs = EditorGUILayout.Foldout(showWeaponConfigs, "武器配置");
            if (showWeaponConfigs)
            {
                EditorGUI.indentLevel++;
                if (GUILayout.Button("创建武器配置CSV"))
                {
                    CreateWeaponConfigCSV();
                }
                if (GUILayout.Button("导入武器配置"))
                {
                    ImportWeaponConfig();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // 道具配置
            showItemConfigs = EditorGUILayout.Foldout(showItemConfigs, "道具配置");
            if (showItemConfigs)
            {
                EditorGUI.indentLevel++;
                if (GUILayout.Button("创建道具配置CSV"))
                {
                    CreateItemConfigCSV();
                }
                if (GUILayout.Button("导入道具配置"))
                {
                    ImportItemConfig();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // 关卡配置
            showLevelConfigs = EditorGUILayout.Foldout(showLevelConfigs, "关卡配置");
            if (showLevelConfigs)
            {
                EditorGUI.indentLevel++;
                if (GUILayout.Button("创建关卡配置CSV"))
                {
                    CreateLevelConfigCSV();
                }
                if (GUILayout.Button("导入关卡配置"))
                {
                    ImportLevelConfig();
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();

            // 批量操作
            GUILayout.Label("批量操作", EditorStyles.boldLabel);
            if (GUILayout.Button("创建所有配置CSV"))
            {
                CreateAllConfigCSV();
            }
            if (GUILayout.Button("导入所有配置"))
            {
                ImportAllConfigs();
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 创建配置文件夹
        /// </summary>
        private void CreateConfigFolder()
        {
            if (!Directory.Exists(configFolderPath))
            {
                Directory.CreateDirectory(configFolderPath);
                AssetDatabase.Refresh();
                Debug.Log($"配置文件夹已创建: {configFolderPath}");
            }
            else
            {
                Debug.Log("配置文件夹已存在");
            }
        }

        /// <summary>
        /// 创建敌人配置CSV
        /// </summary>
        private void CreateEnemyConfigCSV()
        {
            try
            {
                string csvContent = @"id,name,description,health,speed,attackDamage,attackRange,attackSpeed,expReward,goldReward,spritePath,animatorPath,enemyType,isBoss,spawnWeight
1,僵尸,普通的僵尸敌人,100,2.0,10,1.5,1.0,10,5,Sprites/Enemies/zombie,Animations/Enemies/zombie,0,false,1.0
2,快速僵尸,移动速度很快的僵尸,80,4.0,8,1.0,1.5,15,8,Sprites/Enemies/fast_zombie,Animations/Enemies/fast_zombie,1,false,0.7
3,坦克僵尸,生命值很高的僵尸,200,1.0,15,2.0,0.8,20,12,Sprites/Enemies/tank_zombie,Animations/Enemies/tank_zombie,2,false,0.5
4,远程僵尸,可以远程攻击的僵尸,120,1.5,12,5.0,0.6,18,10,Sprites/Enemies/ranged_zombie,Animations/Enemies/ranged_zombie,3,false,0.6
5,僵尸王,强大的Boss敌人,500,1.8,25,3.0,0.5,100,50,Sprites/Enemies/zombie_boss,Animations/Enemies/zombie_boss,4,true,0.1";

                // 确保使用正确的路径分隔符
                string filePath = configFolderPath.Replace('\\', '/') + "/EnemyConfig.csv";

                // 确保目录存在
                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(filePath, csvContent);
                AssetDatabase.Refresh();
                Debug.Log($"敌人配置CSV已创建: {filePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"创建敌人配置CSV失败: {e.Message}");
            }
        }

        /// <summary>
        /// 创建武器配置CSV
        /// </summary>
        private void CreateWeaponConfigCSV()
        {
            try
            {
                string csvContent = @"id,name,description,weaponType,damage,attackSpeed,range,projectileCount,projectileSpeed,cooldown,maxLevel,spritePath,projectilePrefab,isUnlocked,unlockLevel
1,小刀,基础近战武器,0,15,2.0,1.5,1,0,0.5,5,Sprites/Weapons/knife,Prefabs/Projectiles/knife,true,1
2,手枪,基础远程武器,1,20,1.5,8.0,1,10,1.0,5,Sprites/Weapons/pistol,Prefabs/Projectiles/bullet,true,1
3,魔法杖,基础魔法武器,2,25,1.0,6.0,1,8,1.5,5,Sprites/Weapons/staff,Prefabs/Projectiles/magic,false,5
4,霰弹枪,近距离高伤害武器,1,35,0.8,4.0,5,12,2.0,5,Sprites/Weapons/shotgun,Prefabs/Projectiles/shotgun,false,10
5,激光剑,特殊近战武器,3,40,1.2,2.0,1,0,0.8,5,Sprites/Weapons/laser_sword,Prefabs/Projectiles/laser,false,15";

                string filePath = configFolderPath.Replace('\\', '/') + "/WeaponConfig.csv";

                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(filePath, csvContent);
                AssetDatabase.Refresh();
                Debug.Log($"武器配置CSV已创建: {filePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"创建武器配置CSV失败: {e.Message}");
            }
        }

        /// <summary>
        /// 创建道具配置CSV
        /// </summary>
        private void CreateItemConfigCSV()
        {
            try
            {
                string csvContent = @"id,name,description,itemType,effectValue,duration,maxStack,spritePath,isConsumable,isPermanent,rarity,effectDescription
1,生命药水,恢复生命值,0,50,0,10,Sprites/Items/health_potion,true,false,0,恢复50点生命值
2,速度药水,提升移动速度,1,2.0,30.0,5,Sprites/Items/speed_potion,true,false,1,提升移动速度2倍，持续30秒
3,力量药水,提升攻击力,2,10,60.0,3,Sprites/Items/strength_potion,true,false,2,提升攻击力10点，持续60秒
4,护甲,提升防御力,3,20,0,1,Sprites/Items/armor,false,true,3,永久提升防御力20点
5,经验宝石,获得经验值,4,100,0,20,Sprites/Items/exp_gem,true,false,1,获得100点经验值
6,金币袋,获得金币,5,50,0,10,Sprites/Items/gold_bag,true,false,0,获得50金币
7,传送卷轴,瞬间传送,6,0,0,1,Sprites/Items/teleport_scroll,true,false,4,瞬间传送到安全位置";

                string filePath = configFolderPath.Replace('\\', '/') + "/ItemConfig.csv";

                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(filePath, csvContent);
                AssetDatabase.Refresh();
                Debug.Log($"道具配置CSV已创建: {filePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"创建道具配置CSV失败: {e.Message}");
            }
        }

        /// <summary>
        /// 创建关卡配置CSV
        /// </summary>
        private void CreateLevelConfigCSV()
        {
            try
            {
                string csvContent = @"id,name,description,requiredLevel,duration,maxEnemies,enemySpawnRate,enemyTypes,enemySpawnWeights,expReward,goldReward,backgroundPath,musicPath,isBossLevel,bossId
1,新手村,适合新手的简单关卡,1,300,50,2.0,""0,1"",""0.7,0.3"",100,50,Sprites/Backgrounds/village,Audio/Music/village,false,0
2,森林深处,充满危险的森林,5,400,80,2.5,""0,1,2"",""0.5,0.3,0.2"",200,100,Sprites/Backgrounds/forest,Audio/Music/forest,false,0
3,废弃工厂,工业废墟中的战斗,10,500,120,3.0,""1,2,3"",""0.4,0.3,0.3"",300,150,Sprites/Backgrounds/factory,Audio/Music/factory,false,0
4,地下墓穴,黑暗的地下世界,15,600,150,3.5,""2,3,4"",""0.3,0.4,0.3"",400,200,Sprites/Backgrounds/catacomb,Audio/Music/catacomb,false,0
5,Boss战,与僵尸王的决战,20,900,200,4.0,""0,1,2,3,4"",""0.2,0.2,0.2,0.2,0.2"",500,250,Sprites/Backgrounds/boss_arena,Audio/Music/boss,true,5";

                string filePath = configFolderPath.Replace('\\', '/') + "/LevelConfig.csv";

                string directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(filePath, csvContent);
                AssetDatabase.Refresh();
                Debug.Log($"关卡配置CSV已创建: {filePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"创建关卡配置CSV失败: {e.Message}");
            }
        }

        /// <summary>
        /// 创建所有配置CSV
        /// </summary>
        private void CreateAllConfigCSV()
        {
            CreateConfigFolder();
            CreateEnemyConfigCSV();
            CreateWeaponConfigCSV();
            CreateItemConfigCSV();
            CreateLevelConfigCSV();
            Debug.Log("所有配置CSV文件已创建完成");
        }

        /// <summary>
        /// 导入敌人配置
        /// </summary>
        private void ImportEnemyConfig()
        {
            string filePath = EditorUtility.OpenFilePanel("选择敌人配置CSV", configFolderPath, "csv");
            if (!string.IsNullOrEmpty(filePath))
            {
                string csvContent = File.ReadAllText(filePath);
                var enemies = CSVDataConverter.ConvertToObjects<EnemyConfig>(csvContent);
                Debug.Log($"成功导入 {enemies.Count} 条敌人配置");

                // 这里可以添加更多的处理逻辑，比如保存到ScriptableObject
            }
        }

        /// <summary>
        /// 导入武器配置
        /// </summary>
        private void ImportWeaponConfig()
        {
            string filePath = EditorUtility.OpenFilePanel("选择武器配置CSV", configFolderPath, "csv");
            if (!string.IsNullOrEmpty(filePath))
            {
                string csvContent = File.ReadAllText(filePath);
                var weapons = CSVDataConverter.ConvertToObjects<WeaponConfig>(csvContent);
                Debug.Log($"成功导入 {weapons.Count} 条武器配置");
            }
        }

        /// <summary>
        /// 导入道具配置
        /// </summary>
        private void ImportItemConfig()
        {
            string filePath = EditorUtility.OpenFilePanel("选择道具配置CSV", configFolderPath, "csv");
            if (!string.IsNullOrEmpty(filePath))
            {
                string csvContent = File.ReadAllText(filePath);
                var items = CSVDataConverter.ConvertToObjects<ItemConfig>(csvContent);
                Debug.Log($"成功导入 {items.Count} 条道具配置");
            }
        }

        /// <summary>
        /// 导入关卡配置
        /// </summary>
        private void ImportLevelConfig()
        {
            string filePath = EditorUtility.OpenFilePanel("选择关卡配置CSV", configFolderPath, "csv");
            if (!string.IsNullOrEmpty(filePath))
            {
                string csvContent = File.ReadAllText(filePath);
                var levels = CSVDataConverter.ConvertToObjects<LevelConfig>(csvContent);
                Debug.Log($"成功导入 {levels.Count} 条关卡配置");
            }
        }

        /// <summary>
        /// 导入所有配置
        /// </summary>
        private void ImportAllConfigs()
        {
            ImportEnemyConfig();
            ImportWeaponConfig();
            ImportItemConfig();
            ImportLevelConfig();
            Debug.Log("所有配置导入完成");
        }
    }
}