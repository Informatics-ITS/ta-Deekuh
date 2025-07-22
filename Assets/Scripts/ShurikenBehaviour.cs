using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;

public class ShurikenBehaviour : MonoBehaviour
{
    public float homingSpeed = 20f;
    public float homingRotateSpeed = 10f;
    // public GameObject smokeEffectPrefab;

    private Rigidbody rb;
    private Transform target;
    private bool isThrown = false;

    // public AudioClip swooshClip;
    // public AudioClip poofClip; 
    // private AudioSource audioSource;

    // public AudioSource poofSound;
    public AudioSource swooshSound;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // audioSource = GetComponent<AudioSource>();
    }

    public void OnReleasedEvent()
    {
        if (!isThrown && ShurikenAimRay.CurrentLockedEnemy != null)
        {
            Debug.LogWarning("Preparing to throw towards locked enemy: " + ShurikenAimRay.CurrentLockedEnemy.name);

            // Interactor has released the object
            if (ShurikenAimRay.CurrentLockedEnemy != null)
            {
                Debug.LogWarning("Released. Throwing towards locked enemy: " + ShurikenAimRay.CurrentLockedEnemy.name);

                swooshSound.Play();

                ThrowTowards(ShurikenAimRay.CurrentLockedEnemy);
            }
            else
            {
                Debug.LogWarning("Released, but no locked enemy.");
            }

            isThrown = true; // Prevent retriggering
        }
    }

    public void ThrowTowards(Transform targetTransform)
    {
        rb.isKinematic = false;
        target = targetTransform;
        isThrown = true;

        // Set an initial velocity toward the target
        Vector3 aimPosition = target.position + new Vector3(0, 1.0f, 0); // 1m upward
        Vector3 direction = (aimPosition - transform.position).normalized;
        rb.velocity = direction * homingSpeed;

    }

    void FixedUpdate()
    {
        // Debug.DrawRay(transform.position, transform.forward * 0.5f, Color.blue);
        // Debug.DrawRay(transform.position, transform.up * 0.5f, Color.green);
        // Debug.DrawRay(transform.position, transform.right * 0.5f, Color.red);

        if (isThrown && target != null)
        {
            // Vector3 direction = (target.position - transform.position).normalized;

            // Rotate toward enemy smoothly
            // Quaternion lookRotation = Quaternion.LookRotation(direction);
            // rb.MoveRotation(Quaternion.Slerp(rb.rotation, lookRotation, homingRotateSpeed * Time.fixedDeltaTime));
            // // rb.velocity = transform.forward * homingSpeed;
            // rb.velocity = direction * homingSpeed; // Maintain speed towards target

            // rb.AddForce(direction * homingSpeed, ForceMode.Acceleration);

            // Latest approach
            Vector3 aimPosition = target.position + new Vector3(0, 1.0f, 0); // 1m upward
            Vector3 force = (aimPosition - transform.position).normalized * homingSpeed;
            rb.AddForce(force, ForceMode.Acceleration);
            // rb.angularVelocity = transform.up * 20f; // adjust speed

            rb.AddTorque(transform.up * 20f, ForceMode.VelocityChange);

            // Vector3 moveDir = Vector3.RotateTowards(rb.velocity, direction, homingRotateSpeed * Time.deltaTime, 0.0f);
            // rb.velocity = moveDir.normalized * homingSpeed;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.LogWarning("Shuriken hit an enemy: " + collision.gameObject.name);

            // Notify enemy and destroy shuriken
            EnemyAI enemyAI = collision.gameObject.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.OnHit();
            }
            else
            {
                Destroy(collision.gameObject); // fallback
            }

            Destroy(gameObject); // Destroy shuriken on hit
            ShurikenAimRay.ClearTargetIcon(); // Clear the aim icon
        }
    }

    public bool IsThrown()
    {
        return isThrown;
    }
}
