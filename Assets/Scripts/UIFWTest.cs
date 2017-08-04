/*
 *                      Title:"UIFW"
 *                          主题："UIFW"框架测试脚本
 *                      Descriptions:
 *                              此脚本专注于"UIFW"框架的各项功能测试与Bug修复
 * 
 */ 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UIFW;

public class UIFWTest : UIBaseForm {

    MessageTransmitEventHandler msgDel = null;

    // Use this for initialization
    void Start () {
        msgDel = BloodValue;
        Debug.Log(1);
        RegisterMsg("blood", msgDel);
        SendMsg("blood", "Mill", 100);
        RegisterMsg("blood", msgDel);
        SendMsg("blood", "Mill", 99);
        //Debug.Log("1");
        //BloodValue(msgValue);
        //Debug.Log("2");
        //BloodValue(msgValue);

        LuaguageMgr.init();
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    void BloodValue(MessageKeyValueUpdate msgValue)
    {
        Debug.Log(msgValue.Key + "\n" + msgValue.Value);
    }
}
