using UnityEngine;

namespace SampleGame {
    public class Base : DataUIBinder.ModuleBase {
        public Base(string moduleName_) : base(moduleName_) {
            Debug.Log("ModuleBase " + moduleName_ + " New");
            Game.instance.uiManager.openUI("BaseTest");
        }

        public override void create() {
            base.create();
            Debug.Log("ModuleBase " + moduleName + " Create");

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