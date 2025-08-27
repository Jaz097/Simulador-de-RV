using UnityEngine;

public class RotacionEnZSinMovimiento : MonoBehaviour
{
    // Velocidad de rotación en grados por segundo
    public float velocidadRotacion = 100000.0f;

    void Update()
    {
        // Obtener el componente Transform del objeto actual
        Transform objetoTransform = transform;

        // Calcular el ángulo de rotación en el eje Z basado en la velocidad y el tiempo
        float anguloRotacionZ = velocidadRotacion * Time.deltaTime;

        // Rotar el objeto alrededor del eje Z local
        objetoTransform.Rotate(0.0f, 0.0f, anguloRotacionZ);
    }
}

