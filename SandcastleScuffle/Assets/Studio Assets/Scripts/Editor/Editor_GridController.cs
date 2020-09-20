using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Grid_Controller))]
public class Editor_GridController : Editor
{
    //--- Private Variables ---//
    private Grid_Controller m_targetScript;

    //--- Unity Methods ---//
    public override void OnInspectorGUI()
    {
        // Store the target reference
        if (m_targetScript == null)
            m_targetScript = (Grid_Controller)target;

        // Draw the default inspector so all needed variables are visible
        DrawDefaultInspector();
        GUILayout.Space(5.0f);

        // Add buttons to create and destroy the grid
        if (GUILayout.Button("Generate Grid"))
        {
            Undo.RecordObject(m_targetScript, "Generated Grid");
            m_targetScript.GenerateGrid();
            PrefabUtility.RecordPrefabInstancePropertyModifications(m_targetScript);
        }
        if (GUILayout.Button("Clear Grid"))
        {
            Undo.RecordObject(m_targetScript, "Cleared Grid");
            m_targetScript.ClearGrid();
            PrefabUtility.RecordPrefabInstancePropertyModifications(m_targetScript);
        }
    }
}
