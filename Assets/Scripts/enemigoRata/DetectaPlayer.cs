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
        //Detectar si choque con player al entrar dentro del trigger
        if (other.gameObject.CompareTag("Player"))
        {
            _playersEnRango.Add(other.gameObject);
            //Debug.Log("dentro del trigger");
            _distanciaMasCercana = 1000; // pone un valor alto para que si o si encuentre alguno

        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Detectar si choque con player
        if (other.gameObject.CompareTag("Player"))
        {
            _playersEnRango.Remove(other.gameObject);
            //Debug.Log("fuera del trigger");

        }
    }

    private void Update()
    {
        //if (_playersEnRango.Count == 0) // verifica que la lista este vacia
        //{
        //}

        foreach (GameObject playerEnRango in _playersEnRango)
        {
            float distanciaAEsteEnemigo = Vector3.Distance(transform.position, playerEnRango.gameObject.transform.position);
            //Debug.Log("leyendo lista");

            if (_distanciaMasCercana > (distanciaAEsteEnemigo + 0.3f))
            {

                playerMasCerca = playerEnRango;
                _distanciaMasCercana = distanciaAEsteEnemigo;
                Debug.Log("playerMasCerca" + playerMasCerca);

                _enemigo.SetPlayerToPerseguir(playerMasCerca);// envia la orden para buscar el player que esta mas cercano
            }
        }
        //if (playerMasCerca != null)
        //{
            
        //}

    }

}