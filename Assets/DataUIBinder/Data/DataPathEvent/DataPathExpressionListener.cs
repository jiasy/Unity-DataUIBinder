using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
namespace DataUIBinder{
    public class DataPathExpressionListener:DataPathListListener{
        public new static string fullClassName = nameof(DataUIBinder)+"."+nameof(DataPathExpressionListener);
        public new static DataPathExpressionListener reUse(){
            return ReUseObj.reUseObj(fullClassName) as DataPathExpressionListener;
        }
        private static int uniqueID = 0;
        protected override void destroy(){
            for (int _idx = 0; _idx < valueList.Count; _idx++) {
                valueList[_idx] = null;
            }
            valueList.Clear();
            valueList = null;
            pattern = null;
            propertyList = null;
            property2D = null;
            base.destroy();
        }
        protected List<string> valueList;
        protected string pattern;
        private string targetDataPath = null;
        private Property2DWrapper property2D = null;
        private string[] propertyList = null;
        public string reset(string pattern_,Property2DWrapper property2D_ = null){
            pattern = pattern_;
            property2D = property2D_;
            if(targetDataPath == null){
                uniqueID++;
                targetDataPath = "temp.calculation."+uniqueID.ToString();
            }
            List<string> _calculationList = pattern.getExpressionList();
            if(_calculationList.Count < 2){
                throw new Exception("ERROR: 算式切分错误");
            }
            string[] _dataPathList = new string[_calculationList.Count];
            propertyList = new string[_calculationList.Count];
            valueList = new List<string>();
            for (int _idx = 0; _idx < _calculationList.Count; _idx++) {
                string _pathOrStr = _calculationList[_idx];
                valueList.Add(null);
                if(_idx % 2 == 1){
                    if(_pathOrStr.IndexOf('.')<0){
                        if(property2D == null){
                            throw new Exception("ERROR : "+pattern+" 没有和 Property2DWrapper 关联 ，" + _pathOrStr + " 属性转换成值。");
                        }
                        _dataPathList[_idx] = null;
                        propertyList[_idx] = _pathOrStr;
                    }else{
                        _dataPathList[_idx] = _pathOrStr;
                        propertyList[_idx] = null;
                    }
                }else{
                    valueList[_idx] = _pathOrStr;
                    _dataPathList[_idx] = null;
                    propertyList[_idx] = null;
                }
            }
            base.reset(_dataPathList,dataChangeHandle,true);
            return targetDataPath;
        }
        protected void dataChangeHandle(List<string> _){
            bool _hasProperty = false;
            for (int _idx = 0; _idx < pathList.Length; _idx++) {
                var _dataPath = pathList[_idx];
                if(_dataPath != null){
                    valueList[_idx] = DataCenter.root.getValue(_dataPath).AsString;
                }else{
                    if(propertyList[_idx] != null){
                        _hasProperty = true;
                        if(propertyList[_idx] == "x"){
                            valueList[_idx] = property2D.x.ToString();
                        }else if(propertyList[_idx] == "y"){
                            valueList[_idx] = property2D.y.ToString();
                        }else if(propertyList[_idx] == "sx"){
                            valueList[_idx] = property2D.sx.ToString();
                        }else if(propertyList[_idx] == "sy"){
                            valueList[_idx] = property2D.sy.ToString();
                        }else if(propertyList[_idx] == "a"){
                            valueList[_idx] = property2D.a.ToString();
                        }else if(propertyList[_idx] == "r"){
                            valueList[_idx] = property2D.r.ToString();
                        }else{
                            throw new Exception("ERROR : 意料之外的属性 : " + propertyList[_idx]);
                        }
                    }
                }
            }
            string _calculationStr = valueList.joinStr();
            DataCenter.root.setValue(targetDataPath,Convert.ToDouble(_calculationStr.getExpressionResult()));
        }
    }
}