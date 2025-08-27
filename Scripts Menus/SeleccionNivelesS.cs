using System;
using GoogleTextToSpeech.Scripts;
using GoogleTextToSpeech.Scripts.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR;
using System.Linq;
using System.Collections.Generic;

public class SeleccionSurf : MonoBehaviour
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
            CargarSurfFacil();

            if (SonidoBoton != null)
            {
                SonidoBoton.Play();
            }
        });

        botonMedio.onClick.AddListener(() =>
        {
            CargarSurfMedio();

            if (SonidoBoton != null)
            {
                SonidoBoton.Play();
            }
        });

        botonDificil.onClick.AddListener(() =>
        {
            CargarSurfDificil();

            if (SonidoBoton != null)
            {
                SonidoBoton.Play();
            }
        });

        botonTutorial.onClick.AddListener(() =>
        {
            Cargartutorial();

            if (SonidoBoton != null)
            {
                SonidoBoton.Play();
            }
        });

        // Anunciar que se ha seleccionado el paracaidismo y que pueden elegir un nivel
        AnunciarSeleccionSurf();


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
    public void CargarSurfFacil()
    {
        PlayerPrefs.SetString("NivelSeleccionado", "Facil");
        SceneManager.LoadScene("SurfFacil");
    }

    public void CargarSurfMedio()
    {
        PlayerPrefs.SetString("NivelSeleccionado", "Medio");
        SceneManager.LoadScene("SurfMedio");
    }

    public void CargarSurfDificil()
    {
        PlayerPrefs.SetString("NivelSeleccionado", "Dificil");
        SceneManager.LoadScene("SurfDificil");
    }

    public void Cargartutorial()
    {
        PlayerPrefs.SetString("TutorialSurf", "Tutorial");
        SceneManager.LoadScene("SurfTutorial");
    }

    private void AnunciarSeleccionSurf()
    {
        string mensaje = "Has seleccionado el Surf.... Por favor, elige el nivel de dificultad que deseas.";
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
