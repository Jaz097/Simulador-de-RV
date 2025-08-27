using UnityEngine;

public class RotacionEnXSinMovimiento : MonoBehaviour
{
    // Velocidad de rotación en grados por segundo
    public float velocidadRotacion = 100000.0f;

    void Update()
    {
        // Obtener el componente Transform del objeto actual
        Transform objetoTransform = transform;

        // Calcular el ángulo de rotación en el eje Z basado en la velocidad y el tiempo
        float anguloRotacionX = velocidadRotacion * Time.deltaTime;

        // Rotar el objeto alrededor del eje Z local
        objetoTransform.Rotate(anguloRotacionX, 0.0f, 0.0f);
    }
}
