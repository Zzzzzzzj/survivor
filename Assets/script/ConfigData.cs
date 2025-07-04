using System;
using UnityEngine;

namespace SurvivorGame
{
    // /// <summary>
    // /// 敌人配置数据
    // /// </summary>
    // [Serializable]
    // public class EnemyConfig
    // {
    //     public int id;                     // 敌人ID
    //     public string name;                // 敌人名称
    //     public string description;         // 敌人描述
    //     public int health;                 // 生命值
    //     public float speed;                // 移动速度
    //     public float attackDamage;         // 攻击伤害
    //     public float attackRange;          // 攻击范围
    //     public float attackSpeed;          // 攻击速度
    //     public int expReward;              // 经验奖励
    //     public int goldReward;             // 金币奖励
    //     public string spritePath;          // 精灵图片路径
    //     public string animatorPath;        // 动画控制器路径
    //     public EnemyType enemyType;        // 敌人类型
    //     public bool isBoss;                // 是否为Boss
    //     public float spawnWeight;          // 生成权重
    // }

    // /// <summary>
    // /// 武器配置数据
    // /// </summary>
    // [Serializable]
    // public class WeaponConfig
    // {
    //     public int id;                     // 武器ID
    //     public string name;                // 武器名称
    //     public string description;         // 武器描述
    //     public WeaponType weaponType;      // 武器类型
    //     public float damage;               // 伤害值
    //     public float attackSpeed;          // 攻击速度
    //     public float range;                // 攻击范围
    //     public int projectileCount;        // 投射物数量
    //     public float projectileSpeed;      // 投射物速度
    //     public float cooldown;             // 冷却时间
    //     public int maxLevel;               // 最大等级
    //     public string spritePath;          // 精灵图片路径
    //     public string projectilePrefab;    // 投射物预制体路径
    //     public bool isUnlocked;            // 是否已解锁
    //     public int unlockLevel;            // 解锁等级
    // }

    // /// <summary>
    // /// 道具配置数据
    // /// </summary>
    // [Serializable]
    // public class ItemConfig
    // {
    //     public int id;                     // 道具ID
    //     public string name;                // 道具名称
    //     public string description;         // 道具描述
    //     public ItemType itemType;          // 道具类型
    //     public float effectValue;          // 效果数值
    //     public float duration;             // 持续时间
    //     public int maxStack;               // 最大堆叠数量
    //     public string spritePath;          // 精灵图片路径
    //     public bool isConsumable;          // 是否消耗品
    //     public bool isPermanent;           // 是否永久道具
    //     public int rarity;                 // 稀有度
    //     public string effectDescription;   // 效果描述
    // }

    // /// <summary>
    // /// 关卡配置数据
    // /// </summary>
    // [Serializable]
    // public class LevelConfig
    // {
    //     public int id;                     // 关卡ID
    //     public string name;                // 关卡名称
    //     public string description;         // 关卡描述
    //     public int requiredLevel;          // 所需等级
    //     public float duration;             // 关卡时长
    //     public int maxEnemies;             // 最大敌人数
    //     public float enemySpawnRate;       // 敌人生成速率
    //     public int[] enemyTypes;           // 敌人类型数组
    //     public float[] enemySpawnWeights;  // 敌人生成权重
    //     public int expReward;              // 经验奖励
    //     public int goldReward;             // 金币奖励
    //     public string backgroundPath;      // 背景图片路径
    //     public string musicPath;           // 背景音乐路径
    //     public bool isBossLevel;           // 是否为Boss关卡
    //     public int bossId;                 // Boss ID
    // }

    /// <summary>
    /// 敌人类型枚举
    /// </summary>
    public enum EnemyType
    {
        Normal = 0,        // 普通敌人
        Fast = 1,          // 快速敌人
        Tank = 2,          // 坦克敌人
        Ranged = 3,        // 远程敌人
        Boss = 4,          // Boss敌人
        Elite = 5          // 精英敌人
    }

    /// <summary>
    /// 武器类型枚举
    /// </summary>
    public enum WeaponType
    {
        Melee = 0,         // 近战武器
        Ranged = 1,        // 远程武器
        Magic = 2,         // 魔法武器
        Special = 3        // 特殊武器
    }

