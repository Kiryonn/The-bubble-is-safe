using UnityEngine;

public class OrbitalCamera : MonoBehaviour
{
    private Transform xFormCamera;
    private Transform xFormParent;
    private Vector3 localRotation;

    public float distance = 200f;
    public bool disabled = false;

    [Header("Distance")]
    public float maxDistance = 200f;
    public float minDistance = 1.5f;
    [Header("Sensivity")]
    public float mouseSensivity = 4f;
    public float scrollSensivity = 2f;
    [Header("Dampening")]
    public float orbitDampening = 10f;
    public float scrollDampening = 6f;

    // Start is called before the first frame update
    void Start()
    {
        xFormCamera = transform;
        xFormParent = transform.parent;
    }

    // Late update is called once per frame, after Update() on every game object in the scene
    void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            disabled = !disabled;
        }

        if(!disabled && Input.GetMouseButton(1))
        {
            Cursor.visible = false;

            // Rotation of the camera based om mouse coordinates
            if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
            {
                localRotation.x += Input.GetAxis("Mouse X") * mouseSensivity;
                localRotation.y -= Input.GetAxis("Mouse Y") * mouseSensivity;
            }

            // Clamp y rotation to horizon and not flipping over at the top
            localRotation.y = Mathf.Clamp(localRotation.y, -80f, 80f);

        }

        Cursor.visible |= Input.GetMouseButtonUp(1);

        // Zooming input from our mouse scroll wheel
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float scrollAmount = Input.GetAxis("Mouse ScrollWheel") * scrollSensivity;
            
            // Makes camera zoom faster the further away it is from the target
            scrollAmount *= (distance * 0.3f);

            distance += scrollAmount * -1f;

            // Makes camera go no closer than 1.5 meters from target, and no further than 200 meters
            distance = Mathf.Clamp(distance, 1.5f, 200f);

        }

        // Actual camera rig transformation
        var _qt = Quaternion.Euler(localRotation.y, localRotation.x, 0);
        xFormParent.rotation = Quaternion.Lerp(xFormParent.rotation, _qt, Time.deltaTime * orbitDampening);

        if(xFormCamera.localPosition.z != distance * -1f)
        {
            xFormCamera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(xFormCamera.localPosition.z, distance * -1f, Time.deltaTime * scrollDampening));
        }

    }
}
