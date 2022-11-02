using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    public float speed;
    public float lifeTime = 5f;

    private void OnEnable()
    {
        transform.position += Vector3.up;
    }

    private void FixedUpdate()
    {
        transform.position += transform.forward * speed * Time.fixedDeltaTime;

        if (lifeTime <= 0f)
            Destroy(gameObject);

        lifeTime -= Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController _player = other.GetComponent<PlayerController>();

        _player.ApplyDamage(damage);
        UIController.Instance.effectSpawner.ShowHitEffect(_player.transform.position, true);

        Destroy(gameObject);
    }
}
