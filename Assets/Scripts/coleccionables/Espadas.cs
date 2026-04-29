using UnityEngine;

public class Espadas : MonoBehaviour
{
    public int coleccion = 0;
    private void OnTriggerEnter(Collider other)

    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject, 0.5f);
            coleccion++;
        }
    }
}
