using UnityEngine;
using TMPro;
using UnityEngine.UI; // Added for UI components

public class CharacterStatsUI : MonoBehaviour
{
    public TextMeshProUGUI characterNameText;
    public Slider healthSlider; // Dodane pole dla paska zdrowia
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI skillsText;

    public GameObject statsPanel; // Panel for displaying stats
    public Transform skillsContainer; // Container for skill icons

    public Transform characterContainer;
    public Vector3 characterSpacing = new Vector3(100f, 0f, 0f);

    public void ShowCharacterStats(Character character)
    {
        statsPanel.SetActive(true);

        characterNameText.text = character.name;

        healthSlider.value = character.health;
        healthText.text = character.health.ToString();
        healthText.transform.position = new Vector3(healthSlider.transform.position.x, healthSlider.transform.position.y + 20f, healthSlider.transform.position.z);
        healthText.fontSize = 12f;

        attackText.text = "ATT: " + character.attack;
        defenseText.text = "DEF: " + character.defense;
        speedText.text = "SPD: " + character.speed;

        GameObject characterPrefab = Resources.Load<GameObject>(character.name);
        if (characterPrefab == null)
        {
            characterPrefab = Resources.Load<GameObject>("Hero");
        }
        if (characterPrefab != null)
        {
            GameObject characterObject = Instantiate(characterPrefab);
            characterObject.name = character.name;
        }

        GameObject characterPrefabObject = Instantiate(characterPrefab);
        characterPrefabObject.transform.SetParent(characterContainer);
        characterPrefabObject.transform.localPosition = character.index * characterSpacing;

        // Clear previous skill icons
        foreach (Transform child in skillsContainer)
        {
            Destroy(child.gameObject);
        }

        // Display skill icons
        if (character.skills != null && character.skills.Count > 0)
        {
            skillsText.text = "Skills:\n";
            foreach (Skill skill in character.skills)
            {
                // Utwórz przycisk skilla
                GameObject skillButton = new GameObject(skill.name);
                Button button = skillButton.AddComponent<Button>();
                button.onClick.AddListener(() => UseSkill(skill));
                button.transform.SetParent(skillsContainer);

                // Przypisz ikonę skilla do przycisku
                Image iconImage = skillButton.AddComponent<Image>();
                iconImage.sprite = skill.skillIcon;
            }
        }
        else
        {
            skillsText.text = "Skills: None";
        }
    }

    public void UseSkill(Skill skill)
    {
        // Wywołaj metodę w skrypcie CombatManager
        CombatManager.instance.UseSkill(skill);
    }
    public void HideCharacterStats()
    {
        statsPanel.SetActive(false);
    }

    // New method to display action feedback
    public void DisplayActionFeedback(string message)
    {
        // Implement a way to show feedback messages in the UI
        Debug.Log(message); // For now, just log it
    }
}
