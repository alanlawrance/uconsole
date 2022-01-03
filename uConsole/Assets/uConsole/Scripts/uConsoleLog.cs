using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class uConsoleLog
{
	private static List<string> m_Log = new List<string>();
	private static string m_Filename = "ConsoleLog";
	private static int m_MaxNumberOfLines;

	public static void Clear()
	{
		m_Log.Clear();
	}

	public static void Add(string text)
	{
		m_Log.Add(text);
		if (m_Log.Count > m_MaxNumberOfLines) {
			m_Log.RemoveAt(0);
		}
		if (uConsole.m_GUI) {
			uConsole.m_GUI.RefreshLogText();
		}
	}

	public static void SetMaxNumberOfLines(int count)
	{
		m_MaxNumberOfLines = count;
		while (m_Log.Count > m_MaxNumberOfLines) {
			m_Log.RemoveAt(0);
		}
	}
	
	public static int GetNumLines()
	{
		return m_Log.Count;
	}

	public static string GetLine(int index)
	{
		if (index < m_Log.Count) {
			return m_Log[index];
		} else {
			return "";
		}
	}

	public static void Save()
	{
		string path = Application.persistentDataPath;
		if (!Directory.Exists(path)) {
			Directory.CreateDirectory(path);
		}
		
		string fileName = path + "/" + m_Filename;
		StreamWriter stream = File.CreateText(fileName);
		if (stream == null) {
			return;
		}
		
		for (int i=0; i<m_Log.Count; i++) {
			stream.WriteLine(m_Log[i]);
		}
		
		stream.Close();
	}
	
	public static void Restore()
	{
		string path = Application.persistentDataPath;
		if (!Directory.Exists(path)) {
			return;
		}
		
		string fileName = path + "/" + m_Filename;
		if (!File.Exists(fileName)) {
			return;
		}
		
		StreamReader stream = File.OpenText(fileName);
		if (stream == null) {
			return;
		}
		
		string line = null;
		for (;;) {
			line = stream.ReadLine();
			if (line == null) {
				break;
			}
			m_Log.Add(line);
		}
		
		stream.Close();
	}

	public static void HandleLogMessagesFromUnity(string logString, string stackTrace, LogType type)
	{
		if (logString.Length > 128) {
			uConsoleLog.Add(logString.Substring(0, 128));
		} else {
			uConsoleLog.Add(logString);
		}
	}
}