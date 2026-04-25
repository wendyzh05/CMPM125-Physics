using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CoinManager.instance.AddCoins(value);
            Destroy(gameObject);
        }
    }
}