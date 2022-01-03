using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class uConsoleCommandParameterSet
{
	public List<string> m_Commands;
	public List<string> m_AllowedParameters;
}

public class uConsoleAutoComplete
{
	public static List<uConsoleCommandParameterSet> m_CommandParameterSets = new List<uConsoleCommandParameterSet>();

	public static void CreateCommandParameterSet(string command, List<string> parameters)
	{
		List<string> commands = new List<string>();
		commands.Add(command);
		CreateCommandParameterSet(commands, parameters);
	}

	public static void CreateCommandParameterSet(List<string> commands, List<string> parameters)
	{
		uConsoleCommandParameterSet set = new uConsoleCommandParameterSet();
		set.m_Commands = commands;
		set.m_AllowedParameters = parameters;
		m_CommandParameterSets.Add(set);
	}

	public static string GetBestCompletion(string partialCommand)
	{
		string[] tokens = partialCommand.Split(uConsoleInput.m_DelimterChars, System.StringSplitOptions.RemoveEmptyEntries);

		switch (tokens.Length) {
			case 1:
				return GetBestMatchFromList(partialCommand, uConsole.m_CommandsList);
			case 2:
				return GetBestCommandWithParameterCompletion(tokens[0], tokens[1]);
			default:
				return partialCommand;
		}
	}

	public static void DisplayPossibleMatches(string command)
	{
		string[] tokens = command.Split(uConsoleInput.m_DelimterChars, System.StringSplitOptions.RemoveEmptyEntries);

		switch (tokens.Length) {
			case 1:
				DisplayStringsStartingWithMatch(command, uConsole.m_CommandsList);
				break;
			case 2:
				DisplayParametersStartingWithMatch(tokens[0], tokens[1]);
				break;
		}
	}

	public static void DisplayStringsStartingWithMatch(string match, List<string> list)
	{
		int numMatches = 0;
		for (int i = 0; i < list.Count; i++) {
			if (list[i].IndexOf(match) == 0) {
				numMatches++;
			}
		}

		if (numMatches < 2) {
			return;
		}

		numMatches = 0;
		for (int i = 0; i < list.Count; i++) { 
			int index = list[i].IndexOf(match);
			if (index == 0) {
				if (numMatches == 0) {
					uConsole.Log("Possible Matches:");
				}

				uConsole.Log(list[i]);
				numMatches++;
			}
		}
	}

	private static void DisplayParametersStartingWithMatch(string command, string parameter)
	{
		for (int i = 0; i < m_CommandParameterSets.Count; i++) {
			uConsoleCommandParameterSet set = m_CommandParameterSets[i];
			for (int j = 0; j < set.m_Commands.Count; j++) {
				if (command == set.m_Commands[j]) {
					DisplayStringsStartingWithMatch(parameter, set.m_AllowedParameters);
				}
			}
		}
	}

	private static bool CommonCharacterAtIndex(int index, List<string> strings)
	{
		if (index >= strings[0].Length) {
			return false;
		}
		char match = strings[0][index];
		
		for (int i=1; i<strings.Count; i++) {
			if (index >= strings[i].Length) {
				return false;
			}
			
			if (strings[i][index] != match) {
				return false;
			}
		}
		
		return true;
	}

	private static string GetBestMatchFromList(string pattern, List<string> list)
	{
		List<string> candidates = new List<string>();

		for (int i = 0; i < list.Count; i++) { 
			
			if (list[i].IndexOf(pattern) == 0) {
				candidates.Add(list[i]);
			}
		}

		if (candidates.Count == 0) {
			return pattern;
		}

		if (candidates.Count == 1) {
			return candidates[0];
		}

		// extend partialCommand by characters as long as matching on all candidates
		int indexToTest = pattern.Length;
		for (;;) {
			if (!CommonCharacterAtIndex(indexToTest, candidates)) {
				break;
			}
			indexToTest++;
		}

		return candidates[0].Substring(0, indexToTest);

	}

	private static string GetBestCommandWithParameterCompletion(string command, string partialParameter)
	{
		for (int i = 0; i < m_CommandParameterSets.Count; i++) {
			uConsoleCommandParameterSet set = m_CommandParameterSets[i];
			for (int j = 0; j < set.m_Commands.Count; j++) {
				if (command == set.m_Commands[j]) {
					return command + " " + GetBestMatchFromList(partialParameter, set.m_AllowedParameters);
                }
			}
		}
        return null;
	}
}