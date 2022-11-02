using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Entity), true)]
public class EntityEditor : Editor
{
    private string[] knockToolbarOptions = { "Not knockbackable", "Knockbackable" };

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Entity entity = (Entity)target;

        GUIStyle centeredLabel = new GUIStyle(EditorStyles.boldLabel);
        centeredLabel.alignment = TextAnchor.MiddleCenter;

        // Direction and state styling

        GUIStyle dirStateStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);

        dirStateStyle.alignment = TextAnchor.MiddleRight;
        dirStateStyle.fontSize = 12;
        dirStateStyle.fontStyle = FontStyle.Bold;
        dirStateStyle.padding = new RectOffset(0, 40, 0, 0);

        EditorGUILayout.LabelField("State and Direction", centeredLabel);

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField(entity.state.ToString(), dirStateStyle);

        dirStateStyle.alignment = TextAnchor.MiddleLeft;

        EditorGUILayout.LabelField(entity.direction.ToString(), dirStateStyle);

        EditorGUILayout.EndHorizontal();

        // Stats styling

        EditorGUILayout.LabelField("Statistics", centeredLabel);

        SerializedProperty model = serializedObject.FindProperty("_model");
        model.objectReferenceValue = entity.transform.GetChild(0).transform;

        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Health", GUILayout.Width(120));

        SerializedProperty health = serializedObject.FindProperty("health");
        SerializedProperty maxHealth = serializedObject.FindProperty("maxHealth");

        health.intValue = EditorGUILayout.IntField(Mathf.Clamp(entity.health, 0, maxHealth.intValue));
        EditorGUILayout.LabelField("/", GUILayout.Width(5));
        maxHealth.intValue = EditorGUILayout.DelayedIntField(maxHealth.intValue);

        EditorGUILayout.EndHorizontal();

        SerializedProperty damage = serializedObject.FindProperty("damage");
        SerializedProperty moveSpeed = serializedObject.FindProperty("moveSpeed");

        damage.intValue = EditorGUILayout.IntField("Damage", entity.damage);
        moveSpeed.floatValue = EditorGUILayout.FloatField("Movement Speed", entity.moveSpeed);

        EditorGUILayout.Space(10);

        // Knockback styling

        EditorGUILayout.LabelField("Knockback", centeredLabel);

        SerializedProperty isKnockbackable = serializedObject.FindProperty("isKnockbackable");
        SerializedProperty knockbackStrength = serializedObject.FindProperty("knockbackStrength");


        isKnockbackable.boolValue = GUILayout.Toolbar(isKnockbackable.boolValue ? 1 : 0, knockToolbarOptions) == 1 ? true : false;

        //knockbackStrength.floatValue = EditorGUILayout.FloatField("Strength", entity.knockbackStrength);
        knockbackStrength.floatValue = EditorGUILayout.Slider("Strength", knockbackStrength.floatValue, .2f, 1.5f);

        EditorGUILayout.Space(10);

        serializedObject.ApplyModifiedProperties();
        Undo.RecordObject(entity, "Updating entity.");
    }
}
