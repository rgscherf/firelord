using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    Image health1;
    Image health2;
    Image health3;
    HealthController playerHealth;
    PlayerController playerController;

    Image BlastImage;
    Image QuickImage;
    Image SpineImage;
    Image VenomImage;

	// Use this for initialization
	void Start () {
        BlastImage = GameObject.Find("blast-image").GetComponent<Image>();
        QuickImage = GameObject.Find("quick-image").GetComponent<Image>();
        SpineImage = GameObject.Find("spine-image").GetComponent<Image>();
        VenomImage = GameObject.Find("venom-image").GetComponent<Image>();

        BlastImage.color = PotionColors.Blast;
        QuickImage.color = PotionColors.Quick;
        SpineImage.color = PotionColors.Spine;
        VenomImage.color = PotionColors.Venom;

        health1 = GameObject.Find("health1").GetComponent<Image>();
        health2 = GameObject.Find("health2").GetComponent<Image>();
        health3 = GameObject.Find("health3").GetComponent<Image>();

        playerHealth = GameObject.Find("Player").GetComponent<HealthController>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
	}
	
	// Update is called once per frame
	void Update () {
        RenderPlayerHealth(); 
        RenderPotionCooldown();
	}

    void RenderPotionCooldown() {
        BlastImage.color = playerController.blastCooldownCurrent > playerController.blastCooldown ? PotionColors.Blast : PotionColors.Blast * new Color(1f,1f,1f,0.25f);
        QuickImage.color = playerController.quickCooldownCurrent > playerController.quickCooldown ? PotionColors.Quick : PotionColors.Quick * new Color(1f,1f,1f,0.25f);
        SpineImage.color = playerController.spineCooldownCurrent > playerController.spineCooldown ? PotionColors.Spine : PotionColors.Spine * new Color(1f,1f,1f,0.25f);
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
