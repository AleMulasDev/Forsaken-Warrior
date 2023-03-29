using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioSource audioSource2;

    private AudioClip _activatePortalAudioClip;
    private AudioClip _enterPortalAudioClip;

    bool isActive = false;

    private void Awake()
    {
        _activatePortalAudioClip = Resources.Load<AudioClip>("ActivatePortalAudioClip");
        _enterPortalAudioClip = Resources.Load<AudioClip>("EnterPortalAudioClip");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && isActive)
        {
            StartCoroutine(EnterPortalCoroutine(other));
        }
    }

    public void ActivatePortal()
    {
        isActive = true;
        GetComponentInChildren<ParticleSystem>().Play(true);
        AudioManager.Instance.PlaySoundEffect(audioSource1, _activatePortalAudioClip);
        audioSource2.Play();
    }

    private IEnumerator EnterPortalCoroutine(Collider player)
    {
        player.gameObject.SetActive(false);
        AudioManager.Instance.PlaySoundEffect(audioSource1, _enterPortalAudioClip);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
    }
}
