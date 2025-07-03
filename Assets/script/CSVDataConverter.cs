using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace SurvivorGame
{
    /// <summary>
    /// CSV数据转换器
    /// </summary>
    public static class CSVDataConverter
    {
        /// <summary>
        /// 将CSV文件转换为JSON字符串
        /// </summary>
        /// <param name="csvContent">CSV内容</param>
        /// <param name="hasHeader">是否包含表头</param>
        /// <returns>JSON字符串</returns>
        public static string ConvertToJSON(string csvContent, bool hasHeader = true)
        {
            try
            {
                string[] lines = csvContent.Split('\n');
                if (lines.Length < 2)
                {
                    Debug.LogWarning("CSV内容为空或格式不正确");
                    return "[]";
                }

                List<string> headers = new List<string>();
                List<Dictionary<string, string>> data = new List<Dictionary<string, string>>();

                // 处理表头
                if (hasHeader)
                {
                    string[] headerLine = ParseCSVLine(lines[0]);
                    headers.AddRange(headerLine);
                }

                // 处理数据行
                for (int i = hasHeader ? 1 : 0; i < lines.Length; i++)
                {
                    if (string.IsNullOrEmpty(lines[i].Trim()))
                        continue;

                    string[] values = ParseCSVLine(lines[i]);
                    Dictionary<string, string> row = new Dictionary<string, string>();

                    for (int j = 0; j < values.Length; j++)
                    {
                        string key = hasHeader ? headers[j] : $"Column{j}";
                        row[key] = values[j];
                    }

                    data.Add(row);
                }

                return JsonUtility.ToJson(new { data = data }, true);
            }
            catch (Exception e)
            {
                Debug.LogError($"CSV转JSON失败: {e.Message}");
                return "[]";
            }
        }

        /// <summary>
        /// 将CSV文件转换为泛型对象列表
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="csvContent">CSV内容</param>
        /// <param name="hasHeader">是否包含表头</param>
        /// <returns>对象列表</returns>
        public static List<T> ConvertToObjects<T>(string csvContent, bool hasHeader = true) where T : class, new()
        {
            try
            {
                string[] lines = csvContent.Split('\n');
                if (lines.Length < 2)
                {
                    Debug.LogWarning("CSV内容为空或格式不正确");
                    return new List<T>();
                }

                List<string> headers = new List<string>();
                List<T> result = new List<T>();

                // 处理表头
                if (hasHeader)
                {
                    string[] headerLine = ParseCSVLine(lines[0]);
                    headers.AddRange(headerLine);
                }

                // 处理数据行
                for (int i = hasHeader ? 1 : 0; i < lines.Length; i++)
                {
                    if (string.IsNullOrEmpty(lines[i].Trim()))
                        continue;

                    string[] values = ParseCSVLine(lines[i]);
                    T obj = new T();

                    for (int j = 0; j < values.Length; j++)
                    {
                        string fieldName = hasHeader ? headers[j] : $"Column{j}";
                        SetObjectField(obj, fieldName, values[j]);
                    }

                    result.Add(obj);
                }

                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"CSV转对象失败: {e.Message}");
                return new List<T>();
            }
        }

        /// <summary>
        /// 解析CSV行，处理引号和逗号
        /// </summary>
        /// <param name="line">CSV行</param>
        /// <returns>字段数组</returns>
        private static string[] ParseCSVLine(string line)
        {
            List<string> fields = new List<string>();
            StringBuilder currentField = new StringBuilder();
            bool inQuotes = false;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        // 转义的双引号
                        currentField.Append('"');
                        i++; // 跳过下一个引号
                    }
                    else
                    {
                        // 切换引号状态
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    // 字段分隔符
                    fields.Add(currentField.ToString().Trim());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }

            // 添加最后一个字段
            fields.Add(currentField.ToString().Trim());

            return fields.ToArray();
        }

        /// <summary>
        /// 设置对象的字段值
        /// </summary>
        /// <param name="obj">目标对象</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="value">值</param>
        private static void SetObjectField(object obj, string fieldName, string value)
        {
            try
            {
                var field = obj.GetType().GetField(fieldName);
                if (field != null)
                {
                    object convertedValue = ConvertValue(value, field.FieldType);
                    field.SetValue(obj, convertedValue);
                    return;
                }

                var property = obj.GetType().GetProperty(fieldName);
                if (property != null && property.CanWrite)
                {
                    object convertedValue = ConvertValue(value, property.PropertyType);
                    property.SetValue(obj, convertedValue);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"设置字段 {fieldName} 失败: {e.Message}");
            }
        }

        /// <summary>
        /// 转换字符串值为目标类型
        /// </summary>
        /// <param name="value">字符串值</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>转换后的值</returns>
        private static object ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrEmpty(value))
            {
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }

            try
            {
                if (targetType == typeof(string))
                    return value;
                else if (targetType == typeof(int))
                    return int.Parse(value);
                else if (targetType == typeof(float))
                    return float.Parse(value);
                else if (targetType == typeof(double))
                    return double.Parse(value);
                else if (targetType == typeof(bool))
                    return bool.Parse(value);
                else if (targetType == typeof(long))
                    return long.Parse(value);
                else if (targetType.IsEnum)
                    return Enum.Parse(targetType, value);
                else
                    return Convert.ChangeType(value, targetType);
            }
            catch
            {
                Debug.LogWarning($"无法转换值 '{value}' 到类型 {targetType.Name}");
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }
        }

        /// <summary>
        /// 从文件读取CSV内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>CSV内容</returns>
        public static string ReadCSVFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Debug.LogError($"CSV文件不存在: {filePath}");
                    return string.Empty;
                }

                return File.ReadAllText(filePath, Encoding.UTF8);
            }
            catch (Exception e)
            {
                Debug.LogError($"读取CSV文件失败: {e.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 从Resources文件夹读取CSV文件
        /// </summary>
        /// <param name="resourcePath">Resources下的路径（不包含扩展名）</param>
        /// <returns>CSV内容</returns>
        public static string ReadCSVFromResources(string resourcePath)
        {
            try
            {
                TextAsset textAsset = Resources.Load<TextAsset>(resourcePath);
                if (textAsset == null)
                {
                    Debug.LogError($"无法从Resources加载CSV文件: {resourcePath}");
                    return string.Empty;
                }

                return textAsset.text;
            }
            catch (Exception e)
            {
                Debug.LogError($"从Resources读取CSV失败: {e.Message}");
                return string.Empty;
            }
        }
    }
}