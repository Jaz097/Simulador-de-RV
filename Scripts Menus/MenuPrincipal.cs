using System;
using GoogleTextToSpeech.Scripts;
using GoogleTextToSpeech.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.XR;
using System.Linq;
using System.Collections.Generic;

public class MenuPrincipal : MonoBehaviour
{
    public Button botonParacaidismo; // Referencia al botón de Paracaidismo
    public Button botonSurf; // Referencia al botón de Surf
    public Button botonSalir;
    public TextMeshProUGUI textoNombreJugador;

    public AudioSource fuenteDeAudioBienvenida; // Fuente de audio para el mensaje de bienvenida
    public AudioSource fuenteDeAudioDeportes; // Fuente de audio para el mensaje sobre deportes extremos
    public TextToSpeech textoAVoz;
    public VoiceScriptableObject voz;

    private bool mensajeBienvenidaReproducido = false; // Para asegurar que el mensaje de bienvenida se reproduce solo una vez
    private bool reproduciendoDeportesExtremos = false; // Para controlar si se está reproduciendo el mensaje sobre deportes extremos
    private float intervaloReproduccion = 10f; // Intervalo de tiempo en segundos

    public AudioSource SonidoBoton;


    public int contadorReinicios = 0;



    void Start()
    {


        contadorReinicios = PlayerPrefs.GetInt("ContadorReiniciosMenu", 0);

        XRInputSubsystem xrSubsystem = GetXRInputSubsystem();
        if (xrSubsystem != null)
        {
            xrSubsystem.TryRecenter();
        }

        // Obtener el nombre del jugador de PlayerPrefs
        string nombreJugador = PlayerPrefs.GetString("NombreJugador", "Jugador");

        // Mostrar el nombre en el UI
        textoNombreJugador.text = "Bienvenido " + nombreJugador;

        // Reproducir mensaje de bienvenida al jugador

        if (contadorReinicios == 0)
        {
            ReproducirMensajeDeBienvenida(nombreJugador);
        }

        // Configurar eventos de clic en los botones
        if (botonParacaidismo != null)
        {
            botonParacaidismo.onClick.AddListener(() =>
            {

                contadorReinicios++;
                PlayerPrefs.SetInt("ContadorReiniciosMenu", contadorReinicios);
                PlayerPrefs.Save();

                CargarEscenaParacaidismo();

                if (SonidoBoton != null)
                {
                    SonidoBoton.Play();
                }
            });
        }

        if (botonSurf != null)
        {
            botonSurf.onClick.AddListener(() =>
            {
                contadorReinicios++;
                PlayerPrefs.SetInt("ContadorReiniciosMenu", contadorReinicios);
                PlayerPrefs.Save();

                CargarEscenaSurf();

                if (SonidoBoton != null)
                {
                    SonidoBoton.Play();
                }
            });
        }

        if (botonSalir != null)
        {
            botonSalir.onClick.AddListener(() =>
            {
                SalirSimulacion();

                if (SonidoBoton != null)
                {
                    SonidoBoton.Play();
                }
            });
        }
    }

    void Update()
    {

        if (contadorReinicios == 0)
        {
            StartCoroutine(RepetirMensajeDeDeportesExtremos());
        }
    }


    private void ReproducirMensajeDeBienvenida(string nombreJugador)
    {
        string mensajeBienvenida = $"Bienvenido {nombreJugador}";
        SolicitarAudio(mensajeBienvenida, fuenteDeAudioBienvenida, "bienvenida");
    }

    private void ReproducirMensajeDeDeportesExtremos()
    {
        if (!reproduciendoDeportesExtremos)
        {
            string mensajeDeportesExtremos = "¡Prepárate para experimentar los deportes extremos dentro de la Realidad Virtual! Explorando mundos inmersos, la adrenalina es realmente extraordinaria... Con el Paracaidismo y el Surf, la emoción será fantástica.....................................................";
            SolicitarAudio(mensajeDeportesExtremos, fuenteDeAudioDeportes, "deportes");
            reproduciendoDeportesExtremos = true; // Marcar como reproducido
        }
    }

