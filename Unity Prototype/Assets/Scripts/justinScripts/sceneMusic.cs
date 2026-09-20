using UnityEngine;

public class sceneMusic : MonoBehaviour
{
    public AudioClip clip;
    [SerializeField] AudioSource src;
    

    void Start()
    {
        src.clip = clip;
        src.loop = true;
        src.Play();
    }
}
