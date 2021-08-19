using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour 
{
    BoidConfiguration config;

    Vector3 velocity;
    [HideInInspector]
    public Vector3 position;
    [HideInInspector]
    public Vector3 forward;
    
    Vector3 acceleration;
    [HideInInspector]
    public Vector3 groupCenter;
    [HideInInspector]
    public int numOthers;
    [HideInInspector]
    public Vector3 avgHeading;
    [HideInInspector]
    public Vector3 avgAvoidHeading;
    
    public Transform target;
    public Transform cachedTransform;
    float maxSpeed;
    float minSpeed;
    public bool isObjective;

    public void changeVelocity (float delta)
    {
        maxSpeed += delta;
        minSpeed += delta;
    }

    public void Initialize (BoidConfiguration config, Transform target, Object material) 
    {
        maxSpeed = config.maxSpeed; 
        minSpeed = config.minSpeed;
        this.config = config;
        this.target = target;
        if (material != null)
        {
            isObjective = false;
            transform.GetComponentInChildren<SkinnedMeshRenderer>().material = (Material) material;
        }
        else
        {
            isObjective = true;
        }
        float startSpeed = (minSpeed + maxSpeed) / 2;
        velocity = transform.forward * startSpeed;

        position = cachedTransform.position;
        forward = cachedTransform.forward;
    }

    void Awake() 
    {
        cachedTransform = transform;
    }

    bool nextToCollision() 
    {
        RaycastHit hit;
        return Physics.SphereCast(position, config.boundsRadius, forward, out hit, config.coAvoidDst, config.obsMask);
    }

    Vector3 SteerTowards (Vector3 vector) 
    {
        Vector3 v = vector.normalized * config.maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, config.maxSteerForce);
    }
    
    Vector3 ObstacleRays () 
    {
        Vector3[] rayDir = BoidMath.directions;

        for (int i = 0; i < rayDir.Length; i++) 
        {
            Vector3 dir = cachedTransform.TransformDirection(rayDir[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, config.boundsRadius, config.coAvoidDst, config.obsMask)) 
            {
                return dir;
            }
        }
        return forward;
    }

    public void StopAnimation ()
    {
        gameObject.GetComponent<Animator>().enabled = false;
    }

    public void UpdateBoid () 
    {
        gameObject.GetComponent<Animator>().enabled = true;
        Vector3 acceleration = Vector3.zero;

        if (target != null) 
        {
            Vector3 tgOffset = (target.position - position);
            acceleration = SteerTowards(tgOffset) * config.tgWeight;
        }

        if (numOthers > 0) 
        {
            groupCenter /= numOthers;

            Vector3 groupCenterOffset = (groupCenter - position);

            var aligForce = SteerTowards(avgHeading) * config.aligWeight;
            var coForce = SteerTowards(groupCenterOffset) * config.coWeight;
            var sepForce = SteerTowards(avgAvoidHeading) * config.sepWeight;

            acceleration += aligForce;
            acceleration += coForce;
            acceleration += sepForce;
        }

        if (nextToCollision ()) {
            Vector3 coAvoidDir = ObstacleRays();
            Vector3 coAvoidForce = SteerTowards(coAvoidDir) * config.avoidCoWeight;
            acceleration += coAvoidForce;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
        forward = dir;
    }

}