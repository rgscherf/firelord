using UnityEngine;
using System.Collections;

public class Entities : MonoBehaviour {

    public GameObject player;

    public GameObject wall01;
    public GameObject wall02;
    public GameObject wall03;
    public GameObject[] walls;

    public GameObject door;
    public GameObject lastRoomDoor;

    public GameObject mist;

    public GameObject empty;
    public GameObject newline;
    public GameObject damagePhantom;
    public GameObject particle;

    public GameObject ooze;
    public GameObject wizard;
    public GameObject wizorb;
    public GameObject bigdog;
    public GameObject bigdogattack;
    public GameObject firespirit;
    public GameObject firespiritattack;

    public GameObject ammoPickup;
    
    public GameObject thrownBlast;
    public GameObject thrownQuick;
    public GameObject thrownSpine;
    public GameObject thrownVenom;

    public GameObject chest;
    public GameObject shrinegoat;

    
    void Awake() {
        walls = new GameObject[] {wall01, wall02, wall03};
    }

    public void Kill(Vector2 killed) {
        const int numparticles = 40;
        var particles = new GameObject[numparticles];
        for (int i = 0; i < numparticles; i++) {
            var pos = (Vector2) killed + new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
            particles[i] = (GameObject) Instantiate(particle, pos, Quaternion.identity);
        }
        foreach (var p in particles) {
            p.GetComponent<Rigidbody2D>().AddForce(getOutwardExplosionVector(killed, p.transform.position, 600));
            p.GetComponent<ParticleController>().Init(ParticleType.effects, true, PotionColors.White * new Color (0.5f,0.5f,0.5f,1f), 1.5f, 4);
        }
    }

    public Vector2 getOutwardExplosionVector(Vector2 exploder, Vector2 explodee, float blastForce) {
        return -1 * blastForce * (exploder - explodee);
    }
}
