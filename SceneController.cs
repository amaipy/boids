using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SceneController : MonoBehaviour 
{

    int cameraState;
    Camera TowerCamera;
    Camera GroupCamera;
    public Boid prefab;

    void Start()
    {
        TowerCamera = GameObject.FindGameObjectWithTag("TowerCamera").GetComponent<Camera>();
        GroupCamera = GameObject.FindGameObjectWithTag("GroupCamera").GetComponent<Camera>();
        cameraState = 1;
    }

    void lookFromTower()
    {
        TowerCamera.depth = 1;
        GroupCamera.depth = 0;
        Vector3 v = GameObject.FindGameObjectWithTag("BoidController").GetComponent<BoidController>().avgCenter;
        TowerCamera.transform.LookAt(v); 
    }

    void lookFromBehind()
    {
        TowerCamera.depth = 0;
        GroupCamera.depth = 1;
        Vector3 v = GameObject.FindGameObjectWithTag("BoidController").GetComponent<BoidController>().avgCenter;
        GroupCamera.transform.position = (v + new Vector3(0, 0, -20));
        GroupCamera.transform.LookAt(v); 
    }

    void lookFromSide()
    {
        TowerCamera.depth = 0;
        GroupCamera.depth = 1;
        Vector3 v = GameObject.FindGameObjectWithTag("BoidController").GetComponent<BoidController>().avgCenter;
        GroupCamera.transform.position = (v + new Vector3(-20, 0, 0));
        GroupCamera.transform.LookAt(v); 
        
    }

    void lookFromAbove()
    {
        TowerCamera.depth = 0;
        GroupCamera.depth = 1;
        Vector3 v = GameObject.FindGameObjectWithTag("BoidController").GetComponent<BoidController>().avgCenter;
        GroupCamera.transform.position = (v + new Vector3(0, 20, 0));
        GroupCamera.transform.LookAt(v); 
    }

    void Update () 
    {
        if (Input.GetKeyUp(KeyCode.Alpha1) || Input.GetKeyUp(KeyCode.Keypad1))
        {
            cameraState = 1;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2) || Input.GetKeyUp(KeyCode.Keypad2))
        {
            cameraState = 2;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3) || Input.GetKeyUp(KeyCode.Keypad3))
        {
            cameraState = 3;
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4) || Input.GetKeyUp(KeyCode.Keypad4))
        {
            cameraState = 4;
        }
        else if (Input.GetKeyUp(KeyCode.KeypadPlus) || Input.GetKeyUp(KeyCode.Plus))
        {
            Vector3 v = GameObject.FindGameObjectWithTag("BoidController").GetComponent<BoidController>().avgCenter;
            Vector3 pos = v + Random.insideUnitSphere * 10;
            Boid boid = Instantiate(prefab);
            boid.transform.position = pos;
            boid.transform.forward = Random.insideUnitSphere;
            GameObject.FindGameObjectWithTag("BoidController").GetComponent<BoidController>().initializeBoid(boid);
            GameObject.FindGameObjectWithTag("BoidController").GetComponent<BoidController>().updateBoidList();
        }
        else if (Input.GetKeyUp(KeyCode.KeypadMinus) || Input.GetKeyUp(KeyCode.Minus))
        {
            GameObject[] boids = GameObject.FindGameObjectsWithTag("BoidPrefab");
            if (boids.Length > 1)
            {
                int randonDestroy = Random.Range(0, boids.Length);
                Destroy(boids[randonDestroy]);
                GameObject.FindGameObjectWithTag("BoidController").GetComponent<BoidController>().updateBoidList();
            }
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            GameObject.FindGameObjectWithTag("BoidController").GetComponent<BoidController>().changeFreezeState();
        }
        else if (Input.GetKeyUp(KeyCode.M))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Boid>().changeVelocity(0.5f);
        }
        else if (Input.GetKeyUp(KeyCode.N))
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<Boid>().changeVelocity(-0.5f);
        }
        else if (Input.GetKeyUp(KeyCode.F))
        {
            RenderSettings.fog = !RenderSettings.fog; 
        }
        else if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }

        switch (cameraState)
        {
            case 1:
                lookFromTower();
                break;
            case 2:
                lookFromBehind();
                break;
            case 3:
                lookFromSide();
                break;
            case 4:
                lookFromAbove();
                break;
        }

    }

}