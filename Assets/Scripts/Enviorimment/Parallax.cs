using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Vector2 parallaxEffect;
    [SerializeField] private bool infiniteHorizontal;
    [SerializeField] private bool infiniteVertical;
    //
    private Transform cameraTransform;
    private Vector3 lastCameraPos;
    private float textureX;
    private float textureY;
    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPos = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureX = texture.width / sprite.pixelsPerUnit;
        textureY = texture.height / sprite.pixelsPerUnit;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPos;
        transform.position += new Vector3(deltaMovement.x * parallaxEffect.x, deltaMovement.y * parallaxEffect.y);
        lastCameraPos = cameraTransform.position;

        if (infiniteHorizontal)
        {
            if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureX)
            {
                float offsetPosX = (cameraTransform.position.x - transform.position.x) % textureX;
                transform.position = new Vector3(cameraTransform.position.x + offsetPosX, transform.position.y);
            }
        }
        if(infiniteVertical)
        {
            if (Mathf.Abs(cameraTransform.position.y - transform.position.y) >= textureY)
            {
                float offsetPosY = (cameraTransform.position.y - transform.position.y) % textureY;
                transform.position = new Vector3(transform.position.x , offsetPosY+ cameraTransform.position.y);
            }
        }
    }
}
