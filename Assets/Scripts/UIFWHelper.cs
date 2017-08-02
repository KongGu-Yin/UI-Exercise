/*
 *                  Title:"UIFW"项目框架
 *                      主题："UIFW"项目帮助脚本
 *                  Descriptions:
 *                              1.查找"对应物体"的子节点
 *                              2.给子节点添加组件
 *                              3.从子节点上获取组件
 *                              4.设置父物体
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFW
{
    public class UIFWHelper : MonoBehaviour
    {
        /// <summary>
        /// 查找子节点；
        /// </summary>
        /// <param name="traParent"><c>父节点</c></param>
        /// <param name="childNode"><c>子节点名称</c></param>
        /// <returns></returns>
        public static Transform FindTheChildNode(Transform traParent, string childNode)
        {
            Transform resultNode = null;
            
            resultNode = traParent.Find(childNode);
            if (resultNode == null)
            {
                foreach (Transform item in traParent.transform)
                {
                    //递归查找
                    resultNode = FindTheChildNode(traParent, childNode);
                    if (resultNode != null)
                        return resultNode;
                }
            }
            return resultNode;
        }

        /// <summary>
        /// 给子节点添加组件；
        /// Descriptions：
        ///     1.先找到子节点；
        ///     2.如果子节点上有相同组件则先移除，再重新添加；
        /// </summary>
        /// <typeparam name="T"><c>类型</c></typeparam>
        /// <param name="traParent"><c>父节点</c></param>
        /// <param name="childNode"><c>子节点名称</c></param>
        /// <returns></returns>
        public static T AddChildNodeComponent<T>(Transform traParent,string childNode) where T:Component
        {
            Transform traChild = null;
            T result = null;

            //找到对应的子物体
            traChild = FindTheChildNode(traParent, childNode);
            //先判断身上是否有相同的组件
            if (traChild != null)
            {
                T[] componentArrary = traChild.GetComponents<T>();
                //如果身上有相同的组件，先移除这个组件
                if (componentArrary != null)
                {
                    foreach (T item in componentArrary)
                    {
                        Destroy(item);
                    }
                    //再重新添加组件
                    result = traChild.gameObject.AddComponent<T>();
                }
                else
                {
                    result = traChild.gameObject.AddComponent<T>();
                }
                return result;
            }
            else
            {
                Debug.Log("没有在 物体：" + traParent.ToString() + "下找到子物体 " + childNode.ToString() + " ...");
                return null;
            }
        }

        /// <summary>
        /// 从子物体上获取组件；
        /// </summary>
        /// <typeparam name="T"><c>类型</c></typeparam>
        /// <param name="traParent"><c>父节点</c></param>
        /// <param name="childNode"><c>子节点名称</c></param>
        /// <returns></returns>
        public static T GetComponentFromChildNode<T>(Transform traParent, string childNode) where T : Component
        {
            Transform traChild = null;
            T result = null;

            //先找到子节点
            traChild = FindTheChildNode(traParent, childNode);
            if (traChild != null)
            {
                result = traChild.GetComponent<T>();
                if (result == null)
                {
                    Debug.Log("子物体 ：" + childNode.ToString() + " 上没有该组件...");
                }
                return result;
            }
            else
            {
                Debug.Log("没有在 物体：" + traParent.ToString() + "下找到子物体 " + childNode.ToString() + " ...");
                return null;
            }
        }

        /// <summary>
        /// 给子物体添加父物体
        /// </summary>
        /// <param name="traParent"></param>
        /// <param name="traChild"></param>
        public static void AddParent(Transform traParent, Transform traChild)
        {
            traChild.SetParent(traParent);
            traChild.localPosition = Vector3.zero;
            traChild.localScale = Vector3.one;
            traChild.localEulerAngles = Vector3.zero;
        }
    }
}
