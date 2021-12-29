using UnityEngine;

namespace SampleGame {
    public class User : DataUIBinder.ModuleBase {
        UserLoginOut userLoginOut = null;
        public User(string moduleName_) : base(moduleName_) {
            Debug.Log("ModuleBase " + moduleName_ + " New");
        }

        public override void create() {
            base.create();
            Debug.Log("ModuleBase " + moduleName + " Create");
            //添加一个子模块，模拟登陆登出
            userLoginOut =(UserLoginOut)addSubModuleBySuffixName("LoginOut");
            userLoginOut.doLogin();
            userLoginOut.doLogOut();
        }

        public override void destory() {

            Debug.Log("ModuleBase " + moduleName + " Destory");
            base.destory();
        }

        public override void frameUpdate(float dt_) {
            base.frameUpdate(dt_);
        }
    }
}