using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(PlayerController))]
public class PlayerControllerEditor : EntityEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        PlayerController player = (PlayerController)target;

        serializedObject.Update();
        Undo.RecordObject(player, "Updating player.");

        GUIStyle centeredLabel = new GUIStyle(EditorStyles.boldLabel);
        centeredLabel.alignment = TextAnchor.MiddleCenter;

        EditorGUILayout.LabelField("Collisions, Stances", centeredLabel);

        GUIStyle stanceStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);

        stanceStyle.alignment = TextAnchor.MiddleCenter;
        stanceStyle.fontSize = 12;
        stanceStyle.fontStyle = FontStyle.Bold;
        stanceStyle.normal.textColor = player.defensiveStance ? Color.green : Color.red;

        EditorGUILayout.LabelField(player.defensiveStance ? "Defensive Stance" : "Offensive Stance", stanceStyle);

        EditorGUILayout.BeginHorizontal();

        SerializedProperty defensiveSpeedMultiplier = serializedObject.FindProperty("defensiveSpeedMultiplier");

        defensiveSpeedMultiplier.floatValue = EditorGUILayout.IntSlider("Def. Stance Slow", Mathf.RoundToInt(player.defensiveSpeedMultiplier * 100), 1, 100) / 100f;
        EditorGUILayout.LabelField("%", GUILayout.Width(15));

        EditorGUILayout.EndHorizontal();

        SerializedProperty collisionLayerMask = serializedObject.FindProperty("collisionLayerMask");

        collisionLayerMask.intValue = EditorGUILayout.MaskField("Collision Mask", player.collisionLayerMask, InternalEditorUtility.layers);

        serializedObject.ApplyModifiedProperties();
    }
}
