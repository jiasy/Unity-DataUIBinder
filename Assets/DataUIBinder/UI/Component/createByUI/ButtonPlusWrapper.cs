using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
namespace DataUIBinder{
    //支持长按和双击的按钮封装
    public class ButtonPlusWrapper : ButtonWrapper{
        private ButtonPlus _btnPlus;
        private ButtonPlus btnPlus{
            get{
                if(_btnPlus == null){
                    _btnPlus = btn as ButtonPlus;
                    if(uiNode == null){
                        throw new Exception("ERROR : 为获取所在 UINode 对象 - " + DisplayUtils.getDisplayPath(transform));
                    }
                    btnPlus.onDoubleClick.AddListener(()=> {
                        uiNode.onDoubleClick(gameObject.name);
                    });
                    btnPlus.onPress.AddListener(()=> {
                        uiNode.onPress(gameObject.name);
                    });
                }
                return _btnPlus;
            }
        }
        public override void frameUpdate(){
            base.frameUpdate();
            btnPlus.frameUpdate();
        }
        protected override void onBtn(){
            uiNode.onBtn(gameObject.name);
        }
        protected void OnDestroy(){
            if(isDestroyed){ return; }
            if(_btnPlus != null){
                btnPlus.onDoubleClick.RemoveAllListeners();
                btnPlus.onPress.RemoveAllListeners();
                _btnPlus = null;
            }
            base.OnDestroy();
        }
    }
}
