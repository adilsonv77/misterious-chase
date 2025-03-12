using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movements : MonoBehaviour
{
    // For movement
    private Joystick joystick;
    public float playerSpeed;
    Vector3 moveDirection;

    // For jump
    public float jumpForce;
    Rigidbody playerBody;
    public bool isGround;

    // To play audios
    private AudioSource audioSrc;
    public AudioClip[] clips; // 0 - Jump

    private bool playingSound = false;

    // Start is called before the first frame update
    void Start()
    {
        audioSrc = GetComponent<AudioSource>();

        joystick = GameObject.Find("JoystickMovement").GetComponent<Joystick>();
        playerBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Rotate();
    }

    void Movement() 
    {
        moveDirection = (Vector3.forward * joystick.Vertical) + (Vector3.right * joystick.Horizontal);
    }

    void Rotate()
    {
        transform.Translate(moveDirection * (playerSpeed * Time.deltaTime));
    }

    public void Jump() {
        if (isGround)
        {
            playerBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGround = false;

            StartCoroutine(PlayJumpSound());
        }
    }

    // To verify if the player is on ground
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
        }
    }

    // To avoid sound blocks
    public IEnumerator PlayJumpSound()
    {
        playingSound = true;

        audioSrc.clip = clips[0];
        audioSrc.Play();

        // Wait 4 seconds
        yield return new WaitForSecondsRealtime(4f);

        playingSound = false;
    }

    public bool getIsPlayingSound() {
        return playingSound;
    }
}
