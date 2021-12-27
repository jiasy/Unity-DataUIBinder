using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DataUIBinder {
    //悬浮UI。只有一层。各个模块的主入口。-1 为1，Base,Float可点
    //UIManager.instance.FloatContainer
    public class FloatContainer : UIContainer {
        public override void Awake() {
            uiType = UIType.Float;
            base.Awake();

        }
        // public override void Start() {
        //     base.Start();

        // }
        public override UIMain openUI(GameObject gameObject_, string uiName_ = null, string dataPath_ = null) {
            UIMain _uiMain = base.openUI(gameObject_, uiName_,dataPath_);
            _uiMain.uiType = UIType.Float;
            return _uiMain;
        }
    }
}