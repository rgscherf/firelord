using UnityEngine;

public class GeometryController : MapObject {

    SpriteRenderer spr;

    void Awake () {
        spr = gameObject.GetComponent<SpriteRenderer>();
        spr.color = PotionColors.White;
    }

    public virtual void ColorSwap(Potion dispatch) {
        spr.color = PotionColors.GetColor(dispatch);
    }
}