    /// <summary>
    /// 道具类型枚举
    /// </summary>
    public enum ItemType
    {
        Health = 0,        // 生命道具
        Speed = 1,         // 速度道具
        Damage = 2,        // 伤害道具
        Defense = 3,       // 防御道具
        Experience = 4,    // 经验道具
        Gold = 5,          // 金币道具
        Special = 6        // 特殊道具
    }

    /// <summary>
    /// 稀有度枚举
    /// </summary>
    public enum Rarity
    {
        Common = 0,        // 普通
        Uncommon = 1,      // 罕见
        Rare = 2,          // 稀有
        Epic = 3,          // 史诗
        Legendary = 4      // 传说
    }

    /// <summary>
    /// 游戏难度枚举
    /// </summary>
    public enum GameDifficulty
    {
        Easy = 0,          // 简单
        Normal = 1,        // 普通
        Hard = 2,          // 困难
        Nightmare = 3      // 噩梦
    }

    /// <summary>
    /// 配置数据扩展方法
    /// </summary>
    public static class ConfigDataExtensions
    {
        /// <summary>
        /// 获取敌人类型的本地化名称
        /// </summary>
        /// <param name="enemyType">敌人类型</param>
        /// <returns>本地化名称</returns>
        public static string GetLocalizedName(this EnemyType enemyType)
        {
            switch (enemyType)
            {
                case EnemyType.Normal: return "普通敌人";
                case EnemyType.Fast: return "快速敌人";
                case EnemyType.Tank: return "坦克敌人";
                case EnemyType.Ranged: return "远程敌人";
                case EnemyType.Boss: return "Boss敌人";
                case EnemyType.Elite: return "精英敌人";
                default: return "未知类型";
            }
        }

        /// <summary>
        /// 获取武器类型的本地化名称
        /// </summary>
        /// <param name="weaponType">武器类型</param>
        /// <returns>本地化名称</returns>
        public static string GetLocalizedName(this WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.Melee: return "近战武器";
                case WeaponType.Ranged: return "远程武器";
                case WeaponType.Magic: return "魔法武器";
                case WeaponType.Special: return "特殊武器";
                default: return "未知类型";
            }
        }

        /// <summary>
        /// 获取道具类型的本地化名称
        /// </summary>
        /// <param name="itemType">道具类型</param>
        /// <returns>本地化名称</returns>
        public static string GetLocalizedName(this ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Health: return "生命道具";
                case ItemType.Speed: return "速度道具";
                case ItemType.Damage: return "伤害道具";
                case ItemType.Defense: return "防御道具";
                case ItemType.Experience: return "经验道具";
                case ItemType.Gold: return "金币道具";
                case ItemType.Special: return "特殊道具";
                default: return "未知类型";
            }
        }

        /// <summary>
        /// 获取稀有度的本地化名称
        /// </summary>
        /// <param name="rarity">稀有度</param>
        /// <returns>本地化名称</returns>
        public static string GetLocalizedName(this Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common: return "普通";
                case Rarity.Uncommon: return "罕见";
                case Rarity.Rare: return "稀有";
                case Rarity.Epic: return "史诗";
                case Rarity.Legendary: return "传说";
                default: return "未知";
            }
        }

        /// <summary>
        /// 获取稀有度对应的颜色
        /// </summary>
        /// <param name="rarity">稀有度</param>
        /// <returns>颜色</returns>
        public static Color GetRarityColor(this Rarity rarity)
        {
            switch (rarity)
            {
                case Rarity.Common: return Color.white;
                case Rarity.Uncommon: return Color.green;
                case Rarity.Rare: return Color.blue;
                case Rarity.Epic: return Color.magenta;
                case Rarity.Legendary: return Color.yellow;
                default: return Color.gray;
            }
        }

        /// <summary>
        /// 获取游戏难度的本地化名称
        /// </summary>
        /// <param name="difficulty">游戏难度</param>
        /// <returns>本地化名称</returns>
        public static string GetLocalizedName(this GameDifficulty difficulty)
        {
            switch (difficulty)
            {
                case GameDifficulty.Easy: return "简单";
                case GameDifficulty.Normal: return "普通";
                case GameDifficulty.Hard: return "困难";
                case GameDifficulty.Nightmare: return "噩梦";
                default: return "未知";
            }
        }
    }
}