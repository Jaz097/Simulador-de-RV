using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Linq;
using UltimateXR.Extensions.Unity;
using System.Runtime.CompilerServices;



public class SurfTtutorial : MonoBehaviour
{
    public TextMeshProUGUI textoPerdido;
    public TextMeshProUGUI textoGanar;
    public TextMeshProUGUI velocidad_ditancia;
    public TextMeshProUGUI numero_vidas;
    public int limiteColiciones = 5;
    public float distanciaObjetivo = 100f;
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

    private int pasoActual = 0;

    private ControlTablaOculus movimientovertical;

    private float tiempoRetraso = 6f; // Tiempo de espera entre pasos (en segundos)
    private bool esperando = false; // Indica si se está esperando antes de pasar al siguiente paso

    public GameObject tutorial1;

    public GameObject tutorial2;

    public GameObject tutorial3;

    public GameObject tutorial4;

    public GameObject Objetoscomponentes;

    private bool esperar = false;

    private bool EnPausa = false;

    private bool fin;


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
            numero_vidas.gameObject.SetActive(false);
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

        if (sonidos.Length == 0)
        {
            sonidos = FindObjectsOfType<AudioSource>();
        }

        tutorial1?.gameObject.SetActive(false);
        BloquearMovimientoHorizotal();
        BloquearMovimientoVertical();

