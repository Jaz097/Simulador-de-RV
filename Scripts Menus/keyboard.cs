using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;

public class keyboard : MonoBehaviour
{
    private TMP_InputField input;
    public float distancia = 0.5f;
    public float vertOffset = -0.5f;


    void Start()
    {
        input = GetComponent<TMP_InputField>();
        input.onSelect.AddListener(x => Abrirkeyboard());

    }
    public void Abrirkeyboard()
    {
        NonNativeKeyboard.Instance.InputField = input;
        
        NonNativeKeyboard.Instance.PresentKeyboard(input.text);
    }


}