using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
namespace DataUIBinder{
    public class SliderAndBtnTest : UINode{
        /*
            在 UINode 中，支持两种按钮命名，进行数据路径绑定
                toggle型
                    命名 toggle_key_value
                    数据路径 ui.界面名.key
                        值的可选范围为后面的 value 指定
                check型
                    命名 check_key
                    数据路径 ui.界面名.key
                        可选值为 true/false
                普通型
                    命名 btn_name
            满足命名的按钮触发，根据其类型，自动触发以下方法。
                onBtn
                onCheck
                onToggle
            在 UINode 中，可通过 this["节点名"] 的方式获取到节点
                节点名 必须是 特殊命名的。(见 ComponentWrapper.recordTransPrefixArray)
                    btn_
                    check_
                    toggle_
                    sub_
                    list_
                    等等
        */
        private string toggle{
            get{ return dc.gv("ui.SliderAndBtnTest.toggle","first").AsString; }
            set{ dc.sv("ui.SliderAndBtnTest.toggle",value); }
        }
        private float slider{
            get{ return dc.gv("ui.SliderAndBtnTest.slider",-50).AsFloat; }
            set{ dc.sv("ui.SliderAndBtnTest.slider",value); }
        }
        protected override void Awake(){
            DataCenter.defalutInit();
            uiPath = "ui.SliderAndBtnTest";
            base.Awake();
            //不是通过UIManager创建，需要手动进行一次显示数据绑定
            this.resetDataUIBind();
        }
        void Start(){
            toggle = "first";
            slider = -50;
            dc.sv("ui.SliderAndBtnTest.check",true);
            Transform _trackToggleBtnTras = this["trans_track_toggle"];
            void syncPosTo(Transform trans_,float yBuffer_ = -50){
                _trackToggleBtnTras.setXY(trans_.localPosition.x,trans_.localPosition.y + yBuffer_);
            }
            onChange("ui.SliderAndBtnTest.toggle",(toggleCurrent_)=>{
                string _toggleCurrent = toggleCurrent_.AsString;
                // SAMPLE - DUB - 通过节点名获取节点
                syncPosTo(this["toggle_toggle_" + _toggleCurrent]);
                //普通按钮
                if(_toggleCurrent == "first"){
                    dc.sv("ui.SliderAndBtnTest.next","second");
                }else if(_toggleCurrent == "second"){
                    dc.sv("ui.SliderAndBtnTest.next","thrid");
                }else if(_toggleCurrent == "thrid"){
                    dc.sv("ui.SliderAndBtnTest.next","first");
                }
            });
            onChange("ui.SliderAndBtnTest.check",(check_)=>{
                syncPosTo(this["check_check"],check_.AsBool?50:-50);
            });
        }
        // public override void OnDestroy(){
        //     base.OnDestroy();
        // }
        void Update(){
            DataCenter.frameUpdate();
            ComponentWrapper.doFrameUpdate();
        }
        public override void onBtn(string btnName_){
            UnityEngine.Debug.Log("onBtn -> "+btnName_.ToString());
            if(btnName_ == "btn_changeToggleByCode"){
                if(toggle == "first"){
                    toggle = "second";
                }else if(toggle == "second"){
                    toggle = "thrid";
                }else if(toggle == "thrid"){
                    toggle = "first";
                }
            }
            base.onBtn(btnName_);
        }
        public override void onCheck(string btnName_,string key_,bool isOn_){
            UnityEngine.Debug.Log("onCheck -> "+btnName_.ToString()+" : this." + key_+"="+isOn_.ToString());
            string _dataPath = uiPath + "." + key_;
            bool _boolOnPath = dc.gv(_dataPath).AsBool;
            if(_boolOnPath != isOn_){
                UnityEngine.Debug.Log(_dataPath + ":"+_boolOnPath.ToString()+" -> " + isOn_.ToString());
            }else{
                throw new Exception("ERROR : unexpect");
            }
            base.onCheck(btnName_,key_,isOn_);
        }
        public override void onToggle(string btnName_,string key_,string value_){
            UnityEngine.Debug.Log("onToggle -> "+btnName_.ToString()+" : this." + key_+"="+value_.ToString());
            string _dataPath = uiPath + "." + key_;
            string _stringOnPath = dc.gv(_dataPath).AsString;
            if(value_ != _stringOnPath){
                UnityEngine.Debug.Log(_dataPath + ":" + _stringOnPath + " -> " + value_);
            }else{
                throw new Exception("ERROR : unexpect");
            }
            base.onToggle(btnName_,key_,value_);
        }
    }
}
