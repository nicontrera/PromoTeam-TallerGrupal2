using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{
    private Rigidbody _rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnMove(InputValue inputValue)
    {
        var direction = inputValue.Get<Vector2>();
        transform.position += new Vector3(direction.x, 0, direction.y) * Time.deltaTime * 5;
    }

}
