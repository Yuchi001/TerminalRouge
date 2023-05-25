using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class StatsDebuger : MonoBehaviour
{
    [SerializeField] private SOAllModifiersHolder allModifiers;
    [SerializeField] private Slider levelSlider;
    [SerializeField] private TMP_InputField levelInputField;
    [SerializeField] private TMP_Dropdown statTypeDropDown;
    [SerializeField] private TextMeshProUGUI statValue;
    [SerializeField] private TextMeshProUGUI nextLevelThresholdValue;

    private void Awake()
    {
        var options = Enum.GetNames(typeof(EStatType)).ToList();
        statTypeDropDown.AddOptions(options);
        statTypeDropDown.onValueChanged.AddListener(OnStatTypeChange);
        
        levelSlider.maxValue = 20;
        levelSlider.minValue = 1;
        levelSlider.value = 1;
        levelSlider.wholeNumbers = true;
        levelSlider.onValueChanged.AddListener(OnSliderChange);
        
        levelInputField.contentType = TMP_InputField.ContentType.IntegerNumber;
        levelInputField.text = "1";
        levelInputField.onValueChanged.AddListener(OnInputChange);
        
        OnLevelChange();
    }

    private void OnDisable()
    {
        levelInputField.onValueChanged.RemoveAllListeners();
        levelSlider.onValueChanged.RemoveAllListeners();
    }

    private void OnSliderChange(float val)
    {
        var valInt = (int)val;
        levelInputField.text = valInt.ToString();
        
        OnLevelChange();
    }

    private void OnInputChange(string levelStr)
    {
        if (levelStr == "")
        {
            levelInputField.text = ((int)levelSlider.value).ToString();
            return;
        }
        
        var levelInt = Int32.Parse(levelStr);
        if (levelInt > 20)
        {
            levelInputField.text = ((int)levelSlider.value).ToString();
            return;
        }

        levelSlider.value = levelInt;
        
        OnLevelChange();
    }

    private void OnStatTypeChange(int index)
    {
        var enumVal = (EStatType)index;
        var nextLevelThreshold = allModifiers.GetStatThreshold(enumVal, (int)levelSlider.value - 1);
        var statVal = allModifiers.GetStatValue(enumVal, (int)levelSlider.value - 1);

        statValue.text = statVal.ToString();
        nextLevelThresholdValue.text = nextLevelThreshold.ToString();
    }

    private void OnLevelChange()
    {
        var enumVal = (EStatType)statTypeDropDown.value;
        var nextLevelThreshold = allModifiers.GetStatThreshold(enumVal, (int)levelSlider.value - 1);
        var statVal = allModifiers.GetStatValue(enumVal, (int)levelSlider.value - 1);

        statValue.text = statVal.ToString();
        nextLevelThresholdValue.text = nextLevelThreshold.ToString();
    }
}