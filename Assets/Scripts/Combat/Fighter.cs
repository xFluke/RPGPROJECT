using UnityEngine;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using GameDevTV.Utils;
using System;

namespace RPG.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig currentWeaponConfig;
        LazyValue<Weapon> currentWeapon;


        private void Awake() {
            currentWeaponConfig = defaultWeapon;
            currentWeapon = new LazyValue<Weapon>(InitializeDefaultWeapon);
        }

        private Weapon InitializeDefaultWeapon() {
            return AttachWeapon(defaultWeapon); 
        }

        private void Start() {
            currentWeapon.ForceInit();
        }

        private void Update() {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead()) return;

            if (!GetIsInRange()) {
                GetComponent<Mover>().MoveTo(target.transform.position, 1.0f);
            }
            else {
                GetComponent<Mover>().Cancel();
                AttackBehavior();
            }
        }

        private void AttackBehavior() {
            transform.LookAt(target.transform);

            if (timeSinceLastAttack > timeBetweenAttacks) {
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        public void EquipWeapon(WeaponConfig weapon) {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);
        }

        private Weapon AttachWeapon(WeaponConfig weapon) {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        private void TriggerAttack() {
            GetComponent<Animator>().ResetTrigger("Stop Attack");
            GetComponent<Animator>().SetTrigger("Attack");
        }

        public Health GetTarget() {
            return target;
        }

        private bool GetIsInRange() {
            return Vector3.Distance(transform.position, target.transform.position) < currentWeaponConfig.GetRange();
        }

        public bool CanAttack(GameObject combatTarget) {
            if (combatTarget == null) return false;

            Health targetToTest = combatTarget.GetComponent<Health>();
            return targetToTest != null && !targetToTest.IsDead();
        }

        public void Attack(GameObject combatTarget) {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.transform.GetComponent<Health>();
        }

        public void Cancel() {
            StopAttack();
            target = null;
            GetComponent<Mover>().Cancel();
        }

        private void StopAttack() {
            GetComponent<Animator>().ResetTrigger("Attack");
            GetComponent<Animator>().SetTrigger("Stop Attack");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat) {
            if (stat == Stat.Damage) {
                yield return currentWeaponConfig.GetDamage();
            }
        }
        public IEnumerable<float> GetPercentageModifiers(Stat stat) {
            if (stat == Stat.Damage) {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }

        // Animation Event
        void Hit() {
            if (target == null) { return; }

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);

            if (currentWeapon.value != null) {
                currentWeapon.value.OnHit();
            }

            if (currentWeaponConfig.HasProjectile()) {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            }
            else {
                target?.TakeDamage(gameObject, damage);
            }
        }

        void Shoot() {
            Hit();
        }

        public object CaptureState() {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state) {
            string weaponName = (string)state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(weaponName);
            EquipWeapon(weapon);
        }

        
    }
}
