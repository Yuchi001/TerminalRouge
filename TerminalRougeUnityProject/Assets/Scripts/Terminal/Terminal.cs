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
    [SerializeField, Range(-5, -0.1f)] private float padding = -0.5f;
    [SerializeField, Range(0, 1)] private float scrollCooldown = 0.1f;
    [SerializeField] private int autoScrollLinesLimit = 15;
    [SerializeField] private Scrollbar scrollbar;
    [SerializeField] private SAllErrors Errors;

    private float scrollTimer = 0;
    private Vector2 inputStartPos;
    private List<SOMethod> AllMethods => allMethodsSO.AllMethods;
    private List<GameObject> TextInputs = new List<GameObject>();
    private int printedLines = 0;
    private int totalPrintedLines = 0;
    private int visibleLinesCount = 0;

    private float lastScrollValue = 0;

    private int scrollNumOfSteps
    {
        get => totalPrintedLines - visibleLinesCount + 1;
    }

    void Awake()
    {
        InitializeTerminal();
    }

    public void InitializeTerminal()
    {
        //ClearConsole();
        OnLinesPrinted(0, true);
        currentInput.onSubmit.AddListener(OnEnterInput);
        currentInput.onValueChanged.AddListener(OnValueChanged);
        scrollbar.onValueChanged.AddListener(OnScrollBarValueChanged);
        inputStartPos = currentInput.transform.position;
        FocusInput();
    }

    private void OnDisable()
    {
        currentInput.onValueChanged.RemoveListener(OnValueChanged);
        currentInput.onSubmit.RemoveListener(OnEnterInput);
        scrollbar.onValueChanged.RemoveListener(OnScrollBarValueChanged);
    }

    private void Update()
    {
        Scroll();
        //Debug.Log($"ScrollNum: {scrollNumOfSteps}, total: {totalPrintedLines}, visible: {visibleLinesCount}, variation1: {totalPrintedLines - printedLines}");
    }

    private void OnValueChanged(string val)
    {
        if (printedLines >= autoScrollLinesLimit)
        {
            while (printedLines >= autoScrollLinesLimit)
                Scroll(new Vector2Int(0, 1));
        }
    }

    private void OnLinesPrinted(int lines, bool set)
    {
        totalPrintedLines += set ? -totalPrintedLines + lines : lines;
        scrollbar.size = totalPrintedLines < autoScrollLinesLimit ? 1 : 1.0f - (1.0f - (float)visibleLinesCount / totalPrintedLines);
        printedLines += lines;
        var newVisibleLinesCount = visibleLinesCount + lines;
        visibleLinesCount = newVisibleLinesCount > autoScrollLinesLimit ? autoScrollLinesLimit : newVisibleLinesCount;
        if (printedLines >= autoScrollLinesLimit)
            return;
        
        while (printedLines >= autoScrollLinesLimit)
            Scroll(new Vector2Int(0, 1));
    }

    public void OnScrollBarValueChanged(float value)
    {
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.UpArrow))
        {
            scrollbar.SetValueWithoutNotify(value);
            return;
        }
        
        var step = Mathf.RoundToInt(scrollbar.value * scrollNumOfSteps);
        var lastStep = Mathf.RoundToInt(lastScrollValue * scrollNumOfSteps);
        if (step == lastStep)
            return;

        if ((!CanScrollDown(true) && step < lastStep) 
            || (!CanScrollUp(true) && step > lastStep)
            || scrollNumOfSteps == 0)
        {
            if (scrollbar.value is 0 or 1) lastScrollValue = scrollbar.value;
            return;
        }
        
        if (step == lastStep) lastScrollValue = scrollbar.value;

        //Debug.Log($"Step: {step}, StepCount: {scrollNumOfSteps}");
        var dir = step > lastStep ? 1 : -1;
        for(var i = lastStep + dir; (i <= step && dir == 1) || (i >= step && dir == -1); i+=dir)
        {
            var newValue = (float)i / scrollNumOfSteps;
            scrollbar.SetValueWithoutNotify(newValue);
            var scrollDir = new Vector2Int(dir == 1 ? 1 : 0, dir == -1 ? 1 : 0);
            Scroll(scrollDir, false);
            lastScrollValue = newValue;   
        }

        if (scrollbar.value is 0 or 1) lastScrollValue = scrollbar.value;
        //Debug.Log($"EVENT\nLast: {lastScrollValue}, Current: {scrollbar.value}, LastStep:{lastStep}, Step:{step}");
    }
    
    private void SetScrollValue_KeyboardInput(Vector2Int scrollConsoleInput)
    {
        if (totalPrintedLines - printedLines <= 0 || scrollNumOfSteps == 0)
            return;
        
        var currentStep = Mathf.RoundToInt(scrollbar.value * scrollNumOfSteps);
        var stepPoint = scrollConsoleInput.x != 0 ? 1 : (scrollConsoleInput.y != 0 ? -1 : 0);
        var newValue = ((float)currentStep + stepPoint) / scrollNumOfSteps;
        if (newValue is < 0 or > 1)
            return;
        
        Debug.Log(currentStep);
        
        if (!CanScrollUp(true))
        {
            scrollbar.SetValueWithoutNotify(1);
            lastScrollValue = 1;
            //Debug.Log($"KEYBOARD\nLast: {lastScrollValue}, Current: {scrollbar.value}, LastStep:{0}, Step:{0}");
            return;
        }
        if (!CanScrollDown(true))
        {
            scrollbar.SetValueWithoutNotify(0);
            lastScrollValue = 0;
            //Debug.Log($"KEYBOARD\nLast: {lastScrollValue}, Current: {scrollbar.value}, LastStep:{0}, Step:{0}");
            return;
        }
        
        scrollbar.SetValueWithoutNotify(newValue);
        lastScrollValue = newValue;
        //Debug.Log($"KEYBOARD\nLast: {lastScrollValue}, Current: {scrollbar.value}, LastStep:{currentStep}, Step:{currentStep + stepPoint}");
    }

    public void ScrollButton(int dir)
    {
        if (dir is not 1 and not -1)
            return;

        if ((dir == 1 && !CanScrollUp(true)) || (dir == -1 && !CanScrollDown(true)))
            return;

        var currentStep = Mathf.RoundToInt(scrollbar.value * scrollNumOfSteps);
        var nextStep = currentStep + dir;
        var nextStepValue = (float)nextStep / scrollNumOfSteps;
        scrollbar.value = nextStepValue;
        Debug.Log(nextStepValue);
    }

    public void Scroll(Vector2Int? scrollInput = null, bool setScrollValue = true)
    {
        scrollTimer += Time.deltaTime;
        var scrollConsoleInput = scrollInput ?? new Vector2Int();
        if (scrollInput == null)
        {
            var scrollUp = CanScrollUp() ? 1 : 0;
            var scrollDown = CanScrollDown() ? 1 : 0;
            scrollConsoleInput = new Vector2Int(scrollUp,scrollDown);

            if (!CanScrollUp(true))
            {
                scrollbar.SetValueWithoutNotify(1);
                lastScrollValue = 1;
            }
            else if (!CanScrollDown(true))
            {
                scrollbar.SetValueWithoutNotify(0);
                lastScrollValue = 0;
            }
            
            if (scrollConsoleInput.x == scrollConsoleInput.y || scrollTimer < scrollCooldown)
                return;
        }

        if (setScrollValue && scrollConsoleInput.x != scrollConsoleInput.y)
        {
            SetScrollValue_KeyboardInput(scrollConsoleInput);
        }

        scrollTimer = 0;
        printedLines += scrollConsoleInput.x == 1 ? 1 : (scrollConsoleInput.y == 1 ? -1 : 0);
        var dir = new Vector3(0, padding * (scrollConsoleInput.x == 1 ? 1 : -1), 0);
        currentInput.transform.position += dir;
        foreach (var textInput in TextInputs)
            textInput.transform.position += dir;
    }

    private bool CanScrollUp(bool skipKeyCheck = false)
    {
        if (!TextInputs.Any())
            return false;
        
        return (Input.GetKey(KeyCode.DownArrow) || skipKeyCheck) && 
               Vector2.Distance(TextInputs[0].transform.position, inputStartPos) > 0.1f;
    }
    private bool CanScrollDown(bool skipKeyCheck = false)
    {
        return (Input.GetKey(KeyCode.UpArrow) || skipKeyCheck) && printedLines >= autoScrollLinesLimit;
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
        
        OnLinesPrinted(lines, false);
    }

    public void ClearConsole()
    {
        visibleLinesCount = 0;
        printedLines = 0;
        OnLinesPrinted(0, true);
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
