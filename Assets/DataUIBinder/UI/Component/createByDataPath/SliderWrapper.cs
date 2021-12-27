using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
namespace DataUIBinder{
    /*
        滑块/进度条 包装
    */
    public class SliderWrapper : ComponentWrapper{
        DataPathRangeCompareListener dataPathRangeCompareListener;
        private Slider _slider = null;
        private Slider slider{
            get{
                if(_slider == null){
                    _slider = gameObject.GetComponent<Slider>();
                }
                return _slider;
            }
        }
        protected override void Awake(){
            base.Awake();
            slider.onValueChanged.AddListener((value_)=>{
                if(!Recoder.isReplay){
                    if(Recoder.isRecord){
                        JSONObject _jsObject = new JSONObject();
                        _jsObject["type"] = "onSlider";
                        _jsObject["uiPath"] = uiNode.uiPath;
                        _jsObject["name"] = gameObject.name;
                        _jsObject["value"] = value_;
                        Recoder.record(_jsObject);
                    }
#if UNITY_EDITOR
                    if(uiNode){
                        LogToFiles.logByType(LogToFiles.LogType.Record, "[ onSlider ] " + uiNode.uiPath + " -> "+gameObject.name + " : " + value_.ToString());
                    }
#endif
                    onValueChanged(value_);
                }else{//播放状态下，无法改变数值
                    string _dataPath = dataPathRangeCompareListener.getSliderDataPath();
                    slider.value = DataCenter.root.getValue(_dataPath).AsFloat;
                }
            });
        }
        public override void reset(string pattern_,UINode uiNode_ = null){
            clearExpressionList();
            pattern = pattern_;
            resetUINode(uiNode_);
            dataPath = null;
            dataPathRangeCompareListener?.unUse();
            dataPathRangeCompareListener = null;
            dataPathRangeCompareListener = DataPathRangeCompareListener.reUse();
            if(uiNode_ != null){
#if UNITY_EDITOR
                if(pattern.isStartsWith("_ui_.")){
                    string _key = pattern.splitWith(":[")[0].splitWith("_ui_.")[1];
                    UINode.isKeyAvailableOnUI(_key);
                }
                if(pattern.isStartsWith("_dt_.")){
                    string _key = pattern.splitWith(":[")[0].splitWith("_dt_.")[1];
                    UINode.isKeyAvailableOnUI(_key);
                }
#endif
                dataPathRangeCompareListener.resetForSlider(uiNode_.convertThisOrDataPath(pattern),dataChangeHandle,this);
            }else{
                dataPathRangeCompareListener.resetForSlider(pattern,dataChangeHandle,this);
            }
        }
        public void onValueChanged(float value_){
            string _dataPath = dataPathRangeCompareListener.getSliderDataPath();
            if(_dataPath != null){
                DataCenter.root.setValue(_dataPath,Convert.ToDouble(value_));
            }
        }
        public void dataChangeHandle(string[] pathList_ ,JSONNode[] valueList_){
            float _cacheValue = 0;
            string _dataPath = null;
            for (int _idx = 0; _idx < pathList_.Length; _idx++) {
	            _dataPath = pathList_[_idx];
                if(_idx == 0){
                    _cacheValue = DataCenter.root.getValue(_dataPath);
                }else{
                    if(_dataPath != null){
                        valueList_[_idx] = DataCenter.root.getValue(_dataPath);
                    }
                }
            }

            if(valueList_[1] > valueList_[2]){
#if UNITY_EDITOR
                throw new Exception("ERROR : 左侧值一定要比右侧值小。不可以出现 大于 的情况");
#endif
            }
            if(slider.minValue != (float)valueList_[1]){
                slider.minValue = (float)valueList_[1];
            }
            if(slider.maxValue != (float)valueList_[2]){
                slider.maxValue = (float)valueList_[2];
            }
            slider.value = _cacheValue;
        }
        protected override void OnDestroy(){
            if(isDestroyed){ return; }
            if(_slider != null){
                slider.onValueChanged.RemoveAllListeners();
                _slider = null;
            }
            dataPathRangeCompareListener?.unUse();
            dataPathRangeCompareListener = null;
            base.OnDestroy();
        }
    }
}
