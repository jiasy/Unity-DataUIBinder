using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;

public class CSharpExtensionUtils{
    
    [ThreadStatic]
    private static List<String> escapeStrList;
    internal static List<String> StrListInstance{
        get{
            if (escapeStrList == null){
                escapeStrList = new List<String>();
            }
            return escapeStrList;
        }
    }
    [ThreadStatic]
    private static DataTable escapeDataTable;
    internal static DataTable DTInstance{
        get{
            if (escapeDataTable == null){
                escapeDataTable = new DataTable();
            }
            return escapeDataTable;
        }
    }
    [ThreadStatic]
    private static StringBuilder escapeBuilder;
    internal static StringBuilder SBInstance{
        get{
            if (escapeBuilder == null){
                escapeBuilder = new StringBuilder(128);
            }
            return escapeBuilder;
        }
    }
}
public static class CShapeExtension{
    // LIST Extends
    public static void print<T>(this List<T> thisList_,string prefix_){
        List<string> _toStringList = new List<string>();
        for (int _idx = 0; _idx < thisList_.Count; _idx++) {
            _toStringList.Add(thisList_[_idx].ToString());
        }
        UnityEngine.Debug.Log(prefix_ + _toStringList.joinBy(",").ToString()); 
    }
    public static T pull<T>(this List<T> thisList_){
        if(thisList_.Count > 0){
            int _lastIdx = thisList_.Count - 1;
            T _lastItem = thisList_[_lastIdx];
            thisList_.RemoveAt(_lastIdx);
            return _lastItem;
        }
        return default(T);
    }
    public static T getLast<T>(this List<T> thisList_){
        int _count = thisList_.Count;
        if(_count > 0){
            return thisList_[_count - 1];
        }
        return default(T);
    }
    public static List<T> referenceClone<T>(this List<T> thisList_){
        List<T> _referenceCloneList = new List<T>();
        for (int _idx = 0; _idx < thisList_.Count; _idx++) {
	        _referenceCloneList.Add(thisList_[_idx]);
        }
        return _referenceCloneList;
    }
    public static void linkOtherList<T>(this List<T> thisList_,List<T> otherList_,bool clearOther_){
        for (int _idx = 0; _idx < otherList_.Count; _idx++) {
	        thisList_.Add(otherList_[_idx]);
        }
        if(clearOther_){
            otherList_.Clear();
        }
    }
    public static string joinStr(this List<string> strListThis_){
        string _backStr = "";
        for (int _idx = 0; _idx < strListThis_.Count; _idx++) {
            _backStr += strListThis_[_idx];
        }
        return _backStr;
    }
    public static string joinBy(this List<string> strListThis_,string linkStr_){
        return string.Join(linkStr_, strListThis_);
    }
    // Numver Extends
    public static bool approximateTo(this float v1_,float v2_,float range_ = 0.01f){
        if(Mathf.Abs(v1_ - v2_) < range_){
            return true;
        }
        return false;
    }
    public static string toIdxKey(this int intThis_){
        StringBuilder _sb = CSharpExtensionUtils.SBInstance;
        _sb.Clear();
        _sb.Append('[');
        _sb.Append(intThis_);
        _sb.Append(']');
        string _backString = _sb.ToString();
        _sb.Clear();
        return _backString;
    }
    // Char Extends
    public static bool isSymbol(this char thisChar_){
        if(thisChar_ == '('||thisChar_ == ')'||thisChar_ == '+'||thisChar_ == '-'||thisChar_ == '*'||thisChar_ == '/'||thisChar_ == '%'){
            return true;
        }
        return false;
    }
    public static bool isNumber(this char thisChar_){
        if(thisChar_ == '0'||thisChar_ == '1'||thisChar_ == '2'||thisChar_ == '3'||thisChar_ == '4'||thisChar_ == '5'||thisChar_ == '6'||thisChar_ == '7'||thisChar_ == '8'||thisChar_ == '9'){
            return true;
        }
        return false;
    }
    // String Extends
    public static bool isExpressionStyle(this string stringThis_){
		int _strLength = stringThis_.Length;
		int _strIdx = 0;
		while(_strIdx < _strLength){
            //()+-*/%
            if(stringThis_[_strIdx].isSymbol()){
                return true;
            }
            _strIdx++;
        }
        return false;
    }
    public static string dotJoin(this string stringThis_,string childName_){
        string _currentDataPath = null;
        var _sb = JSONNode.EscapeBuilder;
        _sb.Append(stringThis_);
        _sb.Append('.');
        _sb.Append(childName_);
        _currentDataPath = _sb.ToString();
        _sb.Clear();
        return _currentDataPath;
    }
    public static List<string> getExpressionList(this string stringThis_){
		int _strLength = stringThis_.Length;
		int _strIdx = 0;
		int _lastType = -1;
		int _currentType = -1;
		List<string> _splitExpressionList = new List<string>();
		StringBuilder _sb = null;
		while(true){
			char _char = stringThis_[_strIdx];
			if(_char.isNumber()){
				if(_lastType == 3){
					_currentType = 3;
				}else{
					_currentType = 1;
				}
			}else if(_char.isSymbol()){
				_currentType = 2;
			}else{
				if(_lastType == 1 && _char == '.'){
					_currentType = 1;
				}else{
					_currentType = 3;
				}
			}
			if(_currentType != _lastType && (_currentType == 3||_lastType == 3)){
				if(_sb != null){
					_splitExpressionList.Add(_sb.ToString());
					_sb.Clear();
				}else{
					if(_currentType == 3){
						_splitExpressionList.Add("");
					}
				}
				_sb = new StringBuilder();
			}
            if(_lastType == -1){
                _sb = new StringBuilder();
            }
			_sb.Append(_char);
			_lastType = _currentType;
			_strIdx++;
            if(_strIdx >= _strLength){
                _splitExpressionList.Add(_sb.ToString());
                _sb.Clear();
                _sb = null;
                break;
            }
		}
        return _splitExpressionList;
    }
    public static object getExpressionResult(this string stringThis_){
        DataTable _dt = CSharpExtensionUtils.DTInstance;
        return _dt.Compute(stringThis_,"");
    }
    // value1 ==|!=|>|>=|<|<= value2
    public static bool isCompareStyle(this string stringThis_){
        if(stringThis_.Contains("==")||stringThis_.Contains("!=")||stringThis_.Contains(">")||stringThis_.Contains(">=")||stringThis_.Contains("<")||stringThis_.Contains("<=")){
            return true;
        }
        return false;
    }
    public static string[] getCompareArray(this string stringThis_){
        return stringThis_.Split(new string[] { "==","!=",">=",">","<=","<" }, StringSplitOptions.None);
    }
    //value:[min,max] value:[min,max) value:(min,max] value:(min,max)
    public static bool isRangeCompareStyle(this string stringThis_){
        if(( stringThis_.IndexOf(":[",0,StringComparison.Ordinal) > 0 || stringThis_.IndexOf(":(",0,StringComparison.Ordinal) > 0 ) && ( stringThis_.IndexOf(']') == (stringThis_.Length - 1) || stringThis_.IndexOf(')') == (stringThis_.Length - 1) ) && stringThis_.IndexOf(',') > 0){
            return true;
        }
        return false;
    }
    public static string[] getRangeCompareArray(this string stringThis_){
        string[] _splitArr = stringThis_.Split(new string[] { ":[",":(","," }, StringSplitOptions.None);
        string[] _backStringArr = new string[3];
        for (int _idx = 0; _idx < _backStringArr.Length; _idx++) {
            _backStringArr[_idx] = _splitArr[_idx];
        }
        _backStringArr[2] = _backStringArr[2].Substring(0,_backStringArr[2].Length - 1);
        return _backStringArr;
    }
    //${value}
    public static bool isPathAndStrMixedStyle(this string stringThis_){
        int _dollarIdx = stringThis_.IndexOf("${",0,StringComparison.Ordinal);
        if(_dollarIdx >= 0 && stringThis_.IndexOf('}') > _dollarIdx){
            return true;
        }
        return false;
    }
    public static string[] getPathAndStrMixedArray(this string stringThis_){
        return stringThis_.Split(new string[] { "${","}" }, StringSplitOptions.None);
    }
    public static bool isPropertyBindStyle(this string stringThis_){
        if(
            stringThis_.isStartsWith("x:")||
            stringThis_.isStartsWith("y:")||
            stringThis_.isStartsWith("sx:")||
            stringThis_.isStartsWith("sy:")||
            stringThis_.isStartsWith("r:")||
            stringThis_.isStartsWith("a:")
        ){
            return true;
        }
        return false;
    }
    public static string[] getPropertyBindArray(this string stringThis_){
        return stringThis_.Split(new string[] { ":","," }, StringSplitOptions.None);
    }
    public static string toSameLengthSpace(this string stringThis_){
        return "".PadLeft(stringThis_.Length);
    }
    public static bool isIdxKey(this string thisString_){
        if(thisString_[0] == '[' && thisString_[thisString_.Length - 1] == ']'){
            return true;
        }else{
            return false;
        }
    }
    public static string replaceLastChar(this string thisString_,string char_){
        if(char_.Length != 1){
            throw new Exception("ERROR : add string as char.");
        }
        return thisString_.Substring(0,thisString_.Length - 1) + char_;
    }
    public static int asKeyToIdx(this string thisString_){
        return thisString_.Substring(1,thisString_.Length - 2).toInt();
    }
    public static bool isNullOrEmpty(this string thisString_){
        return string.IsNullOrEmpty(thisString_);
    }
    public static bool isStartsWith(this string thisString_,string checkPrefix_){
        if(thisString_.Length >= checkPrefix_.Length && thisString_.IndexOf(checkPrefix_,StringComparison.Ordinal) == 0){
            return true;
        }
        return false;
    }
    
    public static bool isEndsWith(this string thisString_,string checkSuffix_){
        int _checkSuffixLength = checkSuffix_.Length;
        if(thisString_.Length >= _checkSuffixLength && thisString_.Substring(thisString_.Length - _checkSuffixLength,_checkSuffixLength).Equals(checkSuffix_)){
            return true;
        }
        return false;
    }
    public static string[] splitWith(this string thisString_,string splitStr_) {
        string[] _splitResultArr;
        if (thisString_.Contains(splitStr_)) {
            _splitResultArr = thisString_.Split(splitStr_.ToCharArray(), StringSplitOptions.None);
        } else {
            _splitResultArr = new string[1]{thisString_};
        }
        return _splitResultArr;
    }
    public static bool isInt (this string thisString_) {
        return Regex.IsMatch (thisString_, @"^\d+$");
    }
    public static int toInt (this string thisString_) {
        return int.Parse(thisString_);
    }
}
