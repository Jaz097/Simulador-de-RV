using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Linq;



public class SurfFacil : MonoBehaviour
{
    public TextMeshProUGUI textoPerdido;
    public TextMeshProUGUI textoGanar;
    public TextMeshProUGUI velocidad_ditancia;
    public TextMeshProUGUI numero_vidas;
    public int limiteColiciones = 5;
    public float distanciaObjetivo = 250f;
    public Transform playerTransform;
    public GameObject PanelBotones;
    public Button botonMenuPrincipal;
    public Button botonMenuNiveles;
    public Button botonReiniciar;

    public TextMeshProUGUI TextoBotonPausa;



    private bool objetoVisible = true;

    public GameObject PanelPausa;

    private bool juegoPausado = false;


    public GameObject laserizquerdo;

    public GameObject laserderecho;

    public List<GameObject> instrucciones;

    public List<string> textosInstrucciones;

    private int indiceActual = 0;
    public float tiempoEntreInstrucciones = 200.0f;

    public Asistente asistenteDeVoz;

    private bool buttonPressed = false;
    private float tiempoUltimaPresion = 0f;
    private float delayPresion = 0.5f;

    private bool avanceAutomaticoHabilitado = true;

    public int contadorReinicios = 0;






    //Variables para efectos de sonidos 

    public AudioSource sonidoPerdiste;

    public AudioSource sonidoGanaste;

    public AudioSource sonidoBotones;

    public AudioSource golpeColicion;

    public AudioSource[] sonidos;

    public GameObject componetesPlayer;

    private int contador = 0;
    private int n_vidas;
    private Vector3 posicionInicial;

    public GameObject Objetoscomponentes;

    private bool estaenpausa = false;

    private bool intruccioneshabilidatadas = true;

    private void Start()
    {


        XRInputSubsystem xrSubsystem = GetXRInputSubsystem();
        if (xrSubsystem != null)
        {
            xrSubsystem.TryRecenter();
        }

        contadorReinicios = PlayerPrefs.GetInt("ContadorReinicios", 0);

        // Inicializar el número de vidas
        n_vidas = limiteColiciones;

        // Guardar la posición inicial del jugador
        posicionInicial = playerTransform.position;

        PanelPausa?.gameObject.SetActive(false);



        if (golpeColicion != null)
        {
            golpeColicion.Stop();
        }

        if (sonidoPerdiste != null)
        {
            sonidoPerdiste.Stop();
        }

        if (sonidoGanaste != null)
        {
            sonidoGanaste.Stop();
        }

        if (sonidoBotones != null)
        {
            sonidoBotones.Stop();
        }

        if (textoPerdido != null)
        {
            textoPerdido.gameObject.SetActive(false);
        }
        if (textoGanar != null)
        {
            textoGanar.gameObject.SetActive(false);
        }

        // Mostrar el número de vidas al inicio
        if (numero_vidas != null)
        {
            numero_vidas.gameObject.SetActive(true);
            numero_vidas.text = "Vidas: " + n_vidas;
        }

        if (componetesPlayer != null)
        {
            componetesPlayer.gameObject.SetActive(false);
        }

        if (PanelBotones != null)
        {
            PanelBotones.SetActive(false);
            if (botonReiniciar != null)
            {
                botonReiniciar.onClick.AddListener(() =>
                {
                    if (sonidoBotones != null)
                    {
                        sonidoBotones.Play();
                    }
                    ReiniciarSimulacion();
                });
            }

            if (botonMenuNiveles != null)
            {
                botonMenuNiveles.onClick.AddListener(() =>
                {
                    if (sonidoBotones != null)
                    {
                        sonidoBotones.Play();
                    }
                    IrNiveles();
                });
            }

            if (botonMenuPrincipal != null)
            {
                botonMenuPrincipal.onClick.AddListener(() =>
                {
                    if (sonidoBotones != null)
                    {
                        sonidoBotones.Play();
                    }
                    IrMenu();
                });
            }
        }



        if (contadorReinicios == 0)
        {
            MostrarInstruccion(indiceActual);
            TextoBotonPausa?.gameObject.SetActive(false);
            StartCoroutine(AvanzarAutomaticamente());
            BloquearMovimientoHorizotal();
            BloquearMovimientoVertical();

        }
        else if (contadorReinicios > 0)
        {
            DesactivarTodasInstrucciones();
            TextoBotonPausa?.gameObject.SetActive(true);

        }

        if (sonidos.Length == 0)
        {
            sonidos = FindObjectsOfType<AudioSource>();
        }






    }

