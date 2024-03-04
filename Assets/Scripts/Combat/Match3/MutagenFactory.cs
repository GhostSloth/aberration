using GhostSloth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutagenFactory : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;

    [SerializeField]
    private MutagenGameObject prefab;

    public MutagenGameObject GetMutagen(MutagenType type)
    {
        var mutagen = Instantiate(prefab, transform);

        mutagen.SetSprite(sprites[(int)type - 1]);

        return mutagen;
    }
}
