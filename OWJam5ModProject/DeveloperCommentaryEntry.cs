using NewHorizons.Components.Props;
using OWML.Common;
using OWML.ModHelper;
using OWML.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OWJam5ModProject
{
    internal class DeveloperCommentaryEntry : MonoBehaviour
    {
        public enum CommentaryAuthor { Walker, Cleric, Jamie, John}

        const string SIGNAL_FREQUENCY_NAME = "Developer Commentary";
        const string SIGNAL_AUDIO = "TH_RadioSignal_LP";
        const string DEVELOPER_COMMENTARY_OPTION = "developerCommentary";

        [Header("Required References")]
        [SerializeField] NHCharacterDialogueTree dialogTree;
        [SerializeField] MeshRenderer propRenderer;
        [SerializeField] int propAuthorMaterialIndex = 1;
        [SerializeField] Material[] authorMaterials;

        [Header("Settings")]
        [SerializeField] TextAsset dialogXml;
        [SerializeField] CommentaryAuthor author;
        [SerializeField] string signalName = "Commentary Topic";
        [SerializeField] float signalDetectionRange = 50;
        [SerializeField] DeveloperCommentaryDemonstration[] demonstrations;

        bool commentaryEnabled;
        AudioSignal signal;
        Vector3 initialAttentionPoint;

        void Start()
        {
            signal = OWJam5ModProject.Instance.NewHorizons.SpawnSignal(OWJam5ModProject.Instance, gameObject, SIGNAL_AUDIO, signalName, SIGNAL_FREQUENCY_NAME, detectionRadius:signalDetectionRange, identificationRadius: 3);

            dialogTree.OnAdvancePage += DialogTree_OnAdvancePage;
            dialogTree.OnEndConversation += DialogTree_OnEndConversation;
            OWJam5ModProject.Instance.OnConfigurationChanged += OnConfigurationChanged;

            initialAttentionPoint = dialogTree._attentionPoint.localPosition;
            
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

        private void OnConfigurationChanged(IModConfig config)
        {
            bool commentaryEnabled = config.GetSettingsValue<bool>(DEVELOPER_COMMENTARY_OPTION);
            SetCommentaryEnabled(commentaryEnabled);
        }

        private void DialogTree_OnEndConversation()
        {
            ResetAttentionPoint();

            signal.IdentifySignal();
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