    private void Update()
    {


        // Detectar avance manual con botón en Oculus
        if (PuedeAvanzarInstruccion())
        {
            AvanzarInstruccionManual();
        }
        else if (PuedeRetrocederInstruccion())
        {
            RetrocederInstruccion();
        }
        else if (PuedeOmitirInstrucciones())
        {
            OmitirTodasInstrucciones();
        }


        
        if (CheckButtonPress(XRNode.LeftHand, CommonUsages.secondaryButton) && intruccioneshabilidatadas == false)
        {
            PausarJuego();
            EsconderMensaje();
            numero_vidas?.gameObject.SetActive(false);
        }


        if (CheckButtonPress(XRNode.LeftHand, CommonUsages.primaryButton) && intruccioneshabilidatadas == false)
        {
            ReanudarJuego();
            velocidad_ditancia?.gameObject.SetActive(true);
            numero_vidas?.gameObject.SetActive(true);
        }

        // Comprobar la distancia recorrida
        float distanciaRecorrida = Vector3.Distance(posicionInicial, playerTransform.position);
        if (distanciaRecorrida >= distanciaObjetivo)
        {
            MostrarMensajeGanar();
            MostrarPanel();
            EsconderMensaje();
            Playerseparar();
            numero_vidas?.gameObject.SetActive(false);
            laserizquerdo?.gameObject.SetActive(true);
            laserderecho?.gameObject.SetActive(true);


            if (sonidoGanaste != null)
            {
                sonidoGanaste.Play();
            }
        }



    }

    private void BloquearMovimientoHorizotal()
    {

        if (Objetoscomponentes != null)
        {
            // Busca el componente por tipo y lo desactiva
            var componente = Objetoscomponentes.GetComponent<BloqueoMovimientoTabla>();
            if (componente != null)
            {
                componente.enabled = true;
            }

        }
    }
    private void DesbloquearMovimientoHorizotal()
    {

        if (Objetoscomponentes != null)
        {
            // Busca el componente por tipo y lo desactiva
            var componente = Objetoscomponentes.GetComponent<BloqueoMovimientoTabla>();
            if (componente != null)
            {
                componente.enabled = false;
            }
        }
    }

    private void BloquearMovimientoVertical()
    {

        if (Objetoscomponentes != null)
        {
            // Busca el componente por tipo y lo desactiva
            var componente = Objetoscomponentes.GetComponent<ControlTablaOculus>();
            if (componente != null)
            {
                componente.enabled = false;
            }

        }
    }

    private void DesbloquearMovimientoVertical()
    {

        if (Objetoscomponentes != null)
        {
            // Busca el componente por tipo y lo desactiva
            var componente = Objetoscomponentes.GetComponent<ControlTablaOculus>();
            if (componente != null)
            {
                componente.enabled = true;
            }
        }
    }


    XRInputSubsystem GetXRInputSubsystem()
    {
        List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(subsystems);
        return subsystems.FirstOrDefault();
    }
    IEnumerator AvanzarAutomaticamente()
    {
        while (indiceActual < instrucciones.Count && avanceAutomaticoHabilitado)
        {
            yield return new WaitForSeconds(tiempoEntreInstrucciones);
            AvanzarInstruccion();
        }
    }

    private void AvanzarInstruccionManual()
    {
        AvanzarInstruccion();
        avanceAutomaticoHabilitado = false; // Desactivar avance automático cuando se avanza manualmente
    }

    private void AvanzarInstruccion()
    {
        if (indiceActual < instrucciones.Count - 1)
        {
            CerrarInstruccionActual();
            indiceActual++;
            MostrarInstruccion(indiceActual);
        }
        else
        {
            TerminarInstrucciones();
        }
    }

    private void RetrocederInstruccion()
    {
        if (indiceActual > 0)
        {
            CerrarInstruccionActual();
            indiceActual--;
            MostrarInstruccion(indiceActual);
        }
    }

    private void OmitirTodasInstrucciones()
    {
        DesactivarTodasInstrucciones();
        TerminarInstrucciones();
    }

    private bool PuedeAvanzarInstruccion()
    {
        if (CheckButtonPress(XRNode.RightHand, CommonUsages.secondaryButton) &&
            (Time.time - tiempoUltimaPresion) > delayPresion && contadorReinicios == 0 && intruccioneshabilidatadas == true)
        {
            tiempoUltimaPresion = Time.time;
            return true;
        }
        return false;
    }

    private bool PuedeRetrocederInstruccion()
    {
        return CheckButtonPress(XRNode.LeftHand, CommonUsages.secondaryButton) &&
               (Time.time - tiempoUltimaPresion) > delayPresion && contadorReinicios == 0 && intruccioneshabilidatadas == true;
    }

    private bool PuedeOmitirInstrucciones()
    {
        if (CheckButtonPress(XRNode.LeftHand, CommonUsages.primaryButton) &&
            (Time.time - tiempoUltimaPresion) > delayPresion && contadorReinicios == 0 && intruccioneshabilidatadas == true)
        {
            tiempoUltimaPresion = Time.time;
            return true;
        }
        return false;
    }

    private void MostrarInstruccion(int indice)
    {
        DesactivarTodasInstrucciones();
        if (indice >= 0 && indice < instrucciones.Count)
        {
            instrucciones[indice].SetActive(true);
            asistenteDeVoz.GenerarAudio(textosInstrucciones[indice]);
        }
    }

    private void CerrarInstruccionActual()
    {
        if (indiceActual < instrucciones.Count)
        {
            instrucciones[indiceActual].SetActive(false);
        }
    }

