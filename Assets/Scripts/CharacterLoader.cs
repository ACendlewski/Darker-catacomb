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
        foreach (Character character in characters)
        {
            character.maxHealth = character.health;
            character.Initialize();
        }
        LoadEnemies();

        foreach (Enemy enemy in enemies)
        {
            enemy.maxHealth = enemy.health;
            enemy.Initialize();
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
            // Load character prefab from Resources/Characters folder
            character.characterPrefab = Resources.Load<GameObject>($"Prefabs/{character.name}");
            if (character.characterPrefab == null)
            {
                Debug.LogError($"Failed to load prefab for character: {character.name}");
            }

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
            // Load enemy prefab from Resources/Prefabs/Enemies folder
            enemy.characterPrefab = Resources.Load<GameObject>($"Prefabs/Enemies/{enemy.name}");
            if (enemy.characterPrefab == null)
            {
                Debug.LogError($"Failed to load prefab for enemy: {enemy.name}");
            }

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
        Debug.Log($"CharacterLoader has {characters.Count} characters and {enemies.Count} enemies.");

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
