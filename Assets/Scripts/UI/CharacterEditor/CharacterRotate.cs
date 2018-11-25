using UnityEngine;

public class CharacterRotate : MonoBehaviour {

    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private GameObject hero;
    private new bool enabled;
    private bool right;
    private bool left;

    public GameObject Hero {
        set {
            hero = value;
        }
    }

    public void Enable() {
        enabled = true;
    }

    public void Disable() {
        enabled = false;
    }

    public void SetRight() {
        right = true;
        left = false;
    }

    public void SetLeft() {
        right = false;
        left = true;
    }

    private void OnDisable() {
        if (hero != null) {
            hero.transform.rotation = Quaternion.identity;
        }
    }

    private void Update() {
        if (!enabled) {
            return;
        }
        if (Input.GetMouseButton(0) && left) {
            hero.transform.rotation *= Quaternion.AngleAxis(5, Vector3.up);
        }
        if (Input.GetMouseButton(0) && right) {
            hero.transform.rotation *= Quaternion.AngleAxis(5, Vector3.down);
        }
    }

}
