using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SampleGame {
    public class UserLoginOut : DataUIBinder.ModuleSubBase {
        public UserLoginOut(DataUIBinder.ModuleBase belongToModule_) : base(belongToModule_) {
            Debug.Log("ModuleSubBase " + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + " New");
        }

        public override void frameUpdate(float dt_) {
            base.frameUpdate(dt_);
        }

        public override void Dispose() {
            Debug.Log("ModuleSubBase " + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + " Dispose");
            base.Dispose();
        }

        public void doLogin() {
            Debug.Log("ModuleSubBase " + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + " -> doLogin");
        }
        public void doLogOut() {
            Debug.Log("ModuleSubBase " + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + " -> doLogOut");
        }
    }
}