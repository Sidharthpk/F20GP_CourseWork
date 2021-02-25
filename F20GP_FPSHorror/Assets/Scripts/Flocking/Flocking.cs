using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    #region Public Variables
    public float maxSpeed = 1;
    public float maxSpeedClamp = 1;
    public float seekWeight = 0.5f;
    public float alignWeight = 1f;
    public float separateWeight = 1f;
    public float cohesionWeight = 1f;
    #endregion

    #region Private Variables
    private Rigidbody _rg;
    private List<Rigidbody> _boids;
    private GameObject[] _noOfBoids;
    private GameObject _flockingTarget;
    #endregion

    #region Callbacks
    void Start()
    {
        _rg = GetComponent<Rigidbody>();
        _boids = new List<Rigidbody>();

        _flockingTarget = GameObject.FindGameObjectWithTag("Player");
        _noOfBoids = GameObject.FindGameObjectsWithTag("Toys");

        for (int i = 0; i < _noOfBoids.Length; i++)
        {
            Rigidbody rgBoid = _noOfBoids[i].GetComponent<Rigidbody>();
            _boids.Add(rgBoid);
        }
    }

    void Update()
    {
        CompiledAgents();
    }
    #endregion

    #region Function
    void CompiledAgents()
    {
        //This is where all the force gets applied.
        Vector3 seek = Seek(_flockingTarget.transform.position);
        Vector3 separate = Separation();
        Vector3 align = Alignment();
        Vector3 cohesion = Cohesion();

        _rg.AddForce(seek * seekWeight);
        _rg.AddForce(separate * separateWeight);
        _rg.AddForce(align * alignWeight);
        _rg.AddForce(cohesion * cohesionWeight);

        Vector3 target = _flockingTarget.transform.position;
        target.y = transform.position.y;
        transform.LookAt(target + _rg.velocity);
    }

    #region Seeking Behaviors
    Vector3 Seek(Vector3 target)
    {
        Vector3 desiredSeekVel = (target - transform.position).normalized * maxSpeed;
        Vector3 seekSteering = desiredSeekVel - _rg.velocity;
        Vector3 seekSteeringClamped = Vector3.ClampMagnitude(seekSteering, maxSpeedClamp);
        transform.LookAt(_flockingTarget.transform.position);
        return seekSteeringClamped;
    }

    Vector3 Cohesion()
    {
        float distanceFromNeighbour = 6;
        Vector3 totalCohesionDesiredVel = Vector3.zero;
        Vector3 cohesionDesiredVel = Vector3.zero;
        Vector3 sumOfPos = Vector3.zero;
        int count = 0;

        foreach (var other in _boids)
        {
            float distanceBetweenBoids = Vector3.Distance(transform.position, other.transform.position);

            if (distanceBetweenBoids > 0 && distanceBetweenBoids < distanceFromNeighbour)
            {
                sumOfPos += other.transform.position;
                count++;
            }
        }

        if (count > 0)
        {
            Vector3 avgPos = cohesionDesiredVel / _boids.Count;
            return Seek(avgPos);
            //return Seek(avgCohesionVel);
            //return to Seek function with the avgCohesionVel;
        }
        else
        {
            return Vector3.zero;
        }
    }

    Vector3 Separation()
    {
        float desiredSeperation = 10;
        Vector3 totalMoveAwayDesiredVel = Vector3.zero;
        Vector3 separationSteeringClamp = Vector3.zero;
        Vector3 moveAwayDesiredVel = Vector3.zero;
        int count = 0;

        foreach (var other in _boids)
        {
            float distanceBetweenBoids = Vector3.Distance(transform.position, other.transform.position);

            if (distanceBetweenBoids > 0 && distanceBetweenBoids < desiredSeperation)
            {
                moveAwayDesiredVel = (transform.position - other.transform.position).normalized;
                Vector3 divVel = moveAwayDesiredVel / distanceBetweenBoids;
                totalMoveAwayDesiredVel += divVel;
                count++;
            }
        }

        if (count > 0)
        {
            Vector3 divVel = (totalMoveAwayDesiredVel / count).normalized * maxSpeed;
            Vector3 seperationSteering = divVel - _rg.velocity;
            separationSteeringClamp = Vector3.ClampMagnitude(seperationSteering, maxSpeedClamp);
            //Subtract the setMag with velocity.
            //Clamp it.
            //return.
        }
        return separationSteeringClamp;
    }

    Vector3 Alignment()
    {
        float neighbourDistance = 30;
        Vector3 totalVector = Vector3.zero;
        int count = 0;

        foreach (var other in _boids)
        {
            float distanceBetweenBoids = Vector3.Distance(transform.position, other.transform.position);
            if (distanceBetweenBoids > 0 && distanceBetweenBoids < neighbourDistance)
            {
                totalVector = totalVector + other.velocity;
                count++;
            }
        }

        if (count > 0)
        {
            Vector3 avgVel = (totalVector / _boids.Count).normalized * maxSpeed;
            Vector3 steerAlign = avgVel - _rg.velocity;
            Vector3 steerAlignClamped = Vector3.ClampMagnitude(steerAlign, maxSpeedClamp);
            return steerAlignClamped;
            //Set Magnitude.
            //Subtract the setMag with velocity.
            //Clamp it.
            //Return Magnitude.
        }
        else
        {
            //return V3.zero.
            return Vector3.zero;
        }
    }
    #endregion

    #endregion
}
