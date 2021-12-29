using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
namespace DataUIBinder{
    public class UINodeTest : UINode{
        /*
            界面也有相应的数据路径进行关联。
                ui.界面名.enabled 是否处于显示状态，true/false
            字符串格式化，字符串配置格式 : '${数据路径[格式化模式]}'
                可以使用C#自带的数字格式化
                    格式化模式 可以填写 N1,N2 等等。
                自定义格式化，事例 customFormat，可以在 DataCenter.customFormatDict 拓展
                    ${this.max[customFormat]}
        */
        private bool isIntAdd{
            get{ return dc.gv("ui.UINodeTest.isIntAdd").AsBool; }
            set{ dc.sv("ui.UINodeTest.isIntAdd",value); }
        }
        private string picUrl{
            get{ return dc.gv("ui.UINodeTest.picUrl").AsString; }
            set{ dc.sv("ui.UINodeTest.picUrl",value); }
        }
        private int min{
            get{ return dc.gv("ui.UINodeTest.min").AsInt; }
            set{ dc.sv("ui.UINodeTest.min",value); }
        }
        private int current{
            get{ return dc.gv("ui.UINodeTest.current").AsInt; }
            set{ dc.sv("ui.UINodeTest.current",value); }
        }
        private int max{
            get{ return dc.gv("ui.UINodeTest.max").AsInt; }
            set{ dc.sv("ui.UINodeTest.max",value); }
        }
        protected override void Awake(){
            DataCenter.defalutInit();
            uiPath = "ui.UINodeTest";
            base.Awake();
            //不是通过UIManager创建，需要手动进行一次显示数据绑定
            this.resetDataUIBind();
        }
        void Start(){
            min = -100;
            max = 150;
            current = 0;
            isIntAdd = false;
            picUrl = "pics/starpic";
            //ui.UINodeTest.current
            onChange("ui.UINodeTest.current",(current_)=>{
                if(!isIntAdd && current_.AsInt <= min){
                    isIntAdd = true;
                }else if(isIntAdd && current_.AsInt >= max){
                    isIntAdd = false;
                }
            });
        }
        // public override void OnDestroy(){
        //     base.OnDestroy();
        // }
        void Update(){
            float _dt = Time.deltaTime;
            DataCenter.frameUpdate(_dt);
            ComponentWrapper.doFrameUpdate(_dt);
            current = isIntAdd?current+1:current-1;
        }
        public override void onBtn(string btnName_){
            if(btnName_ == "btn_switchIsIntAdd"){
                isIntAdd = !isIntAdd;
            }
            base.onBtn(btnName_);
        }
    }
}
