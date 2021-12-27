using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.ComponentModel;        
using System.Reflection;
using DataUIBinder;

namespace SimpleJSON{
    public partial class JSONObject{
        public void setAsNamespace(string namespace_,JSONObject jsObject_){
            m_Dict.Add(namespace_, jsObject_);
            jsObject_.root = this as JSONRoot;
            jsObject_.dataPath = namespace_;
        }
        public bool ContainsKey(string key_){
            return HasKey(key_);
        }
        public bool TryGetValue(string key_,out JSONNode jsonNode_){
            return m_Dict.TryGetValue(key_,out jsonNode_);
        }
        public virtual void setToObjectRelativePath(string key_,JSONNode jsonNode_){
            if(key_.IndexOf('.')>0){
                setToRelativePath(key_,jsonNode_);
            }else{
                bool _needChange = false;
                JSONNode _jsNode = jsonNode_;
                if (_jsNode == null){
                    _jsNode = JSONNull.CreateOrGet();
                }else if(_jsNode.dataPath != null){
                    _jsNode = _jsNode.Clone();
                }
                JSONNode _jsNodeForKey = null;
                if(m_Dict.TryGetValue(key_, out _jsNodeForKey)){
                    if(_jsNodeForKey != jsonNode_){
                        _jsNodeForKey.dataPath = null;
                        m_Dict[key_] = _jsNode;
                        _needChange = true;
                    }
                    _jsNodeForKey = null;
                }else{
                    m_Dict.Add(key_, _jsNode);
                    _needChange = true;
                }
                if(_needChange && dataPath != null){
                    _jsNode.root = root;
                    _jsNode.dataPath = joinPath(dataPath,key_);
                }
            }
        }
    }
    public partial class JSONArray{
        public JSONNumber arrayLength = new JSONNumber(0);
        public void resetItemIdxAfter(int afterIdx_,int bufferIdx_){
            var _sb = EscapeBuilder;
            _sb.Append(dataPath);
            _sb.Append(".[");
            string _dataPathPrefix = _sb.ToString();
            _sb.Clear();
            for (int _idx = afterIdx_ + 1; _idx < m_List.Count; _idx++) {
                m_List[_idx].changeIdxAsItem(_dataPathPrefix,bufferIdx_);
            }
        }
        public void Add(JSONNode jsNode_){
            this[Count] = jsNode_;
        }
        public List<JSONNode> getList(){
            return m_List;
        }
        public void setList(List<JSONNode> list_){
            m_List.Clear();
            m_List = null;
            m_List = list_;
            arrayLength.AsInt = m_List.Count;
        }
    }
    partial class JSONLazyCreator{
        public override string AsString{
            get { 
                Set(new JSONString("")); 
                return "";
            }
            set { 
                Set(new JSONString(value)); 
            }
        }
    }
    public partial class JSONNode{
        public UINode bindUINodeAsProperty = null;
        public string AsPrintString{
            get{
                if(Tag == JSONNodeType.Array){
                    return "[]";
                }else if(Tag == JSONNodeType.Object){
                    return "{}";
                }else if(Tag == JSONNodeType.String){
                    return ToString();
                }else if(Tag == JSONNodeType.Number){
                    return ToString();
                }else if(Tag == JSONNodeType.NullValue){
                    return "null";
                }else if(Tag == JSONNodeType.Boolean){
                    return ToString();
                }else if(Tag == JSONNodeType.None){
                    throw new Exception("ERROR : JSONNodeType.None");
                }else if(Tag == JSONNodeType.Custom){
                    throw new Exception("ERROR : JSONNodeType.Custom");
                }
                throw new Exception("ERROR : 错误类型");
            }
        }
        public virtual string AsString{
            get {
                return Value; 
            }
            set {
                if(root != null){
                    root.changeValue(dataPath,this);
                }
                Value = value;
            }
        }
        public string TagAsString{
            get{
                if(Tag == JSONNodeType.Array){
                    return "Array";
                }else if(Tag == JSONNodeType.Object){
                    return "Object";
                }else if(Tag == JSONNodeType.String){
                    return "String";
                }else if(Tag == JSONNodeType.Number){
                    return "Number";
                }else if(Tag == JSONNodeType.NullValue){
                    return "NullValue";
                }else if(Tag == JSONNodeType.Boolean){
                    return "Boolean";
                }else if(Tag == JSONNodeType.None){
                    return "None";
                }else if(Tag == JSONNodeType.Custom){
                    return "Custom";
                }
                return null;
            }
        }
        
