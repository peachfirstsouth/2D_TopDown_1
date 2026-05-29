using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(Animator))]
public class Weapon : MonoBehaviour
{
    [Header("Camera Shake")]
    private CinemachineImpulseSource impulseSource;

    private Animator anim;
    private BoxCollider2D col;
    public PlayerController player;

    bool isImpulsed = false;

    private void Start() {
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>(); 
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Attack(int combo){
        anim.CrossFade("Combo"+combo, 0.25f);
        StartCoroutine(ActivateHitboxRoutine());
    }

    private IEnumerator ActivateHitboxRoutine()
    {
        yield return new WaitForSeconds(0.2f);
        col.enabled = true;
        yield return new WaitForSeconds(0.1f);
        col.enabled = false;
    }

    private IEnumerator ImpulseCameraIEnumerator(){
        isImpulsed = true;
        impulseSource.GenerateImpulse();
        yield return new WaitForSeconds(0.3f);
        isImpulsed = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        EnemyController e = other.GetComponent<EnemyController>();
        if(e != null && !e.IsKnockedBack){
            e.TakeDamage(Random.Range(100,999));

            if (impulseSource != null && !isImpulsed)
            {
                StartCoroutine(ImpulseCameraIEnumerator());
            }
        }
    }
}
