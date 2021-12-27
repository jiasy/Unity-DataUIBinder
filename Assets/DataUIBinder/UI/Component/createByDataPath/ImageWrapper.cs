using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
namespace DataUIBinder{
    /*
        图片 绑定 数据路径
            http: 或 https: 开头，会直接访问网络，取得网址图片。
            非网址，按照路径从本地取得图片。
    */
    public class ImageWrapper : ComponentWrapper{
        private Image _image = null;
        private Image image{
            get{
                if(_image == null){
                    _image = gameObject.GetComponent<Image>();
                }
                return _image;
            }
        }
        private  WWW www;
        public override  void reset(string pattern_,UINode uiNode_ = null){
            pattern = pattern_;
            dataPath = null;
            base.reset(pattern_,uiNode_);
        }

        public override void dataChangeHandle(string dataPath_,JSONNode jsNode_){
            string _picLocalPath = jsNode_.AsString;
            if (
                (
                    _picLocalPath[0] =='h'&&
                    _picLocalPath[1] =='t'&&
                    _picLocalPath[2] =='t'&&
                    _picLocalPath[3] =='p'&&
                    _picLocalPath[4] ==':'
                )||(
                    _picLocalPath[0] =='h'&&
                    _picLocalPath[1] =='t'&&
                    _picLocalPath[2] =='t'&&
                    _picLocalPath[3] =='p'&&
                    _picLocalPath[4] =='s'&&
                    _picLocalPath[5] ==':'
                )
            ) {
                setPicByUrlAsync(_picLocalPath);
            }else{
                //TODO ResUtils 管理，通过当前加载类型判断是AB还是Resource.
                UnityEngine.Object _assetObject = Resources.Load(_picLocalPath, typeof(Sprite));
                if(_assetObject == null){
    #if UNITY_EDITOR
                    throw new Exception("ERROR : " + dataPath_ + " : "+ _picLocalPath + " ,get nothing.");
    #else
                    return;
    #endif
                }
                Sprite _targetSp = null;
                try {
                    _targetSp = Instantiate(_assetObject) as Sprite;
                } catch ( System.Exception _ ) {
    #if UNITY_EDITOR
                    throw new Exception("ERROR : " + dataPath_ + " : "+ _picLocalPath + " ,is not a image.");
    #else
                    return;
    #endif
                }
                image.sprite = _targetSp;
                image.SetNativeSize();
            }
        }
        private void setPicByUrlAsync(string url_){
            StopAllCoroutines();
            //// - 协程
            if (!File.Exists(ResourceCache.picCachePath + url_.GetHashCode()+".png")){
                StartCoroutine(downloadPic(url_));
            } else {
                StartCoroutine(loadLocalPic(url_));
            }
            // SAMPLE - res Clear
            Resources.UnloadUnusedAssets();
        }
        IEnumerator loadLocalPic(string url_){
            if(www != null){
                www.Dispose();
            }
            www = new WWW("file:///" + ResourceCache.picCachePath + url_.GetHashCode()+".png");
            yield return www;
            setPicByBytes(www.bytes);
        }
        IEnumerator downloadPic(string url_){
            if(www != null) {
                www.Dispose();
            }
            www = new WWW(url_);
            yield return www;
            if(www.error.isNullOrEmpty()){
                Texture2D _tex2d = setPicByBytes(www.bytes);
                byte[] _pngData = _tex2d.EncodeToPNG();
                if(_pngData.Length > 128){
                    File.WriteAllBytes(ResourceCache.picCachePath + url_.GetHashCode() + ".png", _pngData);
                }else{
                    throw new Exception("ERROR : " + url_ + " ,data error");
                }
            }else{
#if UNITY_EDITOR
                throw new Exception("ERROR : " + url_ + " error : " + www.error);
#endif
            }
        }
        private Texture2D setPicByBytes(byte[] bytes_){
            Texture2D _tex2d = new Texture2D(2, 2,TextureFormat.ARGB32,false);
            _tex2d.LoadImage(bytes_, false);
            _tex2d.Apply();
            image.sprite = Sprite.Create(_tex2d, new Rect(0, 0, _tex2d.width, _tex2d.height), new Vector2(0.5f, 0.5f));
            image.SetNativeSize();
            return _tex2d;
        }
        protected override void OnDestroy(){
            if(isDestroyed){ return; }
            StopAllCoroutines();
            _image = null;
            base.OnDestroy();
        }
    }
}