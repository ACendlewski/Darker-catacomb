using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Metoda, która będzie wywoływana przez skrypt CharacterStatsUI
    public void UseSkill(Skill skill)
    {
        // Wykonaj akcję skilla
        Debug.Log("Wykonano akcję skilla: " + skill.name);
        // ...
    }
}