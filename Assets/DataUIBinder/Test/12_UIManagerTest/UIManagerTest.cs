/*
        createConfig("Assets/DataUIBinder/Test/11_UIManagerTest" ,"Test_Base"       ,UIType.Base,ResLoadType.Resources);
        createConfig("Assets/DataUIBinder/Test/11_UIManagerTest" ,"Test_Pop_Overlay",UIType.Pop ,ResLoadType.Resources);
        createConfig("Assets/DataUIBinder/Test/11_UIManagerTest" ,"Test_Pop_Replace",UIType.Pop ,ResLoadType.Resources);
        createConfig("Assets/DataUIBinder/Test/11_UIManagerTest" ,"Test_Tip"        ,UIType.Tip ,ResLoadType.Resources);
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataUIBinder;
namespace DataUIBinder{
    public class UIManagerTest : MonoBehaviour{
        void Awake() {
            DataPathListener.debugRecord = true;
            DataPathListListener.debugRecord = true;
            ComponentWrapper.debugRecord = true;
            ReUseObj.debugRecord = true;
			DataCenter.defalutInit();
            //设置模块数据
            dc.sv("module.InputTest.name","");
            dc.sv("module.InputTest.time","");
            //设置 UI 配置
            UIConfig.instance.createConfig(
                "UIManagerTestPrefab/" ,
                "BaseTest",
                UIType.Base,
                ResLoadType.Resources
            );
            UIConfig.instance.createConfig(
                "UIManagerTestPrefab/" ,
                "PopTest",
                UIType.Pop,
                ResLoadType.Resources
            );
            UIConfig.instance.createConfig(
                "InputTestPrefab/" ,
                "InputTest",
                UIType.Pop,
                ResLoadType.Resources
            );
            UIConfig.instance.createConfig(
                "uis/" ,
                "DebugMain",
                UIType.Debug,
                ResLoadType.Resources
            );
        }
        
		void Start(){
            UIManager.instance.openUI("BaseTest");
            UIManager.instance.openUI("DebugMain");
		}
        void Update(){
            float _dt = Time.deltaTime;
            DataCenter.frameUpdate(_dt);
			UIManager.doFrameUpdate(_dt);
        }
    }
}