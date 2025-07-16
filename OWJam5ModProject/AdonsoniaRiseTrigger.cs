using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OWJam5ModProject
{
    public class AdonsoniaRiseTrigger : MonoBehaviour
    {
        [SerializeField] private Animator animator = null;
        [SerializeField] private OWTriggerVolume primeTrigger = null;
        [SerializeField] private OWTriggerVolume riseTrigger = null;

        private bool primed = false;
        private bool triggered = false;

        /**
         * Set up the events for making Adonsonia rise up
         */
        private void Start()
        {
            primeTrigger.OnEntry += OnPlayerEnterPrime;
            riseTrigger.OnEntry += OnPlayerEnterRise;
        }

        /**
         * When the player hits the prime trigger, mark the bool
         */
        private void OnPlayerEnterPrime(GameObject other)
        {
            if (other.CompareTag("PlayerDetector"))
                primed = true;
        }

        /**
         * When the player hits the rise trigger, raise her up if we're primed
         */
        private void OnPlayerEnterRise(GameObject other)
        {
            if (primed && !triggered && other.CompareTag("PlayerDetector"))
            {
                triggered = true;
                animator.SetTrigger("rise");
            }
        }

        /**
         * If destroyed, unlink
         */
        private void OnDestroy()
        {
            primeTrigger.OnEntry -= OnPlayerEnterPrime;
            riseTrigger.OnEntry -= OnPlayerEnterRise;
        }
    }
}
