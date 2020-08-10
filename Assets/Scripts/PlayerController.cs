using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PlayerController : MonoBehaviour
{
    private float speedMove = 13.0f;
    private float speedRotateHorizontal = 120.0f;
    private float speedRotateVertical = 120.0f;

    private CharacterController _characterController;
    private float cameraVerticalAngle = 0f;
    
    [SerializeField] public Camera FaceCamera;

    [SerializeField] public GameObject FlashLight;
    private FlashLightController _flashLightController;
    [SerializeField] public GameObject AudioWalk;
    private AudioSource _audioSourceWalk;
    
    private int _itemCount;

    public event Action<int> GetItemEvent;
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _flashLightController = FlashLight.GetComponent<FlashLightController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _itemCount = 0;
        //_flashLightController.ToggleSwitch();
        _audioSourceWalk = AudioWalk.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 forward = transform.forward * v * speedMove * Time.deltaTime;
        Vector3 right = transform.right * h * speedMove * Time.deltaTime;

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
        
        

        float rotateHorizontal = Input.GetAxis("Mouse X") * speedRotateHorizontal * Time.deltaTime;
        float rotateVertical = -1f * Input.GetAxis("Mouse Y") * speedRotateVertical * Time.deltaTime;
        transform.Rotate(Vector3.up, rotateHorizontal);
        cameraVerticalAngle += rotateVertical;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -80f, 80f);
        FaceCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);

        if (Input.GetMouseButtonDown(1))
        {
            _flashLightController.ToggleSwitch();
        }

        if (Input.GetMouseButtonDown(0))
        {
            var ray = FaceCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            var hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                var hitObj = hit.collider.gameObject;
                if (hitObj.CompareTag("item"))
                {
                    hit.collider.gameObject.SetActive(false);
                    _itemCount++;
                    GetItemEvent(_itemCount);
                }

                if (hitObj.CompareTag("battery"))
                {
                    hit.collider.gameObject.SetActive(false);
                    _flashLightController.Charge(20);
                }
            }
        }

    }
}
