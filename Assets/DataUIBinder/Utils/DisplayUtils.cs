using System;
using UnityEngine;
using UnityEngine.Windows;
using System.Collections;
using System.Collections.Generic;
namespace DataUIBinder{
    public class DisplayUtils{
        public static string getDisplayPath(Transform trans_){
            List<string> _nameList = new List<string>();
            Transform _currentTrans = trans_;
            while(_currentTrans.parent != null){
                if(_currentTrans.gameObject == null){
                    break;
                }
                if(_currentTrans.gameObject.name.isNullOrEmpty()){
                    break;
                }
                _nameList.Add(_currentTrans.gameObject.name);
                _currentTrans = _currentTrans.parent;
            }
            _nameList.Reverse();
            return _nameList.joinBy("\\");
        }
        public static void recursiveChildren(Transform trans_,Func<Transform,bool> transHandle){
            int _childrenCount = trans_.childCount;
            for (int _idx = 0;_idx < _childrenCount;_idx++) {
                Transform _transChild = trans_.GetChild(_idx);
                if(transHandle(_transChild)){
                    recursiveChildren(_transChild,transHandle);
                }
            }
        }
        public static GameObject getUIGameObjectAndAddTo(string name_,Transform transParent_){
            GameObject _uiGameObject = new GameObject ();
            _uiGameObject.name = name_;
            _uiGameObject.transform.SetParent(transParent_);
            RectTransform _rectTransform = _uiGameObject.AddComponent<RectTransform>();
            _rectTransform.initPosAndScale();
            _rectTransform.anchorMin = Vector2.zero;
            _rectTransform.anchorMax = new Vector2(1f,1f);
            return _uiGameObject;
        }
        public static void screenShot(string name_,System.Action<string,Texture2D> callBack_){
            string _filePath = System.IO.Path.Combine(Application.persistentDataPath, name_);
            if (System.IO.File.Exists(_filePath)){ 
                System.IO.File.Delete(_filePath); 
            }
            Texture2D _texture2D = ScreenCapture.CaptureScreenshotAsTexture();
            _texture2D.Apply();
            System.IO.File.WriteAllBytes(_filePath, _texture2D.EncodeToPNG());
            callBack_(_filePath,_texture2D);
        }
    }
}