using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXPlayer : MonoBehaviour
{
    [SerializeField] GameObject worldChangeVFX;
    [SerializeField] GameObject respawnVFX;
    [SerializeField] GameObject[] stompParticles;

    /***/

    public void PlayRespawnVFX(Transform trans)
    {
        GameObject respVFX = Instantiate(respawnVFX, trans.position + new Vector3(0,1,0), Quaternion.identity);
        Destroy(respVFX, 2);
    }

    public void PlayWorldChangeVFX(Transform trans)
    {
        GameObject changeVFX = Instantiate(worldChangeVFX, trans.position, Quaternion.identity);
        Destroy(changeVFX, 2);
    }

    public void PlayStompParticles(Transform trans)
    {
        GameObject stompParts = Instantiate(stompParticles[GameManager.currentWorld], trans.position, Quaternion.Euler(0, -90, 0));
        Destroy(stompParts, 2);
    }
}
