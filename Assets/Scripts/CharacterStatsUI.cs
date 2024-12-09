using UnityEngine;
using TMPro;

public class CharacterStatsUI : MonoBehaviour
{
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenseText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI skillsText;

    public GameObject statsPanel; // Panel statów, który bêdzie wyœwietlany

    public void ShowCharacterStats(Character character)
    {
        statsPanel.SetActive(true);

        characterNameText.text =  character.name;
        healthText.text = "HP: " + character.health;
        attackText.text = "ATT: " + character.attack;
        defenseText.text = "DEF: " + character.defense;
        speedText.text = "SPD: " + character.speed;

        if (character.skills != null && character.skills.Count > 0)
        {
            skillsText.text = "Skills:\n";
            foreach (Skill skill in character.skills)
            {
                skillsText.text += $"- {skill.name} (Dmg: {skill.damage}, Hit: {skill.hitChance}%)\n";
            }
        }
        else
        {
            skillsText.text = "Skills: None";
        }
    }

    public void HideCharacterStats()
    {
        statsPanel.SetActive(false);
    }
}
