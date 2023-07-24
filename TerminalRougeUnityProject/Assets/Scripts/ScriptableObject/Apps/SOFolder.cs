using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObject.Apps
{
    [CreateAssetMenu(fileName = "new folder", menuName = "Single/Folder", order = 0)]
    public class SOFolder : SOApp
    {
        public List<SOApp> apps;
    }
}