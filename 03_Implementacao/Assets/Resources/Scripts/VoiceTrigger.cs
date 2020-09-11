using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceTrigger : MonoBehaviour
{
    [SerializeField] AudioClip voice;
    [SerializeField] GameObject VoiceTextArea;
    [TextArea] public string stringText;

    private bool entered = false;


    private void OnTriggerEnter(Collider other) {
        
        GameObject newObject = other.gameObject;
        Debug.Log(newObject.tag);
        if (newObject.CompareTag("player") && !entered) {
            entered = true;
            newObject.GetComponent<AudioSource>().PlayOneShot(voice);
            VoiceTextArea.transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = stringText;
            VoiceTextArea.SetActive(true);
            StartCoroutine(TimeOfAudio(voice.length));
        }
    }


    private IEnumerator TimeOfAudio(float time) {
        yield return new WaitForSeconds(time);
        VoiceTextArea.SetActive(false);
        Destroy(gameObject, 2);
    }
}
