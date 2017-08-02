/*
 *              Title : "UIFW"窗体框架
 *                  主题 ： 窗体框架管理类
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
    public class UIManager : BehaviourSingleton<UIManager>
    {
        //‘预设窗体路径’（参数1：窗体名称，参数2：路径）
        private Dictionary<string, string> _DicFormPaths;
        //‘总缓存’窗体集合（参数1：名称）
        private Dictionary<string, UIBaseForm> _DicAllFormCaches;
        //‘当前显示’窗体集合（参数1：名称）
        private Dictionary<string, UIBaseForm> _DicDisplayForms;
        //‘反向切换栈’：具有反向切换属性窗体集合
        private Stack<UIBaseForm> _StackCurrentForms = null;
        //UI根节点
        private Transform _UIRootNode = null;
        //UI窗体类型节点
        private Transform _UINormalNode = null;

        private Transform _UIFixedNode = null;

        private Transform _UIPopUpNode = null;

        private Transform _UIScriptNode = null;

        void Awake()
        {
            _DicFormPaths = new Dictionary<string, string>();
            _DicAllFormCaches = new Dictionary<string, UIBaseForm>();
            _DicDisplayForms = new Dictionary<string, UIBaseForm>();

            _StackCurrentForms = new Stack<UIBaseForm>();

            //加载UI根节点_UIRootNode
            InitUIRootNodeLoading();

            _UIRootNode = GameObject.FindGameObjectWithTag(UISysDefine.SYS_TAG_CANVAS).transform;

            _UINormalNode = UIFWHelper.FindTheChildNode(_UIRootNode, UISysDefine.SYS_NODE_NORMAL);
            _UIFixedNode = UIFWHelper.FindTheChildNode(_UIRootNode, UISysDefine.SYS_NODE_FIXED);
            _UIPopUpNode = UIFWHelper.FindTheChildNode(_UIRootNode, UISysDefine.SYS_NODE_POPUP);
            _UIScriptNode = UIFWHelper.FindTheChildNode(_UIRootNode, UISysDefine.SYS_NODE_SCRIPTMGR);

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
            UIResourcesMgr.instance.LoadAssets(UISysDefine.SYS_PATH_CANVAS, false);
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
            {
                Debug.LogError(GetType() + "ShowUIForm() 'formName' is null...");
                return;
            }

            baseUIForm = LoadUIToAllFormCaches(formName);
            if (baseUIForm == null)
                return;

            //是否清空“栈集合”中的数据
            if (baseUIForm.CurrentUIType.isClearStack)
            {
                WheatherClearStack();
            }

            //根据窗体不同的‘显示类型’，做不同的处理操作
            switch (baseUIForm.CurrentUIType.UIForm_ShowType)
            {
                case UIFormShowType.Normal:
                    PushUIToNormalCaches(formName);          //:放到‘一般窗体’缓冲中
                    break;
                case UIFormShowType.Reverse:
                    PushUIToReverseStack(formName);         //:放到‘反向切换’窗体的栈中
                    break;
                case UIFormShowType.HideOther:
                    PushUIToHideOtherPool(formName);       //：放到‘隐藏其他’窗体的集合中
                    break;
                default:
                    break;

            }
        }

        /// <summary>
        /// 关闭窗体/返回上一个窗体
        /// </summary>
        /// <param name="formName"></param>
        public void CloseUIForm(string formName)
        {
            UIBaseForm baseUIForm = null;           //窗体基类

            //参数检测：
            if (string.IsNullOrEmpty(formName)) return;

            //检测‘总缓存’窗体集合中是否有此窗体
            _DicAllFormCaches.TryGetValue(formName, out baseUIForm);
            if (baseUIForm == null)
            {
                Debug.Log("‘总缓存’窗体集合中没有此窗体");
                return;
            }

            //根据窗体的不同‘显示类型’，做不同的操作
            switch (baseUIForm.CurrentUIType.UIForm_ShowType)
            {
                case UIFormShowType.Normal:
                    ExitUIFromNormalCaches(formName);
                    break;
                case UIFormShowType.Reverse:
                    ExitUIFromReverseStack(formName);
                    break;
                case UIFormShowType.HideOther:
                    ExitUIFromHideOtherPool(formName);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// ("Normal"属性)窗体进入'显示窗体'的集合中
        /// </summary>
        /// <param name="formName"></param>
        private void PushUIToNormalCaches(string formName)
        {
            UIBaseForm baseUIForm;              //UI窗体基类
            UIBaseForm formFromAllFormCaches;   //从“所有窗体”集合中得到的窗体
            //如果"当前显示"的窗体集合中有这个窗体，那么直接返回：
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
        /// ("Reverse"属性)窗体入‘栈’。
        /// 功能 ：
        ///     1.如果此窗体的栈中有其他‘切换类型’窗体，冻结它们；
        ///     2.显示此窗体；
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
                //显示此窗体
                formFromAllFormCaches.Display();
                //窗体‘入栈’操作
                _StackCurrentForms.Push(formFromAllFormCaches);
            }
        }

        /// <summary>
        /// ("HideOther"属性)窗体显示，隐藏其他窗体
        /// 功能 ：
        ///     1.将‘当前显示’窗体集合中的所有窗体隐藏；
        ///     2.将‘反向切换栈’集合中的所有窗体隐藏；
        ///     3.添加窗体到‘当前显示’集合中；
        ///     4.设置窗体为显示状态；
        /// </summary>
        /// <param name="formName"></param>
        private void PushUIToHideOtherPool(string formName)
        {
            UIBaseForm baseUIForm = null;               //窗体基类
            UIBaseForm formFromAllFormChches = null;    //从‘总缓存’窗体中得到的窗体基类

            //检测‘当前显示’窗体集合中是否有此窗体
            _DicDisplayForms.TryGetValue(formName, out baseUIForm);
            if (baseUIForm != null) return;

            //将‘当前显示’窗体集合中的所有窗体隐藏
            foreach (UIBaseForm formUI in _DicDisplayForms.Values)
            {
                formUI.Hiding();
            }
            //将‘反向切换栈’集合中的所有窗体隐藏
            foreach (UIBaseForm stackUI in _StackCurrentForms)
            {
                stackUI.Hiding();
            }

            _DicAllFormCaches.TryGetValue(formName, out formFromAllFormChches);
            //将此窗体添加到‘当前显示’集合中，并且设置为显示状态
            if (formFromAllFormChches != null)
            {
                _DicDisplayForms.Add(formName, formFromAllFormChches);
                formFromAllFormChches.Display();
            }
        }

        /// <summary>
        /// ("Normal"属性)窗体关闭；
        /// 功能：
        ///     从‘当前显示’窗体集合中退出；
        /// </summary>
        /// <param name="formName"></param>
        private void ExitUIFromNormalCaches(string formName)
        {
            UIBaseForm baseUIForm = null;

            //检测‘当前显示’窗体集合中是否有此窗体
            _DicDisplayForms.TryGetValue(formName, out baseUIForm);
            if (baseUIForm == null) return;

            //隐藏窗体
            baseUIForm.Hiding();
            //从‘当前显示’窗体集合中移除
            _DicDisplayForms.Remove(formName);
        }

        /// <summary>
        /// ("Reverse"属性)窗体关闭，出‘栈’；
        /// 功能：
        ///     如果冻结了上一级窗体，则将其显示；
        /// </summary>
        /// <param name="formName"></param>
        private void ExitUIFromReverseStack(string formName)
        {
            if (_StackCurrentForms.Count >= 2)
            {
                //出‘栈’
                UIBaseForm topUIForm = _StackCurrentForms.Pop();
                topUIForm.Hiding();
                //“被冻结”的窗体重新“显示”
                UIBaseForm nextUIForm = _StackCurrentForms.Peek();
                nextUIForm.ReDisplay();
            }
            else if (_StackCurrentForms.Count == 1)
            {
                UIBaseForm topUIForm = _StackCurrentForms.Pop();
                topUIForm.Hiding();
            }
        }

        /// <summary>
        /// ("HideOther"属性)窗体关闭
        /// 功能：
        ///     1.从‘当前显示’窗体集合中移除
        ///     2.窗体关闭
        ///     3.将‘当前显示’和‘栈’中的窗体重新显示出来
        /// </summary>
        /// <param name="formName"></param>
        private void ExitUIFromHideOtherPool(string formName)
        {
            UIBaseForm baseUIForm = null;

            //判断‘当前显示’窗体集合中是否有此窗体
            _DicDisplayForms.TryGetValue(formName, out baseUIForm);
            if (baseUIForm == null) return;

            _DicDisplayForms.Remove(formName);
            baseUIForm.Hiding();

            foreach (UIBaseForm formUI in _DicDisplayForms.Values)
            {
                formUI.ReDisplay();
            }
            foreach (UIBaseForm stackUI in _StackCurrentForms)
            {
                stackUI.ReDisplay();
            }
        }

        /// <summary>
        /// 加载指定名称‘窗体’到‘总缓存窗体’集合中;
        /// 功能：
        ///     判断‘总缓存窗体’中是否已经加载，没有才加载
        /// </summary>
        /// <param name="formName"></param>
        /// <returns></returns>
        public UIBaseForm LoadUIToAllFormCaches(string formName)
        {
            UIBaseForm baseUIForm;          //加载的返回窗体

            //判断一下‘总缓存窗体’中是否有本窗体
            _DicAllFormCaches.TryGetValue(formName, out baseUIForm);
            if (baseUIForm == null)
            {
                //加载到‘总缓存窗体’集合中
                baseUIForm = LoadUIForm(formName);
            }
            return baseUIForm;
        }

        /// <summary>
        /// 加载指定名称‘窗体’到‘总缓存窗体’集合中;
        /// 功能：
        ///     1.根据‘formName’检测‘窗体预设路径’字典中是否有此窗体的路径；
        ///     2.加载此窗体克隆体；
        ///     3.根据窗体组件的‘窗体类型’属性设置父节点；
        ///     4.设置此窗体为‘隐藏’状态
        /// </summary>
        /// <param name="formName"></param>
        /// <returns></returns>
        private UIBaseForm LoadUIForm(string formName) {
            string formPath = null;            //窗体路径
            GameObject goFormClone = null;     //窗体的克隆体
            UIBaseForm baseUIForm = null;      //窗体基类
            
            //从‘窗体预设路径’集合中根据名称提取路径
            _DicFormPaths.TryGetValue(formName,out formPath);
            if (!string.IsNullOrEmpty(formPath))
            {
                goFormClone = UIResourcesMgr.instance.LoadAssets(formPath, false);
            }
            else
            {
                Debug.Log("‘窗体预设路径’ 中没有此名称的窗体");
                return null;
            }

            //设置‘克隆窗体’父节点
            if (_UIRootNode != null && goFormClone != null)
            {
                //判断‘预置窗体’上是否有了‘UIBaseForm’脚本
                baseUIForm = goFormClone.GetComponent<UIBaseForm>();
                if (baseUIForm == null)
                {
                    Debug.Log("UIBaseForm == null,Please make sure that the baseUIForm subClass script is loaded on the clone`s form...");
                    return null;
                }
                switch (baseUIForm.CurrentUIType.UIForm_Type)
                {
                    case UIFormType.Normal:
                        goFormClone.transform.SetParent(_UINormalNode, false);
                        break;
                    case UIFormType.Fixed:
                        goFormClone.transform.SetParent(_UIFixedNode, false);
                        break;
                    case UIFormType.PopUp:
                        goFormClone.transform.SetParent(_UIPopUpNode, false);
                        break;
                    default:
                        break;
                }

                //设置隐藏
                goFormClone.SetActive(false);
                //把‘此窗体’加载到‘总缓存窗体’集合中
                _DicAllFormCaches.Add(formName, baseUIForm);
                return baseUIForm;
            }
            else{
                Debug.Log(GetType() + "LoadUIForm():_UIRootNode == null or _goFormClone == null,Please check it...,参数：formName : " + formName);
            }
            return null;
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
