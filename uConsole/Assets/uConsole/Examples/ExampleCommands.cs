using UnityEngine;
using System.Collections;

public class ExampleCommands : MonoBehaviour
{
	void Start()
	{
		uConsole.RegisterCommand("EchoNothing", EchoNothing);
		uConsole.RegisterCommand("EchoInteger", EchoInteger);
		uConsole.RegisterCommand("EchoFloat", EchoFloat);
		uConsole.RegisterCommand("EchoBool", EchoBool);
		uConsole.RegisterCommand("EchoString", EchoString);
		uConsole.RegisterCommand("EchoStrings", EchoStrings);
		uConsole.RegisterCommand("LoadScene", LoadScene);
	}

	void OnDestroy()
	{
		uConsole.UnRegisterCommand("EchoNothing");
		uConsole.UnRegisterCommand("EchoInteger");
		uConsole.UnRegisterCommand("EchoFloat");
		uConsole.UnRegisterCommand("EchoBool");
		uConsole.UnRegisterCommand("EchoString");
		uConsole.UnRegisterCommand("EchoStrings");
	}
	
	public void EchoNothing()
	{
		uConsole.Log("Test Command Executed at time: " + Time.time);
	}

	public void EchoInteger()
	{
		uConsole.Log("Integer Entered: " + uConsole.GetInt().ToString());
	}

	public void EchoFloat()
	{
		uConsole.Log("Float Entered: " + uConsole.GetFloat().ToString ());
	}

	public void EchoBool()
	{
		if (uConsole.GetBool()) {
			uConsole.Log("Bool Entered: TRUE");
		} else {
			uConsole.Log("Bool Entered: FALSE");
		}
	}

	public void EchoString()
	{
		uConsole.Log("String Entered: " + uConsole.GetString());
	}

	public void EchoStrings()
	{
		uConsole.Log("String Entered 1: " + uConsole.GetString());
		uConsole.Log("String Entered 2: " + uConsole.GetString());
	}

	public static void LoadScene()
	{
		UnityEngine.SceneManagement.SceneManager.LoadScene(uConsole.GetString());
		uConsole.TurnOff();
	}
}
