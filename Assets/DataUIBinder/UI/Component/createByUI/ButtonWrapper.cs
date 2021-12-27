using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SimpleJSON;
namespace DataUIBinder{
    //按钮的基类
    public class ButtonWrapper : ComponentWrapper,IUpdateAble, IBeginDragHandler, IEndDragHandler {    
        public static bool isAllBtnBlock = false;//锁住所有 ButtonWrapper
        public static void lockAllClick(){
            isAllBtnBlock = true;
        }
        public static void unlockAllClick(){
            isAllBtnBlock = false;
        }
        private ScrollRect scrollRect;
        private Button _btn = null;
        public Button btn{
            get{
                if(_btn == null){
                    _btn = gameObject.GetComponent<Button>();
                }
                return _btn;
            }
        }
        protected bool justClicked = false;//刚点击过
        public int justClickedFrameCount = 0;//刚刚点过的帧计数
        private bool isDrag = false;
        protected override void Awake(){
            base.Awake();
            isOnlyBindToUI = true;
            _btn = GetComponent<Button>();
            btn.onClick.AddListener(delegate() { 
                onBtn();
            });
        }
        public virtual void frameUpdate(){
            if(justClicked){
                justClickedFrameCount++;
                if(justClickedFrameCount > 10){
                    justClickedFrameCount = 0;
                    justClicked = false;
                }
            }
        }

        public void reset(UINode uiNode_ = null){
#if UNITY_EDITOR
            if(debugRecord && isInit == false){
                isInit = true;
                dc.sv("debug.objectCount.wrapper",dc.gv("debug.objectCount.wrapper").AsInt + 1);
            }
#endif
            dataPath = null;
            if(uiNode_ == null){
                throw new Exception("ERROR : 必须有节点");
            }
            resetUINode(uiNode_);
            justClicked = false;
            justClickedFrameCount = 0;
            ComponentWrapper.addUpdateAble(this);
        }
        public override void reset(string pattern_,UINode uiNode_ = null){
            pattern = pattern_;
            dataPath = null;
            if(uiNode_ == null){
                throw new Exception("ERROR : 必须有节点");
            }
            base.reset(pattern,uiNode_);
            justClicked = false;
            justClickedFrameCount = 0;
            ComponentWrapper.addUpdateAble(this);
        }
        public void resetScrollRect(ScrollRect scrollRect_){
            scrollRect = scrollRect_;
            isDrag = false;
        }
        protected bool canClick(){
            if(isAllBtnBlock){
#if UNITY_EDITOR
                UnityEngine.Debug.LogWarning(gameObject.name + ",所有按钮锁定中");
#endif
                return false;
            }
            if(justClicked){
#if UNITY_EDITOR

                UnityEngine.Debug.LogWarning(gameObject.name + "，屏蔽连续按下");
#endif
                return false;
            }
            if(isDrag){
                return false;
            }
            if(uiNode == null){
                throw new Exception("ERROR : " + DisplayUtils.getDisplayPath(transform) + " 没有关联 uiNode");
            }
            return true;
        }
        protected virtual void onBtn(){
            if(canClick()){
                uiNode.onBtn(gameObject.name);
                justClicked = true;
            }
        }
        protected virtual void OnDestroy(){
            if(isDestroyed){ return; }
            if(_btn != null){
                btn.onClick.RemoveAllListeners();
                _btn = null;
            }
            scrollRect = null;
            base.OnDestroy();
            ComponentWrapper.removeUpdateAble(this);
        }
        // begin dragging
        public void OnBeginDrag(PointerEventData evtData_) {
            if(scrollRect != null){
                scrollRect.OnBeginDrag(evtData_);
                isDrag = true;
            }
        }
        // end dragging
        public void OnEndDrag(PointerEventData evtData_) {
            if(scrollRect != null){
                scrollRect.OnEndDrag(evtData_);
                isDrag = false;
            }
        }
    }
}
