using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace OWJam5ModProject
{
    public class UnityEventTriggerVolume : MonoBehaviour
    {
        private OWTriggerVolume _triggerVolume;
        public UnityEvent onEnter;

        public void Start()
        {
            _triggerVolume = base.gameObject.GetAddComponent<OWTriggerVolume>();
            _triggerVolume.OnEntry += OnEntry;
        }
        private void OnEntry(GameObject hitObj)
        {
            if (hitObj.CompareTag("PlayerDetector"))
            {
                onEnter.Invoke();
            }
        }
    }
}
