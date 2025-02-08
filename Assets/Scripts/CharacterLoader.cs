using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.TextCore.Text;

public class CharacterLoader : MonoBehaviour
{
    public List<Character> characters;
    public List<Enemy> enemies;


    void Start()
    {
        LoadCharacters();
        LoadEnemies();
        foreach (Character character in characters)
        {
            character.maxHealth = character.health;
        }
        //DisplayCharacters();
        foreach (Character character in characters)
        {
            character.index = characters.IndexOf(character); // Ustawienie wartości pola index
        }
    }

    void LoadCharacters()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "characters.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            characters = JsonUtility.FromJson<CharacterList>(json).characters;
        }
        else
        {
            Debug.LogError("Cannot find characters.json file");
        }

        foreach (Character character in characters)
        {
            foreach (Skill skill in character.skills)
            {
                skill.skillIcon = Resources.Load<Sprite>("Skills/" + skill.name);
            }
        }
    }

    void LoadEnemies()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "enemies.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            enemies = JsonUtility.FromJson<EnemyList>(json).enemies;
        }
        else
        {
            Debug.LogError("Cannot find enemies.json file");
        }

        foreach (Enemy enemy in enemies)
        {
            foreach (Skill skill in enemy.skills)
            {
                skill.skillIcon = Resources.Load<Sprite>("Skills/" + skill.name);
            }
        }
    }

    void DisplayCharacters()
    {
        Debug.Log($"Found characters:");
        foreach (Character character in characters)
        {

            Debug.Log($"Name: {character.name}");
        }
        Debug.Log("END");
    }

    void DisplayEnemies()
    {
        Debug.Log($"Found enemies:");
        foreach (Enemy enemy in enemies)
        {
            Debug.Log($"Name: {enemy.name}");
        }
        Debug.Log("END");
    }


}


[System.Serializable]
public class CharacterList
{
    public List<Character> characters;
}

[System.Serializable]
public class EnemyList
{
    public List<Enemy> enemies;
}
