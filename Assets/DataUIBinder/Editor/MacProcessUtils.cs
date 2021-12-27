
using System;
using System.Collections;
using System.Collections.Generic;
public class MacProcessUtils{
#if UNITY_EDITOR_OSX
	public static void doCommand(
		string commandFilePath_,//命令行工具的路径
		string arguments_,//执行参数
		Action<bool,List<string>> reslutFunc_,//结果函数
		int waitSeconds_ = 2000//默认 2000 毫秒，等待时间
	) {
		List<string> _logStrList = new List<string>();

		bool _isError = false;

		var _processStartInfo = new System.Diagnostics.ProcessStartInfo();
		_processStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
		_processStartInfo.FileName = commandFilePath_;
		_processStartInfo.Arguments = arguments_;
		_processStartInfo.UseShellExecute = false;
		_processStartInfo.RedirectStandardOutput = true;
		_processStartInfo.RedirectStandardError = true;

		System.Diagnostics.Process _process = new System.Diagnostics.Process();
		_process.StartInfo = _processStartInfo;
		_process.OutputDataReceived += (_, info_) => {
			if(string.IsNullOrEmpty(info_.Data)){
                return;
            }
			_logStrList.Add(info_.Data);
		};
		_process.ErrorDataReceived += (_, err_) => {
			if(string.IsNullOrEmpty(err_.Data)){
                return;
            }
			_logStrList.Add(err_.Data);
			_isError = true;
		};
		//执行命令行
		_process.Start();
		_process.BeginOutputReadLine();
		_process.BeginErrorReadLine();
		_process.WaitForExit(waitSeconds_);
		//是否错误，日志信息
		reslutFunc_(_isError,_logStrList);
		//命令行执行错误
		if(_isError){
			UnityEngine.Debug.LogError("ERROR : " + commandFilePath_ + " " + arguments_);
		}
	}
#endif
}