using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataUIBinder;
//// - MoudleManager 是使用命名空间做隔离的，通过运行时动态解析命名空间，实现游戏代码的隔离（游戏插件实现成为可能）。
namespace SampleGame{
    [RequireComponent(typeof(UIManager))]
    public class ModuleManagerTest : MonoBehaviour{
        void Awake() {
            Game.instance =(Game)TypeUtils.getObjectByNameSpaceAndClassName(
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace,//当前的命名空间
                "Game",//GameBase 的具体实现
                new object[]{GetComponent<UIManager>()}
            );
            StartCoroutine(Game.instance.init());
		}
        void Start(){

        }
        void Update(){
            Game.instance.frameUpdate(Time.deltaTime);
        }
    }
}