using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DataUIBinder {
    //提示层，弹出框，ok/cancel 或则 ok。-5
    //UIManager.instance.TipContainer
    public class TipContainer : UIContainer {
        public override void Awake() {
            uiType = UIType.Tip;
            base.Awake();

        }
        // public override void Start() {
        //     base.Start();

        // }
        public override UIMain openUI(GameObject gameObject_, string uiName_ = null, string dataPath_ = null) {
            UIMain _uiMain = base.openUI(gameObject_, uiName_,dataPath_);
            _uiMain.uiType = UIType.Tip;
            return _uiMain;
        }
        public void tipYes(string uiName_, string title_, string content_, System.Action yesAction_ = null) {
            TipMain _tipBase =(TipMain) UIManager.instance.openUI(uiName_);
            _tipBase.setTitleAndContent(title_, content_);
            _tipBase.setYesAndNoCallBack(yesAction_, null);
        }

        public void tipYesNo(string uiName_, string title_, string content_, System.Action yesAction_ = null, System.Action noAction_ = null) {
            TipMain _tipBase =(TipMain) UIManager.instance.openUI(uiName_);
            _tipBase.setTitleAndContent(title_, content_);
            _tipBase.setYesAndNoCallBack(yesAction_, noAction_);
        }
    }
}