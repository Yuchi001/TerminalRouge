using UnityEngine;

[CreateAssetMenu(fileName = "new app", menuName = "Single/App", order = 0)]
public class SOApp : ScriptableObject
{
    public string appName;
    public Sprite appSprite;
    public bool isFolder = false;

    public GameObject appPrefab;
}