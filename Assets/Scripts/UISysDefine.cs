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
    /// <summary>
    /// 窗体位置类型，匹配"根节点"下的子节点类型
    /// </summary>
    public enum UIFormType
    {
        /// <summary>
        /// 一般窗体类型
        /// </summary>
        Normal,
        /// <summary>
        /// 非全屏，非弹出窗体...诸如：系统消息窗口、血条
        /// </summary>
        Fixed,
        /// <summary>
        /// 弹出窗体
        /// </summary>
        PopUp
    }
    /// <summary>
    /// 窗体显示类型
    /// </summary>
    public enum UIFormShowType
    {
        /// <summary>
        /// 一般窗体，可以与其他窗体并列显示
        /// </summary>
        Normal,
        /// <summary>
        /// 主要应用于弹出窗体，需要维护好多个窗体的“层级”关系
        /// </summary>
        Reverse,
        /// <summary>
        /// 显示窗体的时候需要隐藏所有其他窗体
        /// </summary>
        HideOther
    }
    /// <summary>
    /// 窗体透明类型
    /// </summary>
    public enum UIFormLenecyType
    {
        /// <summary>
        /// 完全透明，不能穿透
        /// </summary>
        Lenecy,
        /// <summary>
        /// 半透明，不能穿透
        /// </summary>
        Translucence,
        /// <summary>
        /// 低透明，不能穿透
        /// </summary>
        ImPenetrable,
        /// <summary>
        /// 可以穿透
        /// </summary>
        Penertable
    }

    /// <summary>
    /// 根据 UIFormLenecyType 的不同类型设定的颜色值，配合着使用
    /// </summary>
    public struct UIMaskColor
    {
        /// <summary>
        /// R G B is (0 0 0 0) .
        /// </summary>
        public static Color lenecy_Color = new Color(0, 0, 0, 0);
        /// <summary>
        /// R G B is (0 0 0 20/225) .
        /// </summary>
        public static Color Translucence_Color = new Color(0, 0, 0, 20/225);
        /// <summary>
        /// R G B is (0 0 0 50/225) .
        /// </summary>
        public static Color ImPenetrable_Color = new Color(0, 0, 0, 50/225);
    }

    public class UISysDefine : MonoBehaviour
    {
        public const string SYS_PATH_CANVAS = "UI/Prefabs/Canvas";

        public const string SYS_TAG_CANVAS = "_TagCanvas";
        //挂载窗体类型的节点
        public const string SYS_NODE_NORMAL = "NORMAL";

        public const string SYS_NODE_FIXED = "FIXED";

        public const string SYS_NODE_POPUP = "POPUP";

        public const string SYS_NODE_SCRIPTMGR = "_UIScriptMgr";

        public const string SYS_NODE_UICAMERA = "UICamera";

        public const string SYS_NODE_MASKPANEL = "_MaskPanel";

        //UI各个场景预制体：
        //路径：
        public const string SYS_PATH_LOGIN = "SYS_PATH_LOGIN";
        public const string SYS_PATH_MENU = "SYS_PATH_MENU";
        public const string SYS_PATH_WINDOW = "SYS_PATH_WINDOW";
        public const string SYS_PATH_FIGTH = "SYS_PATH_FIGTH";
        //名字：
        public const string SYS_NAME_LOGIN = "SYS_NAME_LOGIN";
        public const string SYS_NAME_MENU = "SYS_NAME_MENU";
        public const string SYS_NAME_WINDOW = "SYS_NAME_WINDOW";
        public const string SYS_NAME_FIGHT = "SYS_NAME_FIGHT";
    }
}
