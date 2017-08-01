/*
 *                     Title : "UIFW"UI框架
 *                     功能：资源加载，增加了“缓存”
 * 
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFW
{
    public class UIResourcesMgr : MonoBehaviour
    {
        private static UIResourcesMgr _Instance;

        Hashtable ht;

        public static UIResourcesMgr GetInstance()
        {
            if (_Instance == null)
            {
                _Instance = new GameObject("_ResourcesMgr").AddComponent<UIResourcesMgr>();
            }
            return _Instance;
        }

        void Awake()
        {
            ht = new Hashtable();
        }


        /// <summary>
        /// 资源加载
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="isCatch"><c>是否捕获</c></param>
        /// <returns></returns>
        public T LoadResources<T>(string path,bool isCatch)where T : Object
        {
            if (ht.Contains(path))
                return ht[path] as T;
            T TResources = Resources.Load<T>(path);
            if (TResources == null)
            {
                Debug.LogError(GetType() + "GetInstance() / LoadResources() extracted resources not be found,Please check it! path : " + path);
            }
            else if(isCatch)
            {
                ht.Add(path, TResources);
            }
            return TResources;
        }

        /// <summary>
        /// 加载，克隆资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="isCatch"><c>是否捕获</c></param>
        /// <returns></returns>
        public GameObject LoadAssets(string path, bool isCatch)
        {
            GameObject goObj = LoadResources<GameObject>(path, isCatch);
            GameObject goObjClone = GameObject.Instantiate<GameObject>(goObj);
            if (goObjClone == null)
            {
                Debug.LogError(GetType() + "GetInstance() / LoadAssets() Clone GameObject UnSuccessful! Please check the path, path : " + path);
            }
            return goObjClone;
        }
    }
}