        public void merge(JSONNode jsonNode_) {
            if(dataPath == null){
                throw new Exception("ERROR : JSONObject + 符号，只在 dataPath 存在的情况下才能进行。");
            }
            if(Tag != JSONNodeType.Object){
                throw new Exception("ERROR : " + dataPath + " 只有  JSONObject 才能 merge。");
            }
            if(jsonNode_ == null){
                return;
            }
            if(jsonNode_.Tag != JSONNodeType.Object){
                throw new Exception("ERROR : " + dataPath + " 将要合并的目标不是 JSONObject。");
            }
            JSONObject _jsObj = jsonNode_ as JSONObject;
            var _enum = _jsObj.GetEnumerator();
            while (_enum.MoveNext()) {
                string _mergeKey = _enum.Current.Key;
                JSONNode _mergeJsNode = _enum.Current.Value;
                if(HasKey(_mergeKey)){
                    JSONNode _jsNodeOnMergeKey = this[_mergeKey];
                    if( _jsNodeOnMergeKey.Tag != _mergeJsNode.Tag && !_jsNodeOnMergeKey.IsNull && !_mergeJsNode.IsNull ){
                        throw new Exception("ERROR : " + dataPath + "." +_mergeKey +  "的类型<"+_jsNodeOnMergeKey.TagAsString+">和要合并的类型<"+_mergeJsNode.TagAsString+">不一致。");
                    }
                    if(_jsNodeOnMergeKey.Tag == JSONNodeType.Object){
                        _jsNodeOnMergeKey.merge(_mergeJsNode as JSONObject);
                        root.changeValue(_jsNodeOnMergeKey.dataPath,_jsNodeOnMergeKey);
                    }else{
                        if(_mergeJsNode.dataPath!=null){
                            this[_mergeKey] = _mergeJsNode.Clone();
                        }else{
                            this[_mergeKey] = _mergeJsNode;
                        }
                    }
                }else{
                    if(_mergeJsNode.dataPath!=null){
                        this[_mergeKey] = _mergeJsNode.Clone();
                    }else{
                        this[_mergeKey] = _mergeJsNode;
                    }
                }
            }
        }
#if UNITY_EDITOR
        [ThreadStatic]
        private static List<string> _dataStructUseTabList = new List<string>();
        [ThreadStatic]
        private static List<string> _dataStructUseLogList = new List<string>();
        internal static List<string> reUseTabList{
            get{
                _dataStructUseTabList.Clear();
                return _dataStructUseTabList;
            }
        }
        internal static List<string> reUseLogList{
            get{
                _dataStructUseLogList.Clear();
                return _dataStructUseLogList;
            }
        }
        
        public void printDataByStruct(string dataPath_ = null){
            List<string> _tabList = reUseTabList;
            List<string> _logList = reUseLogList;
            getStructAndData(dataPath_ == null ? dataPath : dataPath_ , _tabList , _logList);
            for (int _idx = 0; _idx < _logList.Count; _idx++) {
                string _logStr = _logList[_idx];
                LogToFiles.logByType(LogToFiles.LogType.DataStruct,_logStr);
            }
            LogToFiles.logByType(LogToFiles.LogType.DataStruct,"\n");
        }
        
