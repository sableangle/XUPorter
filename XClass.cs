using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace UnityEditor.XCodeEditor
{
	public partial class XClass : System.IDisposable
	{

        private string filePath;

		public XClass(string fPath)
		{
            filePath = fPath;
			if( !System.IO.File.Exists( filePath ) ) {
					Debug.LogError( filePath +"路徑下找不到檔案" );
					return;
			}
		}

		public void AddInFunction(string functionName,string text){
			StreamReader streamReader = new StreamReader(filePath);
			string text_all = streamReader.ReadToEnd();
			streamReader.Close();

			int beginIndex = text_all.IndexOf(functionName);
			if(beginIndex == -1){
				Debug.LogError(filePath +"找不到標記"+functionName);
				return; 
			}
			int endIndex = text_all.LastIndexOf("\n", beginIndex + functionName.Length);

			for (int i = endIndex; i < text_all.Length; i++) {
				if (text_all [i] == '{') {
					endIndex = i + 1;
					break;
				}
			}

			text_all = text_all.Substring(0, endIndex) + "\n"+text+"\n" + text_all.Substring(endIndex);

			StreamWriter streamWriter = new StreamWriter(filePath);
			streamWriter.Write(text_all);
			streamWriter.Close();

		}

		public void ChangeFunctionReturn(string functionName,string text){
			StreamReader streamReader = new StreamReader(filePath);
			string text_all = streamReader.ReadToEnd();
			streamReader.Close();

			int beginIndex = text_all.IndexOf(functionName);
			if(beginIndex == -1){
				Debug.LogError(filePath +"找不到標記"+functionName);
				return; 
			}
			int endIndex = text_all.LastIndexOf("\n", beginIndex + functionName.Length);


			int firstReturnIndex = text_all.IndexOf ("return",endIndex);

			int lastReturnIndex = 0;
			for (int i = firstReturnIndex; i < text_all.Length; i++) {
				if (text_all [i] == ';') {
					lastReturnIndex = i + 1;
					break;
				}
			}

			text_all = text_all.Substring(0, firstReturnIndex) + "\n"+text+"\n" + text_all.Substring(lastReturnIndex);

			Debug.Log (firstReturnIndex);

			Debug.Log (lastReturnIndex);
			StreamWriter streamWriter = new StreamWriter(filePath);
			streamWriter.Write(text_all);
			streamWriter.Close();

		}

        public void WriteBelow(string below, string text)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();

            int beginIndex = text_all.IndexOf(below);
            if(beginIndex == -1){
                Debug.LogError(filePath +"找不到標記"+below);
                return; 
            }

            int endIndex = text_all.LastIndexOf("\n", beginIndex + below.Length);

            text_all = text_all.Substring(0, endIndex) + "\n"+text+"\n" + text_all.Substring(endIndex);

            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(text_all);
            streamWriter.Close();
        }

        public void Replace(string below, string newText)
        {
            StreamReader streamReader = new StreamReader(filePath);
            string text_all = streamReader.ReadToEnd();
            streamReader.Close();

            int beginIndex = text_all.IndexOf(below);
            if(beginIndex == -1){
				Debug.LogError(filePath +"找不到標記"+below);
                return; 
            }

            text_all =  text_all.Replace(below,newText);
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(text_all);
            streamWriter.Close();

        }




        public void Dispose()
        {

        }
	}
}