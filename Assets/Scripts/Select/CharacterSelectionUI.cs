using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{
    public CharacterLoader characterLoader;
    public int maxSelection = 4;
    public Transform characterPanel;
    public GameObject characterButtonPrefab;
    public Button startBattleButton;

    public Transform selectedCharacterPanel;
    public GameObject selectedCharacterButtonPrefab;

    // Panel statystyk
    public GameObject characterStatsPanel;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI skillsText;
    public Transform skillsContainer;

    // Kontener postaci i podgląd awatara
    public Transform characterContainer;
    private List<KeyValuePair<Character, GameObject>> activeCharacterPreviews = new List<KeyValuePair<Character, GameObject>>();
    public Vector3 characterSpacing = new Vector3(2f, 0f, 0f); // Odstęp 2 jednostek w poziomie

    private GameObject currentCharacterPreview; // Przechowywanie instancji aktualnego podglądu postaci

    void Start()
    {
        if (characterLoader == null)
        {
            Debug.LogError("CharacterLoader is not assigned!");
            return;
        }

        // Jeśli nie przypisano kontenera, szukamy go w scenie
        if (characterContainer == null)
        {
            GameObject characterContainerObject = GameObject.Find("Characters");
            if (characterContainerObject != null)
            {
                characterContainer = characterContainerObject.transform;
            }
            else
            {
                Debug.LogError("Character container not found in the scene!");
            }
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
        if (characterPanel == null || characterButtonPrefab == null)
        {
            Debug.LogError("CharacterPanel or CharacterButtonPrefab is not assigned!");
            return;
        }

        foreach (Character character in characterLoader.characters)
        {
            GameObject buttonObj = Instantiate(characterButtonPrefab, characterPanel);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (button == null || buttonText == null)
            {
                Debug.LogError("Button or TextMeshProUGUI component is missing on the prefab!");
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

            // Zaktualizuj podgląd z ostatnią wybraną postacią
            UpdateCharacterPreview(character);
        }
        else
        {
            Debug.Log("You have already selected the maximum number of characters.");
        }
        DisplaySelectedCharactersInConsole();
    }

    void UpdateCharacterPreview(Character character)
    {
        string prefabName = character.name.Replace("(Enemy) ", "").Trim();
        GameObject characterPrefab = Resources.Load<GameObject>("Prefabs/" + prefabName);

        if (characterPrefab == null)
        {
            Debug.LogError("Character prefab not found: " + prefabName);
            return;
        }

        // Obliczenie pozycji na podstawie liczby wybranych postaci
        int characterCount = activeCharacterPreviews.Count;
        Vector3 spawnPosition = characterContainer.position + characterSpacing * characterCount;

        GameObject characterObject = Instantiate(characterPrefab, characterContainer);
        characterObject.name = character.name;
        characterObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        characterObject.transform.position = spawnPosition;

        // Dodanie nowej instancji do listy
        activeCharacterPreviews.Add(new KeyValuePair<Character, GameObject>(character, characterObject));
        CharacterManager.Instance.characterCounts[character]++;
        // Removed recursive call to UpdateCharacterPreview
    }

    void UpdateSelectedCharacterButtons()
    {
        foreach (Transform child in selectedCharacterPanel)
        {
            Destroy(child.gameObject);
        }

        foreach (Character selectedCharacter in CharacterManager.Instance.selectedCharacters)
        {
            GameObject buttonObj = Instantiate(selectedCharacterButtonPrefab, selectedCharacterPanel);
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
            {
                buttonText.text = selectedCharacter.name;
            }

            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => RemoveCharacter(selectedCharacter));
            }
        }
    }

    public void RemoveCharacter(Character character)
    {

        {
            if (CharacterManager.Instance != null && CharacterManager.Instance.selectedCharacters.Contains(character))
            {
                CharacterManager.Instance.selectedCharacters.Remove(character);
                Debug.Log(character.name + " has been removed!");

                UpdateSelectedCharacterButtons();

                if (CharacterManager.Instance.selectedCharacters.Count < maxSelection)
                {
                    startBattleButton.interactable = false;
                }

                // Removing character previews from the scene

                for (int i = activeCharacterPreviews.Count - 1; i >= 0; i--)
                {
                    if (activeCharacterPreviews[i].Key == character)
                    {
                        GameObject objToDestroy = activeCharacterPreviews[i].Value;

                        // Check if the object exists in the scene before destroying

                        if (objToDestroy != null)
                        {
                            Debug.Log("Destroying character prefab: " + objToDestroy.name);
                            Destroy(objToDestroy);
                        }

                        // Remove the entry from the list

                        activeCharacterPreviews.RemoveAt(i);
                    }
                }

                // Update positions of remaining character previews

                for (int i = 0; i < activeCharacterPreviews.Count; i++)
                {
                    if (activeCharacterPreviews[i].Value != null)
                    {
                        activeCharacterPreviews[i].Value.transform.position =
                            characterContainer.position + characterSpacing * i;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Attempted to remove character {character?.name} that wasn't in selected characters list");
            }
        }
    }
    public void ShowCharacterStats(Character character)
    {
        if (character == null)
        {
            Debug.LogError("Character is null!");
            return;
        }
            characterStatsPanel.SetActive(true);
            characterNameText.text = "Name: " + character.name;
            healthText.text = "Health: " + character.health;
            attackText.text = "Attack: " + character.attack;
            defenseText.text = "Defense: " + character.defense;
            speedText.text = "Speed: " + character.speed;

            if (character.skills != null && character.skills.Count > 0)
            {
                skillsText.text = "Skills:\n";
                foreach (Skill skill in character.skills)
                {
                    skillsText.text += $"- {skill.name} (Dmg: {skill.damage}, Hit: {skill.hitChance}%, Crit: {skill.critChance}%, Mod: ±{skill.damageModifier}%)\n";
                }
            }
            else
            {
                skillsText.text = "Skills: None";
            }
    }
    public void HideCharacterStats()
    {
        characterStatsPanel.SetActive(false);
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
