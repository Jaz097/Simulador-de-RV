using System;
using GoogleTextToSpeech.Scripts;
using GoogleTextToSpeech.Scripts.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Linq;
using System.Collections.Generic;

public class SeleccionParacaidismo : MonoBehaviour
{
    // Definir los campos públicos para los botones en el Inspector
    public Button botonFacil;
    public Button botonMedio;
    public Button botonDificil;
    public Button botonTutorial;
    public AudioSource fuenteDeAudio;
    public TextToSpeech textoAVoz;
    public VoiceScriptableObject voz;

    private Action<AudioClip> _clipDeAudioRecibido;
    private Action<BadRequestData> _errorRecibido;

    public AudioSource SonidoBoton;

    void Start()
    {
        XRInputSubsystem xrSubsystem = GetXRInputSubsystem();
        if (xrSubsystem != null)
        {
            xrSubsystem.TryRecenter();
        }

        _errorRecibido += ErrorRecibido;
        _clipDeAudioRecibido += ClipDeAudioRecibido;

        if (SonidoBoton != null)
        {
            SonidoBoton.Stop();
        }

        // Asignar los métodos correspondientes a los botones
        botonFacil.onClick.AddListener(() =>
        {
            CargarParacaidismofFacil();

            if (SonidoBoton != null)
            {
                SonidoBoton.Play();
            }
        });

        botonMedio.onClick.AddListener(() =>
        {
            CargarParacaidismoMedio();

            if (SonidoBoton != null)
            {
                SonidoBoton.Play();
            }
        });

        botonDificil.onClick.AddListener(() =>
        {
            CargarParacaidismoDificil();

            if (SonidoBoton != null)
            {
                SonidoBoton.Play();
            }
        });

        botonTutorial.onClick.AddListener(() =>
        {
             CargarTutorial();

         if (SonidoBoton != null)
           {
               SonidoBoton.Play();
           }
        });


        AnunciarSeleccionParacaidismo();


        if (VerificarPresionBoton(XRNode.LeftHand, CommonUsages.secondaryButton))
        {
            SalirSimulacion();
        }
    }

    void Update()
    {
        if (VerificarPresionBoton(XRNode.LeftHand, CommonUsages.primaryButton))
        {
            RegresarMenu();
        }

        if (VerificarPresionBoton(XRNode.LeftHand, CommonUsages.secondaryButton))
        {
            SalirSimulacion();
        }
    }
    public void CargarParacaidismofFacil()
    {
        PlayerPrefs.SetString("NivelSeleccionado", "Facil");
        SceneManager.LoadScene("ParacaidismoFacil");
    }

    public void CargarParacaidismoMedio()
    {
        PlayerPrefs.SetString("NivelSeleccionado", "Medio");
        SceneManager.LoadScene("ParacaidismoMedio");
    }

    public void CargarParacaidismoDificil()
    {
        PlayerPrefs.SetString("NivelSeleccionado", "Dificil");
        SceneManager.LoadScene("ParacaidismoDificil");
    }
    public void CargarTutorial()
    {
        PlayerPrefs.SetString("NivelSeleccionado", "Tutorial");
        SceneManager.LoadScene("ParacaidismoTutorial");
    }

    private void AnunciarSeleccionParacaidismo()
    {
        string mensaje = "Has seleccionado el Paracaidismo.... Por favor, elige el nivel de dificultad que deseas.";
        textoAVoz.GetSpeechAudioFromGoogle(mensaje, voz, _clipDeAudioRecibido, _errorRecibido);
    }

    private void ErrorRecibido(BadRequestData datosDeError)
    {
        Debug.Log($"Error {datosDeError.error.code} : {datosDeError.error.message}");
    }

    private void ClipDeAudioRecibido(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("Clip de audio recibido es nulo, no se puede reproducir.");
            return;
        }

        fuenteDeAudio.Stop();
        fuenteDeAudio.clip = clip;
        fuenteDeAudio.Play();
        Debug.Log("Clip de audio reproducido con éxito.");
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

    private void SalirSimulacion()
    {

        // Cerrar la aplicación
        Debug.Log("Cerrando Aplicacion...");
        Application.Quit();

    }

    private void RegresarMenu()
    {
        SceneManager.LoadScene("Menu");
    }


    XRInputSubsystem GetXRInputSubsystem()
    {
        List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(subsystems);
        return subsystems.FirstOrDefault();
    }


}
