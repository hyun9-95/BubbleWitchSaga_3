using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BattleGrid))]
public class BattleGridEditor : Editor
{
    private BattleGrid battleGrid;

    private void OnEnable()
    {
        battleGrid = (BattleGrid)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Hexagon Direction Controls", EditorStyles.boldLabel);

        // 육각형 방향 버튼
        EditorGUILayout.BeginVertical();

        // TopLeft, TopRight
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("↖ TopLeft", GUILayout.Width(80), GUILayout.Height(30)))
            MoveTestCell(BattleCellDirection.TopLeft);

        GUILayout.Space(20);

        if (GUILayout.Button("↗ TopRight", GUILayout.Width(80), GUILayout.Height(30)))
            MoveTestCell(BattleCellDirection.TopRight);

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        // Left, Right
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("← Left", GUILayout.Width(80), GUILayout.Height(30)))
            MoveTestCell(BattleCellDirection.Left);

        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField($"Current: ({battleGrid.testCellPos.row}, {battleGrid.testCellPos.column})",
                                  EditorStyles.centeredGreyMiniLabel);
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("→ Right", GUILayout.Width(80), GUILayout.Height(30)))
            MoveTestCell(BattleCellDirection.Right);

        EditorGUILayout.EndHorizontal();

        // BottomLeft, BottomRigh
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("↙ BottomLeft", GUILayout.Width(80), GUILayout.Height(30)))
            MoveTestCell(BattleCellDirection.BottomLeft);

        GUILayout.Space(20);

        if (GUILayout.Button("↘ BottomRight", GUILayout.Width(80), GUILayout.Height(30)))
            MoveTestCell(BattleCellDirection.BottomRight);

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
    }

    private void MoveTestCell(BattleCellDirection direction)
    {
        CellPosition newPos = battleGrid.testCellPos;
        newPos.Move(direction);

        battleGrid.testCellPos = newPos;

        EditorUtility.SetDirty(battleGrid);

        SceneView.RepaintAll();
    }
}