using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBar : MonoBehaviour
{
    [SerializeField] private Slider loadingSlider;
    
    [Tooltip("How fast the bar fills. 1 = takes 1 second minimum. 0.5 = takes 2 seconds minimum.")]
    [SerializeField] private float fillSpeed = 1f; 
    
    private float smoothedProgress = 0f;

    void Update()
    {
        if (loadingSlider == null) return;

        float targetProgress = Loader.GetLoadingProgress();

        // FIX 1: Cap the delta time. If the Main Thread freezes for 1 second to load assets, 
        // the visual bar will only advance as if 0.05 seconds passed, preventing a huge jump.
        float safeDeltaTime = Mathf.Min(Time.unscaledDeltaTime, 0.05f);

        // FIX 2: Use MoveTowards instead of Lerp. This guarantees it will take a specific 
        // amount of time to cross the screen, giving your runner time to animate.
        smoothedProgress = Mathf.MoveTowards(smoothedProgress, targetProgress, fillSpeed * safeDeltaTime);

        loadingSlider.value = smoothedProgress;

        // If the background load is done AND the visual runner has crossed the finish line
        if (targetProgress >= 1f && smoothedProgress >= 0.99f)
        {
            Loader.ActivateLoadedScene();
        }
    }
}