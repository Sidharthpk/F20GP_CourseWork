using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Public Variables
    [Space, Header("Player Variables")]
    public float playerWalkSpeed = 2f;
    public float playerRunSpeed = 4f;
    public float gravity = -9.81f;

    [Space, Header("Flashlight")]
    public GameObject flashLight;

    [Space, Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    #endregion

    #region Private Variables
    [Header("Player Variables")]
    private CharacterController _charControl;
    private Vector3 _vel;
    private float _currSpeed;

    [Header("Ground Check")]
    private bool _isGrounded;
    #endregion

    #region Unity Callbacks

    #region Intialization and Loops
    void Start()
    {
        _charControl = GetComponent<CharacterController>();
    }

    void Update()
    {
        PlayerMovement();
        FlashLight();
        GroundCheck();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MouseTrigger"))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MouseTrigger"))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    #endregion

    #endregion

    #region My Functions

    #region Player
    void GroundCheck() // Doing ground check to see if the player is grounded our not, else add gravity;
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (_isGrounded & _vel.y < 0)
            _vel.y = -2f;

        _vel.y += gravity * Time.deltaTime;
        _charControl.Move(_vel * Time.deltaTime);
    }

    void PlayerMovement() // Player Movement with Character Controller;
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (Input.GetKey(KeyCode.LeftShift))
            _currSpeed = playerRunSpeed;
        else
            _currSpeed = playerWalkSpeed;

        Vector3 moveDirection = (transform.right * horizontal + transform.forward * vertical).normalized;
        _charControl.Move(moveDirection * _currSpeed * Time.deltaTime);
    }
    #endregion

    #region Tools
    void FlashLight() // Flashlight Toggle;
    {
        if (Input.GetKeyDown(KeyCode.F))
            flashLight.SetActive(!flashLight.activeSelf);
    }
    #endregion

    #endregion
}