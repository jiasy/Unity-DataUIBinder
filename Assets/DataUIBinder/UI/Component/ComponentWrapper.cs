using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SimpleJSON;
using UnityEngine;
using UnityEngine.UI;
namespace DataUIBinder {
    /*
        组件封装
    */
    public class ComponentWrapper:MonoBehaviour{
        public static bool debugRecord = false;
        //所挂载的可更新对象列表
        private static List<IUpdateAble> updateList = new List<IUpdateAble>();
        public static void addUpdateAble(IUpdateAble updateAble_){
            int _idx = updateList.IndexOf(updateAble_);
            if(_idx < 0){
                updateList.Add(updateAble_);
            }
        }
        public static void removeUpdateAble(IUpdateAble updateAble_){
            int _idx = updateList.IndexOf(updateAble_);
            if(_idx >= 0){
                updateList.RemoveAt(_idx);
            }else{
                throw new Exception("ERROR : 重复移除");
            }
        }
        public static void doFrameUpdate(float dt_){
            int _updateListLength = updateList.Count;
            for (int _idx = 0; _idx < _updateListLength; _idx++) {
                updateList[_idx].frameUpdate(dt_);
            }
        }
        public static Dictionary<UINode, List<ComponentWrapper>> uiNodeWrapperDict = new Dictionary<UINode, List<ComponentWrapper>> ();
        public static List<ComponentWrapper> tryAddWrapperToChildren (Transform rootTrans_, UINode uiNode_ = null) {
            List<ComponentWrapper> _backComponentWrapperList = new List<ComponentWrapper> ();
            DisplayUtils.recursiveChildren (rootTrans_, (transChild_) => {
                GameObject _go = transChild_.gameObject;
                if (_go.name.isStartsWith ("list_")) { //list_ 开头的列表，其下节点又list_做管理
                    return false; //stop
                }
                Property2DWrapper _property2DWrapper = ComponentWrapper.tryAddProperty2DWrapper (_go, uiNode_);
                if (_property2DWrapper == null) {
                    ComponentWrapper _wrapper = ComponentWrapper.tryAddWrapperNameAsPath (_go, uiNode_);
                    if (_wrapper != null) {
                        _backComponentWrapperList.Add (_wrapper);
                    }
                }
                MultiPathTextWrapper _multiPathTextWrapper = ComponentWrapper.tryAddMultiPathTextWrapper (_go, uiNode_);
                if (_multiPathTextWrapper != null) {
                    _backComponentWrapperList.Add (_multiPathTextWrapper);
                }
                return true; //continue
            });
            return _backComponentWrapperList;
        }
        public static Property2DWrapper tryAddProperty2DWrapper (GameObject go_, UINode uiNode_ = null) {
            Property2DWrapper _backWrapper = null;
            string _goName = go_.name;
            if (_goName.isPropertyBindStyle ()) {
                _backWrapper = go_.AddComponent<Property2DWrapper> ();
                _backWrapper.reset (_goName, uiNode_);
            }
            return _backWrapper;
        }
        public static MultiPathTextWrapper tryAddMultiPathTextWrapper (GameObject go_, UINode uiNode_ = null) {
            MultiPathTextWrapper _backWrapper = null;
            Text _txt = go_.GetComponent<Text> ();
            if (_txt != null && _txt.text.isPathAndStrMixedStyle ()) {
                _backWrapper = go_.AddComponent<MultiPathTextWrapper> ();
                _backWrapper.reset (_txt.text, uiNode_);
            }
            return _backWrapper;
        }
        public static ComponentWrapper tryAddWrapperNameAsPath (GameObject go_, UINode uiNode_ = null) {
            string _goName = go_.name;
            ComponentWrapper _backWrapper = null;
            if (_goName.IndexOf ('.') > 0) {
#if UNITY_EDITOR
                float _float;
                if (float.TryParse (_goName, out _float)) {
                    throw new Exception ("ERROR : 节点名不能是纯数字");
                }
#endif
                if (_goName.isCompareStyle ()) {
                    _backWrapper = go_.AddComponent<CompareActiveWrapper> ();
                    _backWrapper.reset (_goName, uiNode_);
                } else if (_goName.isRangeCompareStyle ()) {
                    if (go_.GetComponent<Slider> ()) {
                        _backWrapper = go_.AddComponent<SliderWrapper> ();
                        _backWrapper.reset (_goName, uiNode_);
                    } else {
                        _backWrapper = go_.AddComponent<RangeCompareActiveWrapper> ();
                        _backWrapper.reset (_goName, uiNode_);
                    }
                } else {
                    if (go_.GetComponent<Text> ()) {
                        _backWrapper = go_.AddComponent<TextWrapper> ();
                        _backWrapper.reset (_goName, uiNode_);
                    } else if (go_.GetComponent<InputField> ()) {
                        _backWrapper = go_.AddComponent<InputFieldWrapper> ();
                        _backWrapper.reset (_goName, uiNode_);
                    } else if (go_.GetComponent<Image> ()) {
                        _backWrapper = go_.AddComponent<ImageWrapper> ();
                        _backWrapper.reset (_goName, uiNode_);
                    } else {
                        throw new Exception ("ERROR : " + _goName + " 应用于不支持的组件。");
                    }

                }
            }
            return _backWrapper;
        }
        public static void cacheChild (UINode uiNode_, string goName_, Transform goTrans_) {
            if (uiNode_[goName_] != null) {
#if UNITY_EDITOR
                Debug.LogWarning ("ERROR " + System.Reflection.MethodBase.GetCurrentMethod ().ReflectedType.FullName + " -> " + new System.Diagnostics.StackTrace ().GetFrame (0).GetMethod ().Name + " : " +
                    goName_ + " 名称重复。\n    " + DisplayUtils.getDisplayPath(goTrans_) +" - "+DisplayUtils.getDisplayPath(uiNode_[goName_])
                );
#endif
                return;
            }
            uiNode_[goName_] = goTrans_;
        }
        public static void cacheTransAndWrapSpecialName (UINode uiNode_) {
            DisplayUtils.recursiveChildren (uiNode_.transform, (transChild_) => {
                GameObject _go = transChild_.gameObject;
                StringBuilder _sb = new StringBuilder ();
                string _goName = _go.name;
                if (_goName.isStartsWith ("list_")) {
                    _go.AddComponent<ListWrapper> ();
                    //reset 稍后，在所在的UINode的Start中进行
                    cacheChild (uiNode_, _goName, transChild_);
                    return false; //stop
                } else if (_goName.isStartsWith ("trans_")) {
                    cacheChild (uiNode_, _goName, transChild_);
                } else if (_goName.isStartsWith ("img_")) {
#if UNITY_EDITOR
                    if (_go.GetComponent<Image> () == null) {
                        throw new Exception ("ERROR : " + _goName + " img_<name>，没有 Image 组件。");
                    }
                    if (_goName.splitWith ("_").Length != 2) {
                        throw new Exception ("ERROR : " + _goName + " 应当按照 img_<name> 的方式命名");
                    }
#endif
                    cacheChild (uiNode_, _goName, transChild_);
                } else if (_goName.isStartsWith ("txt_")) {
#if UNITY_EDITOR
                    if (_go.GetComponent<Text> () == null) {
                        throw new Exception ("ERROR : " + _goName + " txt_<name>，没有 Text 组件。");
                    }
                    if (_goName.splitWith ("_").Length != 2) {
                        throw new Exception ("ERROR : " + _goName + " 应当按照 txt_<name> 的方式命名");
                    }
#endif
                    cacheChild (uiNode_, _goName, transChild_);
                } else if (_goName.isStartsWith ("input_")) {
#if UNITY_EDITOR
                    if (_go.GetComponent<InputField> () == null) {
                        throw new Exception ("ERROR : " + _goName + " input_<name>，没有 InputField 组件。");
                    }
                    if (_goName.splitWith ("_").Length != 2) {
                        throw new Exception ("ERROR : " + _goName + " 应当按照 input_<name> 的方式命名");
                    }
#endif
                    cacheChild (uiNode_, _goName, transChild_);
                } else if (_goName.isStartsWith ("slider_")) {
#if UNITY_EDITOR
                    if (_go.GetComponent<Slider> () == null) {
                        throw new Exception ("ERROR : " + _goName + " slider_<name>，没有 Slider 组件。");
                    }
                    if (_goName.splitWith ("_").Length != 2) {
                        throw new Exception ("ERROR : " + _goName + " 应当按照 slider_<name> 的方式命名");
                    }
#endif
                    cacheChild (uiNode_, _goName, transChild_);
                } else if (_goName.isStartsWith ("scroll_")) {
#if UNITY_EDITOR
                    if (_go.GetComponent<ScrollRect> () == null) {
                        throw new Exception ("ERROR : " + _goName + " scroll_<name>，没有 ScrollRect 组件。");
                    }
                    if (_goName.splitWith ("_").Length != 2) {
                        throw new Exception ("ERROR : " + _goName + " 应当按照 scroll_<name> 的方式命名");
                    }
#endif
                    //reset 稍后，在所在的UINode的Start中进行
                    _go.AddComponent<ScrollWrapper>();
                    cacheChild (uiNode_, _goName, transChild_);
                } else if (
                    _goName.isStartsWith ("btn_") ||
                    _goName.isStartsWith ("check_") ||
                    _goName.isStartsWith ("toggle_") ||
                    _goName.isStartsWith ("btnPlus_")
                ) {
                    if (_go.GetComponent<Button> () == null) {
                        throw new Exception ("ERROR : " + _goName + " 没有 Button 组件。");
                    }
                    if (_goName.isStartsWith ("btn_")) {
                        ButtonWrapper _buttonWrapper = _go.AddComponent<ButtonWrapper> ();
                        _buttonWrapper.reset(uiNode_);
                    } else if (_goName.isStartsWith ("check_")) {
                        ButtonCheckWrapper _buttonCheckWrapper = _go.AddComponent<ButtonCheckWrapper> ();
                        string[] _checkNameArray = _goName.splitWith ("_");
                        if (_checkNameArray.Length != 2) {
                            throw new Exception ("ERROR : " + _goName + " 必须是 check_<key> 的格式");
                        }
                        string _checkKey = _checkNameArray[1];
#if UNITY_EDITOR
                        UINode.isKeyAvailableOnUI (_checkKey);
#endif
                        _sb.Append (uiNode_.uiPath);
                        _sb.Append ('.');
                        _sb.Append (_checkKey);
                        _buttonCheckWrapper.reset (_sb.ToString (), _checkKey, uiNode_);
                        _sb.Clear ();
                    } else if (_goName.isStartsWith ("toggle_")) {
                        ButtonToggleWrapper _buttonToggleWrapper = _go.AddComponent<ButtonToggleWrapper> ();
                        string[] _toggleNameArray = _goName.splitWith ("_");
                        if (_toggleNameArray.Length != 3) {
                            throw new Exception ("ERROR : " + _goName + " 必须是 toggle_<key>_<value> 的格式");
                        }
                        string _toggleKey = _toggleNameArray[1];
#if UNITY_EDITOR
                        UINode.isKeyAvailableOnUI (_toggleKey);
#endif
                        string _toggleValue = _toggleNameArray[2];
                        _sb.Append (uiNode_.uiPath);
                        _sb.Append ('.');
                        _sb.Append (_toggleKey);
                        _buttonToggleWrapper.reset (_sb.ToString (), _toggleKey, _toggleValue, uiNode_);
                        _sb.Clear ();
                    } else if (_goName.isStartsWith ("btnPlus_")) {
                        ButtonPlusWrapper _btnPlusWrapper = _go.AddComponent<ButtonPlusWrapper> ();
                        _btnPlusWrapper.reset(uiNode_);
                    }
                    cacheChild (uiNode_, _goName, transChild_);
                }
                return true; //continue
            });
        }
        public static void resetWrapperCacheListByUINode (UINode uiNode_) {
            List<ComponentWrapper> _componentWrapperList = null;
            if (uiNodeWrapperDict.TryGetValue (uiNode_, out _componentWrapperList)) {
                for (int _idx = 0; _idx < _componentWrapperList.Count; _idx++) {
                    ComponentWrapper _componentWrapper = _componentWrapperList[_idx];
                    if (_componentWrapper.pattern == null) {
                        throw new Exception ("ERROR : pattern 为赋值.");
                    }
                    _componentWrapper.reset (_componentWrapper.pattern, uiNode_);
                }
            }
        }
        public static void clearWrapperCacheListWhenUINodeDestroy (UINode uiNode_) {
            List<ComponentWrapper> _componentWrapperList = null;
            if (uiNodeWrapperDict.TryGetValue (uiNode_, out _componentWrapperList)) {
                int _destroyLastIdx = _componentWrapperList.Count - 1;
                while(_destroyLastIdx >= 0){
                    _componentWrapperList[_destroyLastIdx].OnDestroy();
                    _destroyLastIdx --;
                }
                _componentWrapperList.Clear ();
                uiNodeWrapperDict.Remove (uiNode_);
            }else{
                throw new Exception ("ERROR : "+uiNode_.uiName+ " 为被ComponentWrapper记录" );
            }
        }
        protected virtual void Awake () {

        }
        protected bool isInit = false;
        protected bool isDestroyed = false;
        protected bool isOnlyBindToUI = false;
        protected DataPathListener dataPathListener = null;
        protected UINode uiNode = null;
        protected string pattern;
        public string dataPath;
        private List<DataPathExpressionListener> dataPathExpressionListenerList = null;
        public string getExpressionPath (string calculationOrPath_, Property2DWrapper property2D_ = null) {
            if (calculationOrPath_.isExpressionStyle ()) {
                DataPathExpressionListener _dataPathExpressionListener = DataPathExpressionListener.reUse ();
                string _uniquePath = _dataPathExpressionListener.reset (calculationOrPath_, property2D_);
                if (dataPathExpressionListenerList == null) {
                    dataPathExpressionListenerList = new List<DataPathExpressionListener> ();
                }
                dataPathExpressionListenerList.Add (_dataPathExpressionListener);
                return _uniquePath;
            }
            return calculationOrPath_;
        }
        protected void clearExpressionList () {
            if (dataPathExpressionListenerList != null) {
                for (int _idx = 0; _idx < dataPathExpressionListenerList.Count; _idx++) {
                    dataPathExpressionListenerList[_idx].unUse ();
                }
                dataPathExpressionListenerList.Clear ();
                dataPathExpressionListenerList = null;
            }
        }
        protected void clearDataPathListener () {
            dataPathListener?.unUse ();
            dataPathListener = null;
        }

