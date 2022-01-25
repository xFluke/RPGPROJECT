using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        BaseStats baseStats;

        private void OnEnable() {
            baseStats.onLevelUp += UpdateLevelDisplay;
        }

        private void OnDisable() {
            baseStats.onLevelUp += UpdateLevelDisplay;
        }

        private void Awake() {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Start() {
            UpdateLevelDisplay(); 
        }

        void UpdateLevelDisplay() {
            GetComponent<Text>().text = baseStats.GetLevel().ToString();
        }
    }
}
