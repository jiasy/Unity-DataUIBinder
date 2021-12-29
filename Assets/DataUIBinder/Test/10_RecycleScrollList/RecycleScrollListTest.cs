using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
namespace DataUIBinder{
    /*
        列表实例 2
            演示列表数据变化，自动触发列表显示变化。            
                列表数据根据条件过滤到左右两个列表上(list_left,list_right)
                数据源是同一个，通过设置显示列表的过滤方法来过滤掉不符合条件的数据。
                两个列表的元素按照设置进行左右转移。
                剪头指向为数据转移的朝向，从左向右或从右向左按照指定个数转移数据。
                这里的转移只是重置列表中的属性。
    */
    public class RecycleScrollListTest : UINode{
        protected override void Awake(){
            DataCenter.defalutInit();
            uiPath = "ui.RecycleScrollListTest";
            base.Awake();
            //不是通过UIManager创建，需要手动进行一次显示数据绑定
            this.resetDataUIBind();
        }
        void Start(){
            createDataForList("temp.list",100);
            ui_sv("leftToRight",true);
            dc.sv("temp.willMoveNum",0);
            dc.sv("temp.leftLength",0);
            ListWrapper _listLeft = this["list_left"].GetComponent<ListWrapper>();
            //为显示列表设置数据过滤条件
            _listLeft.resetFilterFunc((jsonNode_)=>{//左侧列表
                if(jsonNode_["isLeft"].AsBool == false){//过滤掉右侧元素
                    return true;
                }
                return false;
            });
            ListWrapper _listRight = this["list_right"].GetComponent<ListWrapper>();
            _listRight.resetFilterFunc((jsonNode_)=>{//右侧列表
                if(jsonNode_["isLeft"].AsBool == true){//过滤掉左侧元素
                    return true;
                }
                return false;
            });
            ScrollWrapper _leftScroll = this["scroll_left"].GetComponent<ScrollWrapper>();
            ScrollWrapper _rightScroll = this["scroll_right"].GetComponent<ScrollWrapper>();
            _leftScroll.reset("temp.list",this);
            _rightScroll.reset("temp.list",this);
            onChange( new string[] {
                "temp.list",
                uiPath+".leftToRight"
            },(changePathList_)=>{
                //当前移动朝向
                bool _leftToRight = ui_gv("leftToRight").AsBool;
                //统计左右剩余个数
                int _leftLength = 0;
                int _rightLength = 0;
                JSONArray _dataList = dc.gv("temp.list").AsArray;
                for (int _idx = 0; _idx < _dataList.Count; _idx++) {
                    JSONNode _jsNode = _dataList[_idx];
                    if(_jsNode["isLeft"].AsBool){
                        _leftLength++;
                    }else{
                        _rightLength++;
                    }
                }
                //任何一侧为空
                if((_leftLength == 0 && _leftToRight)||(_rightLength == 0 && !_leftToRight)){
                    dc.sv(uiPath+".leftToRight",!_leftToRight);
                    return;
                }
                //根据朝向设置，当前可移动的个数
                if(_leftToRight){
                    dc.sv("temp.currentLength",_leftLength);
                }else{
                    dc.sv("temp.currentLength",_rightLength);
                }
                //重置左右个数
                dc.sv("temp.leftLength",_leftLength);
                dc.sv("temp.rightLength",_rightLength);
            });
        }
        // public override void OnDestroy(){
        //     base.OnDestroy();
        // }
        void Update(){
            float _dt = Time.deltaTime;
            DataCenter.frameUpdate(_dt);
            UIItem.doFrameUpdate(_dt);
            ComponentWrapper.doFrameUpdate(_dt);
        }
        private void changeWillMoveNum(int bufferInt_){
            dc.sv("temp.willMoveNum",dc.gv("temp.willMoveNum").AsInt + bufferInt_);
        }
        public override void onBtn(string btnName_){
            if(btnName_ == "btnPlus_add" || btnName_ == "btnPlus_sub"){
                if(btnName_ == "btnPlus_add"){
                    changeWillMoveNum(1);
                }else if(btnName_ == "btnPlus_sub"){
                    changeWillMoveNum(-1);
                }
            }else if(btnName_ == "btn_move"){//进行移动
                bool _leftToRight = dc.gv(uiPath+".leftToRight").AsBool;//获取当前移动朝向
                int _moveCount = dc.gv("temp.willMoveNum").AsInt;//当前所要移动的个数
                if(_moveCount > 0){
                    JSONArray _dataList = dc.gv("temp.list").AsArray;
                    for (int _idx = 0; _idx < _dataList.Count; _idx++) {
                        JSONNode _jsNode = _dataList[_idx];
                        bool _isLeft = _jsNode["isLeft"].AsBool;//当前是在左还是在右
                        if(
                            (_leftToRight && _isLeft ) || //向右移动，在左侧
                            (!_leftToRight && !_isLeft)//向左移动，在右侧
                        ){
                            _jsNode["isLeft"].AsBool = !_isLeft;//变更左右
                            _moveCount--;
                        }
                        if(_moveCount <= 0){
                            break;
                        }
                    }
                    dc.setDirty("temp.list");//手动设置一下列表刚刚发生了变更。
                }
                dc.sv("temp.willMoveNum",0);//重置移动个数
            }
            base.onBtn(btnName_);
        }
        public override void onCheck(string btnName_,string key_,bool isOn_){
            if(btnName_ == "check_leftToRight"){
                dc.sv("temp.willMoveNum",0);//重置移动个数
            }
        }
        public override void onPress(string btnName_){
            if(btnName_ == "btnPlus_add"){
                changeWillMoveNum(1);
            }else if(btnName_ == "btnPlus_sub"){
                changeWillMoveNum(-1);
            }
            base.onPress(btnName_);
        }
        public override void onDoubleClick(string btnName_){
            if(btnName_ == "btnPlus_add"){
                dc.sv("temp.willMoveNum",dc.gv("temp.currentLength").AsInt);
            }else if(btnName_ == "btnPlus_sub"){
                dc.sv("temp.willMoveNum",0);
            }
            base.onDoubleClick(btnName_);
        }
        private void createDataForList(string listDataPath_,int count_){
            JSONArray _jsonArray = new JSONArray();
            int _idx = 0;
            while(count_ > 0){
                JSONObject _jsonObject = new JSONObject();
                _jsonObject["idx"] = _idx;
                _jsonObject["value"] = count_;
                _jsonObject["isLeft"] = UnityEngine.Random.Range(0,2) == 1?true:false;
                _idx ++;
                count_--;
                _jsonArray.Add(_jsonObject);
            }
            dc.sv(listDataPath_,_jsonArray);
        }
    }
}
