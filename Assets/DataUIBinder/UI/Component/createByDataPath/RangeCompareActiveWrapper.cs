using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
namespace DataUIBinder{
    public class RangeCompareActiveWrapper : ComponentWrapper{
        DataPathRangeCompareListener dataPathRangeCompareListener;
        public override void reset(string pattern_,UINode uiNode_ = null){
            clearExpressionList();
            pattern = pattern_;
            resetUINode(uiNode_);
            dataPath = null;
            dataPathRangeCompareListener?.unUse();
            dataPathRangeCompareListener = DataPathRangeCompareListener.reUse();
            if(uiNode_ != null){
                dataPathRangeCompareListener.reset(uiNode_.convertThisOrDataPath(pattern),dataChangeHandle,this);
            }else{
                dataPathRangeCompareListener.reset(pattern,dataChangeHandle,this);
            }
        }
        public virtual void dataChangeHandle(bool result_){
            gameObject.SetActive(result_);
        }
        protected override void OnDestroy(){
            if(isDestroyed){ return; }
            dataPathRangeCompareListener?.unUse();
            dataPathRangeCompareListener = null;
            base.OnDestroy();
        }
    }
}