        public virtual void reset (string dataPath_, UINode uiNode_ = null) {
#if UNITY_EDITOR
            if(debugRecord && isInit == false){
                isInit = true;
                dc.sv("debug.objectCount.wrapper",dc.gv("debug.objectCount.wrapper").AsInt + 1);
            }
#endif
            if (dataPath_.isStartsWith ("_ui_.") && uiNode_ == null) {
                throw new Exception ("ERROR : " + dataPath_ + " 是相对路径，必须指定所在的UI节点。");
            }
            if (dataPath_.isStartsWith ("_dt_.") && (uiNode_ == null || uiNode_.dtPath == null)) {
                throw new Exception ("ERROR : " + dataPath_ + " 是相对路径，必须指定所在的UI节点挂载的数据路径。");
            }
            clearDataPathListener ();
            dataPath = getExpressionPath (dataPath_);
            dataPathListener = DataUIBinder.DataPathListener.reUse ();
            if (uiNode_ != null) {
                dataPath = uiNode_.convertThisOrDataPath (dataPath);
            }
            resetUINode (uiNode_);
            dataPathListener.reset(dataPath, dataChangeHandle);
        }
        protected void resetUINode (UINode uiNode_) {
            if (uiNode == null && uiNode_ == null){
                //非UINode承载的组件
            }else if(uiNode == null && uiNode_ != null) {//初始化
                uiNode = uiNode_;
                List<ComponentWrapper> _componentWrapperList = null;
                if (!uiNodeWrapperDict.TryGetValue (uiNode, out _componentWrapperList)) {
                    _componentWrapperList = new List<ComponentWrapper> ();
                    uiNodeWrapperDict[uiNode] = _componentWrapperList;
                }
                _componentWrapperList.Add (this);
            }else if(uiNode != null && uiNode_ == null){
                throw new Exception ("ERROR : 不能在运行时，将已有的 UINode 赋空。");
            }else if(uiNode != uiNode_ && uiNode != null && uiNode_ != null){
                throw new Exception ("ERROR : 运行时，不能更改所在的 uiNode 对象");
            }else if(uiNode == uiNode_){
                //依然是原对象
            }else{
                throw new Exception ("ERROR : \nuiNode is null : "+(uiNode==null).ToString()+"\nuiNode_ is null : " + (uiNode_==null)+"\nuiNode==uiNode_ : "+(uiNode==uiNode_).ToString());
            }
        }
        public virtual void dataChangeHandle (string dataPath_, JSONNode jsNode_) {
            throw new Exception ("ERROR : 必须由子类实现。");
        }
        protected virtual void OnDestroy () {
            if(isDestroyed){ return; }
            isDestroyed = true;
            clearExpressionList ();
            clearDataPathListener ();
            uiNode = null;
            dataPath = null;
#if UNITY_EDITOR
            if (debugRecord){
                dc.sv("debug.objectCount.wrapper",dc.gv("debug.objectCount.wrapper").AsInt - 1);
            }
#endif
        }
    }
}