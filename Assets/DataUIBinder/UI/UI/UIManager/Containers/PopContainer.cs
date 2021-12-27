using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DataUIBinder {
    //弹出层。UI的主要容器，需要层级管理。-2
    //UIManager.instance.PopContainer
    public class PopContainer : UIContainer {
        public override void Awake() {
            uiType = UIType.Pop;
            base.Awake();

        }
        // public override void Start() {
        //     base.Start();

        // }
        public override UIMain openUI(GameObject gameObject_, string uiName_ = null, string dataPath_ = null) {
            UIMain _uiMain = base.openUI(gameObject_, uiName_,dataPath_);
            _uiMain.uiType = UIType.Pop;
            return _uiMain;
        }
    }
}