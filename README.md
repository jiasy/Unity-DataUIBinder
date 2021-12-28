### DataUIBinder

----



#### 简介

`DataUIBinder` 是一个 DataBinding 框架，实现了数据和UI的双向绑定。
借鉴了`VUE`的双向绑定的概念，借鉴了`Redis`中任意数据结构都可以展开成键值对的概念。
将通过特殊命名的方法，在运行时将UI和数据进行绑定。
![111](https://user-images.githubusercontent.com/32121702/147549406-8b699d8a-df2b-4419-9c9f-fcd2acbfa481.png)




#### DataCenter - 数据中心

`DataCenter` 数据中心，在这里所有的结构都会被展开成键值对。

* **配置** - config
* **本地存储** - save
* **设置** - settings
* **服务器交互** - server
* **模块** - module
* **App或原生交互状态** - state
* **UI节点状态** - ui
* **临时数据** - temp
* **调试用** - debug
* **原生交互** - native
* **多国语言本地化** - localize   

#### DataPathDriven - 数据驱动，数据流触发事件分发，多米诺骨牌一样逐个驱动。

* **onChange** - 监听`数据路径`的变化。
* **dictToDict** - 字典和并到字典上，键值合并，相同覆盖。
* **listToDict** - 列表合并到字典中，通过列表元素某键的值映射成字典的键，将自己作为字典当前键的值合并到字典上。
* **listFromDict** - 通过字典信息丰富列表元素的信息，通过列表元素的某键的值对应查找到字典的键，将字典当前键的值合并到列表元素上（如服务器只下发道具的id列表，从本地配置字典中获取id对应的道具信息，形成道具信息列表）。
* **listToList** - 通过两个列表的列表元素的某键作为关联，将两者的匹配到列表元素合并。
* **listFilterSortToList** - 列表通过指定的过滤方法和排序方法生成新列表。

#### UIManager - UI 层级管理

`UIManager` UI管理器，内置了9种常用的UI层级。

* **Base** - 基础
* **Float** - 悬浮
* **Pop** - 弹出层
* **Mask** - 遮罩层
* **Guide** - 引导层
* **Tip** - 提示层
* **LoadingC** - 加载提示
* **Notice** - 漂浮文字
* **Debug** - 调试层

#### UINode - UI节点

`UINode` UI的基本单位，，，另一个是挂载的dtPath（如模块数据，ui销毁时不随之销毁）

* **UIMain** - 主UI
* **UISub** - 子UI
* **UIItem** - 列表元素UI

`Editor` 中，如何和`数据路径`做关联（持有两种数据路径）

* **\_ui\_.xx** - ui对应的uiPath（ui销毁时，随之销毁）
* **\_dt\_.xx** - ui挂载的dataPath（ui销毁时，不随之销毁）（如模块数据，在某个模块中创建UI）



#### CompontentWrapper - 组件包装器，进行dataBinding的实际操作。

前缀命名法，挂载`CompontentWrapper`（以下命名中`xx`并无特殊意义）

* btn_`xx` - 普通按钮 : 单击（`UINode`.`onBtn`）。
* btnPlus_`xx` - 进阶按钮 : 支持双击（`UINode`.`onDoubleClick`）长按（`UINode`.`onPress`）。
* check_`key` - 单选按钮 : 单击切换状态（`UINode`.`onCheck`），修改`ui.uiNode.key`的值为`true`/`false`。
* toggle\_`key`\_`value` - 互斥按钮 : 切换状态（`UINode`.`onToggle`），同时所在`key`的所有按钮状态自动变更，修改`ui.uiNode.key`的值为`value`。
* list_`xx` - 列表 : 承载`UIItem`的列表，管理其排布。
* scroll_`xx` - 滚动层 : 承载 list_`xx`的滚动层，通过帧更新控制其显示区。
* 其他，如txt_、img_...等等，前缀命名可以通过`UINode`\[节点名\]的方式直接获取其`transform`。

数据路径命名法，挂载`CompontentWrapper`

* **Text** - 文本 :  编辑名称为`数据路径`即可绑定（也可以编辑文本，通过指定格式${`数据路径`}做关联，见下文）。
* **Image** - 图片 : 编辑名称为`数据路径`即可绑定（如果`数据路径`中是http开头的，会去下载图片，缓存并显示）。
* **InputFiled** - 输入框 : 编辑名称为`数据路径`即可双向绑定。
* **Slider** - 滑块/进度条 : 编辑名称为`数据路径:[最小值,最大值]`（最大、最小值可替换成`数据路径`）即可双向绑定。
* 判断式命名(任意类型节点) : 编辑名称为`数据路径==值`即可绑定（`值`可以是`数`/`true`/`false`/`数据路径`）（`==`可以是`!=`/`>`/`>=`/`<`/`<=`）。
* 值域判断(任意类型节点) : 编辑名称为`数据路径:[最小值,最大值]`即可绑定（`[]`可替换成`()`，实现开闭区间的判断）（最大、最小值可替换成`数据路径`）。
* 属性关联 : 编辑名称为`x:数据路径,y:数据路径`即可将x,y属性与`数据路径`绑定。


编辑内容，挂载`CompontentWrapper`

* Text - 文本 : 编辑内容，通过指定格式`${数据路径}`做关联。

`数据路径`的特殊格式，用来对其进行二次加工

* 格式化，文本编辑成`${数据路径[格式化]}`就可以对`数据路径`进行`格式化`（也可以自定义格式化，请在事例中查阅`CustomFormat`相关内容）。
* 表达式，文本编辑成`${数据路径%2+1}`就可以对`数据路径`进行取2余数加1的操作，当`数据路径`变化时，作为整体获得结果。


#### Replay - 记录玩家操作（在此基础上，播放录像、新手引导、通用前端打点、漏斗分析等...）

* 录像会录制到`LogToFiles.logFolder`/`Record`文件中，包括sroll滚动也会记录。


#### 日志文件，除录像之外，对事件监听和数据变化也进行了跟踪。

* `数据路径`的变化信息 : `LogToFiles.logFolder`/`ListenerState`，自动记录日志。
* `数据路径`的监听情况 : `LogToFiles.logFolder`/`PathValue`，自动记录日志。
* `数据路径`的结构 : `LogToFiles.logFolder`/`DataStruct`，需要手动调用数据节点的`printDataByStruct`方法。



#### SimpleJson

JSON库，在其基础上实现数据缓存和键值对展开（我进行了一些魔改，这里我改的不是很好...）



#### Test

Test 文件下有各种实例，以供参考...



#### 其他

在任意UI体系下 DataCenter 和 ComponentWrapper ，都可以提供数据绑定特性。

* DataCenter 和 ComponentWrapper 的实现是独立于 UIManager 的，可以在你已有的 UI体系 上加上这些特性。
* 参考 Test/5_ComponentWrapperTest 就可以将数据绑定的特性加入你的工程。



从事游戏前端开发有十多年了，自认为可以解决一些小问题。

如有需要请联系 : jia_shiyang@sina.com
