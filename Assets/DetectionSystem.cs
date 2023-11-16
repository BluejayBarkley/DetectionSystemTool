using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectionSystem : MonoBehaviour
{
    [Header("Game Object to Detect")]
    [SerializeField] private GameObject _thingToDetect;
    public bool objectDetected = false;

    [Header("If Line of Sight is Needed")]
    [SerializeField] bool _needLineOfSight;
    [SerializeField] GameObject _raycastOrigin;

    [Header("How to Detect the Object")]
    [SerializeField] private DetectionMethod _detectionMethod;
    [SerializeField] string _tagToDetect;
    [SerializeField] LayerMask _layerToDetect;
    private enum DetectionMethod
    {
        Tag,
        Layer,
        GameObject
    }

    [Header("Time Until Detector Forgets")]
    public float DetectLeaveDuration = 2.0f;
    private IEnumerator coroutine;

    public UnityEvent DetectedTagNoLOS;

    // Update is called once per frame
    void Update()
    {
        /*
        Vector3 rayStartPos = _raycastOrigin.transform.position;
        //Vector3 rayDirection = gameObject.transform.forward;

        if (objectDetected)
        {
            

            Vector3 rayDirection = _thingToDetect.transform.position - _raycastOrigin.transform.position;
            Ray ray = new Ray(rayStartPos, rayDirection);
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            RaycastHit hitData;
            Physics.Raycast(ray, out hitData);

            if (hitData.transform != null)
            {
                Debug.Log("Detected Object: " + hitData.transform.gameObject.name);

                if (hitData.transform.gameObject.tag == "Player")
                {
                    //Debug.Log("Detected Object: " + hitData.transform.gameObject.name);
                    gameObject.transform.LookAt(_thingToDetect.transform.position);

                    //Below is the action that would be run if the player object is detected. In my case I had it shooting bullets, which requires both a health script and a bullet script.

                    //Begin shooting bullets at the player.
                    
                    shootingTime += Time.deltaTime;
                    if (shootingTime >= interpolationPeriod)
                    {
                        GameObject spawnBullet = Instantiate(_bullet, _bulletOrigin.transform.position, transform.rotation);
                        spawnBullet.GetComponent<BulletScript>().owner = gameObject;
                        PlayClip2DShoot(_bulletSound, 1);
                        shootingTime = shootingTime - interpolationPeriod;
                    }
                    
                }
            }
        }
        */
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (_detectionMethod)
        {

            case DetectionMethod.Tag:
                if (other.tag == _tagToDetect)
                {
                    if (_needLineOfSight == false)
                    {
                        Debug.Log("Detected Player Tag, LoS not needed.");
                        objectDetected = true;

                        DetectedTagNoLOS.Invoke();

                        ObjectLeavingRange();
                        
                    }

                    if (_needLineOfSight == true)
                    {
                        Debug.Log("Detected Player Tag, LoS needed.");
                        objectDetected = true;
                        ObjectLeavingRange();
                    }
                }
                break;

            case DetectionMethod.Layer:
                if (other.gameObject.layer == _layerToDetect)
                {
                    if (_needLineOfSight == false)
                    {
                        Debug.Log("Detected Player Layer, LoS not needed.");
                        objectDetected = true;
                        ObjectLeavingRange();
                    }

                    if (_needLineOfSight == true)
                    {
                        Debug.Log("Detected Player Layer, LoS needed.");
                        objectDetected = true;
                        ObjectLeavingRange();
                    }
                }
                break;

            case DetectionMethod.GameObject:
                if (other.gameObject == _thingToDetect)
                {
                    if (_needLineOfSight == false)
                    {
                        Debug.Log("Detected Player GameObject, LoS not needed.");
                        objectDetected = true;
                        ObjectLeavingRange();
                    }

                    if (_needLineOfSight == true)
                    {
                        Debug.Log("Detected Player GameObject, LoS needed.");
                        objectDetected = true;
                        ObjectLeavingRange();
                    }
                }
                break;

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            //Debug.Log("Player Exited Collider.");
            StartCoroutine(coroutine);
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
        Debug.Log("Player Not Detected");
    }

    private void LineOfSight()
    {
        Vector3 rayStartPos = _raycastOrigin.transform.position;
        //Vector3 rayDirection = gameObject.transform.forward;

        if (objectDetected)
        {

            Vector3 rayDirection = _thingToDetect.transform.position - _raycastOrigin.transform.position;
            Ray ray = new Ray(rayStartPos, rayDirection);
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            RaycastHit hitData;
            Physics.Raycast(ray, out hitData);

            if (hitData.transform != null)
            {
                Debug.Log("Detected Object: " + hitData.transform.gameObject.name);

                if (hitData.transform.gameObject.tag == "Player")
                {
                    //Debug.Log("Detected Object: " + hitData.transform.gameObject.name);
                    gameObject.transform.LookAt(_thingToDetect.transform.position);
                }
            }
        }
    }

    public static AudioSource PlayClip2DShoot(AudioClip clip, float volume)
    {
        GameObject audioObject = new GameObject("2DAudio");
        AudioSource audioSource = audioObject.AddComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.volume = volume;

        audioSource.Play();

        GameObject.Destroy(audioObject, clip.length);

        return audioSource;
    }
}