using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UIElements;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Terminal : AppPrefab, IInitializePotentialDragHandler
{
    [SerializeField] private SOAllMethods allMethodsSO;
    [SerializeField] private RectTransform mainWindow;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject inputPrefab;
    [SerializeField] private TMP_InputField currentInput;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform viewport;
    [SerializeField] private float inputPadding = 2f;
    [SerializeField] private SAllErrors Errors;
    private List<SOMethod> AllMethods => allMethodsSO.AllMethods;
    private List<GameObject> TextInputs = new List<GameObject>();
    private Vector3 startPos;

    public string CurrentInputText { get; private set; }
    public delegate void LinesPrintedDelegate();
    public static event LinesPrintedDelegate OnLinesPrintedEvent;

    #region Initialize

    public override void Setup(SOApp app)
    {
        base.Setup(app);
        
        InitializeTerminal();
        FocusInput();
    }

    public void OnInitializePotentialDrag(PointerEventData eventData)
    {
        eventData.useDragThreshold = false;
    }

    public void InitializeTerminal()
    {
        currentInput.onSubmit.AddListener(OnEnterInput);
        currentInput.onSelect.AddListener(OnSelect);
        startPos = currentInput.transform.localPosition;
        FocusInput();
    }

    private void OnDisable()
    {
        currentInput.onSelect.RemoveListener(OnSelect);
        currentInput.onSubmit.RemoveListener(OnEnterInput);
    }
    
    #endregion
    
    //todo: Terminal.cs ukonczyc metody ui
    #region Event functions
    
    public void OnTerminalDrag(BaseEventData data)
    {
        var pointerEventData = (PointerEventData)data;
        
        mainWindow.anchoredPosition += pointerEventData.delta / MainCanvas.Instance.GetCanvas().scaleFactor;
        
        mainWindow.transform.SetAsLastSibling();
    }

    public void OnTerminalDragEnd()
    {
        FocusInput();
    }

    private void OnSelect(string currentText)
    {
        StartCoroutine(SetBackOldText(currentText));
    }

    IEnumerator SetBackOldText(string currentText)
    {
        yield return new WaitForNextFrameUnit();
        currentInput.SetTextWithoutNotify(currentText);
        currentInput.caretPosition = currentInput.text.Length;
    }
    public void OnScroll(int dir)
    {
        if (dir is 0)
            return;

        var currentVal = scrollRect.verticalNormalizedPosition;
        var contentHeight = scrollRect.content.sizeDelta.y;
        var contentShift = scrollRect.scrollSensitivity * dir;
        var newVal = currentVal + contentShift / contentHeight;
        scrollRect.verticalNormalizedPosition = dir > 0 ? (newVal > 1 ? 1 : newVal) : (newVal < 0 ? 0 : newVal);
    }
    
    #endregion /

    #region Getters

    public float GetPadding()
    {
        return inputPadding;
    }
    
    #endregion

    public void FocusInput()
    {
        currentInput.Select();
        currentInput.ActivateInputField();
        currentInput.text = "";
        scrollRect.verticalNormalizedPosition = 0;
    }

    private void SetScrollBarSize()
    {
        var contentHeight = content.rect.height;
        var viewportHeight = viewport.rect.height;
        if (contentHeight < viewportHeight)
        {
            scrollbar.size = 1;
            return;
        }

        var contentViewportRatio = viewportHeight / contentHeight;
        scrollbar.size = contentViewportRatio;
    }
    public void OnEnterInput(string value)
    {
        value = value.Replace('\n', ' ');
        if (value.Trim() == "")
            return;
         
        Print(value, false);
        CurrentInputText = value;
        var values = value.Split(' ').ToList();
        var methodName = values[0].ToLower();
        values.RemoveAt(0);
        var parameters = new List<string>();
        var flags = new List<string>();
        var paramInQuotationMark = "";
        values.ForEach(val =>
        {
            if (val.StartsWith('-'))
            {
                flags.Add(val);
                return;
            }
            
            if (val.StartsWith('"'))
            {
                paramInQuotationMark += val;
                if (val.EndsWith('"'))
                {
                    parameters.Add(paramInQuotationMark.Trim('"'));
                }
                
                return;
            }
            
            if (paramInQuotationMark != "")
            {
                paramInQuotationMark += " " + val.Replace('"', '\0');
                if (val.EndsWith('"'))
                {
                    parameters.Add(paramInQuotationMark.Trim('"'));
                }

                return;
            }

            if (val.Trim() == "")
                return;
            
            parameters.Add(val);
        });
        foreach (var method in AllMethods)
        {
            if (method.GetMethodName().ToLower() == methodName)
            {
                method.RunMethod(this, parameters, flags);
                return;
            }
        }
        
        var errorParameters = new List<string>(){methodName};
        var errorMessage = Errors.GetErrorMessage(EErrorType.commandNotRecognized, errorParameters);
        Error_CustomError(errorMessage);
    }

    public void Print(string value, bool asText)
    {
        var currentInputTransform = currentInput.GetComponent<RectTransform>();
        var log = Instantiate(inputPrefab, 
            currentInputTransform.position + new Vector3(0.06f, 0.05f, 0), 
            Quaternion.identity, contentParent);
        var height = log.GetComponent<SubmitedTextBehaviour>().SetTextField(value, asText, viewport);
        TextInputs.Add(log);
        
        var newPos = currentInputTransform.anchoredPosition;
        newPos.y -= height + inputPadding;
        currentInputTransform.anchoredPosition = newPos;
        FocusInput();

        OnLinesPrintedEvent?.Invoke();
        SetScrollBarSize();
    }

    public void ClearConsole()
    {
        foreach (var textInput in TextInputs)
        {
            Destroy(textInput);
        }
        TextInputs.Clear();
        
        StartCoroutine(ResetContent());
    }

    IEnumerator ResetContent()
    {
        yield return new WaitForEndOfFrame();
        OnLinesPrintedEvent?.Invoke();
        SetScrollBarSize();
        FocusInput();
        currentInput.transform.localPosition = startPos;
    }

    //todo: ErrorComandNotRecognized
    public void Error_CommandNotRecognized(string value)
    {
        
    }
    
    public void Error_CustomError(string message)
    {
        Print(message.Color(EColorType.Red), true);
    }

    public void Error_WrongParametersCount(int expected, int passed)
    {
        var parameters = new List<string>() { expected.ToString(), passed.ToString() };
        var errorMessage = Errors.GetErrorMessage(EErrorType.wrongParameterCount, parameters);
        Error_CustomError(errorMessage);
    }

    public void Error_FlagNotRecognized(string flag)
    {
        var parameters = new List<string>(){flag};
        var errorMessage = Errors.GetErrorMessage(EErrorType.flagNotRecognized, parameters);
        Error_CustomError(errorMessage);
    }
}

[System.Serializable]
public struct SError
{
    public EErrorType errorType;
    [TextArea(4, 6)]public string errorMessage;

    public string GetError(List<string> parameters)
    {
        if (parameters.Count == 0)
        {
            Debug.LogError("Passed empty list as error parameters, Terminal:157");
            return "";
        }

        var words =errorMessage.Split(' ');
        var paramIndex = 0;
        var error = "";

        foreach (var word in words)
        {
            var correctWord = word;
            if (word.Contains("<param>"))
            {
                paramIndex = parameters.Count <= paramIndex ? 0 : paramIndex;
                correctWord = word.Replace("<param>", parameters[paramIndex]);
                paramIndex++;
            }

            error += correctWord + " ";
        }

        return error;
    }
}

[System.Serializable]
public struct SAllErrors
{
    public List<SError> Errors;

    public string GetErrorMessage(EErrorType errorType, List<string> parameters = default)
    {
        foreach (var error in Errors)
        {
            if (error.errorType == errorType)
                return error.GetError(parameters ?? new List<string>());
        }

        return "";
    }
}
