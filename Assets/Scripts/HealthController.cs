using UnityEngine;
using System.Collections.Generic;

public class HealthController : MonoBehaviour {

    public int health;

    // cooldown for taking damage
    public float invulntimer;
    float invulntimerCurrent;
    bool invulnState = false;

    SpriteRenderer spr;
    Color spriteBaseColor;

    void Start() {
        SetSprite();
    }

    void SetSprite() {
        spr = gameObject.GetComponent<SpriteRenderer>();
        spriteBaseColor = spr.color;
    }
    void Update() {
        if(spr == null) {
            SetSprite();
        }

        if (invulnState) {
            invulntimerCurrent += Time.deltaTime;
            Flicker();
        }
        if (invulntimerCurrent > invulntimer) {
            ResetInvuln();
        }
    }

    void ResetInvuln() {
        spr.color = spriteBaseColor;
        invulnState = false;
        invulntimerCurrent = 0;
    }

    void Flicker () {
        var colors = new Color[] { spriteBaseColor * new Color (1,1,1,0.75f), spriteBaseColor * new Color (1,1,1,0.5f) };
        spr.color = colors[Random.Range(0,colors.Length)];
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
        }

        if (health <= 0) {
            Object.Destroy(gameObject);
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

}
