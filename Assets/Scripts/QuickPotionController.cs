using UnityEngine;
using System.Collections.Generic;

public class QuickPotionController : MonoBehaviour {

    const float rotationSpeed = -600f;
    const float collideRadius = 1f;
    const int damage = 2;
    const int chaindamage = 1;
    const float killTimer = 4f;

    const float staticRadius = 0.5f;
    const int numStatics = 8;
    const float staticvelocity = 15;
    const float staticKillTimer = 0.50f;
    const float electrifyChance = 0.75f;

    const float chainCooldown = 0.25f;
    List<ChainRecord> chainRecord;

    Collider2D[] collisions = new Collider2D[50];
    Entities entities;

	// Use this for initialization
	void Start () {
        chainRecord = new List<ChainRecord>();
        entities = GameObject.Find("GameManager").GetComponent<Entities>();
        Object.Destroy(gameObject, killTimer);
	}
	
	// Update is called once per frame
	void Update () {
        // to give a cooldown for chain reactions.
        // otherwise, particles spawned within a GO will collide with the GO and spawn more infinitely.
        var tempChain = new List<ChainRecord>();
        foreach(var c in chainRecord) {
            c.IncrementTimer(Time.deltaTime);
            if (c.timer > chainCooldown) { 
                tempChain.Add(c); 
            }
        }
        foreach (var e in tempChain) {
            chainRecord.Remove(e);
        }

        gameObject.transform.Rotate(0,0,rotationSpeed);

        // wallchecking. identical code is in BlastPotionController.Update()
        var coll = Physics2D.OverlapPointAll(gameObject.transform.position);
        foreach (var c in coll) {
            if (c.gameObject.tag == "Geometry") {
                Object.Destroy(gameObject);
            }
        }

        Collide();
        SpawnStatic();
	}

    void SpawnStatic() {
        for (int i = 0; i < numStatics; i++) {
            var p = (GameObject) Instantiate(entities.particle, gameObject.transform.position, Quaternion.identity);
            var pc = p.gameObject.GetComponent<ParticleController>();
            if (pc != null) {
                pc.Init(ParticleType.quick, true, PotionColors.Quick, staticKillTimer, 4);
                pc.quickController = this;
            }

            // Vector2 dir = (target.transform.position - gameObject.transform.position).normalized;
            Vector2 dir = ((Vector2)Random.insideUnitCircle) * staticvelocity;

            p.GetComponent<Rigidbody2D>().AddForce(dir);
        }
    }



    void Collide() {
        for (int i = 0; i < collisions.Length; i++) { collisions[i] = null; }

        Physics2D.OverlapCircleNonAlloc(gameObject.transform.position, collideRadius, collisions);
        foreach (var c in collisions) {
            if (c != null) {
                var go = c.gameObject;
                if (go.tag == "MovingEntity") {
                    var hc = go.GetComponent<HealthController>();
                    if (hc != null) {
                        hc.ReceiveDamage(damage);
                    }
                }
                if (go.tag == "Mist") {
                    var mc = go.GetComponent<MistController>();
                    if (mc != null) {
                        mc.ClearMist(new Vector2(0,0));
                    }
                }
                if (go.tag == "Geometry") {
                    var gc = go.GetComponent<GeometryController>();
                    if (gc != null) {
                        gc.Electrify(gameObject, electrifyChance);
                    }
                }
            }
        }
    }

    public void ChainFrom(GameObject other, Vector2 rechainvector) {
        HealthController hc = other.GetComponent<HealthController>();
        if (hc != null) {
            hc.ReceiveDamage(chaindamage);
        }

        foreach (var c in chainRecord) {
            if(c.entity == other) {
                return;
            }
        }

        for (int i = 0; i < (numStatics * 2); i++) {
            Vector2 pos = RandomizeVector(other.transform.position, 0.30f);
            var newpar = (GameObject) Instantiate(entities.particle, pos, Quaternion.identity);
            var pc = newpar.GetComponent<ParticleController>();
            if (pc != null) {
                pc.Init(ParticleType.quick, true, PotionColors.Quick, staticKillTimer, 4);
                pc.quickController = this;
            }
            rechainvector = RandomizeVector(rechainvector, 0.25f);
            newpar.GetComponent<Rigidbody2D>().AddForce(rechainvector * 30f);
        }

        var newRec = new ChainRecord(0f, other);
        chainRecord.Add(newRec);
    }

    GameObject ClosestGameObject() {
        var allMoving = GameObject.FindGameObjectsWithTag("MovingEntity");
        GameObject closestObj = null;
        float closestDist = 9999;
        foreach (var m in allMoving) {
            if (m == gameObject) { continue; }
            if (m == null) { closestObj = m; continue; }
            float dist = Vector2.Distance(gameObject.transform.position, m.transform.position);
            if (dist < closestDist) {
                closestObj = m;
            }
        }
        return closestObj;
    }

    public Vector2 RandomizeVector(Vector2 baseVec, float noise) {
        return (Vector2) baseVec + new Vector2(Random.Range(-1 * noise, noise), Random.Range(-1 * noise, noise));
    }
}


public class ChainRecord {
    public float timer;
    public GameObject entity;
    public ChainRecord(float r, GameObject go) {
        timer = r;
        entity = go;
    }
    public void IncrementTimer(float t) {
        timer += t;
    }
}
