using System;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AISensor))]
public class AISensorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AISensor aiSensor = (AISensor)target;

        serializedObject.Update();
        Undo.RecordObject(aiSensor, "Changing AI sensor values.");

        string[] types = Enum.GetNames(typeof(AISensor.SensorType));

        // Detection settings
        EditorGUILayout.LabelField("Detection Settings", EditorStyles.centeredGreyMiniLabel);

        string[] masks = UnityEditorInternal.InternalEditorUtility.layers;

        SerializedProperty detectionLayer = serializedObject.FindProperty("detectionLayer");
        SerializedProperty sensorType = serializedObject.FindProperty("sensorType");

        detectionLayer.intValue = EditorGUILayout.MaskField("Masks", aiSensor.detectionLayer, masks);

        sensorType.enumValueIndex = GUILayout.Toolbar((int)aiSensor.sensorType, types, GUILayout.MinWidth(20));

        if (aiSensor.sensorType != AISensor.SensorType.HIT)
        {
            SerializedProperty detectionRadius = serializedObject.FindProperty("detectionRadius");
            SerializedProperty redetectionRate = serializedObject.FindProperty("redetectionRate");


            detectionRadius.floatValue = EditorGUILayout.FloatField("Radius", aiSensor.detectionRadius);
            redetectionRate.floatValue = EditorGUILayout.FloatField("Redetection Rate", aiSensor.redetectionRate);
        }

        if (aiSensor.sensorType == AISensor.SensorType.CONE)
        {
            SerializedProperty detectionAngle = serializedObject.FindProperty("detectionAngle");

            detectionAngle.intValue = EditorGUILayout.IntSlider(aiSensor.detectionAngle, 0, 360);
        }

        EditorGUILayout.Space();

        // Gizmo settings
        EditorGUILayout.LabelField("Gizmo Settings", EditorStyles.centeredGreyMiniLabel);

        SerializedProperty showGizmos = serializedObject.FindProperty("showGizmos");

        showGizmos.boolValue = EditorGUILayout.Toggle("Show Gizmos", showGizmos.boolValue);

        if (showGizmos.boolValue)
        {
            SerializedProperty gizmoIdleColor = serializedObject.FindProperty("gizmoIdleColor");
            SerializedProperty gizmoDetectedColor = serializedObject.FindProperty("gizmoDetectedColor");
            
            gizmoIdleColor.colorValue = EditorGUILayout.ColorField("Not detected color", gizmoIdleColor.colorValue);
            gizmoDetectedColor.colorValue = EditorGUILayout.ColorField("Detected color", gizmoDetectedColor.colorValue);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI()
    {
        AISensor aiSensor = (AISensor)target;

        SerializedProperty showGizmos = serializedObject.FindProperty("showGizmos");

        if (!showGizmos.boolValue) return;

        SceneView.RepaintAll();

        SerializedProperty gizmoIdleColor = serializedObject.FindProperty("gizmoIdleColor");
        SerializedProperty gizmoDetectedColor = serializedObject.FindProperty("gizmoDetectedColor");

        Transform transform = aiSensor.transform;
        Entity entity = serializedObject.FindProperty("entity").objectReferenceValue as Entity;
        AISensor.SensorType sensorType = aiSensor.sensorType;

        if (!aiSensor.playerFound)
            Handles.color = gizmoIdleColor.colorValue;
        else
            Handles.color = gizmoDetectedColor.colorValue;

        if (sensorType == AISensor.SensorType.BOX)
        {
            Handles.DrawWireCube(transform.position, Vector3.one * 2 * aiSensor.detectionRadius);
            return;
        }

        if (sensorType == AISensor.SensorType.CIRCLE)
        {
            Handles.DrawWireDisc(transform.position, Vector3.up, aiSensor.detectionRadius);
            return;
        }

        if (sensorType == AISensor.SensorType.CONE)
        {
            Transform modelTransform = aiSensor.GetComponent<Entity>().GetModelTransform();

            Vector3 firstAngle = AngleToVector3(modelTransform.eulerAngles.y, aiSensor.detectionAngle / 2);
            Vector3 secondAngle = AngleToVector3(modelTransform.eulerAngles.y, -aiSensor.detectionAngle / 2);

            Handles.DrawLine(transform.position, transform.position + firstAngle * aiSensor.detectionRadius);
            Handles.DrawLine(transform.position, transform.position + secondAngle * aiSensor.detectionRadius);

            Handles.DrawWireArc(transform.position, transform.up, secondAngle, aiSensor.detectionAngle, aiSensor.detectionRadius);
        }
    }

    private Vector3 AngleToVector3(float eulerY, float angle)
    {
        angle += eulerY;

        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
    }
}
