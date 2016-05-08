using UnityEngine;
using System.Collections;

public class EnemyOoze : MapObject {

    float attackDistance = 2f;
    public const int damage = 1;
    public Transform playerTransform;

    const float attackCooldown = 1f;
    float attackCooldownCurrent = 0f;

    const float attackWindup = 0.5f;
    float attackWindupCurrent = 0f;
    bool isWinding = false;

    Entities entities;

    public override bool poisoned {get; set;}
    PoisonController poisonController;
    const float poisonComeDown = 2f; 
    float poisonComeDownCurrent = 0f;

    HealthController healthController;
    SpriteRenderer spr;
    Color currentColor;
    Color baseColor;

    // private override float invulnTimer {get; set;}

    // Use this for initialization
    void Start () {
        baseColor = PotionColors.White;
        currentColor = baseColor;

        healthController = gameObject.GetComponent<HealthController>();
        healthController.health = healthController.defaultEnemyHealth;
        healthController.invulntimer = healthController.defaultEnemyInvulnTimer;

        spr = gameObject.GetComponent<SpriteRenderer>();

        entities = GameObject.Find("GameManager").GetComponent<Entities>();

        playerTransform = null;
    }
    
    // Update is called once per frame
    void Update () {
        poisonComeDownCurrent += Time.deltaTime;

        if (isWinding) {
            const float windupshake = 0.1f;
            transform.position = (Vector2) transform.position + new Vector2(Random.Range(-windupshake, windupshake), Random.Range(-windupshake, windupshake));
            attackWindupCurrent += Time.deltaTime;

            if (attackWindupCurrent > attackWindup) {
                Attack();
            }
        } else {
            attackCooldownCurrent += Time.deltaTime;
        }

        if (playerTransform == null) {
            playerTransform = GameObject.Find("Player").transform;
        } else {
            if (Vector3.Distance(playerTransform.position, transform.position) <= attackDistance && attackCooldownCurrent > attackCooldown) {
                isWinding = true;
            }
        }

        if (poisoned) {
            poisonComeDownCurrent = 0f;
            int healthdebit = poisonController.Tick(Time.deltaTime);
            if (healthdebit == 99) {
                RemovePoison();
            } else if (healthdebit != 0) {
                healthController.ReceiveDamage(healthdebit);
            }
            currentColor = PotionColors.Venom;
        } else {
            currentColor = baseColor;
        }

        if(isFlickering) {
            Flicker();
        } else {
            spr.color = currentColor;
        }
    }

    public override void Flicker() {
        spr.color = Random.value > 0.5f ? currentColor : currentColor * new Color (1f,1f,1f,0.4f);
    }

    void Attack() {
        var objs = new GameObject[4];
        foreach (var v in new Vector2[]{Vector2.up * 2, Vector2.right * 2, Vector2.down * 2, Vector2.left * 2, Vector2.up + Vector2.right, Vector2.up + Vector2.left, Vector2.down + Vector2.right, Vector2.down + Vector2.left}) {
            Vector2 pos = (Vector2) gameObject.transform.position + v;
            var obj = (GameObject) Instantiate(entities.particle, pos, Quaternion.identity);
            obj.GetComponent<ParticleController>().Init(ParticleType.ooze, false, PotionColors.Danger * new Color(1,1,1,0.75f), 0.15f, 16);
            obj.GetComponent<Rigidbody2D>().AddTorque(800f);
            obj.transform.parent = gameObject.transform;
        }

        attackWindupCurrent = 0;
        attackCooldownCurrent = 0;
        isWinding = false;
    }

    public override void Poison() {
        if (poisoned != true && poisonComeDownCurrent > poisonComeDown) {
            poisoned = true;        
            currentColor = PotionColors.Venom;
            poisonController = new PoisonController();
            // gameObject.AddComponent<VenomPuke>();
        }
    }

    void RemovePoison() {
        poisoned = false;
    }

}
