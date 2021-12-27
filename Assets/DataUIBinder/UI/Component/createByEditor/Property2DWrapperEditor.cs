using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
namespace DataUIBinder{
    /*
        通过编辑器来添加属性绑定
            在编辑器中填写属性要绑定的数据路径即可
    */
    public class Property2DWrapperEditor : Property2DWrapper{
        void Start() {
            UINode _uiNode = gameObject.GetComponentInParent(typeof(UINode)) as UINode;
            string _pattern = simulatePattern();
#if UNITY_EDITOR
            gameObject.name = _pattern;
#endif
            reset(_pattern,_uiNode);
        }
        private string simulatePattern(){
            StringBuilder _sb = CSharpExtensionUtils.SBInstance;
            _sb.Clear();
            List<string> _tempList = new List<string>();
            if(!xPathOrExpression.isNullOrEmpty()){
                _sb.Append("x:");
                _sb.Append(xPathOrExpression);
                _tempList.Add(_sb.ToString());
                _sb.Clear();
            }
            if(!yPathOrExpression.isNullOrEmpty()){
                _sb.Append("y:");
                _sb.Append(yPathOrExpression);
                _tempList.Add(_sb.ToString());
                _sb.Clear();
            }
            if(!scaleXPathOrExpression.isNullOrEmpty()){
                _sb.Append("sx:");
                _sb.Append(scaleXPathOrExpression);
                _tempList.Add(_sb.ToString());
                _sb.Clear();
            }
            if(!scaleYPathOrExpression.isNullOrEmpty()){
                _sb.Append("sy:");
                _sb.Append(scaleYPathOrExpression);
                _tempList.Add(_sb.ToString());
                _sb.Clear();
            }
            if(!rotationPathOrExpression.isNullOrEmpty()){
                _sb.Append("r:");
                _sb.Append(rotationPathOrExpression);
                _tempList.Add(_sb.ToString());
                _sb.Clear();
            }
            if(!alphaPathOrExpression.isNullOrEmpty()){
                _sb.Append("a:");
                _sb.Append(alphaPathOrExpression);
                _tempList.Add(_sb.ToString());
                _sb.Clear();
            }
            if(!widthPathOrExpression.isNullOrEmpty()){
                _sb.Append("w:");
                _sb.Append(widthPathOrExpression);
                _tempList.Add(_sb.ToString());
                _sb.Clear();
            }
            if(!heightPathOrExpression.isNullOrEmpty()){
                _sb.Append("h:");
                _sb.Append(heightPathOrExpression);
                _tempList.Add(_sb.ToString());
                _sb.Clear();
            }
            if(_tempList.Count == 0){
                throw new Exception("ERROR : 没有填入任何属性绑定的路径地址");
            }
            return _tempList.joinBy(",");
        }
    }
}