using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteScript : MonoBehaviour, Interactable
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;

    [Space(10)]
    [Header("Note Settings")]
    [SerializeField] private GameObject NoteCanvas;

    private bool isReadable = false;
    private bool isNoteOpen = false;

    void Start()
    {
        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();
        if (NoteCanvas != null)
            NoteCanvas.SetActive(false);
    }

    public bool CanInteract()
    {
        return isReadable || isNoteOpen;
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        if (isNoteOpen)
            HideNote();
        else
            ShowNote();
    }

    public void ShowNote()
    {
        if (NoteCanvas == null) return;
        NoteCanvas.SetActive(true);
        isNoteOpen = true;
        Time.timeScale = 0f;
        if (playerMovement != null)
            playerMovement.DisableMovement();
    }

    public void HideNote()
    {
        if (NoteCanvas == null) return;
        NoteCanvas.SetActive(false);
        isNoteOpen = false;
        Time.timeScale = 1f;
        if (playerMovement != null)
            playerMovement.EnableMovement();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            isReadable = true;
            Debug.Log("Note: isReadable = true");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            isReadable = false;
            if (isNoteOpen)
                HideNote();
        }
    }
}
