// using UnityEngine;
// using UnityEngine.InputSystem;

// namespace NC
// {
//     public class PlayerCombat : MonoBehaviour
//     {
//         // public Animator animator;
        
//         public PlayerManager player;
//         public CharacterAnimatorManager characterAnimatorManager;
//         public Transform attackPoint; // Un objeto vacío frente al jugador
//         public float attackRange = 0.5f; // Radio del golpe
//         public LayerMask enemyLayers; // Para no pegarle al suelo o a ti mismo
//         public int attackDamage = 20;

//         void Update()
//         {
//             if (Mouse.current.leftButton.wasPressedThisFrame) // Clic izquierdo por defecto
//             {
//                 Debug.Log("attacking");
//                 Attack();
//             }
//         }

//         void Awake()
//         {
//             player = GetComponent<PlayerManager>();
//             characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
//         }

//         public void Attack()
//         {
//             // 1. Reproducir animación
//             // player.playerAnimatorManager.PlayTargetActionAnimation("Attack", true, true);
//             // player.playerAnimatorManager.PlayTargetActionAnimation("Attack", true, false);
//             // player.playerAnimatorManager.PlayTargetActionAnimation("Attack", false, false);
//             // player.playerAnimatorManager.PlayTargetActionAnimation("Attack", false, true);

//             player.playerAnimatorManager.PlayTargetActionAnimationTrigger("Attack", true, true);

//             Debug.Log("after hit animation call");

//             // 2. Detectar enemigos en el rango
//             // Crea una esfera invisible y guarda lo que toque en un array
//             Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

//             // 3. Aplicar daño a cada enemigo detectado
//             foreach (Collider enemy in hitEnemies)
//             {
//                 Debug.Log("Golpeaste a: " + enemy.name);
                
//                 // Aquí llamamos a una función en el script del enemigo
//                 if (enemy.GetComponent<EnemyHealth>() != null) {
//                     enemy.GetComponent<EnemyHealth>().TakeDamage(attackDamage);
//                 }
//             }
//         }

//         // Para poder ver el rango de ataque en el editor de Unity
//         // void OnDrawGizmosSelected()
//         // {
//         //     if (attackPoint == null) return;
//         //     Gizmos.DrawWireSphere(attackPoint.position, attackRange);
//         // }
//     }
// }
