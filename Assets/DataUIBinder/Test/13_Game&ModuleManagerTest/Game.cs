using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SampleGame {
    public class Game : DataUIBinder.GameBase {
        public static Game instance = null;
        public Game(DataUIBinder.UIManager uiManager_) : base(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace,
            uiManager_
        ) {
            Debug.Log("Game" + nameSpace + " New");
        }
        public virtual IEnumerator init(){
            yield return null;
            //设置 UI 配置
            uiManager.uiConfig.createConfig(
                "UIManagerTestPrefab/" ,
                "BaseTest",
                DataUIBinder.UIType.Base,
                DataUIBinder.ResLoadType.Resources
            );
            uiManager.uiConfig.createConfig(
                "UIManagerTestPrefab/" ,
                "PopTest",
                DataUIBinder.UIType.Pop,
                DataUIBinder.ResLoadType.Resources
            );
            uiManager.uiConfig.createConfig(
                "InputTestPrefab/" ,
                "InputTest",
                DataUIBinder.UIType.Pop,
                DataUIBinder.ResLoadType.Resources
            );
            uiManager.uiConfig.createConfig(
                "uis/" ,
                "DebugMain",
                DataUIBinder.UIType.Debug,
                DataUIBinder.ResLoadType.Resources
            );
            yield return null;
            moduleManager.addModuleByName("Base");
            moduleManager.addModuleByName("User");
            moduleManager.addModuleByName("InputTest");
            //yield return null;
        }
        public override void Dispose() {
            Debug.Log("Game" + nameSpace + " Dispose");
            //底层消除
            base.Dispose();
        }
    }
}