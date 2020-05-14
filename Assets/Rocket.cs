using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;


    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State { Alive ,  Dying ,  Transcending }
    State state = State.Alive;

    // Start is called before the first frame update
    void Start() {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (state != State.Alive) { return; }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                state = State.Transcending;
                audioSource.Stop();
                audioSource.PlayOneShot(success);
                successParticles.Play();
                Invoke("LoadNextLevel", levelLoadDelay);
                break;
            default:
                state = State.Dying;
                audioSource.Stop();
                audioSource.PlayOneShot(death);
                deathParticles.Play();
                Invoke("LoadFirstLevel", levelLoadDelay);
                break;
        }
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true;
        float rotationSpeed = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed);
        }

        rigidBody.freezeRotation = false;
    }

    private void Thrust()
    {

        float mainThrustSpeed = mainThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up * mainThrustSpeed * Time.deltaTime);
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(mainEngine);
            }
            mainEngineParticles.Play();
        }
        else
        {
            mainEngineParticles.Stop();
            audioSource.Stop();
        }
    }
}
