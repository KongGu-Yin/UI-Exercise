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

        //UI窗体预设路径（参数1：窗体名称，参数2：路径）
        private Dictionary<string, string> _DicFormPaths;
        //缓存所有的UI窗体（参数1：名称）
        private Dictionary<string, UIBaseForm> _DicAllFormCaches;
        //当前显示的窗体（参数1：名称）
        private Dictionary<string, UIBaseForm> _DicDisplayForms;
        //定义一个“栈”，存储当前具有反向切换的窗体
        private Stack<UIBaseForm> _StackCurrentForms = null;
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
            _DicAllFormCaches = new Dictionary<string, UIBaseForm>();
            _DicDisplayForms = new Dictionary<string, UIBaseForm>();

            _StackCurrentForms = new Stack<UIBaseForm>();

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

            //找到各个场景UI  之后用配置表->读表
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

        /// <summary>
        /// 显示（打开）窗体
        /// 功能：
        ///     1.根据窗体的名字，加载到“所有窗体的集合”中
        ///     2.根据不同窗体的“显示模式”，分别做不同的处理操作
        /// </summary>
        /// <param name="formName"><c>预设的窗体名字</c></param>
        public void ShowUIForm(string formName)
        {
            UIBaseForm baseUIForm = null;

            //参数的检查
            if (string.IsNullOrEmpty(formName))
                Debug.LogError(GetType() + "ShowUIForm() 'formName' is null...");
                return;

            baseUIForm = LoadUIToAllFormCaches(formName);
            if (baseUIForm == null)
                return;

            //是否清空“栈集合”中的数据
            if (baseUIForm.CurrentUIType.isClearStack)
            {
                WheatherClearStack();
            }

            //根据窗体不同的显示模式，做不同的处理操作
            switch (baseUIForm.CurrentUIType.UIForm_ShowType)
            {
                case UIFormShowType.Normal:
                    PushUIToNormalCaches(formName);          //:放到‘一般窗体’缓冲中
                    break;
                case UIFormShowType.Reverse:
                    PushUIToReverseStack(formName);         //:放到‘反向切换’窗体的栈中
                    break;
                case UIFormShowType.HideOther:
                    PushUIToHideOtherArray(formName);       //：放到‘隐藏其他’窗体的集合中
                    break;
                default:
                    break;

            }
        }

        /// <summary>
        /// 把当前正在显示的“Normal”窗体放到'显示窗体'的集合中
        /// </summary>
        /// <param name="formName"></param>
        private void PushUIToNormalCaches(string formName)
        {
            UIBaseForm baseUIForm;              //UI窗体基类
            UIBaseForm formFromAllFormCaches;   //从“所有窗体”集合中得到的窗体
            //如何"当前显示"的窗体集合中有这个窗体，那么直接返回：
            _DicDisplayForms.TryGetValue(formName, out baseUIForm);
            if (baseUIForm != null)
                return;

            //添加这个窗体->"当前显示"的窗体集合中
            _DicAllFormCaches.TryGetValue(formName, out formFromAllFormCaches);
            if (formFromAllFormCaches != null)
            {
                _DicDisplayForms.Add(formName, formFromAllFormCaches);
                //让这个窗体--显示
                formFromAllFormCaches.Display();
            }
        }

        /// <summary>
        /// 让“Reverse”窗体入‘栈’
        /// </summary>
        /// <param name="formName"></param>
        private void PushUIToReverseStack(string formName)
        {
            UIBaseForm formFromAllFormCaches;

            //判断‘栈’中是否有窗体，有责进行‘冻结’处理
            if (_StackCurrentForms.Count > 0)
            {
                UIBaseForm topUIForm = _StackCurrentForms.Peek();
                //栈顶元素 冻结处理
                topUIForm.Freeze();
            }
            //判断“所有窗体缓存”集合中是否有此窗体
            _DicAllFormCaches.TryGetValue(formName,out formFromAllFormCaches);

            if (formFromAllFormCaches != null)
            {

            }
        }

        private void PushUIToHideOtherArray(string formName)
        {

        }

        public UIBaseForm LoadUIToAllFormCaches(string formName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 是否清空“反向切换”窗体的栈集合
        /// </summary>
        private bool WheatherClearStack()
        {
            if (_StackCurrentForms != null && _StackCurrentForms.Count >= 1)
            {
                _StackCurrentForms.Clear();
                return true;
            }
            return false;
        }
    }
}
