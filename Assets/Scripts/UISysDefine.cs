/*
 *                         Title : "UIFW"UI框架
 *                         Descriptions : 
 *                                       1.系统常量
 *                                       2.系统枚举
 *                                       3.全局方法
 *                                       4.委托定义...
 *                                       ...
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFW
{
    //表示窗体类型挂载的节点
    public enum UIFormType
    {
        Normal,
        Fixed,//非全屏，非弹出窗体...诸如：系统消息窗口、血条
        PopUp//弹出窗体
    }
    //窗体显示类型
    public enum UIFormShowType
    {
        Normal,//一般窗体，可以与其他窗体并列显示
        Reverse,//主要应用于弹出窗体，需要维护好多个窗体的“层级”关系
        HideOther//显示窗体的时候回隐藏其他窗体
    }
    //窗体透明类型
    public enum UIFormLenecyType
    {
        Lenecy,//完全透明，不能穿透
        Translucence,//半透明，不能穿透
        ImPenetrable,//低透明，不能穿透
        Penertable//可以穿透
    }
    public class UISysDefine : MonoBehaviour
    {
        public const string SYS_PATH_CANVAS = "UI/Prefabs/Canvas";

        public const string SYS_TAG_CANVAS = "_TagCanvas";
        //挂载窗体类型的节点
        public const string SYS_NODE_NORMAL = "NORMAL";

        public const string SYS_NODE_FIXED = "FIXED";

        public const string SYS_NODE_POPUP = "POPUP";

        //UI各个场景预制体：
        //路径：
        public const string SYS_PATH_LOGIN = "";
        public const string SYS_PATH_MENU = "";
        public const string SYS_PATH_WINDOW = "";
        public const string SYS_PATH_FIGTH = "";
        //名字：
        public const string SYS_NAME_LOGIN = "";
        public const string SYS_NAME_MENU = "";
        public const string SYS_NAME_WINDOW = "";
        public const string SYS_NAME_FIGHT = "";
    }
}
