using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataUIBinder;
namespace DataUIBinder{
    public class InputTimeItem : UIItem{
        private Button _check_isSelected = null;
        public override void reUse(int idx_, string listDataPath_,int dataIdx_,ScrollRect scrollRect_ = null){
            base.reUse(idx_, listDataPath_,dataIdx_,scrollRect_);
            if(_check_isSelected == null){
                _check_isSelected = this["check_isSelected"].GetComponent<ButtonCheckWrapper>().btn;
            }
            onChange(new string[]{
                parentUINode.dtPath+".time",//父节点的time变化，触发
            },(changePathList_)=>{
                //自己挂载的数据路径
                string _itemTime = dt_gv("time").AsString;
                //父节点挂载的数据路径
                string _parentTime = parentUINode.dt_gv("time").AsString;
                bool _isSame = _itemTime == _parentTime;
                //确定 ui 上 isSelected 的值，从而影响 check_isSelected 按钮的显示。
                ui_sv("isSelected",_isSame);
                // 不同的时候，才可选
                _check_isSelected.enabled = !_isSame;
            });
        }
        // public override void onBtn(string btnName_){
        //     base.onBtn(btnName_);
        // }
        // public override void onPress(string btnName_){
        //     base.onPress(btnName_);
        // }
        // public override void onDoubleClick(string btnName_){
        //     base.onDoubleClick(btnName_);
        // }
        public override void onCheck(string btnName_,string key_,bool isOn_){
            base.onCheck(btnName_,key_,isOn_);
            //选中的时候，会通过过载的 dataPath 的 time 属性，去重置父容器挂载的数据源的 time 属性。
            if (btnName_ == "check_isSelected" && isOn_){
                parentUINode.dt_sv("time",dt_gv("time").AsString);
            }
        }
        // public override void onToggle(string btnName_,string key_,string value_){
        //     base.onToggle(btnName_,key_,value_);
        // }
    }
}