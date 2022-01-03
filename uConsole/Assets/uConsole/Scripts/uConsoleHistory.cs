using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class uConsoleHistory
{
	private static List<string> m_History = new List<string>();
	private static int m_MaxNumberOfLines;
	private static string m_Filename = "ConsoleHistory";

	public static void Clear()
	{
		m_History.Clear();
	}

	public static void SetMaxNumberOfLines(int count)
	{
		m_MaxNumberOfLines = count;
		while (m_History.Count > m_MaxNumberOfLines) {
			m_History.RemoveAt(m_History.Count - 1);
		}
	}
	
	public static string GetLine(int index)
	{
		if (index < m_History.Count) {
			return m_History[index];
		} else {
			return "";
		}
	}
	
	public static int GetNumLines()
	{
		return m_History.Count;
	}
	
	public static void Add(string text)
	{
		if (m_History.Count > 0 && m_History[0] == text) {
			return;
		}
		
		m_History.Insert(0, text);
		if (m_History.Count > m_MaxNumberOfLines) {
			m_History.RemoveAt(m_History.Count - 1);
		}
		Save();
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

		for (int i=0; i<m_History.Count; i++) {
			stream.WriteLine(m_History[i]);
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
		for (int i=0; i<m_MaxNumberOfLines; i++) {
			line = stream.ReadLine();
			if (line == null) {
				break;
			}
			m_History.Add(line);
		}
		
		stream.Close();
	}
}