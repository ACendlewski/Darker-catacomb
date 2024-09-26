using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CharacterSelectionUI : MonoBehaviour
{
    public CharacterLoader characterLoader;
    public int maxSelection = 4;
    public Transform characterPanel;
    public GameObject characterButtonPrefab;
    public Button startBattleButton;

    public Transform selectedCharacterPanel;
    public GameObject selectedCharacterButtonPrefab;

    // Panel stat
    public GameObject characterStatsPanel;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI speedText;

    void Start()
    {
        if (characterLoader == null)
        {
            Debug.LogError("CharacterLoader is not assigned!");
            return;
        }

        StartCoroutine(WaitForCharactersToLoad());
    }

    IEnumerator WaitForCharactersToLoad()
    {
        while (characterLoader.characters == null || characterLoader.characters.Count == 0)
        {
            yield return null;
        }

        CreateCharacterButtons();
        UpdateSelectedCharacterButtons();
        startBattleButton.interactable = false;


        characterStatsPanel.SetActive(false);
    }

    void CreateCharacterButtons()
    {
        if (characterPanel == null)
        {
            Debug.LogError("CharacterPanel is not assigned!");
            return;
        }

        if (characterButtonPrefab == null)
        {
            Debug.LogError("CharacterButtonPrefab is not assigned!");
            return;
        }

        foreach (Character character in characterLoader.characters)
        {
            GameObject buttonObj = Instantiate(characterButtonPrefab, characterPanel);
            Button button = buttonObj.GetComponent<Button>();

            if (button == null)
            {
                Debug.LogError("Button component is missing on the prefab!");
                continue;
            }

            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText == null)
            {
                Debug.LogError("TextMeshProUGUI component is missing on the prefab!");
                continue;
            }

            buttonText.text = character.name;


            button.onClick.AddListener(() => SelectCharacter(character));


            EventTrigger trigger = buttonObj.AddComponent<EventTrigger>();
            EventTrigger.Entry enterEvent = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enterEvent.callback.AddListener((data) => ShowCharacterStats(character));
            trigger.triggers.Add(enterEvent);

            EventTrigger.Entry exitEvent = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exitEvent.callback.AddListener((data) => HideCharacterStats());
            trigger.triggers.Add(exitEvent);
        }
    }

    public void SelectCharacter(Character character)
    {
        if (CharacterManager.Instance == null || CharacterManager.Instance.selectedCharacters == null)
        {
            Debug.LogError("CharacterManager.Instance or selectedCharacters is null!");
            return;
        }

        if (CharacterManager.Instance.selectedCharacters.Count < maxSelection)
        {
            CharacterManager.Instance.AddCharacter(character);
            Debug.Log(character.name + " has been selected!");

            UpdateSelectedCharacterButtons();

            if (CharacterManager.Instance.selectedCharacters.Count == maxSelection)
            {
                startBattleButton.interactable = true;
            }
        }
        else
        {
            Debug.Log("You have already selected the maximum number of characters.");
        }
        DisplaySelectedCharactersInConsole();
    }

    void UpdateSelectedCharacterButtons()
    {

    }


    public void ShowCharacterStats(Character character)
    {
        characterStatsPanel.SetActive(true);
        characterNameText.text = "Name: " + character.name;
        healthText.text = "Health: " + character.health;
        attackText.text = "Attack: " + character.attack;
        defenseText.text = "Defense: " + character.defense;
        speedText.text = "Speed: " + character.speed;
    }


    public void HideCharacterStats()
    {

    }

    void DisplaySelectedCharactersInConsole()
    {
        if (CharacterManager.Instance.selectedCharacters == null || CharacterManager.Instance.selectedCharacters.Count == 0)
        {
            Debug.Log("No characters selected.");
            return;
        }

        string selectedCharacters = "Selected characters: ";
        foreach (Character character in CharacterManager.Instance.selectedCharacters)
        {
            selectedCharacters += character.name + ", ";
        }

        selectedCharacters = selectedCharacters.TrimEnd(',', ' ');

        Debug.Log(selectedCharacters);
    }

    public void StartBattle()
    {
        if (CharacterManager.Instance.selectedCharacters.Count == maxSelection)
        {
            SceneManager.LoadScene("BattleScene");
        }
        else
        {
            Debug.Log("You must select " + maxSelection + " characters to start the battle.");
        }
    }
}