        public void getStructAndData(string key_,List<string> tabList_,List<string> logList_,bool justStruct_ = false){
            if(key_ == null){
                key_ = "";
            }
            string _lastTab;
            if(Tag == JSONNodeType.Array || Tag == JSONNodeType.Object){
                bool _needRecover = false;
                if(tabList_.Count > 1){
                    _lastTab = tabList_[tabList_.Count - 1];
                    if(_lastTab[_lastTab.Length - 1] == '┃'){
                        tabList_[tabList_.Count - 1]= _lastTab.replaceLastChar("┣");
                        _needRecover = true;
                    }
                    _lastTab = null;
                }
                tabList_.Add(key_);
                logList_.Add(tabList_.joinStr()+"┓");
                tabList_.RemoveAt(tabList_.Count - 1);
                if(_needRecover){
                    tabList_[tabList_.Count - 1] = tabList_[tabList_.Count - 1].replaceLastChar("┃");
                }
                if(tabList_.Count > 1){
                    _lastTab = tabList_[tabList_.Count - 1];
                    if(_lastTab[_lastTab.Length - 1] == '┗'){
                        tabList_[tabList_.Count - 1] = _lastTab.replaceLastChar(" ");
                    }
                    _lastTab = null;
                }
                tabList_.Add(key_.toSameLengthSpace());
                if(Tag == JSONNodeType.Object){
                    int _keyLength = (this as JSONObject).Count;
                    int _currentIdx = 0;
                    var _thisEnume = (this as JSONObject).GetEnumerator();
                    while (_thisEnume.MoveNext()) {
                        var _key = _thisEnume.Current.Key;
                        var _value = _thisEnume.Current.Value;
                        tabList_.Add((_currentIdx == _keyLength - 1)?"┗":"┃");
                        _value.getStructAndData(_key,tabList_,logList_,justStruct_);
                        tabList_.RemoveAt(tabList_.Count - 1); 
                        _currentIdx ++;
                    }
                    tabList_.RemoveAt(tabList_.Count - 1); 
                }else if(Tag == JSONNodeType.Array){
                    if(Count == 0){
                        tabList_.Add("┗");
                    }else{
                        tabList_.Add("┣");
                    }
                    if(dataPath != null && !justStruct_){
                        tabList_.Add("length");
                        logList_.Add(tabList_.joinStr() + " : " + this["length"].ToString());
                        tabList_.RemoveAt(tabList_.Count - 1);
                    }
                    tabList_.RemoveAt(tabList_.Count - 1); 
                    JSONArray _selfAsArr = (this as JSONArray);
                    for (int _idx = 0; _idx < _selfAsArr.Count; _idx++){
                        if(justStruct_){
                            tabList_.Add("┗");
                            this[_idx].getStructAndData("[]",tabList_,logList_,justStruct_);
                            tabList_.RemoveAt(tabList_.Count - 1);
                            break;
                        }else{
                            tabList_.Add((_idx == _selfAsArr.Count - 1)?"┗":"┃");
                            this[_idx].getStructAndData(_idx.toIdxKey(),tabList_,logList_,justStruct_);
                            tabList_.RemoveAt(tabList_.Count - 1);
                        }
                    }
                    tabList_.RemoveAt(tabList_.Count - 1); 
                }
            }else{
                bool _needRecover = false;
                if(tabList_.Count > 1){
                    _lastTab = tabList_[tabList_.Count - 1];
                    if(_lastTab[_lastTab.Length - 1] == '┃'){
                        tabList_[tabList_.Count - 1]= _lastTab.replaceLastChar("┣");
                        _needRecover = true;
                    }
                    _lastTab = null;
                }
                if(!justStruct_){
                    logList_.Add(tabList_.joinStr() + key_ + " : " + ToString());
                }else{
                    logList_.Add(tabList_.joinStr() + key_ + " - " + TagAsString);
                }
                if(_needRecover){
                    tabList_[tabList_.Count - 1] = tabList_[tabList_.Count - 1].replaceLastChar("┃");
                }
            }   
        }
        
