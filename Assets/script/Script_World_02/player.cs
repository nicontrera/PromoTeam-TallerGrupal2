using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    public Rigidbody rb;
    private float _velocida = 5;
    

    void OnMove(InputValue inputValue)
    {
        var direction = inputValue.Get<Vector2>();
        transform.position += new Vector3(direction.x, 0, direction.y) * Time.deltaTime * _velocida;

    }
    
}
