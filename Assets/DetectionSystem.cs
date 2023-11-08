using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionSystem : MonoBehaviour
{
    [SerializeField] private GameObject _thingToShootAt;
    [SerializeField] GameObject _bullet;
    [SerializeField] AudioClip _bulletSound;
    [SerializeField] GameObject _bulletOrigin;

    public bool playerDetected = false;

    [SerializeField] private float shootingTime = 0.0f;
    [SerializeField] public float interpolationPeriod = 3.0f;

    public float DetectLeaveDuration = 2.0f;
    private IEnumerator coroutine;

    // Update is called once per frame
    void Update()
    {

        Vector3 rayStartPos = gameObject.transform.position;
        //Vector3 rayDirection = gameObject.transform.forward;

        if (playerDetected)
        {

            Vector3 rayDirection = _thingToShootAt.transform.position - gameObject.transform.position;
            Ray ray = new Ray(rayStartPos, rayDirection);
            Debug.DrawRay(ray.origin, ray.direction, Color.red);
            RaycastHit hitData;
            Physics.Raycast(ray, out hitData);

            if (hitData.transform != null)
            {
                //Debug.Log("Detected Object: " + hitData.transform.gameObject.name);

                if (hitData.transform.gameObject.tag == "Player")
                {
                    //Debug.Log("Detected Object: " + hitData.transform.gameObject.name);
                    gameObject.transform.LookAt(_thingToShootAt.transform.position);

                    //Below is the action that would be run if the player object is detected. In my case I had it shooting bullets, which requires both a health script and a bullet script.

                    //Begin shooting bullets at the player.
                    /*
                    shootingTime += Time.deltaTime;
                    if (shootingTime >= interpolationPeriod)
                    {
                        GameObject spawnBullet = Instantiate(_bullet, _bulletOrigin.transform.position, transform.rotation);
                        spawnBullet.GetComponent<BulletScript>().owner = gameObject;
                        PlayClip2DShoot(_bulletSound, 1);
                        shootingTime = shootingTime - interpolationPeriod;
                    }
                    */
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //Debug.Log("Detected Player");
            playerDetected = true;
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
            coroutine = leftRange();
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

    IEnumerator leftRange()
    {
        yield return new WaitForSeconds(DetectLeaveDuration);
        playerDetected = false;
        //Debug.Log("Player Not Detected");
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