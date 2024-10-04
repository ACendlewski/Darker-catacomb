using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;  // Potrzebne do u�ywania korutyn

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

    // Domy�lne ustawienia
    private float defaultVolume;
    private bool defaultFullscreen;
    private int defaultWidth;
    private int defaultHeight;

    void Start()
    {
        // Zapisz domy�lne ustawienia d�wi�ku i pe�nego ekranu
        defaultVolume = volumeSlider.value;
        defaultFullscreen = Screen.fullScreen;

        // Ustaw najwi�ksz� dost�pn� rozdzielczo�� jako domy�ln�
        Resolution maxResolution = Screen.resolutions[Screen.resolutions.Length - 1];  // Najwi�ksza rozdzielczo��
        defaultWidth = maxResolution.width;
        defaultHeight = maxResolution.height;
        Screen.SetResolution(defaultWidth, defaultHeight, Screen.fullScreen);  // Ustaw rozdzielczo�� na najwi�ksz�

        // Ustaw warto�ci tekstowe i stany interfejsu u�ytkownika
        volumeText.text = "Volume: " + volumeSlider.value.ToString("F2");
        fullscreenToggle.isOn = Screen.fullScreen;
        widthInputField.text = defaultWidth.ToString();
        heightInputField.text = defaultHeight.ToString();

        // Ukryj tekst ostrzegawczy na starcie
        warningText.gameObject.SetActive(false);

        // Pod��cz funkcje do przycisk�w i zdarze�
        applyResolutionButton.onClick.AddListener(ApplyResolution);
        resetButton.onClick.AddListener(ResetToDefaults);
        volumeSlider.onValueChanged.AddListener(delegate { OnVolumeChanged(); });  // Aktualizacja tekstu przy zmianie g�o�no�ci
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);  // Ustaw pe�ny ekran
    }

    public void OnVolumeChanged()
    {
        float volumeValue = volumeSlider.value;
        volumeText.text = "Volume: " + volumeValue.ToString("F2");
    }

    // Funkcja wywo�ywana, gdy u�ytkownik pu�ci suwak
    public void OnVolumeSliderReleased()
    {
        float volumeValue = volumeSlider.value;
        Debug.Log("Volume changed to: " + volumeValue);
    }

    // Funkcja wywo�ywana przy zmianie trybu pe�noekranowego
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        Debug.Log("Fullscreen mode: " + isFullscreen);
    }

    public void ApplyResolution()
    {
        int width = int.Parse(widthInputField.text);
        int height = int.Parse(heightInputField.text);

        // Sprawdzenie czy rozdzielczo�� jest odpowiednia
        if (width > 800 && height > 600)
        {
            Screen.SetResolution(width, height, Screen.fullScreen);
            Debug.Log($"Resolution changed to: {width}x{height}");
        }
        else
        {
            Debug.LogError("Invalid resolution values!");
            StartCoroutine(ShowWarningText());  // Poka� ostrze�enie na 2 sekundy
        }
    }

    public void ResetToDefaults()
    {
        // Przywr�� domy�lne warto�ci
        volumeSlider.value = defaultVolume;
        fullscreenToggle.isOn = defaultFullscreen;
        widthInputField.text = defaultWidth.ToString();
        heightInputField.text = defaultHeight.ToString();

        // Aktualizuj zmiany na ekranie
        OnVolumeChanged();
        Screen.SetResolution(defaultWidth, defaultHeight, defaultFullscreen);

        Debug.Log("Settings have been reset to defaults.");
    }

    // Korutyna do pokazywania ostrze�enia przez 2 sekundy
    private IEnumerator ShowWarningText()
    {
        warningText.gameObject.SetActive(true);  // Poka� tekst ostrzegawczy
        yield return new WaitForSeconds(1f);  // Poczekaj 2 sekundy
        warningText.gameObject.SetActive(false);  // Ukryj tekst ostrzegawczy
    }
}
