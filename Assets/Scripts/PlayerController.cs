using UnityEngine;

public class PlayerController : MonoBehaviour {

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
    GameController gm;

    // GameController gameManager;


    // Animator anim;
    // int floorMask;

    void Awake() {
        // floorMask = LayerMask.GetMask("Floor");
        // anim = GetComponent<Animator>();
        gm = GameObject.Find("GameManager").GetComponent<GameController>();

        playerRigidBody = GetComponent<Rigidbody2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerSpriteRenderer.color = colorRollOffCooldown;
        colorRollOnCooldown = colorRollOffCooldown * new Color(1,1,1,0.5f);
        rollOnCooldown = false;
        currentRollCooldown = 0f;
    }

    void Update() {
        DecrementRollTimer();
        ButtonInput();
    }

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

    void DecrementRollTimer() {
        currentRollCooldown -= Time.deltaTime;
        rollOnCooldown = currentRollCooldown > 0f;
        playerSpriteRenderer.color = rollOnCooldown ? colorRollOnCooldown : colorRollOffCooldown;
    }

    void ButtonInput() {
        Potion buttonDispatch = Potion.None;

        if (Input.GetButtonDown("selectBlast")) {
            buttonDispatch = Potion.Blast;
        }
        else if (Input.GetButtonDown("selectQuick")) {
            buttonDispatch = Potion.Quick;
        }
        else if (Input.GetButtonDown("selectSpine")) {
            buttonDispatch = Potion.Spine;
        }
        else if (Input.GetButtonDown("selectVenom")) {
            buttonDispatch = Potion.Venom;
        }

        if (buttonDispatch != Potion.None) {
            gm.ButtonDispatch(buttonDispatch);
        }
    }

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
}
