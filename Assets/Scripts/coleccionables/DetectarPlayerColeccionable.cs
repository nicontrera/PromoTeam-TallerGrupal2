using UnityEngine;

public class DetectPlayerColeccionable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject, 2f);
            Debug.Log("Coleccionable recogido, se destruirá en 2 segundos.");
        }
    }
}