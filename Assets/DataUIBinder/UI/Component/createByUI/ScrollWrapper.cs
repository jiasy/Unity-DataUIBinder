using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
namespace DataUIBinder{
    /*
        滚动列表封装
            持有一个列表
            列表的上下(前后)可以添加其他的非列表元素
    */
    public class ScrollWrapper : ComponentWrapper,IUpdateAble{
        ListWrapper listWrapper;
        int itemNum = 0;
        ListWrapper.ListType listType = ListWrapper.ListType.None;
        Vector3 downRight;
        private RectTransform _rectTrans = null;
        public RectTransform rectTrans{
            get{
                if(_rectTrans == null){
                    _rectTrans = transform as RectTransform;
                }
                return _rectTrans;
            }
        }
        protected override void Awake(){
            base.Awake();
#if UNITY_EDITOR
            if(!rectTrans.pivot.x.approximateTo(0)||!rectTrans.pivot.y.approximateTo(1)){
                throw new Exception("ERROR : 作为滚动层锚点必须是 (0,1).");
            }
#endif
        }
        bool initListWrapper(){
            if (listWrapper){
                return false;
            }
            ScrollRect _scrollRect = GetComponent<ScrollRect>();
            listWrapper = GetComponentInChildren<ListWrapper>();
#if UNITY_EDITOR
            if (listWrapper == null){
                throw new Exception("ERROR : there is no ListWrapper in children.");
            }
#endif
            UIItem _cloneItem = listWrapper.cloneItem;
            float _scrollRange = 0;
            float _itemRange = 0;
#if UNITY_EDITOR
            if(_scrollRect.horizontal && _scrollRect.vertical){
                throw new Exception("ERROR : cann't both sides.");
            }
            if(!_scrollRect.horizontal && !_scrollRect.vertical){
                throw new Exception("ERROR : must set side.");
            }
#endif
            downRight = new Vector3( rectTrans.getWidth() , -rectTrans.getHeight() , 0 );

            if(_scrollRect.horizontal){
                _itemRange = ( _cloneItem.transform as RectTransform ).getHeight();
                _scrollRange = downRight.y;
                listType = ListWrapper.ListType.X;
            }
            if(_scrollRect.vertical){
                _itemRange = ( _cloneItem.transform as RectTransform ).getWidth();
                _scrollRange = downRight.x;
                listType = ListWrapper.ListType.Y;
            }

            itemNum = (int)Mathf.Floor( _scrollRange / _itemRange );
            return true;
        }
#if UNITY_EDITOR
        public void Update(){
            if (uiNode == null){
                throw new Exception("ERROR : ScrollWrapper 需要在所在 UINode 的 Start 中手动初始化 " + DisplayUtils.getDisplayPath(transform));
            }
        }
#endif
        public void frameUpdate(float dt_){
            if( listType == ListWrapper.ListType.None){
                return;
            }
            Vector3 _topLeft = listWrapper.rectTrans.InverseTransformPoint( rectTrans.TransformPoint( Vector3.zero ) );
            Vector3 _downRight = listWrapper.rectTrans.InverseTransformPoint( rectTrans.TransformPoint( downRight ) );
            if( listType == ListWrapper.ListType.X ){
                listWrapper.beginPos = _topLeft.x;
                listWrapper.endPos = _downRight.x;
            }else if( listType == ListWrapper.ListType.Y ){
                listWrapper.beginPos = -_topLeft.y;
                listWrapper.endPos = -_downRight.y;
            }
        }
        protected override void OnDestroy(){
            if(isDestroyed){ return; }
            listWrapper = null;
            _rectTrans = null;
            base.OnDestroy();
            ComponentWrapper.removeUpdateAble(this);
        }
        public override void reset(string pattern_,UINode uiNode_ = null){
            pattern = pattern_;
            dataPath = null;
            if(uiNode_ == null){
              throw new Exception("ERROR : 必须有节点");
            }
            if(uiNode != null && uiNode != uiNode_){
                throw new Exception("ERROR : 显示节点在运行时不能变更。");
            }
            bool _isFirstTime = initListWrapper();
            base.reset(pattern,uiNode_);
            if (!_isFirstTime){//因为第一次会触发 dataChangeHandle，免得做两次列表重置。
                listWrapper.reset(uiNode,dataPath,itemNum,listType,GetComponent<ScrollRect>());
            }
            ComponentWrapper.addUpdateAble(this);
        }
        public override void dataChangeHandle(string dataPath_,JSONNode jsNode_){
            if(listType == ListWrapper.ListType.None){
                throw new Exception("ERROR : 没有初始化");
            }
            listWrapper.reset(uiNode,dataPath_,itemNum,listType,GetComponent<ScrollRect>());
        }
    }
}