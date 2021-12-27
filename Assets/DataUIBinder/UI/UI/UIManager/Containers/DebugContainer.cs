using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DataUIBinder {
    //调试层，只有一层，相关的框或者连线等。-8
    //UIManager.instance.DebugContainer
    public class DebugContainer : UIContainer {
        public override void Awake() {
            uiType = UIType.Debug;
            base.Awake();
        }
        // public override void Start() {
        //     base.Start();

        // }
        public override UIMain openUI(GameObject gameObject_, string uiName_ = null, string dataPath_ = null) {
            UIMain _uiMain = base.openUI(gameObject_, uiName_,dataPath_);
            _uiMain.uiType = UIType.Debug;
            return _uiMain;
        }
    }
}