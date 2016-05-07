using UnityEngine;

public class PlayerController : MonoBehaviour {

#region init
    const float speed = 19000;
    const float camRayLength = 100f;
    const float rollCooldown = 1.25f;

    bool rollOnCooldown ;
    float currentRollCooldown;
    Vector2 movement;
    Rigidbody2D playerRigidBody;
    SpriteRenderer playerSpriteRenderer;
    Color colorRollOffCooldown = PotionColors.White;
    Color colorRollOnCooldown;
    GameController game;

    bool firing;

    LineRenderer blastGuide;
    float blastHoldStrength;
    float blastWindupSpeed = 15f;
    public GameObject _blastOuterIndicator;
    public GameObject _blastInnerIndicator;
    GameObject blastOuterIndicator;
    GameObject blastInnerIndicator;

    Entities entities;

    Potion currentPotion;

    void Awake() {
        game = GameObject.Find("GameManager").GetComponent<GameController>();
        entities = GameObject.Find("GameManager").GetComponent<Entities>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerSpriteRenderer.color = colorRollOffCooldown;
        currentPotion = Potion.None;

        blastGuide = gameObject.AddComponent<LineRenderer>();
        blastGuide.material = new Material (Shader.Find("Particles/Additive"));
        blastGuide.SetWidth(0.01f, 0.01f);
        blastGuide.SetVertexCount(2);
        blastGuide.useWorldSpace = false;

        blastHoldStrength = 0f;

        colorRollOnCooldown = colorRollOffCooldown * new Color(1,1,1,0.5f);
        rollOnCooldown = false;
        currentRollCooldown = 0f;
    }
#endregion

#region FixedUpdate
    void FixedUpdate() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        float j = Input.GetAxis("Jump");
        Move(h, v, j);
        // Turning();
        // Animating(h, v);
    }

    void Move(float h, float v, float j) {
        float dt = Time.deltaTime;
        float boost = 1;

        if (j > 0.1f && !rollOnCooldown) {
            rollOnCooldown = true;
            currentRollCooldown = rollCooldown;
            boost = 1f;
        }

        movement = new Vector2(h, v).normalized;
        movement = movement * speed * dt * boost;
        playerRigidBody.AddForce(movement); 
    }
#endregion

#region update
    void Update() {
        DecrementRollTimer();
        // did player select a new potion?
        // did player roll?
        // did player fire?
        InputPotionSelection();
        InputFire();
    }

    void ReleaseFire() {
        switch (currentPotion) {
            case Potion.Blast:
                var newBlast = (GameObject) Instantiate(entities.thrownBlast, transform.position, Quaternion.identity);
                newBlast.GetComponent<BlastPotionController>().Init(blastInnerIndicator.transform.position);
                break;
        }

    }

    void InputFire() {
        if (Input.GetAxis("Fire") != 1 && firing) {
            ReleaseFire();
            firing = false;
        }
        firing = 1 == Input.GetAxis("Fire");
        BlastGuideCleanup(firing);
        if(firing) {
            switch(currentPotion) {
                case Potion.None:
                    break;
                case Potion.Blast:
                    blastHoldStrength += Time.deltaTime * blastWindupSpeed;
                    BlastGuidePaint();
                    break;
                default:
                    break;
            }
        }
    }

    void BlastGuideCleanup(bool firing) {
        if(!firing || currentPotion != Potion.Blast) {
            blastGuide.enabled = false;
            blastHoldStrength = 0f;
            Object.Destroy(blastOuterIndicator);
            Object.Destroy(blastInnerIndicator);
        }

    }
    void BlastGuidePaint() {
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
        localtarget = Vector3.Scale(localtarget, new Vector3(100,100,1));
        localtarget.z = transform.position.z;
        blastGuide.SetPosition(0, new Vector3(0f,0f,0f));
        blastGuide.SetPosition(1, localtarget);

        Vector2 indicatorPos = ((Vector2)localtarget.normalized) * blastHoldStrength;
        blastOuterIndicator.transform.localPosition = indicatorPos;
        blastInnerIndicator.transform.localPosition = indicatorPos;

    }

    void DecrementRollTimer() {
        currentRollCooldown -= Time.deltaTime;
        rollOnCooldown = currentRollCooldown > 0f;
        playerSpriteRenderer.color = rollOnCooldown ? colorRollOnCooldown : colorRollOffCooldown;
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
            game.ButtonDispatch(potionSelection);
            currentPotion = potionSelection;
        }
    }

#endregion

#region deadcode
    // void Turning() {
    //     Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     RaycastHit floorHit;
    //     if(Physics.Raycast(camRay, out floorHit, camRayLength, floorMask)) {
    //         Vector3 playerToMouse = floorHit.point - transform.position;
    //         playerToMouse.y = 0f;
    //         Quaternion newRotation = Quaternion.LookRotation(playerToMouse);
    //         // playerRigidBody.MoveRotation(newRotation);
    //     }
    // }

    // void Animating(float h, float v) {
    //     bool walking = h != 0f || v != 0f;
    //     anim.SetBool("IsWalking", walking);
    // }

#endregion
}
