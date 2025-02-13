using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[Serializable]
public class Skill
{
    public string name;
    public int damage;
    public int hitChance;
    public int critChance;
    public int damageModifier;
    public Sprite skillIcon; // Unique icon for each skill
}

[Serializable]
public class Character
{
    public string name;
    public int health;
    public int maxHealth;
    public int attack;
    public int defense;
    public int speed;
    public List<Skill> skills = new List<Skill>();
    public Sprite characterIconSprite; // Unique sprite for each character
    public GameObject characterPrefab;
    public int index;
    public bool isEnemy = false; // Domyślnie fałsz, ustawiamy na true dla przeciwników


    public bool IsAlive()
    {
        return health > 0;
    }

    public void Attack(Character target, Skill skill)
    {
        if (UnityEngine.Random.Range(0, 100) <= skill.hitChance)
        {
            bool isCrit = UnityEngine.Random.Range(0, 100) <= skill.critChance;
            int damage = skill.damage;

            int modifier = UnityEngine.Random.Range(-skill.damageModifier, skill.damageModifier + 1);
            damage += damage * modifier / 100;

            if (isCrit)
            {
                damage = (int)(damage * 1.5f);
                Debug.Log($"{name} dealt a critical hit!");
            }

            damage -= target.defense;
            damage = Mathf.Max(damage, 0);

            target.health -= damage;
            Debug.Log($"{name} used {skill.name} on {target.name}, dealing {damage} damage!");

            if (!target.IsAlive())
            {
                Debug.Log($"{target.name} has been defeated!");
            }
        }
        else
        {
            Debug.Log($"{name} missed the attack using {skill.name}!");
        }
    }
    public void TakeDamage(int damage)
    {
        damage = Mathf.Max(damage - defense, 0); // Zmniejszenie obrażeń o obronę, minimum 0
        health -= damage;

        Debug.Log($"{name} took {damage} damage! Remaining HP: {health}");

        if (!IsAlive())
        {
            Debug.Log($"{name} has been defeated!");
        }
    }

}
[Serializable]
public class Enemy : Character
{
    // Enemy-specific properties or overrides can be added here if necessary
    // For now, it simply inherits from Character with isEnemy set to true
    public Enemy()
    {
        isEnemy = true; // All instances of Enemy are considered enemies by default
    }
}