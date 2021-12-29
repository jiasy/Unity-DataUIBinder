using UnityEngine;

namespace SampleGame {
    public class InputTest : DataUIBinder.ModuleBase {
        public InputTest(string moduleName_) : base(moduleName_) {
            Debug.Log("ModuleBase " + moduleName_ + " New");
        }

        public override void create() {
            base.create();
            Debug.Log("ModuleBase " + moduleName + " Create");
            //设置模块数据
            sv("name","");
            sv("time","");
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