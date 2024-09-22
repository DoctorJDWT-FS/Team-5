using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class difficultyStorage : MonoBehaviour
{
    [SerializeField, Range(1, 3)] public int difficulty;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
