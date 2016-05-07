using UnityEngine;
using System.Collections;

public class EnemyOoze : MapObject {

    float attackDistance = 2f;
    const int damage = 1;
    public Transform playerTransform;

    GameObject child;

    const float attackCooldown = 1f;
    float attackCooldownCurrent = 0f;

    const float attackWindup = 0.5f;
    float attackWindupCurrent = 0f;
    bool isWinding = false;

    Color[] attackColors = {PotionColors.White, PotionColors.Danger};


	// Use this for initialization
	void Start () {
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
        var inRadius = Physics2D.OverlapCircleAll(transform.position, attackDistance);
        foreach (var r in inRadius) {
            var i = r.gameObject;
            if(i.tag == "Player") {
                HealthController hc = i.gameObject.GetComponent<HealthController>();
                if (i != null) {
                    hc.ReceiveDamage(damage);
                }
            }
        }
        gameObject.GetComponent<SpriteRenderer>().color = PotionColors.White;
        attackWindupCurrent = 0;
        attackCooldownCurrent = 0;
        isWinding = false;
    }
}
