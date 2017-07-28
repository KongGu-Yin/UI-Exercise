/*
 *                  Title : "UIFW"项目框架
 *                  Description :
 *                              主要是引用定义的系统枚举,方便使用
 * 
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFW
{
    public class UIType
    {
        //是否清空栈集合
        public bool isClearStack = false;

        public UIFormType UIForm_Type = UIFormType.Normal;

        public UIFormShowType UIForm_ShowType = UIFormShowType.Normal;

        public UIFormLenecyType UIForm_LenecyType = UIFormLenecyType.Lenecy;

    }
}
