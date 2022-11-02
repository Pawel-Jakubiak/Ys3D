using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISensor : MonoBehaviour
{
    public event Action OnPlayerFound;

    public Transform target;
    [SerializeField] protected Entity entity;
    public SensorType sensorType = SensorType.HIT;
    public float detectionRadius;
    public float redetectionRate;
    public int detectionAngle;
    public LayerMask detectionLayer;

    public bool playerFound = false;

    [SerializeField] private bool showGizmos = false;
    [SerializeField] private Color gizmoIdleColor = Color.green;
    [SerializeField] private Color gizmoDetectedColor = Color.red;

    private void Awake()
    {
        entity = GetComponent<Entity>();
    }

    private void FixedUpdate()
    {
        if (!playerFound)
            if (sensorType != SensorType.HIT)
                Detect();
    }

    public void Detect()
    {
        Collider[] colliders = new Collider[0];

        if (sensorType == SensorType.BOX)
            colliders = Physics.OverlapBox(transform.position, Vector3.one * detectionRadius, Quaternion.identity, detectionLayer);

        if (sensorType == SensorType.CIRCLE)
            colliders = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

        if (sensorType == SensorType.CONE)
        {
            Collider[] sphereCast = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayer);

            if (sphereCast.Length != 0)
            {
                Transform found = sphereCast[0].transform;
                Vector3 direction = (found.position - transform.position).normalized;

                if (Vector3.Angle(entity.GetModelTransform().forward, direction) < detectionAngle / 2)
                    colliders = sphereCast;
            }
        }


        if (colliders.Length == 0)
            return;

        SetTarget(colliders[0].transform);
    }

    public void SetTarget(Transform objectTransform)
    {
        if (playerFound) return;

        target = objectTransform;
        playerFound = true;

        OnPlayerFound.Invoke();
    }

    public enum SensorType
    {
        HIT = 0,
        CIRCLE = 1,
        BOX = 2,
        CONE = 3
    }
}
