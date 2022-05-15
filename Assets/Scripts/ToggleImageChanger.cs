using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleImageChanger : MonoBehaviour
{
    [Tooltip("Toggle object")]
    [SerializeField] private Toggle floatStayToggle;
    [Tooltip("Image object to display given sprite to")]
    [SerializeField] private Image toggleImage;
    [SerializeField] private Sprite toggleOnImage;
    [SerializeField] private Sprite toggleOffImage;

    [Header("Other working scripts")]
    [SerializeField] PlayerController player;

    /***/

    // Starting with toggle off every time
    void Start() { TurnToggleOff(); }


    public void PlayerStayFloat()
    {
        if (floatStayToggle.isOn)
        {
            toggleImage.sprite = toggleOnImage;
            player.TriggerFloatingState();
        }
        else
        {
            toggleImage.sprite = toggleOffImage;
            player.TriggerIdleState();
        }
    }

    public void SkinStaticMoving()
    {
        if (floatStayToggle.isOn)
        {
            toggleImage.sprite = toggleOnImage;
            player.SetIsSkinMoving(true);
        }
        else
        {
            toggleImage.sprite = toggleOffImage;
            player.SetIsSkinMoving(false);
        }
    }

    public void TurnToggleOff() { floatStayToggle.isOn = false; PlayerStayFloat(); }
}
