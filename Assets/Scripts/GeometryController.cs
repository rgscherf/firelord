using UnityEngine;

public class GeometryController : MapObject {

    SpriteRenderer spr;

    Color offset;

    void Awake () {
        spr = gameObject.GetComponent<SpriteRenderer>();
        spr.color = PotionColors.White;

        offset = new Color (0.85f, 0.85f, 0.85f, 1);
    }

    public virtual void ColorSwap(Potion dispatch) {
        spr.color = PotionColors.GetColor(dispatch) * offset;
    }

    public void Electrify(GameObject inst, float chance) {
        
    }
}
