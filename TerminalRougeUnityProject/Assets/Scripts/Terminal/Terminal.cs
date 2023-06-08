using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Terminal : MonoBehaviour
{
    [SerializeField, Range(10, 200)] private int nextLineCharactersCount = 100;
    [SerializeField] private SOAllMethods allMethodsSO;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject inputPrefab;
    [SerializeField] private TMP_InputField currentInput;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private SAllErrors Errors;
    [SerializeField] private RectTransform content;
    [SerializeField] private RectTransform viewport;

    private float scrollTimer = 0;
    private Vector2 inputStartPos;
    private List<SOMethod> AllMethods => allMethodsSO.AllMethods;
    private List<GameObject> TextInputs = new List<GameObject>();

    public delegate void LinesPrintedDelegate();
    public static event LinesPrintedDelegate OnLinesPrintedEvent;

    void Awake()
    {
        InitializeTerminal();
    }

    public void InitializeTerminal()
    {
        currentInput.onSubmit.AddListener(OnEnterInput);
        inputStartPos = currentInput.transform.position;
        FocusInput();
    }

    private void OnDisable()
    {
        currentInput.onSubmit.RemoveListener(OnEnterInput);
    }

    public void FocusInput()
    {
        currentInput.Select();
        currentInput.ActivateInputField();
        currentInput.text = "";
    }

    private void SetScrollBarSize()
    {
        float contentHeight = content.rect.height;
        float viewportHeight = viewport.rect.height;
        if (contentHeight < viewportHeight)
        {
            scrollbar.size = 1;
            return;
        }

        float contentViewportRatio = viewportHeight / contentHeight;
        scrollbar.size = contentViewportRatio;
    }
    public void OnEnterInput(string value)
    {
        value = value.Replace('\n', ' ');
        if (value.Trim() == "")
            return;
         
        Print(value, false);
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
                    parameters.Add(paramInQuotationMark.Replace('"', '\0'));
                }
                return;
            }
            
            if (paramInQuotationMark != "")
            {
                paramInQuotationMark += " " + val.Replace('"', '\0');
                if (val.EndsWith('"'))
                {
                    parameters.Add(paramInQuotationMark.Replace('"', '\0'));
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
        var log = Instantiate(inputPrefab, currentInputTransform.position, Quaternion.identity, contentParent);
        var result = log.GetComponent<SubmitedTextBehaviour>().SetTextField(value, nextLineCharactersCount, asText);
        var lines = result.lines;
        var yPos = result.yPos;
        TextInputs.Add(log);
        
        var newPos = currentInputTransform.anchoredPosition;
        newPos.y -= yPos;
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

        currentInput.transform.position = inputStartPos;
        SetScrollBarSize();
        FocusInput();
    }

    //todo: ErrorComandNotRecognized
    public void Error_CommandNotRecognized(string value)
    {
        
    }
    
    public void Error_CustomError(string message)
    {
        Print($"<color=#cc3e33>{message}</color>", true);
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
