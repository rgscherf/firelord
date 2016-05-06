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

    LineRenderer blastGuide;

    Potion currentPotion;

    void Awake() {
        game = GameObject.Find("GameManager").GetComponent<GameController>();
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerSpriteRenderer.color = colorRollOffCooldown;
        currentPotion = Potion.None;

        blastGuide = gameObject.AddComponent<LineRenderer>();
        blastGuide.material = new Material (Shader.Find("Particles/Additive"));
        blastGuide.SetWidth(1f, 1f);
        blastGuide.SetVertexCount(2);

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

    void InputFire() {
        bool firing = 1 == Input.GetAxis("Fire");
        if(firing) {
            switch(currentPotion) {
                case Potion.None:
                    break;
                case Potion.Blast:
                    PaintBlastGuide();
                    break;
                default:
                    break;
            }
        }
    }

    void PaintBlastGuide() {
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        target.z = transform.position.z;
        blastGuide.SetPosition(0, transform.position);
        blastGuide.SetPosition(1, target);

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
