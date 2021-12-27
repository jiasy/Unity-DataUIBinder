using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DataUIBinder {
    //引导层，和遮罩层一起使用。-4
    //UIManager.instance.GuideContainer
    public class GuideContainer : UIContainer {
        public override void Awake() {
            uiType = UIType.Guide;
            base.Awake();

        }
        // public override void Start() {
        //     base.Start();

        // }
        public override UIMain openUI(GameObject gameObject_, string uiName_ = null, string dataPath_ = null) {
            UIMain _uiMain = base.openUI(gameObject_, uiName_,dataPath_);
            _uiMain.uiType = UIType.Guide;
            return _uiMain;
        }
        //开始引导
        public void startGuide() {

        }
        //下一步
        public void nextStep() {

        }
        //结束引导
        public void endGuide() {

        }
    }
}