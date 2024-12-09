using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[Serializable]
public class Skill
{
    public string name; // Nazwa umiejêtnoœci
    public int damage; // Podstawowe obra¿enia
    public int hitChance; // Szansa na trafienie (0-100)
    public int critChance; // Szansa na trafienie krytyczne (0-100)
    public int damageModifier; // Modyfikator obra¿eñ w procentach (np. -20, 10)
}

[Serializable]
public class Character
{
    public string name; // Imiê postaci
    public int health; // Zdrowie postaci
    public int attack; // Bazowy atak
    public int defense; // Obrona
    public int speed; // Szybkoœæ
    public List<Skill> skills = new List<Skill>(); // Lista umiejêtnoœci
    public GameObject characterPrefab; // Prefab postaci (awatar)

    public Animator characterAnimator; // Animator postaci (jeœli u¿ywasz animacji)

    // Metoda do sprawdzenia, czy postaæ ¿yje
    public bool IsAlive()
    {
        return health > 0;
    }

    // Metoda do ataku (z u¿yciem konkretnej umiejêtnoœci)
    public void Attack(Character target, Skill skill)
    {
        if (UnityEngine.Random.Range(0, 100) <= skill.hitChance)
        {
            bool isCrit = UnityEngine.Random.Range(0, 100) <= skill.critChance;
            int damage = skill.damage;

            // Dodanie modyfikatora obra¿eñ
            int modifier = UnityEngine.Random.Range(-skill.damageModifier, skill.damageModifier + 1);
            damage += damage * modifier / 100;

            // Obliczenie obra¿eñ krytycznych
            if (isCrit)
            {
                damage = (int)(damage * 1.5f); // Krytyczne obra¿enia: 1.5x podstawowych
                Debug.Log($"{name} dealt a critical hit!");
            }

            // Zmniejszenie obra¿eñ przez obronê celu
            damage -= target.defense;
            damage = Mathf.Max(damage, 0); // Upewnij siê, ¿e obra¿enia nie s¹ ujemne

            // Odejmuje ¿ycie przeciwnika
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
}
