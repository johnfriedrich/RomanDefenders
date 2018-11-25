using SimpleFileBrowser;
using System.Collections;
using System.IO;
using UnityEngine;

public class FileDialog : MonoBehaviour {

    public static FileDialog Instance { get; private set; }

    private void Awake() {
        Instance = this;
    }

    void Start() {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Text Files", ".json"));
        FileBrowser.SetDefaultFilter(".json");
    }

    public void ShowLoadDialog(string pathExtension) {
        StartCoroutine(ShowLoadDialogCoroutine(pathExtension));
    }

    public void ShowSaveDialog(string pathExtension, EntitySaveData entitySaveData) {
        StartCoroutine(ShowSaveDialogCoroutine(pathExtension, entitySaveData));
    }

    public void ShowSaveDialog(string pathExtension, CharacterSaveData characterSaveData) {
        StartCoroutine(ShowSaveDialogCoroutine(pathExtension, characterSaveData));
    }

    IEnumerator ShowLoadDialogCoroutine(string pathExtension) {
        yield return FileBrowser.WaitForLoadDialog(false, Application.persistentDataPath + Path.AltDirectorySeparatorChar + pathExtension, "Load File", "Load");

        if (FileBrowser.Success) {
            CharacterEditor.Instance.SetObject(FileBrowser.Result);
        }
    }

    IEnumerator ShowSaveDialogCoroutine(string pathExtension, EntitySaveData entitySaveData) {
        yield return FileBrowser.WaitForSaveDialog(false, Application.persistentDataPath + Path.AltDirectorySeparatorChar + pathExtension, "Save File", "Save");

        if (FileBrowser.Success) {
            SavaData.Instance.Save(entitySaveData, FileBrowser.Result);
        }
    }

    IEnumerator ShowSaveDialogCoroutine(string pathExtension, CharacterSaveData characterSaveData) {
        yield return FileBrowser.WaitForSaveDialog(false, Application.persistentDataPath + Path.AltDirectorySeparatorChar + pathExtension, "Save File", "Save");

        if (FileBrowser.Success) {
            SavaData.Instance.Save(characterSaveData, FileBrowser.Result);
        }
    }
}
