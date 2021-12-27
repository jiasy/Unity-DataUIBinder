using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace DataUIBinder{
	public class UIManager : MonoBehaviour {
        public static void frameUpdate(){
            _instance.doFrameUpdate();
        }
		private static UIManager _instance;
		public static UIManager instance{
			get{
				if (_instance == null) {
					Debug.LogError ("ERROR " + System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName + " -> " + new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name + " : " +
						"instance is not created."
					);
				}
				return _instance;
			}
			set{
				if (_instance != null) {
					Debug.LogError ("ERROR " + System.Reflection.MethodBase.GetCurrentMethod().ReflectedType.FullName + " -> " + new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name + " : " +
						"instance is already exist."
					);
				}
				_instance = value;
			}
		}		
		BaseContainer baseContainer = null;//基础。只有一层。独立的游戏体。-0
		FloatContainer floatContainer = null;//悬浮UI。只有一层。各个模块的主入口。-1 为1，Base,Float可点
		PopContainer popContainer = null;//弹出层。UI的主要容器，需要层级管理。-2
		MaskContainer maskContainer = null;//遮罩层，只有一层，封锁用户操作用。-3
		GuideContainer guideContainer = null;//引导层，和遮罩层一起使用。-4
		TipContainer tipContainer = null;//提示层，弹出框，ok/cancel 或则 ok。-5
		LoadingContainer loadingContainer = null;//加载提示，网络断线重连提示。-6
		NoticeContainer noticeContainer = null;//漂浮文字，没有按键相应。-7
		DebugContainer debugContainer = null;//调试层，只有一层，相关的框或者连线等。-8
		private List<UIContainer> uiContainerList = new List<UIContainer>();
		public List<UIMain> _uiMainClosingList = new List<UIMain> ();
		public UIConfig uiConfig;
		void Awake(){
			instance = this;
			//framePerSecond = 20;
			Transform _trans = transform;
			baseContainer = initContainer<BaseContainer>(nameof(UIType.Base),_trans);
			floatContainer = initContainer<FloatContainer>(nameof(UIType.Float),_trans);
			popContainer = initContainer<PopContainer>(nameof(UIType.Pop),_trans);
			maskContainer = initContainer<MaskContainer>(nameof(UIType.Mask),_trans);
			guideContainer = initContainer<GuideContainer>(nameof(UIType.Guide),_trans);
			tipContainer = initContainer<TipContainer>(nameof(UIType.Tip),_trans);
			loadingContainer = initContainer<LoadingContainer>(nameof(UIType.Loading),_trans);
			noticeContainer = initContainer<NoticeContainer>(nameof(UIType.Notice),_trans);
			debugContainer = initContainer<DebugContainer>(nameof(UIType.Debug),_trans);
			uiConfig = gameObject.AddComponent<UIConfig>();
		}
		private T initContainer<T>(string uiName_,Transform trans_) where T :UIContainer{
			T _container = DisplayUtils.getUIGameObjectAndAddTo(uiName_,trans_).AddComponent<T>();
			(_container.transform as RectTransform).setTopBottomLefRight(0,0,0,0);
			uiContainerList.Add(_container);
			return _container;
		}
		// 初始化
		void Start () {
			
		}
		// private float _dt = 0.0f; // 帧时间累计
		// private float _frameUpdateDt; //按照每秒帧数，计算每帧时间
		// private int _framePerSecond;
		// public int framePerSecond {
		// 	get {
		// 		return _framePerSecond;
		// 	}
		// 	set {
		// 		_framePerSecond = value;
		// 		_frameUpdateDt = 1.0f / (float) _framePerSecond;
		// 	}
		// }
		// 帧调用
		public void doFrameUpdate () {
			// if (!updateTime()){
			// 	return;
			// }
			updateCheckDestroy();
			updateUIContainers();
			UIItem.doFrameUpdate();
			ComponentWrapper.doFrameUpdate();
		}
		// private bool updateTime(){
		// 	_dt = _dt + Time.deltaTime;
		// 	if (_dt > _frameUpdateDt) {
		// 		//超过的部分要从下一帧的时间间隔内刨除
		// 		_dt = -(_dt - _frameUpdateDt);
		// 	}else{
		// 		return false;
		// 	}
		// 	return true;
		// }
		private void updateCheckDestroy(){
			int _length = _uiMainClosingList.Count;
			for (int _idx = 0;_idx < _length;_idx++) {
				UIMain _uiMain = _uiMainClosingList[_idx];
				_uiMain.frameUpdate();
				if (_uiMain.state == UIState.Destroy){
					Destroy(_uiMain.gameObject);
					_uiMainClosingList.RemoveAt(_idx);
					_idx = _idx - 1;
					_length = _length - 1;
				}
			}
		}
		private void updateUIContainers(){
			for (int _idx = 0;_idx < uiContainerList.Count;_idx++) {
                uiContainerList[_idx].frameUpdate();
			}
		}
		public void destroyInNextFrame( UIMain uiMain_ ){
			_uiMainClosingList.Add(uiMain_);
		}
		public UIContainer getContainer(UIType type_) { 
			if(type_ == UIType.Base){return baseContainer;}
			else if(type_ == UIType.Float){return floatContainer;}
			else if(type_ == UIType.Pop){return popContainer;}
			else if(type_ == UIType.Mask){return maskContainer;}
			else if(type_ == UIType.Guide){return guideContainer;}
			else if(type_ == UIType.Tip){return tipContainer;}
			else if(type_ == UIType.Loading){return loadingContainer;}
			else if(type_ == UIType.Notice){return noticeContainer;}
			else if(type_ == UIType.Debug){return debugContainer;}
			else{
				Debug.LogError ("ERROR " + System.Reflection.MethodBase.GetCurrentMethod ().ReflectedType.FullName + " -> " + new System.Diagnostics.StackTrace ().GetFrame (0).GetMethod ().Name + " : " +
                    "UIType 设置错误"
                );
				return null;
			}
		}
		//打开UI
		public UIMain openUI( string abPackageOrFolderPath_ ,string uiName_ ,string dtPath_,UIType type_ = UIType.Pop ,ResLoadType resLoadType_ = ResLoadType.None ){
			GameObject _gameObject = ResUtils.getPrefab(abPackageOrFolderPath_,uiName_,resLoadType_);
			return getContainer(type_).openUI(_gameObject,uiName_,dtPath_);
		}
		//打开UI
		public UIMain openUI( string uiName_,string dtPath_ = null){
			UIInfo _uiInfo = UIConfig.instance.getUIInfo(uiName_);//配置中存在的，根据配置加载
			return openUI(_uiInfo.folderPath,uiName_,dtPath_,_uiInfo.type,_uiInfo.loadType);
		}
		//关闭UI
		public bool closeUI( string uiName_ ,bool force_ = true){
			for (int _idx = 0;_idx < uiContainerList.Count;_idx++) {
                if(uiContainerList[_idx].closeUI(uiName_,force_)){
					return true;
				}
            }
			return false;
		}
		//通过名称获取 UI
		public UIMain getUI( string uiName_ ){
			UIMain _uiMain = null;
			for (int _idx = 0;_idx < uiContainerList.Count;_idx++) {
                _uiMain = uiContainerList[_idx].getUI(uiName_);
				if(_uiMain !=null){
					return _uiMain;
				}
            }
			return null;
		}
		//关闭 Base Pop Tip 层的内容
		public void closeAllBasePopTip(){
			baseContainer.closeUIAll();
			popContainer.closeUIAll();
			tipContainer.closeUIAll();
		}
		//重置排序
		public void reSortOrder(){
			int _curSortingOrder = 0;//当前的层级
			for (int _idx = 0; _idx < uiContainerList.Count; _idx++) {
				_curSortingOrder = uiContainerList[_idx].reSortOrder(_curSortingOrder);
			}
		}
	}
}