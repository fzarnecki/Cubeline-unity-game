using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InGameNotifications : MonoBehaviour
{
    private bool canDisplay = true;
    [SerializeField] private float timeBetweenDisplays = 4f;
    [SerializeField] private float destroyDelay = 4f;
    // [SerializeField] private float timeBetweenImageAndText = 6f;

    [SerializeField] private GameObject speedUpNotif;
    [SerializeField] private GameObject obstacleUpgradeNotif;
    // [SerializeField] private TextMeshProUGUI textNotifPrefab;

    // [SerializeField] private TextsManager textsManager;

    /***/

    public IEnumerator DisplaySpeedUp()
    {
        if (CanDisplay())
        {
            SetCanDisplay(false);
            speedUpNotif.SetActive(true);
            speedUpNotif.GetComponent<Animator>().SetTrigger("Entry");
        }

        yield return new WaitForSeconds(timeBetweenDisplays);   // destroy delay happens to == time between displays, so no need to additional waiting
        speedUpNotif.SetActive(false);
        SetCanDisplay(true);
    }

    public IEnumerator DisplayObstacleUpgrade()
    {
        if (CanDisplay())
        {
            SetCanDisplay(false);
            obstacleUpgradeNotif.SetActive(true);
            obstacleUpgradeNotif.GetComponent<Animator>().SetTrigger("Entry");
        }

        yield return new WaitForSeconds(timeBetweenDisplays);   // destroy delay happens to == time between displays, so no need to additional waiting
        obstacleUpgradeNotif.SetActive(false);
        SetCanDisplay(true);
    }

    public void SetCanDisplay(bool b) { canDisplay = b; }
    public bool CanDisplay() { return canDisplay; }

    /***/

    /*public IEnumerator DisplayRandomTextNotif()
    {
        yield return new WaitForSeconds(timeBetweenImageAndText);

        if (CanDisplay())
        {
            SetCanDisplay(false);

            string text = textsManager.GetRandomText();
            TextMeshProUGUI t = Instantiate(textNotifPrefab, transform);
            t.gameObject.SetActive(true);
            t.text = text;
            Destroy(t.gameObject, destroyDelay);
        }

        yield return new WaitForSeconds(timeBetweenDisplays);
        SetCanDisplay(true);
    }*/

}
