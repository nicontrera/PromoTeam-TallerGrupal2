using UnityEngine;

public class Escudos : MonoBehaviour
{
    public int coleccion = 0;
    private void OnTriggerEnter(Collider other)

    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject, 2f);
            coleccion++;
        }
    }
}