using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutagenGameObject : MonoBehaviour
{
    private SpriteRenderer sr;

    public void SetSprite(Sprite sprite)
    {
        sr.sprite = sprite;
    }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
}
