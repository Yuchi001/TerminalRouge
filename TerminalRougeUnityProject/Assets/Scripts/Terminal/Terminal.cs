using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    [SerializeField, Range(-5, -0.1f)] private float padding = -0.5f;
    [SerializeField, Range(0, 1)] private float scrollCooldown = 0.1f;
    [SerializeField] private int autoScrollLinesLimit = 15;
    [SerializeField] private SAllErrors Errors;

    private float scrollTimer = 0;
    private Vector2 inputStartPos;
    private List<SOMethod> AllMethods => allMethodsSO.AllMethods;
    private List<GameObject> TextInputs = new List<GameObject>();
    private int printedLines = 0;

    void Awake()
    {
        InitializeTerminal();
    }

    public void InitializeTerminal()
    {
        //ClearConsole();
        currentInput.onSubmit.AddListener(OnEnterInput);
        currentInput.onValueChanged.AddListener(OnValueChanged);
        inputStartPos = currentInput.transform.position;
        FocusInput();
    }

    private void OnDisable()
    {
        currentInput.onValueChanged.RemoveListener(OnValueChanged);
        currentInput.onSubmit.RemoveListener(OnEnterInput);
    }

    private void Update()
    {
        scrollTimer += Time.deltaTime;
        Scroll();
    }

    private void OnValueChanged(string val)
    {
        if (printedLines >= autoScrollLinesLimit)
        {
            while (printedLines >= autoScrollLinesLimit)
                Scroll(new Vector2Int(0, 1));
        }
    }

    public void Scroll(Vector2Int? scrollInput = null)
    {
        var scrollConsoleInput = scrollInput ?? new Vector2Int();
        if (scrollInput == null)
        {
            var scrollUp = CanScrollUp() ? 1 : 0;
            var scrollDown = CanScrollDown() ? 1 : 0;
            scrollConsoleInput = new Vector2Int(scrollUp,scrollDown);
            if (scrollConsoleInput.x == scrollConsoleInput.y || scrollTimer < scrollCooldown)
                return;
        }

        scrollTimer = 0;
        printedLines += scrollConsoleInput.x == 1 ? 1 : (scrollConsoleInput.y == 1 ? -1 : 0);
        var dir = new Vector3(0, padding * (scrollConsoleInput.x == 1 ? 1 : -1), 0);
        currentInput.transform.position += dir;
        foreach (var textInput in TextInputs)
            textInput.transform.position += dir;
    }

    private bool CanScrollUp()
    {
        if (!TextInputs.Any())
            return false;
        
        return UnityEngine.Input.GetKey(KeyCode.DownArrow) && 
               Vector2.Distance(TextInputs[0].transform.position, inputStartPos) > 0.1f;
    }
    private bool CanScrollDown()
    {
        var posToCheck = currentInput.transform.position; //TextInputs.Any() ? TextInputs[0].transform.position : currentInput.transform.position;

        return Input.GetKey(KeyCode.UpArrow) && printedLines >= autoScrollLinesLimit;
        //Vector2.Distance(posToCheck, inputStartPos) > 0.1f;
    }

    public void FocusInput()
    {
        currentInput.Select();
        currentInput.ActivateInputField();
        currentInput.text = "";
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
        var currentInputTransform = currentInput.transform;
        var log = Instantiate(inputPrefab, currentInputTransform.position, Quaternion.identity, contentParent);
        var lines = log.GetComponent<SubmitedTextBehaviour>().SetTextField(value, nextLineCharactersCount, asText);
        TextInputs.Add(log);

        currentInputTransform.position += new Vector3(0, padding * lines, 0);
        FocusInput();

        printedLines += lines;
        if (printedLines >= autoScrollLinesLimit)
        {
            while (printedLines >= autoScrollLinesLimit)
                Scroll(new Vector2Int(0, 1));
        }
    }

    public void ClearConsole()
    {
        printedLines = 0;
        foreach (var textInput in TextInputs)
        {
            Destroy(textInput);
        }
        TextInputs.Clear();

        currentInput.transform.position = inputStartPos;
    }

    //todo: ErrorComandNotRecognized
    public void Error_CommandNotRecognized(string value)
    {
        
    }
    
    public void Error_CustomError(string message)
    {
        Print($"<color=red>{message}</color>", true);
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
