using UnityEngine;
using System.Collections;

public class EnemyOoze : MapObject {

    float attackDistance = 2f;
    public const int damage = 1;
    public Transform playerTransform;

    GameObject child;

    const float attackCooldown = 1f;
    float attackCooldownCurrent = 0f;

    const float attackWindup = 0.5f;
    float attackWindupCurrent = 0f;
    bool isWinding = false;

    Color[] attackColors = {PotionColors.White, PotionColors.Danger};

    Entities entities;


	// Use this for initialization
	void Start () {
        
        GetComponent<HealthController>().health = 4;
        GetComponent<HealthController>().invulntimer = 0.1f;

        entities = GameObject.Find("GameManager").GetComponent<Entities>();

        playerTransform = null;
        child = transform.GetChild(0).gameObject;
        child.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (isWinding) {
            gameObject.GetComponent<SpriteRenderer>().color = attackColors[Random.Range(0,attackColors.Length)];
            attackWindupCurrent += Time.deltaTime;
            if (attackWindupCurrent > attackWindup) {
                Attack();
            }
        }
        attackCooldownCurrent += Time.deltaTime;
        if (playerTransform == null) {
            playerTransform = GameObject.Find("Player").transform;
        } else {
            if (Vector3.Distance(playerTransform.position, transform.position) <= attackDistance && attackCooldownCurrent > attackCooldown) {
                child.SetActive(true);
                isWinding = true;
            }
        }
	}

    void Attack() {
        var objs = new GameObject[4];
        foreach (var v in new Vector2[]{Vector2.up * 2, Vector2.right * 2, Vector2.down * 2, Vector2.left * 2, Vector2.up + Vector2.right, Vector2.up + Vector2.left, Vector2.down + Vector2.right, Vector2.down + Vector2.left}) {
            Vector2 pos = (Vector2) gameObject.transform.position + v;
            var obj = (GameObject) Instantiate(entities.particle, pos, Quaternion.identity);
            obj.GetComponent<ParticleController>().Init(gameObject, false, PotionColors.Danger * new Color(1,1,1,0.75f), 0.15f, 16);
            obj.GetComponent<Rigidbody2D>().AddTorque(800f);
            obj.transform.parent = gameObject.transform;
        }

        gameObject.GetComponent<SpriteRenderer>().color = PotionColors.White;
        attackWindupCurrent = 0;
        attackCooldownCurrent = 0;
        isWinding = false;
    }
}
