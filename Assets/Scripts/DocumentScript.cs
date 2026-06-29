using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DocumentScript : MonoBehaviour, Interactable
{
    [Header("References")]
    [SerializeField] private PlayerMovement playerMovement;

    [Space(10)]
    [Header("Document Settings")]
    [SerializeField] private GameObject DocumentCanvas;

    private bool isReadable = false;
    private bool isDocumentOpen = false;

    void Start()
    {
        if (playerMovement == null)
            playerMovement = FindObjectOfType<PlayerMovement>();
        if (DocumentCanvas != null)
            DocumentCanvas.SetActive(false);
    }

    public bool CanInteract()
    {
        return isReadable || isDocumentOpen;
    }

    public void Interact()
    {
        if (!CanInteract()) return;

        if (isDocumentOpen)
            HideDocument();
        else
            ShowDocument();
    }

    public void ShowDocument()
    {
        if (DocumentCanvas == null) return;
        DocumentCanvas.SetActive(true);
        isDocumentOpen = true;
        if (playerMovement != null)
            playerMovement.DisableMovement();
    }

    public void HideDocument()
    {
        if (DocumentCanvas == null) return;
        DocumentCanvas.SetActive(false);
        isDocumentOpen = false;
        if (playerMovement != null)
            playerMovement.EnableMovement();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            isReadable = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponentInParent<PlayerMovement>() != null)
        {
            isReadable = false;
            if (isDocumentOpen)
                HideDocument();
        }
    }
}
