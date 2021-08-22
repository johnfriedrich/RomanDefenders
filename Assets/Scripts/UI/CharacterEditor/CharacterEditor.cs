using Entities;
using Manager;
using ObjectManagement;
using Parent;
using SaveLoad;
using Sound;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.CharacterEditor {
    public class CharacterEditor : Menu {

        public static CharacterEditor Instance { get; private set; }

        [SerializeField]
        private GameObject devEditorGameObject;
        private GameObject currentEditableObject;
        [SerializeField]
        private InputField characterName;
        [SerializeField]
        private GameObject modelHolder;
        [SerializeField]
        private ColorPicker hairPicker;
        [SerializeField]
        private ColorPicker skinPicker;
        [SerializeField]
        private ColorPicker eyePicker;
        [SerializeField]
        private ColorPicker teamPicker;
        [SerializeField]
        private CharacterRotate characterRotate;
        private CharacterSaveData characterSaveData = new CharacterSaveData();

        public CharacterSaveData CharacterSaveData => characterSaveData;

        public void SetObject(string path) {
            SetEditableObject(SavaData.Instance.GetDataStringFromPath(path));
        }

        public override void Show() {
            base.Show();
            devEditorGameObject.SetActive(true);
        }

        public override void Hide() {
            base.Hide();
            devEditorGameObject.SetActive(false);
        }

        public void BackToMainMenu() {
            Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
            Hide();
            Manager.Manager.Instance.MainMenu.Show();
        }

        public void Save() {
            ApplyCharacterDataData();
            if (!CheckName()) {
                return;
            }
            Manager.Manager.Instance.TeamColour = characterSaveData.TeamColor;
            FileDialog.Instance.ShowSaveDialog(SavaData.Instance.CharacterSavePathExtension, characterSaveData);
        }

        public void StartGame() {
            Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
            if (!CheckName()) {
                return;
            }
            Manager.Manager.Instance.TeamColour = currentEditableObject.GetComponent<Character>().TeamColor;
            Hide();
            Manager.Manager.Instance.BeforeGameScreen.SetActive(false);
            EventManager.Instance.StartGame();
        }

        private void SetEditableObject(string data) {
            if (currentEditableObject == null) {
                currentEditableObject = PoolHolder.Instance.GetObject(ParentObjectNameEnum.Character);
            }
            Character tempCharacter = currentEditableObject.GetComponent<Character>();
            if (data != null) {
                if (!EntityUtils.ValidateData(JsonUtility.FromJson<CharacterSaveData>(data))) {
                    return;
                }
            }
            tempCharacter.Agent.enabled = false;
            currentEditableObject.transform.position = modelHolder.transform.position;
            currentEditableObject.SetActive(true);
            characterRotate.Hero = currentEditableObject;
            if (data != null) {
                tempCharacter.Load(JsonUtility.FromJson<CharacterSaveData>(data));
            }
            LoadValuesFromObject();
        }

        private bool CheckName() {
            if (characterName.text.Length == 0) {
                ActionText.Instance.SetActionText("Your brave Hero needs a name!", 2);
                return false;
            }
            return true;
        }

        private void Awake() {
            Instance = this;
        }

        private void Start() {
            SetupUIElememts();
            SetEditableObject(null);
            EventManager.Instance.OnGameResetPostEvent += ResetObject;
        }

        private void ResetObject() {
            SetEditableObject(null);
        }

        private void SetupUIElememts() {
            characterName.onEndEdit.AddListener(delegate { SaveCharacterNameValue(characterName); });
            characterName.contentType = InputField.ContentType.Name;
            hairPicker.onValueChanged.AddListener(delegate { SetHairColor(); });
            eyePicker.onValueChanged.AddListener(delegate { SetEyeColor(); });
            skinPicker.onValueChanged.AddListener(delegate { SetSkinColor(); });
            teamPicker.onValueChanged.AddListener(delegate { SetTeamColor(); });
        }

        private void ApplyCharacterDataData() {
            currentEditableObject.GetComponent<Character>().Load(characterSaveData);
        }

        public void Load() {
            Sound.Sound.Instance.PlaySoundClip(SoundEnum.UI_Button_Click);
            FileDialog.Instance.ShowLoadDialog(SavaData.Instance.CharacterSavePathExtension);
        }

        private void SetHairColor() {
            characterSaveData.HairColor = hairPicker.CurrentColor;
            ApplyCharacterDataData();
        }

        private void SetEyeColor() {
            characterSaveData.EyeColor = eyePicker.CurrentColor;
            ApplyCharacterDataData();
        }

        private void SetSkinColor() {
            characterSaveData.SkinColor = skinPicker.CurrentColor;
            ApplyCharacterDataData();
        }

        private void SetTeamColor() {
            characterSaveData.TeamColor = teamPicker.CurrentColor;
            ApplyCharacterDataData();
        }

        private void SaveCharacterNameValue(InputField field) {
            characterSaveData.CharacterName = field.text;
            ApplyCharacterDataData();
        }

        private void LoadValuesFromObject() {
            var characterSaveData = currentEditableObject.GetComponent<Character>().Save();
            this.characterSaveData = characterSaveData;
            characterName.text = this.characterSaveData.CharacterName;
            hairPicker.CurrentColor = this.characterSaveData.HairColor;
            eyePicker.CurrentColor = this.characterSaveData.EyeColor;
            skinPicker.CurrentColor = this.characterSaveData.SkinColor;
            teamPicker.CurrentColor = this.characterSaveData.TeamColor;
            Manager.Manager.Instance.TeamColour = this.characterSaveData.TeamColor;
        }

    }
}
