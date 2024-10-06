using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;       // Movement speed of the player
    public float rotationSpeed = 100f; // Speed for rotating the player
    public float jumpForce = 5f;       // Force applied when jumping
    private Rigidbody rb;              // Reference to the Rigidbody component
    private GameObject collectible;    // Reference to the collectible object


    public AudioSource audioSource;    // Reference to the AudioSource component
    public AudioClip jumpSound;        // Sound clip for jumping
    public AudioClip collectSound;        // Sound clip for landing

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the player
        collectible = GameObject.Find("Collectible"); // Find the collectible object in the scene

        // Lock rotation on the X and Z axes to prevent tipping
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Make sure AudioSource is assigned
        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Move forward and backward with W and S relative to the current rotation
        if (Input.GetKey(KeyCode.W))
        {
            rb.MovePosition(transform.position + transform.forward * moveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            rb.MovePosition(transform.position - transform.forward * moveSpeed * Time.deltaTime);
        }

        // Rotate left and right with A and D
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }

        // Check if the player is falling or jumping by checking the velocity
        bool isFalling = rb.velocity.y < -0.1f;
        bool isJumping = rb.velocity.y > 0.1f;

        // Jumping logic (only allow jump if not falling or jumping)
        if (Input.GetKeyDown(KeyCode.Space) && !isFalling && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (jumpSound)
            {
                audioSource.PlayOneShot(jumpSound);
            }
            else {
                Debug.LogWarning("No Jump Sound set!");
            }
        }
    }

    // Check for collision with the collectible (trigger)
    private void OnTriggerEnter(Collider other)
    {
        // Check if the player hits the collectible
        if (other.gameObject == collectible)
        {

            if(collectSound)
            {
                audioSource.PlayOneShot(collectSound);
            }
            else {
                Debug.LogWarning("No collection sound set!");
            }

            StartCoroutine(CollectAndSwitchScene());
        }
    }

    IEnumerator CollectAndSwitchScene()
    {
        collectible.SetActive(false); // Deactivates the collectible GameObject, making it invisible in the scene

        yield return new WaitForSeconds(1f);

        // Switch to the next scene by index
        // Goes by order in which Scenes are added in Build Settings
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);

        //Alternatively, switch to a scene by name
        //SceneManager.LoadScene("Basics");
    }
}
