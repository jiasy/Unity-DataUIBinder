using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
namespace DataUIBinder{
    public class DataChangeDispatcher{
        private Dictionary<string, Action<string, JSONNode>> dataPathActionDict = new Dictionary<string, Action<string, JSONNode>>();
#if UNITY_EDITOR
        class DataPathListenerCountInfo{
            public string dataPath;
            public int count;
            public int lastCount;
        }
        private Dictionary<string, DataPathListenerCountInfo> dataPathListenerCountDict = new Dictionary<string, DataPathListenerCountInfo> ();
        public bool isListenerChanged = false;
        private List<string> clearList = new List<string>();
#endif
        
        public void addListener(string dataPath_, Action<string, JSONNode> callBack_) {
            if (!dataPathActionDict.ContainsKey(dataPath_)) {
                dataPathActionDict.Add(dataPath_, null);
#if UNITY_EDITOR
                if (!dataPathListenerCountDict.ContainsKey(dataPath_)) {
                    DataPathListenerCountInfo _countInfoCreate = new DataPathListenerCountInfo();
                    _countInfoCreate.dataPath = dataPath_;
                    _countInfoCreate.count = 0;
                    _countInfoCreate.lastCount = 0;
                    dataPathListenerCountDict.Add(dataPath_,_countInfoCreate);
                }
#endif
            }
            Action<string,JSONNode> _action = dataPathActionDict[dataPath_];
            dataPathActionDict[dataPath_] = dataPathActionDict[dataPath_] + callBack_;
#if UNITY_EDITOR
            DataPathListenerCountInfo _countInfo;
            if(dataPathListenerCountDict.TryGetValue(dataPath_,out _countInfo)){
                _countInfo.count = _countInfo.count + 1;
                isListenerChanged = true;
            }else{
                throw new Exception("ERROR : " + dataPath_ + " : 事件计数中没有创建过");
            }
#endif
        }
        public void removeListener (string dataPath_, Action<string, JSONNode> callBack_) {
            if (!dataPathActionDict.ContainsKey(dataPath_)) {
                throw new Exception("ERROR : " + string.Format("移除监听错误：不存在事件{0}", dataPath_));
            }
            dataPathActionDict[dataPath_] = dataPathActionDict[dataPath_] - callBack_;
#if UNITY_EDITOR
            DataPathListenerCountInfo _countInfo;
            if(dataPathListenerCountDict.TryGetValue(dataPath_,out _countInfo)){
                _countInfo.count = _countInfo.count - 1;
                isListenerChanged = true;
            }else{
                throw new Exception("ERROR : " + dataPath_ + " : 事件计数中没有创建过");
            }
#endif
            if (dataPathActionDict[dataPath_] == null) {
                dataPathActionDict.Remove(dataPath_);
#if UNITY_EDITOR
                clearList.Add(dataPath_);
#endif
            }
        }
        public void dispatchEvent(string dataPath_, JSONNode jsNode_) {
            Action<string, JSONNode> _actionCallBack;
            if (dataPathActionDict.TryGetValue(dataPath_, out _actionCallBack)) {
                _actionCallBack(dataPath_, jsNode_);
            }
        }
#if UNITY_EDITOR
        public List<string> addListenerList = new List<string>();
        public List<string> subListenerList = new List<string>();
        public List<string> clearListenerList = new List<string>();
        public void prepareListenerLog(){
            var dataPathListenerCountDictEnume = dataPathListenerCountDict.GetEnumerator();
            while (dataPathListenerCountDictEnume.MoveNext()) {
                string _path = dataPathListenerCountDictEnume.Current.Key;
                DataPathListenerCountInfo _countInfo = dataPathListenerCountDictEnume.Current.Value;
                string _logInLoop = string.Format("     {0} [{1} -> {2}]", _path.PadRight(91,' '),_countInfo.lastCount.ToString(),_countInfo.count.ToString());
                if(_countInfo.count != _countInfo.lastCount){
                    if(_countInfo.count > _countInfo.lastCount){
                        addListenerList.Add(_logInLoop);
                    }else{
                        if(clearList.Contains(_countInfo.dataPath)){
                            clearListenerList.Add(_logInLoop);
                        }else{
                            subListenerList.Add(_logInLoop);
                        }
                    }
                    _countInfo.lastCount = _countInfo.count;
                }
            }
            dataPathListenerCountDictEnume.Dispose();
        }
        public void printListenerCount(){
            if(isListenerChanged){
                prepareListenerLog();
                LogToFiles.logByType( LogToFiles.LogType.ListenerState, "     ----------------------------------- Listener Changed -----------------------------------" );
                LogToFiles.printLogList(
                    LogToFiles.LogType.ListenerState,
                    "Add --------------------------",
                    addListenerList
                );
                addListenerList.Clear();
                LogToFiles.printLogList(
                    LogToFiles.LogType.ListenerState,
                    "Sub --------------------------",
                    subListenerList
                );
                subListenerList.Clear();
                LogToFiles.printLogList(
                    LogToFiles.LogType.ListenerState,
                    "Clean ------------------------",
                    clearListenerList
                );
                clearListenerList.Clear();
                isListenerChanged = false;
            }
        }
#endif
    }
}