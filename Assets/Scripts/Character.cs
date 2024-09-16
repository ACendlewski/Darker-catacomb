using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Character
{
    public string name;
    public int health;
    public int attack;
    public int defense;
    public int speed;

    // Funkcja przyjmowania obra�e�
    public void TakeDamage(int damage)
    {
        int damageTaken = Math.Max(damage - defense, 0); // Obrona zmniejsza obra�enia
        health -= damageTaken;
        Debug.Log(name + " received " + damageTaken + " damage!");

        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    // Funkcja ataku na inny cel
    public void Attack(Character target)
    {
        Debug.Log(name + " attacks " + target.name);
        target.TakeDamage(attack);
    }

    // Funkcja �mierci
    void Die()
    {
        Debug.Log(name + " has died.");
        // Mo�esz tu doda� dodatkowe efekty (np. usuni�cie z listy aktywnych postaci)
    }

    public bool IsAlive()
    {
        return health > 0;
    }
}
