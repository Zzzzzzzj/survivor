using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SurvivorGame
{
    /// <summary>
    /// 配置数据生成器 - 用于自动生成配置对象
    /// </summary>
    public class ConfigDataGenerator : ScriptableObject
    {
        [Header("生成设置")]
        [SerializeField] private string configFolderPath = "Configs";
        [SerializeField] private string outputFolderPath = "Scripts/Generated";
        [SerializeField] private bool generateOnStart = false;

        /// <summary>
        /// 设置路径
        /// </summary>
        /// <param name="configPath">配置文件夹路径</param>
        /// <param name="outputPath">输出文件夹路径</param>
        public void SetPaths(string configPath, string outputPath)
        {
            configFolderPath = configPath;
            outputFolderPath = outputPath;
        }

        /// <summary>
        /// 生成所有配置类
        /// </summary>
        [ContextMenu("生成所有配置类")]
        public void GenerateAllConfigClasses()
        {
            try
            {
                string[] csvFiles = GetCSVFilesInConfigsDirectory();
                Debug.Log($"开始生成 {csvFiles.Length} 个配置类");

                foreach (string csvFile in csvFiles)
                {
                    GenerateConfigClass(csvFile);
                }

                Debug.Log("所有配置类生成完成");

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
            }
            catch (Exception e)
            {
                Debug.LogError($"生成配置类失败: {e.Message}");
            }
        }

        /// <summary>
        /// 获取Configs目录下的所有CSV文件
        /// </summary>
        private string[] GetCSVFilesInConfigsDirectory()
        {
            List<string> csvFiles = new List<string>();

            TextAsset[] textAssets = Resources.LoadAll<TextAsset>(configFolderPath);

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
        /// 生成单个配置类
        /// </summary>
        /// <param name="configName">配置文件名</param>
        public void GenerateConfigClass(string configName)
        {
            try
            {
                string csvContent = CSVDataConverter.ReadCSVFromResources($"{configFolderPath}/{configName}");
                if (string.IsNullOrEmpty(csvContent))
                {
                    Debug.LogWarning($"无法读取CSV文件: {configName}");
                    return;
                }

                // 解析CSV内容获取字段信息
                var fieldInfo = ParseCSVFields(csvContent);
                if (fieldInfo == null || fieldInfo.Count == 0)
                {
                    Debug.LogWarning($"无法解析CSV字段: {configName}");
                    return;
                }

                // 生成配置类代码
                string className = GetClassNameFromConfigName(configName);
                string classCode = GenerateClassCode(className, fieldInfo);

                // 保存到文件
                SaveClassToFile(className, classCode);

                Debug.Log($"配置类 {className} 生成完成");
            }
            catch (Exception e)
            {
                Debug.LogError($"生成配置类 {configName} 失败: {e.Message}");
            }
        }

        /// <summary>
        /// 解析CSV字段信息
        /// </summary>
        /// <param name="csvContent">CSV内容</param>
        /// <returns>字段信息列表</returns>
        private List<FieldInfo> ParseCSVFields(string csvContent)
        {
            List<FieldInfo> fields = new List<FieldInfo>();

            string[] lines = csvContent.Split('\n');
            if (lines.Length < 3)
            {
                return fields;
            }

            // 第二行是字段名，第三行是字段类型
            string[] fieldNames = CSVDataConverter.ParseCSVLine(lines[1]);
            string[] fieldTypes = CSVDataConverter.ParseCSVLine(lines[2]);

            for (int i = 0; i < fieldNames.Length && i < fieldTypes.Length; i++)
            {
                fields.Add(new FieldInfo
                {
                    Name = fieldNames[i].Trim(),
                    Type = ConvertCSVTypeToCSType(fieldTypes[i].Trim()),
                    Comment = i < lines[0].Split(',').Length ? lines[0].Split(',')[i].Trim() : ""
                });
            }

            return fields;
        }

        /// <summary>
        /// 将CSV类型转换为C#类型
        /// </summary>
        /// <param name="csvType">CSV类型字符串</param>
        /// <returns>C#类型字符串</returns>
        private string ConvertCSVTypeToCSType(string csvType)
        {
            switch (csvType.ToLower())
            {
                case "int":
                    return "int";
                case "float":
                    return "float";
                case "double":
                    return "double";
                case "string":
                    return "string";
                case "bool":
                    return "bool";
                case "enemytype":
                    return "EnemyType";
                case "weapontype":
                    return "WeaponType";
                case "itemtype":
                    return "ItemType";
                default:
                    return "string"; // 默认使用string
            }
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
        /// 生成类代码预览
        /// </summary>
        /// <param name="configName">配置文件名</param>
        /// <returns>类代码</returns>
        public string GenerateClassCodePreview(string configName)
        {
            try
            {
                string csvContent = CSVDataConverter.ReadCSVFromResources($"{configFolderPath}/{configName}");
                if (string.IsNullOrEmpty(csvContent))
                {
                    return "无法读取CSV文件";
                }

                // 解析CSV内容获取字段信息
                var fieldInfo = ParseCSVFields(csvContent);
                if (fieldInfo == null || fieldInfo.Count == 0)
                {
                    return "无法解析CSV字段";
                }

                // 生成配置类代码
                string className = GetClassNameFromConfigName(configName);
                return GenerateClassCode(className, fieldInfo);
            }
            catch (Exception e)
            {
                return $"生成预览失败: {e.Message}";
            }
        }

        /// <summary>
        /// 生成类代码
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="fields">字段信息</param>
        /// <returns>类代码</returns>
        private string GenerateClassCode(string className, List<FieldInfo> fields)
        {
            StringBuilder code = new StringBuilder();

            // 添加命名空间和using
            code.AppendLine("using System;");
            code.AppendLine("using UnityEngine;");
            code.AppendLine();
            code.AppendLine("namespace SurvivorGame");
            code.AppendLine("{");

            // 添加类注释
            code.AppendLine($"    /// <summary>");
            code.AppendLine($"    /// {className} - 自动生成的配置类");
            code.AppendLine($"    /// </summary>");
            code.AppendLine($"    [Serializable]");
            code.AppendLine($"    public class {className}");
            code.AppendLine("    {");

            // 添加字段
            foreach (var field in fields)
            {
                if (!string.IsNullOrEmpty(field.Comment))
                {
                    code.AppendLine($"        /// <summary>");
                    code.AppendLine($"        /// {field.Comment}");
                    code.AppendLine($"        /// </summary>");
                }
                code.AppendLine($"        public {field.Type} {field.Name};");
                code.AppendLine();
            }

            // 添加ToString方法
            code.AppendLine($"        public override string ToString()");
            code.AppendLine("        {");
            code.AppendLine($"            return $\"{className}({string.Join(", ", fields.ConvertAll(f => $"{f.Name}={{{f.Name}}}"))})\";");
            code.AppendLine("        }");

            code.AppendLine("    }");
            code.AppendLine("}");

            return code.ToString();
        }

        /// <summary>
        /// 保存类到文件
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="classCode">类代码</param>
        private void SaveClassToFile(string className, string classCode)
        {
            // 确保输出目录存在
            string outputPath = Path.Combine(Application.dataPath, outputFolderPath);
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            string filePath = Path.Combine(outputPath, $"{className}.cs");
            File.WriteAllText(filePath, classCode, Encoding.UTF8);
        }

        /// <summary>
        /// 字段信息
        /// </summary>
        private class FieldInfo
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Comment { get; set; }
        }

        /// <summary>
        /// 手动生成指定配置类
        /// </summary>
        /// <param name="configName">配置文件名</param>
        [ContextMenu("生成敌人配置类")]
        public void GenerateEnemyConfigClass()
        {
            GenerateConfigClass("EnemyConfig");
        }

        [ContextMenu("生成武器配置类")]
        public void GenerateWeaponConfigClass()
        {
            GenerateConfigClass("WeaponConfig");
        }

        [ContextMenu("生成道具配置类")]
        public void GenerateItemConfigClass()
        {
            GenerateConfigClass("ItemConfig");
        }

        [ContextMenu("生成关卡配置类")]
        public void GenerateLevelConfigClass()
        {
            GenerateConfigClass("LevelConfig");
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// 配置数据生成器编辑器窗口
    /// </summary>
    public class ConfigDataGeneratorWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private string configFolderPath = "Configs";
        private string outputFolderPath = "Scripts/Generated";
        private List<string> availableConfigs = new List<string>();
        private List<bool> selectedConfigs = new List<bool>();
        private string previewCode = "";
        private string selectedConfigForPreview = "";

        [MenuItem("Tools/配置工具/配置数据生成器")]
        public static void ShowWindow()
        {
            ConfigDataGeneratorWindow window = GetWindow<ConfigDataGeneratorWindow>("配置数据生成器");
            window.minSize = new Vector2(400, 500);
            window.Show();
        }

        [MenuItem("Tools/配置工具/快速生成所有配置")]
        public static void QuickGenerateAllConfigs()
        {
            try
            {
                var generator = ScriptableObject.CreateInstance<ConfigDataGenerator>();
                generator.GenerateAllConfigClasses();
                AssetDatabase.Refresh();
                Debug.Log("快速生成所有配置完成");
            }
            catch (Exception e)
            {
                Debug.LogError($"快速生成配置失败: {e.Message}");
            }
        }

        private void OnEnable()
        {
            RefreshAvailableConfigs();
        }

        private void OnGUI()
        {
            GUILayout.Label("配置数据生成器", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // 配置文件夹路径
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("配置文件夹路径:", GUILayout.Width(120));
            configFolderPath = EditorGUILayout.TextField(configFolderPath);
            if (GUILayout.Button("浏览", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("选择配置文件夹", "Assets/Resources", "");
                if (!string.IsNullOrEmpty(path))
                {
                    configFolderPath = GetRelativePath(path);
                }
            }
            EditorGUILayout.EndHorizontal();

            // 输出文件夹路径
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("输出文件夹路径:", GUILayout.Width(120));
            outputFolderPath = EditorGUILayout.TextField(outputFolderPath);
            if (GUILayout.Button("浏览", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("选择输出文件夹", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                {
                    outputFolderPath = GetRelativePath(path);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // 刷新按钮
            if (GUILayout.Button("刷新可用配置"))
            {
                RefreshAvailableConfigs();
            }

            EditorGUILayout.Space();

            // 显示可用配置
            if (availableConfigs.Count > 0)
            {
                GUILayout.Label("可用配置:", EditorStyles.boldLabel);

                // 全选/取消全选
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("全选", GUILayout.Width(60)))
                {
                    for (int i = 0; i < selectedConfigs.Count; i++)
                    {
                        selectedConfigs[i] = true;
                    }
                }
                if (GUILayout.Button("取消全选", GUILayout.Width(80)))
                {
                    for (int i = 0; i < selectedConfigs.Count; i++)
                    {
                        selectedConfigs[i] = false;
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();

                // 配置列表
                for (int i = 0; i < availableConfigs.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    selectedConfigs[i] = EditorGUILayout.Toggle(selectedConfigs[i], GUILayout.Width(20));
                    GUILayout.Label(availableConfigs[i]);

                    if (GUILayout.Button("预览", GUILayout.Width(60)))
                    {
                        PreviewConfigCode(availableConfigs[i]);
                    }

                    if (GUILayout.Button("生成", GUILayout.Width(60)))
                    {
                        GenerateSingleConfig(availableConfigs[i]);
                    }
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();

                // 生成选中配置按钮
                if (GUILayout.Button("生成选中配置", GUILayout.Height(30)))
                {
                    GenerateSelectedConfigs();
                }

                EditorGUILayout.Space();

                // 生成所有配置按钮
                if (GUILayout.Button("生成所有配置", GUILayout.Height(30)))
                {
                    GenerateAllConfigs();
                }
            }
            else
            {
                GUILayout.Label("未找到可用配置", EditorStyles.helpBox);
                GUILayout.Label("请确保在Resources/Configs目录下有CSV配置文件", EditorStyles.helpBox);
            }

            // 显示代码预览
            if (!string.IsNullOrEmpty(previewCode))
            {
                EditorGUILayout.Space();
                GUILayout.Label($"代码预览 - {selectedConfigForPreview}:", EditorStyles.boldLabel);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.TextArea(previewCode, GUILayout.Height(200));
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 刷新可用配置列表
        /// </summary>
        private void RefreshAvailableConfigs()
        {
            availableConfigs.Clear();
            selectedConfigs.Clear();

            TextAsset[] textAssets = Resources.LoadAll<TextAsset>(configFolderPath);

            foreach (TextAsset asset in textAssets)
            {
                if (asset.name.EndsWith("Config") && !asset.name.Contains("Sample"))
                {
                    availableConfigs.Add(asset.name);
                    selectedConfigs.Add(false);
                }
            }
        }

        /// <summary>
        /// 预览配置代码
        /// </summary>
        /// <param name="configName">配置名称</param>
        private void PreviewConfigCode(string configName)
        {
            try
            {
                var generator = CreateGenerator();
                previewCode = generator.GenerateClassCodePreview(configName);
                selectedConfigForPreview = configName;
            }
            catch (Exception e)
            {
                Debug.LogError($"预览配置 {configName} 失败: {e.Message}");
                previewCode = $"预览失败: {e.Message}";
            }
        }

        /// <summary>
        /// 生成单个配置
        /// </summary>
        /// <param name="configName">配置名称</param>
        private void GenerateSingleConfig(string configName)
        {
            try
            {
                var generator = CreateGenerator();
                generator.GenerateConfigClass(configName);
                AssetDatabase.Refresh();
                Debug.Log($"配置类 {configName} 生成完成");
            }
            catch (Exception e)
            {
                Debug.LogError($"生成配置 {configName} 失败: {e.Message}");
            }
        }

        /// <summary>
        /// 生成选中的配置
        /// </summary>
        private void GenerateSelectedConfigs()
        {
            try
            {
                var generator = CreateGenerator();
                int generatedCount = 0;

                for (int i = 0; i < availableConfigs.Count; i++)
                {
                    if (selectedConfigs[i])
                    {
                        generator.GenerateConfigClass(availableConfigs[i]);
                        generatedCount++;
                    }
                }

                AssetDatabase.Refresh();
                Debug.Log($"成功生成 {generatedCount} 个配置类");
            }
            catch (Exception e)
            {
                Debug.LogError($"生成选中配置失败: {e.Message}");
            }
        }

        /// <summary>
        /// 生成所有配置
        /// </summary>
        private void GenerateAllConfigs()
        {
            try
            {
                var generator = CreateGenerator();
                generator.GenerateAllConfigClasses();
                AssetDatabase.Refresh();
                Debug.Log("所有配置类生成完成");
            }
            catch (Exception e)
            {
                Debug.LogError($"生成所有配置失败: {e.Message}");
            }
        }

        /// <summary>
        /// 创建生成器实例
        /// </summary>
        /// <returns>生成器实例</returns>
        private ConfigDataGenerator CreateGenerator()
        {
            var generator = ScriptableObject.CreateInstance<ConfigDataGenerator>();
            generator.SetPaths(configFolderPath, outputFolderPath);
            return generator;
        }

        /// <summary>
        /// 获取相对路径
        /// </summary>
        /// <param name="absolutePath">绝对路径</param>
        /// <returns>相对路径</returns>
        private string GetRelativePath(string absolutePath)
        {
            if (absolutePath.StartsWith(Application.dataPath))
            {
                return "Assets" + absolutePath.Substring(Application.dataPath.Length);
            }
            return absolutePath;
        }
    }
#endif
}