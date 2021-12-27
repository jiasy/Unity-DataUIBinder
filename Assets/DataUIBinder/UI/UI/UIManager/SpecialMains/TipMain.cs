using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace DataUIBinder{
    //这一层处理 Tip 封装。
    public class TipMain : UIMain {
        private System.Action _yesAction = null;
        private System.Action _noAction = null;
        private string _title;
        private string _content;
        public void setTitleAndContent(string title_,string content_){
            _title = title_;
            _content = content_;
        }
        public void setYesAndNoCallBack(System.Action yesAction_,System.Action noAction_){
            _yesAction = yesAction_;
            _noAction = noAction_;
        }
        public override void onBtn(string btnName_){
            if(btnName_ == "btn_yes"){
                if (_yesAction != null){
                    _yesAction.Invoke();
                }
            }else if(btnName_ == "btn_no" || btnName_.StartsWith("btn_close_") || btnName_.StartsWith("btn_close_force_")){
                if (_noAction != null){
                    _noAction.Invoke();
                }
            }
            base.onBtn(btnName_);
        }

        public override void OnDestroy () {
            _yesAction = null;
            _noAction = null;
            base.OnDestroy();
        }
    }
}