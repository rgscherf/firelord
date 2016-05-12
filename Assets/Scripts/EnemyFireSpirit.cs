using UnityEngine;
using System.Collections;
using System.Linq;


public class EnemyFireSpirit : MapObject {

    enum State { spawning, casting, despawning };

    Entities entities;
    HealthController healthController;
    Rigidbody2D rigid;
    GameObject player;
    GameController game;

    public override bool poisoned {get; set;}
    PoisonController poisonController;
    const float poisonComeDown = 2f; 
    float poisonComeDownCurrent = 0f;

    SpriteRenderer spr;
    Color currentColor;
    Color baseColor;

    float spawningTimer = 2f;
    float spawningTimerCurrent;

    float castingTimer = 3f;
    float castingTimerCurrent;

    float despawningTimer = 2f;
    float despawningTimerCurrent;

    State state; 

    float attackLifetime = 5f;

    void Start() {
        baseColor = PotionColors.White;
        currentColor = baseColor;
        spr = gameObject.GetComponent<SpriteRenderer>();
        game = GameObject.Find("GameManager").GetComponent<GameController>();
        entities = GameObject.Find("GameManager").GetComponent<Entities>();
        healthController = gameObject.GetComponent<HealthController>();
        healthController.health = healthController.defaultEnemyHealth;
        healthController.invulntimer = healthController.defaultEnemyInvulnTimer;
        spr = gameObject.GetComponent<SpriteRenderer>();


        player = GameObject.Find("Player");
        rigid = gameObject.GetComponent<Rigidbody2D>();

        state = State.spawning;
        LeanTween.scale(gameObject, new Vector3(1f,1f,1f), spawningTimer);
    }

    void Update() {

        Vector2 pos = (Vector2)transform.position + ((Vector2) Random.insideUnitCircle * 0.1f);
        transform.position = pos;
        switch (state) {
            case State.spawning:
                IncrementSpawning();
                break;
            case State.casting:
                IncrementCasting();
                break;
            case State.despawning:
                IncrementDespawning();
                break;
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

    void IncrementSpawning() {
        spawningTimerCurrent += Time.deltaTime;
        if (spawningTimerCurrent > spawningTimer) {
            state = State.casting;
        }
    }

    void InitiateAttack() {
        float startingDistance = 4f + Random.Range(0f, 3f);
        var startingPos = (Vector3) (Random.insideUnitCircle.normalized * startingDistance);
        startingPos += player.transform.position;
        var f = (GameObject) Instantiate(entities.firespiritattack, startingPos, Quaternion.identity);
        Object.Destroy(f, attackLifetime);
    }

    void IncrementCasting() {
        castingTimerCurrent += Time.deltaTime;

        if (castingTimerCurrent > castingTimer) {
            InitiateAttack();
            LeanTween.scale(gameObject, new Vector3(.1f,.1f,1f), despawningTimer);
            state = State.despawning;
        }
    }

    void IncrementDespawning() {
        despawningTimerCurrent += Time.deltaTime;

        if (despawningTimerCurrent > despawningTimer) {
            transform.position = RandomClearPosition();
            LeanTween.scale(gameObject, new Vector3(1f,1f,1f), spawningTimer);
            state = State.spawning;
            CleanupTimers();
        }
    }

    void CleanupTimers() {
        spawningTimerCurrent = 0f;
        castingTimerCurrent = 0f;
        despawningTimerCurrent = 0f;
    }

    Vector2 RandomClearPosition() {
        var clearFloors = GameObject
                            .FindGameObjectsWithTag("Floor")
                            .Where( f => Physics2D.OverlapCircleAll(f.transform.position, 0.75f).Length == 0)
                            .Where( e => e.GetComponent<MapObject>().room == game.currentRoom)
                            .ToArray();
        if (clearFloors.Length > 0) {
            return clearFloors[Random.Range(0, clearFloors.Length)].transform.position;
        }
        return transform.position;
    }


    public override void Flicker() {
        spr.color = Random.value > 0.5f ? currentColor : currentColor * new Color (1f,1f,1f,0.4f);
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

    void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject.tag == "Player") {
            var hc = other.gameObject.GetComponent<HealthController>();
            if (hc != null) {
                hc.ReceiveDamage(1);
            }
        }
    }
}
