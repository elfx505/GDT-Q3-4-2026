using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(GraphicRaycaster))]
public class CanvasClickAudio : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip leftClickClip;
    [SerializeField] private float lClickSFXstartTime;
    [SerializeField] private float lClickSFXendTime;
    [SerializeField] private float lClickSFXvolume = 1f;
    [SerializeField] private AudioClip rightClickClip;

    [SerializeField] private float rClickSFXstartTime;
    [SerializeField] private float rClickSFXendTime;
    [SerializeField] private float rClickSFXvolume = 1f;

    private GraphicRaycaster _raycaster;
    private EventSystem _eventSystem;

    private void Awake()
    {
        _raycaster = GetComponent<GraphicRaycaster>();
        _eventSystem = EventSystem.current;
    }

    private void Update()
    {
        // 0 = Left Click, 1 = Right Click
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverThisCanvas())
            {
                PlaySound(leftClickClip, lClickSFXvolume, lClickSFXstartTime, lClickSFXendTime);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (IsPointerOverThisCanvas())
            {
                PlaySound(rightClickClip, rClickSFXvolume, rClickSFXstartTime, rClickSFXendTime);
            }
        }
    }

    private bool IsPointerOverThisCanvas()
    {
        if (_eventSystem == null) return false;

        // Set up raycast pointer data
        PointerEventData pointerData = new PointerEventData(_eventSystem)
        {
            position = Input.mousePosition
        };

        // Raycast specifically through this Canvas's GraphicRaycaster
        List<RaycastResult> results = new List<RaycastResult>();
        _raycaster.Raycast(pointerData, results);

        // If at least one UI element on this canvas was hit
        return results.Count > 0;
    }

    private void PlaySound(AudioClip clip, float v, float sTime, float eTime)
    {
        if (clip == null) return;

        AudioManager.Instance.PlaySFX(clip, volume: v, startTime: sTime, endTime: eTime, canSpam: true);
    }
}