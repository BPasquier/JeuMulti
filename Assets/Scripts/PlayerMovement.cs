using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    private Vector3 camRotation;
    public Transform cam;
    private Vector3 moveDirection;
    private bool menu;

    [Range(-80, -15)]
    public int minAngle = -80;
    [Range(30, 80)]
    public int maxAngle = 80;
    [Range(50, 500)]
    public int sensitivity = 200;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menu = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu == false)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                menu = true;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                menu = false;
            }
        }
        if (menu == false)
            Rotate();
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up * sensitivity * Time.deltaTime * Input.GetAxis("Mouse X"));

        camRotation.x -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        camRotation.x = Mathf.Clamp(camRotation.x, minAngle, maxAngle);

        cam.localEulerAngles = camRotation;
    }
}