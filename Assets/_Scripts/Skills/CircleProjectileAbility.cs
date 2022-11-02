using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/AoE/Circle Projectiles")]
public class CircleProjectileAbility : Ability
{
    public GameObject projectile;
    public int projectileCount;
    public float projectileDelay;
    public float radius;
    public float repeatRate;
    public float wavesCount;
    public float wavesDelay;


    private void OnEnable()
    {
        channelTime = (wavesCount * wavesDelay) + (projectileCount * projectileDelay);
    }

    public override void Use(Transform transform, Transform target)
    {
        float random = 0;

        _coroutine = GameController.instance.StartCoroutine(Activate());

        IEnumerator Activate()
        {
            for (int i = 0; i < wavesCount; i++)
            {
                yield return SpawnProjectiles();

                yield return new WaitForSeconds(wavesDelay);
            }
        }

        IEnumerator SpawnProjectiles()
        {
            for (int i = 0; i < projectileCount; i++)
            {
                float radians = 2 * Mathf.PI / projectileCount * i;

                float vertical = Mathf.Sin(radians);
                float horizontal = Mathf.Cos(radians);

                Vector3 spawnDirection = Quaternion.Euler(0f, random, 0f) * new Vector3(horizontal, 0f, vertical);

                Vector3 spawnPosition = transform.position + spawnDirection * radius;

                GameObject _projectile = Instantiate(projectile, spawnPosition, Quaternion.identity);

                _projectile.transform.forward = spawnDirection;

                yield return new WaitForSeconds(projectileDelay);
            }

            random += 30;
        }
    }
}
