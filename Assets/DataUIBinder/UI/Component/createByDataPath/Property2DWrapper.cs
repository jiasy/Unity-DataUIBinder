using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
namespace DataUIBinder{
    /*
        属性 绑定 数据路径
    */
    public class Property2DWrapper : ComponentWrapper{
        [SerializeField]
        protected string xPathOrExpression = null;
        [SerializeField]
        protected string yPathOrExpression = null;
        [SerializeField]
        protected string scaleXPathOrExpression = null;
        [SerializeField]
        protected string scaleYPathOrExpression = null;
        [SerializeField]
        protected string rotationPathOrExpression = null;
        [SerializeField]
        protected string alphaPathOrExpression = null;
        [SerializeField]
        protected string widthPathOrExpression = null;
        [SerializeField]
        protected string heightPathOrExpression = null;
        private DataPathListener xDataPathListener = null;
        private DataPathListener yDataPathListener = null;
        private DataPathListener scaleXDataPathListener = null;
        private DataPathListener scaleYDataPathListener = null;
        private DataPathListener rotationDataPathListener = null;
        private DataPathListener alphaDataPathListener = null;
        private DataPathListener widthDataPathListener = null;
        private DataPathListener heightDataPathListener = null;
        private CanvasGroup canvasGroup = null;
        private Transform _trans = null;
        private Transform trans{
            get{
                if(_trans == null){
                    _trans = gameObject.GetComponent<Transform>();
                }
                return _trans;
            }
        }
        
