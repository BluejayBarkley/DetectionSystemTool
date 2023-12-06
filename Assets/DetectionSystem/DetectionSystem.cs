using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionSystem : MonoBehaviour
{
    [Header("Game Object to Detect")]
    //Drag in the specific game object the detector is watching for.
    [SerializeField] private GameObject _objectToDetect;
    //A bool statement set to false by default so the object is not detected on game start.
    [SerializeField][Tooltip("The object is not detected by default on game start.")]
    public bool objectDetected = false;

    [Header("Line of Sight")]
    //A boolean used to determine if the detector requires line of sight when detecting an object. True means line of sight is required, and false means it is not.
    [SerializeField][Tooltip("Check if line of sight is required for detection.")]
    bool _lineOfSightRequired;
    //Line of sight uses raycasting, so the raycastOrigin is the point from which the raycast originates.
    [SerializeField][Tooltip("Line of Sight point of origin.")]
    GameObject _raycastOrigin;

    [Header("Method of Detection")]
    [SerializeField] private DetectionMethod _detectionMethod;
    //If using Tags to detect, input the Tag name to detect.
    [SerializeField] string _tagToDetect;
    //If using Layers to detect, input the Layer name to detect.
    [SerializeField] LayerMask _layerToDetect;
    private enum DetectionMethod
    {
        Tag,
        Layer,
        GameObject
    }

    [Header("Time Until Detector Forgets")]
    //DetectLeaveDuration allows the detector to maintain detection on the object after the object has left detection range. Set to 0 to disable this feature.
    [SerializeField][Tooltip("The amount of time in seconds it takes for the detector to forget the object after it leaves the detection range.")] 
    public float DetectLeaveDuration = 2.0f;
    private IEnumerator coroutine;

    [Header("Events to be Run")]
    //DetectedWithoutLineOfSight will be used for events that require detection without a line of sight requirement.
    public UnityEvent DetectedWithoutLineOfSight;
    //While DetectedWithLineOfSight detects with a line of sight requirement.
    public UnityEvent DetectedWithLineOfSight;

    private void Awake()
    {
        //To ensure that the DetectLeaveDuration does not cause issues on game start.
        coroutine = leftRange();
    }

    // Update is called once per frame
    void Update()
    {
        //If the object to detect enters the trigger volume of the detector...
        if (objectDetected)
        {
            //and if line of sight is not required...
            if(_lineOfSightRequired == false)
            {
                //Continously run the event where the object is detected and line of sight is not required.
                DetectedWithoutLineOfSight.Invoke();
            }

            //Continuously run the line of sight code.
            LineOfSight();
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        switch (_detectionMethod)
        {

            //Detection method set to Tag.
            case DetectionMethod.Tag:
                //Comparing the tag of the detected object with the tag to detect.
                if (other.tag == _tagToDetect)
                {
                    //If line of sight is not required...
                    if (_lineOfSightRequired == false)
                    {
                        //Tag entered collider, line of sight not required.
                        //Then the object is detected.
                        objectDetected = true;

                        //Run the code to detect when the object has left the detection range.
                        ObjectLeavingRange();
                        
                    }

                    //If line of sight is required...
                    if (_lineOfSightRequired == true)
                    {
                        //Tag entered collider, line of sight required.
                        objectDetected = true;
                        ObjectLeavingRange();
                    }
                }
                break;

            //Detection method set to Layer.
            case DetectionMethod.Layer:
                //Mathf.Pow is required to compare layer values to the power of 2.
                if (Mathf.Pow(2, other.gameObject.layer) == _layerToDetect.value)
                {
                    if (_lineOfSightRequired == false)
                    {
                        //Layer entered collider, line of sight not required.
                        objectDetected = true;
                        ObjectLeavingRange();
                    }

                    if (_lineOfSightRequired == true)
                    {
                        //Layer entered collider, line of sight required.
                        objectDetected = true;
                        ObjectLeavingRange();
                    }
                }
                break;

            //Detection method set to GameObject.
            case DetectionMethod.GameObject:
                if (other.gameObject == _objectToDetect)
                {
                    if (_lineOfSightRequired == false)
                    {
                        //GameObject entered collider, line of sight not required.
                        objectDetected = true;
                        ObjectLeavingRange();
                    }

                    if (_lineOfSightRequired == true)
                    {
                        //GameObject entered collider, line of sight required.
                        objectDetected = true;
                        ObjectLeavingRange();
                    }
                }
                break;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        switch (_detectionMethod)
        {

            case DetectionMethod.Tag:
                if (other.tag == _tagToDetect)
                {
                    if (_lineOfSightRequired == false)
                    {
                        //Tag exited collider, line of sight not required.
                        StartCoroutine(coroutine);

                    }

                    if (_lineOfSightRequired == true)
                    {
                        //Tag exited collider, line of sight required.
                        StartCoroutine(coroutine);
                    }
                }
                break;

            case DetectionMethod.Layer:
                if (Mathf.Pow(2, other.gameObject.layer) == _layerToDetect.value)
                {
                    if (_lineOfSightRequired == false)
                    {
                        //Layer exited collider, line of sight not required.
                        StartCoroutine(coroutine);
                    }

                    if (_lineOfSightRequired == true)
                    {
                        //Layer exited collider, line of sight required.
                        StartCoroutine(coroutine);
                    }
                }
                break;

            case DetectionMethod.GameObject:
                if (other.gameObject == _objectToDetect)
                {
                    if (_lineOfSightRequired == false)
                    {
                        //Object exited collider, line of sight not required.
                        StartCoroutine(coroutine);
                    }

                    if (_lineOfSightRequired == true)
                    {
                        //Object exited collider, line of sight required.
                        StartCoroutine(coroutine);
                    }
                }
                break;
        }
    }

    private void ObjectLeavingRange()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = leftRange();
    }

    IEnumerator leftRange()
    {
        yield return new WaitForSeconds(DetectLeaveDuration);
        objectDetected = false;
        //The object is no longer detected.
    }

    public void MoveToObject()
    {
        float speed = 1.0f;
        var step = speed * Time.deltaTime;
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, _objectToDetect.transform.position, step);
    }

    public void LookAtObject()
    {
        gameObject.transform.LookAt(_objectToDetect.transform.position);
    }

    //Method used to raycast for line of sight detection requirements.
    public void LineOfSight()
    {       
        Vector3 rayStartPos = _raycastOrigin.transform.position;

        if (objectDetected)
        {
            Vector3 rayDirection = _objectToDetect.transform.position - _raycastOrigin.transform.position;
            Ray ray = new Ray(rayStartPos, rayDirection);
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            RaycastHit hitData;
            Physics.Raycast(ray, out hitData);

            if (hitData.transform != null)
            {

                switch (_detectionMethod)
                {

                    case DetectionMethod.Tag:
                        if (hitData.transform.gameObject.tag == _tagToDetect)
                        {
                            //If line of sight is required...
                            if (_lineOfSightRequired == true)
                            {
                                //Run the event where line of sight is required.
                                DetectedWithLineOfSight.Invoke();
                            }
                        }
                        break;

                    case DetectionMethod.Layer:
                        if (Mathf.Pow(2, hitData.transform.gameObject.layer) == _layerToDetect.value)
                        {
                            if (_lineOfSightRequired == true)
                            {
                                DetectedWithLineOfSight.Invoke();
                            }
                        }
                        break;

                    case DetectionMethod.GameObject:
                        if (hitData.transform.gameObject == _objectToDetect)
                        {
                            if (_lineOfSightRequired == true)
                            {
                                DetectedWithLineOfSight.Invoke();
                            }
                        }
                        break;
                }
            }
        }
    }
}