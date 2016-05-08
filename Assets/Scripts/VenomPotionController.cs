using UnityEngine;
using System.Collections;

public class VenomPotionController : MonoBehaviour {
    Entities entities;

    const float lifetime = 1.75f;
    public float cloudLifetime = 5f;
    public int cloudSize = 8;

    float spawningTimer;

    const float spawnCooldown = 0.25f;
    float spawnCooldownCurrent;

	// Use this for initialization
	void Start () {
        Object.Destroy(gameObject, lifetime + cloudLifetime * 2);
        entities = GameObject.Find("GameManager").GetComponent<Entities>();
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.localScale += new Vector3(0.06f, 0.06f, 0f);

        spawningTimer += Time.deltaTime;

        // wallchecking. 
        var coll = Physics2D.OverlapPointAll(gameObject.transform.position);
        foreach (var c in coll) {
            if (c.gameObject.tag == "Geometry") {
                spawningTimer = 99;
                // Object.Destroy(gameObject);
            }
        }

        if (spawningTimer < lifetime) {
            SpawnChild();
        }
	}

    void SpawnChild() {
        // the second part of pos is an attempt to always spawn inside the radius of the square
        // times 0.75 so that we should never see edges popping out
        Vector2 pos = (Vector2) transform.position + (Random.insideUnitCircle * (transform.localScale.x / 2) * 0.75f);
        var ch = (GameObject) Instantiate(entities.particle, pos, Quaternion.identity);
        ParticleController pc = ch.gameObject.GetComponent<ParticleController>();
        if (pc != null) {
            pc.Init(ParticleType.venom, true, PotionColors.Venom, cloudLifetime, cloudSize);
        }
    }
}
