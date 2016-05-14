using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    bool showTutorials = true;

    Image health1;
    Image health2;
    Image health3;
    HealthController playerHealth;
    PlayerController playerController;
    GameController game;

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

    Text potionHelpText;

    GameObject tutroom1;
    GameObject tutroom2;
    GameObject tutroom3;
    GameObject tutroom4;
    GameObject[] tutrooms;

    GameObject stats;
    GameObject gameovertext;

    SpriteRenderer background;

    float animationTime;
    float animationTimeCurrent;
    bool damageAnimationFirstFrame;
    bool damageAnimation;
    bool flashHealth;

    Potion lastPotion;

    void Awake () {
        playerHealth = GameObject.Find("Player").GetComponent<HealthController>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        game = GameObject.Find("GameManager").GetComponent<GameController>();

        potionHelpText = GameObject.Find("potionHelpText").GetComponent<Text>();

        tutroom1 = transform.Find("UICanvas").Find("room1").gameObject;
        tutroom2 = transform.Find("UICanvas").Find("room2").gameObject;
        tutroom3 = transform.Find("UICanvas").Find("room3").gameObject;
        tutroom4 = transform.Find("UICanvas").Find("room4").gameObject;
        tutrooms = new[] {tutroom1, tutroom2, tutroom3, tutroom4};
        ClearTutoral();

        stats = transform.Find("UICanvas").Find("stats").gameObject;
        stats.SetActive(false);

        gameovertext = transform.Find("UICanvas").Find("gameover").gameObject;
        gameovertext.SetActive(false);
    }

    public void ClearTutoral() {
        foreach (var r in tutrooms) {
            r.SetActive(false);
        }
    }

    public void ShowStats(int room) {
        if (game.level < 2 || room != 1) { 
            stats.SetActive(false);
            return; 
        }
        stats.SetActive(true);
        string line1 = "Entering level " + game.level;
        string line2 = System.String.Format("Time last level: {0:F2}", game.lastLevelTime);
        string line3 = "Potions thrown last level: " + game.lastLevelThrown;
        string line4 = "Damage taken last level: " + game.lastLevelDamage;
        string combinedString = line1 + "\n" + line2 + "\n" + line3 + "\n" + line4;
        stats.GetComponent<Text>().text = combinedString;
    }

    public void SetRoom(int room) {
        if (showTutorials) {
            ClearTutoral();
            if (room == 5) { return; }
            tutrooms[room - 1].SetActive(true);
        }
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

        maincamera = Camera.main;

        background = GameObject.Find("spritebackground").GetComponent<SpriteRenderer>();

        animationTime = playerHealth.invulntimer;

        lastPotion = Potion.None;
	}

    public void AnimateHealth() {
        cameraOriginalPos = maincamera.transform.position;
        flashHealth = true;
    }

    public void AnimateDamage() {
        cameraOriginalPos = maincamera.transform.position;
        damageAnimationFirstFrame = true;
        damageAnimation = true;
        flashHealth = true;
    }
	
    public void ChangePlayerDeathState(bool playerIsDead) {
        if (playerIsDead) {
            gameovertext.SetActive(true);
            Text deathstats = gameovertext.transform.Find("deathtext").GetComponent<Text>();
            deathstats.text = System.String.Format("Your quest ended on level {0},\nhaving thrown {1} potions.\n\n A good death.\n\n\n\n\nPress R to restart.", game.level, game.lastLevelThrown + game.thisLevelThrown);
        } else {
            gameovertext.SetActive(false);
        }
    }

	// Update is called once per frame
	void Update () {
        RenderPlayerHealth(); 
        RenderPotionCooldown();
        RenderPotionAmmo();

        if(damageAnimationFirstFrame) {
            background.color = PotionColors.White;
            damageAnimationFirstFrame = false;
        }

        if (damageAnimation || flashHealth) {
            animationTimeCurrent += Time.deltaTime;
        }

        if(damageAnimation) {
            float colorCo = (1 - (animationTimeCurrent / animationTime)) * 0.3f;
            Color[] animationColors = new[] {PotionColors.Danger * new Color(colorCo, colorCo, colorCo, 1f), Color.black};
            background.color = animationColors[Random.Range(0, animationColors.Length)];

            if(animationTimeCurrent < 0.5f) {
                var rand = new Vector3(Random.Range(-cameraShakeAmt, cameraShakeAmt), Random.Range(-cameraShakeAmt, cameraShakeAmt), 0f);
                maincamera.transform.position += (Vector3) rand;
            }
        }

        if(flashHealth) {
            var colors = new[] { PotionColors.Blast, PotionColors.Quick, PotionColors.Spine, PotionColors.Venom };
            health1.color = colors[Random.Range(0, colors.Length)];
            health2.color = colors[Random.Range(0, colors.Length)];
            health3.color = colors[Random.Range(0, colors.Length)];
        }

        if (animationTimeCurrent > animationTime) {
            maincamera.transform.position = cameraOriginalPos;
            damageAnimation = false;
            flashHealth = false;
            animationTimeCurrent = 0f;
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
        UpdateHelpText(dispatch, ammo);
        if (ammo == 0 || dispatch == Potion.None) { 
            dispatch = Potion.None; 
        }
        var activeGeometry = GameObject.FindGameObjectsWithTag("Geometry");
        foreach (var g in activeGeometry) {
            var geo = g.GetComponent<GeometryController>();
            geo.ColorSwap(dispatch);
        }
    }

    void UpdateHelpText(Potion dispatch, int ammo) {
        string text;
        switch (dispatch) {
            case Potion.Blast:
                text = "Blast: Hold MOUSE1 to wind up. Inner ring damages, outer ring pushes.";
                break;
            case Potion.Quick:
                text = "Quick: Fast projectile. Static charge can chain to other enemies.";
                break;
            case Potion.Spine:
                text = "Spine: Sprout after delay. Greatly slow enemies. Damage at end of life.";
                break;
            case Potion.Venom:
                text = "Venom: Sicken many enemies. Steady damage over long period.";
                break;
            default:
                text = "";
                break;
        }
        if (potionHelpText == null) {
            potionHelpText = GameObject.Find("potionHelpText").GetComponent<Text>();
        }
        potionHelpText.text = text;
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
