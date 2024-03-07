using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArcticWarfare
{
    public class BuffCell
    {
        public BuffName Name;
        public int LastingTime;

        public BuffCell(BuffName N,int LT)
        {
            Name = N;
            LastingTime = LT;
        }
    }

    public class SupplyCell
    {
        public SupplyName Name;//补给品名称
        public int Durable;//补给品耐久
        public SupplyCell(SupplyName N,int D)
        {
            Name = N;
            Durable = D;
        }
    }

    public enum SupplyName
    {
        Nul,//空
        FixTool,//维修组件
        SklReverce,//技能恢复组件
        APShell,//穿甲弹
        HighFlare,//高空照明弹
        FlameGrenade,//燃烧榴弹
        SmokeGrenade,//烟幕榴弹
        FlameBottle,//燃烧瓶
        FlashBomb,//闪光弹
        SmokeBomb//烟幕弹
    }

    public enum BuffName
    {
        Singel_Action,//单人行动
        SF_Leader,//铁血头目
        Badly_Injured,//重创
        Cloud_Terrified,//心智威慑
        Riders_Eye,//突击者之眼
        Globalcorrected,//全局修正
        Disabled,//被瘫痪
        Badly_Disabled,//严重瘫痪
        InFire,//处于火中
        InSmoke,//处于烟幕中
        LightByFire,//被火照亮
        LightByFlare//被照明弹照亮
    }

}
