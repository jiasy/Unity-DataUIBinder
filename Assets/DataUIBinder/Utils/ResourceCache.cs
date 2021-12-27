using System;
using System.Text;
using System.IO;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Windows;
using System.Collections;
using System.Collections.Generic;
namespace DataUIBinder{
    public class ResourceCache{
        //图片路劲
        private static string _picCachePath = null;
        public static string picCachePath{
            get{
                if(_picCachePath == null){
                    _picCachePath = Application.persistentDataPath + "/picCache/";
                    if (!UnityEngine.Windows.Directory.Exists(_picCachePath)){
                        UnityEngine.Windows.Directory.CreateDirectory(_picCachePath);
                    }
                }
                return _picCachePath;
            }
        }
        //文件路径
        private static string _fileCachePath = null;
        public static string fileCachePath{
            get{
                if(_fileCachePath == null){
                    _fileCachePath = Application.persistentDataPath + "/fileCache/";
                    if (!UnityEngine.Windows.Directory.Exists(_fileCachePath)){
                        UnityEngine.Windows.Directory.CreateDirectory(_fileCachePath);
                    }
                }
                return _fileCachePath;
            }
        }
        public static void writeToFile(string filePath_,string content_,bool appendBool_ = true){
            if (!appendBool_){ System.IO.File.Delete(filePath_ );}
            FileStream _fileStream = new FileStream(filePath_,FileMode.Append,FileAccess.Write);
            byte[] _byteList = System.Text.Encoding.Default.GetBytes(content_);
            _fileStream.Seek(0, SeekOrigin.End);
            _fileStream.Write(_byteList,0,_byteList.Length);
            _fileStream.Flush();
            _fileStream.Close();
            _fileStream.Dispose();
            _fileStream = null;
        }
        public static string readFromFile(string filePath_){
            StreamReader _streamReader =null;
            try{
                _streamReader = System.IO.File.OpenText(filePath_);
            }catch(Exception e){
                return null;
            }
            string _line;
            StringBuilder _sb = CSharpExtensionUtils.SBInstance;
            _sb.Clear();
            while ((_line = _streamReader.ReadLine()) != null){
                _sb.Append(_line);
            }
            _streamReader.Close();
            _streamReader.Dispose();
            string _contentStr = _sb.ToString();
            _sb.Clear();
            return _contentStr;
        }  
    }
}