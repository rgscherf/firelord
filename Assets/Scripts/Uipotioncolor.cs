using UnityEngine;
using UnityEngine.UI;

public class Uipotioncolor : MonoBehaviour {

    public string potionType;

	// Use this for initialization
	void Start () {
        Image img = GetComponent<Image>();
        img.color = ImageColor(potionType);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    Color ImageColor(string col) {
        switch (col) {
            case "blast":
                return PotionColors.Blast;
            case "quick":
                return PotionColors.Quick;
            case "spine":
                return PotionColors.Spine;
            case "venom":
                return PotionColors.Venom;
            default:
                return PotionColors.White;
        }
    }
}
