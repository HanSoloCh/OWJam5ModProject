using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWJam5ModProject
{
    class AdonsoniaConversationController : MonoBehaviour
    {
        private GameObject dialogueTrigger = null;

        /**
         * Grab and disable the dialogue trigger
         */
        private void Start()
        {
            dialogueTrigger = GetComponentInChildren<RemoteDialogueTrigger>().gameObject;
            dialogueTrigger.SetActive(false);
        }

        /**
         * Enable the dialogue trigger
         */
        private void TriggerDialogue()
        {
            dialogueTrigger.SetActive(true);
        }
    }
}
