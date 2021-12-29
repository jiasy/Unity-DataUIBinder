using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// 通过字符串进行驱动，可以方便lua调用
namespace DataUIBinder {
    public class ModuleManager : BaseObj,IUpdateAble {
        private string nameSpace = "DEFAULT";
        private int currentCombinModuleTimes = 0;
        private List<ModuleBase> currentRunningModuleList = new List<ModuleBase>();
        private List<ModuleBase> justNewModuleList = new List<ModuleBase>();

        public ModuleManager(string nameSpace_) : base() {
            nameSpace = nameSpace_;
        }

        public override void Dispose() {
            base.Dispose();
        }

        //通过模块名称，添加运行模块
        public ModuleBase addModuleByName(string moduleName_) {
            ModuleBase _module = createModuleByName(moduleName_);
            currentRunningModuleList.Add(_module);
            return _module;
        }

        //通过模块名移除模块
        public ModuleBase removeModuleByName(string moduleName_) {
            ModuleBase _module = getModuleByName(moduleName_);
            if(_module != null) { //找到就移除，并返回
                currentRunningModuleList.Remove(_module); //移除这个模块
                return _module;
            } else { //找不到返回空
                return null;
            }
        }

        //通过模块名称创建模块
        public ModuleBase createModuleByName(string moduleName_) {
            ModuleBase _module =(ModuleBase) TypeUtils.getObjectByClassName(nameSpace + "."+moduleName_,new object[]{moduleName_});
            justNewModuleList.Add(_module);
            return _module;
        }

        //通过名称获取当前运行的模块
        public ModuleBase getModuleByName(string moduleName_) {
            for(int _idx = 0; _idx < currentRunningModuleList.Count; _idx++) {
                ModuleBase _module = currentRunningModuleList[_idx];
                if(_module.moduleName == moduleName_) {
                    return _module;
                }
            }
            return null;
        }

        // 通过当前目标的模块列表，清理掉 目标模块组合中，不存在的模块
        public List<ModuleBase> switchRunningModules(List<string> moduleNameList_) {
            //重新组合的次数加一
            currentCombinModuleTimes = currentCombinModuleTimes + 1;
            //当前正在运行的模块名称集合
            List<string> _runningModuleNames = currentRunningModuleList.Select(_runningModule => { return _runningModule.moduleName; }).ToList();
            //当前要切换的模块名称集合
            List<string> _targetModuleNames = moduleNameList_;
            //不在要切换的目标内的当前模块
            List<string> _removeModuleNames = _runningModuleNames.Except(_targetModuleNames).ToList();
            //获取要移除的模块
            List<ModuleBase> _removeModules = _removeModuleNames.Select(_removeModuleName => { return removeModuleByName(_removeModuleName); }).ToList();
            //实际执行移除操作
            _removeModules.ForEach(_removeModule => _removeModule.destory());
            //在切换目标内但不在正在运行的模块集合
            List<string> _createModuleList = _targetModuleNames.Except(_runningModuleNames).ToList();
            //创建并添加模块
            List<ModuleBase> _createModules = _createModuleList.Select(_createModuleName => { return addModuleByName(_createModuleName); }).ToList();
            //返回创建出来的数组
            return _createModules;
        }
        public void frameUpdate(float dt_) {
            while(justNewModuleList.Count > 0) {
                justNewModuleList[0].create();
                justNewModuleList.RemoveAt(0);
            }
            for(int _idx = 0; _idx < currentRunningModuleList.Count; _idx++) {
                currentRunningModuleList[_idx].frameUpdate(dt_);
            }
        }
    }
}