using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    // Singleton instance
    public static CombatManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    public void ExecuteAction(Character attacker, Skill skill)
    {
        Debug.Log($"{attacker.name} is using {skill.name}!");

        Character target = FindTarget(attacker);
        if (target == null)
        {
            Debug.Log("No valid target found.");
            return;
        }

        // Obliczanie trafienia
        if (UnityEngine.Random.Range(0, 100) > skill.hitChance)
        {
            Debug.Log($"{attacker.name} missed {skill.name} on {target.name}!");
            return;
        }

        // Obliczanie obrażeń
        int damage = skill.damage;
        int modifier = UnityEngine.Random.Range(-skill.damageModifier, skill.damageModifier + 1);
        damage += damage * modifier / 100;

        bool isCrit = UnityEngine.Random.Range(0, 100) <= skill.critChance;
        if (isCrit)
        {
            damage = (int)(damage * 1.5f);
            Debug.Log($"{attacker.name} dealt a critical hit!");
        }

        // Zastosowanie obrażeń
        target.TakeDamage(damage);
    }

    Character FindTarget(Character attacker)
    {
        List<Character> potentialTargets = attacker.isEnemy ? CharacterManager.Instance.selectedCharacters : TurnManager.Instance.enemyTeam;

        foreach (Character target in potentialTargets)
        {
            if (target.IsAlive()) return target;
        }
        return null;
    }

}
