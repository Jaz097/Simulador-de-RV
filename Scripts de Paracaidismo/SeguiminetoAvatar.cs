using UnityEngine;

public class SeguirAvatarConFisicas : MonoBehaviour
{
    public Transform avatarTransform;
    public float followForce = 10f; // Fuerza de seguimiento
    public float maxSpeed = 5f; // Velocidad máxima

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Asegúrate de que el objeto tenga un Rigidbody
    }

    void FixedUpdate()
    {
        // Calcular la dirección hacia el avatar
        Vector3 direction = (avatarTransform.position - transform.position).normalized;

        // Aplicar fuerza hacia el avatar
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(direction * followForce);
        }
    }
}