        public void printAsKeyValue(bool withDictAndArray_ = false){
            List<string> _logList = new List<string>();
            checkDataPath(withDictAndArray_,_logList);
            for (int _idx = 0; _idx < _logList.Count; _idx++) {
                LogUtils.WriteLog(_logList[_idx]);
            }
        }
#endif
        
        public void checkDataPath(bool withDictAndArray_ = false,List<string> logList_ = null){
#if UNITY_EDITOR
            Dictionary<string,JSONNode> _dataPathAndValueDict = convertToDataPathAndValueDict(dataPath,withDictAndArray_);
            if(withDictAndArray_){
                if(logList_ != null ){
                    logList_.Add(dataPath);
                }
            }
            if (_dataPathAndValueDict.Keys.Count > 0){
                var _pairEnume = _dataPathAndValueDict.GetEnumerator();
                while (_pairEnume.MoveNext()) {
                    string _convertedDataPath = _pairEnume.Current.Key;
                    JSONNode _jsNode = _pairEnume.Current.Value;
                    if(_jsNode.root == null){
                        throw new Exception("ERROR : " + _convertedDataPath + " root 缺失");
                    }
                    if(_jsNode.dataPath == null){
                        throw new Exception("ERROR : " + _convertedDataPath + " dataPath 缺失");
                    }
                    if(_jsNode.dataPath != _convertedDataPath){
                        throw new Exception("ERROR : 结构错误 : <结构递归得出>"+_convertedDataPath + " != " +  _jsNode.dataPath+"<节点dataPath>");
                    }
                    if(logList_ != null ){
                        if(_jsNode.Tag == JSONNodeType.Object){
                            logList_.Add(_convertedDataPath);
                        }else if(_jsNode.Tag == JSONNodeType.Array){
                            logList_.Add(_convertedDataPath);
                            logList_.Add(_convertedDataPath + ".length : " +  _jsNode["length"].ToString());
                        }else {
                            logList_.Add(_convertedDataPath + " : " + _jsNode.ToString());
                        }
                    }
                }
                _pairEnume.Dispose();
            }
#endif
        }
        
        public Dictionary<string,JSONNode> convertToDataPathAndValueDict(
            string dataPath_,
            bool withDictAndArray_ = false
        ){
            Dictionary<string,JSONNode> _recordToKeyValueDict = new Dictionary<string, JSONNode>();
            if(Tag == JSONNodeType.Object){
                jsonObjectKeyValueRecord(this as JSONObject,dataPath_,_recordToKeyValueDict,withDictAndArray_);
            }else if(Tag == JSONNodeType.Array){
                jsonArrayKeyValueRecord(this as JSONArray,dataPath_,_recordToKeyValueDict,withDictAndArray_);
            }else{
                return null;
            }
            return _recordToKeyValueDict;
        }
        private static void jsonObjectKeyValueRecord(JSONObject jsonDict_,string jsonDictDataPath_,Dictionary<string,JSONNode> recordToKeyValueDict_,bool withDictAndArray_){
            var _jsonDictEnume = jsonDict_.GetEnumerator();
            while (_jsonDictEnume.MoveNext()) {
                string _key = _jsonDictEnume.Current.Key;
                JSONNode _jsNode = _jsonDictEnume.Current.Value;
                string _currentDataPath = joinPath(jsonDictDataPath_,_key);
                if(_jsNode.Tag == JSONNodeType.Object) {
                    if(withDictAndArray_){
                        recordToKeyValueDict_[_currentDataPath] = _jsNode;
                    }
                    jsonObjectKeyValueRecord(_jsNode as JSONObject,_currentDataPath,recordToKeyValueDict_,withDictAndArray_);
                }else if(_jsNode.Tag == JSONNodeType.Array){
                    if(withDictAndArray_){
                        recordToKeyValueDict_[_currentDataPath] = _jsNode;
                    }
                    jsonArrayKeyValueRecord(_jsNode as JSONArray,_currentDataPath,recordToKeyValueDict_,withDictAndArray_);
                }else{
                    recordToKeyValueDict_[_currentDataPath] = _jsNode;
                }
            }
        }
        private static void jsonArrayKeyValueRecord(JSONArray jsonArray_,string jsonDictDataPath_,Dictionary<string,JSONNode> recordToKeyValueDict_,bool withDictAndArray_){
            for (int _idx = 0; _idx < jsonArray_.Count; _idx++) {
                JSONNode _jsNode = jsonArray_[_idx];
                string _currentDataPath = joinPath(jsonDictDataPath_,_idx.toIdxKey());
                if(_jsNode.Tag == JSONNodeType.Object) {
                    if(withDictAndArray_){
                        recordToKeyValueDict_[_currentDataPath] = _jsNode;
                    }
                    jsonObjectKeyValueRecord(_jsNode as JSONObject,_currentDataPath,recordToKeyValueDict_,withDictAndArray_);
                }else if(_jsNode.Tag == JSONNodeType.Array){
                    if(withDictAndArray_){
                        recordToKeyValueDict_[_currentDataPath] = _jsNode;
                    }
                    jsonArrayKeyValueRecord(_jsNode as JSONArray,_currentDataPath,recordToKeyValueDict_,withDictAndArray_);
                }else{
                    recordToKeyValueDict_[_currentDataPath] = _jsNode;
                }
            }
        }
        
