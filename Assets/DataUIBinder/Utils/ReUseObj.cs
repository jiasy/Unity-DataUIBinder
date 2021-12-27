using System;
using System.Collections.Generic;
namespace DataUIBinder{
    //数据变化触发方法
    public class ReUseObj{
        public static bool debugRecord = false;
        public static Dictionary<string,List<ReUseObj>> simplePool = new Dictionary<string, List<ReUseObj>>();
        public static Dictionary<string,Type> nameToTypeDict = new Dictionary<string,Type>();
        private static ReUseObj createReUseObj(Type type_){
            return System.Activator.CreateInstance(type_) as ReUseObj;
        }
        public static ReUseObj reUseObj(string fullClassName_){
            Type _classType;
            if(!nameToTypeDict.TryGetValue(fullClassName_, out _classType)){
                _classType = Type.GetType(fullClassName_);
                if(_classType == null){
                    throw new Exception("ERROR : " + fullClassName_ + " 无法获得的对象");
                }
                nameToTypeDict[fullClassName_] = _classType;
            }
            List<ReUseObj> _objPool;
            if(!simplePool.TryGetValue(fullClassName_, out _objPool)){
                _objPool = new List<ReUseObj>();
                simplePool[fullClassName_] = _objPool;
            }
            ReUseObj _reUseObj;
            if(_objPool.Count > 0){
                _reUseObj = _objPool.pull<ReUseObj>();
            }else{
                _reUseObj = createReUseObj(_classType) as ReUseObj;
                _reUseObj.className = fullClassName_;
            }
            _reUseObj.inUse = true;
#if UNITY_EDITOR
            if(debugRecord){
                if(_reUseObj.className == "DataUIBinder.DataPathListener"){
                    dc.sv("debug.objectCount.dp",dc.gv("debug.objectCount.dp").AsInt + 1);
                }else if(_reUseObj.className == "DataUIBinder.DataPathListListener"){
                    dc.sv("debug.objectCount.dpList",dc.gv("debug.objectCount.dpList").AsInt + 1);
                }else if(_reUseObj.className == "DataUIBinder.DataPathCompareListener"){
                    dc.sv("debug.objectCount.dpCompare",dc.gv("debug.objectCount.dpCompare").AsInt + 1);
                }else if(_reUseObj.className == "DataUIBinder.DataPathExpressionListener"){
                    dc.sv("debug.objectCount.dpExpression",dc.gv("debug.objectCount.dpExpression").AsInt + 1);
                }else if(_reUseObj.className == "DataUIBinder.DataPathRangeCompareListener"){
                    dc.sv("debug.objectCount.dpRange",dc.gv("debug.objectCount.dpRange").AsInt + 1);
                }
            }
#endif
            return _reUseObj;
        }
        public static void unUseObj(ReUseObj reUseObj_){
            if(reUseObj_.className == null){
                throw new Exception("ERROR :  对象必须，通过 reUse 方法创建。");
            }
            if(reUseObj_.inUse == false){
                throw new Exception("ERROR :  当前对象，不在使用中。");
            }
            List<ReUseObj> _pool;
            if(!simplePool.TryGetValue(reUseObj_.className, out _pool)){
                throw new Exception("ERROR : " + reUseObj_.className + " 池还未创建");
            }
            reUseObj_.inUse = false;
            _pool.Add(reUseObj_);
#if UNITY_EDITOR
            if(debugRecord){
                if(reUseObj_.className == "DataUIBinder.DataPathListener"){
                    dc.sv("debug.objectCount.dp",dc.gv("debug.objectCount.dp").AsInt - 1);
                }else if(reUseObj_.className == "DataUIBinder.DataPathListListener"){
                    dc.sv("debug.objectCount.dpList",dc.gv("debug.objectCount.dpList").AsInt - 1);
                }else if(reUseObj_.className == "DataUIBinder.DataPathCompareListener"){
                    dc.sv("debug.objectCount.dpCompare",dc.gv("debug.objectCount.dpCompare").AsInt - 1);
                }else if(reUseObj_.className == "DataUIBinder.DataPathExpressionListener"){
                    dc.sv("debug.objectCount.dpExpression",dc.gv("debug.objectCount.dpExpression").AsInt - 1);
                }else if(reUseObj_.className == "DataUIBinder.DataPathRangeCompareListener"){
                    dc.sv("debug.objectCount.dpRange",dc.gv("debug.objectCount.dpRange").AsInt - 1);
                }
            }
#endif
        }
        public virtual void unUse(){
            if(!inUse){
                return;
            }
            unUseObj(this);
        }
        public bool inUse = false;
        public string className = null;
    }
}