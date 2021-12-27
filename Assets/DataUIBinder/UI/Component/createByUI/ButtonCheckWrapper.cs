using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
namespace DataUIBinder{
    /*
        单选复选类的按钮
            1.有名为 select 的节点，用来做选中状态
            2.有名为 unSelect 的节点，用来做非选中状态
    */
    public class ButtonCheckWrapper : ButtonWrapper{
        private GameObject _select = null;
        private bool _selectInited = false;
        protected GameObject select{
            get{
                if(!_selectInited){
                    Transform _trans = transform.Find("select");
                    if(_trans != null){
                        _select = _trans.gameObject;
                    }
                    _selectInited = true;
                }
                return _select;
            }
        }
        private GameObject _unSelect = null;
        private bool _unSelectInited = false;
        protected GameObject unSelect{
            get{
                if(!_unSelectInited){
                    Transform _trans = transform.Find("unSelect");
                    if(_trans != null){
                        _unSelect = _trans.gameObject;
                    }
                    _unSelectInited = true;
                }
                return _unSelect;
            }
        }
        protected string key = null;
        public void reset(string pattern_,string key_,UINode uiNode_ = null){
            key = key_;
            base.reset(pattern_,uiNode_);
        }
        protected override void onBtn(){
            bool _currentBool = DataCenter.root.getValue(dataPath).AsBool;
            if(select && unSelect){
                if(select.activeSelf == unSelect.activeSelf){
                    throw new Exception("ERROR : "+dataPath+" 交替显示错误。");
                }
            }
            if(select){
                if(_currentBool != select.activeSelf){
                    throw new Exception("ERROR : "+dataPath+" 数据显示不一致");
                }
            }
            if(canClick()){
                uiNode.onCheck(gameObject.name,key,!_currentBool);
                DataCenter.root.setValue(dataPath,!_currentBool);
                justClicked = true;
            }
        }
        protected void doCheck(bool targetBool_){
            if(!targetBool_){
                select?.SetActive(false);
                unSelect?.SetActive(true);
            }else{
                select?.SetActive(true);
                unSelect?.SetActive(false);
            }
        }
        public override void dataChangeHandle(string dataPath_,JSONNode jsNode_){
            bool _targetBool = jsNode_.AsBool;
            doCheck(_targetBool);
        }
        protected override void OnDestroy(){
            if(isDestroyed){ return; }
            _select = null;
            _unSelect = null;
            base.OnDestroy();
        }
    }
}