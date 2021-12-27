using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
namespace DataUIBinder{
    public class InputTest : UIMain{
        /*
            常用输入的绑定实例
        */
        protected override void Awake(){
            base.Awake();
        }
        void Start(){
            //和check_selectTime相互关联，所以必须 ui 的 setValue，是UI的数据。
            ui_sv("selectTime",false);
            createDataForList("temp.timeList",24);
            Transform _timeScrollTrans = this["scroll_selectTime"];
            ScrollWrapper _timeScroll = _timeScrollTrans.GetComponent<ScrollWrapper>();
            _timeScroll.reset("temp.timeList",this);
        }
        // public override void OnDestroy(){
        //     base.OnDestroy();
        // }
        // public override void onBtn(string btnName_){
        //     base.onBtn(btnName_);
        // }
        // public override void onPress(string btnName_){
        //     base.onPress(btnName_);
        // }
        // public override void onDoubleClick(string btnName_){
        //     base.onDoubleClick(btnName_);
        // }
        // public override void onCheck(string btnName_,string key_,bool isOn_){
        //     base.onCheck(btnName_,key_,isOn_);
        // }
        // public override void onToggle(string btnName_,string key_,string value_){
        //     base.onToggle(btnName_,key_,value_);
        // }
        private void createDataForList(string listDataPath_,int count_){
            JSONArray _jsonArray = new JSONArray();
            int _idx = 0;
            while(count_ > 0){
                JSONObject _jsonObject = new JSONObject();
                _jsonObject["idx"] = _idx;//数据序号
                _jsonObject["time"] = string.Format("{0:00}",_idx)+":00";//持有的时间
                _jsonArray.Add(_jsonObject);
                _idx++;
                count_--;
            }
            dc.sv(listDataPath_,_jsonArray);
        }
    }
}