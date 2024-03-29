﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class fingerClick : MonoBehaviour
{
    public Vector3 hitPosition;
    public List<GameObject> wayPoints = new List<GameObject>();
    public GameObject wayPointObject;
    public Material waypointMaterial;
    NavMeshAgent navMeshAgent;
    GameObject waypointParent;
    List<Vector3> origins = new List<Vector3>();
    List<Vector3> directions = new List<Vector3>();
    float lastTime;
    float distance = 0;
 //   bool obstaclePoint = false;
    private bool first;
    public bool debug = false;


    // Use this for initialization
    void Start()
    {

        waypointParent = new GameObject();
        waypointParent.name = "FingerWaypoints";
        if (GameObject.FindGameObjectWithTag("ImageTarget") != null)
        {
            waypointParent.transform.parent = GameObject.FindGameObjectWithTag("ImageTarget").transform;
        }
        navMeshAgent = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!debug)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                setNewPosition(Input.GetTouch(0).position);
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                setNewPosition(Input.mousePosition);
            }
        }


        //Move player to waypoints
        if (wayPoints.Count > 0)
        {
            navMeshAgent.destination = wayPoints[0].transform.position;
/*
                     //Draw path to next waypoint
                     this.transform.GetComponent<LineRenderer>().SetVertexCount(navMeshAgent.path.corners.Length);
                     this.transform.GetComponent<LineRenderer>().SetPosition(0, this.transform.position);
                     for (int i = 1; i < navMeshAgent.path.corners.Length; i++)
                     {
                         this.transform.GetComponent<LineRenderer>().SetPosition(i, navMeshAgent.path.corners[i] + transform.up/6);
                     }


            //Detect if NavMeshAgent is hitting an obstacle
            for (int i = 1; i < navMeshAgent.path.corners.Length; i++)
            {
                Vector3 pathPoint = navMeshAgent.path.corners[i - 1];
                Vector3 nextPoint = navMeshAgent.path.corners[i];
                RaycastHit hit;

                //    Debug.DrawLine(pathPoint, nextPoint, Color.red);

                if (Physics.Linecast(pathPoint, nextPoint, out hit))
                {
                    if (hit.transform.gameObject.tag == "Gate" && wayPoints[0].transform.position != hit.transform.position)
                    {
                        Debug.DrawLine(pathPoint, hit.point, Color.green);
                        wayPoints[0].transform.position = hit.point;
                        distance = 4.5f;
          //              obstaclePoint = true;
                    }
                }
            }*/

            if (distance == 0) distance = 0.5f;
            
            if (Vector3.Distance(this.transform.position, wayPoints[0].transform.position) < distance)
            {
                Destroy(wayPoints[0]);
                wayPoints.RemoveAt(0);
                distance = 0;
                navMeshAgent.speed = 15;
                navMeshAgent.destination = this.transform.position;
                //         obstaclePoint = false;

                if (wayPoints.Count == 0)
                {
                    navMeshAgent.ResetPath();
                    this.transform.GetComponent<LineRenderer>().SetVertexCount(0);
                }
            }
        }
    }

    //Apparently this code has to be in FixedUpdate instead of Update
    public void setNewPosition(Vector3 inputPosition)
    {
        //Create waypoints
        var ray = Camera.main.ScreenPointToRay(new Vector3(inputPosition.x, inputPosition.y, 0));

        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit))
        {
            //For debug purposes
            origins.Add(ray.origin);
            directions.Add(rayHit.point);
            //#######//

            //Check that raycast point is on ground, and on the navmesh
            NavMeshHit navmeshHit;
            int walkable = 1 << NavMesh.GetAreaFromName("Walkable");
            if (rayHit.collider.tag == "Ground" && NavMesh.SamplePosition(rayHit.point, out navmeshHit, 1.0f, walkable))
            {
                if (GameObject.Find("FingerWaypoint") != null) Destroy(GameObject.Find("FingerWaypoint"));
                hitPosition = new Vector3(navmeshHit.position.x, navmeshHit.position.y, navmeshHit.position.z);

                GameObject newWaypoint = Instantiate(wayPointObject) as GameObject;
                newWaypoint.transform.position = hitPosition + transform.up / 4;
                newWaypoint.transform.parent = waypointParent.transform;
                newWaypoint.name = "FingerWaypoint";

                if (wayPoints.Count > 0) wayPoints.RemoveAt(0);
                wayPoints.Add(newWaypoint);
            }
        }

    }
}
