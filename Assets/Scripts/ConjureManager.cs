using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ConjureManager : MonoBehaviour
{
    [SerializeField] private Text sessionState;
    [SerializeField] private Text sessionID;

    [SerializeField] private GameObject raccoon;
    [SerializeField] private Button spawnButton;
    [SerializeField] private Button qrCodeButton;

    private bool qrCodeBool;
    private IConjureKit _conjureKit;
    private Manna _manna;
    private ARCameraManager arCameraManager;
    private Camera arCamera;
    private ARRaycastManager arRaycastManager;
    private HandTracker _handTracker;
    private Texture2D _videoTexture;

    private void Awake()
    {
        if (arCamera == null) arCamera = Camera.main;
        if (arCameraManager == null) arCameraManager = FindObjectOfType<ARCameraManager>();
        if (arRaycastManager == null) arRaycastManager = FindObjectOfType<ARRaycastManager>();
    }

    public void ToggleControlsState(bool interactable)
    {
        if (spawnButton) spawnButton.interactable = interactable;
        if (qrCodeButton) qrCodeButton.interactable = interactable;
    }

    public void ToggleLighthouse()
    {
        qrCodeBool = !qrCodeBool;
        if (_manna != null) _manna.SetLighthouseVisible(qrCodeBool);
    }

    public void CreateRaccoonEntity()
    {
        if (_conjureKit == null || _conjureKit.GetState() != State.Calibrated)
            return;

        Vector3 position = arCamera.transform.position + arCamera.transform.forward * 0.5f;
        Quaternion rotation = Quaternion.Euler(0, arCamera.transform.eulerAngles.y, 0);

        Pose entityPos = new Pose(position, rotation);

        _conjureKit.GetSession().AddEntity(
            entityPos,
            onComplete: entity => CreateRaccoon(entity),
            onError: error => Debug.LogError(error)
        );
    }

    private void CreateRaccoon(Entity entity)
    {
        if (entity.Flag == EntityFlag.EntityFlagParticipantEntity) return;

        var pose = _conjureKit.GetSession().GetEntityPose(entity);
        Instantiate(raccoon, pose.position, pose.rotation);
    }

    private void Update()
    {
        if (_handTracker != null) _handTracker.Update();
        FeedMannaWithVideoFrames();
    }

    private void FeedMannaWithVideoFrames()
    {
        if (arCameraManager == null) return;

        var imageAcquired = arCameraManager.TryAcquireLatestCpuImage(out var cpuImage);
        if (!imageAcquired)
        {
            Debug.Log("Couldn't acquire CPU image");
            return;
        }

        if (_videoTexture == null || _videoTexture.width != cpuImage.width || _videoTexture.height != cpuImage.height)
        {
            _videoTexture = new Texture2D(cpuImage.width, cpuImage.height, TextureFormat.R8, false);
        }

        var conversionParams = new XRCpuImage.ConversionParams(cpuImage, TextureFormat.R8);
        cpuImage.ConvertAsync(conversionParams, (status, @params, buffer) =>
        {
            _videoTexture.LoadRawTextureData(buffer);
            _videoTexture.Apply();
            cpuImage.Dispose();

            if (_manna != null && arCamera != null)
            {
                _manna.ProcessVideoFrameTexture(_videoTexture, arCamera.projectionMatrix, arCamera.worldToCameraMatrix);
            }
        });
    }
}
