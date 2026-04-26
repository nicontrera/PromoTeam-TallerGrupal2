using UnityEngine;

public class enemigo1 : MonoBehaviour
{
    private Rigidbody _rb; // referencia al componente Rigidbody de la rata
    private Vector3 _objetivo;
    private float _velocidad = 5f;
    private GameObject playerAPerseguir;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>(); 
        InvokeRepeating("PersigoPlayer", 0f, 3f);
        _objetivo = _rb.transform.position;
    }

    public void SetPlayerToPerseguir(GameObject playerCerca) 
    {
        Debug.Log("viene de detectar un enemigo" + playerCerca);
        playerAPerseguir = playerCerca;
    }

    private void FixedUpdate()
    {
        if (_objetivo == null) return;
        Vector3 nuevaPosicion = Vector3.MoveTowards(_rb.position, _objetivo, _velocidad * Time.fixedDeltaTime);
        _rb.MovePosition(nuevaPosicion);
        Vector3 direccion = (_objetivo - transform.position).normalized;
        if (direccion != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(new Vector3(direccion.x, 0, direccion.z));
            _rb.MoveRotation(Quaternion.Slerp(transform.rotation, rotacionObjetivo, 0.2f));
        }
    }

    void PersigoPlayer()
    {
        if (playerAPerseguir != null)
        {
            _objetivo = playerAPerseguir.transform.position;
        }
    }
}

