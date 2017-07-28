/*
 *              Tilt : "UIFW"窗体框架
 *              主题 ： 窗体框架管理类
 *              Description ：
 *                          整个UI框架的核心，各个窗体之间的交互都在整个脚本里面，它们个体之间不直接联系
 *                          负责窗体的加载，缓存，以及对于各种生命周期的操作
 *                          
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFW
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _Instance = null;

        //UI窗体预设路径
        private Dictionary<string, string> _DicFormPaths;
        //缓存所有的UI窗体
        private Dictionary<string, UIBaseForm> _DicAllFormCatchs;
        //当前显示的窗体
        private Dictionary<string, UIBaseForm> _DicDisplayForms;
        //定义一个“栈”，存储当前具有反向切换的窗体
        private Stack<UIBaseForm> _StackReverseForm = null;
        //UI根节点
        private Transform _UIRootNode = null;
        //UI窗体类型节点
        private Transform _UINormalNode = null;

        private Transform _UIFixedNode = null;

        private Transform _UIPopUpNode = null;

        private Transform _UIScriptNode = null;

        public static UIManager GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("_UIManager").AddComponent<UIManager>();
            }
            return _Instance;
        }

        void Awake()
        {
            _DicFormPaths = new Dictionary<string, string>();
            _DicAllFormCatchs = new Dictionary<string, UIBaseForm>();
            _DicDisplayForms = new Dictionary<string, UIBaseForm>();
            _StackReverseForm = new Stack<UIBaseForm>();

            //加载UI根节点_UIRootNode
            InitUIRootNodeLoading();

            _UIRootNode = GameObject.FindGameObjectWithTag(UISysDefine.SYS_TAG_CANVAS).transform;

            _UINormalNode = _UIRootNode.Find("NORMAL");
            _UIFixedNode = _UIRootNode.Find("FIXED");
            _UIPopUpNode = _UIRootNode.Find("POPUP");
            _UIScriptNode = _UIRootNode.Find("_UIScriptMgr");

            //将本脚本作为“根UI节点的”子节点
            this.transform.SetParent(_UIScriptNode, false);

            //"场景转换"后，根节点不会消失
            DontDestroyOnLoad(_UIRootNode);

            //找到各个场景UI
            if (_DicFormPaths != null)
            {
                _DicFormPaths.Add(UISysDefine.SYS_NAME_LOGIN, UISysDefine.SYS_PATH_LOGIN);
                _DicFormPaths.Add(UISysDefine.SYS_NAME_MENU, UISysDefine.SYS_PATH_MENU);
                _DicFormPaths.Add(UISysDefine.SYS_NAME_WINDOW, UISysDefine.SYS_PATH_WINDOW);
                _DicFormPaths.Add(UISysDefine.SYS_NAME_FIGHT, UISysDefine.SYS_PATH_FIGTH);
            }
        }

        private void InitUIRootNodeLoading()
        {
            UIResourcesMgr.GetInstance().LoadAssets(UISysDefine.SYS_PATH_CANVAS, false);
        }

        public void ShowUIForm(string formName)
        {

        }
    }
}
