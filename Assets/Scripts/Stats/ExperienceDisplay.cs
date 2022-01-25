using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience experience;
        BaseStats baseStats;

        private void OnEnable() {
            experience.onExperienceGained += UpdateExperienceDisplay;
            baseStats.onLevelUp += UpdateExperienceDisplay;
        }

        private void OnDisable() {
            experience.onExperienceGained -= UpdateExperienceDisplay;
            baseStats.onLevelUp -= UpdateExperienceDisplay;

        }

        private void Awake() {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();

        }

        private void Start() {
            UpdateExperienceDisplay();
        }

        void UpdateExperienceDisplay() {
            float xpUntilNextLevel = baseStats.GetXPTillNextLevel();
            string xpUntilNextLevelText;
            if (xpUntilNextLevel == 0)
                xpUntilNextLevelText = "MAX";
            else
                xpUntilNextLevelText = xpUntilNextLevel.ToString();

            GetComponent<Text>().text = string.Format("{0:0} / {1:0}", experience.GetPoints(), xpUntilNextLevelText);
             
        }
    }
}
