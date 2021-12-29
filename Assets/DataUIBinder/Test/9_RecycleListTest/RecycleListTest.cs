using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
namespace DataUIBinder{
    public class RecycleListTest : UINode{
        private int itemNum = -1;
        protected override void Awake(){
            DataCenter.defalutInit();
            uiPath = "ui.RecycleListTest";
            base.Awake();
            //不是通过UIManager创建，需要手动进行一次显示数据绑定
            this.resetDataUIBind();
        }
        IEnumerator Start(){
            dc.sv("temp.scroll",250);//滚动值设置在中位
            dc.sv("temp.min",150);//起始值 150
            dc.sv("temp.max",350);//终止值 350
            dc.sv("temp.itemNum",4);// 道具一行/列个数
            ui_sv("face","xy");//列表显示状态
            ui_sv("sort",true);//是否需要排序列表元素
            ui_sv("filter",true);//是否需要过滤列表元素
            ui_sv("list",new JSONArray());//显示挂载数据
            createDataForList("temp.list",50);//创建初始数据
            ListWrapper _lisX = this["list_x"].GetComponent<ListWrapper>();
            ListWrapper _lisY = this["list_y"].GetComponent<ListWrapper>();
            onChange("temp.min",(min_)=>{//最小值决定列表起始位置
                _lisX.beginPos = min_.AsFloat;
                _lisY.beginPos = min_.AsFloat;
            });
            onChange("temp.max",(max_)=>{//最大值决定列表的终止位置
                _lisX.endPos = max_.AsFloat;
                _lisY.endPos = max_.AsFloat;
            });
            onChange(new string[]{//当一下数据变化
                "temp.list",//列表数据
                "temp.itemNum",//列表每行显示的个数
                uiPath+".filter",//列表过滤模式
                uiPath+".sort"//列表的排列模式
            },(changeDataPathList_)=>{//根据变更后的数据，刷新列表显示
                int _target = dc.gv("temp.itemNum").AsInt;
                if(
                    changeDataPathList_.Count == 1 &&
                    changeDataPathList_[0] == "temp.itemNum" &&
                    _target == itemNum
                ){
                    //有且只有 temp.itemNum 变化时，判断其整数部是否变更
                    //若个数没有变化，就不进行刷新，以免频繁刷新造成列表闪烁。
                    return;
                }
                itemNum = _target;
                resetList(
                    "temp.list",
                    uiPath + ".list",
                    itemNum,
                    ui_gv("filter").AsBool,
                    ui_gv("sort").AsBool
                );
            });
            yield return null;
        }
        public override void OnDestroy(){
            base.OnDestroy();
        }
        void Update(){
            float _dt = Time.deltaTime;
            DataCenter.frameUpdate(_dt);
            UIItem.doFrameUpdate(_dt);
            ComponentWrapper.doFrameUpdate(_dt);
        }
        private void resetList(string lisDataPath_,string showListDataPath_,int itemNum_,bool needFilter_,bool needSort_){
            ListWrapper _lisX = this["list_x"].GetComponent<ListWrapper>();
            ListWrapper _lisY = this["list_y"].GetComponent<ListWrapper>();
            Func<JSONNode,bool> _filterFunc = null;
            Func<JSONNode, JSONNode,int> _sortFunc = null;
            if(needFilter_){
                _filterFunc = (item_)=>{
                    if(item_["value"].AsInt > 25){
                        return true;
                    }
                    return false;
                };
            }
            if(needSort_){
                _sortFunc = (x_,y_)=>{
                    if(x_["value"].AsInt > y_["value"].AsInt){
                        return 1;
                    }
                    return -1;
                };
            }
            //使用数据流过滤排序数据
            DataCenter.fliterSortListToList(lisDataPath_,showListDataPath_,_filterFunc,_sortFunc);
            //显示过滤排序后的数据
            _lisX.reset(this,showListDataPath_,itemNum_,ListWrapper.ListType.X);
            _lisY.reset(this,showListDataPath_,itemNum_,ListWrapper.ListType.Y);
        }
        private void createDataForList(string listDataPath_,int count_){
            JSONArray _jsonArray = new JSONArray();
            int _idx = 0;
            while(count_ > 0){
                JSONObject _jsonObject = new JSONObject();
                _jsonObject["idx"] = _idx;
                _jsonObject["value"] = count_;
                _idx ++;
                count_--;
                _jsonArray.Add(_jsonObject);
            }
            dc.sv(listDataPath_,_jsonArray);
        }
    }
}
