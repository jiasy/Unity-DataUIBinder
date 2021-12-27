using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
namespace DataUIBinder{
    public class CompareActiveWrapper : ComponentWrapper{
        DataPathCompareListener dataPathCompareListener;
        public override void reset(string pattern_,UINode uiNode_ = null){
            clearExpressionList();
            pattern = pattern_;
            resetUINode(uiNode_);
            dataPath = null;
            dataPathCompareListener?.unUse();
            dataPathCompareListener = DataPathCompareListener.reUse();
            if(uiNode_ != null){
                dataPathCompareListener.reset(uiNode_.convertThisOrDataPath(pattern),dataChangeHandle,this);
            }else{
                dataPathCompareListener.reset(pattern,dataChangeHandle,this);
            }
#if UNITY_EDITOR
            if(debugRecord && isInit == false){
                isInit = true;
                dc.sv("debug.objectCount.wrapper",dc.gv("debug.objectCount.wrapper").AsInt + 1);
            }
#endif
        }
        public virtual void dataChangeHandle(bool result_){
            gameObject.SetActive(result_);
        }
        protected override void OnDestroy(){
            if(isDestroyed){ return; }
            dataPathCompareListener?.unUse();
            dataPathCompareListener = null;
            base.OnDestroy();
        }
    }
}
