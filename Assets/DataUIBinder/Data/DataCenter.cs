using System;
using UnityEngine;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
namespace DataUIBinder{
#region 获取设置值，缩写
    public class dc{
        public static JSONRoot rt = null;
        public static JSONNode gv(string dataPath_,object value_ = null){
            return rt.getJsonNodeByRelativePath(dataPath_,value_,true);
        }
        public static void sv(string dataPath_,JSONNode jsNode_){
            rt.mergeValue(dataPath_,jsNode_,true);
        }
        public static void sv(string dataPath_,object value_){
            rt.mergeValue(dataPath_,value_,true);
        }
        public static void mv(string dataPath_,JSONNode jsNode_){
            rt.mergeValue(dataPath_,jsNode_,true);
        }
        public static void mv(string dataPath_,object value_){
            rt.mergeValue(dataPath_,value_,true);
        }
        public static void setDirty(string dataPath_){
            DataCenter.setDirty(dataPath_);
        }
    }
#endregion

    public class DataCenter{
        public static JSONRoot root = null;
        public static Dictionary<string, Func<string,string>> customFormatDict = null;

        public static string getFormatString(JSONNode jsonNode_,string format_){
            Func<string,string> _formatFunc = null;
            string _value = null;
            if (DataCenter.customFormatDict.TryGetValue(format_,out _formatFunc)){//自定义格式化
                _value = _formatFunc(jsonNode_.ToString());
            }else{
                if(jsonNode_.Tag == JSONNodeType.Number){//C#自带格式化
                    _value = string.Format("{0:"+format_+"}",jsonNode_.AsDouble);
                }else{
                    UnityEngine.Debug.LogWarning(jsonNode_.ToString()+" 使用 "+format_+" 进行格式化，没有实现");
                    _value = jsonNode_.AsString;
                }
            }
            return _value;
        }

#region init and clear
        public static void defalutInit(){
            init(new string[]{
                //配置，游戏本身的数据配置，道具信息，UI的路径和类型等等。
                "config",
                //本地存储，本地的热更版本号等等，需要缓存的信息。App启动、关闭的时候，进行读写。
                "save",
                //本地存储的一个子类，这里独立出来，用户设置 - 音量等
                "settings",
                //服务器交互 - 服务器下行的数据的空间，需要进行数据流分流（根据实际需要，每帧清理，将数据平移到对应的module中）。
                "server",
                //模块 - 业务逻辑的数据空间，每个模块都可以挂在多个ui节点。
                "module",
                //UI节点状态 - 当 ui 显示之后，节点上的控件数据映射到 ui 的数据节点上。
                "ui",
                /*临时数据，
                    一些临时用到的，随手创建
                    一些引擎内使用的路径转换，比如监听数学运算表达式的结果。
                        module.xx.lastTime + 10 -> temp.calculation.<uniqueID>
                */
                "temp",
                //调试用，可以记录当前有多少个监听存在，虚拟机内存等等信息。
                "debug",
                //原生交互，在Swift和Java实现同一套SimpleJSON，Native 和 C# 之间靠推送 JSON，来保持 native 数据空间的同步
                "native",
                /*多国语言本地化
                    通过插件，静态识别所有Prefab的中文然后转换成表
                        生成Excel然后找人翻译，每一个country列对应一种语言。
                    ui初始化的时候将中文拼写进行二次转换后再加载组件封装，从而实现本地化。
                        用户名:${user.name} -> ${localize.<country>.用户名:}:${user.name}
                */
                "localize",
            });
            //设置字符串的自定义格式化${数据路径[格式化方式]}
            customFormatDict = new Dictionary<string, Func<string,string>>();
            customFormatDict["customFormat"] = (string_)=>{
                return "<" + string_ + ">";
            };
            root.setValue("config.resLoadType","None");
            root.setValue("debug.objectCount.dp",0);
            root.setValue("debug.objectList.dp","");
            root.setValue("debug.objectCount.dpList",0);
            root.setValue("debug.objectCount.dpCompare",0);
            root.setValue("debug.objectCount.dpExpression",0);
            root.setValue("debug.objectCount.dpRange",0);
            root.setValue("debug.objectCount.wrapper",0);
            root.setValue("debug.objectCount.uiNode",0);
        }
        
        public static void init(string[] namespaceList_){
            root = new JSONRoot(namespaceList_);
            dc.rt = root;
#if UNITY_EDITOR
            LogToFiles.init();
#endif
        }
        
