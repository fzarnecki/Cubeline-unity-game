using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [Tooltip("Background movement speed")]
    [SerializeField] float scrollSpeed = 0.1f;

    // Background material
    Material material;
    // Scroll speed will be transformed into vector2
    Vector2 offset;

    /***/

    void Start()
    {
        material = GetComponent<Renderer>().material;
        offset = new Vector2(scrollSpeed, 0);
    }
    
    void Update()
    {
        material.mainTextureOffset += offset * Time.deltaTime;
    }
}
