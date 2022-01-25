using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Saving;

namespace RPG.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "save";

        [SerializeField] Text textBox;

        private void Awake() {
            StartCoroutine(LoadLastScene());
        }

        IEnumerator LoadLastScene() {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            Fader fader = FindObjectOfType<Fader>();
            fader.FadeOutImmediate();
            yield return fader.FadeIn(2f);
        }

        private void Update() {
            if (Input.GetKeyDown(KeyCode.L)) {
                Load();
            }
            else if (Input.GetKeyDown(KeyCode.S)) {
                Save();
            }
            else if (Input.GetKeyDown(KeyCode.D)) {
                Delete();
            }
        }

        public void Load() {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
            StartCoroutine(ShowText("Loaded Game!"));

        }

        public void Save() {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
            StartCoroutine(ShowText("Saved Game!"));
        }

        public void Delete() {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }

        IEnumerator ShowText(string text) {
            if (textBox == null) yield break;

            textBox.text = text;
            yield return new WaitForSeconds(2);
            textBox.text = "";
        }
    }
}
