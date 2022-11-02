using UnityEngine;
using UnityEngine.UI;

public class HealthbarScript : MonoBehaviour
{
    [SerializeField] private Image healthbarFillImage;

    public void UpdateHealthbar(int current, int max)
    {
        healthbarFillImage.fillAmount = (float)current / max;
    }
}
