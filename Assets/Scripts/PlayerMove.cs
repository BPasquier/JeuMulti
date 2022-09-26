using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Unity.Netcode;

public class PlayerMove : NetworkBehaviour
{
    // Movement Rotation
    private Vector3 camRotation;
    public Transform cam;
    private Vector3 moveDirection;
    private bool menu;
    private Animator animator;
    Transform[] listTransform;
    [Range(-80, -15)]
    public int minAngle = -80;
    [Range(30, 80)]
    public int maxAngle = 80;
    [Range(50, 500)]
    public int sensitivity = 200;

    public GameObject bodySpin;

    //
    [SerializeField]
    private float m_speed;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menu = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(MovePlayer());
        animator = GetComponent<Animator>();
        SupprCameraMenu();
        InitMorph();
    }

    // Update is called once per frame
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
        {
            if (IsOwner)
            {
                Rotate();
                Move();
            }
            NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<SyncroObjects>().synchro();
        }
    }

    private void SupprCameraMenu()
    {
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("CameraMenu");
        foreach (GameObject tmpCam in tmp)
            tmpCam.SetActive(false);
    }

    private void InitMorph()
    {
        listTransform = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform possibleObject in listTransform)
        {
            if (possibleObject.tag == "PossibleForm")
            {
                if (possibleObject.name == "Body")
                {
                    possibleObject.gameObject.SetActive(true);
                }
                else
                {
                    possibleObject.gameObject.SetActive(false);
                }
            }
        }
    }

    private void Move()
    {
        var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        var player = playerObject.GetComponent<SyncroObjects>();

        animator.SetFloat("Speed", 0);
        if (Input.GetKey(KeyCode.Z) == true)
        {
            playerObject.transform.position = playerObject.transform.position + transform.forward * m_speed * Time.deltaTime;
            animator.SetFloat("Speed", m_speed);
        }
        if (Input.GetKey(KeyCode.S) == true)
        {
            playerObject.transform.position = playerObject.transform.position + transform.forward * -m_speed * Time.deltaTime;
            animator.SetFloat("Speed", m_speed);
        }
        if (Input.GetKey(KeyCode.D) == true)
        {
            playerObject.transform.position = playerObject.transform.position + transform.right * m_speed * Time.deltaTime;
            animator.SetFloat("Speed", m_speed);
        }
        if (Input.GetKey(KeyCode.Q) == true)
        {
            playerObject.transform.position = playerObject.transform.position + transform.right * -m_speed * Time.deltaTime;
            animator.SetFloat("Speed", m_speed);
        }

        if (Input.GetKey(KeyCode.E) == true)
        {
            Morph();
        }
        Aim();
    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up * sensitivity * Time.deltaTime * Input.GetAxis("Mouse X"));

        camRotation.x -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        camRotation.x = Mathf.Clamp(camRotation.x, minAngle, maxAngle);

        bodySpin.transform.localEulerAngles = camRotation;
        cam.localEulerAngles = camRotation;
    }

    private void Aim()
    {
        RaycastHit hit;
        Physics.Raycast(cam.transform.position, cam.transform.forward, out hit);
    }

    private void Morph()
    {
        RaycastHit hit;
        Physics.Raycast(cam.transform.position, cam.transform.forward, out hit);

        if (hit.transform.gameObject.tag == "MorphableObject")
        {
            foreach (Transform possibleObject in listTransform)
            {
                if (possibleObject.tag == "PossibleForm")
                {
                    if (hit.transform.name.IndexOf(possibleObject.name)>-1)
                    {
                        possibleObject.gameObject.SetActive(true);
                    }
                    else
                    {
                        possibleObject.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}
