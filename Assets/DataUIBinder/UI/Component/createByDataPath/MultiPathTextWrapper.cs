using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
namespace DataUIBinder{
    /*
        文字封装，多路径
            ${数据[格式化]}
    */
    public class MultiPathTextWrapper : ComponentWrapper{
        protected DataPathListListener dataPathListListener;
        protected string[] dataPathAndStrList;
        private Text _txt = null;
        private Text txt{
            get{
                if(_txt == null){
                    _txt = gameObject.GetComponent<Text>();
                }
                return _txt;
            }
        }
        private string[] formatList;
        protected virtual void Awake(){
            base.Awake();
#if UNITY_EDITOR
            if(GetComponent<TextWrapper>() != null){
                throw new Exception("ERROR : TextWrapper and MultiPathTextWrapper cann't exist in the same time.\ndisplayPath : "+DisplayUtils.getDisplayPath(transform)+"\ntext:"+txt.text);
            }
#endif
        }
        private string tryGetNumberFormatStr(string pathAndFormat_,int idx_){
            if(pathAndFormat_.IndexOf('[') > 0 && pathAndFormat_.IndexOf(']') > 0){
                string[] _pathAndFormatArray = pathAndFormat_.Split(new string[] { "[","]" }, StringSplitOptions.None);
                formatList[idx_] = _pathAndFormatArray[1];
                return _pathAndFormatArray[0];
            }
            return pathAndFormat_;
        }
        public override void reset(string pattern_,UINode uiNode_ = null){
            pattern = pattern_;
            resetUINode(uiNode_);
            dataPath = null;
            clearExpressionList();
            dataPathAndStrList = pattern.getPathAndStrMixedArray();
            string[] _pathList = new string[(dataPathAndStrList.Length - 1)/2];
            formatList = new string[dataPathAndStrList.Length];
            for (int _idx = 0; _idx < dataPathAndStrList.Length; _idx++) {
                formatList[_idx] = null;
            }
            for (int _idx = 0; _idx < dataPathAndStrList.Length; _idx++) {
                if(_idx % 2 == 1){//路径
                    string _convertedDataPathByUINode = dataPathAndStrList[_idx];
                    if(uiNode_ != null){
                        _convertedDataPathByUINode = uiNode_.convertThisOrDataPath(_convertedDataPathByUINode);   
                    }
                    _convertedDataPathByUINode = tryGetNumberFormatStr(_convertedDataPathByUINode,_idx);
                    _convertedDataPathByUINode = getExpressionPath(_convertedDataPathByUINode);
                    int _targetIdx = (_idx - 1)/2;
                    _pathList[_targetIdx] = _convertedDataPathByUINode;
                    dataPathAndStrList[_idx] = _convertedDataPathByUINode;
                }
            }
            dataPathListListener?.unUse();
            dataPathListListener = DataPathListListener.reUse();
            dataPathListListener.reset(_pathList,dataChangeHandle,false);
#if UNITY_EDITOR
            if(debugRecord && isInit == false){
                isInit = true;
                dc.sv("debug.objectCount.wrapper",dc.gv("debug.objectCount.wrapper").AsInt + 1);
            }
#endif
        }
        public virtual void dataChangeHandle(List<string> _){ 
            StringBuilder _sb = CSharpExtensionUtils.SBInstance;
            _sb.Clear();
            for (int _idx = 0; _idx < dataPathAndStrList.Length; _idx++) {
                string _dataPathOrStr = dataPathAndStrList[_idx];
                if(_idx % 2 == 1){//路径
                    JSONNode _valueJsNode = DataCenter.root.getValue(_dataPathOrStr);
                    if (_valueJsNode == null){
                        _sb.Append("<NIL>");
                    }else{
                        string _value;
                        if(formatList[_idx]!=null){
                            _value = DataCenter.getFormatString(_valueJsNode,formatList[_idx]);
                        }else{
                            _value = _valueJsNode.AsString;
                        }
                        _sb.Append(_value);
                    }
                }else{
                    _sb.Append(_dataPathOrStr);
                }
            }
            txt.text = _sb.ToString();
            _sb.Clear();
        }
        protected override void OnDestroy(){
            if(isDestroyed){ return; }
            dataPathListListener?.unUse();
            dataPathListListener = null;
            _txt = null;
            base.OnDestroy();
        }
    }
}
