using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    private bool isActionComplete = false;

    void Awake()
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

    public bool IsActionComplete()
    {
        return isActionComplete;
    }

    public void MarkActionComplete()
    {
        isActionComplete = true;
    }

    public void ResetActionState()
    {
        isActionComplete = false;
    }

    public void ExecuteAction(Character character, Skill skill, Character target)

    {
        if (character == null)
        {
            Debug.LogError("Character is null in ExecuteAction");
            return;
        }

        Debug.Log($"{character.name} is using {skill.name}!");
        ResetActionState();
        StartCoroutine(PerformAction(character, skill, target));
    }

    private IEnumerator PerformAction(Character character, Skill skill, Character target)
    {
        if (target == null)

        {
            Debug.LogWarning("No target specified for action");
            MarkActionComplete();
            yield break;
        }

        if (UnityEngine.Random.Range(0, 100) > skill.hitChance)
        {
            Debug.Log($"{character.name} missed {skill.name} on {target.name}!");
            MarkActionComplete();
            yield break;
        }

        // Apply damage
        int damage = skill.damage;
        int modifier = UnityEngine.Random.Range(-skill.damageModifier, skill.damageModifier + 1);
        damage += damage * modifier / 100;

        bool isCrit = UnityEngine.Random.Range(0, 100) <= skill.critChance;
        if (isCrit)
        {
            damage = (int)(damage * 1.5f);
            Debug.Log($"{character.name} dealt a critical hit!");
        }

        if (target != null)
        {
            target.TakeDamage(damage);
            int remainingHP = target.health;

            Debug.Log($"{character.name} uses {skill.name} on {target.name}");
            Debug.Log($"Damage dealt: {damage}");
            Debug.Log($"{target.name}'s remaining HP: {remainingHP}");
        }

        // Simulate action duration
        yield return new WaitForSeconds(1.5f);
        
        if (target != null && !target.IsAlive())
        {
            Debug.Log($"{target.name} has died!");
            yield return new WaitForSeconds(1.5f);
        }
        
        MarkActionComplete();
    }
}
