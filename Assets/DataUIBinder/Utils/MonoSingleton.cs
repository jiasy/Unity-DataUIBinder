using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DataUIBinder{
    /*
        public class ExtendClass : MonoSingleton<ExtendClass>{
            void Awake(){
                ...
            }
        }
    */
	public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour {
		private static T _instance;
		private static Transform _transform;
		public static T instance {
			get {
				if ( _instance == null ) {
					GameObject _go = null;
					if(_transform == null){
						_go = new GameObject("MonoSingleton");
						_transform = _go.transform;
					}
					_go = _transform.gameObject;
					_instance = _go.AddComponent<T>();
					DontDestroyOnLoad(_go);
				}
				return _instance;
			}
		}
	}
}