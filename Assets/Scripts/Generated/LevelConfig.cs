using System;
using UnityEngine;

namespace SurvivorGame
{
    /// <summary>
    /// LevelConfig - 自动生成的配置类
    /// </summary>
    [Serializable]
    public class LevelConfig
    {
        /// <summary>
        /// 关卡ID
        /// </summary>
        public int id;

        /// <summary>
        /// 关卡名称
        /// </summary>
        public string name;

        /// <summary>
        /// 关卡描述
        /// </summary>
        public string description;

        /// <summary>
        /// 所需等级
        /// </summary>
        public int requiredLevel;

        /// <summary>
        /// 关卡时长
        /// </summary>
        public float duration;

        /// <summary>
        /// 最大敌人数
        /// </summary>
        public int maxEnemies;

        /// <summary>
        /// 敌人生成速率
        /// </summary>
        public float enemySpawnRate;

        /// <summary>
        /// 敌人类型数组
        /// </summary>
        public string enemyTypes;

        /// <summary>
        /// 敌人生成权重
        /// </summary>
        public string enemySpawnWeights;

        /// <summary>
        /// 经验奖励
        /// </summary>
        public int expReward;

        /// <summary>
        /// 金币奖励
        /// </summary>
        public int goldReward;

        /// <summary>
        /// 背景图片路径
        /// </summary>
        public string backgroundPath;

        /// <summary>
        /// 背景音乐路径
        /// </summary>
        public string musicPath;

        /// <summary>
        /// 是否Boss关卡
        /// </summary>
        public bool isBossLevel;

        /// <summary>
        /// Boss ID
        /// </summary>
        public int bossId;

        public override string ToString()
        {
            return $"LevelConfig(id={id}, name={name}, description={description}, requiredLevel={requiredLevel}, duration={duration}, maxEnemies={maxEnemies}, enemySpawnRate={enemySpawnRate}, enemyTypes={enemyTypes}, enemySpawnWeights={enemySpawnWeights}, expReward={expReward}, goldReward={goldReward}, backgroundPath={backgroundPath}, musicPath={musicPath}, isBossLevel={isBossLevel}, bossId={bossId})";
        }
    }
}
