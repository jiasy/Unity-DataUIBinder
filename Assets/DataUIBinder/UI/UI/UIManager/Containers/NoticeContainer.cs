using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DataUIBinder {
    //漂浮文字，没有按键相应。-7
    //UIManager.instance.NoticeContainer
    public class NoticeContainer : UIContainer {
        public override void Awake() {
            uiType = UIType.Notice;
            base.Awake();

        }
        // public override void Start() {
        //     base.Start();

        // }
        public override UIMain openUI(GameObject gameObject_, string uiName_ = null, string dataPath_ = null) {
            UIMain _uiMain = base.openUI(gameObject_, uiName_,dataPath_);
            _uiMain.uiType = UIType.Notice;
            return _uiMain;
        }
    }
}