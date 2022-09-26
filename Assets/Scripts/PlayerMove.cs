using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using Unity.Netcode;

public class PlayerMove : MonoBehaviour
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
    Vector3 pos;
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
        pos = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform.position;
        animator = GetComponent<Animator>();

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
            Rotate();

        var playerObject = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        var player = playerObject.GetComponent<SyncroObjects>();
        pos = playerObject.transform.position;

        animator.SetFloat("Speed", 0);
        if (Input.GetKey(KeyCode.Z) == true)
        {
            pos = pos + transform.forward * m_speed * Time.deltaTime;
            animator.SetFloat("Speed", m_speed);
        }
        if (Input.GetKey(KeyCode.S) == true)
        {
            pos = pos + transform.forward * -m_speed * Time.deltaTime;
            animator.SetFloat("Speed", m_speed);
        }
        if (Input.GetKey(KeyCode.D) == true)
        {
            pos = pos + transform.right * m_speed * Time.deltaTime;
            animator.SetFloat("Speed", m_speed);
        }
        if (Input.GetKey(KeyCode.Q) == true)
        {
            pos = pos + transform.right * -m_speed * Time.deltaTime;
            animator.SetFloat("Speed", m_speed);
        }
                
        if (Input.GetKey(KeyCode.E) == true)
        {
            Morph();
        }

        Aim();
        /*if (Input.GetKeyDown(KeyCode.UpArrow) == true)
            pos = playerObject.transform.position + new Vector3(0, 0, 1f);
        if (Input.GetKeyDown(KeyCode.DownArrow) == true)
            pos = playerObject.transform.position + new Vector3(0, 0, -1f);
        if (Input.GetKeyDown(KeyCode.RightArrow) == true)
            pos = playerObject.transform.position + new Vector3(1f, 0, 0);
        if (Input.GetKeyDown(KeyCode.LeftArrow) == true)
            pos = playerObject.transform.position + new Vector3(-1f, 0, 0);*/

        playerObject.transform.position = pos;
        player.Move(pos);
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

    /*IEnumerator MovePlayer()
    {
        while(true)
        {
            NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<SyncroObjects>().Move(pos);
            yield return new WaitForSeconds(0.001f);
        }
    }*/
}
