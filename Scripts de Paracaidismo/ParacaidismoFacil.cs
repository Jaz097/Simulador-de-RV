using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ParacaidismoFacil : MonoBehaviour
{
    //Varibles para la fisica y acción del paracaidismo...
    private Rigidbody rb;
    private bool isFalling = false;
    private bool isParachuteOpen = false;
    public float jumpForce = 50.0f;
    public float rotationSpeed = 2.0f;

    public float parachuteDrag = 5.0f;
    public float terminalVelocity = 50.0f;
    private float originalDrag;
    public float moveSpeed = 20.0f;
    private Quaternion initialRotation;
    private bool isRotatingForward = false;
    private Quaternion targetRotation;
    private bool hasCollided = false;
    public float tiltAngle = 15.0f;
    public float tiltSpeed = 2.0f;

    //Variblaes para manjear información y eventos que ocurren dentro de la simulacion
    public TextMeshProUGUI TextoInformacion;
    public GameObject BlackScreen;
    public TextMeshProUGUI Hasperdido;
    public TextMeshProUGUI AtExitoso;
    public GameObject LazerIzquierdo;
    public GameObject LazerDerecho;
    public TextMeshProUGUI Alerta;
    private bool alertaParpadeoIniciado = false;
    private float ObtenerAltura_boton = 0.0f;
    public EventoAgarre eventoAgarre;
    public GameObject abreParacaidas;

    public GameObject Paracaidas;

    public GameObject Panelbotones1;
    public Button BotonReiniciar1;
    public Button BotonContinuar1;
    public Button BotonMenu1;

    public GameObject Panelbotones2;
    public Button BotonReiniciar2;
    public Button BotonContinuar2;
    public Button BotonMenu2;

    public int contadorReinicios = 0;


    public float radius = 5.0f;  // Radio del círculo
    public float rotationSpeed2 = 30.0f;  // Velocidad de rotación
    private Vector3 centerPoint;  // Centro del círculo


    //Varibles para eventos de sonido
    public AudioSource sonidoCaida;

    public AudioSource sonido_AbrirParacaidas;

    public AudioSource sonido_alerta;

    public AudioSource Sonido_golpeSuelo;

    public AudioSource Sonido_pieSuelo;

    public AudioSource Sonido_Celebracion;

    public AudioSource Sonido_Perdida;

    public AudioSource sonidoBotones;


    //Intrucciones de la simulación
    public List<GameObject> instrucciones; // Lista de instrucciones
    public Asistente asistenteDeVoz; // Referencia a tu asistente de voz
    private int indiceActual = 0; // Índice actual de la instrucción
    public float tiempoEntreInstrucciones = 200.0f;

    //Varibles del asistente de voz 

    public List<string> textosInstrucciones;

    private float tiempoUltimaPresion = 0f;
    private float delayPresion = 0.5f;
    private bool buttonPressed = false;
    private bool avanceAutomaticoHabilitado = true;



    //Sistema de Pausa...
    public AudioSource[] sonidos;

    public GameObject PanelPausa;

    private bool juegoPausado = false;

    public TextMeshProUGUI TextoBotonPausa;
    private bool avanceManual = false;


    private float rotationAmount = 5f;  // Cuánto girará el avatar en cada oscilación
    private float rotationOffset;  // Para almacenar el valor de la rotación actual

    private bool saltohabilitado = false;
    private float velocidadRotacion = 30f; // Velocidad constante de rotación
    private float direccionRotacion = 1f; // Dirección de rotación (1 = derecha, -1 = izquierda)
    private float intervaloCambioDireccion = 2f; // Tiempo entre cambios de dirección
    private float tiempoDesdeCambioDireccion = 0f; // Temporizador que lleva la cuenta del tiempo transcurrido


    public GameObject teleDerecho;

    private bool estaenpausa = false;

    private bool intruccioneshabilidatadas = true;


    void Start()
    {


        contadorReinicios = PlayerPrefs.GetInt("ContadorReinicios", 0);

        // Definir el centro del círculo como la posición inicial del avatar
        centerPoint = transform.position;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.isKinematic = true;
        originalDrag = rb.drag;
        initialRotation = transform.rotation;

        Debug.Log("Inicio del script completado. Rigidbody inicializado.");

        PanelPausa?.gameObject.SetActive(false);


        if (BlackScreen != null)
        {
            BlackScreen.SetActive(false);
        }

        if (Hasperdido != null)
        {
            Hasperdido.gameObject.SetActive(false);
        }

        if (AtExitoso != null)
        {
            AtExitoso.gameObject.SetActive(false);
        }

        if (Alerta != null)
        {
            Alerta.gameObject.SetActive(false);
        }

        if (abreParacaidas != null)
        {
            abreParacaidas.gameObject.SetActive(false);
        }

        if (sonidoCaida != null)
        {
            sonidoCaida.Stop();  // Asegura que el sonido no se reproduzca al iniciar
        }

        if (sonido_alerta != null)
        {
            sonido_alerta.Stop();
        }

        if (sonido_AbrirParacaidas != null)
        {
            sonido_AbrirParacaidas.Stop();
        }

        if (Sonido_golpeSuelo != null)
        {
            Sonido_golpeSuelo.Stop();
        }

        if (Sonido_pieSuelo != null)
        {
            Sonido_pieSuelo.Stop();
        }

        if (Sonido_Celebracion != null)
        {
            Sonido_Celebracion.Stop();
        }

        if (Sonido_Perdida != null)
        {
            Sonido_Perdida.Stop();
        }

        if (sonidoBotones != null)
        {
            sonidoBotones.Stop();
        }


        if (Panelbotones1 != null)
        {
            Panelbotones1.gameObject.SetActive(false);


            if (BotonReiniciar1 != null)
            {
                BotonReiniciar1.onClick.AddListener(() =>
               {
                   if (sonidoBotones != null)
                   {
                       sonidoBotones.Play();
                   }
                   ReiniciarSimulacion();
               });
            }
            if (BotonContinuar1 != null)
            {

                BotonContinuar1.onClick.AddListener(() =>
                  {
                      if (sonidoBotones != null)
                      {
                          sonidoBotones.Play();
                      }
                      IrNiveles();
                  });
            }
            if (BotonMenu1 != null)
            {

                BotonMenu1.onClick.AddListener(() =>
               {
                   if (sonidoBotones != null)
                   {
                       sonidoBotones.Play();
                   }
                   IrMenu();
               });
            }
        }

        if (Panelbotones2 != null)
        {
            Panelbotones2.gameObject.SetActive(false);


            if (BotonReiniciar2 != null)
            {
                BotonReiniciar2.onClick.AddListener(() =>
               {
                   if (sonidoBotones != null)
                   {
                       sonidoBotones.Play();
                   }
                   ReiniciarSimulacion();
               });
            }
            if (BotonContinuar2 != null)
            {

                BotonContinuar2.onClick.AddListener(() =>
                  {
                      if (sonidoBotones != null)
                      {
                          sonidoBotones.Play();
                      }
                      IrNiveles();
                  });
            }
            if (BotonMenu2 != null)
            {
                BotonMenu2.onClick.AddListener(() =>
               {
                   if (sonidoBotones != null)
                   {
                       sonidoBotones.Play();
                   }
                   IrMenu();
               });
            }
        }

        if (Paracaidas != null)
        {
            Paracaidas.gameObject.SetActive(false);
        }



        if (contadorReinicios == 0)
        {
            TextoBotonPausa?.gameObject.SetActive(false);
            MostrarInstruccion(indiceActual);
            StartCoroutine(AvanzarAutomaticamente());
        }
        else if (contadorReinicios > 0)
        {
            TextoBotonPausa?.gameObject.SetActive(true);
            DesactivarTodasInstrucciones();
            saltohabilitado = true;

        }


        if (sonidos.Length == 0)
        {
            sonidos = FindObjectsOfType<AudioSource>();
        }



    }

    void Update()
    {

        float altitud = transform.position.y;

        if (CheckButtonPress(XRNode.LeftHand, CommonUsages.secondaryButton) && intruccioneshabilidatadas == false)
        {
            PausarJuego();
        }

        if (CheckButtonPress(XRNode.LeftHand, CommonUsages.primaryButton) && intruccioneshabilidatadas == false)
        {
            ReanudarJuego();
        }

        if (!isFalling && !hasCollided)
        {


            // Verifica si se presiona el botón A del Oculus Quest 2
            if (CheckButtonPress(XRNode.RightHand, CommonUsages.primaryButton) && saltohabilitado == true)
            {
                Debug.Log("Botón A presionado. Comienza la caída libre.");
                StartFreeFall();
            }
        }

        if (isFalling && !isParachuteOpen && !hasCollided)
        {

            HandleFreeFall();

            if (eventoAgarre.esAbierto && altitud >= 650f)
            {
                OpenParachute();
                ObtenerAltura_boton = altitud;


                Debug.Log("Altura que se abrió el paracaidas: " + ObtenerAltura_boton);
            }
            else if (eventoAgarre.esAbierto && altitud <= 650f)
            {
                StartCoroutine(MensajENoParacaidas());

            }
        }

        if (isParachuteOpen && !hasCollided)
        {
            HandleParachuteMovement();
        }

        if (altitud <= 900f && !alertaParpadeoIniciado)
        {
            alertaParpadeoIniciado = true; // Evita que se reinicie el parpadeo
            StartCoroutine(ParpadearAlerta());
            if (abreParacaidas != null)
            {
                abreParacaidas.gameObject.SetActive(true);
            }

        }

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


        UpdateCanvasInfo();
    }


    IEnumerator MensajENoParacaidas()
    {

        yield return new WaitForSeconds(1);
        abreParacaidas?.gameObject.SetActive(false);
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
        TextoBotonPausa?.gameObject.SetActive(true);
        intruccioneshabilidatadas = false;
        Debug.Log("No hay más instrucciones.");
        saltohabilitado = true;
    }


    void HandleMovementInput()
    {
        // Actualizamos el temporizador de cambio de dirección
        tiempoDesdeCambioDireccion += Time.deltaTime;

        // Si ha pasado el intervalo de tiempo, cambiamos la dirección de la rotación
        if (tiempoDesdeCambioDireccion >= intervaloCambioDireccion)
        {
            // Cambiamos la dirección aleatoriamente entre 1 (derecha) o -1 (izquierda)
            direccionRotacion = Random.Range(0, 2) == 0 ? 1f : -1f;
            tiempoDesdeCambioDireccion = 0f; // Reseteamos el temporizador
        }

        // Aplicamos la rotación continua en el eje Y, dependiendo de la dirección
        transform.Rotate(Vector3.up, direccionRotacion * velocidadRotacion * Time.deltaTime, Space.World);

        // Movimiento hacia adelante
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 moveDirection = transform.forward * verticalInput * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + moveDirection);
    }

    void StartFreeFall()
    {
        isFalling = true;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(transform.forward * jumpForce, ForceMode.VelocityChange);
        Debug.Log("Fuerza de salto aplicada: " + (transform.forward * jumpForce).ToString());
        isRotatingForward = true;
        targetRotation = Quaternion.Euler(90, transform.eulerAngles.y, transform.eulerAngles.z);


        // Reproducir el sonido de caída libre si está asignado
        if (sonidoCaida != null)
        {
            sonidoCaida.Play();
        }
        else
        {
            Debug.LogWarning("El sonido de caída libre no está asignado.");
        }
    }

    void ApplyLightRotation()
    {
        // La oscilación de la rotación se genera con la función seno para hacer un movimiento suave
        rotationOffset = Mathf.Sin(Time.time * rotationSpeed) * rotationAmount;

        // Aplicamos la rotación al avatar en el eje Y (puedes ajustar el eje si lo prefieres)
        transform.rotation = Quaternion.Euler(0, 0, rotationOffset);
    }

    void HandleFreeFall()
    {

        if (isRotatingForward)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                isRotatingForward = false;
            }
        }

        ApplyCustomGravity();

        if (rb.velocity.magnitude > terminalVelocity)
        {
            rb.velocity = rb.velocity.normalized * terminalVelocity;
        }

        HandleMovementInput();

    }


    void ApplyCustomGravity()
    {
        float customGravity = Mathf.Lerp(9.81f, 50.0f, rb.velocity.magnitude / terminalVelocity);
        rb.AddForce(Vector3.down * customGravity, ForceMode.Acceleration);
    }

    void HandleParachuteMovement()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        Vector3 moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;

        Vector3 gravityMovement = Vector3.down * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement + gravityMovement);

        float targetTiltVertical = verticalInput * tiltAngle;
        float targetTiltHorizontal = horizontalInput * tiltAngle;
        Quaternion targetRotation = Quaternion.Euler(targetTiltVertical, transform.eulerAngles.y, -targetTiltHorizontal);

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * tiltSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Terrain")) // Asegúrate de que el terreno tenga la etiqueta "Terrain"
        {
            Debug.Log("Entró en el área de aterrizaje");

            if (isParachuteOpen && ObtenerAltura_boton >= 650f)
            {
                // Éxito en el aterrizaje
                AtExitoso?.gameObject.SetActive(true);
                AtExitoso.text = "!Aterrezaje con Exito!";

                LazerDerecho?.gameObject.SetActive(true);
                LazerIzquierdo?.gameObject.SetActive(true);

                TextoBotonPausa?.gameObject.SetActive(false);

                MostrarPanel1();
                TextoInformacion?.gameObject.SetActive(false);

                if (sonidoCaida != null)
                {
                    sonidoCaida.Stop();
                }

                if (Sonido_pieSuelo != null)
                {
                    Sonido_pieSuelo.Play();
                }

                if (Sonido_Celebracion != null)
                {
                    Sonido_Celebracion.Play();
                }

                Paracaidas?.gameObject.SetActive(false);

                abreParacaidas?.gameObject.SetActive(false);

                teleDerecho?.gameObject.SetActive(true);



                Debug.Log("¡Aterrizaje con Éxito!");
            }
            else
            {
                // Fallo en el aterrizaje
                transform.rotation = initialRotation;
                BlackScreen?.SetActive(true);

                Hasperdido?.gameObject.SetActive(true);
                Hasperdido.text = "¡Haz muerto! No abriste el paracaídas...";

                LazerDerecho?.gameObject.SetActive(true);
                LazerIzquierdo?.gameObject.SetActive(true);
                TextoBotonPausa?.gameObject.SetActive(false);
                abreParacaidas?.gameObject.SetActive(false);
                Paracaidas?.gameObject.SetActive(false);


                if (sonidoCaida != null)
                {
                    sonidoCaida.Stop();
                }

                if (Sonido_golpeSuelo != null)
                {
                    Sonido_golpeSuelo.Play();
                }

                if (Sonido_Perdida != null)
                {
                    Sonido_Perdida.Play();
                }

                teleDerecho?.gameObject.SetActive(false);

                MostrarPanel2();

                Debug.Log("¡Haz muerto! No abriste el paracaídas...");
            }

            // Desactivar la física después del aterrizaje
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            hasCollided = true;
            rb.isKinematic = true; // Mover aquí para evitar interferencias con la colisión
        }
    }

    void OpenParachute()
    {
        if (isFalling && !isParachuteOpen)
        {
            // Reproduce el sonido de apertura del paracaídas
            if (sonido_AbrirParacaidas != null)
            {
                sonido_AbrirParacaidas.Play();
            }

            // Cambia el estado del paracaídas
            isParachuteOpen = true;

            if (Paracaidas != null)
            {
                Paracaidas.gameObject.SetActive(true);
            }

            abreParacaidas?.gameObject.SetActive(false);
            teleDerecho?.gameObject.SetActive(true);

            // Ajusta el drag para simular la resistencia del paracaídas
            rb.drag = parachuteDrag; // Asegúrate de que parachuteDrag esté configurado adecuadamente

            // Detiene el movimiento vertical inicial para evitar que el paracaídas se despliegue
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        }
    }

    void UpdateCanvasInfo()
    {
        if (TextoInformacion != null)
        {
            float altitud = transform.position.y;
            float velocidad = rb.velocity.magnitude;

            Debug.Log($"Altitud: {altitud:F1} m, Velocidad: {velocidad:F1} m/s");

            TextoInformacion.text = $"Altitud: {altitud:F1} m --- Velocidad: {velocidad:F1} m/s";
        }
        else
        {
            Debug.LogError("TextoInformacion no está asignado en el Inspector.");
        }
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




    private IEnumerator ParpadearAlerta()
    {
        int parpadeos = 3; // Número total de parpadeos
        float intervalo = 0.5f; // Intervalo entre parpadeos (en segundos)

        for (int i = 0; i < parpadeos; i++)
        {
            if (Alerta != null)
            {
                Alerta.text = "!Abre el paracidas!";
                sonido_alerta.Play();
                Alerta.gameObject.SetActive(!Alerta.gameObject.activeSelf); // Alterna la visibilidad
            }
            yield return new WaitForSeconds(intervalo); // Espera antes de alternar de nuevo
        }

        if (Alerta != null)
        {
            Alerta.gameObject.SetActive(false);
        }
    }

    public void PausarJuego()
    {
        Time.timeScale = 0f; // Pausa el juego
        juegoPausado = true;
        PanelPausa?.gameObject.SetActive(true);
        TextoBotonPausa?.gameObject.SetActive(false);
        TextoInformacion?.gameObject.SetActive(false);

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
        TextoInformacion?.gameObject.SetActive(true);

        // Reanudar todos los sonidos
        foreach (AudioSource sonido in sonidos)
        {
            sonido.UnPause(); // Reanudar cada AudioSource
        }
        // Aquí puedes ocultar el menú de pausa si lo tienes
    }
    private void MostrarPanel1()
    {
        if (Panelbotones1 != null)
        {
            Panelbotones1.gameObject.SetActive(true); // Corrige el nombre de la variable
        }
    }

    private void MostrarPanel2()
    {
        if (Panelbotones2 != null)
        {
            Panelbotones2.gameObject.SetActive(true); // Corrige el nombre de la variable
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
        SceneManager.LoadScene("NivelesParacaidismo");
    }

    private void IrMenu()
    {
        SceneManager.LoadScene("Menu");
    }


    void OnApplicationQuit()
    {
        //Al salir de la aplicación, reiniciar el contador de reinicios
        PlayerPrefs.DeleteKey("ContadorReinicios");
        Debug.Log("Contador de reinicios reseteado al cerrar la aplicación.");
    }
}