        StartCoroutine(EjecutarPasoConEspera());



    }

    private void Update()
    {

        if (CheckButtonPress(XRNode.LeftHand, CommonUsages.secondaryButton))
        {
            PausarJuego();
            EsconderMensaje();
            numero_vidas?.gameObject.SetActive(false);
            EnPausa = true;
        }


        if (CheckButtonPress(XRNode.LeftHand, CommonUsages.primaryButton))
        {
            ReanudarJuego();
            velocidad_ditancia?.gameObject.SetActive(true);
            numero_vidas?.gameObject.SetActive(true);
            EnPausa = true;
        }

        float distanciaRecorrida = Vector3.Distance(posicionInicial, playerTransform.position);
        if (distanciaRecorrida >= distanciaObjetivo)
        {
            fin = true;
            Debug.Log(fin);
        }
    }

    private IEnumerator EjecutarPasoConEspera()
    {
        while (pasoActual <= 5) // Continuar mientras haya pasos por ejecutar
        {
            switch (pasoActual)
            {
                case 0: // Paso de bienvenida
                    esperar = true;
                    if (asistenteDeVoz && esperar == true)
                    {
                        asistenteDeVoz.GenerarAudio("Este es el tutorial de surf. Sigue con atención todas las instrucciones...");
                        esperar = false;
                    }
                    yield return new WaitForSeconds(6);
                    break;

                case 1: // Paso sobre movimiento hacia el frente
                    esperar = true;
                    if (asistenteDeVoz && esperar == true)
                    {
                        asistenteDeVoz.GenerarAudio("Para realizar el movimiento hacia el frente, utiliza la palanca de pulgar del mando izquierdo. Puedes visualizar en pantalla su ubicación...");
                        esperar = false;
                    }
                    tutorial1?.gameObject.SetActive(true);
                    DesbloquearMovimientoHorizotal(); // Habilitar el movimiento horizontal
                    yield return new WaitUntil(() => Input.GetAxisRaw("Horizontal") > 0.01f);
                    tutorial1?.gameObject.SetActive(false);
                    BloquearMovimientoHorizotal();
                    esperar = true;
                    if (asistenteDeVoz && esperar == true)
                    {
                        asistenteDeVoz.GenerarAudio("¡Muy bien! Espera el siguiente paso...");
                        esperar = false;
                    }
                    yield return new WaitForSeconds(4); // Esperar 4 segundos para continuar

                    break;

                case 2: // Paso sobre movimiento hacia la derecha
                    esperar = true;
                    if (asistenteDeVoz && esperar == true)
                    {
                        asistenteDeVoz.GenerarAudio("Para el movimiento hacia la derecha, utiliza el disparador del mando derecho. En pantalla tienes la ubicación del botón...");
                        esperar = false;
                    }
                    tutorial2?.gameObject.SetActive(true);
                    DesbloquearMovimientoVertical();
                    yield return new WaitUntil(() => CheckButtonPress(XRNode.RightHand, CommonUsages.triggerButton));
                    tutorial2?.gameObject.SetActive(false);
                    yield return new WaitForSeconds(4);
                    BloquearMovimientoVertical();
                    esperar = true;
                    if (asistenteDeVoz && esperar == true)
                    {
                        asistenteDeVoz.GenerarAudio("¡Muy bien! Espera el siguiente paso...");
                        esperar = false;
                    }
                    yield return new WaitForSeconds(4); // Esperar 4 segundos antes de finalizar
                    break;

                case 3: // Paso sobre movimiento hacia la derecha
                    esperar = true;
                    if (asistenteDeVoz && esperar == true)
                    {
                        asistenteDeVoz.GenerarAudio("Para el movimiento hacia la izquierda, utiliza el disparador del mando derecho. En pantalla tienes la ubicación del botón...");
                        esperar = false;
                    }
                    DesbloquearMovimientoVertical();
                    tutorial3?.gameObject.SetActive(true);
                    yield return new WaitUntil(() => CheckButtonPress(XRNode.LeftHand, CommonUsages.triggerButton));
                    tutorial3?.gameObject.SetActive(false);
                    yield return new WaitForSeconds(4);
                    BloquearMovimientoHorizotal();
                    BloquearMovimientoVertical();
                    esperar = true;
                    if (asistenteDeVoz && esperar == true)
                    {
                        asistenteDeVoz.GenerarAudio("¡Muy bien! Ya has identificado todos los botones...");
                        esperar = false;
                    }
                    yield return new WaitForSeconds(4); // Esperar 4 segundos antes de finalizar
                    break;

                case 4: // Paso sobre movimiento hacia la derecha

                    esperar = true;
                    if (asistenteDeVoz && esperar == true)
                    {
                        asistenteDeVoz.GenerarAudio("A continuación, en pantalla veras cierta informacion para la simulacion...");
                        esperar = false;
                    }
                    yield return new WaitForSeconds(6);
                    esperar = true;
                    if (asistenteDeVoz && esperar == true)
                    {
                        asistenteDeVoz.GenerarAudio("Se muestra el numero de vidas que tienes para terminar la simulación.");
                        if (numero_vidas != null)
                        {
                            numero_vidas.gameObject.SetActive(true);
                            numero_vidas.text = "Vidas: " + n_vidas;
                        }
                        esperar = false;
                    }
                    yield return new WaitForSeconds(6);
                    esperar = true;
                    if (asistenteDeVoz && esperar == true)
                    {
                        asistenteDeVoz.GenerarAudio("Tambien se muestra la velocidad y distancia recorrida al surfear.");
                        velocidad_ditancia?.gameObject.SetActive(true);
                        esperar = false;
                    }
                    yield return new WaitForSeconds(6);
                    esperar = true;
                    if (asistenteDeVoz && esperar == true)
                    {
                        TextoBotonPausa?.gameObject.SetActive(true);
                        asistenteDeVoz.GenerarAudio("En la parte de abajo, se muestra una indicacion para el sistema de pausa. Puedes probar presionando i griega para pausar, X para reunudar. En pantalla puedes ubicar los botones...");
                        yield return new WaitForSeconds(2);
                        tutorial4?.gameObject.SetActive(true);
                        yield return new WaitUntil(() => CheckButtonPress(XRNode.LeftHand, CommonUsages.primaryButton));
                        tutorial4?.gameObject.SetActive(false);
                        esperar = false;
                    }
                    yield return new WaitForSeconds(2);
                    break;
                case 5:
                    esperar = true;
                    if (asistenteDeVoz && esperar == true)
                    {
                        yield return new WaitForSeconds(6);
                        asistenteDeVoz.GenerarAudio("¡Muy bien! Ahora puedes hacer una prueba para poner en practica todo lo que aprendiste...");
                        esperar = false;
                    }
                    esperar = true;
                    if (asistenteDeVoz && esperar == true)
                    {
                        yield return new WaitForSeconds(6);
                        asistenteDeVoz.GenerarAudio("Surfea un poco para probar tus conocimiento en este tutorial...");
                        esperar = false;
                    }

                    esperar = true;
                    if (asistenteDeVoz && esperar == true)
                    {
                        yield return new WaitForSeconds(6);
                        asistenteDeVoz.GenerarAudio("Si chocas con los obstaculos, retrocede un poco y continuar. Cada vez que choques se restará vidas en el indicador... Empieza a surfear, hasta que se indique que has completado la prueba... ");
                        esperar = false;
                    }
                    DesbloquearMovimientoHorizotal();
                    DesbloquearMovimientoVertical();
                    yield return new WaitUntil(() => fin == true);
                    BloquearMovimientoHorizotal();
                    BloquearMovimientoVertical();
                    esperar = true;
                    if (asistenteDeVoz && esperar == true)
                    {
                        asistenteDeVoz.GenerarAudio("¡Excelente! Lo hiciste muy bien. Completaste el tutorial... Buena suerte en la simulación.");
                        esperar = false;
                    }
                    yield return new WaitForSeconds(7);

                    break;
            }

            pasoActual++; // Avanzar al siguiente paso después de ejecutar el caso actual
        }

        SceneManager.LoadScene("NivelesSurf");
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

    private bool PuedeAvanzarInstruccion()
    {
        if (CheckButtonPress(XRNode.RightHand, CommonUsages.secondaryButton) &&
            (Time.time - tiempoUltimaPresion) > delayPresion)
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


