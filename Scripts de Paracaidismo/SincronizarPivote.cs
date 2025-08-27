using UnityEngine;

public class SincronizarPivote : MonoBehaviour
{
    [Header("Referencias")]
    public Transform pivoteAvatar; // El Transform del pivote del avatar
    public Transform puntoAgarre; // El Transform del punto de anclaje del agarre

    [Header("Configuración")]
    public Vector3 desplazamientoLocal; // Desplazamiento relativo del agarre respecto al pivote del avatar

    void LateUpdate()
    {
        if (pivoteAvatar != null && puntoAgarre != null)
        {
            // Actualizar posición del punto de anclaje
            puntoAgarre.position = pivoteAvatar.TransformPoint(desplazamientoLocal);

            // Actualizar rotación del punto de anclaje
            puntoAgarre.rotation = pivoteAvatar.rotation;
        }
    }
}
