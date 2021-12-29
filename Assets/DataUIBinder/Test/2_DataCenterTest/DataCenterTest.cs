using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataUIBinder;
using SimpleJSON;
public class DataCenterTest : MonoBehaviour{
    private void Awake(){
        DataCenter.defalutInit();
    }
    IEnumerator Start(){
        yield return null;

        LogToFiles.logByType(LogToFiles.LogType.ListenerState,nameof(JSONRootTest) + " < Start > ---------------------------------");
        //监听数据路径
        DataCenter.addListener("save.localSave",save_localSave_Changed);
        DataCenter.addListener("module.user.userInfo",user_userInfo_Changed);
        DataCenter.addListener("module.user.skillInfoList",user_skillInfoList_Changed);
        //将 DataCenterTest.json 的内容写入 save.localSave。触发 save.localSave 变更，调用 save_localSave_Changed
        DataCenter.setValueByJsonFile("save.localSave","DataCenterTest");
    }
    private void OnDestroy(){
        LogToFiles.logByType(LogToFiles.LogType.ListenerState,nameof(JSONRootTest) + " < OnDestroy > ---------------------------------");
        //移除监听，一对一
        DataCenter.removeListener("save.localSave",save_localSave_Changed);
        DataCenter.removeListener("module.user.userInfo",user_userInfo_Changed);
        DataCenter.removeListener("module.user.skillInfoList",user_skillInfoList_Changed);
    }
    void Update(){
        
        DataCenter.frameUpdate(Time.deltaTime);
    }
    private void save_localSave_Changed(string dataPath_, JSONNode jsNode_) {
        LogToFiles.logByType(LogToFiles.LogType.DataStruct,nameof(JSONRootTest) + " < save_localSave_Changed > -------------------- save.localSave 变化");
        DataCenter.root.printDataByStruct();
        // 将 save.localSave.userInfo 和 save.localSave.skillInfoList 设置给 module.user 的对应键
        DataCenter.mergeValue("module.user.userInfo",jsNode_["userInfo"]);
        DataCenter.mergeValue("module.user.skillInfoList",jsNode_["skillInfoList"]);
        //转移后清理数据
        jsNode_["userInfo"] = null;
        jsNode_["skillInfoList"] = null;
        //打印转移后的数据状态
        LogToFiles.logByType(LogToFiles.LogType.DataStruct,nameof(JSONRootTest) + " < save_localSave_Changed > --------- module.user 变化，save.localSave 清理");
        DataCenter.root.printDataByStruct();
    }
    private void user_userInfo_Changed(string dataPath_, JSONNode jsNode_) {
        LogToFiles.logByType(LogToFiles.LogType.DataStruct,nameof(JSONRootTest) + " < user_userInfo_Changed > -------------------- module.user.userInfo 变化");
        jsNode_.printDataByStruct();

    }
    private void user_skillInfoList_Changed(string dataPath_, JSONNode jsNode_) {
        LogToFiles.logByType(LogToFiles.LogType.DataStruct,nameof(JSONRootTest) + " < user_skillInfoList_Changed > --------------- module.user.skillInfoList 变化");
        jsNode_.printDataByStruct();
    }
}
