using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLoseText : MonoBehaviour
{
    // Not really a lose text only, but it's main purpose is there, therefore not changing the name of the script

    [Header("Text object to display text to")]
    [SerializeField] Text text;
    [SerializeField] TextsManager textsManager;

    /***/

    void Start()
    {
        if (text)
        {
            text.text = textsManager.GetRandomText();
        }
    }
}
