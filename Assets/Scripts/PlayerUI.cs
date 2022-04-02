using TMPro;
using UnityEngine;

public class PlayerUI : MonoBehaviour {
    public TMP_Text interactText;

    private void Awake() {
        interactText.gameObject.SetActive(false);
    }

    public void RefreshForObject(InteractiveObject interactiveObject) {
        if(interactiveObject != null) {
            interactText.text = interactiveObject.InteractText;
            interactText.gameObject.SetActive(true);
        } else {
            interactText.gameObject.SetActive(false);
        }
    }
}
