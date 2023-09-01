using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingController : MonoBehaviour
{
    public PostProcessVolume postProcessVolume;

    Vignette vignette;
    ChromaticAberration chromaticAberration;

    private void Start()
    {
        postProcessVolume.profile.TryGetSettings(out vignette);
        postProcessVolume.profile.TryGetSettings(out chromaticAberration);
    }

    private void Update()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        // Vignette-Einstellungen anpassen
        vignette.enabled.value = isSprinting;
        vignette.intensity.value = isSprinting ? 0.35f : 0f;

        // Chromatische Aberration anpassen
        chromaticAberration.intensity.value = isSprinting ? 0.3f : 0f;
    }
}
