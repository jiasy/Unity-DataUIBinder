using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
namespace DataUIBinder{
    /*
        录像
            记录操作，严格按照顺序记录
                用户操作
                非用户导致的数据变更
            播放操作，严格按照顺序播放
                用户操作
                数据的变更
            以一个转场为起点，也即是界面全切换点
                这个节点进行DataCenter的数据截屏操作。
                之后按照顺序进行记录
    */
    public class Recoder{
        public static bool isReplay = false;//是否在重播
        public static bool isRecord = true;//是否在录制
        private static JSONArray recoderJsArray = new JSONArray();
        public static void record(JSONNode recoderJsNode_){
            recoderJsArray.Add(recoderJsNode_);
        }
        public static void clear(JSONNode recoderJsNode_){
            recoderJsArray.Clear();
        }
        //将录像存成文件
        public static void recoderToFile(string name_){
            ResourceCache.writeToFile(
                ResourceCache.fileCachePath + name_ + ".json",
                recoderJsArray.ToString(),
                false
            );
        }
        //将文件读取成录像
        public static void fileToRecoder(string name_){
            string _str = ResourceCache.readFromFile( ResourceCache.fileCachePath + name_ + ".json" );
            recoderJsArray = JSONNode.Parse( _str ) as JSONArray;
            recoderJsArray.printDataByStruct();
        }
    }
}