using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Enemy))]
public class EnemyEditor : EntityEditor
{
    public override void OnInspectorGUI()
    {
        Enemy enemy = (Enemy)target;

        base.OnInspectorGUI();

        serializedObject.Update();
        Undo.RecordObject(enemy, "Updating entity.");

        GUIStyle centeredLabel = new GUIStyle(EditorStyles.boldLabel);
        centeredLabel.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Wander", centeredLabel);

        SerializedProperty wanderRadius = serializedObject.FindProperty("wanderRadius");
        SerializedProperty idleTime = serializedObject.FindProperty("idleTime");


        wanderRadius.floatValue = EditorGUILayout.FloatField("Radius", enemy.wanderRadius);
        idleTime.floatValue = EditorGUILayout.FloatField("Idle Time", enemy.idleTime);

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Abilities", centeredLabel);

        SerializedProperty ability = serializedObject.FindProperty("ability");

        EditorGUILayout.ObjectField(ability);

        EditorGUILayout.Space(10);

        serializedObject.ApplyModifiedProperties();
    }
}
