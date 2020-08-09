using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speedMove = 13.0f;
    private float speedRotateHorizontal = 20.0f;
    private float speedRotateVertical = 20.0f;

    private CharacterController _characterController;
    private float cameraVerticalAngle = 0f;
    
    public Camera faceCamera;
    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 forward = transform.forward * v * speedMove * Time.deltaTime;
        Vector3 right = transform.right * h * speedMove * Time.deltaTime;
        
        _characterController.Move(forward + right);
        

        float rotateHorizontal = Input.GetAxis("Mouse X") * speedRotateHorizontal * Time.deltaTime;
        float rotateVertical = -1f * Input.GetAxis("Mouse Y") * speedRotateVertical * Time.deltaTime;
        transform.Rotate(Vector3.up, rotateHorizontal);
        cameraVerticalAngle += rotateVertical;
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -80f, 80f);
        faceCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);

    }
}
