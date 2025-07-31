using NewHorizons.Components.Props;
using System;
using UnityEngine;

namespace OWJam5ModProject
{
    internal class DeveloperCommentaryDemonstration : MonoBehaviour
    {
        [Tooltip("The name of the dialog node in which to activate this demonstration")]
        [SerializeField] string activationNodeName;
        [Tooltip("The page of the dialog node on which to activate this demonstration")]
        [SerializeField] int activationDialogPage;
        [Tooltip("The name of the dialog node in which to deactivate this demonstration (if not set, demonstration remains active after exiting dialog)")]
        [SerializeField] string deactivationNodeName;
        [Tooltip("The page of the dialog node in which to deactivate this demonstration (if a deactivation node has been set)")]
        [SerializeField] int deactivationDialogPage;
        [Tooltip("The point at which to make the player look while the demonstration is active")]
        [SerializeField] Transform attentionPoint;
        [Tooltip("The objects to activate while the demonstration is active")]
        [SerializeField] GameObject activationRoot;

        void Start()
        {
            activationRoot?.SetActive(false);
        }

        public void CheckActivation(DeveloperCommentaryEntry entry, NHCharacterDialogueTree dialogTree)
        {
            if (activationNodeName == "")
                return; // No activation node name set

            if (dialogTree._currentNode.Name == activationNodeName && dialogTree._currentNode.CurrentPage == activationDialogPage)
            {
                if (attentionPoint != null)
                    entry.MoveAttentionPoint(attentionPoint);
                
                if (activationRoot != null)
                    activationRoot.SetActive(true);
            }
        }

        public void CheckDeactivation(DeveloperCommentaryEntry entry, NHCharacterDialogueTree dialogTree)
        {
            if (deactivationNodeName == "")
                return; // No deactivation node name set

            if (dialogTree._currentNode.Name == deactivationNodeName && dialogTree._currentNode.CurrentPage == deactivationDialogPage)
            {
                if (attentionPoint != null)
                    entry.ResetAttentionPoint();

                if (activationRoot != null)
                    activationRoot.SetActive(false);
            }
        }


    }
}
