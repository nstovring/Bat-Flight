using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerodynamicBody : RigidObject {

    [Header("Aerodynamics Variables")]
    public Transform wings;
    public float wingArea;
    public float angleOfAttack;
    public float zeroLiftAngle;
    public float liftCoefficient;
    public Vector3 windDirection = -Vector3.forward;
    public Vector3 momentum;
    public float thrustScale = 4;
    float GetCuboidArea(Vector3 scale)
    {
        return (scale.x * scale.z + scale.y * scale.z + scale.y * scale.x) * 2;

    }

	// Use this for initialization
	void Start () {
        wingArea = GetCuboidArea(wings.localScale);
        zeroLiftAngle = Vector3.SignedAngle(transform.forward, Vector3.up, transform.right);

        Initialize();
    }
	
	// Update is called once per frame
	void Update () {
        if (IsKinematic)
            return;
        ResetForces();

        //Wind direction equal to the opposite direction of the velocity
        windDirection = -velocity;

        //Thrust force is the forward vector times a scalar (thrust) divided by mass
        thrustForce = (transform.forward.normalized * thrustScale)/Mass;

        //angle of attack is the zerolift angle subtracted by the singedangle between the forward vector and the upward facing vector
        //around the right axis (X axis).
        angleOfAttack = (zeroLiftAngle - Vector3.SignedAngle(transform.forward, Vector3.up , transform.right));

        //Lift ceofficient calculated by the wingarea and angleofattack
        liftCoefficient = 2 * wingArea * (angleOfAttack / 180);

        //drag force opposite velocity (as the force) 0.9 is the drag coefficent, lasty the wingarea
        dragForce = calculateAirResistanceForce(-velocity, 0.9f, wingArea);

        //lift force calculated by the vector "attacking" the wings (cross by velocity and right vector) lift coefficient and wingarea.
        liftForce = calculateAirResistanceForce(Vector3.Cross(transform.right,velocity), liftCoefficient, wingArea) ;

        //Gravity
        gravityForce = calculateGravityForce(gravity, Mass);

        //Add forces to the plane
        AddForce(gravityForce);
        AddForce(dragForce);
        AddForce(liftForce);
        AddForce(thrustForce);

        //Draw the vectors affecting the plane
        Debug.DrawRay(transform.position + transform.right * 0.1f, liftForce, Color.red);
        Debug.DrawRay(transform.position, dragForce - transform.right * 0.1f, Color.green);
        Debug.DrawRay(transform.position, gravityForce, Color.blue);
        Debug.DrawRay(transform.position, thrustForce, Color.yellow);
        Debug.DrawRay(transform.position, netforce, Color.white);
        Debug.DrawRay(transform.position, velocity, Color.cyan);

        //Applying the transform to the plane
        Vector3 position = ApplyForces();
        transform.position = position;

        //Input from the user
        float HSpeed = Input.GetAxis("Horizontal");
        float VSpeed = Input.GetAxis("Vertical");

        //Performs rotation from the input
        Vector3 rotation = transform.rotation.eulerAngles;
        Quaternion qRotation =  Quaternion.Euler(new Vector3(VSpeed, 0, -HSpeed));
        transform.rotation *= qRotation;
    }
}
