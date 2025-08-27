using UnityEngine;
using UnityEngine.SceneManagement;

public class LimiteZona : MonoBehaviour
{
    public GameObject boatAlingNormal;
    public int contadorReinicios = 0;

    private void Start()
    {
         contadorReinicios = PlayerPrefs.GetInt("ContadorReinicios", 0);

        if (boatAlingNormal == null)
        {
            Debug.LogWarning("La referencia 'boatAlingNormal' no está asignada en el Inspector.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Trigger Exit: " + other.gameObject.name);
        
        if (other.gameObject == boatAlingNormal)
        {
            Debug.Log("BoatAlingNormal ha salido de la franja límite. Reiniciando escena...");
            ReiniciarEscena();
        }
    }

    private void ReiniciarEscena()
    {

        contadorReinicios++;
        PlayerPrefs.SetInt("ContadorReinicios", contadorReinicios);
        PlayerPrefs.Save(); // Asegurarse de guardar los cambios inmediatamente

        Debug.Log("Reiniciando escena por " + contadorReinicios + "ª vez.");

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

     void OnApplicationQuit()
    {
        //Al salir de la aplicación, reiniciar el contador de reinicios
        PlayerPrefs.DeleteKey("ContadorReinicios");
        Debug.Log("Contador de reinicios reseteado al cerrar la aplicación.");
    }
}
