using UnityEngine;
using System.Collections.Generic;

public class HealthController : MonoBehaviour {

    public int health;

    // cooldown for taking damage
    public float invulntimer;
    float invulntimerCurrent;
    bool invulnState = false;

    //////////////////////////////////
    // default values for the lazy dev
    //////////////////////////////////
    public float defaultEnemyInvulnTimer = 0.5f;
    public int defaultEnemyHealth = 5;

    Entities entities;
    MapObject myObject;

    void Start() {
        myObject = gameObject.GetComponent<MapObject>();
    }

    void Update() {
        if (entities == null) {
            entities = GameObject.Find("GameManager").GetComponent<Entities>();
        }


        if (invulnState) {
            invulntimerCurrent += Time.deltaTime;
            if (!myObject.isFlickering) {
                myObject.isFlickering = true;
            }
        }
        if (invulntimerCurrent > invulntimer) {
            ResetInvuln();
        }
    }

    void ResetInvuln() {
        invulnState = false;
        invulntimerCurrent = 0;
        myObject.isFlickering = false;
    }

    public bool ReceiveDamage(int debitamt) {
        // set health and send signal back if the object will die.
        if (!invulnState) {
            if (gameObject.tag == "Player") {
                if (gameObject.GetComponent<PlayerController>().isRollingInvuln) {
                    return false;
                }
            }
            health -= debitamt;
            DamagePhantom(debitamt);
            if(gameObject.tag == "Player") {
                GameObject.Find("UI Manager").GetComponent<UIController>().AnimateDamage();
                GameObject.Find("GameManager").GetComponent<GameController>().thisLevelDamage += debitamt;
            }
        }

        if (health <= 0) {
            if (gameObject.tag == "Player") {
                gameObject.GetComponent<PlayerController>().PlayerDeath();
            } else {
                Object.Destroy(gameObject);
                entities.Kill(gameObject.transform.position);

                RollForPotion();
            }

            return true;
        }

        // have to remember to explicitly set the invuln cooldown
        // for enemies it will be a low number.
        // for player it will be higher.
        if (invulntimer == 0) {
            throw new System.NotImplementedException("No invulntimer set for object " + this);
        } else {
            invulnState = true;
        }

        return false;
    }

    void RollForPotion() {
        if (Random.value < 0.2f) {
            Vector2 pos = (Vector2) gameObject.transform.position + new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f));
            var newPot = (GameObject) Instantiate(entities.ammoPickup, pos, Quaternion.identity );
            newPot.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitCircle * 10f);

        }
    }

    public void GiveHealth(int num) {
        if (health <= 6 - num) {
            health = health + num;
            GameObject.Find("UI Manager").GetComponent<UIController>().AnimateHealth();
        }
    }

    void DamagePhantom(int amt) {
        var dp = (GameObject) Instantiate(entities.damagePhantom, gameObject.transform.position, Quaternion.identity);
        var spr = dp.GetComponent<SpriteRenderer>();
        var sourceSpr = gameObject.GetComponent<SpriteRenderer>();
        Sprite sourceSprite = sourceSpr.sprite;
        spr.sprite = sourceSprite;
        spr.color = sourceSpr.color * new Color(1f,1f,1f,0.4f);
        var tex = dp.transform.GetChild(0).gameObject.GetComponent<TextMesh>();
        tex.color = PotionColors.Danger * new Color(1f,1f,1f,0.6f);
        tex.text = amt.ToString();
        dp.GetComponent<Rigidbody2D>().AddForce(((Vector2)gameObject.transform.position + new Vector2(10f,20f)) * 5f);
        Object.Destroy(dp, 0.6f);
    }



}
