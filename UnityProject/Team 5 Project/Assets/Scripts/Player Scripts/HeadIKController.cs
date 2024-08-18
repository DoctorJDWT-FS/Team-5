using UnityEngine;

public class HeadIKController : MonoBehaviour
{
    private Animator animator;

    public Transform lookTarget;  // Assign the target to look at (usually the camera or an object in the scene)
    public float lookWeight = 1.0f;  // Control how much the head follows the target (0 to 1)

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // Set the look weight, which determines how much the character should look at the target
            animator.SetLookAtWeight(lookWeight);

            // Set the target the head should look at
            animator.SetLookAtPosition(lookTarget.position);
        }
    }
}