        public static void reset(string[] namespaceList_){
            root.reset(namespaceList_);
        }
#endregion

#region event listen
        public static void addListener(string dataPath_,Action<string,JSONNode> callBack_){
            root.dataChangeDispatcher.addListener(dataPath_,callBack_);
        }
        public static void removeListener(string dataPath_,Action<string,JSONNode> callBack_){
            root.dataChangeDispatcher.removeListener(dataPath_,callBack_);
        }
#endregion

#region get and set value
        public static void setValueByJsonFile(string dataPath_,string filePathInResource_){
            TextAsset _textAsset = Resources.Load<TextAsset>(filePathInResource_);
            SimpleJSON.JSONNode _jsNode = SimpleJSON.JSONNode.Parse(_textAsset.ToString());
            setValue(dataPath_,_jsNode);
        }
        public static void mergeValueByJsonFile(string dataPath_,string filePathInResource_){
            TextAsset _textAsset = Resources.Load<TextAsset>(filePathInResource_);
            SimpleJSON.JSONNode _jsNode = SimpleJSON.JSONNode.Parse(_textAsset.ToString());
            mergeValue(dataPath_,_jsNode);
        }
        public static void setValue(string dataPath_,object value_){
            root.mergeValue(dataPath_,value_,true);
        }
        public static void setValue(string dataPath_,JSONNode jsNode_){
            root.mergeValue(dataPath_,jsNode_,true);
        }
        public static void mergeValue(string dataPath_,object value_,bool isSet_ = false){
            root.mergeValue(dataPath_,value_,isSet_);
        }
        public static void mergeValue(string dataPath_,JSONNode jsNode_,bool isSet_ = false){
            root.mergeValue(dataPath_,jsNode_,isSet_);
        }
        public static JSONNode getValue(string dataPath_,object default_ = null){
            if(dataPath_.IndexOf('.') < 0){
                if(default_ != null){
                    throw new Exception("ERROR : default can not use on namespace.");
                }
                return root[dataPath_];
            }else{
                JSONNode _jsNodeOnPath = root.getValue(dataPath_);
                if(_jsNodeOnPath == null && default_ != null){
                    _jsNodeOnPath = JSONNode.convertValueToJsonNode(default_);
                    root[dataPath_] = _jsNodeOnPath;
                }
                return _jsNodeOnPath;
            }
        }
        public static void setDirty(string dataPath_){
            JSONNode _jsNode = root.getJsonNodeByRelativePath(dataPath_);
            if(_jsNode != null){
                root.changeValue(dataPath_,_jsNode);
            }
        }
#endregion

#region merge key-value on path
        public static void mergeDictToDict(string dictPath_,string targetDictPath_){
            JSONNode _jsNodeOnDictPath = getValue(dictPath_);
            if(_jsNodeOnDictPath == null){
                return;
            }
            JSONObject _fromObject = _jsNodeOnDictPath.AsObject;
            if(_fromObject == null){
                throw new Exception("ERROR : from " + dictPath_ +" is not array.");
            }

            JSONNode _jsNodeOnTargetListPath = getValue(targetDictPath_);
            if(_jsNodeOnTargetListPath == null){
                _jsNodeOnTargetListPath = new JSONObject();
                mergeValue(targetDictPath_,_jsNodeOnTargetListPath);
            }
            JSONObject _targetDictObject = _jsNodeOnTargetListPath.AsObject;
            if(_targetDictObject == null){
                throw new Exception("ERROR : to " + targetDictPath_ +" is not dictionary.");
            }
            
            _fromObject.merge(_targetDictObject);
        }
        public static void mergeDictToList(string dictPath_,string targetListPath_,string mergeKey_){
            JSONNode _jsNodeOnTargetListPath = getValue(targetListPath_);
            if(_jsNodeOnTargetListPath == null){
                return;
            }
            JSONArray _toArray = _jsNodeOnTargetListPath.AsArray;
            if(_toArray == null){
                throw new Exception("ERROR : to " + targetListPath_ +" is not array.");
            }
            
            JSONNode _jsNodeOnDictPath = getValue(dictPath_);
            if(_jsNodeOnDictPath == null){
                throw new Exception("ERROR : from " + dictPath_ +" is null.");
            }
            JSONObject _fromObject = _jsNodeOnDictPath.AsObject;
            if(_fromObject == null){
                throw new Exception("ERROR : from " + dictPath_ +" is not dictionary.");
            }
            
            JSONNode _itemJsNode = null;
            JSONObject _toItemObject = null;
            JSONNode _valueOnToItemMergeKey = null;
            string _keyOnDict = null;
            JSONNode _valueOnDict = null;
            for (int _idx = 0; _idx < _toArray.Count; _idx++) {
                _itemJsNode = _toArray[_idx];
                if(_itemJsNode.Tag != JSONNodeType.Object){
                    throw new Exception("ERROR : to " + targetListPath_ + "["+_idx.ToString()+"] is not dictionary.");
                }
                _toItemObject = _itemJsNode.AsObject;
                _valueOnToItemMergeKey = _toItemObject[mergeKey_];
                if(_valueOnToItemMergeKey == null){
                    throw new Exception("ERROR : to " + targetListPath_ + "["+_idx.ToString()+"] lost merge key : "+mergeKey_);
                }
                _keyOnDict = _valueOnToItemMergeKey.asListMergeToDictKey();
                _valueOnDict =  _fromObject[_keyOnDict];
                if(_valueOnDict != null){
                    if(_valueOnDict.Tag != JSONNodeType.Object){
                        throw new Exception("ERROR : from " + dictPath_ + " 's "+_keyOnDict+" is not dictionary.");
                    }
                    _toItemObject.merge(_valueOnDict);
                }
            }
        }
        public static void mergeListToDict(string listPath_,string targetDictPath_,string mergeKey_){
            JSONNode _jsNodeOnListPath = getValue(listPath_);
            if(_jsNodeOnListPath == null){
                return;
            }
            JSONArray _fromArray = _jsNodeOnListPath.AsArray;
            if(_fromArray == null){
                throw new Exception("ERROR : from " + listPath_ +" is not array");
            }
            
            JSONNode _jsNodeOnTargetListPath = getValue(targetDictPath_);
            if(_jsNodeOnTargetListPath == null){
                _jsNodeOnTargetListPath = new JSONObject();
                mergeValue(targetDictPath_,_jsNodeOnTargetListPath);
            }
            JSONObject _targetDictObject = _jsNodeOnTargetListPath.AsObject;
            if(_targetDictObject == null){
                throw new Exception("ERROR : to " + targetDictPath_ +" is not dictionary.");
            }
            
            JSONNode _fromItemNode = null;
            JSONObject _fromItemObj = null;
            JSONNode _valueOnFromItemMergeKey = null;
            for (int _idx = 0; _idx < _fromArray.Count; _idx++) {
                _fromItemNode = _fromArray[_idx];
                if(_fromItemNode.Tag != JSONNodeType.Object){
                    throw new Exception("ERROR : from " + listPath_ + "["+_idx.ToString()+"] is not dictionaty");
                }
                _fromItemObj = _fromItemNode.AsObject;
                _valueOnFromItemMergeKey = _fromItemObj[mergeKey_];
                if(_valueOnFromItemMergeKey == null){
                    throw new Exception("ERROR : from " + listPath_ + "["+_idx.ToString()+"] lost merge key "+mergeKey_);
                }
                _targetDictObject[_valueOnFromItemMergeKey.asListMergeToDictKey()] = _fromItemObj;
            }
        }
        public static List<int> getFilterSortIdxList(string listPath_,Func<JSONNode,bool> filterFunc_ = null,Func<JSONNode,JSONNode,int> sortFunc_ = null){
            List<int> _idxList = new List<int>();
            JSONNode _jsNode = root.getValue(listPath_);
            if(_jsNode == null){
                return _idxList;
            }
            JSONArray _jsArray = _jsNode as JSONArray;
            if(_jsArray == null){
                throw new Exception("ERROR : "+listPath_+" is not list.");
            }
            if(sortFunc_ == null && filterFunc_ == null){
                int _jsArrayLength = _jsArray.Count;
                int _idx = 0;
                while(_idx < _jsArrayLength){
                    _idxList.Add(_idx);
                    _idx++;
                }
                return _idxList;
            }
            JSONArray _jsTempArray = _jsArray.Clone() as JSONArray;
            List<JSONNode> _jsNodeList = _jsTempArray.getList();
            if(filterFunc_ != null){
                List<JSONNode> _filterList = new List<JSONNode>();
                for (int _idx = 0; _idx < _jsNodeList.Count; _idx++) {
                    JSONNode _jsItemNode = _jsNodeList[_idx];
                    //// - 排序会添加一个临时的字段
                    _jsItemNode["__idx__"] = _idx;
                    if(!filterFunc_(_jsItemNode)){
                        _filterList.Add(_jsItemNode);
                    }
                }
                _jsNodeList = _filterList;
            }
            if(sortFunc_ != null){
                _jsNodeList.Sort((x_,y_)=>{
                    return sortFunc_(x_,y_);
                });
            }
            for (int _idx = 0; _idx < _jsNodeList.Count; _idx++) {
	            _idxList.Add(_jsNodeList[_idx]["__idx__"].AsInt);
            }
            _jsTempArray.Clear();
            _jsNodeList.Clear();
            return _idxList;
        }
        public static void fliterSortListToList(string listPath_,string targetListPath_,Func<JSONNode,bool> filterFunc_ = null,Func<JSONNode,JSONNode,int> sortFunc_ = null){
            JSONNode _jsNode = root.getValue(listPath_);
            if(_jsNode == null){
                return;
            }
            JSONArray _jsArray = _jsNode as JSONArray;
            _jsNode = null;
            if(_jsArray == null){
                throw new Exception("ERROR : "+listPath_+" is not list.");
            }
            JSONArray _jsTempArray = _jsArray.Clone() as JSONArray;
            _jsArray = null;
            if(sortFunc_ == null && filterFunc_ == null){
                root.setValue(targetListPath_,_jsTempArray);
                return;
            }
            List<JSONNode> _jsNodeList = _jsTempArray.getList();
            int _jsNodeListLength = _jsNodeList.Count;
            if(_jsNodeListLength == 0){
                root.setValue(targetListPath_,_jsTempArray);
                return;
            }
            if(filterFunc_ != null){
                List<JSONNode> _filterList = new List<JSONNode>();
                for (int _idx = 0; _idx < _jsNodeListLength; _idx++) {
                    JSONNode _jsItemNode = _jsNodeList[_idx];
                    if(!filterFunc_(_jsItemNode)){
                        _filterList.Add(_jsItemNode);
                    }
                }
                if(_filterList.Count != _jsNodeListLength){
                    _jsNodeList = null;
                    _jsTempArray.setList(_filterList);
                    _jsNodeList = _jsTempArray.getList();
                }
                _filterList = null;
            }
            if(sortFunc_ != null){
                _jsNodeList.Sort((x_,y_)=>{
                    return sortFunc_(x_,y_);
                });
            }
            root.setValue(targetListPath_,_jsTempArray);
        }
        public static void mergeListToList(string listPath_,string targetListPath_,string mergeKey_){
            JSONNode _jsNodeOnListPath = getValue(listPath_);
            if(_jsNodeOnListPath == null){
                return;
            }
            JSONArray _fromArray = _jsNodeOnListPath.AsArray;
            if(_fromArray == null){
                throw new Exception("ERROR : " + listPath_ +" is not array.");
            }
            
            JSONNode _jsNodeOnTargetListPath = getValue(targetListPath_);
            if(_jsNodeOnTargetListPath == null){
                _jsNodeOnTargetListPath = new JSONArray();
                mergeValue(targetListPath_,_jsNodeOnTargetListPath);
            }
            JSONArray _toArray = _jsNodeOnTargetListPath.AsArray;
            if(_toArray == null){
                throw new Exception("ERROR : " + targetListPath_ +" is not dictionary.");
            }
            
            int _toArrayBeginLength = _toArray.Count;
            
            JSONNode _fromItemNode = null;
            JSONObject _fromItemObj = null;
            JSONNode _fromMatchValue = null;
            JSONNode _toItemNode = null;
            JSONObject _toItemObj = null;
            JSONNode _toMatchValue = null;
            for (int _idx_from = 0; _idx_from < _fromArray.Count; _idx_from++) {
                _fromItemNode = _fromArray[_idx_from];
                if(_fromItemNode.Tag != JSONNodeType.Object){
                    throw new Exception("ERROR : from " + listPath_ + "["+_idx_from.ToString()+"] is not dictionary.");
                }
                _fromItemObj = _fromItemNode.AsObject;
                _fromMatchValue = _fromItemObj[mergeKey_];
                if(_fromMatchValue == null||!_fromMatchValue.asListMergeToListMatchValue()){
                    throw new Exception("ERROR : from " + listPath_ + "["+_idx_from.ToString()+"]["+mergeKey_+"] is not a value.");
                }
                bool _match = false;
                for (int _idx_to = 0; _idx_to < _toArrayBeginLength; _idx_to++) {
                    _toItemNode = _toArray[_idx_to];
                    if(_toItemNode.Tag != JSONNodeType.Object){
                        throw new Exception("ERROR : to " + targetListPath_ + "["+_idx_to.ToString()+"] is not dictionary.");
                    }
                    _toItemObj = _toItemNode.AsObject;
                    _toMatchValue = _toItemObj[mergeKey_];
                    if(_toMatchValue == null||!_toMatchValue.asListMergeToListMatchValue()){
                        throw new Exception("ERROR : to " + targetListPath_ + "["+_idx_to.ToString()+"]["+mergeKey_+"] is not a value.");
                    }
                    
                    if(_toMatchValue == _fromMatchValue){
                        _toItemObj.merge(_fromItemObj);
                        _match = true;
                        break;
                    }
                }
                
                if(!_match){
                    _toArray[_toArray.Count] = _fromItemObj;
                }
            }
        }
#endregion

        public static void frameUpdate(){
#if UNITY_EDITOR
            root.dataChangeDispatcher.printListenerCount();
#endif
            root.dispatchJustChange();
#if UNITY_EDITOR
            LogToFiles.frameUpdate();
#endif
        }
    }
}