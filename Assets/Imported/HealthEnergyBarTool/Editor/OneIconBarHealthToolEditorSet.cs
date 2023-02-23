using UnityEditor;
using UnityEngine;
 

[CanEditMultipleObjects]
[CustomEditor(typeof(MultipleIconValueBarTool))]

public class OneIconBarHealthToolEditorSet : Editor
{
    public override void OnInspectorGUI()
    {
        MultipleIconValueBarTool oneIconBarHealthTool = target as MultipleIconValueBarTool;

        base.OnInspectorGUI();


        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("SetInitAndRefreshUI"))
        {
            oneIconBarHealthTool.SetInit();
            oneIconBarHealthTool.RefreshUI();
        }
        // EditorGUILayout.BeginHorizontal();
        EditorGUILayout.EndHorizontal();
        
        if (Application.isPlaying)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("RefreshUI"))
            {
                oneIconBarHealthTool.RefreshUI();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Test_AttackValue"))
            {
                oneIconBarHealthTool.SetNowValue(oneIconBarHealthTool.NowTotalValue - 1);
            }
            if (GUILayout.Button("Test_AddValue"))
            {
                oneIconBarHealthTool.SetNowValue(oneIconBarHealthTool.NowTotalValue + 1);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Test_AttackBigValue"))
            {
                oneIconBarHealthTool.SetNowValue(0);
            }
            if (GUILayout.Button("Test_AddBigValue"))
            {
                oneIconBarHealthTool.SetNowValue(oneIconBarHealthTool.MaxTotalValue);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
