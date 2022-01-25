using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Core;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour
    {
        bool cinematicPlayed = false;

        private void OnTriggerEnter(Collider other) {
            if (!other.CompareTag("Player")) return;
            if (cinematicPlayed) return;

            cinematicPlayed = true;
            GetComponent<PlayableDirector>().Play();
        }
    }
}
