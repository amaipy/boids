using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoidConfiguration : ScriptableObject 
{
    public float aligWeight = 1;
    public float coWeight = 1;
    public float sepWeight = 1;
    public float tgWeight = 1;

    public float minSpeed = 2;
    public float maxSpeed = 5;

    public float percepRadius = 2.5f;
    public float avoidRadius = 1;
    public float maxSteerForce = 3;

    [Header ("Collisions")]
    public LayerMask obsMask;
    public float boundsRadius = 1.3f;
    public float avoidCoWeight = 10;
    public float coAvoidDst = 5;
}