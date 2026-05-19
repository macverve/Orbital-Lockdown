using System.Collections.Generic;
using UnityEngine;

public class AutomaticDoorOpener : MonoBehaviour
{
    [Header("Door panels")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Blocking colliders on the actual door panels")]
    public Collider leftDoorCollider;
    public Collider rightDoorCollider;

    [Header("How far each door moves in LOCAL space")]
    public Vector3 leftOpenOffset = new Vector3(-1f, 0f, 0f);
    public Vector3 rightOpenOffset = new Vector3(1f, 0f, 0f);

    [Header("Movement")]
    public float moveSpeed = 2.5f;
    public float closedTolerance = 0.02f;

    [Header("Player detection")]
    public string playerTag = "Player";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openClip;
    public AudioClip closeClip;

    [Header("Security Settings")]
    [Tooltip("If true, the door sensors are offline until UnlockDoor() is triggered via the control board.")]
    public bool isLocked = true;

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;

    private bool shouldOpen = false;

    private HashSet<Transform> playersInside = new HashSet<Transform>();

    private void Start()
    {
        if (leftDoor != null)
        {
            leftClosedPos = leftDoor.localPosition;
            leftOpenPos = leftClosedPos + leftOpenOffset;
        }

        if (rightDoor != null)
        {
            rightClosedPos = rightDoor.localPosition;
            rightOpenPos = rightClosedPos + rightOpenOffset;
        }
    }

    private void Update()
    {
        if (leftDoor != null)
        {
            Vector3 target = shouldOpen ? leftOpenPos : leftClosedPos;
            leftDoor.localPosition = Vector3.MoveTowards(
                leftDoor.localPosition,
                target,
                moveSpeed * Time.deltaTime
            );
        }

        if (rightDoor != null)
        {
            Vector3 target = shouldOpen ? rightOpenPos : rightClosedPos;
            rightDoor.localPosition = Vector3.MoveTowards(
                rightDoor.localPosition,
                target,
                moveSpeed * Time.deltaTime
            );
        }

        if (!shouldOpen)
        {
            bool leftClosed = true;
            bool rightClosed = true;

            if (leftDoor != null)
                leftClosed = Vector3.Distance(leftDoor.localPosition, leftClosedPos) <= closedTolerance;

            if (rightDoor != null)
                rightClosed = Vector3.Distance(rightDoor.localPosition, rightClosedPos) <= closedTolerance;

            bool allClosed = leftClosed && rightClosed;

            if (allClosed)
            {
                if (leftDoorCollider != null) leftDoorCollider.enabled = true;
                if (rightDoorCollider != null) rightDoorCollider.enabled = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {

        if (isLocked)
            return;

        Transform root = other.transform.root;

        if (!root.CompareTag(playerTag))
            return;

        playersInside.Add(root);

        if (!shouldOpen)
        {
            shouldOpen = true;

            if (leftDoorCollider != null) leftDoorCollider.enabled = false;
            if (rightDoorCollider != null) rightDoorCollider.enabled = false;

            PlayDoorOpenSound();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Transform root = other.transform.root;

        if (!root.CompareTag(playerTag))
            return;

        playersInside.Remove(root);

        if (playersInside.Count == 0 && shouldOpen)
        {
            shouldOpen = false;
            PlayDoorCloseSound();
        }
    }


    public void UnlockDoor()
    {
        isLocked = false;
        Debug.Log($"[Security System] {gameObject.name} has been successfully unlocked!");
    }

    private void PlayDoorOpenSound()
    {
        if (audioSource != null && openClip != null)
        {
            audioSource.PlayOneShot(openClip);
        }
    }

    private void PlayDoorCloseSound()
    {
        if (audioSource != null && closeClip != null)
        {
            audioSource.PlayOneShot(closeClip);
        }
    }
}