using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
namespace DataUIBinder{
    public class DataPathRangeCompareListener:DataPathCompareListener{
        public new static string fullClassName = nameof(DataUIBinder)+"."+nameof(DataPathRangeCompareListener);
        public new static DataPathRangeCompareListener reUse(){
            return ReUseObj.reUseObj(fullClassName) as DataPathRangeCompareListener;
        }
        // public override void unUse(){
        //     base.unUse();
        // }
        protected override void destroy(){
            callbackForSlider = null;
            base.destroy();
        }
        protected bool isLeftClose = false;
        protected bool isRightClose = false;
        private Action<string[],JSONNode[]> callbackForSlider = null;
        protected override void setCompareType(JSONNodeType compareType_){
            if(compareType_ != JSONNodeType.Number){
                throw new Exception("ERROR : " + pattern + " 范围比较只能是 Number 型比较");
            }
            compareType = compareType_;
        }
        public override void reset(string pattern_,Action<bool> callback_,ComponentWrapper wrapper_){
            valueList = new JSONNode[3]{null,null,null};
            callback = callback_;
            pattern = pattern_;
            firstTime = true;
            if(pattern.IndexOf(":[",0,StringComparison.Ordinal)>0){
                isLeftClose = true;
            }
            if(pattern.IndexOf(']')>0){
                isRightClose = true;
            }
            string[] _dataPathList = pattern.getRangeCompareArray();
            _dataPathList = resetByCompareArray(_dataPathList,wrapper_);
            base.reset(_dataPathList,dataChangeHandle,true);
            if(compareType != JSONNodeType.None && compareType != JSONNodeType.Number){
                throw new Exception("ERROR : " + pattern + " 范围比较只能是 Number 型比较，或数据路径内容比较");
            }
        }
        protected new void dataChangeHandle(List<string> _){
            for (int _idx = 0; _idx < pathList.Length; _idx++) {
	            string _dataPath = pathList[_idx];
                if(_dataPath != null){
                    valueList[_idx] = DataCenter.root.getValue(_dataPath);
                }
            }
            if(valueList[1] > valueList[2]){
#if UNITY_EDITOR
                throw new Exception("ERROR : 左侧值一定要比右侧值小。不可以出现 大于 或者 等于 的情况");
#endif
            }
            bool _conditionLeft;
            if(isLeftClose){
                _conditionLeft = valueList[0] >= valueList[1];
            }else{
                _conditionLeft = valueList[0] > valueList[1];
            }
            bool _conditionRight;
            if(isRightClose){
                _conditionRight = valueList[0] <= valueList[2];
            }else{
                _conditionRight = valueList[0] < valueList[2];
            }
            if(callback != null){
                bool _isConditionsEstablished = _conditionLeft && _conditionRight;
                if(firstTime){
                    callback(_isConditionsEstablished);
                    lastResult = _isConditionsEstablished;
                    firstTime = false;
                }else{
                    if(lastResult != _isConditionsEstablished){
                        callback(_isConditionsEstablished);
                        lastResult = _isConditionsEstablished;
                    }
                }
            }else{
                throw new Exception("ERROR : 回调不存在。");
            }
        }
        public void resetForSlider(string pattern_,Action<string[],JSONNode[]> callbackForSlider_,ComponentWrapper wrapper_){
            valueList = new JSONNode[3]{null,null,null};
            callbackForSlider = callbackForSlider_;
            pattern = pattern_;

            if(pattern.IndexOf(":[",0,StringComparison.Ordinal)>0){
                isLeftClose = true;
            }
            if(pattern.IndexOf(']')>0){
                isRightClose = true;
            }

            string[] _dataPathList = pattern.getRangeCompareArray();
            _dataPathList = resetByCompareArray(_dataPathList,wrapper_);

#if UNITY_EDITOR
            if(_dataPathList[0].isStartsWith("temp.calculation.")){
                throw new Exception("ERROR : 滑块值不能是算式");
            }
            double _tempResult;
            if(_dataPathList[0].IndexOf('.')<0 || double.TryParse(_dataPathList[0],out _tempResult)){
                throw new Exception("ERROR : 滑块值必须指定关联路径");
            }
#endif

            base.reset(_dataPathList,dataChangeHandleForSlider,true);

            if(!isRightClose || !isLeftClose){
                throw new Exception("ERROR : " + pattern + "必须是闭合空间");
            }
            if(pathList[0] == null){
                throw new Exception("ERROR : " + pattern + " 第一个参数，必须是路径，否者无法关联值。");
            }
            if(compareType != JSONNodeType.None && compareType != JSONNodeType.Number){
                throw new Exception("ERROR : " + pattern + " 范围比较只能是 Number 型比较，或数据路径内容比较");
            }
        }
        protected void dataChangeHandleForSlider(List<string> _){
            if(callbackForSlider != null){
                callbackForSlider(pathList,valueList);
            }else{
                throw new Exception("ERROR : " + pattern + " 不存在监听器");
            }
        }
        public string getSliderDataPath(){
            return pathList[0];
        }
    }
}