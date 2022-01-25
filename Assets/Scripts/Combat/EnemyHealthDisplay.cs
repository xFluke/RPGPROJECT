using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Combat;

namespace RPG.Attributes
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        Fighter fighter;

        private void Awake() {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        private void Update() {
            if (fighter.GetTarget() == null) {
                GetComponent<Text>().text = "N/A";
                return;
            }

            Health health = fighter.GetTarget();
            GetComponent<Text>().text = string.Format("{0:0} / {1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }

    }
}
