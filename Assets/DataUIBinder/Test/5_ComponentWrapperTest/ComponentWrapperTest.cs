using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using DataUIBinder;
public class ComponentWrapperTest : MonoBehaviour{
    /*
        通过命名将组件和数据源绑定
            将显示和数据解耦在代码层面解耦。
            通过名称进行数据和显示对象的关联。
        值区间绑定
            Slider 组件 temp.test.max:[50,150]
                数据路径绑定到 temp.test.max 上，值的范围绑定到 50 - 150 两侧闭区间。
            GameObject 节点 temp.test.current:[50,temp.test.max]
                节点显示，绑定到 temp.test.current 上，当其在 50 - temp.test.max 两侧闭区间时，显示。
            GameObject 节点 temp.test.current:[temp.test.min,-50]
                节点显示，绑定到 temp.test.current 上，当其在 temp.test.min - -50 两侧闭区间时，显示。
            GameObject 节点 temp.test.current>0、temp.test.current<0、temp.test.current==0
                节点显示，绑定到 temp.test.current 上，根据其比较条件，进行显示。
        Bool绑定
            GameObject 节点 temp.test.isIntAdd!=true
                节点的显示，绑定到 temp.test.isIntAdd 上，当其不等于 true 的时候，显示。
                也就是当前是负向移动，减号显示
            GameObject 节点 temp.test.isIntAdd==true 
                也就是当前是正向移动，加号显示
        String绑定
            Text 组件，命名 temp.test.isIntAdd
                Text 组件的 text 属性和 temp.test.isIntAdd 绑定
            Text 组件，编辑文本 '${数据路径}' 来做字符串拼接
                temp.test.current[50,${temp.test.max}]
                temp.test.current[${temp.test.min},-50]
                picUrl:${temp.test.picUrl}
            Image 组件，命名 temp.test.picUrl
                Image 的 Sprite 为 temp.test.picUrl 指定的图片
                    可以是本地图片
                    也可以是网络图片，http或https开头的字符串。
        Numeric绑定
            属性绑定，'x:路径,y:路径'，x,y属性会随值变化，表现为移动
                x:temp.test.max,y:temp.test.max
                x:temp.test.min,y:temp.test.min
                x:temp.test.current,y:temp.test.current
                x:temp.test.min,y:temp.test.current
                x:temp.test.max,y:temp.test.current
                x:temp.test.current,y:temp.test.min
                x:temp.test.current,y:temp.test.max
    */
    private DataPathDriven dataPathDriven = null;
    //变量和数据路竞中的值关联
    private string picUrl{
        get{ return dc.gv("temp.test.picUrl").AsString; }
        set{ dc.sv("temp.test.picUrl",value); }
    }
    private bool isIntAdd{
        get{ return dc.gv("temp.test.isIntAdd").AsBool; }
        set{ dc.sv("temp.test.isIntAdd",value); }
    }
    private int current{
        get{ return dc.gv("temp.test.current").AsInt; }
        set{ dc.sv("temp.test.current",value); }
    }
    private int min{
        get{ return dc.gv("temp.test.min").AsInt; }
        set{ dc.sv("temp.test.min",value); }
    }
    private int max{
        get{ return dc.gv("temp.test.max").AsInt; }
        set{ dc.sv("temp.test.max",value); }
    }
    void Awake(){
        Recoder.isRecord = false;
        DataCenter.defalutInit();
        ComponentWrapper.tryAddWrapperToChildren(transform);
        dataPathDriven = DataPathDriven.reUse();
    }
    IEnumerator Start(){
        dataPathDriven.onChange("temp.test.current",(current_)=>{
            if(!isIntAdd && current_.AsInt <= min){
                isIntAdd = true;
                picUrl = "pics/starpic";
            }else if(isIntAdd && current_.AsInt >= max){
                isIntAdd = false;
                picUrl = "https://www.baidu.com/img/flexible/logo/pc/result.png";
            }
        });
        min = -100;
        max = 150;
        current = 0;
        isIntAdd = true;
        picUrl = "pics/starpic";
        yield return null;
    }
    void OnDestroy(){
        dataPathDriven.unUse();
        dataPathDriven = null;
    }
    void Update(){
        float _dt = Time.deltaTime;
        DataCenter.frameUpdate(_dt);
        ComponentWrapper.doFrameUpdate(_dt);
        current = isIntAdd?current+1:current-1;
    }
    public void onBtnClick(){
        isIntAdd = !isIntAdd;
    }

}