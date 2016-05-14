using UnityEngine;
using System.Collections;
using System.Linq;

public class ShrineHeartController : MapObject {

    bool hasPaid = false;

    Vector3 originalPos;
    bool shaking;
    const float shakeTimer = 1f;
    float shakeTimerCurrent;

    const float actionCooldown = 0.25f;
    float actionCooldownCurrent;

    ShrineHeartShard shBlast;
    ShrineHeartShard shQuick;
    ShrineHeartShard shSpine;
    ShrineHeartShard shVenom;

    HealthController playerHealthController;

    void Awake() {
        shBlast = gameObject.transform.Find("heartBlast").gameObject.GetComponent<ShrineHeartShard>();
        shQuick = gameObject.transform.Find("heartQuick").gameObject.GetComponent<ShrineHeartShard>();
        shSpine = gameObject.transform.Find("heartSpine").gameObject.GetComponent<ShrineHeartShard>();
        shVenom = gameObject.transform.Find("heartVenom").gameObject.GetComponent<ShrineHeartShard>();
    }
	// Use this for initialization
	void Start () {

        playerHealthController = GameObject.Find("Player").GetComponent<HealthController>();

        originalPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        actionCooldownCurrent += Time.deltaTime;

        shBlast.SetColor(PotionColors.Blast);
        shQuick.SetColor(PotionColors.Quick);
        shSpine.SetColor(PotionColors.Spine);
        shVenom.SetColor(PotionColors.Venom);

        if (shaking) {
            shakeTimerCurrent += Time.deltaTime;
            transform.position = originalPos + ((Vector3) Random.insideUnitCircle * 0.1f);
            if (shakeTimerCurrent > shakeTimer) {
                shaking = false;
                transform.position = originalPos;
                shakeTimerCurrent = 0f;
            }
        }
	
	}

    ShrineHeartShard PotionToShard(Potion dispatch) {
        switch (dispatch) {
            case Potion.Blast:
                return shBlast;
            case Potion.Quick:
                return shQuick;
            case Potion.Spine:
                return shSpine;
            case Potion.Venom:
                return shVenom;
            default:
                return shBlast;
        }
    }

    void ProcessTrigger(Potion dispatch) {
        int numActive = new[] {shBlast, shQuick, shSpine, shVenom}.Where( s => s.active).ToArray().Length;
        if (numActive > 1 || playerHealthController.health <= 4) {
            var shard = PotionToShard(dispatch);
            if (shard.active) {
                shard.active = false;
                numActive -= 1;
            }
            if (numActive == 0) {
                PayOut();
            }
        } else {
            shaking = true;
        }
    }

    void PayOut() {
        hasPaid = true;
        playerHealthController.GiveHealth(2);
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!hasPaid) {
            if (other.gameObject.tag == "Particle") {
                if (actionCooldownCurrent > actionCooldown) {
                    var pc = other.gameObject.GetComponent<ParticleController>();
                    if (pc != null) {
                        Potion pot = ParticleController.ParticleTypeToPotion(pc.instantiator);
                        ProcessTrigger(pot);
                    }
                    actionCooldownCurrent = 0f;
                }
                Object.Destroy(other.gameObject);
            }
        }
    }
}
