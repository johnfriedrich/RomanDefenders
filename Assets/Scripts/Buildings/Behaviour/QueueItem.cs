using UnityEngine;
using UnityEngine.UI;

public class QueueItem : Menu {

    public ParentObjectNameEnum Type;

    [SerializeField]
    private Image icon;
    [SerializeField]
    private Text countText;
    private int count = 0;

	public void Add() {
        icon.sprite = PrefabHolder.Instance.GetInfo(Type).GetComponent<Entity>().UiSprite;
        Show();
        count++;
        countText.text = count.ToString();
    }

    public void Remove() {
        count--;
        if (count <= 0) {
            Hide();
            count = 0;
        }
        countText.text = count.ToString();
    }

}
