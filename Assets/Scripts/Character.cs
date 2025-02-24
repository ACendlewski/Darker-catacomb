using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Skill
{
    public string name;
    public int damage;
    public int hitChance;
    public int critChance;
    public int damageModifier;
    public Sprite skillIcon;
}

[Serializable]
public class Character
{
    public string name;
    public int health;
    public int maxHealth;
    public event Action<int> OnHealthChanged;

    public int attack;
    public int defense;
    public int speed;
    public List<Skill> skills = new List<Skill>();
    public Sprite characterIconSprite;
    public GameObject characterPrefab;

    public int index;
    public bool isEnemy = false;

    public void Initialize()
    {
        if (characterPrefab == null)
        {
            Debug.LogError($"Character prefab is missing for {name}");
            return;
        }

        characterPrefab.SetActive(true);
    }

    public bool IsAlive() => health > 0;

    public void Attack(Character target, Skill skill)
    {
        if (skill.damage > 0) // Attack skill
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
        else // Healing skill
        {
            if (UnityEngine.Random.Range(0, 100) <= skill.hitChance)
            {
                int healAmount = Mathf.Abs(skill.damage);
                int modifier = UnityEngine.Random.Range(-skill.damageModifier, skill.damageModifier + 1);
                healAmount += healAmount * modifier / 100;

                target.health = Mathf.Min(target.health + healAmount, target.maxHealth);
                Debug.Log($"{name} used {skill.name} on {target.name}, healing {healAmount} HP!");
            }
            else
            {
                Debug.Log($"{name} failed to heal using {skill.name}!");
            }
        }
    }


    public void TakeDamage(int damage)
    {
        damage = Mathf.Max(damage - defense, 0);
        health -= damage;
        OnHealthChanged?.Invoke(health);

        Debug.Log($"{name} took {damage} damage! Remaining HP: {health}");

        if (!IsAlive())
        {
            Debug.Log($"{name} has been defeated!");
        }
    }

    public Character Clone()
    {
        return (Character)this.MemberwiseClone();
    }

}


[Serializable]
public class Enemy : Character
{
    public Enemy()
    {
        isEnemy = true;
    }
}
