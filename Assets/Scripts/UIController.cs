using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    Image health1;
    Image health2;
    Image health3;
    HealthController playerHealth;

	// Use this for initialization
	void Start () {
        GameObject.Find("blast-image").GetComponent<Image>().color = PotionColors.Blast;
        GameObject.Find("quick-image").GetComponent<Image>().color = PotionColors.Quick;
        GameObject.Find("spine-image").GetComponent<Image>().color = PotionColors.Spine;
        GameObject.Find("venom-image").GetComponent<Image>().color = PotionColors.Venom;

        health1 = GameObject.Find("health1").GetComponent<Image>();
        health2 = GameObject.Find("health2").GetComponent<Image>();
        health3 = GameObject.Find("health3").GetComponent<Image>();

        playerHealth = GameObject.Find("Player").GetComponent<HealthController>();
	}
	
	// Update is called once per frame
	void Update () {
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
