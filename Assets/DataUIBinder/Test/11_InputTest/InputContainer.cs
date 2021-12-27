using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataUIBinder;
namespace DataUIBinder{
    public class InputContainer : MonoBehaviour{
        InputTest inputTest = null;
        void Awake() {
            DataCenter.defalutInit();
            dc.sv("module.InputTest.name","");
            dc.sv("module.InputTest.time","");
        }
        void Start() {
            GameObject _gameObject = ResUtils.getPrefab("InputTestPrefab/","InputTest",ResLoadType.Resources);
            inputTest = _gameObject.GetComponent<InputTest>();
			inputTest.uiName = "InputTest";
			inputTest.uiPath = "ui.InputTest";
			inputTest.dtPath = "module.InputTest";
			_gameObject.transform.SetParent(transform);
			_gameObject.transform.initPosAndScale();
			inputTest.rectTrans.setTopBottomLefRight(0,0,0,0);
            inputTest.resetDataUIBind();
        }
        void Update(){
            DataCenter.frameUpdate();
            UIItem.doFrameUpdate();
            ComponentWrapper.doFrameUpdate();
        }
    }
}