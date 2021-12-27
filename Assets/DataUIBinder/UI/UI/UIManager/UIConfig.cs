using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
namespace DataUIBinder{
    public class UIConfig : MonoBehaviour {
		private static UIConfig _instance;
		public static UIConfig instance{
			get{
				if (_instance == null) {
					Debug.LogError ("ERROR " + System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName + " -> " + new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name + " : " +
						"instance is not created."
					);
				}
				return _instance;
			}
			set{
				if (_instance != null) {
					Debug.LogError ("ERROR " + System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName + " -> " + new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name + " : " +
						"instance is already exist."
					);
				}
				_instance = value;
			}
		}
        private Dictionary<string,UIInfo> uiConfigDict = new Dictionary<string,UIInfo>();
        void Awake () {
            instance = this;
        }
        public UIInfo getUIInfo(string uiName_){
            if (uiConfigDict.ContainsKey(uiName_)){
                return uiConfigDict[uiName_];
            }
            Debug.LogError("ERROR " + System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName + " -> " +
                new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name + " : " +
                "配置不存在 " + uiName_
            );
            return null;
        }
        public void createConfig(string folderPath_,string uiName_,UIType type_,ResLoadType loadType_) {
            UIInfo _uiInfo = new UIInfo(folderPath_,uiName_,type_,loadType_);
            uiConfigDict[uiName_] = _uiInfo;
        }
        public void getUIConfigByJson(JSONArray uiConfigList_){
            JSONNode _uiConfig = null;
            for (int _idx = 0;_idx < uiConfigList_.Count;_idx++) {
                _uiConfig = uiConfigList_[_idx];
                createConfig(
                    _uiConfig["folderPath"].AsString,
                    _uiConfig["uiName"].AsString,
                    (UIType)Enum.Parse(typeof(UIType), _uiConfig["type"]),
                    (ResLoadType)Enum.Parse(typeof(ResLoadType), _uiConfig["loadType"])
                );
            }
        }
    }
}