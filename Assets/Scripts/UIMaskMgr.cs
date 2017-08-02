/*
 *                  Title : "UIFW"框架
 *                         主题：UI遮罩
 *                  Description:
 *                      1.主要与"PopForm"窗体配合使用
 *                      2.根据"UIFormLenecyType"不同，遮罩的特性也不尽相同
 *                      3.启动/关闭
 * 
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UIFW
{
    public class UIMaskMgr : BehaviourSingleton<UIMaskMgr>
    {
        //UI预制体"根"节点/ 顶层 面板
        private Transform _TraRootNode;
        //脚本 节点
        private Transform _TraScriptNode;
        //Mask 面板 节点
        private Transform _TraMaskNode;
        //UI摄像机
        private Camera _UICamera;
        //UI摄像机原始"层深"
        private float _UICameraDepth;

        /// <summary>
        /// 初始化
        /// </summary>
        void Awake()
        {
            _TraRootNode = GameObject.FindGameObjectWithTag(UISysDefine.SYS_TAG_CANVAS).transform;
            _TraScriptNode = UIFWHelper.FindTheChildNode(_TraRootNode, UISysDefine.SYS_NODE_SCRIPTMGR);

            //将本脚本实例设置为 脚本 节点的子节点
            UIFWHelper.AddParent(_TraScriptNode, this.gameObject.transform);

            _TraMaskNode = UIFWHelper.FindTheChildNode(_TraRootNode, UISysDefine.SYS_NODE_MASKPANEL);
            
            _UICamera = UIFWHelper.FindTheChildNode(_TraRootNode, UISysDefine.SYS_NODE_UICAMERA).GetComponent<Camera>();

            if (_UICamera != null)
            {
                _UICameraDepth = _UICamera.depth;
            }
        }

        /// <summary>
        /// 设置遮罩---多为（"PopForm"属性窗体）需要添加；
        /// Descriptions:
        ///     1.根据窗体不同的透明类型，设置遮罩的属性（透明度，是否可穿透）；
        /// </summary>
        /// <param name="formUI"><c>显示的窗体</c></param>
        /// <param name="lenecyType"><c>窗体的透明类型</c></param>
        public void SetMask(UIBaseForm formUI, UIFormLenecyType lenecyType)
        {
            switch (lenecyType)
            {
                case UIFormLenecyType.Lenecy:
                    //设置颜色、透明度
                    _TraMaskNode.GetComponent<Image>().color = Color.black;
                    _TraMaskNode.gameObject.SetActive(true);
                    break;
                case UIFormLenecyType.Translucence:
                    _TraMaskNode.GetComponent<Image>().color =
                    _TraMaskNode.gameObject.SetActive(true);
                    break;
            }
            
        }
    }
}
