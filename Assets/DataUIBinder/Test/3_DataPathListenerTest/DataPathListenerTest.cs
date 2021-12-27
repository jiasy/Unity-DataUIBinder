using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using DataUIBinder;

public class DataPathListenerTest : MonoBehaviour{
    DataPathListener dataPathListener;
    DataPathListListener dataPathListListener;

    void Awake(){
        DataCenter.defalutInit();
    }
    IEnumerator Start(){
        yield return null;
        
        printStep("0-创建监听器");
        dataPathListener = DataUIBinder.DataPathListener.reUse();
        dataPathListListener = DataUIBinder.DataPathListListener.reUse();
        
        printStep("1-设置监听");
        /*
            同时监听两个数据路径
                本地 module.invetory.itemList 的数据来源于 配置和服务器的组合
                任意一个路径变化，都要进行 module.invetory.itemList 重构
        */
        dataPathListListener.reset(
            new string[]{ 
                "config.itemConfig", //配置变化
                "server.itemList" //服务器同步的数据变化
            },
            dataPathList_Changed
        );
        // 当 module.invetory.itemList 重构发生时的处理。
        dataPathListener.reset( "module.invetory.itemList" , module_invetory_itemList_Changed );
        
        printStep("2-配置Config");
        DataCenter.setValueByJsonFile("config","DataPathListenerTest_config");
        dc.gv("config").printDataByStruct();
        yield return null;

        printStep("3-模拟服务器数据结构");
        //创建一个临时数据节点 RemoteServer
        JSONRoot _removeServer = new JSONRoot(new string[]{"RemoteServer"});
        //将 DataPathListenerTest_inventory.内容合并到 RemoteServer 节点上。
        TextAsset _textAsset = Resources.Load<TextAsset>("DataPathListenerTest_inventory");
        SimpleJSON.JSONNode _jsNode = SimpleJSON.JSONNode.Parse(_textAsset.ToString());
        _removeServer["RemoteServer"].merge(_jsNode.AsObject);
        //打印 RemoteServer 的键值状态
        _removeServer.dispatchJustChange();
        //打印 RemoteServer 的结构
        _removeServer.printDataByStruct();
        yield return null;

        
        printStep("4-模拟服务器下发消息");
        // RemoteServer 下发的 itemList 对应到 server 上。
        dc.mv("server.itemList",_removeServer["RemoteServer.itemList"]);
        dc.gv("server").printDataByStruct();
        yield return null;
    }
    void OnDestroy(){
        dataPathListener.unUse();
        dataPathListListener.unUse();
    }

    void Update(){
        DataCenter.frameUpdate();
    }

    
    private void dataPathList_Changed(List<string> dataPathList_) {
        printStep("dataPathList_Changed");
        //打印发生变化的路径
        for (int _idx = 0; _idx < dataPathList_.Count; _idx++) {
	        var _dataPath = dataPathList_[_idx];
            LogToFiles.log("    " + _dataPath + " -> " + dc.gv(_dataPath).ToString());
        }
        //获取配置和服务器下发道具列表
        JSONArray _itemConfigList = dc.gv("config.itemConfig").AsArray;
        JSONArray _itemInventoryList = dc.gv("server.itemList").AsArray;
        //清空原有
        dc.sv("module.invetory.itemList",null);
        //将道具配置和服务器下发列表整合成新的列表。
        for (int _idx = 0; _idx < _itemInventoryList.Count; _idx++) {
            JSONObject _itemInventoryInfo = _itemInventoryList[_idx].AsObject;
            for (int _idxInside = 0; _idxInside < _itemConfigList.Count; _idxInside++) {
                JSONObject _itemConfig = _itemConfigList[_idxInside].AsObject;
                if(_itemInventoryInfo["itemID"] == _itemConfig["itemID"]){
                    _itemInventoryInfo["name"] = _itemConfig["name"];
                    _itemInventoryInfo["desc"] = _itemConfig["desc"];
                    _itemInventoryInfo["price"] = _itemConfig["price"];
                }
            }
        }
        //重新设置
        dc.mv("module.invetory.itemList",_itemInventoryList);
        //清空服务器下发数据。
        dc.sv("server.itemList",null);
    }
    
    private void module_invetory_itemList_Changed(string dataPath_, JSONNode jsNodeOnPath_) {
        printStep("module_invetory_itemList_Changed");
        jsNodeOnPath_.printDataByStruct();
    }
    private void printStep(string step_){
        LogToFiles.printToAll("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ┏ " +step_ + " ┓ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }
}

