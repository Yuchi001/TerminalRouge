
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Single/AllMethods", fileName = "new AllMethods")]
public sealed class SOAllMethods : UnityEngine.ScriptableObject
{
    public List<SOMethod> AllMethods = new List<SOMethod>();
}