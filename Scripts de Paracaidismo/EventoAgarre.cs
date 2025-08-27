using UnityEngine;
using UltimateXR.Manipulation;



public class EventoAgarre : MonoBehaviour
{
    [Tooltip("Asignar el objeto de manipulación para observar los eventos de liberación.")]
    public GameObject objetoAObservar; // Objeto al que se le asigna el componente de manipulación

    private Vector3 posicionOriginal; // Guarda la posición original del objeto

    private Quaternion rotacionOriginal;

    public bool esAbierto = false; 


    // Suscribirse al evento cuando se inicializa
    private void Start()
    {
        // Guarda la posición original del objeto
        posicionOriginal = objetoAObservar.transform.position;
        rotacionOriginal = objetoAObservar.transform.rotation;

        // Verifica si el objeto a observar tiene el componente adecuado
        if (objetoAObservar != null)
        {
            var manipulable = objetoAObservar.GetComponent<UxrGrabbableObject>(); // Obtén el componente adecuado
            if (manipulable != null)
            {
                manipulable.Released += OnObjectReleased; // Suscríbete al evento de liberación
            }
        }
    }

    // Anular la suscripción al evento cuando se desactiva
    private void OnDestroy()
    {
        // Verifica si el objeto a observar tiene el componente adecuado
        if (objetoAObservar != null)
        {
            var manipulable = objetoAObservar.GetComponent<UxrGrabbableObject>(); // Obtén el componente adecuado
            if (manipulable != null)
            {
                manipulable.Released -= OnObjectReleased; // Anula la suscripción al evento de liberación
            }
        }
    }

    // Este método se llama cuando un objeto es soltado
    private void OnObjectReleased(object sender, UxrManipulationEventArgs e) // Ajusta el tipo de evento aquí
    {
        // Verifica si el objeto que se soltó es el objeto que estamos observando
        if (objetoAObservar != null && e.GrabbableObject == objetoAObservar.GetComponent<UxrGrabbableObject>())
        {
            // Mueve el objeto de vuelta a su posición original
            transform.position = posicionOriginal;
            transform.rotation = rotacionOriginal;

            // Mostrar en la consola que el objeto ha sido soltado y movido
            Debug.Log($"El objeto {e.GrabbableObject.name} ha sido soltado y movido a la posición original.");

            esAbierto = true; 
                 
        }
    }
}
