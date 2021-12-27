using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using DataUIBinder;

public class JSONRootTest : MonoBehaviour{
    void Awake(){
        LogToFiles.init();//日志初始化
    }
    IEnumerator Start(){
        //0------------------------------------------------------------
        //初始化一个根节点
        JSONRoot _testRoot = new JSONRoot(new string[]{"temp"});
        doPrintThenDispatchJustChange(_testRoot,0);
        //设置监听器
        _testRoot.dataChangeDispatcher.addListener("temp.arr.[0].a",dataChangeHandle);
        yield return null;

        //1------------------------------------------------------------
        doPrintThenDispatchJustChange(_testRoot,1);
        //定义字典结构和初始值
        _testRoot["temp.str"] = "str";
        _testRoot["temp.arr.[0].a"] = "a";
        _testRoot["temp.arr.[0].b"] = "b";
        _testRoot["temp.arr.[1].a"] = "c";
        _testRoot["temp.arr.[2]"] = "d";
        _testRoot["temp.int"] = 1;
        _testRoot["temp.arr.[3].[0]"] = "e";
        _testRoot["temp.arr.[3].[1].a"] = "f";
        _testRoot["temp.bool"] = false;
        _testRoot["temp.dict.a"] = "g";
        _testRoot["temp.dict.arr.[0]"] = "h";
        _testRoot["temp.dict.arr.[1]"] = "i";
        yield return null;

        //2------------------------------------------------------------
        doPrintThenDispatchJustChange(_testRoot,2);
        //字典结构中的一个节点移除
        _testRoot["temp.arr"].AsArray.RemoveAt(0);
        //删除一个键值
        _testRoot["temp.dict.arr"] = null;
        yield return null;

        //3------------------------------------------------------------
        doPrintThenDispatchJustChange(_testRoot,3);
        //将一个json文件转换成数据节点，添加到根节点上。合并
        TextAsset _textAsset = Resources.Load<TextAsset>("JSONRootTest");
        SimpleJSON.JSONNode _jsNode = SimpleJSON.JSONNode.Parse(_textAsset.ToString());
        _testRoot["temp"].merge(_jsNode.AsObject);
        yield return null;
        //4------------------------------------------------------------
        //字典合并，数组替换
        doPrintThenDispatchJustChange(_testRoot,4);
        yield return null;
    }

    private void doPrintThenDispatchJustChange(JSONRoot jsRoot_,int step_){
        //进行一次校验
        jsRoot_.checkDataPath(true);
        printStep(step_.ToString());
        //打印事件监听器的情况
        jsRoot_.dataChangeDispatcher.printListenerCount();
        //打印变化后的结构
        jsRoot_.printDataByStruct();
        //打印变化的键值对
        jsRoot_.printJustChangeValueDict();
        //进行事件分发，变化内容分发出去。
        jsRoot_.dispatchJustChange();
    }

    private void dataChangeHandle(string dataPath_, JSONNode jsNode_) {
        LogToFiles.log("    DATA-CHANGED : " + dataPath_ + " -> " + jsNode_);
    }
    void Update(){
        LogToFiles.frameUpdate();//日志实际输出
    }
    private void printStep(string step_){
        LogToFiles.printToAll("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━ ┏ " +step_ + " ┓ ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
    }
}
