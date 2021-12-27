using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static partial class UnityExtends{
    public static Vector2 GetSize(this RectTransform thisRectTrans_){
        return thisRectTrans_.rect.size;
    }
    public static void setTopBottomLefRight(this RectTransform thisRectTrans_,float top_,float buttom_,float left_,float right_){
        thisRectTrans_.offsetMin = new Vector2(left_,buttom_);
        thisRectTrans_.offsetMax = new Vector2(right_,top_);
    }
    public static void setSize(this RectTransform thisRectTrans_, float width_, float height_){
        thisRectTrans_.setWidth(width_);
        thisRectTrans_.setHeight(height_);
    }
    public static void setSize(this RectTransform thisRectTrans_, Vector2 size_){
        thisRectTrans_.setWidth(size_.x);
        thisRectTrans_.setHeight(size_.y);
    }
    public static void setWidth(this RectTransform thisRectTrans_, float width_){
        thisRectTrans_.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width_);
    }
    public static void setHeight(this RectTransform thisRectTrans_, float height_){
        thisRectTrans_.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height_);
    }
    public static float getWidth(this RectTransform thisRectTrans_){
        return thisRectTrans_.rect.size.x;
    }
    public static float getHeight(this RectTransform thisRectTrans_){
        return thisRectTrans_.rect.size.y;
    }
    public static Vector3[] getCorners(this RectTransform thisRectTrans_, bool isWorldSpace_ = true){
        Vector3[] _corners = new Vector3[4];
        if (isWorldSpace_){
            thisRectTrans_.GetWorldCorners(_corners);
        }else{
            thisRectTrans_.GetLocalCorners(_corners);
        }
        return _corners;
    }
    public static Vector3 getLeftBottonCorner(this RectTransform thisRectTrans_, bool isWorldSpace_ = true){
        return thisRectTrans_.getCorners(isWorldSpace_)[0];
    }
    public static Vector3 getLeftTopCorner(this RectTransform thisRectTrans_, bool isWorldSpace_ = true){
        return thisRectTrans_.getCorners(isWorldSpace_)[1];
    }
    public static Vector3 getRightTopCorner(this RectTransform thisRectTrans_, bool isWorldSpace_ = true){
        return thisRectTrans_.getCorners(isWorldSpace_)[2];
    }
    public static Vector3 getRightBottonCorner(this RectTransform thisRectTrans_, bool isWorldSpace_ = true){
        return thisRectTrans_.getCorners(isWorldSpace_)[3];
    }
    public static void initPosAndScale(this Transform thisTrans_){
        thisTrans_.localPosition = Vector3.zero;
        thisTrans_.localScale = Vector3.one;
        thisTrans_.localRotation = Quaternion.identity;
    }
    public static void setX(this Transform thisTrans_, float x_, bool isWorldSpace_ = false){
        Vector3 _pos;
        if(isWorldSpace_){
            _pos = thisTrans_.position;
            _pos.x = x_;
            thisTrans_.position = _pos;
        }else{
            _pos = thisTrans_.localPosition;
            _pos.x = x_;
            thisTrans_.localPosition = _pos;
        }
    }
    public static void setY(this Transform thisTrans_, float y_, bool isWorldSpace_ = false){
        Vector3 _pos;
        if(isWorldSpace_){
            _pos = thisTrans_.position;
            _pos.y = y_;
            thisTrans_.position = _pos;
        }else{
            _pos = thisTrans_.localPosition;
            _pos.y = y_;
            thisTrans_.localPosition = _pos;
        }
    }
    public static void setXY(this Transform thisTrans_, float x_,float y_, bool isWorldSpace_ = false){
        Vector3 _pos;
        if(isWorldSpace_){
            _pos = thisTrans_.position;
            _pos.x = x_;
            _pos.y = y_;
            thisTrans_.position = _pos;
        }else{
            _pos = thisTrans_.localPosition;
            _pos.x = x_;
            _pos.y = y_;
            thisTrans_.localPosition = _pos;
        }
    }
    public static void setZ(this Transform thisTrans_, float z_, bool isWorldSpace_ = false){
        Vector3 _pos;
        if(isWorldSpace_){
            _pos = thisTrans_.position;
            _pos.z = z_;
            thisTrans_.position = _pos;
        }else{
            _pos = thisTrans_.localPosition;
            _pos.z = z_;
            thisTrans_.localPosition = _pos;
        }
    }
    public static void setScaleX(this Transform thisTrans_, float sx_){
        Vector3 _localScale = thisTrans_.localScale;
        _localScale.x = sx_;
        thisTrans_.localScale = _localScale;
    }
    public static void setScaleY(this Transform thisTrans_, float sy_){
        Vector3 _localScale = thisTrans_.localScale;
        _localScale.y = sy_;
        thisTrans_.localScale = _localScale;
    }
    public static void setScaleXY(this Transform thisTrans_, float sx_,float sy_){
        Vector3 _localScale = thisTrans_.localScale;
        _localScale.x = sx_;
        _localScale.y = sy_;
        thisTrans_.localScale = _localScale;
    }
    public static void setScale(this Transform thisTrans_, float sxy_){
        thisTrans_.setScaleXY(sxy_,sxy_);
    }
    public static void setRotation(this Transform thisTrans_, float rotation_){
        thisTrans_.localRotation = Quaternion.Euler(new Vector3(0, 0, rotation_));
    }
    public static float getRotation(this Transform thisTrans_){
        return thisTrans_.eulerAngles.z;
    }
}
