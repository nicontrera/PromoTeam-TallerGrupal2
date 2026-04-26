using UnityEngine;

public class enemigo1 : MonoBehaviour
{
    private Rigidbody _rb; // referencia al componente Rigidbody de la rata
    //public Rigidbody rbPlayer; // referencia al componente Rigidbody del player
    private Vector3 objetivo;
    private float _velocidad = 5f;
    bool flag = false; // banderea para saber si la rata muerde
    //public player ScriptPlayer;
    private GameObject playerAPerseguir;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>(); // obtener el componente Rigidbody del objeto
        //InvokeRepeating("AnuloAtaque", 0f, 1f);// la fn InvokeRepeating es para ejecutar cada segundo (google)
        InvokeRepeating("PersigoPlayer", 0f, 3f);

    }

    public void SetPlayerToPerseguir(GameObject playerCerca) //viene del script DetectaPlayer
    {
        Debug.Log("viene de detectar un enemigo" + playerCerca);
        playerAPerseguir = playerCerca;
    }

    //private void Update()
    //{
        
    //}

    // Update is called once per frame
    //void FixedUpdate()
    //{
    //    // nada por ahora
    //}

    void PersigoPlayer()
    {
        if (playerAPerseguir != null)
        {
            objetivo = playerAPerseguir.transform.position;
            //if (flag != true)//verifica que no muerda
            //{
            Debug.Log("persiguiendo al player");
            // Mueve el objeto desde su posición actual hacia el destino a una velocidad constante (google)
            transform.position = Vector3.MoveTowards(transform.position, objetivo, _velocidad);
            //}
        }
        //if (playerAPerseguir != null)
        //{
        //    objetivo = playerAPerseguir.transform.position;
        //}
    }

    //void AnuloAtaque()
    //{
    //    //    if (ScriptPlayer._ataque) // if el player ataca
    //    //    {
    //    //        ScriptPlayer._ataque = false;
    //    //    }
    //}

//private void OnTriggerStay(Collider collider)//la rata alcanza al player
//    {
//        if (collider.gameObject.CompareTag("Player"))// if el tag == "Player" then muerde 
//        {
//            _rb.transform.position = new Vector3(collider.gameObject.transform.position.x + 0.5f, collider.gameObject.transform.position.y, collider.gameObject.transform.position.z + 0.5f);
//            //if (ScriptPlayer._ataque) // if el player me ataca then suelto y muero
//            //{
//                flag = false;
//                _rb.transform.position = new Vector3(collider.gameObject.transform.position.x + 1f, collider.gameObject.transform.position.y, collider.gameObject.transform.position.z + 1f);
//                Destroy(gameObject, 1f);
//            //}
//        }
//    }

}

