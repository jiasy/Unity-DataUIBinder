using System;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace DataUIBinder{
//同一类型的界面在一个层级，集中管理
	public class UIContainer : MonoBehaviour,IUpdateAble {
		public UIType uiType = UIType.None;
		public Dictionary<string,UIMain> uiDict = new Dictionary<string,UIMain>();
		public List<UIMain> uiList = new List<UIMain> ();
		public virtual void Awake () {
			
		}
		public virtual void Start () {
			
		}
		//帧更新集中处理
		public virtual void frameUpdate (float dt_) {
			int _listLength = uiList.Count;
			for (int _idx = 0;_idx < _listLength;_idx++) {
				UIMain _uiMain = uiList[_idx];
				_uiMain.frameUpdate(dt_);
			} 
		}
		// 重置 sortingOrder
		public int reSortOrder(int curSortingOrder_){
			int _listLength = uiList.Count;
			for (int _idx = 0; _idx < _listLength; _idx++) {
				UIMain _uiMain = uiList[_idx];
				_uiMain.sortingOrder = curSortingOrder_ + _uiMain.rectTrans.GetSiblingIndex();
			}
			return curSortingOrder_ + _listLength;
		}
		public virtual UIMain openUI(GameObject gameObject_,string uiName_,string dataPath_ = null){
			UIMain _uiMain = gameObject_.GetComponent<UIMain>();
			if(_uiMain == null){
				throw new Exception("ERROR : 必须挂载 UIMain 的继承类作为UI的控制器 : " +uiName_);
			}
			_uiMain.uiName = uiName_;
			_uiMain.uiPath = "ui".dotJoin(uiName_);
			_uiMain.dtPath = dataPath_;
			uiDict.Add(_uiMain.uiName,_uiMain);
			uiList.Add(_uiMain);
			gameObject_.transform.SetParent(transform);
			gameObject_.transform.initPosAndScale();
			(gameObject_.transform as RectTransform).setTopBottomLefRight(0,0,0,0);
			_uiMain.resetDataUIBind();
			return _uiMain;
		}
		public UIMain getUI(string uiName_){
			UIMain _uiMain;
			if(uiDict.TryGetValue(uiName_, out _uiMain)){
				return _uiMain;
			}
			return null;
		}
		public bool closeUI(string uiName_,bool force_ = true){
			UIMain _uiMain = getUI(uiName_);
			if (_uiMain == null){ 
				return false; 
			}
			return closeUI(_uiMain,force_);
		}

		public bool closeUI(UIMain uiMain_,bool force_ = true){
			int _uiMainIndex = uiList.IndexOf(uiMain_);
			if (_uiMainIndex >= 0){
				uiMain_.closeUI(force_);
				uiDict.Remove(uiMain_.uiName);
				uiList.RemoveAt(_uiMainIndex);
				return true;
			}
			throw new Exception("ERROR " + System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName + " -> " + new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name + " : " +
				"字典和数组对用一个ui的关联不一致"
			);
			return false;
		}

		public bool closeUITop(bool force_){
			UIMain _uiMain = uiList.getLast();
			if (_uiMain == null){ 
				return false; 
			}
			return closeUI(_uiMain,force_);
		}

		public void closeUIAll(){
			while (closeUITop(true)){}
		}
	}
}