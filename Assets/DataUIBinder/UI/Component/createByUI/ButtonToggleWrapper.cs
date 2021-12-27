using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
namespace DataUIBinder{
    public class ButtonToggleWrapper : ButtonCheckWrapper{
        private string value = null;
        protected override void onBtn(){
            if(select.activeSelf == unSelect.activeSelf){
                throw new Exception("ERROR : "+dataPath+" 交替显示错误。");
            }
            string _currentValue = DataCenter.root.getValue(dataPath).AsString;
            if((_currentValue==value) != select.activeSelf){
                throw new Exception("ERROR : "+dataPath+" 数据显示不一致");
            }
            if(canClick()){
                if(_currentValue != value){
                    uiNode.onToggle(gameObject.name,key,value);
                    DataCenter.root.setValue(dataPath,value);
                    justClicked = true;
                }
            }
        }
        public void reset(string pattern_,string key_,string value_,UINode uiNode_ = null){
            value = value_;
            base.reset(pattern_,key_,uiNode_);
        }

        public override void dataChangeHandle(string dataPath_,JSONNode jsNode_){
            bool _targetBool = jsNode_.AsString == value;
            doCheck(_targetBool);
        }
    }
}