        public void changeIdxAsItem(string arrayDataPathPrefix_,int bufferIdx_){
            if(bufferIdx_ == 0){
                return;
            }
            if(_dataPath[_dataPath.Length - 1] == ']'){
                int _idx = _dataPath.LastIndexOf('[');
                int _targetIdx = _dataPath.Substring(_idx + 1,_dataPath.Length - _idx - 2).toInt() + bufferIdx_;
                if(_targetIdx < 0){
                    throw new Exception("ERROR : " +  arrayDataPathPrefix_ + " 调整序号时，出现序号错误。" );
                }
                var _sb = EscapeBuilder;
                _sb.Append(arrayDataPathPrefix_);
                _sb.Append(_targetIdx.ToString());
                _sb.Append(']');
                string _targetDataPath = _sb.ToString();
                _sb.Clear();
                dataPath = _targetDataPath;
            }else{
                throw new Exception("ERROR : " +  _dataPath + " 不是列表中的元素。" );
            }
        }
        public virtual JSONNode getJsonNodeByRelativePath(string dataPath_,object value_ = null,bool setDefaultToDataPath_ = false){
            string[] _dataPathArr = dataPath_.splitWith(".");
            JSONNode _currentJsonNode = this;
            JSONNode _lastJsonNode = null;
            int _length = _dataPathArr.Length;
            for (int _idx = 0; _idx < _length; _idx++) {
                _lastJsonNode = _currentJsonNode;
                _currentJsonNode = _currentJsonNode[_dataPathArr[_idx]];
                if(_currentJsonNode == null){
                    if(value_ == null){
                        _currentJsonNode = null;
                    }else if(value_ is string){
                        _currentJsonNode = new JSONString((string)value_);
                    }else if(JSONNumber.IsNumeric(value_)){
                        _currentJsonNode = new JSONNumber(Convert.ToDouble(value_));
                    }else if(value_ is bool){
                        _currentJsonNode = new JSONBool((bool)value_);
                    }else if(value_ is Array) {
                        _currentJsonNode = JSONNode.convertArrayToJsonList((Array)value_);
                    }else if(value_ is object) {
                        _currentJsonNode = JSONNode.convertObjectToJsonDict(value_);
                    }else{
                        throw new Exception("ERROR : set to " + dataPath_+ " ,value is unknow type : " + value_.ToString());
                    }
                    if(setDefaultToDataPath_){
                        _lastJsonNode[_dataPathArr[_idx]] = _currentJsonNode;
                    }
                    break;
                }
            }
            return _currentJsonNode;
        }
        public string asListMergeToDictKey(){
            if(Tag == JSONNodeType.String){
                return AsString;
            }else if(Tag == JSONNodeType.Number){
                return ToString();
            }else{
                throw new Exception("ERROR : 无法作为字典的键");
            }
        }
        public bool asListMergeToListMatchValue(){
            if(
                Tag == JSONNodeType.Object || 
                Tag == JSONNodeType.Array || 
                Tag == JSONNodeType.None ||
                Tag == JSONNodeType.Custom
            ){
                return false;
            }
            return true;
        }

