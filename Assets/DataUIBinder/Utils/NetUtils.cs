using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DataUIBinder{
	public class NetUtils {
        public static bool isNetAvailable(){
            if (UnityEngine.Application.internetReachability == UnityEngine.NetworkReachability.NotReachable){//没网打不开
                return false;
            }
            return true;
        }
	}
}