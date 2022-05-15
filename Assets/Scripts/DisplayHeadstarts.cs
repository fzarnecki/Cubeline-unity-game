using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayHeadstarts : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] Text headstartText;

    /***/

    // No real need for optimizing with coroutine since its only when game is not actively running (pause)
    void Update()
    {
        headstartText.text = gameManager.GetHeadstarts().ToString();
    }
}
