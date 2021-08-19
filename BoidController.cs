using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BoidController : MonoBehaviour 
{
    const int threadGroupSize = 1024;

    public BoidConfiguration config;
    public ComputeShader compute;
    Boid[] boids;
    Object[] materials;
    public Vector3 avgCenter;
    Transform objBoidPosition;
    bool freezeState = false;


    public struct BoidData 
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 groupHeading;
        public Vector3 groupCenter;
        public Vector3 avoidHeading;
        public int numOthers;

        public static int Size 
        {
            get 
            {
                return (sizeof(float) * 3 * 5) + sizeof(int);
            }
        }
    }

    public void changeFreezeState()
    {
        freezeState = !freezeState;
    }

    public void updateBoidList()
    {
        boids = FindObjectsOfType<Boid>();
    }

    public void initializeBoid(Boid boid)
    {
        int materialIndex = Random.Range(0, materials.Length);
        boid.Initialize(config, objBoidPosition, materials[materialIndex]);
    }

    void Start () 
    {
        materials = Resources.LoadAll("MyMaterials", typeof(Material));
        GameObject objBoid = GameObject.FindGameObjectWithTag("Player");
        boids = FindObjectsOfType<Boid>();
        objBoidPosition = objBoid.GetComponent<Transform>();
        Boid obj = objBoid.GetComponent<Boid>();
        foreach (Boid b in boids) 
        {
            if (b != obj)
            {
                initializeBoid(b);
            }
            else
            {
                b.Initialize(config, null, null);    
            }
        }
    }

    void Update () 
    {
        if (boids != null) 
        {
            avgCenter = Vector3.zero;
            int numBoids = boids.Length;
            var boidData = new BoidData[numBoids];

            for (int i = 0; i < boids.Length; i++) 
            {
                boidData[i].position = boids[i].position;
                boidData[i].direction = boids[i].forward;
            }

            var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
            boidBuffer.SetData(boidData);

            compute.SetBuffer(0, "boids", boidBuffer);
            compute.SetInt("numBoids", boids.Length);
            compute.SetFloat("viewRadius", config.percepRadius);
            compute.SetFloat("avoidRadius", config.avoidRadius);

            int threadGroups = Mathf.CeilToInt(numBoids / (float) threadGroupSize);
            compute.Dispatch(0, threadGroups, 1, 1);

            boidBuffer.GetData(boidData);
            int validBoids = 0;
            for (int i = 0; i < boids.Length; i++) 
            {
                if (boids[i] != null)
                {
                    boids[i].avgHeading = boidData[i].groupHeading;
                    boids[i].groupCenter = boidData[i].groupCenter;
                    boids[i].avgAvoidHeading = boidData[i].avoidHeading;
                    boids[i].numOthers = boidData[i].numOthers;
                    if (!freezeState) 
                    {
                        boids[i].UpdateBoid();
                    }
                    else
                    {
                        boids[i].StopAnimation();
                    }
                    if (!boids[i].isObjective)
                    {
                        avgCenter += boids[i].position;
                        validBoids++;
                    }
                }
                
            }
            avgCenter /= validBoids;
            boidBuffer.Release();
        }
    }
}