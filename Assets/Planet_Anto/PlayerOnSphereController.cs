using UnityEngine;

public class PlayerOnSphereController : MonoBehaviour
{
    public Transform sphere; // Référence à la sphère
    public float gravityStrength = 10f; // Force de gravité vers la sphère
    public float moveSpeed = 5f; // Vitesse de déplacement
    public float rotationSpeed = 10f; // Vitesse de rotation

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // Désactiver la gravité par défaut
        rb.constraints = RigidbodyConstraints.FreezeRotation; // Éviter que le Rigidbody tourne par lui-même
    }

    void FixedUpdate()
    {
        // Calcul de la direction de la gravité vers le centre de la sphère
        Vector3 gravityDirection = (sphere.position - transform.position).normalized;

        // Appliquer une force vers le centre de la sphère
        rb.AddForce(gravityDirection * gravityStrength, ForceMode.Acceleration);

        // Orienter le joueur pour qu'il reste perpendiculaire à la surface de la sphère
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, -gravityDirection) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Déplacement sur la sphère
        MoveOnSphere(gravityDirection);
    }

    private void MoveOnSphere(Vector3 gravityDirection)
    {
        // Entrées de déplacement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calcul de la direction du déplacement
        Vector3 moveDirection = Vector3.Cross(transform.right, gravityDirection) * vertical +
                                Vector3.Cross(gravityDirection, transform.forward) * horizontal;

        // Appliquer le mouvement
        rb.MovePosition(rb.position + moveDirection.normalized * moveSpeed * Time.deltaTime);
    }
}