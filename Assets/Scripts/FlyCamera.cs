using System.Collections.Generic;
using Manager;
using Parent;
using UnityEngine;

public class FlyCamera : MonoBehaviour {

    public static FlyCamera Instance { get; private set; }

    private float yVelocity = 0.0f;
    [SerializeField]
    private Camera mainCamera;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float height;
    [SerializeField]
    private float scrollBorder;
    [SerializeField]
    private Vector2 scrollLimitEnd;
    [SerializeField]
    private Vector2 scrollLimitBegin;
    [SerializeField]
    private float minFov;
    [SerializeField]
    private float maxFov;
    [SerializeField]
    private float sensitivity = 10f;
    private ParentObject selection;

    public void JumpToTarget(Vector3 pos) {
        transform.position = new Vector3(pos.x, transform.position.y, pos.z + 50);
    }

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        EventManager.Instance.OnParentObjectSelectedEvent += SetObject;
        EventManager.Instance.OnDeselectEvent += Deselect;
    }

    private void SetObject(List<ParentObject> selection) {
        this.selection = selection[0];
    }

    private void Deselect() {
        selection = null;
    }

    private void SetCameraY(double smoothTime) {
        float newPosition = Mathf.SmoothDamp(transform.position.y, Terrain.activeTerrain.SampleHeight(transform.position) + height, ref yVelocity, (float)smoothTime);
        transform.position = new Vector3(transform.position.x, newPosition, transform.position.z);
    }

    private void HandleZooming() {
        float fov = Camera.main.fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;
    }

    private void Update () {
        if (Input.GetKey(KeyCode.LeftShift)) {
            return;
        }
        HandleZooming();

        double smoothTime = 0.1 / (mainCamera.velocity.magnitude / 100);
        float finalSpeed = speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - scrollBorder) {
            transform.Translate(new Vector3(finalSpeed, 0, 0));
            SetCameraY(smoothTime);
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= scrollBorder) {
            transform.Translate(new Vector3(-finalSpeed, 0, 0));
            SetCameraY(smoothTime);
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= scrollBorder) {
            transform.Translate(new Vector3(0, 0, -finalSpeed));
            SetCameraY(smoothTime);
        }
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - scrollBorder) {
            transform.Translate(new Vector3(0, 0, finalSpeed));
            SetCameraY(smoothTime);
        }

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, scrollLimitBegin.x, scrollLimitEnd.x);
        pos.z = Mathf.Clamp(pos.z, scrollLimitBegin.y, scrollLimitEnd.y);

        transform.position = pos;

        if (Input.GetKeyDown(KeyCode.Space)) {
            JumpToTarget(Manager.Manager.Instance.EnemyTarget.transform.position);
        }

        if (Input.GetKeyDown(KeyCode.F) && selection != null) {
            JumpToTarget(selection.gameObject.transform.position);
        }

    }

}
