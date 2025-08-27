using UnityEngine;

public class FlotacionHelicoptero : MonoBehaviour
{
    public float amplitud = 0.5f; // Amplitud del movimiento
    public float frecuencia = 1.0f; // Frecuencia del movimiento

    private Vector3 posicionInicial;

    void Start()
    {
        posicionInicial = transform.position; // Guardar la posición inicial del objeto
    }

    void Update()
    {
        // Crear el movimiento de flotación
        float nuevaY = posicionInicial.y + Mathf.Sin(Time.time * frecuencia) * amplitud;
        transform.position = new Vector3(transform.position.x, nuevaY, transform.position.z);
    }
}
