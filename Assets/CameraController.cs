using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
    public static bool isMoving;
    
    public float moveSpeed = 15;
    public float zoomStrength = 3;
    public float minZoomSize = 1f;
    public float maxZoomSize = 6f;

    public float extraPercentage = 0.05f;

    private Camera cam;
    private Vector2 prevMousePos;

    private void Start() {
        cam = GetComponent<Camera>();
    }
    
    private void Update() {
        if (Input.GetAxis("Mouse ScrollWheel") != 0) {
            float dir = Input.GetAxis("Mouse ScrollWheel");
            cam.orthographicSize += zoomStrength * 100 * Time.deltaTime * dir * -1;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoomSize, maxZoomSize);
        }

        if (Input.GetMouseButton(0)) {
            Vector3 md = new(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            cam.transform.position += md * (Time.deltaTime * -1 * moveSpeed);

            if (!isMoving)
                if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0)
                    isMoving = true;
        }

        if (Input.GetMouseButtonUp(0))
            isMoving = false;
    }

    public void ResizeCameraToShowContent(float contentWidth) {
        float unitsPerPixel = contentWidth / Screen.width;
        float desiredSize = 0.5f * unitsPerPixel * Screen.height;
        cam.orthographicSize = desiredSize + desiredSize * extraPercentage;
    }
}