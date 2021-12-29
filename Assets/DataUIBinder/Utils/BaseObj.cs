using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataUIBinder {
    public class BaseObj : IDisposable {
        public static Dictionary<string, List<BaseObj>> _runningObjDict = new Dictionary<string, List<BaseObj>> ();
        //运行BaseObj的子类集合--------------------------------------------------------------------------------------------------------------
        //当前类对应的运行时列表
        public static List<BaseObj> getRunningList (string fullClassName_) {
            List<BaseObj> _objList = null;
            if (!_runningObjDict.ContainsKey (fullClassName_)) {
                _objList = new List<BaseObj> ();
                _runningObjDict[fullClassName_] = _objList;
            } else {
                _objList = _runningObjDict[fullClassName_];
            }
            return _objList;
        }
        public static string runningInfo (bool _print = false) {
            string _infoStr = "";
            foreach (string _fullClassName in _runningObjDict.Keys) {
                List<BaseObj> _objList = getRunningList (_fullClassName);
                _infoStr = _infoStr + _fullClassName + " : " + _objList.Count.ToString () + System.Environment.NewLine;
            }
            if (_print) {
                Debug.Log ("当前运行状态如下" + System.Environment.NewLine + _infoStr);
            }
            return _infoStr;
        }
        public string fullClassName = null;//类全名
        public bool _disposed = false;//是否已经消除
        private List<BaseObj> _belongToRunningList = null;
        public BaseObj () {
            //获取当前子类的类名
            fullClassName = GetType ().Namespace + "." + GetType ().Name;
            //添加到运行时，获取所属的运行时列表，放入其中
            _belongToRunningList = getRunningList (fullClassName);
            _belongToRunningList.Add (this);
            //runningInfo(true);
        }
        ~BaseObj () { //析构
            //即使没有手动调用过 Dispose，在GC回收的时候，还是会调用一次。
            Dispose (false);
        }
        public virtual void Dispose () {
            Dispose (true);
            GC.SuppressFinalize (this); //标记gc不在调用析构函数
        }
        //调用自己的销毁
        public void Dispose (bool disposing) {
            if (_disposed) return; //如果已经被回收，就中断执行
            if (disposing) { //释放本对象中管理的托管资源
                //由 手动调用Dispose 释放
            } else {
                //由 析构 释放
            }
            //释放非托管资源
            _belongToRunningList.Remove (this);
            fullClassName = null;
            _disposed = true;
        }
    }
}