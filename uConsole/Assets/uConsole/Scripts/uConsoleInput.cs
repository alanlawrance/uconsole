using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class uConsoleInput
{
	public static bool m_ForceSubmit;
	public static bool m_ForceAutoComplete;
	public static bool m_ForceRecallUp;
	public static bool m_ForceRecallDown;
	public static bool m_ForceScrollLogUp;
	public static bool m_ForceScrollLogDown;

	public static char[] m_DelimterChars = { ' ', ',', '\t' };

	private static int m_LastHistoryIndexRecalled;
	private static float m_TimeRecallUpPressed;
	private static float m_TimeRecallDownPressed;
	private static float m_TimeScrollUpPressed;
	private static float m_TimeScrollDownPressed;
	private static float m_CommandRepeatTimeSeconds;

	private static bool m_ControlTipDisplayed;

	public static void Initialize()
	{
		m_LastHistoryIndexRecalled = -1;
	}
	
	public static void DoFrame()
	{
		ProcessActivationInput();
		
		if (!uConsole.IsOn()) {
			return;
		}
		
		ProcessSubmitInput();
		ProcessAutoCompleteInput();
		ProcessHistoryInput();
		ProcessLogInput();
	}

	private static void SubmitCommand()
	{
		string command = uConsole.m_GUI.InputFieldGetText();
		if (command == null) {
			return;
		}

		uConsole.RunCommand(command);

		uConsole.m_GUI.InputFieldSetFocus();
		uConsole.m_GUI.InputFieldClearText();

		m_LastHistoryIndexRecalled = -1;
	}

	private static void AutoComplete()
	{
		string current = uConsole.m_GUI.InputFieldGetText();
		if (current == null || current == "") {
			return;
		}

		string bestAutoComplete = uConsoleAutoComplete.GetBestCompletion(current);
		if (bestAutoComplete == null) {
			return;
		}

		if (uConsole.CommandIsUnabmiguousAutoComplete(bestAutoComplete)) {
			uConsole.m_GUI.InputFieldSetText(bestAutoComplete + " ");
		} else {
			uConsole.m_GUI.InputFieldSetText(bestAutoComplete);
			uConsoleAutoComplete.DisplayPossibleMatches(bestAutoComplete);
		}

		uConsole.m_GUI.InputFieldMoveCaretToEnd();
	}

	private static void RecallCommandUp()
	{
		if (uConsoleHistory.GetNumLines() == 0) {
			return;
		}

		if (m_LastHistoryIndexRecalled >= uConsoleHistory.GetNumLines() - 1) {
			m_LastHistoryIndexRecalled = -1;
		}

		m_LastHistoryIndexRecalled++;

		string command = uConsoleHistory.GetLine(m_LastHistoryIndexRecalled);
		uConsole.m_GUI.InputFieldSetText(command);
		uConsole.m_GUI.InputFieldMoveCaretToEnd();
	}
	
	private static void RecallCommandDown()
	{
		if (uConsoleHistory.GetNumLines() == 0) {
			return;
		}

		if (m_LastHistoryIndexRecalled < 1) {
			m_LastHistoryIndexRecalled = uConsoleHistory.GetNumLines();
		}

		m_LastHistoryIndexRecalled--;

		string command = uConsoleHistory.GetLine(m_LastHistoryIndexRecalled);
		uConsole.m_GUI.InputFieldSetText(command);
		uConsole.m_GUI.InputFieldMoveCaretToEnd();
	}

	private static void ProcessActivationInput()
	{
		if (Input.GetKeyDown(uConsole.m_Instance.m_Activate)) {
			if (!uConsole.IsOn()) {
				uConsole.TurnOn();
				uConsole.m_GUI.InputFieldMoveCaretToEnd();
				if (!m_ControlTipDisplayed) {
					UnityEngine.Debug.Log("Press tilde [~] again to close console");
					m_ControlTipDisplayed = true;
				}
			} else {
				uConsole.TurnOff();
				uConsole.m_GUI.InputFieldDeactivate();
			}
		}
		
		if (Input.GetKeyUp(uConsole.m_Instance.m_Activate)) {
			if (uConsole.IsOn()) {
				uConsole.m_GUI.InputFieldSetFocus();
			}
		}

		string current = uConsole.m_GUI.InputFieldGetText();
		if (current != null && current.Contains("`")) {
			if (current.Length == 1) {
				uConsole.m_GUI.InputFieldClearText();
			} else {
				uConsole.m_GUI.InputFieldSetText(current.Substring(0, current.Length - 1));
				uConsole.m_GUI.InputFieldMoveCaretToEnd();
			}
		}
	}

	private static void ProcessSubmitInput()
	{
		if (Input.GetKeyUp(uConsole.m_Instance.m_Submit) || m_ForceSubmit) {
			SubmitCommand();
			m_ForceSubmit = false;
		}
	}

	private static void ProcessAutoCompleteInput()
	{
		if (Input.GetKeyDown(uConsole.m_Instance.m_AutoComplete) || m_ForceAutoComplete) {
			AutoComplete();
			m_ForceAutoComplete = false;
		}
	}

	private static void ProcessHistoryInput()
	{	
		if (Input.GetKeyDown(uConsole.m_Instance.m_HistoryUp) || m_ForceRecallUp) {
			RecallCommandUp();
			m_TimeRecallUpPressed = Time.realtimeSinceStartup;
			m_CommandRepeatTimeSeconds = 0.5f;
			m_ForceRecallUp = false;
		}
		
		if (Input.GetKeyDown(uConsole.m_Instance.m_HistoryDown) || m_ForceRecallDown) {
			RecallCommandDown();
			m_TimeRecallDownPressed = Time.realtimeSinceStartup;
			m_CommandRepeatTimeSeconds = 0.5f;
			m_ForceRecallDown = false;
		}
		
		if (Input.GetKey(uConsole.m_Instance.m_HistoryUp)) {
			float timeDelta = Time.realtimeSinceStartup - m_TimeRecallUpPressed;
			if (timeDelta > m_CommandRepeatTimeSeconds) {
				RecallCommandUp();
				m_TimeRecallUpPressed = Time.realtimeSinceStartup;
				m_CommandRepeatTimeSeconds = 0.1f;
			}
		}
		
		if (Input.GetKey(uConsole.m_Instance.m_HistoryDown)) {
			float timeDelta = Time.realtimeSinceStartup - m_TimeRecallDownPressed;
			if (timeDelta > m_CommandRepeatTimeSeconds) {
				RecallCommandDown();
				m_TimeRecallDownPressed = Time.realtimeSinceStartup;
				m_CommandRepeatTimeSeconds = 0.1f;
			}
		}
	}

	private static void ProcessLogInput()
	{
		if (Input.GetAxis("Mouse ScrollWheel") > 0) {
			uConsole.m_GUI.ScrollLogUp();
		}
		
		if (Input.GetAxis("Mouse ScrollWheel") < 0) {
			uConsole.m_GUI.ScrollLogDown();
		}
		
		if (Input.GetKeyDown(uConsole.m_Instance.m_ScrollLogUp) || m_ForceScrollLogUp) {
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
				uConsole.m_GUI.ScrollLogUpMax();
			} else {
				uConsole.m_GUI.ScrollLogUp();
				m_TimeScrollUpPressed = Time.realtimeSinceStartup;
				m_CommandRepeatTimeSeconds = 0.5f;
			}

			m_ForceScrollLogUp = false;
		}
		
		if (Input.GetKeyDown(uConsole.m_Instance.m_ScrollLogDown) || m_ForceScrollLogDown) {
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
				uConsole.m_GUI.ScrollLogDownMax();
			} else {
				uConsole.m_GUI.ScrollLogDown();
				m_TimeScrollDownPressed = Time.realtimeSinceStartup;
				m_CommandRepeatTimeSeconds = 0.5f;
			}

			m_ForceScrollLogDown = false;
		}

		if (Input.GetKey(uConsole.m_Instance.m_ScrollLogUp)) {
			float timeDelta = Time.realtimeSinceStartup - m_TimeScrollUpPressed;
			if (timeDelta > m_CommandRepeatTimeSeconds) {
				uConsole.m_GUI.ScrollLogUp();
				m_TimeScrollUpPressed = Time.realtimeSinceStartup;
				m_CommandRepeatTimeSeconds = 0.1f;
			}
		}

		if (Input.GetKey(uConsole.m_Instance.m_ScrollLogDown)) {
			float timeDelta = Time.realtimeSinceStartup - m_TimeScrollDownPressed;
			if (timeDelta > m_CommandRepeatTimeSeconds) {
				uConsole.m_GUI.ScrollLogDown();
				m_TimeScrollDownPressed = Time.realtimeSinceStartup;
				m_CommandRepeatTimeSeconds = 0.1f;
			}
		}
	}
}