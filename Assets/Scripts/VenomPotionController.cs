using UnityEngine;
using System.Collections;

public class VenomPotionController : MonoBehaviour {
    Entities entities;

    const float lifetime = 1.75f;
    const float cloudLifetime = 5f;

    const float spawnCooldown = 0.25f;
    float spawnCooldownCurrent;

	// Use this for initialization
	void Start () {
        Object.Destroy(gameObject, lifetime);
        entities = GameObject.Find("GameManager").GetComponent<Entities>();
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.localScale += new Vector3(0.06f, 0.06f, 0f);

        SpawnChild();
	}

    void SpawnChild() {
        // the second part of pos is an attempt to always spawn inside the radius of the square
        // times 0.75 so that we should never see edges popping out
        Vector2 pos = (Vector2) transform.position + (Random.insideUnitCircle * (transform.localScale.x / 2) * 0.75f);
        var ch = (GameObject) Instantiate(entities.particle, pos, Quaternion.identity);
        ParticleController pc = ch.gameObject.GetComponent<ParticleController>();
        if (pc != null) {
            pc.Init(gameObject, true, PotionColors.Venom, cloudLifetime, 8);
        }
    }
}
