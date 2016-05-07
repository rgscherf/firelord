using UnityEngine;

public class HealthController : MonoBehaviour {

    public int health;

    public void ReceiveDamage(int debitamt) {
        health -= debitamt;
        if (health <= 0) {
            Object.Destroy(gameObject);
        }
    }
}
