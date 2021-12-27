using System;
using UnityEngine;
using UnityEngine.Windows;
using System.Collections;
using System.Collections.Generic;
namespace DataUIBinder{
    public enum ResLoadType{
        None,
        Resources,
        AssetBundle,
        Local
    }
    public class ResUtils{
        public static ResLoadType loadType = ResLoadType.None;
        public static GameObject getPrefab(string abPackageOrFolderPath_,string uiName_,ResLoadType loadType_ = ResLoadType.None){
            ResLoadType _currentResLoadType = loadType_;
            if (_currentResLoadType == ResLoadType.None){//没有指定读取模式，用全局设置的模式
                _currentResLoadType = loadType;
            }
            if (_currentResLoadType == ResLoadType.Resources){
                return getPrefabFromResource(abPackageOrFolderPath_,uiName_);
            }else if (_currentResLoadType == ResLoadType.AssetBundle){
                return null;
            }else if (_currentResLoadType == ResLoadType.Local){
                return null;
            }else{//设置模式和全局模式均为空。
                Debug.LogError ("ERROR " + System.Reflection.MethodBase.GetCurrentMethod ().ReflectedType.FullName + " -> " + new System.Diagnostics.StackTrace ().GetFrame (0).GetMethod ().Name + " : " +
                    "没指定读取方式"
                );
                return null;
            }
        }
        public static GameObject getPrefabFromResource(string folderPath_,string uiName_){
            return getPrefabFromResource(System.IO.Path.Combine(folderPath_,uiName_));
        }
        public static GameObject getPrefabFromResource(string uiPrefabPath_){
            GameObject _uiPrefabGameObject = Resources.Load<GameObject>(uiPrefabPath_);
            if (_uiPrefabGameObject == null){
                Debug.LogError("ERROR " + System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName + " -> " + new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name + " : " +
                    uiPrefabPath_ + " : 没有找到这个界面."
                );
                return null;
            }
            return GameObject.Instantiate(_uiPrefabGameObject);
        }
    }
}