        public void mergeToRelativePath(string dataPath_,object value_){
            setToRelativePath(dataPath_,value_,true);
        }
        public void mergeToRelativePath(string dataPath_,JSONNode jsonNode_){
            setToRelativePath(dataPath_,jsonNode_,true);
        }
        
        public void setToRelativePath(string dataPath_,object value_,bool isMerge_ = false){
            setToRelativePath(dataPath_,convertValueToJsonNode(value_),isMerge_);
        }
        public void setToRelativePath(string dataPath_,JSONNode jsonNode_,bool isMerge_ = false){
            string[] _dataPathArr = dataPath_.splitWith(".");
            JSONNode _currentJsonNode = this;
            JSONNode _tempJsonNode = null;
            int _length = _dataPathArr.Length;
            StringBuilder _sb = EscapeBuilder;
            for (int _idx = 0; _idx < _length; _idx++) {
                string _currentKey = _dataPathArr[_idx];
                _tempJsonNode = _currentJsonNode[_currentKey];
                if(jsonNode_ == null && _tempJsonNode == null){
                    _sb.Clear();
                    return;
                }
                if(_idx < (_length - 1)){
                    if(_tempJsonNode == null){
                        string _nextKey = _dataPathArr[_idx + 1];
                        if(_nextKey != null && _nextKey[0] == '['){
                            _tempJsonNode = new JSONArray();
                        }else{
                            _tempJsonNode = new JSONObject();
                        }
                        _currentJsonNode[_currentKey] = _tempJsonNode;
                        _currentJsonNode = _tempJsonNode;
                    }else{
                        _currentJsonNode = _tempJsonNode;
                    }
                }else{
                    if(_tempJsonNode != null){
                        if(isMerge_){
                            _tempJsonNode.merge(jsonNode_);
                        }else{
                            _currentJsonNode[_currentKey] = jsonNode_;
                        }
                    }else{
                        _currentJsonNode[_currentKey] = jsonNode_;
                    }
                }
                _tempJsonNode = null;
            }
            _sb.Clear();
        }
        
        public DataUIBinder.JSONRoot root = null;
        
        private string _dataPath = null;
        
        public string dataPath{
            get { 
                return _dataPath; 
            }
            set {
                if(_dataPath != null){
                    if(root == null){
                        throw new Exception("ERROR : 先设 root 和 dataPath 必须同时存在。");
                    }
                    if(value == null){
                        setDataPathToNull();
                    }else{
                        if(_dataPath != value){
                            DataUIBinder.JSONRoot _tempRoot = root;
                            setDataPathToNull();
                            root = _tempRoot;
                            resetDataPath(value);
                        }
                    }
                }else{
                    if(root == null){
                        if(value != null){
                            throw new Exception("ERROR : 先设 root 再设置 dataPath。");
                        }
                    }
                    if(value == null){
                        if(root != null){
                            setDataPathToNull();
                        }
                    }else{
                        _dataPath = value;
                        resetDataPath(value);
                    }
                }
            }
        }
        
