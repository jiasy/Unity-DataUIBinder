using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
namespace DataUIBinder{
    public class DataPathDrivenComponent : MonoBehaviour{
        private DataPathDriven _dataPathDriven = null;
        private DataPathDriven dataPathDriven{
            get{
                if(_dataPathDriven == null){
                    _dataPathDriven = DataPathDriven.reUse();
                }
                return _dataPathDriven;
            }
            set{
                _dataPathDriven?.unUse();
                _dataPathDriven = value;
            }
        }
        public virtual void OnDestroy(){
            _dataPathDriven?.unUse();
            _dataPathDriven = null;
        }
        public void removeListener(ReUseObj listener_){
            dataPathDriven.removeListener(listener_);
        }
#region data flow trigger
        public void compareTrigger(string pattern_,Action<bool> resultHandle_){
            dataPathDriven.compareTrigger(pattern_, resultHandle_);
        }
        public void rangeTrigger(string pattern_,Action<bool> resultHandle_){
            dataPathDriven.rangeTrigger(pattern_, resultHandle_);
        }
        public virtual ReUseObj onChange(string dataPath_,Action<JSONNode> pathChangeHandle_){
            return dataPathDriven.onChange(dataPath_, pathChangeHandle_);
        }
        public virtual ReUseObj onChange(string[] dataPathList_,Action<List<string>> pathChangeHandle_){
            return dataPathDriven.onChange(dataPathList_, pathChangeHandle_);
        }
        public void dictToDict(string dictPath_,string targetDictPath_,Action beginAction_ = null,Action endAction_ = null){
            dataPathDriven.dictToDict(dictPath_,targetDictPath_,beginAction_,endAction_);
        }
        public void listToDict(string listPath_,string dictPath_,string mergeKey_,Action beginAction_ = null,Action endAction_ = null){
            dataPathDriven.listToDict(listPath_,dictPath_,mergeKey_,beginAction_,endAction_);
        }
        public void listFromDict(string listPath_,string dictPath_,string mergeKey_,Action beginAction_ = null,Action endAction_ = null){
            dataPathDriven.listFromDict(listPath_,dictPath_,mergeKey_,beginAction_,endAction_);
        }
        public void listToList(string listPath_,string targetListPath_,string mergeKey_,Action beginAction_ = null,Action endAction_ = null){
            dataPathDriven.listToList(listPath_,targetListPath_,mergeKey_,beginAction_,endAction_);
        }
        public void listFilterSortToList(string listPath_,string targetListPath_,Func<JSONNode,bool> filterFunc_ = null,Func<JSONNode,JSONNode,int> sortFunc_ = null,Action beginAction_ = null,Action endAction_ = null){
            dataPathDriven.listFilterSortToList(listPath_,targetListPath_,filterFunc_,sortFunc_,beginAction_,endAction_);
        }
    }
#endregion
}