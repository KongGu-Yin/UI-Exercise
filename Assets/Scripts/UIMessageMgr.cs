/*
 *                  Title:"UIFW"项目框架
 *                  主题：消息传递管理中心
 *                  Descriptions:
 *                              负责UI框架中窗口之间的数据传递
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFW
{
    /// <summary>
    /// 消息传递委托
    /// </summary>
    /// <param name="mg"><c>消息键值对儿（对象）</c></param>
    public delegate void MessageTransmitEventHandler(MessageKeyValueUpdate mg);

    public class UIMessageMgr
    {
        /// <summary>
        /// 消息缓存---
        /// Descriptions:---
        ///     string : 消息类型;---
        ///     MessageTransmitEventHandler : 数据执行 委托
        /// </summary>
        public static Dictionary<string, MessageTransmitEventHandler> _dicMessages = new Dictionary<string, MessageTransmitEventHandler>();

        /// <summary>
        /// 添加消息监听 
        /// </summary>
        /// <param name="msgType"><c>消息分类</c></param>
        /// <param name="msgDel"><c>数据执行的委托</c></param>
        public static void AddMsgListener(string msgType, MessageTransmitEventHandler msgDel)
        {
            if (!_dicMessages.ContainsKey(msgType))
            {
                _dicMessages.Add(msgType, null);
            }
            _dicMessages[msgType] += msgDel;
        }

        /// <summary>
        /// 取消以某种委托方式对消息的监听
        /// </summary>
        /// <param name="msgType"><c>消息分类</c></param>
        /// <param name="msgDel"><c>数据执行的委托</c></param>
        public static void RemoveMsgListener(string msgType, MessageTransmitEventHandler msgDel)
        {
            if (_dicMessages.TryGetValue(msgType, out msgDel))
            {
                _dicMessages[msgType] -= msgDel;
            }
        }

        /// <summary>
        /// 取消对特定消息的所有监听
        /// Descriptions:
        ///     可以防止字典缓存越来越多的问题;
        ///     当某个类别的消息不在关心时，注销它会是一个好习惯;
        /// </summary>
        /// <param name="msgType"><c>消息分类</c></param>
        public static void ClearMsgListener(string msgType)
        {
            if (_dicMessages.ContainsKey(msgType))
            {
                _dicMessages.Remove(msgType);
            }
        }

        /// <summary>
        /// 清理消息缓存/取消对所有消息的监听
        /// </summary>
        public static void ClearAllMsgListener()
        {
            if (_dicMessages != null)
            {
                _dicMessages.Clear(); 
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msgType"><c>消息类型</c></param>
        /// <param name="msgValue"><c>消息键值对儿（对象）</c></param>
        public static void SendMsg(string msgType,MessageKeyValueUpdate msgValue)
        {
            MessageTransmitEventHandler msgHandler = null;
            _dicMessages.TryGetValue(msgType, out msgHandler);
            if (msgHandler != null)
            {
                msgHandler(msgValue);
            }
        }
    }

    /// <summary>
    /// 消息数据内容中心
    /// </summary>
    public class MessageKeyValueUpdate
    {
        //键
        private string _Key;
        //值
        private object _Value;
        /// <summary>
        /// 消息类别（eg:消息名称...）
        /// </summary>
        public string Key
        {
            get { return _Key; }
        }
        /// <summary>
        /// 数据内容
        /// </summary>
        public object Value
        {
            get { return _Value; }
        }

        /// <summary>
        /// 消息数据键值对儿
        /// </summary>
        /// <param name="key"><c>消息类别（eg:消息名称...）</c></param>
        /// <param name="value"><c>数据内容</c></param>
        public MessageKeyValueUpdate(string key, object value)
        {
            _Key = key;
            _Value = value;
        }
    }
}
