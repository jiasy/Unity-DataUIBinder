using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using DataUIBinder;
public class DataPathDrivenTest : MonoBehaviour{
    /*
        数据驱动（数据多米诺）
            数据发生变更时，通过指定的关联关系，创建或修改已有的数据源
        支持类型
            listToDict 列表合并到字典上
            listToList 列表追加合并到列表上
            listFromDict 列表变化时，从字典上获取信息
    */
    private DataPathDriven dataPathDriven = null;
    void Awake(){
        DataCenter.defalutInit();
    }
    IEnumerator Start(){
        yield return null;
        
        printStep("0-创建 数据驱动");
        dataPathDriven = DataPathDriven.reUse();
        
        dataPathDriven.listToDict(
            "temp.itemConfig",//temp.itemConfig 变化时，将其上的列表
            "config.itemConfigDict",//合并到 config.itemConfigDict 字典上
            "itemID",//使用 temp.itemConfig.[x].itemID 的值作为 config.itemConfigDict 上的键
            ()=>{//合并之前
                printStep("1.1 listToDict Begin");
                dc.gv("temp").printDataByStruct();
                dc.gv("config").printDataByStruct();
            },
            ()=>{//合并之后
                printStep("1.2 listToDict End");
                dc.sv("temp.itemConfig",null);
                dc.gv("temp").printDataByStruct();
                dc.gv("config").printDataByStruct();
            }
        );
        dataPathDriven.listToList(
            "server.itemList",//temp.itemList 变化时，将其上的列表
            "module.inventory.itemList",//合并到 module.inventory.itemList 列表上
            "itemID",//使用列表元素的 itemID 键，进行判断合并
            ()=>{//合并之前
                printStep("2.1 listToList Begin");
                dc.gv("server").printDataByStruct();
                dc.gv("module").printDataByStruct();
            },
            ()=>{//合并之后
                printStep("2.2 listToList End");
                dc.sv("server.itemList",null);
                dc.gv("server").printDataByStruct();
                dc.gv("module").printDataByStruct();
            }
        );
        /*
            比如，服务器下发了背包的道具列表，但是不包括道具信息，从道具配置上将信息拽到列表元素上。
        */
        dataPathDriven.listFromDict(
            "module.inventory.itemList",//module.inventory.itemList 变化时
            "config.itemConfigDict",//从 config.itemConfigDict 字典上获取数据
            "itemID",//通过 module.inventory.itemList 元素的 itemID 键和 config.itemConfigDict 的 itemID 键进行匹配。
            ()=>{
                printStep("2.3 listFromDict Begin");
                dc.gv("module").printDataByStruct();
            },
            ()=>{
                printStep("2.4 listFromDict End");
                dc.gv("module").printDataByStruct();
            }
        );
        yield return null;

        
        printStep("1-读取配置到 temp");
        DataCenter.setValueByJsonFile("temp","DataPathListenerTest_config");
        yield return null;

        printStep("2-模拟服务器下发消息");
        TextAsset _textAsset = Resources.Load<TextAsset>("DataPathListenerTest_inventory");
        SimpleJSON.JSONNode _jsNode = SimpleJSON.JSONNode.Parse(_textAsset.ToString());
        DataCenter.mergeValue("server",_jsNode);
        yield return null;
    }
    void OnDestroy(){
        printStep("3-清理 数据驱动");
        dataPathDriven.unUse();
        dataPathDriven = null;
    }
    void Update(){
        DataCenter.frameUpdate(Time.deltaTime);
    }
    private void dataPathList_Changed(string[] dataPathList_) {
        
    }
    private void printStep(string step_){
        LogToFiles.printToAll("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ┏ " +step_ + " ┓ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }
}

