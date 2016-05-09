using UnityEngine;

public class PlayerController : MonoBehaviour {

#region init
    //////////////////////////////
    // gameobjects and other inits
    //////////////////////////////
    Rigidbody2D playerRigidBody;
    SpriteRenderer playerSpriteRenderer;
    GameController game;
    Entities entities;


    bool firing;

    ////////
    // ammo
    ///////

    public int blastammo = 5;
    public int quickammo = 5;
    public int spineammo = 5;
    public int venomammo = 5;

    ////////////////////
    // vars for movement
    ////////////////////
    const float speed = 9000;
    const float camRayLength = 100f;

    const float rollSpeedBoost = 2.5f;

    Vector2 movement;

    ////////////////////////
    // vars for Blast potion
    ////////////////////////
    public float blastCooldown = 1f;
    public float blastCooldownCurrent = 99f;

    public GameObject _blastOuterIndicator;
    public GameObject _blastInnerIndicator;
    GameObject blastOuterIndicator;
    GameObject blastInnerIndicator;

    LineRenderer blastGuide;
    float blastHoldStrength;
    const float blastWindupSpeed = 12f;

    ////////////////////////
    // vars for Quick potion
    ////////////////////////
    public float quickCooldown = 0.75f;
    public float quickCooldownCurrent = 99f;

    const float quickspeed = 1700f;

    ////////////////////////
    // vars for Spine potion
    ////////////////////////
    public float spineCooldown = 2f;
    public float spineCooldownCurrent = 99f;

    const float spineSpinSpeed = 60f;

    GameObject spineIndicator;

    ////////////////////////
    // vars for venom potion
    ////////////////////////
    public float venomCooldown = 2f;
    public float venomCooldownCurrent = 99f;

    const float venomRollSpeed = 250f;

    ///////////////////
    // vars for rolling
    ///////////////////
    public GameObject _rollOverlay1;
    public GameObject _rollOverlay2;
    GameObject rollOverlay1;
    GameObject rollOverlay2;

    const float timeToFinishRoll = 0.75f;
    float timeToFinishRollCurrent;

    const float rollCooldown = 0.75f;
    float rollCooldownCurrent;

    bool isRolling;
    public bool isRollingInvuln;

    public bool dead;

    UIController uiController;

    Potion _currentPotion;
    public Potion currentPotion {
        get { return _currentPotion; }
        set {
            _currentPotion = value;
            uiController.PotionSwap(value);
        }
    }

///////////
// end init
///////////

    void Awake() {
        game = GameObject.Find("GameManager").GetComponent<GameController>();
        entities = GameObject.Find("GameManager").GetComponent<Entities>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerSpriteRenderer.color = PotionColors.White;

        uiController = GameObject.Find("UI Manager").GetComponent<UIController>();

        playerStart();

        blastGuide = gameObject.AddComponent<LineRenderer>();
        blastGuide.material = new Material (Shader.Find("Particles/Additive"));
        blastGuide.SetWidth(0.01f, 0.01f);
        blastGuide.SetVertexCount(2);
        blastGuide.useWorldSpace = false;

        blastHoldStrength = 0f;

    }

    void playerStart() {
        dead = false;
        blastammo = 5; 
        quickammo = 5; 
        spineammo = 5; 
        venomammo = 5; 
        GetComponent<HealthController>().health = 6;
        GetComponent<HealthController>().invulntimer = 1f;
        currentPotion = Potion.Blast;
        blastHoldStrength = 0f;
    }

#endregion

#region FixedUpdate
    void FixedUpdate() {
        if(dead) {return;}
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Move(h, v);
        // Turning();
        // Animating(h, v);
    }

    void Move(float h, float v) {
        float dt = Time.deltaTime;
        float boost = isRolling ? rollSpeedBoost : 1f;

        movement = new Vector2(h, v).normalized;
        movement = movement * speed * dt * boost;
        playerRigidBody.AddForce(movement); 

    }
#endregion

#region update

    void Restart() {
        playerStart();
        game.Restart();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            Restart();
            return;
        }

        blastCooldownCurrent += Time.deltaTime;
        quickCooldownCurrent += Time.deltaTime;
        spineCooldownCurrent += Time.deltaTime;
        venomCooldownCurrent += Time.deltaTime;

