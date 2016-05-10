using UnityEngine;

public static class PotionColors {

    // color constants for potion types
    public static Color Blast = new Color(1f, 0.149f, 0f, 1);
    public static Color Quick = new Color(0.999f, 0.978f, 0.378f, 1);
    public static Color Spine = new Color(0.171f, 0.847f, 0.346f, 1);
    public static Color Venom = new Color(0.704f, 0.317f, 0.896f, 1);
    
    public static Color Mist = new Color(0.291f, 0.728f, 0.985f, 1);
    public static Color MistFade = new Color(0.291f, 0.728f, 0.985f, 0.8f);
    public static Color MistFadeTwo = new Color(0.291f, 0.728f, 0.985f, 0.9f);

    public static Color Danger = new Color(1f, 0.632f, 0f, 1f);

    // basic color

    public static Color Gray = new Color(0.5f,0.5f,0.5f,1f);
    public static Color White = new Color(1f,1f,1f,1f);
    // public static Color White = new Color(0.75f,0.75f,0.75f,1f);

    // fading for UI potion selection
    public static Color UIFade = new Color(0.5f,0.5f,0.5f,1f);

    public static Color GetColor(Potion dispatch) {
        switch (dispatch) {
            case Potion.Blast:
                return PotionColors.Blast;
            case Potion.Quick:
                return PotionColors.Quick;
            case Potion.Spine:
                return PotionColors.Spine;
            case Potion.Venom:
                return PotionColors.Venom;
            case Potion.None:
                return PotionColors.Gray;
            default:
                return PotionColors.White;
        }
    }
}
