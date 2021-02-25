using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAround : MonoBehaviour
{
    #region Public Variables
    [Space, Header("Mouse Settings")]
    public float mouseSens = 300f;
    public Transform playerRoot;
    #endregion

    #region Public Variables
    private float m_xRotate = 0f;
    #endregion

    #region Unity Callbacks
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSens * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSens * Time.deltaTime;

        m_xRotate -= mouseY;
        m_xRotate = Mathf.Clamp(m_xRotate, -90f, 90f);

        transform.localRotation = Quaternion.Euler(m_xRotate, 0f, 0f);

        playerRoot.Rotate(Vector3.up * mouseX);
    }
    #endregion
}