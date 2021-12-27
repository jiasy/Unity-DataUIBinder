using SimpleJSON;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace DataUIBinder{
    public class UIItem : UINode{
        //// - 列表元素有独立的帧循环队列，在 reUse/unUse 方法中进行增删。
        public static List<UIItem> uiItemList = new List<UIItem>();
        public static void doFrameUpdate(){
            List<UIItem> _uiItemList = UIItem.uiItemList;
			int _listLength = _uiItemList.Count;
            for (int _idx = 0; _idx < _listLength; _idx++) {
	            _uiItemList[_idx].frameUpdate();
            }
        }
        public static void addUpdate(UIItem uiItem_){
#if UNITY_EDITOR
            if (uiItemList.IndexOf(uiItem_) != -1){
                throw new Exception("ERROR : " + uiItem_.uiName + " 更新添加重复");
            }
#endif
            uiItemList.Add(uiItem_);
        }
        public static void removeUpdate(UIItem uiItem_){
#if UNITY_EDITOR
            if (uiItemList.IndexOf(uiItem_) == -1){
                throw new Exception("ERROR : " + uiItem_.uiName + " 更新移除失败");
            }
#endif
            uiItemList.Remove(uiItem_);
        }
        public int itemIdx;
        public int dataIdx;
        public string listDataPath;
        protected List<ReUseObj> dynamicListenerList = new List<ReUseObj>();
        protected override void Awake() {
            base.Awake();
#if UNITY_EDITOR
            if(!rectTrans.pivot.x.approximateTo(0)||!rectTrans.pivot.y.approximateTo(1)){
                throw new Exception("ERROR : item's pivot must be (0,1).");
            }
#endif
        }
        public virtual void reUse(int itemIdx_,string listDataPath_,int dataIdx_,ScrollRect scrollRect_ = null){
            itemIdx = itemIdx_;
            dataIdx = dataIdx_;
            listDataPath = listDataPath_;
            StringBuilder _sb = CSharpExtensionUtils.SBInstance;
            _sb.Clear();
            _sb.Append(listDataPath_);
            _sb.Append(".[");
            _sb.Append(dataIdx.ToString());
            _sb.Append(']');
            this.dtPath = _sb.ToString();
            _sb.Clear();
            this.resetDataUIBind();
            gameObject.SetActive(true);
            resetBtnScrollRect(scrollRect_);
            addUpdate(this);
        }
        public virtual void unUse(bool isDestroyed_ = false){
            clearAllDynamicListeners();
            gameObject.SetActive(false);
            this.dtPath = null;
            itemIdx = -1;
            dataIdx = -1;
            resetBtnScrollRect(null);
            removeUpdate(this);
            if (isDestroyed_){
                OnDestroy();
            }
        }
        public override void OnDestroy(){
            if (isDestroyed){ return; }
            clearAllDynamicListeners();
            dynamicListenerList = null;
            listDataPath = null;
            base.OnDestroy();
        }
        private void resetBtnScrollRect(ScrollRect scrollRect_){
            ButtonWrapper[] _btnWrappers = GetComponentsInChildren<ButtonWrapper>();
            for (int _idx = 0; _idx < _btnWrappers.Length; _idx++) {
                ButtonWrapper _btnWrapper = _btnWrappers[_idx];
                _btnWrapper.resetScrollRect(scrollRect_);
            }
        }
        //清理当前显示节点所关联的数据路径的监听器
        private void clearAllDynamicListeners(){
            for (int _idx = 0; _idx < dynamicListenerList.Count; _idx++) {
                removeListener(dynamicListenerList[_idx]);
            }
            dynamicListenerList.Clear();
        }
        //
        public override ReUseObj onChange(string dataPath_,Action<JSONNode> pathChangeHandle_){
            ReUseObj _dataPathListener = base.onChange(dataPath_,
                (jsNode_)=>{
                    if(dataPath_.isStartsWith(uiPath + ".") || dataPath_.isStartsWith(dtPath + ".") ){
                        if(dc.gv(dataPath_)==null){
                            return;
                        }
                    }
                    pathChangeHandle_(jsNode_);
                }
            );
            dynamicListenerList.Add(_dataPathListener);
            return _dataPathListener;
        }
        public override ReUseObj onChange(string[] dataPathList_,Action<List<string>> pathChangeHandle_){
            ReUseObj _dataPathListListener = base.onChange(dataPathList_,
            (changePathList_)=>{
                //// - 作为 UIItem 监听路径变化时，路径包括自己的路径时，要判断一下。只有全部都满足的情况才会触发。
                string _uiPathWithDot = uiPath + ".";
                string _dataPathWithDot = dtPath + ".";
                for (int _idx = 0; _idx < dataPathList_.Length; _idx++) {
                    var _dataPath = dataPathList_[_idx];
                    if(_dataPath.isStartsWith(_uiPathWithDot) || _dataPath.isStartsWith(_dataPathWithDot) ){
                        if(dc.gv(_dataPath)==null){
                            return;
                        }
                    }
                }
                pathChangeHandle_(changePathList_);
            });
            dynamicListenerList.Add(_dataPathListListener);
            return _dataPathListListener;
        }
        public override void onBtn(string btnName_){
            base.onBtn(btnName_);
        }
        public override void onPress(string btnName_){
            base.onPress(btnName_);
        }
        public override void onDoubleClick(string btnName_){
            base.onDoubleClick(btnName_);
        }
        public override void onCheck(string btnName_,string key_,bool isOn_){
            base.onCheck(btnName_,key_,isOn_);
        }
        public override void onToggle(string btnName_,string key_,string value_){
            base.onToggle(btnName_,key_,value_);
        }
    }
}
