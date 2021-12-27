using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
namespace DataUIBinder{
    /*
        x:temp.x*100
            x轴上的点，随x变化
        y:temp.x*100
            y轴上的点，随x变化
        x:temp.x*100,y:temp.x*100
            45度的直线，随x变化
        temp.x*temp.x<temp.x
            x值的平方和x值比较大小（x平方抛物线和45度直线的焦点处变化）
        sy:((temp.x-1)*temp.x)/10*100
            调整scaleY的大小，显示x方和x的差的长度（方形的大小为10x10所以要除10）
        x:((x/100)+(temp.x-(x/100))*(0.11-temp.factor))*100,y:((y/100)+(temp.x*temp.x-(y/100))*(0.11-temp.factor))*100
            缓动公式 : next = current + (target - current) * factor
            x:((x/100)+(temp.x-(x/100))*(0.11-temp.factor))*100
                x 轴 目标是 x
            y:((y/100)+(temp.x*temp.x-(y/100))*(0.11-temp.factor))*100
                y 轴 目标是 x平方
    */
    public class ExpressionTest : UINode{
        private bool isAdd = true;
        protected override void Awake(){
            DataCenter.defalutInit();
            uiPath = "ui.ExpressionTest";
            base.Awake();
            //不是通过UIManager创建，需要手动进行一次显示数据绑定
            this.resetDataUIBind();
        }
        void Start(){
            dc.sv("temp.x",0.001);//设置x的初始值
            dc.sv("temp.factor",0);//缓动系数
        }
        // public override void OnDestroy(){
        //     base.OnDestroy();
        // }
        void Update(){
            DataCenter.frameUpdate();//更新数据中心
            ComponentWrapper.doFrameUpdate();
            //x值在[0，1.5]之间摇摆
            double _x = dc.gv("temp.x").AsDouble;
            if(isAdd){
                _x += 0.005;
                if(_x > 1.5){
                    isAdd = false;
                }
            }else{
                _x -= 0.005;
                if(_x < 0){
                    isAdd = true;
                }
            }
            dc.sv("temp.x", _x);
        }
    }
}
