using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DataUIBinder {
    public class GameBase : BaseObj,IUpdateAble {
        public string nameSpace = null;//游戏命名空间，同过命名空间来做隔离
        public ModuleManager moduleManager = null;
        public UIManager uiManager = null;
        // private float _dt = 0.0f; // 帧时间累计
        // private float _frameUpdateDt; //按照每秒帧数，计算每帧时间
        // private int _logicFramePerSecond;//每秒帧数
        // public int logicFramePerSecond {
        //     get {
        //         return _logicFramePerSecond;
        //     }
        //     set {//调整帧频时间
        //         _logicFramePerSecond = value;
        //         _frameUpdateDt = 1.0f /(float) _logicFramePerSecond;
        //     }
        // } // 帧数

        public GameBase(string nameSpace_,UIManager uiManager_,int logicFramePerSecond_ = 20) {
            nameSpace = nameSpace_;
            moduleManager = new ModuleManager(nameSpace_);// 模块管理器
            if (uiManager_ == null){
                throw new Exception("ERROR : UIManager 未指定");
            }
            uiManager = uiManager_;
            //logicFramePerSecond = logicFramePerSecond_; //逻辑更新帧数
            DataCenter.defalutInit();
        }
        public void frameUpdate(float dt_) {
            // _dt = _dt + dt_;
            // if(_dt > _frameUpdateDt) {
            DataCenter.frameUpdate(dt_);
            uiManager.frameUpdate(dt_);
            moduleManager.frameUpdate(dt_);//与上一帧的时间间隔。
            //     _dt = -(_dt - _frameUpdateDt);//超过的部分要从下一帧的时间间隔内刨除
            // }
        }
        public override void Dispose() {
            moduleManager.Dispose();
            moduleManager = null;
            uiManager = null;
            base.Dispose();
        }
    }
}