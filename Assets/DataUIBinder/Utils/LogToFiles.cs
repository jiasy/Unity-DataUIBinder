using System;
using SimpleJSON;
using System.IO;
using System.Collections;
using System.Collections.Generic;
namespace DataUIBinder{
    /*
        将日志输出到本地文件
    */
    public class LogToFiles{
        //日志文件夹
        private static string logFolder = "/Volumes/Files/develop/selfDevelop/Unity/DataCenter/C#Temp/";
        //是否有过日志输出的行为
        private static bool _logged = false;
        private static bool _inited = false;
        //将不同种类的内容输出到不同的文件
        public enum LogType{
            PathValue = 1,//按照 dataPath : orginalValue -> currentValue 的格式，来输出当前的键值变化。
            ListenerState = 2,//按照 dataPath : 1 的格式，来输出当前 dataPath 的监听对象个数。
            DataStruct = 3,//输出指定的 JSONNode 的结构。
            Record = 4,//按照时序，保存UI的点击和服务器的操作。
            Log = 5//只是单纯的日志。
        }
        
        private class ByteListContainer{
            public byte[] byteList;
            public ByteListContainer(byte[] byteList_){
                byteList = byteList_;
            }
        }
        //日志 <类别 : 路径>
        private static Dictionary<LogType,string> logToFileDict = new Dictionary<LogType,string>();
        //日志，内容缓存
        private static Dictionary<string,List<ByteListContainer>> contentCacheDict = new Dictionary<string,List<ByteListContainer>> ();    

        public static void init(){
            _inited = true;
            logToFileDict[LogType.PathValue] = logFolder + nameof(LogType.PathValue);
            logToFileDict[LogType.ListenerState] = logFolder + nameof(LogType.ListenerState);
            logToFileDict[LogType.DataStruct] = logFolder + nameof(LogType.DataStruct);
            logToFileDict[LogType.Record] = logFolder + nameof(LogType.Record);
            logToFileDict[LogType.Log] = logFolder + nameof(LogType.Log);
            if (logToFileDict.Keys.Count > 0){
                var _logToFileDictEnume = logToFileDict.GetEnumerator();
                while (_logToFileDictEnume.MoveNext()) {
                    string _filePath = _logToFileDictEnume.Current.Value;
                    if(System.IO.File.Exists(_filePath)){
                        FileStream _clearFS = new FileStream(_filePath, FileMode.Truncate, FileAccess.ReadWrite);
                        _clearFS.Close();
                    }else{
                        FileStream _createFS = new FileStream(_filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        _createFS.Close();
                    }
                    contentCacheDict[_filePath] = new List<ByteListContainer>();
                }
                _logToFileDictEnume.Dispose();
            }
        }
        public static void log(string log_){
#if UNITY_EDITOR
            logByType(LogType.Log, log_);
#else
            UnityEngine.Log(log_);
#endif
        }
#if UNITY_EDITOR
        public static void printToAll(string log_){
            LogToFiles.logByType(LogToFiles.LogType.ListenerState, log_);
            LogToFiles.logByType(LogToFiles.LogType.DataStruct, log_);
            LogToFiles.logByType(LogToFiles.LogType.PathValue, log_);
            LogToFiles.logByType(LogToFiles.LogType.Record, log_);
            LogToFiles.logByType(LogToFiles.LogType.Log, log_);
        }
        public static void logByType(LogType logType_,string log_){
            if(_inited == false){
                throw new Exception("ERROR : 没有初始化过");
            }
            List<ByteListContainer> _contentCacheList = contentCacheDict[logToFileDict[logType_]];
            lock(contentCacheDict){//缓存为锁
                _logged = true;//重置标示为进行过输出
                byte[] _bs = System.Text.Encoding.Default.GetBytes(log_ + "\n");
                _contentCacheList.Add(new ByteListContainer(_bs));
            }
        }
        public static void printLogList(LogType logType_,string title_,List<string> logList_){
            string _logStr;
            if(logList_.Count > 0){
                _logStr = title_ + "\n";
                logList_.Sort();
                for (int _idx = 0; _idx < logList_.Count; _idx++) {
                    _logStr += logList_[_idx] + "\n";
                }
                LogToFiles.logByType(logType_,_logStr);
            } 
        }
#endif
        public static void frameUpdate(float dt_){
#if UNITY_EDITOR
            if(_inited == false){
                throw new Exception("ERROR : 没有初始化过");
            }
            if(!_logged){//有输出日志的行为才会输出日志。
                return;
            }
            var _contentCacheDictEnume = contentCacheDict.GetEnumerator();//在实际存在的缓存键值中进行遍历。
            while (_contentCacheDictEnume.MoveNext()) {
                string _logFilePath = _contentCacheDictEnume.Current.Key;
                List<ByteListContainer> _contentCacheList = _contentCacheDictEnume.Current.Value;
                if(_contentCacheList.Count > 0 ){//缓存有东西才需要进行写入
                    FileStream _fileStream = new FileStream(_logFilePath,FileMode.Append,FileAccess.Write);
                    for (int _idx = 0; _idx < _contentCacheList.Count; _idx++) {
                        ByteListContainer _byteListContainer = _contentCacheList[_idx];
                        _fileStream.Seek(0, SeekOrigin.End);
                        _fileStream.Write(_byteListContainer.byteList,0,_byteListContainer.byteList.Length);
                        _fileStream.Flush();
                    }
                    _contentCacheList.Clear();//清理缓存
                    _fileStream.Close();
                    _fileStream.Dispose();
                    _fileStream = null;
                }
            }
            _contentCacheDictEnume.Dispose();
            //重置标示
            _logged = false;
#endif
        }

    }
}