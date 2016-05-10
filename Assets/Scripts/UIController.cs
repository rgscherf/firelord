using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    Image health1;
    Image health2;
    Image health3;
    HealthController playerHealth;
    PlayerController playerController;

    float cameraShakeAmt = 0.075f;
    Camera maincamera;
    Vector3 cameraOriginalPos;

    Image BlastImage;
    Image QuickImage;
    Image SpineImage;
    Image VenomImage;

    Text Blastammo;
    Text Quickammo;
    Text Spineammo;
    Text Venomammo;

    SpriteRenderer background;

    public GameObject _playerDeathUI;
    GameObject playerDeathUI;

    float animationTime;
    float animationTimeCurrent;
    bool DamageAnimationFirstFrame;
    bool DamageAnimation;

    Potion lastPotion;


    void Awake () {
        playerHealth = GameObject.Find("Player").GetComponent<HealthController>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
    }

	void Start () {
        BlastImage = GameObject.Find("blast-image").GetComponent<Image>();
        QuickImage = GameObject.Find("quick-image").GetComponent<Image>();
        SpineImage = GameObject.Find("spine-image").GetComponent<Image>();
        VenomImage = GameObject.Find("venom-image").GetComponent<Image>();

        Blastammo = GameObject.Find("blast-num").GetComponent<Text>();
        Quickammo = GameObject.Find("quick-num").GetComponent<Text>();
        Spineammo = GameObject.Find("spine-num").GetComponent<Text>();
        Venomammo = GameObject.Find("venom-num").GetComponent<Text>();

        BlastImage.color = PotionColors.Blast;
        QuickImage.color = PotionColors.Quick;
        SpineImage.color = PotionColors.Spine;
        VenomImage.color = PotionColors.Venom;

        health1 = GameObject.Find("health1").GetComponent<Image>();
        health2 = GameObject.Find("health2").GetComponent<Image>();
        health3 = GameObject.Find("health3").GetComponent<Image>();

        // _playerDeathUI = GameObject.Find("death");

        maincamera = Camera.main;

        background = GameObject.Find("spritebackground").GetComponent<SpriteRenderer>();

        animationTime = playerHealth.invulntimer;

        lastPotion = Potion.None;
	}


    public void AnimateDamage() {
        DamageAnimationFirstFrame = true;
        DamageAnimation = true;
    }
	
    public void ChangePlayerDeathState(bool playerIsDead) {
        if (playerIsDead) {
            playerDeathUI = (GameObject) Instantiate(_playerDeathUI);
            var canvas = GameObject.Find("UICanvas");
            playerDeathUI.transform.SetParent(canvas.transform, false);
        } else {
            Object.Destroy(playerDeathUI);
        }
    }

	// Update is called once per frame
	void Update () {
        RenderPlayerHealth(); 
        RenderPotionCooldown();
        RenderPotionAmmo();

        if(DamageAnimationFirstFrame) {
            background.color = PotionColors.White;
            DamageAnimationFirstFrame = false;
            cameraOriginalPos = maincamera.transform.position;
        }
        if(DamageAnimation) {
            animationTimeCurrent += Time.deltaTime;
            float colorCo = (1 - (animationTimeCurrent / animationTime)) * 0.3f;
            Color[] animationColors = new[] {PotionColors.Danger * new Color(colorCo, colorCo, colorCo, 1f), Color.black};
            background.color = animationColors[Random.Range(0, animationColors.Length)];

            if(animationTimeCurrent < 0.5f) {
                var rand = new Vector3(Random.Range(-cameraShakeAmt, cameraShakeAmt), Random.Range(-cameraShakeAmt, cameraShakeAmt), 0f);
                maincamera.transform.position += (Vector3) rand;
            }
            if (animationTimeCurrent > animationTime) {
                maincamera.transform.position = cameraOriginalPos;
                DamageAnimation = false;
                animationTimeCurrent = 0f;
            }
        }

	}

    void RenderPotionAmmo() {
        Blastammo.text = playerController.blastammo.ToString();
        Quickammo.text = playerController.quickammo.ToString();
        Spineammo.text = playerController.spineammo.ToString();
        Venomammo.text = playerController.venomammo.ToString();
    }

    void RenderPotionCooldown() {
        BlastImage.color = playerController.blastCooldownCurrent < playerController.blastCooldown || playerController.blastammo == 0 ? PotionColors.Gray : PotionColors.Blast;
        QuickImage.color = playerController.quickCooldownCurrent < playerController.quickCooldown || playerController.quickammo == 0 ? PotionColors.Gray : PotionColors.Quick;
        SpineImage.color = playerController.spineCooldownCurrent < playerController.spineCooldown || playerController.spineammo == 0 ? PotionColors.Gray : PotionColors.Spine;
        VenomImage.color = playerController.venomCooldownCurrent < playerController.venomCooldown || playerController.venomammo == 0 ? PotionColors.Gray : PotionColors.Venom;
    }

    public void UpdateRoom() {
        PotionSwap(playerController.currentPotion, playerController.AmmoCount(playerController.currentPotion));
    }

    public void PotionSwap(Potion dispatch, int ammo) {
        if (ammo == 0 || dispatch == Potion.None) { 
            dispatch = Potion.None; 
        }
        var activeGeometry = GameObject.FindGameObjectsWithTag("Geometry");
        foreach (var g in activeGeometry) {
            var geo = g.GetComponent<GeometryController>();
            geo.ColorSwap(dispatch);
        }
    }

    void RenderPlayerHealth() {
        switch (playerHealth.health) {
            case 6:
                health1.enabled = true;
                health1.color = PotionColors.White;
                health2.enabled = true;
                health2.color = PotionColors.White;
                health3.enabled = true;
                health1.color = PotionColors.White;
                break;

            case 5:
                health1.enabled = true;
                health1.color = PotionColors.White * new Color(1,1,1,0.5f);
                health2.enabled = true;
                health2.color = PotionColors.White;
                health3.enabled = true;
                health3.color = PotionColors.White;
                break;

            case 4:
                health1.enabled = false;
                health1.color = PotionColors.White;

                health2.enabled = true;
                health2.color = PotionColors.White;
                health3.enabled = true;
                health3.color = PotionColors.White;
                break;

            case 3:
                health1.enabled = false;
                health1.color = PotionColors.White;

                health2.enabled = true;
                health2.color = PotionColors.White * new Color(1,1,1,0.5f);
                health3.enabled = true;
                health3.color = PotionColors.White;
                break;

            case 2:
                health1.enabled = false;
                health1.color = PotionColors.White;
                health2.enabled = false;
                health2.color = PotionColors.White;

                health3.enabled = true;
                health3.color = PotionColors.White;
                break;

            case 1:
                health1.enabled = false;
                health1.color = PotionColors.White;
                health2.enabled = false;
                health2.color = PotionColors.White;
                
                health3.enabled = true;
                health3.color = PotionColors.White * new Color(1,1,1,0.5f);
                break;

            default:
                health1.enabled = false;
                health1.color = PotionColors.White;
                health2.enabled = false;
                health2.color = PotionColors.White;
                health3.enabled = false;
                health3.color = PotionColors.White;
                break;
            }
        }
}
