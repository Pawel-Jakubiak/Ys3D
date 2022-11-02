using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerHitEffect;
    [SerializeField] private GameObject enemyHitEffect;
    [SerializeField] private int poolAmount;

    [SerializeField] private List<GameObject> playerHitEffectPool = new List<GameObject>();
    [SerializeField] private List<GameObject> enemyHitEffectPool = new List<GameObject>();

    void Start()
    {
        CreatePool(playerHitEffectPool, playerHitEffect);
        CreatePool(enemyHitEffectPool, enemyHitEffect);
    }

    private void CreatePool(List<GameObject> pool, GameObject objectToPool)
    {
        GameObject temp;

        for (int i = 0; i < poolAmount; i++)
        {
            temp = Instantiate(objectToPool);
            temp.SetActive(false);
            pool.Add(temp);
        }
    }

    private GameObject GetPooledObject(List<GameObject> pool)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (!pool[i].activeSelf) return pool[i];
        }

        return null;
    }

    public void ShowHitEffect(Vector3 position, bool player = false)
    {
        GameObject pooled = GetPooledObject(player ? playerHitEffectPool : enemyHitEffectPool);

        if (!pooled) return;

        pooled.transform.position = position;
        pooled.SetActive(true);
    }
}
