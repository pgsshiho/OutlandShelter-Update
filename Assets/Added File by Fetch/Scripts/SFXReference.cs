using UnityEngine;

public class SFXReference : MonoBehaviour
{
    private static SFXReference _instance;
    public static SFXReference Instance => _instance;

    public AudioClip hammer;
    public AudioClip melee;
    public AudioClip gun;
    public AudioClip shotgun;
    public AudioClip reload;
    public AudioClip bomb;
    public AudioClip fire;
    public AudioClip alram;

    private void Awake()
    {
        _instance = this;
    }
}
