using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    public EffectSpawner effectSpawner;
    [SerializeField] private Image playerHealthbar;

    private void Awake()
    {
        Instance = this;
        effectSpawner = GetComponent<EffectSpawner>();
    }

    public void UpdateHealthbar(int current, int max)
    {
        playerHealthbar.fillAmount = (float)current / max;
    }

    public static void UpdateHealthbar(Image image, int current, int max)
    {
        image.fillAmount = (float)current / max;
    }

    public void PlaySound()
    {
        GetComponent<AudioSource>().Play();
    }
}
