using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace KowaiKit
{
    [RequireComponent(typeof(CharacterController))]
    public class Survivor : MonoBehaviour
    {
        [SerializeField] public float SpeedMove = 8.0f;
        [SerializeField] public float SpeedRotate = 120.0f;
        [SerializeField] public int CurrentItemCount = 0;
        [SerializeField] public AudioClip SeWalk;
        [SerializeField] public AudioClip SeScream;
        [NonSerialized] public FlashLight HandLight;
        
        private float _cameraVerticalAngle = 0f;
        private CharacterController _characterController;
        private Camera _faceCamera;
        private AudioSource _audioSourceWalk;
        private AudioSource _audioSourceScream;

        public event Action<int> OnGetItemEvent;

        private void Awake()
        {
            HandLight = transform.Find("HandLight").GetComponent<FlashLight>();
            _faceCamera = transform.Find("FaceCamera").GetComponent<Camera>();
            _audioSourceWalk = transform.Find("SoundWalk").GetComponent<AudioSource>();
            _audioSourceScream = transform.Find("SoundScream").GetComponent<AudioSource>();
        }

        void Start()
        {
            _characterController = GetComponent<CharacterController>();
            
            _audioSourceWalk.clip = SeWalk;
            
            _audioSourceScream.clip = SeScream;
        }

        // Update is called once per frame
        void Update()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Vector3 forward = transform.forward * v * SpeedMove * Time.deltaTime;
            Vector3 right = transform.right * h * SpeedMove * Time.deltaTime;

            Vector3 moveVec = forward + right;
            if (moveVec.magnitude > 0)
            {
                if (!_audioSourceWalk.isPlaying)
                {
                    _audioSourceWalk.Play();
                }
            }
            else
            {
                if (_audioSourceWalk.isPlaying)
                {
                    _audioSourceWalk.Stop();
                }
            }

            _characterController.Move(moveVec);



            float rotateHorizontal = Input.GetAxis("Mouse X") * SpeedRotate * Time.deltaTime;
            float rotateVertical = -1f * Input.GetAxis("Mouse Y") * SpeedRotate * Time.deltaTime;
            transform.Rotate(Vector3.up, rotateHorizontal);
            _cameraVerticalAngle += rotateVertical;
            _cameraVerticalAngle = Mathf.Clamp(_cameraVerticalAngle, -80f, 80f);
            _faceCamera.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0, 0);
            HandLight.transform.localEulerAngles = new Vector3(_cameraVerticalAngle, 0, 0);

            if (Input.GetMouseButtonDown(1))
            {
                HandLight.ToggleSwitch();
            }

            if (Input.GetMouseButtonDown(0))
            {
                var ray = _faceCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                var hit = new RaycastHit();
                if (Physics.Raycast(ray, out hit))
                {
                    var hitObj = hit.collider.gameObject;
                    MissionItem item; 
                    if (hitObj.TryGetComponent(out item))
                    {
                        hitObj.SetActive(false);
                        CurrentItemCount++;
                        OnGetItemEvent(CurrentItemCount);
                    }

                    Battery battery;
                    if (hitObj.TryGetComponent(out battery))
                    {
                        hitObj.SetActive(false);
                        HandLight.Charge(20);
                    }
                }
            }
        }

        public void Scream()
        {
            _audioSourceScream.loop = false;
            _audioSourceScream.Play();
        }
    }
}