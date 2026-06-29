using UnityEngine;

public class PlayButtonClickSound : MonoBehaviour
{
    public void PlayClickSFX()
    {
        if (SFXReference.Instance.buttonclick != null)
        {
            SoundManager.SFX.PlayOneShot(SFXReference.Instance.buttonclick);
        }
    }
}
