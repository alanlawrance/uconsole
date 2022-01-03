using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class uConsoleCommandFile
{
	private static bool m_ProcessedMainConfigFile;
	private static string[] m_PendingScriptCommands;
	private static float m_ApplyScriptCommandsTime;
	private static int m_ScriptCommandIndex;

	public static void Initialize()
	{
#if __DEBUG
		if (!m_ProcessedMainConfigFile) {
			ReadConfigFile("cmd_game.txt");
			m_ProcessedMainConfigFile = true;
		}
		ReadConfigFile("cmd_" + SceneManager.GetActiveScene().name + ".txt");

		int i = 0;
		if (m_PendingScriptCommands != null) {
			while ((i < m_PendingScriptCommands.Length) && (m_PendingScriptCommands[i].StartsWith("//"))) {
				i++;
			}
			if (i < m_PendingScriptCommands.Length) {
				float delay = 0.1f;
				// See if we have a custom delay for our first command...
				string cmd = m_PendingScriptCommands[i];
				if (cmd.StartsWith("delay ")) {
					int j = cmd.LastIndexOf(' ');
					if (j >= 0) {
						float.TryParse(cmd.Substring(j), out delay);
					}
				}
				m_ScriptCommandIndex = i;
				m_ApplyScriptCommandsTime = Time.time + delay;
			}
		}
#endif
	}

	public static void DoFrame()
	{
#if __DEBUG
		if (m_PendingScriptCommands != null) {
			// Process any commands that were loaded from config files in Start()...
			if (Time.time > m_ApplyScriptCommandsTime) {
				ProcessScriptCommands();
			}
		}
#endif
	}

	public static void RegisterPendingCommands(string[] commandLines)
	{
		if (m_PendingScriptCommands == null) {
			m_PendingScriptCommands = commandLines;
		} else {
			List<string> tmp = new List<string>();
			tmp.AddRange(m_PendingScriptCommands);
			tmp.AddRange(commandLines);
			m_PendingScriptCommands = tmp.ToArray();
		}
	}

	private static void ReadConfigFile(string filename)
	{
#if __DEBUG
		string path = Application.dataPath;
		path = path.Substring(0, path.Length - "Assets".Length) + filename;
		string text = null;
		if (File.Exists(path)) {
			Debug.Log("Reading command file: " + path);
			try {
				using (StreamReader reader = new StreamReader(path)) {
					text = reader.ReadToEnd();
					reader.Close();
				}
			}
			catch (Exception) {
				Debug.Log("Failed to read command file: " + path);
				text = null;
			}
		}

		if (text != null) {
			string[] commandLines = text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
			if (commandLines.Length > 0) {
				RegisterPendingCommands(commandLines);
			}
		}
#endif
	}

    private static void ProcessScriptCommands()
    {
#if __DEBUG
	    while( m_ScriptCommandIndex < m_PendingScriptCommands.Length) {   
		    string cmd = m_PendingScriptCommands[m_ScriptCommandIndex];
			m_ScriptCommandIndex++;
		    if (!cmd.StartsWith("//")) {
				if (cmd.StartsWith("delay ")) {
					float delay = 0.0f;
					int i = cmd.LastIndexOf(' ');
					if (i >= 0) {
						float.TryParse(cmd.Substring(i), out delay);
						if (delay > 0.0f) {
							m_ApplyScriptCommandsTime = Time.time + delay;
							return;
						}
					}
				} else if (cmd.StartsWith(">>")) {
					int cm = cmd.IndexOf("//");
					if (cm >= 0) {
						cmd = cmd.Substring(0, cm).TrimEnd();
					}
					Debug.Log(cmd);
					HUDMessage.AddMessage(cmd.Substring(2).Trim());
				} else {
					uConsole.RunCommand(cmd);
				}
			}
		}
      	m_PendingScriptCommands = null;
#endif
	}
}
