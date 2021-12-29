using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

namespace DataUIBinder {
    public class ModuleBase : BaseObj,IUpdateAble {
        public string moduleName = null;
        public string modulePath = null;
        private bool _isCreated = false;
        private bool _isDestory = false;
        private List<ModuleSubBase> subModules = null;
        public ModuleBase(string moduleName_) : base() {
            moduleName = moduleName_;
            modulePath = "module." + moduleName;
            if (DataCenter.root.getValue(modulePath) == null){//没有创建过的话，创建根节点数据路径。
                DataCenter.root.setValue(modulePath,new JSONObject());
            }
        }
        public override void Dispose() {
            if(subModules != null) {//同时销毁旗下所有
                for(int _idx = 0; _idx < subModules.Count; _idx++) {
                    ModuleSubBase _sub = subModules[_idx];
                    _sub.Dispose();
                }
            }
            DataCenter.root.clearKeyValueOnPath(modulePath);//清理模块内的键值数据。但是模块的根节点保存。
            base.Dispose();
        }
        //对象在创建后，下一帧统一create
        public virtual void create() {
            if(_isDestory) {
                throw new Exception(this.fullClassName + "is already destoryed ~ !");
            }
            if(_isCreated) {
                throw new Exception(this.fullClassName + "is already created ~ !");
            }
            _isCreated = true;
        }
        public virtual void destory() {
            if(!_isCreated) {//没有被disposed的才需要判断。
                throw new Exception(this.fullClassName + "is not created ~ !");
            } else {
                _isDestory = true;
                Dispose();
            }
        }
        public virtual void frameUpdate(float dt_) {
            if(subModules != null) {//同时更新
                for(int _idx = 0; _idx < subModules.Count; _idx++) {
                    subModules[_idx].frameUpdate(dt_);
                }
            }
        }
        //添加子模块
        public ModuleSubBase addSubModule(ModuleSubBase moduleSub_) {
            if(subModules == null) {
                subModules = new List<ModuleSubBase>();
            }
            if(subModules.Contains(moduleSub_)) {
                throw new Exception(fullClassName + " 中 addSubModule 当前模块已经存在 : " + moduleSub_.fullClassName);
            } else {
                subModules.Add(moduleSub_);
            }
            return moduleSub_;
        }

        //移除子模块
        public ModuleSubBase removeSubModule(ModuleSubBase moduleSub_) {
            if(subModules.Contains(moduleSub_)) {
                return moduleSub_;
            } else {
                throw new Exception(fullClassName + " 中 removeSubModule 当前模块并不存在 : " + moduleSub_.fullClassName);
                return null;
            }
        }

        public ModuleSubBase getSubModule(string shortName_) {
            return getSubModuleByFullClassName(fullClassName + shortName_);
        }
        //通过类名获取模块，完整类名的后半部分
        public ModuleSubBase getSubModuleByFullClassName(string className_) {
            if(subModules == null) { //还没创建过subModules缓存，肯定就没有。
                return null;
            }
            for(int _idx = 0; _idx < subModules.Count; _idx++) {
                ModuleSubBase _sub = subModules[_idx];
                if(_sub.fullClassName == className_) {
                    return _sub;
                }

            }
            return null;
        }

        // 通过子模块后缀名，拼接出子模块名称
        public ModuleSubBase addSubModuleBySuffixName(string suffixName_) {
            ModuleSubBase _sub = getSubModuleByFullClassName(fullClassName + suffixName_);
            if(_sub != null) {
                throw new Exception(fullClassName + " addSubModuleBySuffixName : " + suffixName_ + " is already exist~!");
                return _sub;
            }
            //将自己作为构建对象的参数，传递给自模块
            object[] _parameters = new object[1];
            _parameters[0] = this;
            _sub =(ModuleSubBase) TypeUtils.getObjectByClassName(fullClassName + suffixName_, _parameters);
            return addSubModule(_sub);
        }
        public void sv(string key_,object value_){
            setValueOnDataPath(key_,value_);
        }
        private void setValueOnDataPath(string key_,object value_){
            StringBuilder _sb = CSharpExtensionUtils.SBInstance;
            _sb.Clear();
            _sb.Append(modulePath);
            _sb.Append('.');
            _sb.Append(key_);
            DataCenter.root.setValue(_sb.ToString(),value_);
            _sb.Clear();
        }
        public JSONNode gv(string key_){
            return getValueOnDataPath(key_);
        }
        private JSONNode getValueOnDataPath(string key_){
            StringBuilder _sb = CSharpExtensionUtils.SBInstance;
            _sb.Clear();
            _sb.Append(modulePath);
            _sb.Append('.');
            _sb.Append(key_);
            JSONNode _returnJSONNode = DataCenter.root.getValue(_sb.ToString());
            _sb.Clear();
            return _returnJSONNode;
        }
    }
}