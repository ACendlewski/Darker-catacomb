using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CharacterStatsUI : MonoBehaviour
{
    public TextMeshProUGUI characterNameText;
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI skillsText;

    public GameObject statsPanel;
    public GameObject skillButtonPrefab;
    public Transform skillsContainer;

    public Transform characterContainer;
    public Vector3 characterSpacing = new Vector3(100f, 0f, 0f);

    void Start()
    {
        if (CombatManager.Instance != null) { }
        if (TurnManager.Instance != null) { }

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
    }

    public void ShowCharacterStats(Character character)
    {
        if (statsPanel == null || character == null) return;
        statsPanel.SetActive(true);

        characterNameText.text = character.name;

        healthSlider.maxValue = character.maxHealth;
        healthSlider.value = character.health;
        healthText.text = $"{character.health} / {character.maxHealth}";

        attackText.text = "ATT: " + character.attack;
        defenseText.text = "DEF: " + character.defense;
        speedText.text = "SPD: " + character.speed;

        string prefabName = character.name.Replace("(Enemy) ", "").Trim();
        GameObject characterPrefab = Resources.Load<GameObject>("Prefabs/" + prefabName);

        if (characterPrefab == null)
        {
            Debug.LogError("Character prefab not found: " + prefabName);
            return;
        }

        GameObject existingCharacter = characterContainer.Find(character.name)?.gameObject;
        if (existingCharacter == null)
        {
            Debug.Log($"Instantiating character: {character.name}");
            GameObject characterObject = Instantiate(characterPrefab, characterContainer);
            characterObject.name = character.name;
            characterObject.transform.localPosition = character.index * characterSpacing;
        }

        // Usuń poprzednie przyciski skilli
        for (int i = skillsContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(skillsContainer.GetChild(i).gameObject);
        }

        // Sprawdź, czy prefab przycisku jest przypisany
        if (skillButtonPrefab == null)
        {
            Debug.LogError("Skill Button Prefab is not assigned!");
            return;
        }

        // Wyświetl skille
        if (character.skills != null && character.skills.Count > 0)
        {
            foreach (Skill skill in character.skills)
            {
                GameObject skillButton = Instantiate(skillButtonPrefab, skillsContainer);
                skillButton.name = skill.name;

                Button button = skillButton.GetComponent<Button>();
                Image iconImage = skillButton.GetComponent<Image>();

                if (iconImage != null)
                {
                    iconImage.sprite = skill.skillIcon;
                }

                button.onClick.AddListener(() => UseSkill(skill, character)); // Pass the character here
            }
        }
        else
        {
            skillsText.text = "Skills: None";
        }
    }

    public void UseSkill(Skill skill, Character character)
    {
        if (CombatManager.Instance != null)
        {
            CombatManager.Instance.ExecuteAction(character, skill); // Change UseSkill to ExecuteAction
        }
        else
        {
            Debug.LogError("CombatManager instance is missing!");
        }
    }

    public void HideCharacterStats()
    {
        if (statsPanel != null)
        {
            statsPanel.SetActive(false);
        }
    }

    public void DisplayActionFeedback(string message)
    {
        Debug.Log(message);
    }
}
