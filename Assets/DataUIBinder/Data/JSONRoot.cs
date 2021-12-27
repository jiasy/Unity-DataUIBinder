using SimpleJSON;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
namespace DataUIBinder{
    public class JSONRoot : JSONObject {
        private Dictionary<string, JSONNode> justChangeValueDict = new Dictionary<string, JSONNode>();
        private Dictionary<string, JSONNode> dispatchingJustChangeValueDict = null;
        private Dictionary<string,JSONNode> pathAndValueDict = new Dictionary<string, JSONNode>();
        public DataChangeDispatcher dataChangeDispatcher = new DataChangeDispatcher();
        public bool isDataChanged = false;
        public JSONRoot(string[] namespaceList_){
            root = this;
            for (int _idx = 0; _idx < namespaceList_.Length; _idx++) {
                var _namespace = namespaceList_[_idx];
                if(_namespace.IndexOf('.')>=0){
                    throw new Exception("ERROR : 命名空间不可以是路径，只可以是一个键名 : "+_namespace);
                }
                setAsNamespace(_namespace,new JSONObject());
            }
        }
        public void reset(string[] namespaceList_){
            List<string> _willRemovePath = new List<string>();
            string _namespace;
            string _nameSpacePrefix;
            string _currentKey;
            for (int _idx = 0; _idx < namespaceList_.Length; _idx++) {
                _namespace = namespaceList_[_idx];
                if(this[_namespace]){
                    this[_namespace] = new JSONObject();
                    _nameSpacePrefix = _namespace + ".";
                    var _enume = pathAndValueDict.GetEnumerator();
                    while (_enume.MoveNext()) {
                        _currentKey = _enume.Current.Key;
                        if(_currentKey.isStartsWith(_nameSpacePrefix)){
                            _willRemovePath.Add(_currentKey);
                        }
                    }
                    _enume.Dispose();
                }else{
                    throw new Exception("ERROR : " + _namespace + " not exist.");
                }
            }
            for (int _idx = 0; _idx < _willRemovePath.Count; _idx++) {
                pathAndValueDict.Remove( _willRemovePath[_idx] );
            }
        }
        public void setValue(string dataPath_,object value_){
            mergeValue(dataPath_,value_,true);
        }
        public void mergeValue(string dataPath_,object value_,bool isSet_ = false){
#if UNITY_EDITOR
            if(isSet_ == false &&((value_ is string)||(value_ is bool)||(JSONNumber.IsNumeric(value_))||(value_ is Array))){
                throw new Exception("ERROR : merge to dataPath must use dictionaty.");
            }
            if(dataPath_.IndexOf('.')<0){
                if(value_!=null&&((value_ is string)||(value_ is bool)||(JSONNumber.IsNumeric(value_))||(value_ is Array))){
                    throw new Exception("ERROR : only dictionary can set/merge to namespace.");
                }
            }
#endif
            JSONNode  _jsNode;
            if(value_ == null){
                this[dataPath_] = null;
            }else if(value_ is string){
                _jsNode = getValue(dataPath_);
                string _valueStr = (string)value_;
                if(_jsNode != null){
                    if(_valueStr != _jsNode.AsString){
                        _jsNode.AsString = _valueStr;
                        changeValue(dataPath_,_jsNode);
                    }
                }else{
                    this[dataPath_] = new JSONString(_valueStr);
                }
            }else if(JSONNumber.IsNumeric(value_)){
                _jsNode = getValue(dataPath_);
                double _valueDouble = Convert.ToDouble(value_);
                if(_jsNode != null){
                    if(_valueDouble != _jsNode.AsDouble){
                        _jsNode.AsDouble = _valueDouble;
                        changeValue(dataPath_,_jsNode);
                    }
                }else{
                    this[dataPath_] = new JSONNumber(_valueDouble);
                }
            }else if(value_ is bool){
                _jsNode = getValue(dataPath_);
                bool _valueBool = (bool)value_;
                if(_jsNode != null){
                    if(_valueBool != _jsNode.AsBool){
                        _jsNode.AsBool  = _valueBool;
                        changeValue(dataPath_,_jsNode);
                    }
                }else{
                    this[dataPath_] = new JSONBool(_valueBool);
                }
            }else if(value_ is Array) {
                mergeValue(dataPath_,JSONNode.convertArrayToJsonList((Array)value_),isSet_);
            }else if(value_ is object) {
                mergeValue(dataPath_,JSONNode.convertObjectToJsonDict(value_),isSet_);
            }else{
                throw new Exception("ERROR : set to " + dataPath_+ " ,value is unknow type : " + value_.ToString());
            }
        }
        public void setValue(string dataPath_,JSONNode jsonNode_){
            mergeValue(dataPath_,jsonNode_,true);
        }
        public void mergeValue(string dataPath_,JSONNode jsNode_,bool isSet_ = false){
#if UNITY_EDITOR
            if(dataPath_[0] == '.'){
                throw new Exception("ERROR : first char can not be '.' ");
            }
#endif
            if(dataPath_.IndexOf('.') > 0){
                if(jsNode_ == null){
                    if(isSet_){
                        setToObjectRelativePath(dataPath_,null);
                    }else{
                        throw new Exception("ERROR : "+dataPath_+ " can not merge null。If you want set null to path,please use setValue.");
                    }
                }else{
                    JSONNode _jsNodeOnPath = getValue(dataPath_);
                    if(_jsNodeOnPath != null){
                        if(isSet_){
#if UNITY_EDITOR
                            if(
                                _jsNodeOnPath.Tag != jsNode_.Tag &&(_jsNodeOnPath.Tag != JSONNodeType.Number || 
                                jsNode_.Tag != JSONNodeType.Number)
                            ){
                                if (jsNode_.Tag == JSONNodeType.Object){
                                    throw new Exception("ERROR : " + dataPath_ + " 当前的类型 " + _jsNodeOnPath.TagAsString + " 和目标类型 "+jsNode_.TagAsString + " 不一致。\n"+_jsNodeOnPath.ToString() + " -> "+jsNode_.ToString()+"\n请检查是否忘记写.AsXX来指定类型");
                                }else{
                                    throw new Exception("ERROR : " + dataPath_ + " 当前的类型 " + _jsNodeOnPath.TagAsString + " 和目标类型 "+jsNode_.TagAsString + " 不一致。\n"+_jsNodeOnPath.ToString() + " -> "+jsNode_.ToString());
                                }
                            }
#endif
                            this[dataPath_] = jsNode_;
                        }else{
                            if(_jsNodeOnPath.Tag == JSONNodeType.Object){
                                _jsNodeOnPath.merge(jsNode_);
                                changeValue(_jsNodeOnPath.dataPath,_jsNodeOnPath);
                            }else if(_jsNodeOnPath.Tag == JSONNodeType.Array){
                                this[dataPath_] = jsNode_;
                                changeValue(dataPath_,jsNode_);
                            }else{
#if UNITY_EDITOR
                                if(_jsNodeOnPath.Tag != jsNode_.Tag &&(_jsNodeOnPath.Tag != JSONNodeType.Number || jsNode_.Tag != JSONNodeType.Number)){
                                    throw new Exception("ERROR : " + dataPath_ + " 当前的类型 " + _jsNodeOnPath.TagAsString + " 和目标类型 "+jsNode_.TagAsString + " 不一致。\n"+_jsNodeOnPath.ToString() + " -> "+jsNode_.ToString());
                                }
#endif
                                this[dataPath_] = jsNode_;
                            }
                        }
                    }else{
                        this[dataPath_] = jsNode_;
                    }
                    _jsNodeOnPath = null;
                }
            }else{
                if(jsNode_.Tag != JSONNodeType.Object){
                    throw new Exception("ERROR : 向 "+dataPath_+" 命名空间节点合并，值必须是一个字典。");
                }
                if(isSet_){
                    List<string> _strList = CSharpExtensionUtils.StrListInstance;
                    _strList.Clear();
                    var _keysEnume = this[dataPath_].Keys.GetEnumerator();
                    while (_keysEnume.MoveNext()) {
                        _strList.Add(_keysEnume.Current);
                    }
                    for (int _idx = 0; _idx < _strList.Count; _idx++) {
                        this[dataPath_].Remove(_strList[_idx]);
                    }
                    _strList.Clear();
                }
                this[dataPath_].merge(jsNode_);
            }
        }
        public override void setToObjectRelativePath(string dataPath_,JSONNode jsonNode_){
            JSONNode  _jsNode = getValue(dataPath_);
            if(_jsNode == null){
                setToRelativePath(dataPath_,jsonNode_);
            }else{
                if(jsonNode_ == null){
                    setToRelativePath(dataPath_,JSONNull.CreateOrGet());
                }else if(jsonNode_.Tag == JSONNodeType.NullValue){
                    setToRelativePath(dataPath_,JSONNull.CreateOrGet());
                }else{
                    if(_jsNode.Tag != jsonNode_.Tag){
                        throw new Exception("ERROR : type not match : " + jsonNode_.TagAsString + " -> " + jsonNode_.TagAsString);
                    }
                    if(_jsNode.Tag == JSONNodeType.String){
                        _jsNode.AsString = jsonNode_.AsString;
                        changeValue(dataPath_,_jsNode);
                    }else if(_jsNode.Tag == JSONNodeType.Number){
                        _jsNode.AsDouble = jsonNode_.AsDouble;
                        changeValue(dataPath_,_jsNode);
                    }else if(_jsNode.Tag == JSONNodeType.Boolean){
                        _jsNode.AsBool = jsonNode_.AsBool;
                        changeValue(dataPath_,_jsNode);
                    }else{
#if UNITY_EDITOR
                        if(_jsNode.Tag != JSONNodeType.Object && _jsNode.Tag != JSONNodeType.Array){
                            throw new Exception("ERROR : unsupport type : " + _jsNode.TagAsString);
                        }
#endif
                        setToRelativePath(dataPath_,jsonNode_);
                    }
                }
            }
        }
        public override JSONNode getJsonNodeByRelativePath(string dataPath_,object value_ = null,bool setDefaultToDataPath_ = false){
            if(value_ != null){
                JSONNode _jsNode = base.getJsonNodeByRelativePath(dataPath_,value_,setDefaultToDataPath_);
                if(setDefaultToDataPath_){
                    changeValue(dataPath_,_jsNode);
                }
                return _jsNode;
            }else{
                return getValue(dataPath_);
            }
        }
        public JSONNode getValue(string dataPath_){
            JSONNode _jsNodeOnPath = null;
            bool _findBoo = true;
            if(!justChangeValueDict.TryGetValue(dataPath_,out _jsNodeOnPath)){
                _findBoo = false;
            }else{
                if (_jsNodeOnPath.dataPath == null){
                    _findBoo = false;
                    _jsNodeOnPath = null;
                }else{
                    if(_jsNodeOnPath.Tag == JSONNodeType.NullValue){
                        _findBoo = true;
                        _jsNodeOnPath = null;
                    }
                }
            }
            if (_findBoo == false){
                if(dispatchingJustChangeValueDict == null){
                    pathAndValueDict.TryGetValue(dataPath_,out _jsNodeOnPath);
                }else{
                    if(!dispatchingJustChangeValueDict.TryGetValue(dataPath_,out _jsNodeOnPath)){
                        pathAndValueDict.TryGetValue(dataPath_,out _jsNodeOnPath);
                    }
                }
            }
            return _jsNodeOnPath;
        }
        public override JSONNode this[string key_]{
            get{
                if(key_.IndexOf('.')>0){
                    return getValue(key_);
                }
                JSONNode _jsNodeForKey = null;
                if(TryGetValue(key_, out _jsNodeForKey)){
                    return _jsNodeForKey;
                }
                return null;
            }
            set{
                if(key_ == null){
                    throw new Exception("ERROR :  取值时，键为空");
                }
                if(key_.IndexOf('.')<0){
                    throw new Exception("ERROR :  不可以直接对命名空间级别赋值 : "+key_+"\n    不在定义的命名空间内。请确保 DataCenter.init 方法中指定过此命名空间。");
                }
                setToObjectRelativePath(key_,value);
            }
        }
        public void changeValue(string dataPath_,JSONNode jsonNode_){
#if UNITY_EDITOR
            if(jsonNode_ != null && jsonNode_.dataPath != dataPath_){
                throw new Exception("ERROR : \'" + dataPath_ + "\' is out of sync : \'"+jsonNode_.dataPath+"\'");
            }
#endif
            justChangeValueDict[dataPath_] = jsonNode_;
            isDataChanged = true;
        }
        public void dispatchJustChange(){
            if(isDataChanged == false){
                return;
            }
            string _dataPath;
            JSONNode _jsNodeOnPath = null;
            bool _needClear = false;
            while( justChangeValueDict.Keys.Count>0 ){
                dispatchingJustChangeValueDict = justChangeValueDict;
                justChangeValueDict = new Dictionary<string, JSONNode>();
                var _dispatchingDictEnume = dispatchingJustChangeValueDict.GetEnumerator();
                while ( _dispatchingDictEnume.MoveNext() ) {
                    _dataPath = _dispatchingDictEnume.Current.Key;
                    _jsNodeOnPath = _dispatchingDictEnume.Current.Value;
                    if (_jsNodeOnPath != null){
                        if(_jsNodeOnPath.dataPath == null){//有可能废弃掉了
                            //UnityEngine.Debug.LogWarning("路径数据废弃 : \'"+_dataPath+"\'");
                            _jsNodeOnPath = null;
                        }else if(_jsNodeOnPath.dataPath != _dataPath){
#if UNITY_EDITOR
                            UnityEngine.Debug.LogError("不同步 : \'"+_dataPath+"\' -> \'"+_jsNodeOnPath.dataPath+"\'");
#endif
                            _jsNodeOnPath.root = this;
                            _jsNodeOnPath.dataPath = _dataPath;
                        }
                    }
                    dataChangeDispatcher.dispatchEvent(_dataPath,_jsNodeOnPath);
#if UNITY_EDITOR
                    if(!_dataPath.isStartsWith("temp.calculation")){
                        StringBuilder _sb = CSharpExtensionUtils.SBInstance;
                        _sb.Clear();
                        _sb.Append("    ");
                        _sb.Append(_dataPath);
                        _sb.Append(" : ");
                        _sb.Append(_jsNodeOnPath == null ? "<NULL>" : _jsNodeOnPath.AsPrintString);
                        pathAndValueList.Add(_sb.ToString());
                        _sb.Clear();
                    }
#endif
                    _needClear = false;
                    if( _jsNodeOnPath == null ){
                        _needClear = true;
                    }else{
                        if(  _jsNodeOnPath.Tag == JSONNodeType.NullValue || _jsNodeOnPath.dataPath == null ){
                            _needClear = true;
                        }
                    }
                    if( _needClear ){
                        if( pathAndValueDict.ContainsKey( _dataPath ) ){
                            pathAndValueDict.Remove( _dataPath );
                        }
                    }else{
                        pathAndValueDict[ _dataPath ] = _jsNodeOnPath;
                    }
                }
                _dispatchingDictEnume.Dispose(); 
                dispatchingJustChangeValueDict.Clear();
                dispatchingJustChangeValueDict = null;
            }
#if UNITY_EDITOR
            printJustChangeValueDict();
#endif
            isDataChanged = false;
        }
#if UNITY_EDITOR
        List<string> pathAndValueList = new List<string>();
        public void printJustChangeValueDict(){
            if(pathAndValueList.Count > 0){
                LogToFiles.printLogList(
                    LogToFiles.LogType.PathValue,
                    "path : value ---------------------------------------------------- : "+pathAndValueList.Count.ToString(),
                    pathAndValueList
                );
                pathAndValueList.Clear();
            }
        }
#endif
    }
}