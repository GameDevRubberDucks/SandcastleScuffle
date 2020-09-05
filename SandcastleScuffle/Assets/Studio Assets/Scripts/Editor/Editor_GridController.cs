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

        // Add buttons to create and destroy the grid
        GUILayout.Space(5.0f);
        if (GUILayout.Button("Generate Grid"))
            m_targetScript.GenerateGrid();
        if (GUILayout.Button("Clear Grid"))
            m_targetScript.ClearGrid();
    }
}
