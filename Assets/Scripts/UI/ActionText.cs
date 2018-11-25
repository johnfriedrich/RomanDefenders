using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ActionText : MonoBehaviour {

    public static ActionText Instance { get; private set; }

    [SerializeField]
    private Text actionText;
    [SerializeField]
    private GameObject notificationObject;

    public void SetActionText(string text, float duration) {
        actionText.text = text;
        notificationObject.SetActive(true);
        StartCoroutine(ApplyText(duration));
    }

    private void Awake() {
        Instance = this;
        actionText.text = string.Empty;
    }

    private IEnumerator ApplyText(float duration) {
        yield return new WaitForSecondsRealtime(duration);
        notificationObject.SetActive(false);
        actionText.text = string.Empty;
    }

}