        if(currentPotion == Potion.Spine) {
            PaintSpine();
        } else {
            UnpaintSpine();
        }

        // standard update operations:
        InputPotionSelection();
        InputDodge();
        InputFire();
    }

    void InputDodge() {
        if (rollOverlay1 != null && rollOverlay2 != null) {
            // update roll graphics every frame during roll. parenting didn't work...?
            rollOverlay1.transform.position = (Vector2) gameObject.transform.position + new Vector2(0, -.20f);
            rollOverlay2.transform.position = (Vector2) gameObject.transform.position + new Vector2(0, -.20f);

        }
        if (!isRolling && Input.GetAxis("Roll") == 1 && rollCooldownCurrent > rollCooldown) {
            BeginRolling();
        }
        if (isRolling) {
            ContinueRolling();
        } else {
            rollCooldownCurrent += Time.deltaTime;
        }
    }

    void BeginRolling() {
        isRollingInvuln = true;
        isRolling = true;
        timeToFinishRollCurrent = 0;
        InitiateRollGraphics();
    }

    void ContinueRolling() {
        timeToFinishRollCurrent += Time.deltaTime;
        if(timeToFinishRollCurrent > timeToFinishRoll) {
            FinishRolling();
        }
    }

    void FinishRolling() {
        isRollingInvuln = false;
        isRolling = false;
        rollCooldownCurrent = 0;
    }

    void InitiateRollGraphics() {
        rollOverlay1 = (GameObject) Instantiate(_rollOverlay1, gameObject.transform.position, Quaternion.identity);
        rollOverlay1.GetComponent<Rigidbody2D>().AddTorque(1200f);
        rollOverlay1.GetComponent<SpriteRenderer>().color = PotionColors.GetColor(currentPotion);
        Object.Destroy(rollOverlay1, timeToFinishRoll);

        rollOverlay2 = (GameObject) Instantiate(_rollOverlay2, gameObject.transform.position, Quaternion.identity);
        rollOverlay2.GetComponent<Rigidbody2D>().AddTorque(1200f);
        rollOverlay2.GetComponent<SpriteRenderer>().color = PotionColors.GetColor(currentPotion);
        Object.Destroy(rollOverlay2, timeToFinishRoll);
    }

    void ReleaseFire() {
        switch (currentPotion) {
            case Potion.Blast:
                var newBlast = (GameObject) Instantiate(entities.thrownBlast, transform.position, Quaternion.identity);
                newBlast.GetComponent<BlastPotionController>().Init(blastInnerIndicator.transform.position);
                blastCooldownCurrent = 0;
                blastammo -= 1;
                game.thisLevelThrown += 1;
                break;
        }

    }

    public void PlayerDeath() {
        dead = true;
        transform.position = new Vector2(100f,100f);
        entities.Kill(transform.position);
    }

    void InputFire() {
        if (Input.GetAxis("Fire") != 1 && firing) {
            if (blastCooldownCurrent > blastCooldown && blastammo > 0) {
                ReleaseFire();
            }

            firing = false;
        }
        firing = 1 == Input.GetAxis("Fire");
        BlastGuideCleanup();
        if(firing) {
            switch(currentPotion) {
                case Potion.None:
                    break;
                case Potion.Blast:
                    if(blastCooldownCurrent > blastCooldown) {
                        blastHoldStrength += Time.deltaTime * blastWindupSpeed;
                        BlastGuidePaint();
                    }
                    break;
                case Potion.Quick:
                    FireQuick();
                    break;
                case Potion.Spine:
                    FireSpine();
                    break;
                case Potion.Venom:
                    FireVenom();
                    break;
                default:
                    break;
            }
        }
    }

    void FireVenom() {
        if (venomCooldownCurrent < venomCooldown || venomammo <= 0) { return; }
        venomammo -= 1;
        game.thisLevelThrown += 1;
        venomCooldownCurrent = 0;

        Vector2 targetDir = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        targetDir = targetDir - (Vector2) gameObject.transform.position;
        var vgo = (GameObject) Instantiate(entities.thrownVenom, gameObject.transform.position, Quaternion.identity);
        vgo.GetComponent<Rigidbody2D>().AddForce(targetDir.normalized * venomRollSpeed);
    }

    void PaintSpine() {
        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(spineIndicator == null) {
            spineIndicator = (GameObject) Instantiate(_blastOuterIndicator, target, Quaternion.identity);
            spineIndicator.GetComponent<SpriteRenderer>().color = PotionColors.Spine;
        } else {
            spineIndicator.transform.position = target;
        }
        spineIndicator.transform.Rotate(new Vector3(0f,0f,spineSpinSpeed));
    }

    void UnpaintSpine() {
        if(spineIndicator != null) {
            Object.Destroy(spineIndicator);
        }
    }


    void FireSpine() {
        if (spineCooldownCurrent < spineCooldown || spineammo <= 0) { return; }
        spineammo -= 1;
        game.thisLevelThrown += 1;
        spineCooldownCurrent = 0f;

        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Instantiate(entities.thrownSpine, target, Quaternion.identity);
    }

    void FireQuick() {
        if (quickCooldownCurrent < quickCooldown || quickammo <= 0) { return; }
        quickammo -= 1;
        game.thisLevelThrown += 1;
        quickCooldownCurrent = 0f;

        Vector2 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 forceDirection = (target - (Vector2) gameObject.transform.position).normalized;
        var qui = (GameObject) Instantiate(entities.thrownQuick, gameObject.transform.position, Quaternion.identity);
        qui.GetComponent<Rigidbody2D>().AddForce(forceDirection * quickspeed);

    }

    void BlastGuideCleanup() {
        if(!firing || currentPotion != Potion.Blast) {
            blastGuide.enabled = false;
            blastHoldStrength = 0f;
            Object.Destroy(blastOuterIndicator);
            Object.Destroy(blastInnerIndicator);
        }
    }

    void BlastGuidePaint() {
        if (blastCooldownCurrent > blastCooldown) {
            if (!blastGuide.enabled) {
                blastGuide.enabled = true;
            }
            if (blastOuterIndicator == null) {
                blastOuterIndicator = (GameObject) Instantiate(_blastOuterIndicator, transform.position, Quaternion.identity);
                blastOuterIndicator.transform.parent = transform;
                blastOuterIndicator.GetComponent<SpriteRenderer>().color = PotionColors.Venom;

                blastInnerIndicator = (GameObject) Instantiate(_blastInnerIndicator, transform.position, Quaternion.identity);
                blastInnerIndicator.transform.parent = transform;
                blastInnerIndicator.GetComponent<SpriteRenderer>().color = PotionColors.Venom;
            }
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 localtarget = target - transform.position;
            float mult = 100;
            localtarget = Vector3.Scale(localtarget, new Vector3(mult,mult,1));
            localtarget.z = transform.position.z;
            blastGuide.SetPosition(0, new Vector3(0f,0f,0f));
            blastGuide.SetPosition(1, localtarget);

            Vector2 indicatorPos = ((Vector2)localtarget.normalized) * blastHoldStrength;
            blastOuterIndicator.transform.localPosition = indicatorPos;
            blastInnerIndicator.transform.localPosition = indicatorPos;
        }
    }

    public bool ReceivePotion(Potion newPotion) {
        switch (newPotion) {
            case Potion.Blast:
                if (blastammo >= 9) {
                    return true;
                } else {
                    blastammo++;
                    return false;
                }
            case Potion.Quick:
                if (quickammo >= 9) {
                    return true;
                } else {
                    quickammo++;
                    return false;
                }
            case Potion.Spine:
                if (spineammo >= 9) {
                    return true;
                } else {
                    spineammo++;
                    return false;
                }
            case Potion.Venom:
                if (venomammo >= 9) {
                    return true;
                } else {
                    venomammo++;
                    return false;
                }
            default:
                return false;
        }
    }

    void InputPotionSelection() {
        Potion potionSelection = Potion.None;

        if (Input.GetButtonDown("selectBlast")) {
            potionSelection = Potion.Blast;
        }
        else if (Input.GetButtonDown("selectQuick")) {
            potionSelection = Potion.Quick;
        }
        else if (Input.GetButtonDown("selectSpine")) {
            potionSelection = Potion.Spine;
        }
        else if (Input.GetButtonDown("selectVenom")) {
            potionSelection = Potion.Venom;
        }

        if (potionSelection != Potion.None) {
            if(firing) {
                ReleaseFire();
            }
            currentPotion = potionSelection;
        }
    }

#endregion

}
