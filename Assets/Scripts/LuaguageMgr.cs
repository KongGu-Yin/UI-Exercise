/*                  
 *                      Title:"UIFW"项目框架
 *                          主题：语言管理
 *                      Descriptions:
 *                              根据默认设置或用户设置显示不同的语言
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFW
{
    public class LuaguageMgr
    {
        /*单例*/
        private static LuaguageMgr _instance;
        /// <summary>
        /// 语言缓存集合
        /// </summary>
        private Dictionary<string, string> _dicLuaguageCache;

        public static LuaguageMgr Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LuaguageMgr();

                init();
                return _instance;
            }
        }

        private LuaguageMgr()
        {
            _dicLuaguageCache = new Dictionary<string, string>();
            InitLuaguageCache();
        }

        /// <summary>
        /// 根据配置表格语言ID进行显示
        /// </summary>
        /// <param name="luaguageID"></param>
        public string Show(string luaguageID)
        {
            string result = string.Empty;

            //参数判断
            if (string.IsNullOrEmpty(luaguageID))
                return null;

            if (_dicLuaguageCache != null && _dicLuaguageCache.Count > 0)
            {
                _dicLuaguageCache.TryGetValue(luaguageID, out result);

                if (!string.IsNullOrEmpty(result))
                    return result;
            }

            Debug.Log("_dicLuaguae is empty or does not have the Luaguage ID corresponding to the content...");
            return null;
        }

        /// <summary>
        /// 初始化语言缓存
        /// Descriptions:
        ///     初始情况下，从配置表中获取语言内容放到集合中
        /// </summary>
        private void InitLuaguageCache()
        {

        }

        public static void init() { }
    }
}
