using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DataUIBinder{
    //加载提示，网络断线重连提示。-6
    //UIManager.instance.LoadingContainer
    public class LoadingContainer : UIContainer {
        public override void Awake () {
            uiType = UIType.Loading;
            base.Awake ();

        }
        // public override void Start () {
        //     base.Start ();

        // }
        public override UIMain openUI(GameObject gameObject_,string uiName_ = null,string dataPath_ = null){
            UIMain _uiMain = base.openUI (gameObject_,uiName_,dataPath_);
            _uiMain.uiType = UIType.Loading;
            return _uiMain;
        }
        public void startLoading(){
            
        }
        public void endLoading(){

        }
    }
}