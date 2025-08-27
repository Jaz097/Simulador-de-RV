using UnityEngine;
using UnityEngine.XR;


public class ControlTablaOculus : MonoBehaviour
{
    public float velocidadMovimiento = 3.0f; // Velocidad de movimiento hacia adelante
    public float fuerzaRotacion = 1.0f; // Fuerza de rotación en el eje Y
    public Rigidbody rb; // Asigna el Rigidbody desde el Inspector

    private bool buttonPressed = false;       // Estado del botón

    void Start()
    {
        // Verificar que el Rigidbody esté asignado
        if (rb == null)
        {
            Debug.LogError("Rigidbody no asignado. Asegúrate de arrastrar el Rigidbody al Inspector.");
        }


    }

    void Update()
    {
        if (rb != null) // Verificar si el Rigidbody está asignado
        {
            MoverYRotar();
        }
    }

    void MoverYRotar()
    {
        // Obtener la entrada de los joysticks
        float verticalInput = Input.GetAxis("Vertical"); // Joystick hacia adelante y atrás
                                                         
        if (CheckButtonPress(XRNode.RightHand, CommonUsages.triggerButton)) 
        {
            // Movimiento hacia la derecha
            Vector3 movimientoDerecha = transform.right * velocidadMovimiento * Time.deltaTime;
            rb.MovePosition(rb.position + movimientoDerecha);

            // Rotación hacia la derecha
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, fuerzaRotacion, 0));
        }

        // Obtener el estado del gatillo izquierdo
        if (CheckButtonPress(XRNode.LeftHand, CommonUsages.triggerButton))
        {
            // Movimiento hacia la izquierda
            Vector3 movimientoIzquierda = -transform.right * velocidadMovimiento * Time.deltaTime;
            rb.MovePosition(rb.position + movimientoIzquierda);

            // Rotación hacia la izquierda
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, -fuerzaRotacion, 0));
        }
    }

    // Verificar la presión de un botón
    bool CheckButtonPress(XRNode node, InputFeatureUsage<bool> button)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(node);
        bool isPressed;

        if (device.TryGetFeatureValue(button, out isPressed) && isPressed)
        {
            if (!buttonPressed)
            {
                buttonPressed = true;
                return true;  // El botón fue presionado
            }
        }
        else
        {
            buttonPressed = false;  // El botón ya no está siendo presionado
        }

        return false;  // No hay acción de botón
    }
}