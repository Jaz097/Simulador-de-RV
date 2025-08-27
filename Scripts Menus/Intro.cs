using System;
using GoogleTextToSpeech.Scripts;
using GoogleTextToSpeech.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Linq;
using System.Collections.Generic;
using UltimateXR;
using UltimateXR.CameraUtils;


public class Intro : MonoBehaviour
{
    public TMP_InputField userName;
    public Button botonContinuar;
    public TextMeshProUGUI mensajeAdvertencia;
    public AudioSource fuenteDeAudio;
    public TextToSpeech textoAVoz;
    public VoiceScriptableObject voz;

    public AudioSource Sonido_Boton;

    private UxrCameraFade uxrCameraFade;



    void Start()
    {

        uxrCameraFade = FindAnyObjectByType<UxrCameraFade>();

        if (uxrCameraFade.IsFading)
        {
            ejecucionSimulador();
        }




    }


    private void ejecucionSimulador()
    {
        XRInputSubsystem xrSubsystem = GetXRInputSubsystem();
        if (xrSubsystem != null)
        {
            xrSubsystem.TryRecenter();
        }

        if (Sonido_Boton != null)
        {
            Sonido_Boton.Stop();
        }

        // Reproducir mensaje de bienvenida al inicio
        ReproducirMensajeDeBienvenida();

        // Añadir el listener para el botón continuar
        botonContinuar.onClick.AddListener(() =>
        {
            ButtonDemo();

            if (Sonido_Boton != null)
            {
                Sonido_Boton.Play();
            }
        });

        // Añadir el listener para cuando se selecciona el campo de texto
        userName.onSelect.AddListener(delegate
        {
            OcultarMensajeAdvertencia();

            if (Sonido_Boton != null)
            {
                Sonido_Boton.Play();
            }
        });
    }

    void ReproducirMensajeDeBienvenida()
    {
        string textoBienvenida = "¡Hola! Soy el asistente del simulador. A continuación escribe tu nombre...";
        SolicitarAudio(textoBienvenida, "bienvenida");
    }

    public void ButtonDemo()
    {
        if (string.IsNullOrEmpty(userName.text))
        {
            mensajeAdvertencia.text = "Por favor, ingresa tu nombre...";
            ReproducirMensajeAdvertencia();
        }
        else
        {
            PlayerPrefs.SetString("NombreJugador", userName.text);
            PlayerPrefs.Save();

            SceneManager.LoadScene("Menu");
        }
    }

    void ReproducirMensajeAdvertencia()
    {
        string textoAdvertencia = "No has escrito tu nombre. Por favor, ingresa tu nombre para continuar.";
        SolicitarAudio(textoAdvertencia, "advertencia");
    }

    void SolicitarAudio(string texto, string tipo)
    {
        // Crear una instancia temporal de TextToSpeech para manejar la solicitud
        TextToSpeech tempTextoAVoz = Instantiate(textoAVoz);
        Action<AudioClip> clipDeAudioRecibido = (clip) => ClipDeAudioRecibido(clip, tipo);
        Action<BadRequestData> errorRecibido = (error) => ErrorRecibido(error);

        tempTextoAVoz.GetSpeechAudioFromGoogle(texto, voz, clipDeAudioRecibido, errorRecibido);
    }

    void ClipDeAudioRecibido(AudioClip clip, string tipo)
    {
        if (clip == null)
        {
            Debug.LogError($"Clip de audio recibido para {tipo} es nulo, no se puede reproducir.");
            return;
        }

        fuenteDeAudio.Stop();
        fuenteDeAudio.clip = clip;
        fuenteDeAudio.Play();
        Debug.Log($"Clip de audio para {tipo} reproducido con éxito.");
    }

    void OcultarMensajeAdvertencia()
    {
        mensajeAdvertencia.text = "";
    }

    private void ErrorRecibido(BadRequestData datosDeError)
    {
        Debug.Log($"Error {datosDeError.error.code} : {datosDeError.error.message}");
    }

    XRInputSubsystem GetXRInputSubsystem()
    {
        List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(subsystems);
        return subsystems.FirstOrDefault();
    }

}


//delegate { OcultarMensajeAdvertencia(); }//