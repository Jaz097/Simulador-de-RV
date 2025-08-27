using System;
using UnityEngine;
using GoogleTextToSpeech.Scripts;
using GoogleTextToSpeech.Scripts.Data;

public class Asistente : MonoBehaviour
{
    public TextToSpeech textoAVoz; // Referencia al componente de TextToSpeech
    public VoiceScriptableObject voz; // Referencia al objeto de voz
    public AudioSource fuenteDeAudio; // AudioSource para reproducir el audio

    public void GenerarAudio(string texto)
    {
        if (textoAVoz == null)
        {
            Debug.LogError("textoAVoz no está asignado. Por favor, asigna un componente de TextToSpeech.");
            return;
        }

        if (voz == null)
        {
            Debug.LogError("voz no está asignada. Por favor, asigna un objeto de voz.");
            return;
        }

        // Crear una instancia temporal de TextToSpeech para manejar la solicitud
        TextToSpeech tempTextoAVoz = Instantiate(textoAVoz);

        // Acciones a realizar cuando se reciba el audio
        Action<AudioClip> clipDeAudioRecibido = (clip) => ClipDeAudioRecibido(clip);
        Action<BadRequestData> errorRecibido = (error) => ErrorRecibido(error);

        // Solicitar el audio
        Debug.Log($"Solicitando audio para el texto: {texto}");
        tempTextoAVoz.GetSpeechAudioFromGoogle(texto, voz, clipDeAudioRecibido, errorRecibido);
    }

    void ClipDeAudioRecibido(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogError("Clip de audio recibido es nulo, no se puede reproducir.");
            return;
        }

        Debug.Log("Clip de audio recibido correctamente.");
        if (fuenteDeAudio == null)
        {
            Debug.LogError("AudioSource no está asignado. Por favor, asigna un componente AudioSource.");
            return;
        }

        fuenteDeAudio.Stop();
        fuenteDeAudio.clip = clip;
        fuenteDeAudio.Play();
        Debug.Log("Clip de audio reproducido con éxito.");
    }

    private void ErrorRecibido(BadRequestData datosDeError)
    {
        if (datosDeError != null && datosDeError.error != null)
        {
            Debug.LogError($"Error {datosDeError.error.code} : {datosDeError.error.message}");
        }
        else
        {
            Debug.LogError("Error desconocido recibido al solicitar el audio.");
        }
    }
}
