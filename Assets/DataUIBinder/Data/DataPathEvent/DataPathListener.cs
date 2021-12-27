
using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
namespace DataUIBinder{
    public class DataPathListener:ReUseObj{
        public static bool debugRecord = false;
        public static string fullClassName = nameof(DataUIBinder)+"."+nameof(DataPathListener);
        public static DataPathListener reUse(){
            return ReUseObj.reUseObj(fullClassName) as DataPathListener;
        }
        public override void unUse(){
            destroy();
            base.unUse();
        }
        private void destroy(){
            if(path != null){
                DataCenter.removeListener(path,dataChangeHandle);
            }
            path = null;
            callback = null;
#if UNITY_EDITOR
        if (debugRecord){
            runningDPs.Remove(this);
            dc.sv("debug.objectList.dp",getDPListAsStr());
        }
#endif
        }
        
        private Action<string,JSONNode> callback = null;
        protected string path = null;
#if UNITY_EDITOR
        private static List<DataPathListener> runningDPs = new List<DataPathListener>();
        private string getDPListAsStr(){//这个拼接可能造成卡顿。
            string _returnStr = "";
            for (int _idx = 0; _idx < runningDPs.Count; _idx++) {
                _returnStr += runningDPs[_idx].path + "\n";
            }
            return _returnStr;
        }
#endif
        public void reset(string path_,Action<string,JSONNode> callback_){
            if(path_ == null){
                throw new Exception("ERROR : 监听数据路径为空");
            }else if(path_.Contains("_ui_.")||path_.Contains("_dt_.")){
                throw new Exception("ERROR : 监听路径不能是相路径 : "+path_);
            }
            path = path_;
            callback = callback_;
            DataCenter.addListener(path,dataChangeHandle);
            dataChangeHandle(path,DataCenter.root.getValue(path));
#if UNITY_EDITOR
        if (debugRecord){
            runningDPs.Add(this);
            dc.sv("debug.objectList.dp",getDPListAsStr());
        }
#endif
        }
        private void dataChangeHandle(string dataPath_, JSONNode value_) {
            if(value_ != null){
                if(callback != null){
                    callback(dataPath_,value_);
                }else{
                    throw new Exception("ERROR : " + path+ " 监听器为空");
                }
            }
        }
    }
}