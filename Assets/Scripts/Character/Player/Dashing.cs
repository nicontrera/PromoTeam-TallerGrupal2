// using System.Collections;
// using NC;
// using UnityEngine;

// public class Dashing : MonoBehaviour
// {
//     PlayerManager playerManager;
//     public float dashSpeed;
//     public float dashTime;


//     // Start is called once before the first execution of Update after the MonoBehaviour is created
//     void Start()
//     {
//         playerManager = GetComponent<PlayerManager>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.R))
//         {
//             StartCoroutine(Dash());
//         }
//     }

//     IEnumerator Dash()
//     {
//         float startTime = Time.time;

//         Vector3 rollDirection;

//         rollDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
//         rollDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
//         rollDirection.y = 0;
//         rollDirection.Normalize();

//         while(Time.time < startTime + dashTime)
//         {
//             playerManager.characterController.Move()
//         }
//     }
// }
