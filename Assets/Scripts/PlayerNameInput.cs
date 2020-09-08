﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameInputField = null; //Set the name
    [SerializeField] private Button continueButton = null;

    public static string DisplayName { get; private set; }

    private const string PlayerPrefsNameKey= "PlayerName";

    private void Start ()
    {
        SetUpInputField ();
    }

    private void SetUpInputField ()
    {
        if ( PlayerPrefs.HasKey ( PlayerPrefsNameKey ) )
        {
            string defaultName = PlayerPrefs.GetString ( PlayerPrefsNameKey );

            nameInputField.text = defaultName;

            SetPlayerName ( defaultName );
        }
    }
    public void SetPlayerName ( string name )
    {
        continueButton.interactable = !string.IsNullOrEmpty ( name ); 
    }

    public void SavePlayerName () {
        DisplayName = nameInputField.text;
        PlayerPrefs.SetString ( PlayerPrefsNameKey , DisplayName );
    }
}
