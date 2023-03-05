using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private string sceneName;

    bool isActive = false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && isActive)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void ActivatePortal()
    {
        isActive = true;
        GetComponentInChildren<ParticleSystem>().Play(true);
    }
}
