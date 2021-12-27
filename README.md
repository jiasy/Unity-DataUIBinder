DataUIBinder 是一个 DataBinding 框架，实现了数据和UI的双向绑定。
借鉴了VUE的双向绑定的概念，借鉴了Redis中任意数据结构都可以展开成键值对的概念。
将通过特殊命名的方法，在运行时将UI和数据进行绑定。

DataCenter 数据中心，在这里所有的结构都会被展开成键值对。
    配置 - config
    本地存储 - save
    设置 - settings
    服务器交互 - server
    模块 - module
    UI节点状态 - ui
    临时数据 - temp
    调试用 - debug
    原生交互 - native
    多国语言本地化 - localize
UIManager UI管理器，内置了9种常用的UI层级。
    Base - 基础
    Float - 悬浮
    Pop - 弹出层
    Mask - 遮罩层
    Guide - 引导层
    Tip - 提示层
    LoadingC - 加载提示
    Notice - 漂浮文字
    Debug - 调试层
UINode 为ui节点
    _ui_.xx - 关联uiNode对应的ui数据路径（DataCenter.ui 下的）。
    _dt_.xx - 关联uiNode挂载的数据路径。
Replay 交互组件封装，用户操作录制和播放
SimpleJson JSON库，在其基础上实现数据缓存和键值对展开。
Test 文件下有各种实例，以供参考...

有问题请联系 : jia_shiyang@sina.com