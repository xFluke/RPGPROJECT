using RPG.Combat;
using UnityEngine;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using GameDevTV.Utils;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float waypointDwellTime = 2f;
        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;

        Fighter fighter;
        Health health;
        Mover mover;
        GameObject player;
        bool attackPlayer;

        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;

        private void Awake() {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            player = GameObject.FindWithTag("Player");
            guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private void Start() {
            guardPosition.ForceInit();

            attackPlayer = !FindObjectOfType<PlayerController>().TestMode;
        }
        
        private Vector3 GetGuardPosition() {
            return transform.position;
        }

        private void Update() {
            if (health.IsDead()) return;

            if (InAttackRangeOfPlayer() && fighter.CanAttack(player)) {
                if (attackPlayer)
                    AttackBehavior();
            }
            else if (timeSinceLastSawPlayer < suspicionTime) {
                SuspicionBehavior();
            }
            else {
                PatrolBehavior();
            }

            UpdateTimers();
        }

        private void UpdateTimers() {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehavior() {
            Vector3 nextPosition = guardPosition.value;

            if (patrolPath != null) {
                if (AtWaypoint()) {
                    timeSinceArrivedAtWaypoint = 0f;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (timeSinceArrivedAtWaypoint > waypointDwellTime) {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }

        }

        private Vector3 GetCurrentWaypoint() {
            return patrolPath.GetWaypoint(currentWaypointIndex);        
        }

        private void CycleWaypoint() {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWaypoint() {
            return Vector3.Distance(transform.position, GetCurrentWaypoint()) < waypointTolerance;
        }

        private void SuspicionBehavior() {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehavior() {
            timeSinceLastSawPlayer = 0;
            fighter.Attack(player);
        }

        private bool InAttackRangeOfPlayer() {
            return Vector3.Distance(transform.position, player.transform.position) < chaseDistance;
        }

        private void OnDrawGizmosSelected() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
