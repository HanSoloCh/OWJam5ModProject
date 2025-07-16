using NewHorizons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using OWML.Common;
using OWML.ModHelper;
using NewHorizons.Components.Orbital;
using HarmonyLib;
using NewHorizons.External;
using Newtonsoft.Json.Linq;
using NewHorizons.Utility;

namespace OWJam5ModProject
{
    public class GrandOrb : MonoBehaviour
    {
        public GrandOrreryController.SystemAndBodies system;
        GrandOrreryController controller;
        Transform parent;
        Transform player;
        bool IsAbsolutelyActive = false; // im just paranoid

        GameObject center;
        public void Start()
        {
            player = Locator.GetPlayerBody().transform;
            controller = FindObjectOfType<GrandOrreryController>();
            center = GameObject.Find("CentralStation_Body");
        }

        public void SetIsAbsolutelyActive(bool isActive)
        {
            IsAbsolutelyActive = isActive;
        }

        public void InitializeOrb(Transform p)
        {
            parent = p;
        }

        public void FixedUpdate()
        {
            if (Vector3.Distance(player.transform.position, transform.position) < 20 && IsAbsolutelyActive)
            {
                Vector3[] positions = new Vector3[system.childBodies.Count];
                for (int i = 0; i < system.childBodies.Count; i++)
                {
                    positions[i] = transformOf(system.centerBody).InverseTransformPoint(transformOf(system.childBodies[i]).position);
                }

                Vector3 pos = center.transform.TransformPoint(parent.transform.InverseTransformPoint(transform.position) / controller.downScale);
                Transform toMove = system.centerBody.Object.transform;
                pos = new Vector3(pos.x, center.transform.position.y, pos.z);
                system.centerBody.Object.transform.position = pos;

                for (int i = 0; i < system.childBodies.Count; i++)
                {
                    transformOf(system.childBodies[i]).position = transformOf(system.centerBody).TransformPoint(positions[i]);
                }
            }
        }

        public Transform transformOf(NewHorizonsBody b)
        {
            return b.Object.gameObject.transform;
        }
    }
}
