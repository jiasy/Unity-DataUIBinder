using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
namespace DataUIBinder{
    public class DataPathCompareListener:DataPathListListener{
        public new static string fullClassName = nameof(DataUIBinder)+"."+nameof(DataPathCompareListener);
        public new static DataPathCompareListener reUse(){
            return ReUseObj.reUseObj(fullClassName) as DataPathCompareListener;
        }
        // public override void unUse(){
        //     base.unUse();
        // }
        protected override void destroy(){
            callback = null;
            for (int _idx = 0; _idx < valueList.Length; _idx++) {
                valueList[_idx] = null;
            }
            compareType = JSONNodeType.None;
            compareSymbol = CompareType.None;
            pattern = null;
            firstTime = true;
            base.destroy();
        }
        public enum CompareType{
            None = -1,
            Equal = 0,
            NotEqual = 1,
            BiggerEqual = 2,
            Bigger = 3,
            SmallerEqual = 4,
            Smaller = 5
        }
        protected JSONNode[] valueList;
        private CompareType compareSymbol = CompareType.None;
        protected JSONNodeType compareType = JSONNodeType.None;
        protected Action<bool> callback;
        protected string pattern;
        protected bool lastResult = false;
        protected bool firstTime = true;
        public virtual void reset(string pattern_,Action<bool> callback_,ComponentWrapper wrapper_){
            callback = callback_;
            pattern = pattern_;
            firstTime = true;
            valueList = new JSONNode[2]{null,null};
            if(pattern.Contains("==")){
                compareSymbol = CompareType.Equal;
            }else if(pattern.Contains("!=")){
                compareSymbol = CompareType.NotEqual;
            }else if(pattern.Contains(">=")){
                compareSymbol = CompareType.BiggerEqual;
            }else if(pattern.Contains(">")){
                compareSymbol = CompareType.Bigger;
            }else if(pattern.Contains("<=")){
                compareSymbol = CompareType.SmallerEqual;
            }else if(pattern.Contains("<")){
                compareSymbol = CompareType.Smaller;
            }else{
                throw new Exception("ERROR : " + pattern + " 不存在比较符号");
            }

            string[] _dataPathList = pattern.getCompareArray();
            _dataPathList = resetByCompareArray(_dataPathList,wrapper_);
            base.reset(_dataPathList,dataChangeHandle,true);

            if(compareType == JSONNodeType.String){
                if(compareSymbol != CompareType.Equal && compareSymbol != CompareType.NotEqual){
                    throw new Exception("ERROR : " + pattern + " 字符串类型只支持 == / != " + compareSymbol.ToString() );
                }
            }else if(compareType == JSONNodeType.Boolean){
                if(compareSymbol != CompareType.Equal && compareSymbol != CompareType.NotEqual){
                    throw new Exception("ERROR : " + pattern + " 布尔类型只支持 == / !="+compareSymbol.ToString());
                }
            }else if(compareType == JSONNodeType.NullValue){
                if(compareSymbol != CompareType.Equal && compareSymbol != CompareType.NotEqual){
                    throw new Exception("ERROR : " + pattern + " 空类型只支持 == / !="+compareSymbol.ToString());
                }
            }
        }
        protected virtual void setCompareType(JSONNodeType compareType_){
            compareType = compareType_;
        }
        protected string[] resetByCompareArray(string[] compareList_,ComponentWrapper wrapper_ = null){
            string[] _backDataPathList = new string[compareList_.Length];
            for (int _idx = 0; _idx < compareList_.Length; _idx++) {
                string _dataPathOrValue = compareList_[_idx];
                if(_dataPathOrValue.IndexOf('.') > 0 ){                    
                    double _doubleFromString;
                    if (double.TryParse(_dataPathOrValue, out _doubleFromString)){
                        valueList[_idx] = new JSONNumber(_doubleFromString);
                        setCompareType(JSONNodeType.Number);
                        continue;
                    }
                    if((_dataPathOrValue.Contains("_ui_.") || _dataPathOrValue.Contains("_dt_."))&& wrapper_!=null){
                        throw new Exception("ERROR : " + pattern + " 中 " +_dataPathOrValue + " 是相对路径。在这个层面应当不会存在");
                    }
                    if(wrapper_ != null){
                        _dataPathOrValue = wrapper_.getExpressionPath(_dataPathOrValue);
                    }
                    _backDataPathList[_idx] = _dataPathOrValue;
                }else{
                    int _intFromString;
                    if (int.TryParse(_dataPathOrValue, out _intFromString)){
                        valueList[_idx] = new JSONNumber((double)_intFromString);
                        setCompareType(JSONNodeType.Number);
                    }else if(_dataPathOrValue.ToLower() == "false"){
                        valueList[_idx] = new JSONBool(false);
                        setCompareType(JSONNodeType.Boolean);
                    }else if(_dataPathOrValue.ToLower() == "true"){
                        valueList[_idx] = new JSONBool(true);
                        setCompareType(JSONNodeType.Boolean);
                    }else if(_dataPathOrValue.ToLower() == "null"){
                        valueList[_idx] = null;
                        setCompareType(JSONNodeType.NullValue);
                    }else{
                        valueList[_idx] = new JSONString(_dataPathOrValue);
                        setCompareType(JSONNodeType.String);
                    }
                }
            }
            return _backDataPathList;
        }
        protected virtual void dataChangeHandle(List<string> _){
            JSONNode _jsNode_0 = pathList[0] != null ? DataCenter.root.getValue(pathList[0]) : valueList[0];
            JSONNode _jsNode_1 = pathList[1] != null ? DataCenter.root.getValue(pathList[1]) : valueList[1];
            bool _isConditionsEstablished = false;
            if(compareType == JSONNodeType.None){//没有给定值，全靠路径内容比较
                if(_jsNode_0 == null || _jsNode_1 == null){//任意内容为空就返回
                    return;
                }
                if(_jsNode_0.Tag != _jsNode_1.Tag){
                    throw new Exception("ERROR : " + pattern + " 类型不同，无法比较。" + _jsNode_0.TagAsString + " - " + _jsNode_1.TagAsString);
                }
                if(compareSymbol == CompareType.Equal || compareSymbol == CompareType.NotEqual){//0==. 1!=.
                    if(_jsNode_0.Tag == JSONNodeType.String){
                        _isConditionsEstablished = _jsNode_0.AsString == _jsNode_1.AsString;
                    }else if(_jsNode_0.Tag == JSONNodeType.Boolean){
                        _isConditionsEstablished = _jsNode_0.AsBool == _jsNode_1.AsBool;
                    }else if(_jsNode_0.Tag == JSONNodeType.Number){
                        _isConditionsEstablished = _jsNode_0.AsDouble == _jsNode_1.AsDouble;
                    }else{
                        throw new Exception("ERROR : " + pattern + " 意外类型，无法比较" + _jsNode_0.TagAsString);
                    }
                    if(compareSymbol == CompareType.NotEqual){
                        _isConditionsEstablished = !_isConditionsEstablished;
                    }
                }else{// 2>=. 3>. 4<=. 5<
                    if(_jsNode_0.Tag != JSONNodeType.Number){
                        throw new Exception("ERROR : " + pattern + " 非 数字 类型，无法比较大小");    
                    }
                    if(compareSymbol == CompareType.BiggerEqual){
                        _isConditionsEstablished = _jsNode_0.AsDouble >= _jsNode_1.AsDouble;
                    }else if(compareSymbol == CompareType.Bigger){
                        _isConditionsEstablished = _jsNode_0.AsDouble > _jsNode_1.AsDouble;
                    }else if(compareSymbol == CompareType.SmallerEqual){
                        _isConditionsEstablished = _jsNode_0.AsDouble <= _jsNode_1.AsDouble;
                    }else if(compareSymbol == CompareType.Smaller){
                        _isConditionsEstablished = _jsNode_0.AsDouble < _jsNode_1.AsDouble;
                    }
                }
            }else if(compareType == JSONNodeType.NullValue){//空比较的话，两个都是空就成立
                if(compareSymbol == CompareType.Equal){
                    _isConditionsEstablished = _jsNode_0 == null && _jsNode_1 == null;
                }else if(compareSymbol == CompareType.NotEqual){
                    _isConditionsEstablished = !(_jsNode_0 == null && _jsNode_1 == null);
                }else{
                    throw new Exception("ERROR : JSONNodeType.NullValue " + pattern + " 意外类型，无法比较" + _jsNode_0.TagAsString);
                }
            }else if(compareType == JSONNodeType.Boolean){//布尔
                if(compareSymbol == CompareType.Equal){
                    _isConditionsEstablished = _jsNode_0.AsBool == _jsNode_1.AsBool;
                }else if(compareSymbol == CompareType.NotEqual){
                    _isConditionsEstablished = _jsNode_0.AsString != _jsNode_1.AsString;
                }else{
                    throw new Exception("ERROR : JSONNodeType.Boolean " + pattern + " 意外类型，无法比较" + _jsNode_0.TagAsString);
                }
            }else if(compareType == JSONNodeType.String){//字符串 的比较
                if(compareSymbol == CompareType.Equal){
                    _isConditionsEstablished = _jsNode_0.AsString == _jsNode_1.AsString;
                }else if(compareSymbol == CompareType.NotEqual){
                    _isConditionsEstablished = _jsNode_0.AsString != _jsNode_1.AsString;
                }else{
                    throw new Exception("ERROR : JSONNodeType.String " + pattern + " 意外类型，无法比较 " + _jsNode_0.TagAsString);
                }
            }else if(compareType == JSONNodeType.Number){
                if(compareSymbol == CompareType.Equal){//0==. 1!=. 2>=. 3>. 4<=. 5<
                    _isConditionsEstablished = _jsNode_0 == _jsNode_1;
                }else if(compareSymbol == CompareType.NotEqual){
                    _isConditionsEstablished = _jsNode_0 != _jsNode_1;
                }else if(compareSymbol == CompareType.BiggerEqual){
                    _isConditionsEstablished = _jsNode_0 >= _jsNode_1;
                }else if(compareSymbol == CompareType.Bigger){
                    _isConditionsEstablished = _jsNode_0 > _jsNode_1;
                }else if(compareSymbol == CompareType.SmallerEqual){
                    _isConditionsEstablished = _jsNode_0 <= _jsNode_1;
                }else if(compareSymbol == CompareType.Smaller){
                    _isConditionsEstablished = _jsNode_0 < _jsNode_1;
                }
            }
            if(callback != null){
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
                throw new Exception("ERROR : 回调不存在");
            }
        }
    }
}