using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DataUIBinder;
namespace DataUIBinder{
    /*
        列表元素实例
            无论是列表的副本还是列表的过滤，对列表元素本身并无影响。
            所以，两个列表实例的列表元素使用同一个。
    */
    public class RecycleScrollItem : UIItem{
        public override void reUse(int idx_, string listDataPath_,int dataIdx_,ScrollRect scrollRect_ = null){
            base.reUse(idx_, listDataPath_,dataIdx_,scrollRect_);
            onChange(new string[]{
                dtPath+".isLeft",
                parentUINode.uiPath + ".leftToRight",
                "temp.willMoveNum",
            },(changePathList_)=>{
                bool _isLeft = dt_gv("isLeft").AsBool;
                bool _leftToRight = parentUINode.ui_gv("leftToRight").AsBool;
                int _willMoveNum = dc.gv("temp.willMoveNum").AsInt;
                Transform _trans = this["trans_markAsMove"];
                if(_isLeft && _leftToRight || !_isLeft && !_leftToRight){
                    if(itemIdx < _willMoveNum){
                        _trans.gameObject.SetActive(true);
                        return;
                    }
                }
                _trans.gameObject.SetActive(false);
            });
        }
    }
}