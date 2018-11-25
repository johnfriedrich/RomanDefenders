using UnityEngine;

public class Billboard : MonoBehaviour {

    private new Camera camera;

    private void Start() {
        camera = Camera.main;
    }

    private void Update() {
        transform.forward = camera.transform.forward;
    }

}
