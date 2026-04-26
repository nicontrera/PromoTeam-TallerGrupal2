using System.Collections.Generic;
using UnityEngine;

public class DetectPlayer : MonoBehaviour
{
    private List<GameObject> _playersEnRango = new List<GameObject>();
    [SerializeField] private enemigo1 _enemigo;//tomo el script enemigo1 y se lo asigno a la variable _enemigo
    private float _distanciaMasCercana;
    GameObject playerMasCerca = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _playersEnRango.Add(other.gameObject);
            _distanciaMasCercana = 1000;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            _playersEnRango.Remove(other.gameObject);
        }
    }

    private void Update()
    {
        foreach (GameObject playerEnRango in _playersEnRango)
        {
            float distanciaAEsteEnemigo = Vector3.Distance(transform.position, playerEnRango.gameObject.transform.position);
            if (_distanciaMasCercana > (distanciaAEsteEnemigo + 0.3f))
            {
                playerMasCerca = playerEnRango;
                _distanciaMasCercana = distanciaAEsteEnemigo;
                _enemigo.SetPlayerToPerseguir(playerMasCerca);
            }
        }
    }
}