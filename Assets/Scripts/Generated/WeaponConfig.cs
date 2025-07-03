using System;
using UnityEngine;

namespace SurvivorGame
{
    /// <summary>
    /// WeaponConfig - 自动生成的配置类
    /// </summary>
    [Serializable]
    public class WeaponConfig
    {
        /// <summary>
        /// 武器ID
        /// </summary>
        public int id;

        /// <summary>
        /// 武器名称
        /// </summary>
        public string name;

        /// <summary>
        /// 武器描述
        /// </summary>
        public string description;

        /// <summary>
        /// 武器类型
        /// </summary>
        public WeaponType weaponType;

        /// <summary>
        /// 伤害值
        /// </summary>
        public float damage;

        /// <summary>
        /// 攻击速度
        /// </summary>
        public float attackSpeed;

        /// <summary>
        /// 攻击范围
        /// </summary>
        public float range;

        /// <summary>
        /// 投射物数量
        /// </summary>
        public int projectileCount;

        /// <summary>
        /// 投射物速度
        /// </summary>
        public float projectileSpeed;

        /// <summary>
        /// 冷却时间
        /// </summary>
        public float cooldown;

        /// <summary>
        /// 最大等级
        /// </summary>
        public int maxLevel;

        /// <summary>
        /// 精灵图片路径
        /// </summary>
        public string spritePath;

        /// <summary>
        /// 投射物预制体路径
        /// </summary>
        public string projectilePrefab;

        /// <summary>
        /// 是否已解锁
        /// </summary>
        public bool isUnlocked;

        /// <summary>
        /// 解锁等级
        /// </summary>
        public int unlockLevel;

        public override string ToString()
        {
            return $"WeaponConfig(id={id}, name={name}, description={description}, weaponType={weaponType}, damage={damage}, attackSpeed={attackSpeed}, range={range}, projectileCount={projectileCount}, projectileSpeed={projectileSpeed}, cooldown={cooldown}, maxLevel={maxLevel}, spritePath={spritePath}, projectilePrefab={projectilePrefab}, isUnlocked={isUnlocked}, unlockLevel={unlockLevel})";
        }
    }
}
