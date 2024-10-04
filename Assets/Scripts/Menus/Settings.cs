using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;  // Potrzebne do u¿ywania korutyn

public class SettingsManager : MonoBehaviour
{
    public TMP_Text volumeText;
    public Slider volumeSlider;
    public Toggle fullscreenToggle;

    public TMP_InputField widthInputField;
    public TMP_InputField heightInputField;
    public Button applyResolutionButton;
    public Button resetButton;

    public TMP_Text warningText;  // Tekst ostrzegawczy

    // Domyœlne ustawienia
    private float defaultVolume;
    private bool defaultFullscreen;
    private int defaultWidth;
    private int defaultHeight;

    void Start()
    {
        // Zapisz domyœlne ustawienia dŸwiêku i pe³nego ekranu
        defaultVolume = volumeSlider.value;
        defaultFullscreen = Screen.fullScreen;

        // Ustaw najwiêksz¹ dostêpn¹ rozdzielczoœæ jako domyœln¹
        Resolution maxResolution = Screen.resolutions[Screen.resolutions.Length - 1];  // Najwiêksza rozdzielczoœæ
        defaultWidth = maxResolution.width;
        defaultHeight = maxResolution.height;
        Screen.SetResolution(defaultWidth, defaultHeight, Screen.fullScreen);  // Ustaw rozdzielczoœæ na najwiêksz¹

        // Ustaw wartoœci tekstowe i stany interfejsu u¿ytkownika
        volumeText.text = "Volume: " + volumeSlider.value.ToString("F2");
        fullscreenToggle.isOn = Screen.fullScreen;
        widthInputField.text = defaultWidth.ToString();
        heightInputField.text = defaultHeight.ToString();

        // Ukryj tekst ostrzegawczy na starcie
        warningText.gameObject.SetActive(false);

        // Pod³¹cz funkcje do przycisków i zdarzeñ
        applyResolutionButton.onClick.AddListener(ApplyResolution);
        resetButton.onClick.AddListener(ResetToDefaults);
        volumeSlider.onValueChanged.AddListener(delegate { OnVolumeChanged(); });  // Aktualizacja tekstu przy zmianie g³oœnoœci
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);  // Ustaw pe³ny ekran
    }

    public void OnVolumeChanged()
    {
        float volumeValue = volumeSlider.value;
        volumeText.text = "Volume: " + volumeValue.ToString("F2");
    }

    // Funkcja wywo³ywana, gdy u¿ytkownik puœci suwak
    public void OnVolumeSliderReleased()
    {
        float volumeValue = volumeSlider.value;
        Debug.Log("Volume changed to: " + volumeValue);
    }

    // Funkcja wywo³ywana przy zmianie trybu pe³noekranowego
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log("Fullscreen mode: " + isFullscreen);
    }

    public void ApplyResolution()
    {
        int width = int.Parse(widthInputField.text);
        int height = int.Parse(heightInputField.text);

        // Sprawdzenie czy rozdzielczoœæ jest odpowiednia
        if (width > 800 && height > 600)
        {
            Screen.SetResolution(width, height, Screen.fullScreen);
            Debug.Log($"Resolution changed to: {width}x{height}");
        }
        else
        {
            Debug.LogError("Invalid resolution values!");
            StartCoroutine(ShowWarningText());  // Poka¿ ostrze¿enie na 2 sekundy
        }
    }

    public void ResetToDefaults()
    {
        // Przywróæ domyœlne wartoœci
        volumeSlider.value = defaultVolume;
        fullscreenToggle.isOn = defaultFullscreen;
        widthInputField.text = defaultWidth.ToString();
        heightInputField.text = defaultHeight.ToString();

        // Aktualizuj zmiany na ekranie
        OnVolumeChanged();
        Screen.SetResolution(defaultWidth, defaultHeight, defaultFullscreen);

        Debug.Log("Settings have been reset to defaults.");
    }

    // Korutyna do pokazywania ostrze¿enia przez 2 sekundy
    private IEnumerator ShowWarningText()
    {
        warningText.gameObject.SetActive(true);  // Poka¿ tekst ostrzegawczy
        yield return new WaitForSeconds(1f);  // Poczekaj 2 sekundy
        warningText.gameObject.SetActive(false);  // Ukryj tekst ostrzegawczy
    }
}
