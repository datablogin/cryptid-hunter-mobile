using UnityEngine;
using UnityEngine.AI;

namespace CryptidHunter.Cryptids
{
    public enum CryptidType
    {
        Bigfoot,
        Mothman,
        Chupacabra,
        JerseyDevil,
        Wendigo
    }
    
    public enum CryptidBehaviorState
    {
        Idle,
        Wandering,
        Investigating,
        Fleeing,
        Aggressive
    }
    
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class CryptidBase : MonoBehaviour
    {
        [Header("Cryptid Info")]
        [SerializeField] protected CryptidType cryptidType;
        [SerializeField] protected string cryptidName;
        [SerializeField] protected int dangerLevel = 1;
        
        [Header("Stats")]
        [SerializeField] protected float health = 100f;
        [SerializeField] protected float speed = 3.5f;
        [SerializeField] protected float detectionRange = 20f;
        [SerializeField] protected float fleeRange = 30f;
        
        [Header("Behavior")]
        [SerializeField] protected CryptidBehaviorState currentState = CryptidBehaviorState.Idle;
        [SerializeField] protected float stateChangeInterval = 5f;
        
        protected NavMeshAgent agent;
        protected Transform player;
        protected float stateTimer;
        protected Vector3 wanderTarget;
        
        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.speed = speed;
        }
        
        protected virtual void Start()
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
            stateTimer = stateChangeInterval;
        }
        
        protected virtual void Update()
        {
            UpdateBehavior();
            CheckPlayerDistance();
        }
        
        protected virtual void UpdateBehavior()
        {
            stateTimer -= Time.deltaTime;
            
            switch (currentState)
            {
                case CryptidBehaviorState.Idle:
                    HandleIdleState();
                    break;
                case CryptidBehaviorState.Wandering:
                    HandleWanderingState();
                    break;
                case CryptidBehaviorState.Investigating:
                    HandleInvestigatingState();
                    break;
                case CryptidBehaviorState.Fleeing:
                    HandleFleeingState();
                    break;
                case CryptidBehaviorState.Aggressive:
                    HandleAggressiveState();
                    break;
            }
            
            if (stateTimer <= 0)
            {
                DecideNextState();
                stateTimer = stateChangeInterval;
            }
        }
        
        protected virtual void CheckPlayerDistance()
        {
            if (player == null) return;
            
            float distance = Vector3.Distance(transform.position, player.position);
            
            if (distance < fleeRange && currentState != CryptidBehaviorState.Fleeing)
            {
                ChangeState(CryptidBehaviorState.Fleeing);
            }
        }
        
        protected virtual void HandleIdleState()
        {
            agent.isStopped = true;
        }
        
        protected virtual void HandleWanderingState()
        {
            if (!agent.hasPath || agent.remainingDistance < 0.5f)
            {
                wanderTarget = GetRandomWanderPoint();
                agent.SetDestination(wanderTarget);
            }
        }
        
        protected virtual void HandleInvestigatingState()
        {
            if (player != null)
            {
                Vector3 investigatePos = GetInvestigatePosition(player.position);
                agent.SetDestination(investigatePos);
            }
        }
        
        protected virtual void HandleFleeingState()
        {
            if (player != null)
            {
                Vector3 fleeDirection = (transform.position - player.position).normalized;
                Vector3 fleePosition = transform.position + fleeDirection * 10f;
                agent.SetDestination(fleePosition);
                agent.speed = speed * 1.5f;
            }
        }
        
        protected virtual void HandleAggressiveState()
        {
            if (player != null)
            {
                agent.SetDestination(player.position);
                agent.speed = speed * 1.2f;
            }
        }
        
        protected virtual void DecideNextState()
        {
            float random = Random.Range(0f, 1f);
            
            if (random < 0.3f)
                ChangeState(CryptidBehaviorState.Idle);
            else if (random < 0.7f)
                ChangeState(CryptidBehaviorState.Wandering);
            else
                ChangeState(CryptidBehaviorState.Investigating);
        }
        
        protected virtual void ChangeState(CryptidBehaviorState newState)
        {
            currentState = newState;
            agent.speed = speed;
            agent.isStopped = false;
        }
        
        protected Vector3 GetRandomWanderPoint()
        {
            Vector3 randomDirection = Random.insideUnitSphere * 20f;
            randomDirection += transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, 20f, 1);
            return hit.position;
        }
        
        protected Vector3 GetInvestigatePosition(Vector3 target)
        {
            Vector3 direction = (target - transform.position).normalized;
            float investigateDistance = Random.Range(5f, 15f);
            return target - direction * investigateDistance;
        }
        
        public virtual void TakeDamage(float damage)
        {
            health -= damage;
            if (health <= 0)
            {
                Die();
            }
            else
            {
                ChangeState(CryptidBehaviorState.Fleeing);
            }
        }
        
        protected virtual void Die()
        {
            Destroy(gameObject);
        }
        
        public CryptidType GetCryptidType() => cryptidType;
        public string GetCryptidName() => cryptidName;
        public int GetDangerLevel() => dangerLevel;
    }
}