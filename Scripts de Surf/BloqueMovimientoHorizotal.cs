using UnityEngine;
using UnityEngine.XR;

public class BloqueoMovimientoTabla : MonoBehaviour
{
    private bool bloqueoMovimiento = false; // Bandera para indicar si el movimiento está bloqueado
    private Rigidbody rb; // Referencia al componente Rigidbody de la tabla

    void Start()
    {
        // Obtén el componente Rigidbody de la tabla
        rb = GetComponent<Rigidbody>();

        // Suscribirse al evento de conexión de dispositivos para detectar el controlador de Oculus
        InputDevices.deviceConnected += InputDevices_DeviceConnected;
    }

    void OnDestroy()
    {
        // Desuscribirse del evento para evitar fugas de memoria
        InputDevices.deviceConnected -= InputDevices_DeviceConnected;
    }

    void Update()
    {
        // Si el movimiento está bloqueado, detén el movimiento en los ejes X y Z
        if (bloqueoMovimiento)
        {
            Vector3 velocidad = rb.velocity;
            velocidad.x = 0; // Detenemos el movimiento horizontal
            velocidad.z = 0; // Detenemos el movimiento en el eje Z
            rb.velocity = velocidad;
        }
    }

    private void InputDevices_DeviceConnected(InputDevice device)
    {
        // Verifica si el dispositivo conectado es el controlador izquierdo de Oculus
        if (device.characteristics.HasFlag(InputDeviceCharacteristics.Left) &&
            device.characteristics.HasFlag(InputDeviceCharacteristics.Controller))
        {
            Debug.Log("Controlador izquierdo de Oculus conectado.");
            BloquearMovimiento();
        }
    }

    // Función para bloquear el movimiento de la tabla
    public void BloquearMovimiento()
    {
        bloqueoMovimiento = true;
        Debug.Log("Movimiento bloqueado en los ejes X y Z.");
    }

    // Función para desbloquear el movimiento de la tabla
    public void DesbloquearMovimiento()
    {
        bloqueoMovimiento = false;
        Debug.Log("Movimiento desbloqueado en los ejes X y Z.");
    }
}
