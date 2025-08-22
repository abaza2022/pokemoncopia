using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Walk : MonoBehaviour
{
    public float walkSpeed;
    private bool isWalking;
    private Vector2 input;
    private Animator animator;
    public LayerMask Solids;
    public LayerMask Cliff;
    public LayerMask Grass;
    public AudioSource audioSource;
    public AudioClip hopSFX;
    public AudioClip blockSFX;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isWalking)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                animator.SetFloat("walkX", input.x);
                animator.SetFloat("walkY", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (input.y < 0 && Physics2D.OverlapCircle(targetPos, 0.2f, Cliff) != null)
                {
                    targetPos.y += input.y;
                    audioSource.PlayOneShot(hopSFX);
                }

                if (IsWalkable(targetPos)) StartCoroutine(Move(targetPos));
                else audioSource.PlayOneShot(blockSFX);
                    return;
            }
        }

        animator.SetBool("isWalking", isWalking);
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isWalking = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, walkSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isWalking = false;

        CheckForBattle();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, Solids) != null || Physics2D.OverlapCircle(targetPos, 0.2f, Cliff) != null && input.y > 0)
        {
            return false;
        }

        return true;
    }

    private void CheckForBattle()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, Grass) != null)
        {
            if (Random.Range(1, 11) == 1)
            {
                SceneManager.LoadScene("Batalla");
            }
        }
    }
}
