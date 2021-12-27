using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DataUIBinder{
	public class MathUtils {
        public static bool approximatelyEqual(float valueA_,float valueB_,float diff_ = 0.001f){
            if(Mathf.Abs(valueA_ - valueB_) < diff_){
                return true;
            }
            return false;
        }
        public static bool approximatelyEqual(Vector3 valueA_,Vector3 valueB_,float diff_ = 0.001f){
            float dx = valueA_.x - valueB_.x;
            float dy = valueA_.y - valueB_.y;
            return (dx * dx + dy * dy) < diff_ * diff_;
        }
	}
}