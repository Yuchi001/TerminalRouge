using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(SOMethod), true)]
    public class CustomEditorMethod : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var method = target as SOMethod;

            if(GUILayout.Button("Populate info lists")) // Custom button. Refresh local object stat list
            {
                method.PopulateInfoLists_EM();
            }
        }
    }
}