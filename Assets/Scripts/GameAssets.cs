using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    // Start is called before the first frame update
    private static GameAssets instance;

    public static GameAssets getInstance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    public Sprite obstacle;
    public Transform pfWall;
}
