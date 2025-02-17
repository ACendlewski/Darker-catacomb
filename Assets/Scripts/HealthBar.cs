using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;
    private bool isMaxHealthSet = false; // 🛑 Flaga do ustawiania maxHealth tylko raz

    public void SetMaxHealth(int maxHealth)
    {
        if (slider == null)
        {
            Debug.LogError("[HealthBar] Slider is NULL! Sprawdź, czy pasek zdrowia ma przypisany komponent Slider.");
            return;
        }

        if (!isMaxHealthSet) // 🔥 Ustawiamy tylko raz!
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
            isMaxHealthSet = true;
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        if (slider == null)
        {
            Debug.LogError("[HealthBar] Slider is NULL! Sprawdź, czy pasek zdrowia ma przypisany komponent Slider.");
            return;
        }

        slider.value = currentHealth;
    }
}


// Compare this snippet from Select/CharacterSelectionUI.cs:
