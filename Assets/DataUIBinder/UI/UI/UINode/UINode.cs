using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
namespace DataUIBinder{
    public class UINode : DataPathDrivenComponent,IUpdateAble{
        public UINode parentUINode = null;//自己所在的父节点
        protected bool isDestroyed = false;
        public string uiName = null;//ui类名
#if UNITY_EDITOR
        public static void isKeyAvailableOnUI(string key_){
            if(
                key_ == "enabled"||
                key_ == "__idx__"|| // 列表数据，排序用
                key_ == "_dt_"  || key_ == "_ui_"|| // 关键字
                key_ == "dtPath"|| key_ == "uiPath"|| // 关键字对应的字符串
                key_ == "state"//UIMain 使用
            ){
                throw new Exception("ERROR : 不合法的键 : "+key_);
            }
            if (key_.isStartsWith("_dt_")||key_.isStartsWith("_ui_")){
                throw new Exception("ERROR : 请检查是否忘记写逗号了 : "+key_);
            }
        }
#endif
        protected Dictionary<string,Transform> transformDict = new Dictionary<string,Transform>();
        protected Dictionary<string,UISub> subUIDict = new Dictionary<string,UISub>();
        public Transform this[string goName_] { 
            get {
                Transform _trans = null;
                if(transformDict.TryGetValue(goName_, out _trans)){
                    return _trans;
                }
                return _trans;
            } 
            set {
                if(!transformDict.ContainsKey(goName_)){
                    transformDict[goName_] = value;   
                }else{
                    throw new Exception("ERROR " + System.Reflection.MethodBase.GetCurrentMethod ().ReflectedType.FullName + " -> " + new System.Diagnostics.StackTrace ().GetFrame (0).GetMethod ().Name + " : " +
                        goName_ + " 已存在"
                    );
                }
            }
        }
        private string _dtPath = null;
        public string dtPath{
            get{
                return _dtPath;
            }
            set{
                if(value == null){
                    if(_dtPath != null){
                        _dtPath = value;
                    }
                }else{
                    if(value != _dtPath){
                        _dtPath = value;
                    }
                }
                ui_sv("dtPath",_dtPath);
            }
        }
        private string _uiPath = null;
        public string uiPath {
            get{
                if (_uiPath == null){
                    throw new Exception("ERROR : UIPath 没有赋值 : " + DisplayUtils.getDisplayPath(rectTrans));
                }
                return _uiPath;
            }
            set{
                if(_uiPath == null){
                    _uiPath = value;
                    ui_sv("uiPath",_uiPath);
                    ui_sv("enabled",enabled);//有可能OnEnabled时，uiPath还没赋值。
                }else{
                    throw new Exception("ERROR : "+_uiPath+" 现实路径一旦确定，不能在更改。");
                }
            }
        }
        private RectTransform _rectTrans = null;
        public RectTransform rectTrans{
            get{
                if(_rectTrans == null){
                    _rectTrans = transform as RectTransform;
                }
                return _rectTrans;
            }
        }
        public bool isUseForCopy = false;
        protected virtual void Awake() {
        }
        // 将 this 转换成 uiPath，将 data 转换成 dtPath.
        public string convertThisOrDataPath(string dataPath_){
            string _tempDataPath = dataPath_;
            if(_tempDataPath.Contains("_ui_.")){
                _tempDataPath = _tempDataPath.Replace("_ui_.", uiPath + ".");
            }
            if(_tempDataPath.Contains("_dt_.")){
                if(dtPath == null){
                    throw new Exception("ERROR : "+_tempDataPath+" data指定的相对路径，并未指定dtPath。");
                }
                _tempDataPath = _tempDataPath.Replace("_dt_.", dtPath + ".");
            }
            return _tempDataPath;
        }
        private bool isDataUIBinded = false;
        public void resetDataUIBind(){
            if (isDataUIBinded){
                ComponentWrapper.resetWrapperCacheListByUINode(this);
            }else{
                ComponentWrapper.tryAddWrapperToChildren(rectTrans,this);
                ComponentWrapper.cacheTransAndWrapSpecialName(this);
                isDataUIBinded = true;
            }
        }
        public override void OnDestroy(){
            if (isDestroyed){ return; }
            if(isUseForCopy){
                return; 
            }
            transformDict?.Clear();
            transformDict = null;
            _rectTrans = null;
            parentUINode = null;
            ComponentWrapper.clearWrapperCacheListWhenUINodeDestroy(this);
            base.OnDestroy();
            isDestroyed = true;
        }
        public virtual void frameUpdate(float dt_){
            
        }
        //向自己所在的UI数据路径上设置值
        public void ui_sv(string key_,object value_){
            setValueOnUIPath(key_,value_);
        }
        private void setValueOnUIPath(string key_,object value_){
            StringBuilder _sb = CSharpExtensionUtils.SBInstance;
            _sb.Clear();
            _sb.Append(uiPath);
            _sb.Append('.');
            _sb.Append(key_);
            DataCenter.root.setValue(_sb.ToString(),value_);
            _sb.Clear();
        }
        public void ui_sv(string key_,JSONNode jsNode_){
            setValueOnUIPath(key_,jsNode_);
        }
        private void setValueOnUIPath(string key_,JSONNode jsNode_){
            StringBuilder _sb = CSharpExtensionUtils.SBInstance;
            _sb.Clear();
            _sb.Append(uiPath);
            _sb.Append('.');
            _sb.Append(key_);
            DataCenter.root.setValue(_sb.ToString(),jsNode_);
            _sb.Clear();
        }
        //私有方法，以免忘记.AsString，造成JSONNode当做As的值来使用。
        public JSONNode ui_gv(string key_){
            return getValueOnUIPath(key_);
        }
        private JSONNode getValueOnUIPath(string key_){
            StringBuilder _sb = CSharpExtensionUtils.SBInstance;
            _sb.Clear();
            _sb.Append(uiPath);
            _sb.Append('.');
            _sb.Append(key_);
            JSONNode _returnJSONNode = DataCenter.root.getValue(_sb.ToString());
            _sb.Clear();
            return _returnJSONNode;
        }
        //向自己所在的数据路径上设置值
        public bool dt_sv(string key_,object value_){
            return setValueOnDataPath(key_,value_);
        }
        private bool setValueOnDataPath(string key_,object value_){
            if (dtPath == null){
                return false;
            }
            StringBuilder _sb = CSharpExtensionUtils.SBInstance;
            _sb.Clear();
            _sb.Append(dtPath);
            _sb.Append('.');
            _sb.Append(key_);
            DataCenter.root.setValue(_sb.ToString(),value_);
            _sb.Clear();
            return true;
        }
        public JSONNode dt_gv(string key_){
            return getValueOnDataPath(key_);
        }
        private JSONNode getValueOnDataPath(string key_){
            if (dtPath == null){
                return null;
            }
            StringBuilder _sb = CSharpExtensionUtils.SBInstance;
            _sb.Clear();
            _sb.Append(dtPath);
            _sb.Append('.');
            _sb.Append(key_);
            JSONNode _returnJSONNode = DataCenter.root.getValue(_sb.ToString());
            _sb.Clear();
            return _returnJSONNode;
        }
        protected virtual void OnEnable(){
            if(isUseForCopy){ 
                return; 
            }
            if(_uiPath != null){
                ui_sv("enabled",true);
            }
        }
        protected virtual void OnDisable(){
            if(isUseForCopy){ 
                return; 
            }
            if(_uiPath != null){
                ui_sv("enabled",false);
            }
        }
        private UIMain getUIMainOnSelfOrParent(){
            UIMain _uiMain = gameObject.GetComponent<UIMain>();
            if(_uiMain == null){
                _uiMain = gameObject.GetComponentInParent<UIMain>();
            }
            return _uiMain;
        }
        public virtual void onBtn(string btnName_){
            if(!Recoder.isReplay){
                if(Recoder.isRecord){
                    JSONObject _jsObject = new JSONObject();
                    _jsObject["type"] = "onBtn";
                    _jsObject["uiPath"] = uiPath;
                    _jsObject["btnName"] = btnName_;
                    Recoder.record(_jsObject);
                }
#if UNITY_EDITOR
                LogToFiles.logByType(LogToFiles.LogType.Record, "[ onBtn ] " + uiPath + " -> " + btnName_ );
#endif
            }
            /*
            //// - btn_<方法,参数>
                按钮的特殊命名
                    btn_<close,UIMain> 关闭所在的UIMain节点
                    btn_<hide,UISub> 隐藏所在的UISub节点
                    btn_<hide,parent> 隐藏所在的父容器节点
            */
            if (btnName_.isStartsWith("btn_<")){
                Regex regex1 = new Regex(@"btn_<(\w+?),(\w+?)>");
                MatchCollection matchs = regex1.Matches(btnName_);
                if (matchs.Count == 0){
                    throw new Exception("ERROR : "+btnName_+" 不符合执行方法的格式");
                }
                string _func = matchs[0].Groups[1].ToString();
                string _params = matchs[0].Groups[2].ToString();
                if(_func == "close"){
                    if(_params == "UIMain"){//所在UI父节点关闭
                        getUIMainOnSelfOrParent()?.closeSelf();
                    }else{
                        throw new Exception("ERROR : "+btnName_+" 方法 close 未支持参数 " + _params);
                    }
                }else if(_func == "hide"){
                    if(_params == "UISub"){//子节点隐藏
                        getUIMainOnSelfOrParent()?.gameObject.SetActive(false);
                    }else if(_params == "parent"){//父容器隐藏
                        this[btnName_].parent.gameObject.SetActive(false);
                    }else{
                        throw new Exception("ERROR : "+btnName_+" 方法 hide 未支持参数 " + _params);
                    }
                }else if(_func == "open"){
                    UIInfo _uiInfo = UIConfig.instance.getUIInfo(_params);
                    UIManager.instance.openUI(_uiInfo.folderPath,_uiInfo.uiName,null,_uiInfo.type,_uiInfo.loadType);
                }else{
                    throw new Exception("ERROR : "+btnName_+" 方法未支持 " + _func);
                }
            }
        }
        public virtual void onCheck(string btnName_,string key_,bool isOn_){
            if(!Recoder.isReplay){
                if(Recoder.isRecord){
                    JSONObject _jsObject = new JSONObject();
                    _jsObject["type"] = "onCheck";
                    _jsObject["uiPath"] = uiPath;
                    _jsObject["btnName"] = btnName_;
                    _jsObject["key"] = key_;
                    _jsObject["isOn"] = isOn_.ToString();
                    Recoder.record(_jsObject);
                }
#if UNITY_EDITOR
                LogToFiles.logByType(LogToFiles.LogType.Record, "[ onCheck ] " + uiPath + " -> " + btnName_ + " : " + key_ + " = " + isOn_.ToString());
#endif
            }
        }
        public virtual void onToggle(string btnName_,string key_,string value_){
            if(!Recoder.isReplay){
                if(Recoder.isRecord){
                    JSONObject _jsObject = new JSONObject();
                    _jsObject["type"] = "onToggle";
                    _jsObject["uiPath"] = uiPath;
                    _jsObject["btnName"] = btnName_;
                    _jsObject["key"] = key_;
                    _jsObject["value"] = value_;
                    Recoder.record(_jsObject);
                }
#if UNITY_EDITOR
                string _currentValue = dc.gv(uiPath.dotJoin(key_));
                LogToFiles.logByType(LogToFiles.LogType.Record, "[ onToggle ] " + uiPath + " -> " + btnName_ + " : " + key_ + " = " + _currentValue + " -> " + value_ );
#endif
            }
        }
        public virtual void onPress(string btnName_){
            if(!Recoder.isReplay){
                if(Recoder.isRecord){
                    JSONObject _jsObject = new JSONObject();
                    _jsObject["type"] = "onPress";
                    _jsObject["uiPath"] = uiPath;
                    _jsObject["btnName"] = btnName_;
                    Recoder.record(_jsObject);
                }
#if UNITY_EDITOR
                LogToFiles.logByType(LogToFiles.LogType.Record, "[ onPress ] " + uiPath + " -> " + btnName_ );
#endif
            }
        }
        public virtual void onDoubleClick(string btnName_){
            if(!Recoder.isReplay){
                if(Recoder.isRecord){
                    JSONObject _jsObject = new JSONObject();
                    _jsObject["type"] = "onDoubleClick";
                    _jsObject["uiPath"] = uiPath;
                    _jsObject["btnName"] = btnName_;
                    Recoder.record(_jsObject);
                }
#if UNITY_EDITOR
                LogToFiles.logByType(LogToFiles.LogType.Record, "[ onDoubleClick ] " + uiPath + " -> " + btnName_ );
#endif
            }
        }

    }
}