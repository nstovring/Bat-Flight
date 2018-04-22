using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidObject : MonoBehaviour
{
    [Header("Body Variables")]
    public float Mass;
    public bool AirResistance;
    public bool UseGravity;
    public float AirResistanceCoefficient = 0.7f;
    public bool ConnectedToSpring;
    public bool IsKinematic;
    public bool ResetPosition;

    [Header("Movement Vectors")]
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 acceleration;

    private float radius;
    [Header("Constant Forces")]
    //Gravity defined as -9.8 units in the y-axis
    public Vector3 gravity = new Vector3(0, -9.8f, 0);
    [Header("Forces")]
    public Vector3 gravityForce = Vector3.zero;
    public Vector3 dragForce = Vector3.zero;
    public Vector3 liftForce = Vector3.zero;
    public Vector3 thrustForce = Vector3.zero;
    public Vector3 netforce = Vector3.zero;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        position = transform.position;
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
        //Naive Radius calculation on the assumption that object is a sphere
        radius = transform.lossyScale.x / 2;
    }

    //Sum forces forces
    public void AddForce(Vector3 force)
    {
        netforce += force;
    }

    public virtual Vector3 ApplyForces()
    {
        //Calculate Acceleration
        acceleration = calculateAcceleration(netforce, Mass);

        //Apply acceleration to velocity and velocity to position multiplied by deltaTime
        velocity = velocity + acceleration * Time.deltaTime;
        position = position + velocity * Time.deltaTime;

        return position;
    }

    // Update is called once per frame
    void Update()
    {
        //If the object is kinematic apply no forces
        if (IsKinematic)
            return;

        //If Air Resistance is enabled call calculateAirResistanceForce, final variable is crossection calculated as the area of a circle
        if (AirResistance)
            AddForce(calculateAirResistanceForce(velocity, AirResistanceCoefficient, (Mathf.PI) * Mathf.Pow(radius, 2)));
        if (UseGravity)
            AddForce(calculateGravityForce(gravity, Mass));
        //Apply new position to object 
        transform.position = ApplyForces();
    }

    public void ResetForces()
    {
        //Reset all forces
        netforce = Vector3.zero;
        gravityForce = Vector3.zero;
        dragForce = Vector3.zero;
        liftForce = Vector3.zero;
        thrustForce = Vector3.zero;
    }

    private void LateUpdate()
    {
        ResetForces();
    }

    //Calculating the air resistance based on value of air density at 25 degrees celcius, otherwise using the drag equation
    public Vector3 calculateAirResistanceForce(Vector3 velocity, float dragCoeffecient, float crossAreal)
    {
        float airDensity = 1.1839f;
        Vector3 force = (0.5f) * airDensity * velocity.magnitude * dragCoeffecient * crossAreal * Vector3.Normalize(velocity);
        return force;
    }

    //public float GetDragCoefficient(Vector3 dragForce, float p, Vector3 flowSpeed, float area)
    //{
    //   // float cD = ((2 * dragForce) / (p * flowSpeed.sqrMagnitude * area));
    //   // return cD;
    //}
    //No built in vector pow function so defined one myself
    public Vector3 Pow(Vector3 v, int p)
    {
        v.x = Mathf.Pow(v.x, p);
        v.y = Mathf.Pow(v.y, p);
        v.z = Mathf.Pow(v.z, p);
        return v;
    }

    //Simply the defined gravity vector multiplied by mass
    protected Vector3 calculateGravityForce(Vector3 gravity, float mass)
    {
        return gravity * mass;
    }

    //The force divided by the mass
    protected Vector3 calculateAcceleration(Vector3 force, float mass)
    {
        return force / mass;
    }
}