    private IEnumerator EsperarInicioDeDeportesExtremos()
    {
        // Esperar hasta que termine la reproducción del mensaje de bienvenida
        while (fuenteDeAudioBienvenida.isPlaying)
        {
            yield return null;
        }

        // Reproducir el mensaje sobre deportes extremos
        ReproducirMensajeDeDeportesExtremos();
        StartCoroutine(RepetirMensajeDeDeportesExtremos());
    }

    private IEnumerator RepetirMensajeDeDeportesExtremos()
    {
        while (true)
        {
            // Esperar a que el audio de deportes extremos termine
            yield return new WaitUntil(() => !fuenteDeAudioDeportes.isPlaying);

            // Esperar 10 segundos antes de reproducirlo de nuevo
            yield return new WaitForSeconds(intervaloReproduccion);

            // Reproducir el mensaje de deportes extremos de nuevo
            ReproducirMensajeDeDeportesExtremos();
        }
    }

    private void SolicitarAudio(string texto, AudioSource fuenteDeAudio, string tipo)
    {
        // Crear una instancia temporal de TextToSpeech para manejar la solicitud
        TextToSpeech tempTextoAVoz = Instantiate(textoAVoz);
        Action<AudioClip> clipDeAudioRecibido = (clip) => ClipDeAudioRecibido(clip, fuenteDeAudio, tipo);
        Action<BadRequestData> errorRecibido = (error) => ErrorRecibido(error);

        tempTextoAVoz.GetSpeechAudioFromGoogle(texto, voz, clipDeAudioRecibido, errorRecibido);
    }

    private void ClipDeAudioRecibido(AudioClip clip, AudioSource fuenteDeAudio, string tipo)
    {
        if (clip == null)
        {
            Debug.LogError($"Clip de audio recibido para {tipo} es nulo, no se puede reproducir.");
            return;
        }

        if (tipo == "bienvenida")
        {
            fuenteDeAudio.Stop();
            fuenteDeAudio.clip = clip;
            fuenteDeAudio.Play();
            mensajeBienvenidaReproducido = true; // Marcar como reproducido

            // Esperar un momento antes de iniciar el mensaje sobre deportes extremos
            StartCoroutine(EsperarInicioDeDeportesExtremos());
        }
        else if (tipo == "deportes")
        {
            ReproducirAudio(fuenteDeAudio, clip);
        }
        Debug.Log($"Clip de audio para {tipo} reproducido con éxito.");
    }

    private void ReproducirAudio(AudioSource fuente, AudioClip clip)
    {
        fuente.Stop();
        fuente.clip = clip;
        fuente.loop = false;
        fuente.Play();
    }

    private void CargarEscenaParacaidismo()
    {
        SceneManager.LoadScene("NivelesParacaidismo");
    }

    private void CargarEscenaSurf()
    {
        SceneManager.LoadScene("NivelesSurf");
    }

    private void ErrorRecibido(BadRequestData datosDeError)
    {
        Debug.Log($"Error {datosDeError.error.code} : {datosDeError.error.message}");
    }

    private void SalirSimulacion()
    {
        // Cerrar la aplicación
        Debug.Log("Cerrando Aplicacion...");
        Application.Quit();
    }

    private bool VerificarPresionBoton(XRNode nodoMano, InputFeatureUsage<bool> boton)
    {
        InputDevice dispositivo = InputDevices.GetDeviceAtXRNode(nodoMano);
        bool estaPresionado;

        if (dispositivo.TryGetFeatureValue(boton, out estaPresionado) && estaPresionado)
        {
            return true;
        }

        return false;
    }

    XRInputSubsystem GetXRInputSubsystem()
    {
        List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(subsystems);
        return subsystems.FirstOrDefault();
    }



    void OnApplicationQuit()
    {
        //Al salir de la aplicación, reiniciar el contador de reinicios
        PlayerPrefs.DeleteKey("ContadorReiniciosMenu");
        Debug.Log("Contador de reinicios reseteado al cerrar la aplicación.");
    }


}
