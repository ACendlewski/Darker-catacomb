using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.TextCore.Text;

public class CharacterLoader : MonoBehaviour
{
    public List<Character> characters;

    void Start()
    {
        LoadCharacters();
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

    void DisplayCharacters()
    {
        Debug.Log($"Found characters:");
        foreach (Character character in characters)
        {

            Debug.Log($"Name: {character.name}");
        }
        Debug.Log("END");
    }



}


[System.Serializable]
public class CharacterList
{
    public List<Character> characters;
}