        public float x{
            get{
                return trans.localPosition.x;
            }
            set{
                trans.setX(value);
            }
        }
        public float y{
            get{
                return trans.localPosition.y;
            }
            set{
                trans.setY(value);
            }
        }
        public float sx{
            get{
                return trans.localScale.x;
            }
            set{
                trans.setScaleX(value);
            }
        }
        public float sy{
            get{
                return trans.localScale.y;
            }
            set{
                trans.setScaleY(value);
            }
        }
        public float r{
            get{
                return trans.getRotation();
            }
            set{
                trans.setRotation(value);
            }
        }
        public float a{
            get{
                if(canvasGroup == null){
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
                return canvasGroup.alpha;
            }
            set{
                if(canvasGroup == null){
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
                float _finalValue = value;
                if(_finalValue > 1.0f){
                    canvasGroup.alpha = 1.0f;
                }else if(_finalValue < 0.0f){
                    canvasGroup.alpha = 0.0f;
                }else{
                    canvasGroup.alpha = _finalValue;
                }
            }
        }
        public float w{
            get{
                return (trans as RectTransform).getWidth();
            }
            set{
                (trans as RectTransform).setWidth(value);
            }
        }
        public float h{
            get{
                return (trans as RectTransform).getHeight();
            }
            set{
                (trans as RectTransform).setHeight(value);
            }
        }
        protected void reUseListeners(UINode uiNode_){
            if(!xPathOrExpression.isNullOrEmpty()){
                xDataPathListener = DataPathListener.reUse();
                xDataPathListener.reset(getExpressionPath(uiNode_==null?xPathOrExpression:uiNode_.convertThisOrDataPath(xPathOrExpression),this),xPathChangeHandle);
            }
            if(!yPathOrExpression.isNullOrEmpty()){
                yDataPathListener = DataPathListener.reUse();
                yDataPathListener.reset(getExpressionPath(uiNode_==null?yPathOrExpression:uiNode_.convertThisOrDataPath(yPathOrExpression),this),yPathChangeHandle);
            }
            if(!scaleXPathOrExpression.isNullOrEmpty()){
                scaleXDataPathListener = DataPathListener.reUse();
                scaleXDataPathListener.reset(getExpressionPath(uiNode_==null?scaleXPathOrExpression:uiNode_.convertThisOrDataPath(scaleXPathOrExpression),this),scaleXPathChangeHandle);
            }
            if(!scaleYPathOrExpression.isNullOrEmpty()){
                scaleYDataPathListener = DataPathListener.reUse();
                scaleYDataPathListener.reset(getExpressionPath(uiNode_==null?scaleYPathOrExpression:uiNode_.convertThisOrDataPath(scaleYPathOrExpression),this),scaleYPathChangeHandle);
            }
            if(!rotationPathOrExpression.isNullOrEmpty()){
                rotationDataPathListener = DataPathListener.reUse();
                rotationDataPathListener.reset(getExpressionPath(uiNode_==null?rotationPathOrExpression:uiNode_.convertThisOrDataPath(rotationPathOrExpression),this),rotationChangeHandle);
            }
            if(!alphaPathOrExpression.isNullOrEmpty()){
                alphaDataPathListener = DataPathListener.reUse();
                alphaDataPathListener.reset(getExpressionPath(uiNode_==null?alphaPathOrExpression:uiNode_.convertThisOrDataPath(alphaPathOrExpression),this),alphaPathChangeHandle);
            }
            if(!widthPathOrExpression.isNullOrEmpty()){
                widthDataPathListener = DataPathListener.reUse();
                widthDataPathListener.reset(getExpressionPath(uiNode_==null?widthPathOrExpression:uiNode_.convertThisOrDataPath(widthPathOrExpression),this),widthPathChangeHandle);
            }
            if(!heightPathOrExpression.isNullOrEmpty()){
                heightDataPathListener = DataPathListener.reUse();
                heightDataPathListener.reset(getExpressionPath(uiNode_==null?heightPathOrExpression:uiNode_.convertThisOrDataPath(heightPathOrExpression),this),heightPathChangeHandle);
            }
        }
        private void unUseListeners(){
            xPathOrExpression = null;
            yPathOrExpression = null;
            scaleXPathOrExpression = null;
            scaleYPathOrExpression = null;
            rotationPathOrExpression = null;
            alphaPathOrExpression = null;
            widthPathOrExpression = null;
            heightPathOrExpression = null;
            xDataPathListener?.unUse();
            xDataPathListener = null;
            yDataPathListener?.unUse();
            yDataPathListener = null;
            scaleXDataPathListener?.unUse();
            scaleXDataPathListener = null;
            scaleYDataPathListener?.unUse();
            scaleYDataPathListener = null;
            rotationDataPathListener?.unUse();
            rotationDataPathListener = null;
            alphaDataPathListener?.unUse();
            alphaDataPathListener = null;
            widthDataPathListener?.unUse();
            widthDataPathListener = null;
            heightDataPathListener?.unUse();
            heightDataPathListener = null;
        }
        public override void reset(string pattern_,UINode uiNode_ = null){
            pattern = pattern_;
            resetUINode(uiNode_);
            dataPath = null;
            clearExpressionList();
            unUseListeners();
            resetPaths(pattern);
            reUseListeners(uiNode_);
#if UNITY_EDITOR
            if(debugRecord && isInit == false){
                isInit = true;
                dc.sv("debug.objectCount.wrapper",dc.gv("debug.objectCount.wrapper").AsInt + 1);
            }
#endif
        }
        private void resetPaths(string pattern_,UINode uiNode_ = null){
            string[] _propertyBindList = pattern_.getPropertyBindArray();
            string _currentProperty = null;
            for (int _idx = 0; _idx < _propertyBindList.Length; _idx++) {
                string _propertyOrDataPath = _propertyBindList[_idx];
                if(_idx % 2 == 0){
                    _currentProperty = _propertyOrDataPath;
                }else{
                    if(_currentProperty == "x"){
                        xPathOrExpression = _propertyOrDataPath;
                    }else if(_currentProperty == "y"){
                        yPathOrExpression = _propertyOrDataPath;
                    }else if(_currentProperty == "sx"){
                        scaleXPathOrExpression = _propertyOrDataPath;
                    }else if(_currentProperty == "sy"){
                        scaleYPathOrExpression = _propertyOrDataPath;
                    }else if(_currentProperty == "r"){
                        rotationPathOrExpression = _propertyOrDataPath;
                    }else if(_currentProperty == "a"){
                        alphaPathOrExpression = _propertyOrDataPath;
                    }else if(_currentProperty == "w"){
                        widthPathOrExpression = _propertyOrDataPath;
                    }else if(_currentProperty == "h"){
                        heightPathOrExpression = _propertyOrDataPath;
                    }else{
                        throw new Exception("ERROR : " + _currentProperty + ",cann't recognize.");
                    }
                }
            }
        }
        protected override void OnDestroy(){
            if(isDestroyed){ return; }
            unUseListeners();
            _trans = null;
            canvasGroup = null;
            base.OnDestroy();
        }
        private void xPathChangeHandle(string _,JSONNode jsNode_){
            x = jsNode_.AsFloat;
        }
        private void yPathChangeHandle(string _,JSONNode jsNode_){
            y = jsNode_.AsFloat;
        }
        private void scaleXPathChangeHandle(string _,JSONNode jsNode_){
            sx = jsNode_.AsFloat;
        }
        private void scaleYPathChangeHandle(string _,JSONNode jsNode_){
            sy = jsNode_.AsFloat;
        }
        private void rotationChangeHandle(string _,JSONNode jsNode_){
            r = jsNode_.AsFloat;
        }
        private void alphaPathChangeHandle(string _,JSONNode jsNode_){
            a = jsNode_.AsFloat;
        }
        private void widthPathChangeHandle(string _,JSONNode jsNode_){
            w = jsNode_.AsFloat;
        }
        private void heightPathChangeHandle(string _,JSONNode jsNode_){
            h = jsNode_.AsFloat;
        }
    }
}