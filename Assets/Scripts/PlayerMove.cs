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
    public int playerHealth = 10;

    public GameObject bodySpin;
    public int Seeker; // Seeker =-1 Pas choisi / =0 Chass� / =1 Chasseur
    [SerializeField] private GameObject BodySeeker;
    [SerializeField] private GameObject BodyHider;
    [SerializeField] private GameObject Crosshair_Hider;
    [SerializeField] private GameObject Crosshair_Seeker;
    [SerializeField] private GameObject MenuCYD;


    public NetworkVariable<Vector3> Position = new(writePerm: NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> Rotation = new(writePerm: NetworkVariableWritePermission.Owner);
    /*public NetworkVariable<Vector2> ID_Morph = new(writePerm: NetworkVariableWritePermission.Owner); // [ID_Joueur][ID_Obj_Transform]
    public NetworkVariable<bool> Seeker_sync = new(writePerm: NetworkVariableWritePermission.Owner);*/

    //
    [SerializeField]
    private float m_speed;

    [SerializeField]
    Canvas deathCanvas;

    private void Awake()
    {
        menu = true;
        Seeker = -1;

    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(MovePlayer());
        animator = GetComponent<Animator>();
        SupprCameraMenu();
        InitMorph();
        BodySeeker.SetActive(false);
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
                if (Seeker >= 0)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    menu = false;
                }
                else
                {

                }
            }
        }
        if (menu == false)
        {
            if (IsOwner)
            {
                if (playerHealth > 0)
                {
                    Rotate();
                    Move();
                }
            }
            synchro2();
        }
    }

    private void synchro2()
    {
        if (IsOwner)
        {
            Position.Value = transform.position;
            Rotation.Value = transform.rotation;
        }
        else
        {
            transform.position = Position.Value;
            transform.rotation = Rotation.Value;
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
                if (possibleObject.name == "BodySeeker" || possibleObject.name == "BodyHider")
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
        if (Input.GetKeyDown(KeyCode.Space) == true && (gameObject.GetComponent<Rigidbody>().velocity.y > -.01 && gameObject.GetComponent<Rigidbody>().velocity.y < .01))
        {
            gameObject.GetComponent<Rigidbody>().velocity += Vector3.up*5;
        }

        if (Input.GetKey(KeyCode.E) == true)
        {
            if (Seeker == 0)
            {
                if (IsServer)
                    Morph();
                else
                    SubmitRequestMorphServerRpc();
            }
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
                        transform.position = transform.position + new Vector3(0f,.2f,0f);
                        cam.transform.SetParent(possibleObject.transform);
                    }
                    else
                    {
                        possibleObject.gameObject.SetActive(false);
                    }
                }
            }
        }
    }
    
    public void ChooseSeeker()
    {
        Seeker = 1;
        BodySeeker.SetActive(true);
        MenuCYD.SetActive(false);
        Crosshair_Seeker.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menu = false;
    }

    public void ChooseHider()
    {
        Seeker = 0;
        BodyHider.SetActive(true);
        MenuCYD.SetActive(false);
        Crosshair_Hider.SetActive(true);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menu = false;
    }

    public void dealDamage(int damage)
    {
        playerHealth -= damage;
        if (playerHealth<=0)
        {
            UnityEngine.Debug.LogWarning("DealDamages");
            deathCanvas.gameObject.SetActive(true);
        }
    }

    [ServerRpc]
    private void SubmitRequestMorphServerRpc(ServerRpcParams rpcParams = default)
    {
        Morph();
    }
}
