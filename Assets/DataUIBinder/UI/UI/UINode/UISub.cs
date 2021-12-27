using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DataUIBinder{
    public class UISub : UINode{
        protected override void Awake(){
            base.Awake();
        }
        public override void onBtn(string btnName_){
            base.onBtn(btnName_);
        }
        public override void onPress(string btnName_){
            base.onPress(btnName_);
        }
        public override void onDoubleClick(string btnName_){
            base.onDoubleClick(btnName_);
        }
        public override void onCheck(string btnName_,string key_,bool isOn_){
            base.onCheck(btnName_,key_,isOn_);
        }
        public override void onToggle(string btnName_,string key_,string value_){
            base.onToggle(btnName_,key_,value_);
        }
    }
}