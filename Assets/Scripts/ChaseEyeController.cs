using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseEyeController : MonoBehaviour
{
    // Start is called before the first frame update
    public Action<Collider> OnDetect;
    public Action<Collider> OnLost;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        OnDetect?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnLost?.Invoke(other);
    }
}
