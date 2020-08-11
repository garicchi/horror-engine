using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KowaiKit
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class SearchArea : MonoBehaviour
    {
        // Start is called before the first frame update
        public event Action<Collider> OnDetect;
        public event Action<Collider> OnLost;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            Survivor s = null;
            if (other.gameObject.TryGetComponent(out s)) {
                OnDetect(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Survivor s = null;
            if (other.gameObject.TryGetComponent(out s)) {
                OnLost(other);
            }
        }
    }

}