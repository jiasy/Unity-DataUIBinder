using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
namespace DataUIBinder{
    /*
        列表包装器
            列表元素宽高应当一致
    */
    public class ListWrapper : ComponentWrapper,IUpdateAble{
        //列表种类，横向还是纵向
        public enum ListType{
            None = 0,
            X = 1,
            Y = 2
        }
        private RectTransform _rectTrans = null;
        public RectTransform rectTrans{
            get{
                if(_rectTrans == null){
                    _rectTrans = transform as RectTransform;
                }
                return _rectTrans;
            }
        }
        private UIItem _cloneItem;
        public UIItem cloneItem{
            get{
                if(_cloneItem){
                    return _cloneItem;
                }
                Transform _uiItemTrans = rectTrans.Find("item");
#if UNITY_EDITOR
                if(_uiItemTrans == null){
                    throw new Exception("ERROR : list_xx 必须有个名为 \"item\" 子节点.");
                }
#endif
                _cloneItem = _uiItemTrans.GetComponent<UIItem>();
#if UNITY_EDITOR
                if(_cloneItem == null){
                    throw new Exception("ERROR : \"item\" 子节点必须挂载 UIItem 继承类的组件");
                }
                // SAMPLE - UGUI 锚点获取
                if(!(_uiItemTrans as RectTransform).pivot.x.approximateTo(0)||!(_uiItemTrans as RectTransform).pivot.y.approximateTo(1)){
                    throw new Exception("ERROR : \"item\" 锚点必须是 (0,1).");
                }
#endif
                _cloneItem.isUseForCopy = true;
                _cloneItem.gameObject.SetActive(false);
                RectTransform _cloneItemRectTrans = _cloneItem.transform as RectTransform;
                width = _cloneItemRectTrans.getWidth();
                height = _cloneItemRectTrans.getHeight();
                return _cloneItem;
            }
        }
        private List<UIItem> itemPool = new List<UIItem>();
        private List<UIItem> activeItemList = new List<UIItem>();
        private int itemInstanceID = 0;
        private ListType type = ListType.None;
        private bool isChanged = false;
        private bool isShowRangeChanged = false;
        private float _beginPos = 0;
        private float _endPos = 0;
        private int begin = -1;
        private int end = -1;
        private int itemNum = 0;
        public float beginPos{
            get{
                return _beginPos;
            }
            set{
                float _targetValue = value;
                if(_targetValue < 0){
                    _targetValue = 0;
                }
                if(!_beginPos.approximateTo(_targetValue)){
                    isChanged = true;
                    _beginPos = _targetValue;
                }
            }
        }
        public float endPos{
            get{
                return _endPos;
            }
            set{
                float _targetValue = value;
                if(_targetValue < 0){
                    _targetValue = 0;
                }
                if(!_endPos.approximateTo(_targetValue)){
                    isChanged = true;
                    _endPos = _targetValue;
                }
            }
        }
        private float width;
        private float height;
        [SerializeField]
        private float itemRange{
            get{
                if(type == ListType.X){
                    return width;
                }else if(type == ListType.Y){
                    return height;
                }else{
                    throw new Exception("ERROR : 在取itemRange之前，要先确定类型。");
                }
            }       
        }
        private RectTransform parentLayoutGroupRectTrans = null;
        private bool parentLayoutGroupSearched = false;
        private Func<JSONNode,bool> filterFunc = null;
        private Func<JSONNode,JSONNode,int> sortFunc = null;
        private List<int> filterAndSortIdxList = null;
        private ScrollRect scrollRect = null;
        protected override void Awake(){
            base.Awake();
#if UNITY_EDITOR
            if(!rectTrans.pivot.x.approximateTo(0)||!rectTrans.pivot.y.approximateTo(1)){
                throw new Exception("ERROR : 作为列表锚点必须是 (0,1).");
            }
#endif
        }
        // 判断列表是否处于显示状态
        public void checkActive(string listDataPath_){
            JSONNode _jsNode = DataCenter.getValue(listDataPath_);
            if(_jsNode == null){
                rectTrans.gameObject.SetActive(false);
            }else{
                JSONArray _jsArray = _jsNode as JSONArray;
                if(_jsArray == null){
                    rectTrans.gameObject.SetActive(false);
                }else{
                    if(_jsArray.Count == 0){
                        rectTrans.gameObject.SetActive(false);
                    }else{
                        rectTrans.gameObject.SetActive(true);
                    }
                }
            }
        }
        public void reset(UINode uiNode_,string listDataPath_,int itemNum_ = 1,ListType type_ = ListType.X,ScrollRect scrollRect_ = null){
            if (itemNum_ <= 0){
                return;
            }
            if(uiNode_ == null){
              throw new Exception("ERROR : 必须有节点");
            }
            if(uiNode != null && uiNode != uiNode_){
                throw new Exception("ERROR : 显示节点在运行时不能变更。");
            }
            if(pattern != listDataPath_){
                pattern = listDataPath_;
                checkActive(pattern);
            }
            if(itemNum != itemNum_){
                itemNum = itemNum_;
            }
            if(type != type_){
                if(type != ListType.None){
                    throw new Exception("ERROR : 运行中不能改变横纵方向");
                }
                type = type_;
            }
            scrollRect = scrollRect_;
            allBackToPool();
            isChanged = true;
            isShowRangeChanged = true;
            base.reset(pattern,uiNode_);
            filterAndSortList();
            ComponentWrapper.addUpdateAble(this);
        }
        public void filterAndSortList(){
            if(pattern == null){
                return;
            }
            filterAndSortIdxList = DataCenter.getFilterSortIdxList(pattern,filterFunc,sortFunc);
        }
        public void resetSortAndFilter(Func<JSONNode,bool> filterFunc_ = null,Func<JSONNode,JSONNode,int> sortFunc_ = null){
            if(filterFunc != filterFunc_ || sortFunc != sortFunc_){
                filterFunc = filterFunc_;
                sortFunc = sortFunc_;
                filterAndSortList();
            }
        }
        public void resetSortFunc(Func<JSONNode,JSONNode,int> sortFunc_ = null){
            if(sortFunc != sortFunc_){
                sortFunc = sortFunc_;
                filterAndSortList();
            }
        }
        public void resetFilterFunc(Func<JSONNode,bool> filterFunc_ = null){
            if(filterFunc != filterFunc_){
                filterFunc = filterFunc_;
                filterAndSortList();
            }
        }
        public override void dataChangeHandle(string dataPath_,JSONNode jsNode_){
            if(jsNode_ != null){
                checkActive(dataPath_);
                allBackToPool();
                isChanged = true;
                isShowRangeChanged = true;
                changeRange(jsNode_["length"].AsInt);
                filterAndSortList();
            }
        }
        public void changeRange(int listLength_){
            int _lineNum = (int)Mathf.Floor(listLength_ / itemNum);
            if(listLength_ % itemNum != 0){
                _lineNum ++;
            }
            float _targetRange = _lineNum * itemRange;
            float _currentRange;
            if(type == ListType.X){
                _currentRange = rectTrans.getWidth();
                if(!_currentRange.approximateTo(_targetRange)){
                    rectTrans.setWidth(_targetRange);
                }
            }else if(type == ListType.Y){
                _currentRange = rectTrans.getHeight();
                if(!_currentRange.approximateTo(_targetRange)){
                    rectTrans.setHeight(_targetRange);
                }
            }
            if(parentLayoutGroupSearched == false){
                HorizontalOrVerticalLayoutGroup _layoutGroup = rectTrans.parent.GetComponent<HorizontalOrVerticalLayoutGroup>();
                if(_layoutGroup != null){
                    if(type == ListType.X){
                        _layoutGroup.childForceExpandWidth = true;
                    }else if(type == ListType.Y){
                        _layoutGroup.childForceExpandHeight = true;
                    }
                    parentLayoutGroupRectTrans = _layoutGroup.transform as RectTransform;
                }
                parentLayoutGroupSearched = true;    
            }
            if(parentLayoutGroupRectTrans != null){
                float _layoutRange = 0;
                int _idx = parentLayoutGroupRectTrans.childCount - 1;
                if(_idx >= 0){
                    while(true){
                        Transform _trans = parentLayoutGroupRectTrans.GetChild(_idx);
                        if(_trans.gameObject.activeSelf){
                            if(type == ListType.X){
                                _layoutRange += (_trans as RectTransform).getWidth();
                            }else if(type == ListType.Y){
                                _layoutRange += (_trans as RectTransform).getHeight();
                            }
                        }
                        _idx -- ;
                        if(_idx < 0){
                            break;
                        }
                    }
                }
                if(type == ListType.X){
                    parentLayoutGroupRectTrans.setWidth(_layoutRange);
                }else if(type == ListType.Y){
                    parentLayoutGroupRectTrans.setHeight(_layoutRange);
                }
                LayoutRebuilder.ForceRebuildLayoutImmediate(parentLayoutGroupRectTrans);
            }
        }
#if UNITY_EDITOR
        public void Update(){
            if (uiNode == null){
                throw new Exception("ERROR : "+DisplayUtils.getDisplayPath(transform)+"\n如果没有被 ScrollWrapper 包裹，那么需要在所在 UINode 的 Start 中手动初始化\n如果被ScrollWrapper包裹，那么需要在所在 UINode 的 Start 中手动初始化 ScrollWrapper.");
            }
        }
#endif
        public void frameUpdate(){
            if(begin == end){//只显示一行的时候，强制判断一下
                isChanged = true;
            }
            if(isChanged){//第一层，判断是否初始化，值是否可能是未变动
                isChanged = false;
                int _lastBegin = -99999;
                int _lastEnd = -99999;
                if(!Recoder.isReplay){
                    _lastBegin = begin;
                    _lastEnd = end;
                    begin = (int)Mathf.Floor(_beginPos/itemRange);
                    end = (int)Mathf.Floor(_endPos/itemRange);
                }
                if(!isShowRangeChanged && (begin != _lastBegin || end != _lastEnd)){
                    isShowRangeChanged = true;
                }
                if(isShowRangeChanged){//第二层，实际值是否变更的判断
                    isShowRangeChanged = false;
                    if(end < begin){
                        throw new Exception("ERROR : begin end error.");
                    }
                    int _listLength = filterAndSortIdxList.Count;
                    changeRange(_listLength);
                    int _beginIdx = begin * itemNum;
                    if (_beginIdx < 0){ _beginIdx = 0; }
                    int _endIdx = (end + 1)* itemNum;
                    int[] _targetIdxArr = new int[_endIdx - _beginIdx];
                    for (int _idx = _beginIdx; _idx < _endIdx; _idx++) {
                        if(_idx >= _listLength){
                            _targetIdxArr[_idx-_beginIdx] = -1;
                        }else{
                            _targetIdxArr[_idx-_beginIdx] = _idx;
                        }
                    }
                    List<int> _targetIdxList = new List<int>(_targetIdxArr);
                    List<int> _removeOverFlow = new List<int>();
                    int[] _currentIdxArr = new int[activeItemList.Count];
                    for (int _idx = 0; _idx < activeItemList.Count; _idx++) {
                        int _activeIdx = activeItemList[_idx].itemIdx;
                        if(_activeIdx >= _listLength){
                            _removeOverFlow.Add(_activeIdx);
                        }else{
                            _currentIdxArr[_idx] = _activeIdx;
                        }
                    }
                    backToPoolByIdxList(_removeOverFlow);
                    List<int> _currentIdxList = new List<int>(_currentIdxArr);
                    List<int> _addIdxList = _targetIdxList.Except(_currentIdxList).ToList();
                    for (int _idx = 0; _idx < _addIdxList.Count; _idx++) {
                        int _itemIdx = _addIdxList[_idx];
                        if(_itemIdx != -1){
                            getItem(_itemIdx,pattern,filterAndSortIdxList[_itemIdx],width,height);   
                        }
                    }
                    List<int> _removeIdxList = _currentIdxList.Except(_targetIdxList).ToList();
                    backToPoolByIdxList(_removeIdxList);
                    if(!Recoder.isReplay){
                        if(Recoder.isRecord){
                            JSONObject _jsObject = new JSONObject();
                            _jsObject["type"] = "onScroll";
                            _jsObject["uiPath"] = uiNode.uiPath;
                            _jsObject["name"] = gameObject.name;
                            _jsObject["begin"] = begin;
                            _jsObject["end"] = end;
                            Recoder.record(_jsObject);
                        }
#if UNITY_EDITOR
                        LogToFiles.logByType(LogToFiles.LogType.Record, "[ onScroll ] " + uiNode.uiPath + " -> "+gameObject.name + " : " + begin.ToString() +" - " + end.ToString());
#endif
                    }
                }
            }
        }
        private void allBackToPool(bool isDestroyed_ = false){
            while(activeItemList.Count > 0){
                backToPool(activeItemList[activeItemList.Count - 1],isDestroyed_);
            }
            activeItemList.Clear();
            if(type == ListType.X){
                rectTrans.setWidth(0);
            }else if(type == ListType.Y){
                rectTrans.setHeight(0);
            }
        }
        private void backToPoolByIdxList(List<int> idxList_){
            UIItem _uiItem = null;
            for (int _idx = 0; _idx < idxList_.Count; _idx++) {
                for (int _itemIdx = 0; _itemIdx < activeItemList.Count; _itemIdx++) {
                    _uiItem = activeItemList[_itemIdx];
                    if(_uiItem.itemIdx == idxList_[_idx]){
                        backToPool(_uiItem);
                        break;
                    }
                }
            }
        }
        protected override void OnDestroy(){
            if(isDestroyed){ return; }
            allBackToPool(true);
            activeItemList = null;
            for (var _idx = 0; _idx < itemPool.Count; _idx++){
                Destroy(itemPool[_idx].gameObject);
            }
            itemPool.Clear();
            itemPool = null;
            Destroy(cloneItem);
            _cloneItem = null;
            _rectTrans = null;
            parentLayoutGroupRectTrans = null;
            parentLayoutGroupSearched = false;
            filterFunc = null;
            sortFunc = null;
            base.OnDestroy();
            ComponentWrapper.removeUpdateAble(this);
        }
        public UIItem getItem(int itemIdx_,string listDataPath_,int dataIdx_,float width_,float height_){
            UIItem _uiItem = getFromPool();
#if UNITY_EDITOR
            _uiItem.gameObject.name = _uiItem.gameObject.name.splitWith("<")[0] + "<" + itemIdx_.ToString() + ">[" + dataIdx_.ToString() + "]";
#endif
            int _lineIdx = (int)Mathf.Floor( (float)itemIdx_ / itemNum );
            int _colIdx = itemIdx_ % itemNum;
            float _x;
            float _y;
            if(type == ListWrapper.ListType.X){
                _x = _lineIdx * width_;
                _y = -_colIdx * height_;
            }else if(type == ListWrapper.ListType.Y){
                _x = _colIdx * width_;
                _y = -_lineIdx * height_;
            }else{
                throw new Exception("ERROR : 所在列表未初始化");
            }
            Transform _uiItemTrans = _uiItem.transform;
            _uiItemTrans.SetParent(rectTrans);
            _uiItemTrans.setX( _x );
            _uiItemTrans.setY( _y );
            activeItemList.Add(_uiItem);
            _uiItem.reUse(itemIdx_,listDataPath_,dataIdx_,scrollRect);
            return _uiItem;
        }
        private UIItem getFromPool(){
            int _length = itemPool.Count;
            UIItem _uiItem = itemPool.pull();
            if(_uiItem != null){
                return _uiItem;
            }
            GameObject _cloneGameObject = Instantiate(cloneItem.gameObject);
            _uiItem = _cloneGameObject.GetComponent<UIItem>();
            //// - 拷贝出来的对象，属性也复制出来了。要重置
            _uiItem.isUseForCopy = false;
            _cloneGameObject.transform.initPosAndScale();
            StringBuilder _sb = CSharpExtensionUtils.SBInstance;
            _sb.Clear();
            _sb.Append(uiNode.uiPath);
            _sb.Append('.');
            _sb.Append(gameObject.name);
            _sb.Append('.');
            _sb.Append((++itemInstanceID).ToString());
#if UNITY_EDITOR
            _cloneGameObject.name = "item_"+(itemInstanceID).ToString()+"<";
#endif
            _uiItem.uiPath = _sb.ToString();
            _sb.Clear();
            _uiItem.parentUINode = uiNode;
            return _uiItem;
        }
        private void backToPool(UIItem uiItem_,bool isDestroyed_=false){
            itemPool.Add(uiItem_);
            uiItem_.unUse(isDestroyed_);
            int _idx = activeItemList.IndexOf(uiItem_);
            if(_idx < 0){
                throw new Exception("ERROR : 池对象关联错误");
            }else{
                activeItemList.RemoveAt(_idx);
            }
        }
    }
}
