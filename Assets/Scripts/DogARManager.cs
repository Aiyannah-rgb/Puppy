using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
public class DogARManager : MonoBehaviour
{
    [SerializeField] private GameObject dogPrefab;
    [SerializeField] private GameObject placementIndicator;

    private ARRaycastManager _raycastManager;
    private Pose _placementPose;
    private bool _placementPoseIsValid = false;
    private GameObject _spawnedDog;
    private static readonly List<ARRaycastHit> Hits = new List<ARRaycastHit>();

    private void Awake()
    {
        _raycastManager = GetComponent<ARRaycastManager>();
    }

    private void Update()
    {
        UpdatePlacementPose();
        UpdatePlacementIndicator();
        HandleTouch();
    }

    private void HandleTouch()
    {
        if (!_placementPoseIsValid) return;
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);
        if (touch.phase != TouchPhase.Began) return;

        if (_spawnedDog == null)
        {
            _spawnedDog = dogPrefab != null ? Instantiate(dogPrefab, _placementPose.position, _placementPose.rotation) : CreateProceduralDog(_placementPose.position, _placementPose.rotation);
        }
        else
        {
            _spawnedDog.transform.SetPositionAndRotation(_placementPose.position, _placementPose.rotation);
        }
    }

    private void UpdatePlacementIndicator()
    {
        if (placementIndicator == null) return;

        placementIndicator.SetActive(_placementPoseIsValid);
        if (_placementPoseIsValid)
        {
            placementIndicator.transform.SetPositionAndRotation(_placementPose.position, _placementPose.rotation);
        }
    }

    private void UpdatePlacementPose()
    {
        var screenCenter = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        _placementPoseIsValid = _raycastManager.Raycast(screenCenter, Hits, TrackableType.Planes);

        if (_placementPoseIsValid)
        {
            _placementPose = Hits[0].pose;
            var cameraForward = Camera.main.transform.forward;
            var cameraBearing = new Vector3(cameraForward.x, 0, cameraForward.z).normalized;
            _placementPose.rotation = Quaternion.LookRotation(cameraBearing);
        }
    }

    private GameObject CreateProceduralDog(Vector3 position, Quaternion rotation)
    {
        GameObject dog = new GameObject("ProceduralDog");
        dog.transform.SetPositionAndRotation(position, rotation);

        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
        body.name = "Body";
        body.transform.SetParent(dog.transform, false);
        body.transform.localPosition = new Vector3(0, 0.1f, 0);
        body.transform.localScale = new Vector3(0.4f, 0.2f, 0.25f);
        SetColor(body, new Color(0.75f, 0.55f, 0.35f));

        GameObject head = GameObject.CreatePrimitive(PrimitiveType.Cube);
        head.name = "Head";
        head.transform.SetParent(dog.transform, false);
        head.transform.localPosition = new Vector3(0, 0.18f, 0.2f);
        head.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
        SetColor(head, new Color(0.8f, 0.6f, 0.4f));

        CreateEar(dog.transform, new Vector3(-0.07f, 0.24f, 0.25f));
        CreateEar(dog.transform, new Vector3(0.07f, 0.24f, 0.25f));
        CreateLeg(dog.transform, new Vector3(-0.12f, 0.0f, 0.08f));
        CreateLeg(dog.transform, new Vector3(0.12f, 0.0f, 0.08f));
        CreateLeg(dog.transform, new Vector3(-0.12f, 0.0f, -0.08f));
        CreateLeg(dog.transform, new Vector3(0.12f, 0.0f, -0.08f));
        CreateTail(dog.transform, new Vector3(0, 0.18f, -0.15f));
        CreateEye(dog.transform, new Vector3(-0.04f, 0.2f, 0.28f));
        CreateEye(dog.transform, new Vector3(0.04f, 0.2f, 0.28f));

        return dog;
    }

    private static void CreateEar(Transform parent, Vector3 localPosition)
    {
        GameObject ear = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ear.name = "Ear";
        ear.transform.SetParent(parent, false);
        ear.transform.localPosition = localPosition;
        ear.transform.localScale = new Vector3(0.05f, 0.08f, 0.02f);
        SetColor(ear, new Color(0.6f, 0.4f, 0.3f));
    }

    private static void CreateEye(Transform parent, Vector3 localPosition)
    {
        GameObject eye = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        eye.name = "Eye";
        eye.transform.SetParent(parent, false);
        eye.transform.localPosition = localPosition;
        eye.transform.localScale = Vector3.one * 0.03f;
        SetColor(eye, Color.black);
    }

    private static void CreateLeg(Transform parent, Vector3 localPosition)
    {
        GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leg.name = "Leg";
        leg.transform.SetParent(parent, false);
        leg.transform.localPosition = localPosition;
        leg.transform.localScale = new Vector3(0.05f, 0.16f, 0.05f);
        SetColor(leg, new Color(0.75f, 0.55f, 0.35f));
    }

    private static void CreateTail(Transform parent, Vector3 localPosition)
    {
        GameObject tail = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tail.name = "Tail";
        tail.transform.SetParent(parent, false);
        tail.transform.localPosition = localPosition;
        tail.transform.localRotation = Quaternion.Euler(30, 0, 0);
        tail.transform.localScale = new Vector3(0.05f, 0.15f, 0.05f);
        SetColor(tail, new Color(0.75f, 0.55f, 0.35f));
    }

    private static void SetColor(GameObject primitive, Color color)
    {
        var renderer = primitive.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = new Material(Shader.Find("Standard"));
            renderer.material.color = color;
        }
    }
}
