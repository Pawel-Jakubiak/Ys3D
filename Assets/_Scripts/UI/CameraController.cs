using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private Transform target;
    private Vector3 velocity = Vector3.zero;
    public float smoothTime;

    void Awake()
    {
        Instance = this;
    }

    void FixedUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
    }

    public void Shake(float power, float length)
    {
        StartCoroutine(StartShake());

        IEnumerator StartShake()
        {
            float timer = 0f;

            while (timer <= length)
            {
                float x = target.position.x + Random.Range(-1f, 1f) * power;
                float z = target.position.z + Random.Range(-1f, 1f) * power;

                transform.position = new Vector3(x, 0f, z);

                timer += Time.deltaTime;

                yield return null;
            }
        }
    }
}
