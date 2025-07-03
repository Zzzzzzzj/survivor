using System;
using UnityEngine;

namespace SurvivorGame
{
    /// <summary>
    /// ItemConfig - 自动生成的配置类
    /// </summary>
    [Serializable]
    public class ItemConfig
    {
        /// <summary>
        /// 道具ID
        /// </summary>
        public int id;

        /// <summary>
        /// 道具名称
        /// </summary>
        public string name;

        /// <summary>
        /// 道具描述
        /// </summary>
        public string description;

        /// <summary>
        /// 道具类型
        /// </summary>
        public ItemType itemType;

        /// <summary>
        /// 效果数值
        /// </summary>
        public float effectValue;

        /// <summary>
        /// 持续时间
        /// </summary>
        public float duration;

        /// <summary>
        /// 最大堆叠数量
        /// </summary>
        public int maxStack;

        /// <summary>
        /// 精灵图片路径
        /// </summary>
        public string spritePath;

        /// <summary>
        /// 是否消耗品
        /// </summary>
        public bool isConsumable;

        /// <summary>
        /// 是否永久道具
        /// </summary>
        public bool isPermanent;

        /// <summary>
        /// 稀有度
        /// </summary>
        public int rarity;

        /// <summary>
        /// 效果描述
        /// </summary>
        public string effectDescription;

        public override string ToString()
        {
            return $"ItemConfig(id={id}, name={name}, description={description}, itemType={itemType}, effectValue={effectValue}, duration={duration}, maxStack={maxStack}, spritePath={spritePath}, isConsumable={isConsumable}, isPermanent={isPermanent}, rarity={rarity}, effectDescription={effectDescription})";
        }
    }
}
