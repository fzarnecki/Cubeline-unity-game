using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchManager : MonoBehaviour
{
    [Tooltip("Height of the top border where the pause button is")]
    [SerializeField] private float topBorder = 150f;

    [Header("Other working scripts")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private WorldsManager worldsManager;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameManager gameManager;

    /***/

    void Update()
    {
        if (gameCanvas.active == false) return;

        if (Input.touchCount > 0)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;

            var touch = Input.GetTouch(0);
            if (touch.position.x < Screen.width / 2)
            {
                worldsManager.ExchangeWorlds();
            }
            else if (touch.position.x > Screen.width / 2 && touch.position.y < Screen.height - topBorder)
            {
                playerController.OnRightScreenPress();
            }
        }
    }
}
