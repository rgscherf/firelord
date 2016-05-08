using UnityEngine;
using System.Collections;

public class VenomPuke : MonoBehaviour {
    float spawningTimer = 0.03f;
    float spawningTimerCurrent = 0f;

    const float lifetime = 5f;
    float lifetimeTimer = 0f;


    float cloudLifetime = 2f;
    int cloudSize = 4;

    Entities entities;

	// Use this for initialization
	void Start () {
        entities = GameObject.Find("GameManager").GetComponent<Entities>();
	}
	
	// Update is called once per frame
	void Update () {
        spawningTimerCurrent += Time.deltaTime;
        if (spawningTimerCurrent > spawningTimer) {
            Vector2 pos = (Vector2) transform.position + (Random.insideUnitCircle);
            var ch = (GameObject) Instantiate(entities.particle, pos, Quaternion.identity);
            ParticleController pc = ch.gameObject.GetComponent<ParticleController>();
            if (pc != null) {
                pc.Init(ParticleType.venom, true, PotionColors.Venom, cloudLifetime, cloudSize);
            }
            spawningTimerCurrent = 0f;
        }
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer > lifetime) {
            Destroy(this);
        }
	}

}
