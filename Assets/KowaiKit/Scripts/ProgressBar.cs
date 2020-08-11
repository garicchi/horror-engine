using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace KowaiKit
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] public Image ImageBar;

        [Range(0f, 1f)] public float Progress;

        // Start is called before the first frame update
        void Start()
        {
            Progress = 1f;
            ImageBar.fillAmount = Progress;
        }

        // Update is called once per frame
        void Update()
        {
            ImageBar.fillAmount = Progress;
        }
    }
}