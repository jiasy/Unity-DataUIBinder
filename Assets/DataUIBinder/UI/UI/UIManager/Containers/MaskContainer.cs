using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DataUIBinder {
    //遮罩层，只有一层，封锁用户操作用。-3
    //UIManager.instance.MaskContainer
    public class MaskContainer : UIContainer {
        private MaskMain currentMaskUIMain = null;
        private System.Func<bool, bool> whenClickFunc = null;
        public override void Awake() {
            uiType = UIType.Mask;
            base.Awake();

        }
        // public override void Start() {
        //     base.Start();

        // }
        public override UIMain openUI(GameObject gameObject_, string uiName_ = null, string dataPath_ = null) {
            UIMain _uiMain = base.openUI(gameObject_, uiName_,dataPath_);
            _uiMain.uiType = UIType.Mask;
            return _uiMain;
        }
        public bool doMask(string uiName_) {
            currentMaskUIMain =(MaskMain) UIManager.instance.openUI(uiName_);
            if(currentMaskUIMain == null) {
                return false;
            }
            return true;
        }
        public bool unDoMask(bool force_) {
            if(currentMaskUIMain == null) {
                return false;
            }
            return closeUI(currentMaskUIMain, force_);
        }
        public void focusOn(string uiName_, string goName_, System.Func<bool, bool> whenClickFunc_ = null) {
            if(currentMaskUIMain == null) {
                Debug.LogError("ERROR " + System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName + " -> " + new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name + " : " +
                    "没有遮罩层");
                return;
            }
            UIMain _uiMain = UIManager.instance.getUI(uiName_);
            //TODO
            //currentMaskUIMain.focusOnTransfrom(_uiMain[goName_]);
            //currentMaskUIMain.whenClickFunc = whenClickFunc_;
        }
        public void focusOnItem(string uiName_, string listName_, int dataIdx_, string goName_) {
            if(currentMaskUIMain == null) {
                Debug.LogError("ERROR " + System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName + " -> " + new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name + " : " +
                    "没有遮罩层");
                return;
            }
            UIMain _uiMain = UIManager.instance.getUI(uiName_);
            //TODO
            //currentMaskUIMain.focusOnTransfrom(_uiMain[goName_]);
            //currentMaskUIMain.whenClickFunc = whenClickFunc_;
        }
    }
}