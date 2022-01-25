using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas rootCanvas = null;

        // Update is called once per frame
        void Update() {
            float currentHealthFraction = healthComponent.GetFraction();
            if (Mathf.Approximately(currentHealthFraction, 0)
                || Mathf.Approximately(currentHealthFraction, 1)) {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;
            foreground.localScale = new Vector3(currentHealthFraction, 1, 1);
        }
    }
}
