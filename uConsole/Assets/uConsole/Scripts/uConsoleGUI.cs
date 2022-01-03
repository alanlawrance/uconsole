using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class uConsoleGUI : MonoBehaviour
{
	public GameObject m_ConsoleEventSystem;
	public RectTransform m_PanelRectTransform;
	public Image m_PanelImage;
	public TextMeshProUGUI m_LogText;
	public TMP_InputField m_InputField;
	public TextMeshProUGUI m_InputFieldText;
	
	private RectTransform m_InputFieldRectTransform;
	private int m_LogScrollOffset;

	void Start()
	{
		m_PanelRectTransform.anchoredPosition = new Vector2(0, 0);
        m_PanelRectTransform.sizeDelta = new Vector2(Screen.width, 0f);
		m_InputField = GetComponentInChildren<TMP_InputField>();
		m_InputFieldRectTransform = m_InputField.GetComponent<RectTransform>();
		m_InputField.DeactivateInputField();
		m_LogScrollOffset = 0;
	}

	void Update()
	{
		MaybeInstantiateEventSystem();
		MaybeDeactivateInputField();
		UpdateDimensions();
		UpdateWithCustomizationSettings();
		Animate(Time.unscaledDeltaTime);
	}

	public void ScrollLogUp()
	{
		m_LogScrollOffset++;
		if (m_LogScrollOffset > uConsoleLog.GetNumLines()) {
			m_LogScrollOffset = uConsoleLog.GetNumLines();
		}
		RefreshLogText();
	}

	public void ScrollLogDown()
	{
		m_LogScrollOffset--;
		if (m_LogScrollOffset < 0) {
			m_LogScrollOffset = 0;
		}
		RefreshLogText();
	}

	public void ScrollLogUpMax()
	{
		m_LogScrollOffset = uConsoleLog.GetNumLines();
		RefreshLogText();
	}
	
	public void ScrollLogDownMax()
	{
		m_LogScrollOffset = 0;
		RefreshLogText();
	}

	public void RefreshLogText()
	{
		int maxDisplayLines = ComputeMaxDisplayLinesForLog();
		int start = uConsoleLog.GetNumLines() - m_LogScrollOffset - maxDisplayLines;
		if (start < 0) {
			start = 0;
		}
		
		m_LogText.text = "";
		for (int i=start; i<uConsoleLog.GetNumLines() - m_LogScrollOffset; i++) {
			m_LogText.text += "\n";
			m_LogText.text += uConsoleLog.GetLine(i);
		}
	}
	
	public void InputFieldSetFocus()
	{
		if (!m_InputField.isFocused) {
			m_InputField.ActivateInputField();
			m_InputField.Select();

			// Start a coroutine to deselect text and move caret to end. 
			// This can't be done now, must be done in the next frame.
			StartCoroutine(MoveTextEnd_NextFrame());
        }
	}

	IEnumerator MoveTextEnd_NextFrame()
	{
		yield return 0; // Skip the first frame in which this is called.
		m_InputField.MoveTextEnd(false); // Do this during the next frame.
	}

	public string InputFieldGetText()
	{
		return m_InputField.text;
	}
	
	public void InputFieldClearText()
	{
		m_InputField.text = "";
		m_InputField.MoveTextStart(false);
	}
	
	public void InputFieldSetText(string text)
	{
		m_InputField.text = text;
	}
	
	public void InputFieldMoveCaretToEnd()
	{
		m_InputField.MoveTextEnd(false);
	}
	
	public void InputFieldDeactivate()
	{
		m_InputField.DeactivateInputField();
	}
	
	private void Animate(float deltaTimeSeconds)
	{
		if (uConsole.IsOn()) {
			float targetHeight = uConsole.m_Instance.m_ConsoleHeightNormalized * Screen.height;
			
			if (Mathf.Approximately(m_PanelRectTransform.sizeDelta.y, targetHeight)) {
				return;
			}
			
			float deltaHeight = CalculatePixelsMovedForAnimation(deltaTimeSeconds, uConsole.m_Instance.m_SecondsToAnimateDown);
			
			if (m_PanelRectTransform.sizeDelta.y < targetHeight) {
				m_PanelRectTransform.sizeDelta += new Vector2(0f, deltaHeight);
                if (m_PanelRectTransform.sizeDelta.y >= targetHeight) {
                    m_PanelRectTransform.sizeDelta = new Vector2(m_PanelRectTransform.sizeDelta.x, targetHeight);
                }
			}
		} else {

			if (m_PanelRectTransform.sizeDelta.y > 0f) {
				float deltaHeight = CalculatePixelsMovedForAnimation(deltaTimeSeconds, uConsole.m_Instance.m_SecondsToAnimateUp);
				m_PanelRectTransform.sizeDelta -= new Vector2(0f, Mathf.Min(m_PanelRectTransform.sizeDelta.y, deltaHeight));
			}
		}
	}
	
	private float CalculatePixelsMovedForAnimation(float deltaSeconds, float fullAnimateSeconds)
	{
		float pixelHeight = Screen.height * uConsole.m_Instance.m_ConsoleHeightNormalized;
		float percentChange = Mathf.Clamp(deltaSeconds/fullAnimateSeconds, 0f, 1f);
		return pixelHeight * percentChange;
	}
	
	private void UpdateDimensions()
	{		
        m_PanelRectTransform.sizeDelta = new Vector2(Screen.width, m_PanelRectTransform.sizeDelta.y);
        m_InputFieldRectTransform.sizeDelta = new Vector2(Screen.width, uConsole.m_Instance.m_InputFieldHeight);		
	}
		
	private void UpdateWithCustomizationSettings()
	{
		m_LogText.font = uConsole.m_Instance.m_LogFont;
		m_LogText.fontSize = uConsole.m_Instance.m_LogFontSize;
		m_LogText.color = uConsole.m_Instance.m_LogFontColor;
		
		m_InputFieldText.font = uConsole.m_Instance.m_InputFieldFont;
		m_InputFieldText.color = uConsole.m_Instance.m_InputFieldFontColor;
        m_InputFieldText.fontSize = uConsole.m_Instance.m_InputFieldFontSize;
		
		m_PanelImage.color = uConsole.m_Instance.m_LogBackGroundColor;
		m_InputField.image.color = uConsole.m_Instance.m_InputFieldBackGroundColor;
	}
	
	private int ComputeMaxDisplayLinesForLog()
	{
		/*
		float lineHeight = 8;
		float consoleHeight = uConsole.m_Instance.m_ConsoleHeightNormalized * Screen.height;
		int numLines = Mathf.FloorToInt(consoleHeight / lineHeight);
		return numLines;
		*/
		return 32;
	}

	private void MaybeInstantiateEventSystem()
	{
		if (EventSystem.current != null) {
			return;
		}

		GameObject go = Instantiate(m_ConsoleEventSystem) as GameObject;
		if (go) {
			go.name = m_ConsoleEventSystem.name;
			go.transform.parent = transform;
		}
	}

	private void MaybeDeactivateInputField()
	{
		if (!uConsole.IsOn()) {
			m_InputField.DeactivateInputField();
		}
	}
}