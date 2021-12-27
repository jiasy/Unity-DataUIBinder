using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace DataUIBinder{
    public enum UIType{
        None = -1,
        Base,
        Float,
        Pop,
        Mask,
        Guide,
        Tip,
        Loading,
        Notice,
        Debug
    }
    public enum UIState{
        None = -1,
        Open,
        Close,
        Destroy
    }
    public class UIInfo {
        public string folderPath = null;
        public string uiName = null;
        public UIType type = UIType.None;
        public ResLoadType loadType = ResLoadType.None;
        public UIInfo(string folderPath_,string uiName_,UIType type_,ResLoadType loadType_) {
            folderPath = folderPath_;
            uiName = uiName_;
            type = type_;
            loadType = loadType_;
        }
        public void printInfo(){
            Debug.Log("folderPath : "+folderPath);
            Debug.Log("uiName : "+uiName);
            Debug.Log("type : "+type.ToString());
            Debug.Log("loadType : "+loadType.ToString());
        }
    }
}