    private void DesactivarTodasInstrucciones()
    {
        foreach (GameObject instruccion in instrucciones)
        {
            if (instruccion != null)
                instruccion.SetActive(false);
        }
    }

    private void TerminarInstrucciones()
    {
        DesactivarTodasInstrucciones();
        DesbloquearMovimientoHorizotal();
        DesbloquearMovimientoVertical();
        TextoBotonPausa?.gameObject.SetActive(true);
        estaenpausa = true;
        intruccioneshabilidatadas = false;
        Debug.Log("No hay más instrucciones.");
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Detecta colisión con objetos etiquetados como "Obstaculos"
        if (collision.gameObject.CompareTag("Obstaculos"))
        {
            contador++;
            n_vidas--; // Reducir el número de vidas

            if (golpeColicion != null)
            {
                golpeColicion.Play();
            }

            // Actualizar el texto del número de vidas
            if (numero_vidas != null)
            {
                numero_vidas.text = "Vidas: " + n_vidas;
            }

            Debug.Log("Colisión detectada con obstáculo. Contador: " + contador);

            if (contador >= limiteColiciones)
            {
                MostrarMensajePerdido();
                EsconderMensaje();
                numero_vidas.gameObject.SetActive(false);
                MostrarPanel();
                laserizquerdo?.gameObject.SetActive(true);
                laserderecho?.gameObject.SetActive(true);
                Playerseparar();
                if (sonidoPerdiste != null)
                {
                    sonidoPerdiste.Play();
                }
            }
        }
    }

    private void MostrarPanel()
    {
        if (PanelBotones != null)
        {
            PanelBotones.SetActive(true);
        }
    }

    private void MostrarMensajePerdido()
    {
        if (textoPerdido != null)
        {
            textoPerdido.gameObject.SetActive(true);
            textoPerdido.text = "¡Has perdido!";
            TextoBotonPausa?.gameObject.SetActive(false);
            PanelPausa?.gameObject.SetActive(false);
            if (componetesPlayer != null)
            {
                componetesPlayer.gameObject.SetActive(true);
                Debug.Log("Componentes Activados");
            }
            Playerseparar();
        }
        else
        {
            Debug.LogWarning("El texto de 'Perdido' no está asignado en el Inspector.");
        }
    }

    private void MostrarMensajeGanar()
    {
        if (textoGanar != null)
        {
            textoGanar.gameObject.SetActive(true);
            textoGanar.text = "¡Has ganado!";
            TextoBotonPausa?.gameObject.SetActive(false);
            PanelPausa?.gameObject.SetActive(false);
            if (componetesPlayer != null)
            {
                componetesPlayer.gameObject.SetActive(true);
                Debug.Log("Componentes Activados");
            }
            Playerseparar();
        }
        else
        {
            Debug.LogWarning("El texto de 'Ganar' no está asignado en el Inspector.");

        }
    }


    private void EsconderMensaje()
    {
        if (velocidad_ditancia != null)
        {
            velocidad_ditancia.gameObject.SetActive(false);
        }
    }

    private void Playerseparar()
    {
        if (playerTransform != null)
        {
            playerTransform.transform.SetParent(null);
        }
    }

    private void ReiniciarSimulacion()
    {

        // Incrementar el contador y guardarlo en PlayerPrefs
        contadorReinicios++;
        PlayerPrefs.SetInt("ContadorReinicios", contadorReinicios);
        PlayerPrefs.Save(); // Asegurarse de guardar los cambios inmediatamente

        Debug.Log("Reiniciando escena por " + contadorReinicios + "ª vez.");

        // Reiniciar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void IrNiveles()
    {
        SceneManager.LoadScene("NivelesSurf");
    }

    private void IrMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void PausarJuego()
    {
        Time.timeScale = 0f; // Pausa el juego
        juegoPausado = true;
        PanelPausa?.gameObject.SetActive(true);
        TextoBotonPausa?.gameObject.SetActive(false);

        foreach (AudioSource sonido in sonidos)
        {
            if (sonido.isPlaying)
            {
                sonido.Pause(); // Pausar cada AudioSource
            }
        }
    }


    public void ReanudarJuego()
    {
        Time.timeScale = 1f; // Reanuda el juego
        juegoPausado = false;
        TextoBotonPausa?.gameObject.SetActive(true);
        PanelPausa?.gameObject.SetActive(false);


        // Reanudar todos los sonidos
        foreach (AudioSource sonido in sonidos)
        {
            sonido.UnPause(); // Reanudar cada AudioSource
        }
        // Aquí puedes ocultar el menú de pausa si lo tienes
    }


    private bool CheckButtonPress(XRNode handNode, InputFeatureUsage<bool> button)
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(handNode);
        bool isPressed;
        if (device.TryGetFeatureValue(button, out isPressed) && isPressed)
        {
            return true;
        }
        return false;
    }


    void OnApplicationQuit()
    {
        //Al salir de la aplicación, reiniciar el contador de reinicios
        PlayerPrefs.DeleteKey("ContadorReinicios");
        Debug.Log("Contador de reinicios reseteado al cerrar la aplicación.");
    }
}


