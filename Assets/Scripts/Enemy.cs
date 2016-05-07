using UnityEngine;
using System.Collections;
using Pathfinding;

public class Enemy : MapObject {

    public int health;
    float speed = 6f;
    public Path path;
    public Transform target;
    public float nextWaypointDistance = 0.01f;

    const float wpCooldown = 0.4f;
    float wpCooldownCurrent = 0f;

    Rigidbody2D myrigidbody;

    int currentWaypoint;
    Seeker seeker;

	void Start () {
        GetComponent<HealthController>().health = 4;
        myrigidbody = GetComponent<Rigidbody2D>();
        seeker = GetComponent<Seeker>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        // target = new Vector3(-13, -3, 0);
        seeker.StartPath(transform.position, target.position, OnPathComplete);
	}

    public void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            currentWaypoint = 0;
        } else {
            Debug.Log("Path error, " + p);
        }
    }

	void Update () {
        if (path == null) {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count) {
            return;
        }

        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.deltaTime;
        myrigidbody.AddForce(new Vector2(dir.x, dir.y));

        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
            if (wpCooldownCurrent > wpCooldown) {
                currentWaypoint++;
                wpCooldownCurrent = 0;
                return;
            }  else {
                wpCooldownCurrent += Time.deltaTime;
            }
        }
	}
}



















