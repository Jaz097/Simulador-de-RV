using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class EstadisticasTablaSurf : MonoBehaviour
{
    public Rigidbody rigidbodyTabla; // Asigna aquí el Rigidbody de la tabla
    public TextMeshProUGUI textoInfo; // Asigna el Text único para la velocidad y la distancia

    private Vector3 posicionInicial;

    void Start()
    {
        // Guardar la posición inicial de la tabla
        posicionInicial = rigidbodyTabla.transform.position;
    }

    void Update()
    {
        // Calcular la velocidad en km/h
        float velocidad = rigidbodyTabla.velocity.magnitude * 1.6f; // Convertir de m/s a km/h

        // Calcular la distancia recorrida desde la posición inicial
        float distancia = Vector3.Distance(posicionInicial, rigidbodyTabla.transform.position);

        // Actualizar el texto con ambos valores
        textoInfo.text = "Velocidad: " + velocidad.ToString("F2") + " km/h -- " +
                         "Distancia: " + distancia.ToString("F2") + " metros";
    }
}