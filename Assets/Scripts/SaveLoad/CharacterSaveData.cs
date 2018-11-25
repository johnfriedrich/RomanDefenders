using System;
using UnityEngine;

[Serializable]
public class CharacterSaveData {

    [SerializeField]
    private string characterName = string.Empty;
    [SerializeField]
    private SerializableColor hairColor = new SerializableColor();
    [SerializeField]
    private SerializableColor eyeColor = new SerializableColor();
    [SerializeField]
    private SerializableColor skinColor = new SerializableColor();
    [SerializeField]
    private SerializableColor teamColor = new SerializableColor();

    public CharacterSaveData() { }

    public CharacterSaveData(string characterName, Color hairColor, Color eyeColor, Color skinColor, Color teamColor) {
        this.characterName = characterName;
        HairColor = hairColor;
        EyeColor = eyeColor;
        SkinColor = skinColor;
        TeamColor = teamColor;
    }

    public string CharacterName {
        get {
            return characterName;
        }

        set {
            characterName = value;
        }
    }

    public Color TeamColor {
        get {
            return teamColor.GetColor();
        }

        set {
            teamColor = new SerializableColor(value);
        }
    }

    public Color EyeColor {
        get {
            return eyeColor.GetColor();
        }

        set {
            eyeColor = new SerializableColor(value);
        }
    }

    public Color SkinColor {
        get {
            return skinColor.GetColor();
        }

        set {
            skinColor = new SerializableColor(value);
        }
    }

    public Color HairColor {
        get {
            return hairColor.GetColor();
        }

        set {
            hairColor = new SerializableColor(value);
        }
    }

}
