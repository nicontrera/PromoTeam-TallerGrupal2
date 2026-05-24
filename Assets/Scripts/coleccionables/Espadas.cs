using UnityEngine;

public class Espadas : MonoBehaviour
{
    [SerializeField] private GameManager _gamemanager;


    public int coleccion = 0;
    private void OnTriggerEnter(Collider other)

    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject, 0.5f);
            _gamemanager._espadaCantidad++;
        }
    }
}
