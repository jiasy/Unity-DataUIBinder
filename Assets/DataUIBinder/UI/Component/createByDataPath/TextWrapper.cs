using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
namespace DataUIBinder{
    /*
        文字封装
            [ , ] 包装，指定显示格式
    */
    public class TextWrapper : ComponentWrapper{
        private Text _txt = null;
        private Text txt{
            get{
                if(_txt == null){
                    _txt = gameObject.GetComponent<Text>();
                }
                return _txt;
            }
        }
        private string format = null;
        protected override void Awake(){
            base.Awake();
#if UNITY_EDITOR
            if(gameObject.GetComponent<MultiPathTextWrapper>() != null){
                throw new Exception("ERROR : TextWrapper 和 MultiPathTextWrapper 不能同时存在。");
            }
#endif
            // if(!txt.text.Contains("<") && !txt.text.Contains(">")){
            //     txt.supportRichText = false;
            // }
        }
        public override void reset(string pattern_,UINode uiNode_ = null){
            pattern = pattern_;
            dataPath = null;
            clearExpressionList();
            format = null;
            string _dataPath = pattern_;
            if(_dataPath.IndexOf('[') > 0 && _dataPath.IndexOf(']') > 0){
                //SAMPLE - String - 两个 字符串 切 字符串
                string[] _pathAndFormatArray = _dataPath.Split(new string[] { "[","]" }, StringSplitOptions.None);
                format = _pathAndFormatArray[1];
                _dataPath = _pathAndFormatArray[0];
            }
            base.reset(_dataPath,uiNode_);
        }
        public override void dataChangeHandle(string dataPath_,JSONNode jsNode_){
            string _value;
            if(format != null){
                _value = DataCenter.getFormatString(jsNode_,format);
            }else{
                _value = jsNode_.AsString;
            }
            txt.text = _value;
        }
        protected override void OnDestroy(){
            if(isDestroyed){ return; }
            _txt = null;
            base.OnDestroy();
        }
    }
}