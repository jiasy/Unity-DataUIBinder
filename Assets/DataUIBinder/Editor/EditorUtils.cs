using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class EditorUtils{
#if UNITY_EDITOR_OSX
	[MenuItem("Assets/Show References", false, 2000)]
	private static void FindProjectReferences() {
		string _appDataPath = Application.dataPath;
		string _selectedAssetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
		string _selectAssetGuid = AssetDatabase.AssetPathToGUID (_selectedAssetPath);

        MacProcessUtils.doCommand(
            "/usr/bin/mdfind",
            "-onlyin " + Application.dataPath + " " + _selectAssetGuid,
            (isError_,logStrList_)=>{
                if(!isError_){
                    int _count = 0;
                    for (int _idx = 0; _idx < logStrList_.Count; _idx++) {
                        string _filePath = logStrList_[_idx];
                        string _relativePath = "Assets" + _filePath.Replace(_appDataPath, "");
                        if(_relativePath == _selectedAssetPath + ".meta"){
                            continue;
                        }
                        _count ++;
                        Debug.Log(_relativePath, AssetDatabase.LoadMainAssetAtPath(_relativePath));
                    }
                    Debug.LogError(_count + " references found for object " + Selection.activeObject.name);
                }
            }
        );
	}
#endif
}