using UnityEngine;
using System.Collections;

public class Firespiritattack : MonoBehaviour {

    SpriteRenderer spr;
    GameObject player;
    float dir;
    Entities entities;
    Vector3 orbitPosition;

	void Start () {
        spr = gameObject.GetComponent<SpriteRenderer>();
        spr.color = PotionColors.Danger;

        entities = GameObject.Find("GameManager").GetComponent<Entities>();

        player = GameObject.Find("Player");
        orbitPosition = player.transform.position;

        dir = Random.value > 0.5f ? 1 : -1;
        gameObject.GetComponent<Rigidbody2D>().AddTorque(25f * dir);
	}

	void Update () {
        if (player == null) {
            player = GameObject.Find("Player");
        }
        transform.RotateAround(orbitPosition, new Vector3(0f,0f,3f), 120 * Time.deltaTime * dir);
        var spawnPos = (Random.insideUnitCircle * 0.75f) + (Vector2) transform.position;
        var p = (GameObject) Instantiate(entities.particle, spawnPos, transform.rotation);
        p.GetComponent<ParticleController>().Init(ParticleType.enemyattack, true, PotionColors.Danger, 2f, 4);
	}

    void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            var hc = other.gameObject.GetComponent<HealthController>();
            if (hc != null) {
                hc.ReceiveDamage(1);
            }
        }
    }
}
