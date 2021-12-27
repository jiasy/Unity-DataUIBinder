using System;
using SimpleJSON;
using System.Collections.Generic;
namespace DataUIBinder{
    public class DataPathDriven:ReUseObj{
        public static string fullClassName = nameof(DataUIBinder)+"."+nameof(DataPathDriven);
        public static DataPathDriven reUse(){
            return ReUseObj.reUseObj(fullClassName) as DataPathDriven;
        }
        public override void unUse(){
            destroy();
            base.unUse();
        }
        private void destroy(){
            for (int _idx = 0; _idx < dataPathListenerList.Count; _idx++) {
                dataPathListenerList[_idx].unUse();
            }
            dataPathListenerList.Clear();
        }

        public List<ReUseObj> dataPathListenerList = new List<ReUseObj>();        
        public void removeListener(ReUseObj listener_){
            int _idx = dataPathListenerList.IndexOf(listener_);
            if(_idx >= 0){
                dataPathListenerList.RemoveAt(_idx);
            }
            listener_.unUse();
        }
#region data flow trigger
        public void compareTrigger(string pattern_,Action<bool> resultHandle_){
            DataPathCompareListener _dataPathListener = DataPathCompareListener.reUse();
            dataPathListenerList.Add(_dataPathListener);
            _dataPathListener.reset(pattern_,resultHandle_,null);
        }
        public void rangeTrigger(string pattern_,Action<bool> resultHandle_){
            DataPathRangeCompareListener _dataPathListener = DataPathRangeCompareListener.reUse();
            dataPathListenerList.Add(_dataPathListener);
            _dataPathListener.reset(pattern_,resultHandle_,null);
        }
        public ReUseObj onChange(string dataPath_,Action<JSONNode> pathChangeHandle_){
            DataPathListener _dataPathListener = DataPathListener.reUse();
            dataPathListenerList.Add(_dataPathListener);
            _dataPathListener.reset(dataPath_,(_,jsNodeOnDataPath_)=>{
                pathChangeHandle_(jsNodeOnDataPath_);
            });
            return _dataPathListener;
        }
        public ReUseObj onChange(string[] dataPathList_,Action<List<string>> pathChangeHandle_){
            DataPathListListener _dataPathListListener = DataPathListListener.reUse();
            dataPathListenerList.Add(_dataPathListListener);
            _dataPathListListener.reset(dataPathList_,(changePathList_)=>{
                pathChangeHandle_(changePathList_);
            },true);
            return _dataPathListListener;
        }
        public void dictToDict(string dictPath_,string targetDictPath_,Action beginAction_ = null,Action endAction_ = null){
            DataPathListener _dataPathListener = DataPathListener.reUse();
            dataPathListenerList.Add(_dataPathListener);
            _dataPathListener.reset(dictPath_,(_,jsNodeOnDataPath_)=>{
                if(beginAction_ != null){
                    beginAction_();
                }
                DataCenter.mergeDictToDict(dictPath_,targetDictPath_);
                if(endAction_ != null){
                    endAction_();
                }
            });
        }
        public void listToDict(string listPath_,string dictPath_,string mergeKey_,Action beginAction_ = null,Action endAction_ = null){
            DataPathListener _dataPathListener = DataPathListener.reUse();
            dataPathListenerList.Add(_dataPathListener);
            _dataPathListener.reset(listPath_,(_,jsNodeOnDataPath_)=>{
                if(beginAction_ != null){
                    beginAction_();
                }
                DataCenter.mergeListToDict(listPath_,dictPath_,mergeKey_);
                if(endAction_ != null){
                    endAction_();
                }
            });
        }
        public void listFromDict(string listPath_,string dictPath_,string mergeKey_,Action beginAction_ = null,Action endAction_ = null){
            DataPathListener _dataPathListener = DataPathListener.reUse();
            dataPathListenerList.Add(_dataPathListener);
            _dataPathListener.reset(listPath_,(_,jsNodeOnDataPath_)=>{
                if(beginAction_ != null){
                   beginAction_();
                }
                DataCenter.mergeDictToList(dictPath_,listPath_,mergeKey_);
                if(endAction_ != null){
                    endAction_();
                }
            });
        }
        public void listToList(string listPath_,string targetListPath_,string mergeKey_,Action beginAction_ = null,Action endAction_ = null){
            DataPathListener _dataPathListener = DataPathListener.reUse();
            dataPathListenerList.Add(_dataPathListener);
            _dataPathListener.reset(listPath_,(_,jsNodeOnDataPath_)=>{
                if(beginAction_ != null){
                   beginAction_();
                }
                DataCenter.mergeListToList(listPath_,targetListPath_,mergeKey_);
                if(endAction_ != null){
                    endAction_();
                }
            });
        }
        public void listFilterSortToList(string listPath_,string targetListPath_,Func<JSONNode,bool> filterFunc_ = null,Func<JSONNode,JSONNode,int> sortFunc_ = null,Action beginAction_ = null,Action endAction_ = null){
            DataPathListener _dataPathListener = DataPathListener.reUse();
            dataPathListenerList.Add(_dataPathListener);
            _dataPathListener.reset(listPath_,(_,jsNodeOnDataPath_)=>{
                if(beginAction_ != null){
                   beginAction_();
                }
                DataCenter.fliterSortListToList(listPath_,targetListPath_,filterFunc_,sortFunc_);
                if(endAction_ != null){
                    endAction_();
                }
            });
        }
#endregion
    }
}