using GameDevTV.Utils;
using RPG.Combat;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;using System;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float regenerationPercentage = 100f;
        [SerializeField] UnityEvent<float> takeDamage;
        [SerializeField] UnityEvent onDie;

        LazyValue<float> healthPoints;

        bool isDead = false;

        private void Awake() {
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        private float GetInitialHealth() {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Start() {
            healthPoints.ForceInit();
        }

        private void OnEnable() {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable() {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;

        }

        public bool IsDead() {
            return isDead;
        }

        public void TakeDamage(GameObject instigator, float damage) {
            healthPoints.value = Mathf.Max(healthPoints.value - damage, 0);
            
            if (healthPoints.value == 0) {
                onDie.Invoke();
                Die();
                AwardExperience(instigator);
            }
            else {
                takeDamage.Invoke(damage);
            }
        }

        public float GetHealthPoints() {
            return healthPoints.value;
        }

        public float GetMaxHealthPoints() {
            return GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetPercentage() {
            return 100 * GetFraction();
        }

        public float GetFraction() {
            return healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        private void Die() {
            if (isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("Die");
            GetComponent<ActionScheduler>().CancelCurrentAction();

            Destroy(GetComponent<CombatTarget>());
        }

        private void AwardExperience(GameObject instigator) {
            Experience experience = instigator.GetComponent<Experience>();

            if (experience == null) return;

            experience.GainExperience(GetComponent<BaseStats>().GetStat(Stat.ExperienceReward));
        }

        private void RegenerateHealth() {
            //float regenHealthPoints = GetComponent<BaseStats>().GetStat(Stat.Health) * (regenerationPercentage / 100);
            healthPoints.value = GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public object CaptureState() {
            return healthPoints.value;
        }

        public void RestoreState(object state) {
            healthPoints.value = (float)state;

            if (healthPoints.value <= 0) {
                Die();
            }
        }
    }
}