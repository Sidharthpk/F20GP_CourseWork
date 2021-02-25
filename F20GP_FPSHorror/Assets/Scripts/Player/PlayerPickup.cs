using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    #region Public Variables
    [Space, Header("Pickup Object References")]
    public LayerMask examineLayer;
    [Tooltip("How Long do you want to ray to be")] public float rayDistance = 2f;
    [Tooltip("Radius of the sphere ray")] public float raySphereRadius = 0.1f;

    [Space, Header("Toy Spawner")]
    public Transform toySpawnPos;
    public Vector3 downscaleVal;

    public delegate void SendEvents();
    public static event SendEvents OnItemPicked;
    public static event SendEvents OnSetFlock;
    #endregion

    #region Private Variables
    private RaycastHit _hit;
    private bool _isInteracting;
    #endregion

    #region Unity Callbacks

    void Update()
    {
        PickupRaycast();
    }
    #endregion

    #region My Functions
    void PickupRaycast() // Creates new ray, then creates a sphere cast, added sphere case as it gives width to the ray;
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

        _isInteracting = Physics.SphereCast(ray, raySphereRadius, out _hit, rayDistance, examineLayer);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, _isInteracting ? Color.green : Color.red);

        if (_isInteracting) // Does a raycast bool check and if true and key E is presssed;
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OnItemPicked?.Invoke(); // Event sent to UIManagerScript;
                Destroy(_hit.collider.gameObject);
                //SpawnToy();
            }
        }
    }

    void SpawnToy()
    {
        GameObject toyObj = Instantiate(_hit.collider.gameObject, toySpawnPos.position, Quaternion.identity);
        toyObj.transform.localScale = downscaleVal;
        toyObj.layer = LayerMask.NameToLayer("Default");
        toyObj.tag = "Toys";

        toyObj.AddComponent<Rigidbody>();
        Rigidbody toyRg = toyObj.GetComponent<Rigidbody>();
        toyRg.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ;
        toyObj.AddComponent<Flocking>();
        OnSetFlock?.Invoke();
    }
    #endregion
}
