
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
namespace DataUIBinder{
    public class DataPathListListener:ReUseObj{
        public static bool debugRecord = false;
        public static string fullClassName = nameof(DataUIBinder)+"."+nameof(DataPathListListener);
        public static DataPathListListener reUse(){
            return ReUseObj.reUseObj(fullClassName) as DataPathListListener;
        }
        public override void unUse(){
            destroy();
            base.unUse();
        }
        protected virtual void destroy(){
            for (int _idx = 0; _idx < pathList.Length; _idx++) {
                string _dataPath = pathList[_idx];
                if(_dataPath != null){
                    DataCenter.removeListener(_dataPath,dataChangeHandle);
                }
            }
            pathList = null;
            callback = null;
            isCurrentAllSatisfy = false;
            needAllSatisfy = false;
#if UNITY_EDITOR
            if (debugRecord){
                runningDPLists.Remove(this);
                dc.sv("debug.objectList.dpList",getDPListListAsStr());
            }
#endif
        }
        protected string[] pathList;
        private JSONNodeType[] typeList;
        private object[] lastValueList;
        private bool isCurrentAllSatisfy = false;
        private bool needAllSatisfy = false;
        private Action<List<string>> callback;
#if UNITY_EDITOR
        private static List<DataPathListListener> runningDPLists = new List<DataPathListListener>();
        private string getDPListListAsStr(){//这个拼接可能造成卡顿。
            string _returnStr = "";
            for (int _idx = 0; _idx < runningDPLists.Count; _idx++) {
                for (int _idxInside = 0; _idxInside < runningDPLists[_idx].pathList.Length; _idxInside++) {
                    _returnStr += runningDPLists[_idx].pathList[_idxInside] + ",";
                }
                _returnStr += "\n";
            }
            return _returnStr;
        }
#endif
        public void reset(string[] pathList_,Action<List<string>> callback_,bool needAllSatisfy_ = true){
            pathList = pathList_;
            int pathListLength = pathList.Length;
            typeList = new JSONNodeType[pathListLength];
            lastValueList = new object[pathListLength];
            for (int _idx = 0; _idx < pathListLength; _idx++) {
                lastValueList[_idx] = null;
            }
            callback = callback_;
            needAllSatisfy = needAllSatisfy_;
            isCurrentAllSatisfy = false;
#if UNITY_EDITOR
            int _nullCount = 0;
#endif
            for (int _idx = 0; _idx < pathListLength; _idx++) {
                string _path = pathList_[_idx];
                if(_path != null){
                    if(_path.Contains("_ui_.")||_path.Contains("_dt_.")){
                        throw new Exception("ERROR : 监听路径不能是相对路径 : "+_path + " ，应当是转换过的路径");
                    }
                    DataCenter.addListener(_path,dataChangeHandle);
                }
#if UNITY_EDITOR
                else{
                    _nullCount++;
                }
#endif
            }
#if UNITY_EDITOR
            if(_nullCount == pathList_.Length){
                throw new Exception("ERROR : 路径列表全为空。");
            }
#endif
            dataChangeHandle(null,null);
#if UNITY_EDITOR
            if (debugRecord){
                runningDPLists.Add(this);
                dc.sv("debug.objectList.dpList",getDPListListAsStr());
            }
#endif
        }
        private void changeValueOnIdx(int idx_,JSONNode jsNode_){
            if(jsNode_.Tag == JSONNodeType.String){
                lastValueList[idx_] = jsNode_.AsString;
            }else if(jsNode_.Tag == JSONNodeType.Number){
                lastValueList[idx_] = jsNode_.AsDouble;
            }else if(jsNode_.Tag == JSONNodeType.Boolean){
                lastValueList[idx_] = jsNode_.AsBool;
            }else{
                lastValueList[idx_] = null;    
            }
        }