        private void resetDataPath(string dataPath_){
            _dataPath = dataPath_;
            if(Tag == JSONNodeType.Object){
                var _jsonDictEnume = (this as JSONObject).GetEnumerator();
                JSONNode _jsNode;
                while (_jsonDictEnume.MoveNext()) {
                    _jsNode = _jsonDictEnume.Current.Value;
                    _jsNode.root = root;
                    _jsNode.dataPath = joinPath(_dataPath,_jsonDictEnume.Current.Key);
                }
            }else if(Tag == JSONNodeType.Array){
                (this as JSONArray).arrayLength.root = root;
                (this as JSONArray).arrayLength.dataPath = joinPath(_dataPath,"length");
                JSONNode _jsNode;
                for (int _idx = 0; _idx < Count; _idx++) {
                    _jsNode = this[_idx];
                    _jsNode.root = root;
                    _jsNode.dataPath = joinPath(_dataPath,_idx.toIdxKey());
                }
            }
            root.changeValue(_dataPath,this);
        }
        
        public void setDataPathToNull(){
            if(bindUINodeAsProperty != null){
                throw new Exception("ERROR : " + bindUINodeAsProperty.uiPath + " bind this value.Cann't set to null.");
            }
            if(Tag == JSONNodeType.Object){
                var _jsonDictEnume = (this as JSONObject).GetEnumerator();
                while (_jsonDictEnume.MoveNext()) {
                    _jsonDictEnume.Current.Value.setDataPathToNull();
                }
            }else if(Tag == JSONNodeType.Array){
                for (int _idx = 0; _idx < Count; _idx++) {
                    this[_idx].setDataPathToNull();
                }
                JSONArray _selfAsArr = (this as JSONArray);
                _selfAsArr.arrayLength.dataPath = null;
                _selfAsArr.arrayLength.root = null;
            }
            root.changeValue(_dataPath,null);
            _dataPath = null;
            root = null;
        }
#region C# 对象 转换成 JSONObject
        
        public static JSONObject convertObjectToJsonDict(object obj_){
            JSONObject _jsonDict = new JSONObject();
            var _propertiesList = obj_.GetType ().GetProperties ();
            for (int _idx = 0; _idx < _propertiesList.Length; _idx++) {
	            var _propertyInfo = _propertiesList[_idx];
                if (_propertyInfo.DeclaringType != _propertyInfo.ReflectedType) { 
                    continue;
                }
                _jsonDict[_propertyInfo.Name] = convertValueToJsonNode(_propertyInfo.GetValue(obj_));
            }
            return _jsonDict;
        }
        
        public static JSONArray convertArrayToJsonList(Array list_){
            JSONArray _jsonArr = new JSONArray();
            for (int _idx = 0; _idx < list_.Length; _idx++) {
                _jsonArr[_idx + 1] = convertValueToJsonNode(list_.GetValue(_idx));
            }
            return _jsonArr;
        }
        
        public static JSONNode convertValueToJsonNode(object value_){
            if(value_ is bool){
                return new JSONBool((bool)value_);
            }else if(value_ is string){
                return new JSONString((string)value_);
            }else if(JSONNumber.IsNumeric(value_)){
                return new JSONNumber(Convert.ToDouble(value_));
            }else if(value_ == null){
                return JSONNull.CreateOrGet();
            }else if(value_ is Array) {
                return convertArrayToJsonList((Array)value_);
            }else if(value_ is object) {
                return convertObjectToJsonDict(value_);
            }
            return null;
        }
#endregion
        
        protected static string joinPath(string parentPath_,string key_){
            string _currentDataPath = null;
            if(parentPath_ == null){
                _currentDataPath = key_;
            }else{
                var _sb = EscapeBuilder;
                _sb.Append(parentPath_);
                _sb.Append('.');
                _sb.Append(key_);
                _currentDataPath = _sb.ToString();
                _sb.Clear();
            }
            return _currentDataPath;
        }
    }
}