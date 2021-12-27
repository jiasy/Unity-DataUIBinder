using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
 
public class ButtonPlus : Button,IUpdateAble{
    //支持长按和双击的按钮
    protected ButtonPlus(){
        onDoubleClick = new ButtonClickedEvent();
        onPress = new ButtonClickedEvent();
    }
    public ButtonClickedEvent onPress;
    public ButtonClickedEvent onDoubleClick;

    private bool isDowning = false;
    private float downTime = 0f;
    private float pressTriggerTime = 0.8f;
    private float pressTriggerIntervalTime = 0.1f;
    private bool pressTigger = false;
    public void frameUpdate(){
        if(pressTigger){
            if(Time.time > downTime + pressTriggerIntervalTime){
                downTime = Time.time;
                onPress.Invoke();
            } 
        }
        if (isDowning){
            if(!pressTigger){
                if (Time.time > downTime + pressTriggerTime){
                    pressTigger = true;
                    isDowning = false;
                    onPress.Invoke();
                    downTime = Time.time;
                }
            }
        }
    }
    public override void OnPointerDown(PointerEventData evt_){
        base.OnPointerDown(evt_);
        downTime = Time.time;
        isDowning = true;
        pressTigger = false;
    }
    public void backToNormal(){
        isDowning = false;
        pressTigger = false;
    }
    public override void OnPointerUp(PointerEventData evt_){
        base.OnPointerUp(evt_);
        backToNormal();
    }
    public override void OnPointerExit(PointerEventData evt_){
        base.OnPointerExit(evt_);
        backToNormal();
    }
    public override void OnPointerClick(PointerEventData evt_){
        if (!pressTigger){
            if (evt_.clickCount == 2 ){
                onDoubleClick.Invoke();//双击
            }else if (evt_.clickCount == 1 ){
                onClick.Invoke();//单击
            }
        }
    }
}