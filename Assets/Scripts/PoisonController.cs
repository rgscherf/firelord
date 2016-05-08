using System.Collections;

public class PoisonController {
    // damage per tick of poison
    const int poisonDamage = 1;

    // number of ticks total
    const int totalPoisonTicks = 3;

    // amount of time per tick
    const float poisonTickTimer = 1.5f;

    int poisonTickCurrent = 0;
    float poisonTickTimerCurrent = 0;


    public PoisonController() {
    }

    public int Tick(float dt) {
        int ret = 0;
        poisonTickTimerCurrent += dt;

        // have we done enough ticks of poison damage?
        if (poisonTickCurrent == totalPoisonTicks) {
            return 99;
        }

        // have we ticked over on this frame?
        if (poisonTickTimerCurrent > poisonTickTimer) {
            poisonTickCurrent++;
            poisonTickTimerCurrent = 0f;
            ret = 1;
        } 

        // 99 signals that the poison is over
        return ret;
    }
}
