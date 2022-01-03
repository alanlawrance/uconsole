using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class uConsoleCommands
{
	public static void RegisterBuiltInCommands()
	{
		uConsole.RegisterCommand("clear", "clears uConsole log", ClearLog);
		uConsole.RegisterCommand("search", "usage: search <command>", SearchForCommand);
		uConsole.RegisterCommand("help", "usage: help [command]", ShowHelp);
		uConsole.RegisterCommand("quit", "immediately quit, without confirmation", Quit);
		uConsole.RegisterCommand("version", "show uConsole version", ShowVersion);
	}

	public static void ClearLog()
	{
		uConsoleLog.Clear();
		if (uConsole.m_GUI) {
			uConsole.m_GUI.RefreshLogText();
		}
	}

	public static void ShowVersion()
	{
		uConsoleLog.Add("uConsole Version " + uConsole.m_Version);
	}

	public static void SearchForCommand()
	{
		string command = uConsole.GetString();
		if (System.String.IsNullOrEmpty(command)) {
			uConsoleLog.Add("Usage: search <name>");
			return;
		}

		foreach(string key in uConsole.m_CommandsDict.Keys) {
			int index = key.IndexOf(command);
			if (index >= 0) {
				uConsole.ShowHelp(key);
			}
		}
	}

	public static void ShowHelp()
	{
		string helpForCommand = uConsole.GetString();
		if (!System.String.IsNullOrEmpty(helpForCommand)) {
			uConsole.ShowHelp(helpForCommand);
		} else {
			List<string> allCommands = new List<string>();
			foreach(string key in uConsole.m_CommandsDict.Keys) {
				allCommands.Add(key);
			}
			
			allCommands.Sort();
			foreach(string command in allCommands) {
				uConsole.ShowHelp(command);
			}
		}
	}

	public static void Quit()
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}
}