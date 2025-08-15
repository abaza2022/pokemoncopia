using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : MonoBehaviour
{
    public float walkSpeed;
    private bool isWalking;
    private Vector2 input;
    private Animator animator;
    public LayerMask Solids;
    public LayerMask Cliff;

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

                if (IsWalkable(targetPos)) StartCoroutine(Move(targetPos));
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
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if (Physics2D.OverlapCircle(targetPos, 0.2f, Solids) != null || Physics2D.OverlapCircle(targetPos, 0.2f, Cliff) != null && input.y > 0)
        {
            return false;
        }

        return true;
    }
}
