using NewHorizons.Components.Props;
using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OWJam5ModProject
{
    internal class DeveloperCommentaryEntry : MonoBehaviour
    {
        public enum CommentaryAuthor { Walker, Cleric, Jamie, John}

        public const string SIGNAL_FREQUENCY_NAME = "Developer Commentary";
        public const string DEVELOPER_COMMENTARY_OPTION = "developerCommentary";
        const string SIGNAL_AUDIO = "TH_RadioSignal_LP";
        const string EMISSION_COLOR_PARAMETER = "_EmissionColor";

        [Header("Global Options")]
        [SerializeField] NHCharacterDialogueTree dialogTree;
        [SerializeField] MeshRenderer propRenderer;
        [SerializeField] int propAuthorMaterialIndex = 1;
        [SerializeField] Material[] authorMaterials;
        [SerializeField] float materialFadeMultiplier = 0.25f;
        [SerializeField] float materialFadeDuration = 1;

        [Header("Settings")]
        [Tooltip("The XML file for the commentary dialog, automatically copied to conversation tree")]
        [SerializeField] TextAsset dialogXml;
        [Tooltip("The author of the commentary, automatically sets prop material")]
        [SerializeField] CommentaryAuthor author;
        [Tooltip("The name of the signal emitted by this commentary. Should be the topic discussed by the commentary. Must be unique")]
        [SerializeField] string signalName = "Commentary Topic";
        [Tooltip("The range at which the signal is detected. Leave at default unless you have a reason to change it")]
        [SerializeField] float signalDetectionRange = 50;
        [Tooltip("An array of demonstration components this commentary can activate")]
        [SerializeField] DeveloperCommentaryDemonstration[] demonstrations;

        AudioSignal signal;
        Vector3 initialAttentionPoint;
        bool commentaryRead;

        void Start()
        {
            signal = OWJam5ModProject.Instance.NewHorizons.SpawnSignal(OWJam5ModProject.Instance, gameObject, SIGNAL_AUDIO, signalName, SIGNAL_FREQUENCY_NAME, detectionRadius:signalDetectionRange, identificationRadius: 3);
            signal._signalVolume = 0.5f;
            signal.transform.parent = transform;

            dialogTree.OnAdvancePage += DialogTree_OnAdvancePage;
            dialogTree.OnEndConversation += DialogTree_OnEndConversation;
            dialogTree.OnStartConversation += DialogTree_OnStartConversation;

            OWJam5ModProject.Instance.OnConfigurationChanged += OnConfigurationChanged;

            initialAttentionPoint = dialogTree._attentionPoint.localPosition;

            UpdateCommentaryRead();
            
            SetCommentaryEnabled(OWJam5ModProject.Instance.ModHelper.Config.GetSettingsValue<bool>(DEVELOPER_COMMENTARY_OPTION));
        }

        void OnDestroy()
        {
            OWJam5ModProject.Instance.OnConfigurationChanged -= OnConfigurationChanged;
        }

        void OnValidate()
        {
            UpdateAuthorMaterial();

            if (dialogXml != null)
                dialogTree._xmlCharacterDialogueAsset = dialogXml;
        }

        public void MoveAttentionPoint(Transform target)
        {
            dialogTree._attentionPoint.transform.position = target.position;
        }

        public void ResetAttentionPoint()
        {
            dialogTree._attentionPoint.transform.localPosition = initialAttentionPoint;
        }

        private void SetCommentaryEnabled(bool value)
        {
            gameObject.SetActive(value);

            if (value)
                signal.IdentifyFrequency();
            else
                PlayerData.ForgetFrequency(signal._frequency);
        }

        private void UpdateAuthorMaterial()
        {
            Material[] sharedMaterials = propRenderer.sharedMaterials;
            sharedMaterials[propAuthorMaterialIndex] = authorMaterials[(int)author];
            propRenderer.sharedMaterials = sharedMaterials;
        }

        private void UpdateCommentaryRead(bool setRead = false)
        {
            bool readPrevious = commentaryRead;

            if (setRead && !readPrevious)
            {
                signal.IdentifySignal();
                commentaryRead = true;
            }
            else
            {
                commentaryRead = PlayerData.KnowsSignal(signal.GetName());
            }

            if (commentaryRead && !readPrevious)
                StartCoroutine(FadeMaterial());
        }

        IEnumerator FadeMaterial()
        {
            Color initialColor = propRenderer.materials[propAuthorMaterialIndex].GetColor(EMISSION_COLOR_PARAMETER);
            Color targetColor = initialColor * materialFadeMultiplier;

            float t = 0;
            while (t < 1)
            {
                propRenderer.materials[propAuthorMaterialIndex].SetColor(EMISSION_COLOR_PARAMETER, Color.Lerp(initialColor, targetColor, t));
                t += Time.deltaTime / materialFadeDuration;

                yield return new WaitForEndOfFrame();
            }

            propRenderer.materials[propAuthorMaterialIndex].SetColor(EMISSION_COLOR_PARAMETER, targetColor);
        }

        private void OnConfigurationChanged(IModConfig config)
        {
            bool commentaryEnabled = config.GetSettingsValue<bool>(DEVELOPER_COMMENTARY_OPTION);
            SetCommentaryEnabled(commentaryEnabled);
        }

        private void DialogTree_OnStartConversation()
        {
            RequirementsScreenPrompt.CommentaryDialogStarted();
        }

        private void DialogTree_OnEndConversation()
        {
            ResetAttentionPoint();
            UpdateCommentaryRead(true);
            RequirementsScreenPrompt.CommentaryDialogEnded();
        }

        private void DialogTree_OnAdvancePage(string nodeName, int pageNum)
        {
            foreach (DeveloperCommentaryDemonstration demonstration in demonstrations)
            {
                demonstration.CheckActivation(this, dialogTree);
                demonstration.CheckDeactivation(this, dialogTree);
            }
        }
    }
}
