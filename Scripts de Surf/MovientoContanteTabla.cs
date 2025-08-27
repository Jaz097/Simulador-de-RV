using UnityEngine;

public class BoyaMovimientoAleatorio : MonoBehaviour
{
    public float rangoMovimiento = 1.0f; // Rango máximo de movimiento en metros
    public float velocidadMin = 1.0f;    // Velocidad mínima del movimiento
    public float velocidadMax = 3.0f;    // Velocidad máxima del movimiento

    private Vector3 posicionInicial;     // Guarda la posición inicial de la boya
    private float velocidad;             // Velocidad actual de esta boya
    private float desfase;               // Desfase aleatorio para variar el movimiento entre boyas
    private int direccion;               // Dirección del movimiento (1 para derecha, -1 para izquierda)

    void Start()
    {
        // Guarda la posición inicial de la boya
        posicionInicial = transform.position;

        // Asigna una velocidad aleatoria dentro del rango
        velocidad = Random.Range(velocidadMin, velocidadMax);

        // Asigna un desfase aleatorio para el movimiento
        desfase = Random.Range(0, 2 * Mathf.PI);

        // Define una dirección aleatoria de movimiento (1 o -1)
        direccion = Random.Range(0, 2) == 0 ? 1 : -1;
    }

    void Update()
    {
        // Calcula el desplazamiento lateral con la función seno y el desfase
        float desplazamiento = Mathf.Sin(Time.time * velocidad + desfase) * rangoMovimiento * direccion;

        // Aplica el desplazamiento lateral a la posición inicial
        transform.position = posicionInicial + new Vector3(desplazamiento, 0, 0);
    }
}
