using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
namespace DataUIBinder{
    /*
        输入封装
    */
    public class InputFieldWrapper : ComponentWrapper{
        private InputField _inputField = null;
        private InputField inputField{
            get{
                if(_inputField == null){
                    _inputField = gameObject.GetComponent<InputField>();
                }
                return _inputField;
            }
        }
        protected override void Awake(){
            base.Awake();
            inputField.onEndEdit.AddListener((strInput_)=>{
                if(!Recoder.isReplay){
                    if(Recoder.isRecord){
                        JSONObject _jsObject = new JSONObject();
                        _jsObject["type"] = "onInput";
                        _jsObject["uiPath"] = uiNode.uiPath;
                        _jsObject["name"] = gameObject.name;
                        _jsObject["value"] = strInput_;
                        Recoder.record(_jsObject);
                    }
#if UNITY_EDITOR
                    if(uiNode){
                        LogToFiles.logByType(LogToFiles.LogType.Record, "[ onInput ] " + uiNode.uiPath + " -> "+gameObject.name + " : " + strInput_.ToString());
                    }
#endif
                    DataCenter.root.setValue(dataPath,strInput_);
                }else{
                    //播放状态下，无法改变值
                    inputField.text = DataCenter.root.getValue(dataPath).AsString;
                }
            });
        }
        public override void reset(string pattern_,UINode uiNode_ = null){
            pattern = pattern_;
            dataPath = null;
#if UNITY_EDITOR
            if(pattern.isStartsWith("_ui_.")){
                string _key = pattern.splitWith("_ui_.")[1];
                UINode.isKeyAvailableOnUI(_key);
            }
            if(pattern.isStartsWith("_dt_.")){
                string _key = pattern.splitWith("_dt_.")[1];
                UINode.isKeyAvailableOnUI(_key);
            }
#endif
            base.reset(pattern,uiNode_);
        }
        public override void dataChangeHandle(string dataPath_,JSONNode jsNode_){
            inputField.text = jsNode_.AsString;
        }
        protected override void OnDestroy(){
            if(isDestroyed){ return; }
            if(_inputField != null){
                inputField.onValueChanged.RemoveAllListeners();
                _inputField = null;
            }
            base.OnDestroy();
        }
    }
}