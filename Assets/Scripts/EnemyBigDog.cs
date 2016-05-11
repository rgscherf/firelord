using UnityEngine;
using System.Collections;

public class EnemyBigDog : MapObject {

    bool charging;
    bool windingUp;

    float chargeFluctuation = 0.5f;
    float baseChargeCooldown = 3f;
    float chargeCooldown;
    float chargeCooldownCurrent;

    float windUpFluctuation = 0.5f;
    float baseWindingUpTimer = 2f;
    float windingUpTimer;
    float windingUpTimerCurrent;

    float magnitudeCheckGracePeriod = 1f;
    float magnitudeCheckGracePeriodCurrent;

    GameObject player;
    Entities entities;
    HealthController healthController;

    Rigidbody2D rigid;

    const float shakeAmt = 0.07f;
    const float chargePower = 9000f;
    const float velocityThresholdToBreakCharge = 0.8f;

    GameObject attack;
    const float attackTorque = 600f;

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
        spr = gameObject.GetComponent<SpriteRenderer>();

        player = GameObject.Find("Player");
        rigid = gameObject.GetComponent<Rigidbody2D>();
        entities = GameObject.Find("GameManager").GetComponent<Entities>();
        healthController = gameObject.GetComponent<HealthController>();
        healthController.health = healthController.defaultEnemyHealth;
        healthController.invulntimer = healthController.defaultEnemyInvulnTimer;
	}
	
	// Update is called once per frame
	void Update () {
        if (player == null) {
            player = GameObject.Find("Player");
        }
        if (!charging) {
            chargeCooldownCurrent += Time.deltaTime;
            if (chargeCooldownCurrent > chargeCooldown) {
                BeginAttackSequence();
            }
        }

        if (windingUp) {
            transform.position = (Vector2) transform.position + new Vector2(Random.Range(-shakeAmt, shakeAmt), Random.Range(-shakeAmt, shakeAmt));
            windingUpTimerCurrent += Time.deltaTime;
            if (windingUpTimerCurrent > windingUpTimer) {
                windingUp = false;
                Charge();
            }   
        } else {
            magnitudeCheckGracePeriodCurrent += Time.deltaTime;
            if (magnitudeCheckGracePeriodCurrent > magnitudeCheckGracePeriod) {
                float velocityMagnitude = rigid.velocity.magnitude;
                if (charging && velocityMagnitude < velocityThresholdToBreakCharge) {
                    CleanUpCharge();
                }
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
            currentColor = PotionColors.Venom + new Color (0.2f, 0.2f, 0.2f, 0f);
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

    void CleanUpCharge() {
        charging = false;
        windingUp = false;
        windingUpTimerCurrent = 0f;
        chargeCooldownCurrent = 0f;
        chargeCooldown = baseChargeCooldown + Random.Range(-chargeFluctuation, chargeFluctuation);
        if (attack != null) {
            Object.Destroy(attack);
        }
    }

    void Charge() {
        var playerpos = (Vector2) player.transform.position;
        playerpos += Random.insideUnitCircle * 2;
        Vector2 dir = (playerpos - (Vector2)transform.position).normalized;
        rigid.AddForce(dir * chargePower);
        magnitudeCheckGracePeriodCurrent = 0f;

        var mypos = (Vector2) transform.position;
        attack = (GameObject) Instantiate(entities.bigdogattack, new Vector3(mypos.x, mypos.y, 1f), Quaternion.identity);
        attack.GetComponent<Bigdogattack>().SetTransform(gameObject.transform);
        attack.GetComponent<SpriteRenderer>().color = PotionColors.Danger * new Color(1f,1f,1f,0.5f);
        attack.GetComponent<Rigidbody2D>().AddTorque(attackTorque);
        attack.transform.SetParent(gameObject.transform);
    }

    void BeginAttackSequence() {
        windingUpTimer = baseWindingUpTimer + Random.Range(-windUpFluctuation, windUpFluctuation);
        charging = true;
        windingUp = true;
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Player") {
            var hc = other.gameObject.GetComponent<HealthController>();
            if (hc != null) {
                hc.ReceiveDamage(1);
            }
        }
    }

    public override void Poison() {
        if (poisoned != true && poisonComeDownCurrent > poisonComeDown) {
            poisoned = true;        
            currentColor = PotionColors.Venom;
            poisonController = new PoisonController();
        }
    }

    void RemovePoison() {
        poisoned = false;
    }

}
