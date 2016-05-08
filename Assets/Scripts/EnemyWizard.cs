using UnityEngine;
using System.Collections;

public class EnemyWizard : MapObject {

    Entities entities;
    HealthController healthController;
    Rigidbody2D rigid;

    const float attackCooldown = 2f;
    float attackCooldownCurrent;
    bool attacking;

    const float windingUpTimer = 3f;
    float windingUpTimerCurrent;
    bool windingUp;

    const float coolingDownTimer = 1f;
    float coolingDownTimerCurrent;
    bool coolingDown;

    float minimumDistance = 8f;
    float projectileSpeed = 1000f;

    GameObject player;
    Transform playertransform;

    public override bool poisoned {get; set;}
    PoisonController poisonController;
    const float poisonComeDown = 2f; 
    float poisonComeDownCurrent = 0f;

    SpriteRenderer spr;
    Color currentColor;
    Color baseColor;


	// Use this for initialization
	void Start () {
        baseColor = PotionColors.White;
        currentColor = baseColor;

        entities = GameObject.Find("GameManager").GetComponent<Entities>();
        healthController = gameObject.GetComponent<HealthController>();
        healthController.health = healthController.defaultEnemyHealth;
        healthController.invulntimer = healthController.defaultEnemyInvulnTimer;
        spr = gameObject.GetComponent<SpriteRenderer>();


        player = GameObject.Find("Player");
        playertransform = player.transform;
        rigid = gameObject.GetComponent<Rigidbody2D>();
	}

	
    void FixedUpdate() {
        if (attacking) {
            rigid.velocity = new Vector2(0f,0f);
        }
    }

	// Update is called once per frame
	void Update () {
        if (player == null) {
            player = GameObject.Find("Player");
        }
        if (!attacking) {
            attackCooldownCurrent += Time.deltaTime;
            if (attackCooldownCurrent > attackCooldown && Vector2.Distance(gameObject.transform.position, player.transform.position) >= minimumDistance) {
                BeginAttackSequence();
            }
        }
        if (windingUp) {
            const float windupshake = 0.09f;
            transform.position = (Vector2) transform.position + new Vector2(Random.Range(-windupshake, windupshake), Random.Range(-windupshake, windupshake));
            windingUpTimerCurrent += Time.deltaTime;
            if(windingUpTimerCurrent > windingUpTimer) {
                windingUp = false;
                coolingDown = true;
                ExecuteAttack();
            }
        }
        if (coolingDown) {
            coolingDownTimerCurrent += Time.deltaTime;
            if (coolingDownTimerCurrent > coolingDownTimer) {
                AttackCleanup();
            }
        }

        poisonComeDownCurrent += Time.deltaTime;

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


    void ExecuteAttack() {
        float angleto = Vector2.Angle((Vector2) gameObject.transform.position, (Vector2) playertransform.position);
        Transform caster = transform.GetChild(0);
        caster.transform.LookAt(playertransform);
        Vector2 targetDirection = caster.forward.normalized;
        var wizorb = (GameObject) Instantiate(entities.wizorb, gameObject.transform.position, Quaternion.identity);
        wizorb.GetComponent<Rigidbody2D>().AddForce(targetDirection * projectileSpeed);
        wizorb.GetComponent<SpriteRenderer>().color = PotionColors.Danger * new Color(1f,1f,1f,0.5f);
        wizorb.GetComponent<Rigidbody2D>().AddTorque(1200f);
        coolingDown = true;
    }

    void AttackCleanup() {
        attacking = false;
        windingUp = false;
        coolingDown = false;

        attackCooldownCurrent = 0f;
        windingUpTimerCurrent = 0f;
        coolingDownTimerCurrent = 0f;
    }
    void BeginAttackSequence() {
        attacking = true;
        windingUp = true;
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
