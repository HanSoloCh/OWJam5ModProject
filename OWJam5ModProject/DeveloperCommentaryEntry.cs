using System;
using UnityEngine;
using OWML.Utils;
using NewHorizons.Components.Props;

namespace OWJam5ModProject
{
    internal class DeveloperCommentaryEntry : MonoBehaviour
    {
        const string SIGNAL_FREQUENCY_NAME = "Developer Commentary";
        const string SIGNAL_AUDIO = "TH_RadioSignal_LP";

        [Serializable]
        public struct CommentaryDemonstration
        {
            public string nodeName;
            public int dialogPage;
            public Transform attentionPoint;
            public Transform enableRoot;
        }

        [Header("Required References")]
        [SerializeField] NHCharacterDialogueTree dialogTree;

        [Header("Settings")]
        [SerializeField] string signalName = "COMMENTARY TOPIC";
        [SerializeField] CommentaryDemonstration[] commentaryDemonstrations;

        AudioSignal signal;

        void Start()
        {
            signal = OWJam5ModProject.Instance.NewHorizons.SpawnSignal(OWJam5ModProject.Instance, gameObject, SIGNAL_AUDIO, signalName, SIGNAL_FREQUENCY_NAME, identificationRadius: 3);
        }
    }
}
