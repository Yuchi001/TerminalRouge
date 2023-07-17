using System;
using UnityEngine;

public static class StringExtensions
{
    public static string Capitalize(this string s)
    {
        if (String.IsNullOrEmpty(s)) {
            Debug.LogError($"Given string: {s} was null or empty.");
            return s;
        }
 
        return s[0].ToString().ToUpper() + s.Substring(1);
    }

    public static string Reverse(this string s)
    {
        char[] charArray = s.ToCharArray();
        Array.Reverse(charArray);
        return new String(charArray);
    }

    public static string Color(this string s, EColorType colorType)
    {
        return $"<color={Colors.GetColorHex(colorType)}>{s}</color>";
    }
}