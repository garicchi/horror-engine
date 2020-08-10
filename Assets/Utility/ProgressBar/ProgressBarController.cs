using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarController : MonoBehaviour
{
    [SerializeField] public Image ImageFront;

    [SerializeField] public Image ImageBack;

    [Range(0f, 1f)] public float Progress;
    
    // Start is called before the first frame update
    void Start()
    {
        Progress = 1f;
        ImageFront.fillAmount = Progress;
    }

    // Update is called once per frame
    void Update()
    {
        ImageFront.fillAmount = Progress;
    }
}
