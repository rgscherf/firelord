using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject.Find("blast-image").GetComponent<Image>().color = PotionColors.Blast;
        GameObject.Find("quick-image").GetComponent<Image>().color = PotionColors.Quick;
        GameObject.Find("spine-image").GetComponent<Image>().color = PotionColors.Spine;
        GameObject.Find("venom-image").GetComponent<Image>().color = PotionColors.Venom;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
