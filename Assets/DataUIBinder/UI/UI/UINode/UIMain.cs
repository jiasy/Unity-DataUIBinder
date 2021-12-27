using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace DataUIBinder{
    public class UIMain : UINode{
        public UIType uiType = UIType.None;
        private UIState _state;
        public UIState state { 
            get{
                return _state;
            }
            set{
                if(value != _state){
                    if(value == UIState.Open){
                        ui_sv("state",nameof(UIState.Open));
                    }else if(value == UIState.Close){
                        ui_sv("state",nameof(UIState.Close));
                    }else if(value == UIState.Destroy){
                        ui_sv("state",nameof(UIState.Destroy));
                    }
                    _state = value;
                }
            }
        }
        private Canvas canvas;
        private GraphicRaycaster raycaster;
        public int sortingOrder{
            get{
                return canvas.sortingOrder;
            }
            set{
                canvas.sortingOrder = value;
            }
        }
        protected override void Awake(){
            state = UIState.Open;
            base.Awake();
            canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            raycaster = gameObject.AddComponent<GraphicRaycaster>();
        }
        public override void onBtn(string btnName_){
            base.onBtn(btnName_);
        }
        public override void onPress(string btnName_){
            base.onPress(btnName_);
        }
        public override void onDoubleClick(string btnName_){
            base.onDoubleClick(btnName_);
        }
        public override void onCheck(string btnName_,string key_,bool isOn_){
            base.onCheck(btnName_,key_,isOn_);
        }
        public override void onToggle(string btnName_,string key_,string value_){
            base.onToggle(btnName_,key_,value_);
        }
        public void closeUI(bool forceClose_){
            if(forceClose_){
                state = UIState.Destroy;
                UIManager.instance.destroyInNextFrame(this);
            }else{//TODO 延时关闭
                state = UIState.Close;
            }
        }
        public void closeSelf(){
            UIManager.instance.closeUI(uiName);
        }
    }
}