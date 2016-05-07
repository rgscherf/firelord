using UnityEngine;
using System.Collections;
using Pathfinding;

public class EnemyFollow : MonoBehaviour {

    float speed = 10f;
    public Path path;
    public Transform target;
    public float nextWaypointDistance = 0.1f;

    const float wpCooldown = 0.4f;
    float wpCooldownCurrent = 0f;


    const float repath = 1f;
    float repathCooldown = 0f;

    Rigidbody2D myrigidbody;
    Vector2 movementVector;

    int currentWaypoint;
    Seeker seeker;

	void Start () {
        myrigidbody = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        seeker.StartPath(transform.position, target.position, OnPathComplete);
        movementVector = new Vector2(0,0);
	}

    public void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        } else {
            Debug.Log("Path error, " + p);
        }
    }

    void FixedUpdate() {
        myrigidbody.AddForce(movementVector);
    }
	void Update () {

        repathCooldown += Time.deltaTime;
        if (repathCooldown > repath) {
            seeker.StartPath(transform.position, target.position, OnPathComplete);
            repathCooldown = 0;
        }

        if (path == null) {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count) {
            return;
        }

        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
            currentWaypoint++;
            // if (wpCooldownCurrent > wpCooldown) {
            //     currentWaypoint++;
            //     wpCooldownCurrent = 0;
            //     return;
            // }  else {
            //     wpCooldownCurrent += Time.deltaTime;
            // }
        }

        Vector2 dir = (path.vectorPath[currentWaypoint + 1] - transform.position).normalized;
        movementVector = dir * speed;
	}
}



















