using UnityEditor;
using UnityEngine;


[CanEditMultipleObjects]
[CustomEditor(typeof(LongIconBarTool))]
public class OneLongBarToolEditorSet : Editor
{
    public override void OnInspectorGUI()
    {
        LongIconBarTool longIconBarTool = target as LongIconBarTool;

        base.OnInspectorGUI();


        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("RefreshUI"))
        {
            longIconBarTool.SetNowValue(longIconBarTool.NowTotalValue);
            longIconBarTool.SetMaxValue(longIconBarTool.MaxTotalValue);
            longIconBarTool.RefreshShowSpriteBar();
        }
        EditorGUILayout.EndHorizontal();
        if (Application.isPlaying) {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Test_AttackValue"))
            {
                longIconBarTool.SetNowValue(longIconBarTool.NowTotalValue - longIconBarTool.MaxTotalValue / 10);
            }
            if (GUILayout.Button("Test_AddValue"))
            {
                longIconBarTool.SetNowValue(longIconBarTool.NowTotalValue + longIconBarTool.MaxTotalValue / 10);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Test_AttackMaxValue"))
            {
                longIconBarTool.SetNowValue(0);
            }
            if (GUILayout.Button("Test_AddMaxValue"))
            {
                longIconBarTool.SetNowValue(longIconBarTool.MaxTotalValue);
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
