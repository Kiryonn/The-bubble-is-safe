using UnityEngine;

public class RotatingCamera : MonoBehaviour
{
    public float speed = 2f;

    private void Update()
    {
        transform.RotateAround(transform.parent.transform.position, Vector3.up, speed * Time.deltaTime);
    }

}
