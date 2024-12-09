using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[Serializable]
public class Skill
{
    public string name; // Nazwa umiej�tno�ci
    public int damage; // Podstawowe obra�enia
    public int hitChance; // Szansa na trafienie (0-100)
    public int critChance; // Szansa na trafienie krytyczne (0-100)
    public int damageModifier; // Modyfikator obra�e� w procentach (np. -20, 10)
}

[Serializable]
public class Character
{
    public string name; // Imi� postaci
    public int health; // Zdrowie postaci
    public int attack; // Bazowy atak
    public int defense; // Obrona
    public int speed; // Szybko��
    public List<Skill> skills = new List<Skill>(); // Lista umiej�tno�ci
    public GameObject characterPrefab; // Prefab postaci (awatar)

    public Animator characterAnimator; // Animator postaci (je�li u�ywasz animacji)

    // Metoda do sprawdzenia, czy posta� �yje
    public bool IsAlive()
    {
        return health > 0;
    }

    // Metoda do ataku (z u�yciem konkretnej umiej�tno�ci)
    public void Attack(Character target, Skill skill)
    {
        if (UnityEngine.Random.Range(0, 100) <= skill.hitChance)
        {
            bool isCrit = UnityEngine.Random.Range(0, 100) <= skill.critChance;
            int damage = skill.damage;

            // Dodanie modyfikatora obra�e�
            int modifier = UnityEngine.Random.Range(-skill.damageModifier, skill.damageModifier + 1);
            damage += damage * modifier / 100;

            // Obliczenie obra�e� krytycznych
            if (isCrit)
            {
                damage = (int)(damage * 1.5f); // Krytyczne obra�enia: 1.5x podstawowych
                Debug.Log($"{name} dealt a critical hit!");
            }

            // Zmniejszenie obra�e� przez obron� celu
            damage -= target.defense;
            damage = Mathf.Max(damage, 0); // Upewnij si�, �e obra�enia nie s� ujemne

            // Odejmuje �ycie przeciwnika
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
