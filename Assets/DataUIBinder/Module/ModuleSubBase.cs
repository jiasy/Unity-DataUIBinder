using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataUIBinder {
    public class ModuleSubBase : BaseObj,IUpdateAble {
        public ModuleBase belongToModule;
        public ModuleSubBase(ModuleBase belongToModule_) : base() {
            belongToModule = belongToModule_;
        }
        public override void Dispose() {
            if(belongToModule._disposed) {
                throw new Exception(fullClassName + " Dispose 调用时间，所在模块应当已经调用了base.Dispose()");
            }
            base.Dispose();
        }
        public virtual void frameUpdate(float dt_) {

        }
    }
}