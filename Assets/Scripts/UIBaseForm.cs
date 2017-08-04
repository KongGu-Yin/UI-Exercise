/*
 *                         Title : "UIFW"项目框架
 *                              主题  ： UI框架基类
 *                         Description :
 *                                     所有窗体类型都要继承于此基类
 *                                     定义了窗体的基本生命周期：显示、隐藏、再显示、冻结
 *                                     窗体遮罩在生命周期函数中进行了封装，无需客户端程序关心
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFW
{
    public class UIBaseForm : MonoBehaviour
    {
        private UIType _CurrentUIType = null;

        public UIType CurrentUIType
        {
            get { return _CurrentUIType; }
            set { _CurrentUIType = value; }
        }

        #region 窗体生命周期
        /// <summary>
        /// 显示
        /// </summary>
        public virtual void Display()
        {
            this.gameObject.SetActive(true);
            if (_CurrentUIType.UIForm_Type == UIFormType.PopUp)
            {
                UIMaskMgr.instance.SetMask(this, _CurrentUIType.UIForm_LenecyType);
            }
        }

        /// <summary>
        /// 隐藏
        /// </summary>
        public virtual void Hiding()
        {
            this.gameObject.SetActive(false);
            if (_CurrentUIType.UIForm_Type == UIFormType.PopUp)
            {
                UIMaskMgr.instance.CloseMask();
            }
        }

        /// <summary>
        /// 再显示
        /// </summary>
        public virtual void ReDisplay()
        {
            this.gameObject.SetActive(true);
            if (_CurrentUIType.UIForm_Type == UIFormType.PopUp)
            {
                UIMaskMgr.instance.SetMask(this, _CurrentUIType.UIForm_LenecyType);
            }
        }

        /// <summary>
        /// 窗体冻结
        /// </summary>
        public virtual void Freeze()
        {
            this.gameObject.SetActive(true);
        }
        #endregion

        /// <summary>
        /// 发送消息(可供子类修改)
        /// </summary>
        /// <param name="msgType"><c>消息类别</c></param>
        /// <param name="msgName"><c>消息名称</c></param>
        /// <param name="msgContent"><c>消息内容</c></param>
        protected virtual void SendMsg(string msgType,string msgName,object msgContent)
        {
            MessageKeyValueUpdate msgValue = new MessageKeyValueUpdate(msgName, msgContent);
            UIMessageMgr.SendMsg(msgType, msgValue);
        }

        /// <summary>
        /// 订阅消息 / 就是对消息增加监听;
        /// Description :
        ///     为防止重复监听消息委托，故：先取消对其监听再重新恢复...
        /// </summary>
        /// <param name="msgType"><c></c></param>
        /// <param name="msgTransmitDel"></param>
        public void RegisterMsg(string msgType,MessageTransmitEventHandler msgTransmitDel)
        {
            //先注销对"msgType"类型消息的监听
            UIMessageMgr.ClearMsgListener(msgType);
            //重新添加对"msgType"类型消息的监听
            UIMessageMgr.AddMsgListener(msgType, msgTransmitDel);
        }
    }
}
