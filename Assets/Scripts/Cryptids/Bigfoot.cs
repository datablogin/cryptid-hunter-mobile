using UnityEngine;

namespace CryptidHunter.Cryptids
{
    public class Bigfoot : CryptidBase
    {
        [Header("Bigfoot Specific")]
        [SerializeField] private float roarCooldown = 30f;
        [SerializeField] private float roarRadius = 50f;
        [SerializeField] private AudioClip roarSound;
        
        private AudioSource audioSource;
        private float roarTimer;
        
        protected override void Awake()
        {
            base.Awake();
            
            cryptidType = CryptidType.Bigfoot;
            cryptidName = "Sasquatch";
            dangerLevel = 3;
            
            health = 150f;
            speed = 4f;
            detectionRange = 25f;
            fleeRange = 15f;
            
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
        
        protected override void Start()
        {
            base.Start();
            roarTimer = roarCooldown;
        }
        
        protected override void Update()
        {
            base.Update();
            
            roarTimer -= Time.deltaTime;
            if (roarTimer <= 0)
            {
                PerformRoar();
                roarTimer = roarCooldown;
            }
        }
        
        protected override void DecideNextState()
        {
            if (player == null)
            {
                base.DecideNextState();
                return;
            }
            
            float distance = Vector3.Distance(transform.position, player.position);
            float random = Random.Range(0f, 1f);
            
            if (distance < 10f)
            {
                ChangeState(CryptidBehaviorState.Aggressive);
            }
            else if (distance < fleeRange)
            {
                if (random < 0.6f)
                    ChangeState(CryptidBehaviorState.Fleeing);
                else
                    ChangeState(CryptidBehaviorState.Aggressive);
            }
            else if (distance < detectionRange)
            {
                ChangeState(CryptidBehaviorState.Investigating);
            }
            else
            {
                base.DecideNextState();
            }
        }
        
        protected override void HandleAggressiveState()
        {
            base.HandleAggressiveState();
            
            if (player != null && Vector3.Distance(transform.position, player.position) < 3f)
            {
                PerformAttack();
            }
        }
        
        private void PerformRoar()
        {
            if (roarSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(roarSound);
            }
            
            Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, roarRadius);
            foreach (Collider col in nearbyObjects)
            {
                if (col.CompareTag("Player"))
                {
                    Debug.Log("Player heard Bigfoot roar!");
                }
            }
        }
        
        private void PerformAttack()
        {
            Debug.Log("Bigfoot attacks!");
        }
        
        protected override void Die()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.RegisterCryptidCapture(this);
            }
            
            base.Die();
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, fleeRange);
            
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, roarRadius);
        }
    }
}