        protected void dataChangeHandle(string changeDataPath_, JSONNode jsNode_) {
            List<string> _changePathList = new List<string>();
            bool _isChange = false;
            int _pathListLength = pathList.Length;
            JSONNode _jsNode;
            if(needAllSatisfy){//是否需要全满足有值的条件
                if(!isCurrentAllSatisfy){//当前未满足，判断是否全满足。
                    JSONNode[] _jsNodeList = new JSONNode[_pathListLength];
                    for (int _idx = 0; _idx < _pathListLength; _idx++) {
                        string _dataPath = pathList[_idx];
                        if(_dataPath != null){
                            _jsNode = DataCenter.root.getValue(_dataPath);
                            _jsNodeList[_idx] = _jsNode;
                            if(_jsNode == null){
                                return;
                            }
                        }else{
                            _jsNodeList[_idx] = null;
                        }
                    }
                    for (int _idx = 0; _idx < _pathListLength; _idx++) {
                        _jsNode = _jsNodeList[_idx];
                        if(_jsNode != null){
                            typeList[_idx] = _jsNode.Tag;
                            changeValueOnIdx(_idx,_jsNode);
                            _changePathList.Add(pathList[_idx]);
                        }else{
                            typeList[_idx] = JSONNodeType.None;
                            lastValueList[_idx] = null;
                        }
                    }
                    isCurrentAllSatisfy = true;
                    _isChange = true;
                }else{//当前全满足，看哪一项变化。是否变化
                    for (int _idx = 0; _idx < _pathListLength; _idx++) {
                        if(pathList[_idx] == changeDataPath_){//当前变化的那一项
                            _jsNode = DataCenter.root.getValue(changeDataPath_);
                            if(typeList[_idx] != JSONNodeType.None ){
                                if(lastValueList[_idx] != null &&(_jsNode == null || _jsNode.Tag == JSONNodeType.NullValue)){
                                    lastValueList[_idx] = null;
                                    _isChange = true;
                                }else if(lastValueList[_idx] == null &&(_jsNode == null || _jsNode.Tag == JSONNodeType.NullValue)){
                                    _isChange = false;
                                }else if(_jsNode.Tag != typeList[_idx]){
                                    typeList[_idx] = _jsNode.Tag;
                                    changeValueOnIdx(_idx,_jsNode);
                                    _isChange = true;
                                }else{
                                    if(lastValueList[_idx] == null){
                                        changeValueOnIdx(_idx,_jsNode);
                                        _isChange = true;
                                    }else{
                                        if(typeList[_idx] == JSONNodeType.String){
                                            string _targetStr = _jsNode.AsString;
                                            if((string)lastValueList[_idx] != _targetStr){
                                                lastValueList[_idx] = _targetStr;
                                                _isChange = true;
                                            }
                                        }else if(typeList[_idx] == JSONNodeType.Number){
                                            double _targetDouble = _jsNode.AsDouble;
                                            if((double)lastValueList[_idx] != _targetDouble){
                                                lastValueList[_idx] = _targetDouble;
                                                _isChange = true;
                                            }
                                        }else if(typeList[_idx] == JSONNodeType.Boolean){
                                            bool _targetBool = _jsNode.AsBool;
                                            if((bool)lastValueList[_idx] != _targetBool){
                                                lastValueList[_idx] = _targetBool;
                                                _isChange = true;
                                            }
                                        }else{
                                            lastValueList[_idx] = null;
                                            _isChange = true;
                                        }
                                    }
                                }
                            }
                            if(_isChange){
                                _changePathList.Add(changeDataPath_);
                            }
                            break;
                        }
                    }
                }
            }else{
                if(changeDataPath_ != null){
                    _isChange = true;
                    _changePathList.Add(changeDataPath_);
                }else{//// - 初始化的时候，全发
                    _isChange = true;
                    for (int _idx = 0; _idx < pathList.Length; _idx++) {
                        _changePathList.Add(pathList[_idx]);
                    }
                }
            }
            if(_isChange){
                if(callback != null){
                    callback(_changePathList);
                }else{
                    throw new Exception("ERROR : 没有监听器");
                }
            }
        }
    }
}