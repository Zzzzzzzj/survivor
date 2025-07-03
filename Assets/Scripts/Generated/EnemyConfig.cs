using System;
using UnityEngine;

namespace SurvivorGame
{
    /// <summary>
    /// EnemyConfig - 自动生成的配置类
    /// </summary>
    [Serializable]
    public class EnemyConfig
    {
        /// <summary>
        /// 敌人ID
        /// </summary>
        public int id;

        /// <summary>
        /// 敌人名称
        /// </summary>
        public string name;

        /// <summary>
        /// 敌人描述
        /// </summary>
        public string description;

        /// <summary>
        /// 生命值
        /// </summary>
        public int health;

        /// <summary>
        /// 移动速度
        /// </summary>
        public float speed;

        /// <summary>
        /// 攻击伤害
        /// </summary>
        public float attackDamage;

        /// <summary>
        /// 攻击范围
        /// </summary>
        public float attackRange;

        /// <summary>
        /// 攻击速度
        /// </summary>
        public float attackSpeed;

        /// <summary>
        /// 经验奖励
        /// </summary>
        public int expReward;

        /// <summary>
        /// 金币奖励
        /// </summary>
        public int goldReward;

        /// <summary>
        /// 精灵图片路径
        /// </summary>
        public string spritePath;

        /// <summary>
        /// 动画控制器路径
        /// </summary>
        public string animatorPath;

        /// <summary>
        /// 敌人类型
        /// </summary>
        public EnemyType enemyType;

        /// <summary>
        /// 是否Boss
        /// </summary>
        public bool isBoss;

        /// <summary>
        /// 生成权重
        /// </summary>
        public float spawnWeight;

        public override string ToString()
        {
            return $"EnemyConfig(id={id}, name={name}, description={description}, health={health}, speed={speed}, attackDamage={attackDamage}, attackRange={attackRange}, attackSpeed={attackSpeed}, expReward={expReward}, goldReward={goldReward}, spritePath={spritePath}, animatorPath={animatorPath}, enemyType={enemyType}, isBoss={isBoss}, spawnWeight={spawnWeight})";
        }
    }
}
