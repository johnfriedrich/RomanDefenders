﻿using System.IO;
using Entities;
using UnityEngine;

namespace SaveLoad {
    public class SavaData : MonoBehaviour {

        public static SavaData Instance { get; private set; }

        public readonly string CharacterSavePathExtension = "characters";

        public void Save(Character entity, string path) {
            var jsonString = JsonUtility.ToJson(entity.Save());
            File.WriteAllText(path, jsonString);
        }

        public void Save(EntitySaveData entitySaveData, string path) {
            var jsonString = JsonUtility.ToJson(entitySaveData);
            File.WriteAllText(path, jsonString);
        }

        public void Save(CharacterSaveData characterSaveData, string path) {
            var jsonString = JsonUtility.ToJson(characterSaveData);
            File.WriteAllText(path, jsonString);
        }

        public string GetDataStringFromPath(string path) {
            return File.ReadAllText(path);
        }

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            Directory.CreateDirectory(Application.persistentDataPath + Path.AltDirectorySeparatorChar + CharacterSavePathExtension);
        }

        private string BuildPath(string fileName) {
            return Application.persistentDataPath + Path.AltDirectorySeparatorChar + fileName + ".json";
        }

    }
}
