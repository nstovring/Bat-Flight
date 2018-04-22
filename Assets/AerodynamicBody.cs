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

    //Vector3 GetAerodynamicForce()
    //{
    //    Vector3 dynamicPressure = (airDensity * Pow(airVelocity)) / 2;
    //    Vector3 aerodynamicFoce = (dynamicPressure / 2) * wingArea;
    //    return Vector3.zero;
    //}


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

        //windDirection = velocity.normalized;

        windDirection = -velocity;
        thrustForce = (transform.forward.normalized * thrustScale)/Mass;
        angleOfAttack = (zeroLiftAngle - Vector3.SignedAngle(transform.forward, Vector3.up , transform.right));
        liftCoefficient = 2 * wingArea * (angleOfAttack / 180);
        dragForce = calculateAirResistanceForce(-velocity, 0.9f, wingArea);
        liftForce = calculateAirResistanceForce(Vector3.Cross(transform.right,velocity), liftCoefficient, wingArea) ;
        gravityForce = calculateGravityForce(gravity, Mass);

      

        //Vector3 direction = Vector3.zero - transform.position;
        //AddForce(direction.normalized);

        AddForce(gravityForce);
        AddForce(dragForce);
        AddForce(liftForce);
        AddForce(thrustForce);

        Quaternion lookDirection = Quaternion.LookRotation(netforce);

        
        //velocity += thrustForce * Time.deltaTime;
        //Debug.DrawRay(transform.position, Vector3.zero - transform.position, Color.gray);


        Debug.DrawRay(transform.position + transform.right * 0.1f, liftForce, Color.red);
        Debug.DrawRay(transform.position, dragForce - transform.right * 0.1f, Color.green);
        Debug.DrawRay(transform.position, gravityForce, Color.blue);
        Debug.DrawRay(transform.position, thrustForce, Color.yellow);

        Debug.DrawRay(transform.position, netforce, Color.white);
        Debug.DrawRay(transform.position, velocity, Color.cyan);

        Vector3 position = ApplyForces();
        transform.position = position;

        //transform.rotation = Quaternion.Slerp(transform.rotation, lookDirection, 0.001f);
    }

    private void LateUpdate()
    {
       
    }
}
