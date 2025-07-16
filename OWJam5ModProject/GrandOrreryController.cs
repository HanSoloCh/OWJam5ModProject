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
using UnityEngine.InputSystem;
using UnityEngine.Events;

namespace OWJam5ModProject
{
    public class GrandOrreryController : MonoBehaviour
    {
        public INewHorizons NewHorizons;
        string systemName = "Jam5";
        public GameObject ernesto;
        public GameObject instantiateOrb;
        public Transform spinRoot;
        public Transform orbSpawnRoot;
        public Transform orbParent;
        public Transform alignParent;
        Transform player;
        public UnityEvent onEnable;
        NomaiComputer computer;
        bool CanActivate = false;



        PlayerSpacesuit suit;
        public float downScale = 0.0005f;

        ReferenceFrameVolume ernestoRefFrame;

        new List<GrandOrb> grandOrbs = new();

        public class SystemAndBodies
        {
            public NewHorizonsBody centerBody;
            public List<NewHorizonsBody> childBodies = new();
            public string modName;
        }
        public List<SystemAndBodies> systems = new();
        bool Activated = false;

        public void Start()
        {
            ernestoRefFrame = ernesto.GetComponent<ReferenceFrameVolume>();
            ernesto.SetActive(false);
            computer = GetComponentInChildren<NomaiComputer>();
            Activated = false;
            suit = Locator._playerSuit;
            player = Locator.GetPlayerBody().transform;
            Apply(Main.BodyDict[systemName]);
            PlaceOrbs();

        }

        public void SetCanActivate()
        {
            CanActivate = true;
        }

        public void PlaceOrbs()
        {
            foreach (SystemAndBodies s in systems)
            {
                GameObject newOrb = instantiateOrb.InstantiateInactive();
                newOrb.transform.parent = orbParent;
                newOrb.transform.position = orbParent.transform.position;
                GrandOrb o = newOrb.GetComponent<GrandOrb>();
                grandOrbs.Add(o);
                o.InitializeOrb(alignParent);
                o.system = s;
                o.enabled = false;
            }
            EnableOrbs();
            Invoke("PositionOrbs", 1f);
            Invoke("SetCanActivate", 5f);

        }

        public void EnableOrbs()
        {
            foreach (GrandOrb orb in grandOrbs)
            {
                orb.gameObject.SetActive(true);
            }
        }

        public void PositionOrbs()
        {
            computer.ClearAllEntries();
            foreach (GrandOrb o in grandOrbs)
            {
                spinRoot.Rotate(0, UnityEngine.Random.Range(0f, 360f), 0);
                orbSpawnRoot.transform.localPosition = new Vector3(UnityEngine.Random.Range(3f, 7f), 0, 0);
                o.transform.position = orbSpawnRoot.transform.position;
                o.GetComponent<FakeOrbSlot>()._localLockPos = orbParent.InverseTransformPoint(o.transform.position);
            }
        }

        public void StartGrandOrrery()
        {
            computer.DisplayEntry(1);
            onEnable.Invoke();

            GameObject centralStation = GameObject.Find("CentralStation_Body");

            alignParent.rotation = Quaternion.identity;
            
            

            foreach (GrandOrb orb in grandOrbs)
            {
                orb.SetIsAbsolutelyActive(true);
                Vector3 starPos = centralStation.transform.InverseTransformPoint(orb.system.centerBody.Object.gameObject.transform.position);
                OWJam5ModProject.DebugLog(starPos.ToString());
                Vector3 toPos = alignParent.TransformPoint(starPos * downScale);
                orb.transform.position = new Vector3(toPos.x, orb.transform.position.y, toPos.z);
                orb.GetComponent<FakeOrbSlot>()._localLockPos = orbParent.InverseTransformPoint(orb.transform.position);
                orb.enabled = true;

            }
        }

        public void Update()
        {
            if (Vector3.Distance(player.position, transform.position) < 30)
            {
                if (!Activated && CanActivate)
                {
                    if (!suit._isWearingSuit)
                    {
                        float min = 999;
                        float max = 0;
                        foreach (GrandOrb orb in grandOrbs)
                        {
                            float dist = Vector3.Distance(orb.transform.position, alignParent.position);
                            if (dist < min) min = dist;
                            if (dist > max) max = dist;
                        }
                        if ((max - min) < 0.15f && min > 0.5f)
                        {
                            Activated = true;
                            StartGrandOrrery();
                        }
                    }
                }
                Vector3 ourSystemPosition = Vector3.zero;
                Vector3 avg = Vector3.zero;
                foreach (SystemAndBodies b in systems)
                {
                    avg += b.centerBody.Object.transform.position;
                    if (b.modName == "2walker2.OWJam5ModProject") ourSystemPosition = b.centerBody.Object.transform.position;
                }
                avg /= systems.Count;
                ernesto.transform.position = avg;
                ernesto.SetActive(Vector3.Distance(ernesto.transform.position, ourSystemPosition) < 2500);
                ernestoRefFrame._referenceFrame._localPosition = ernestoRefFrame._attachedOWRigidbody.transform.InverseTransformPoint(ernesto.transform.position);
            }

        }

        public void Apply(List<NewHorizonsBody> bodies)
        {
            // get all the systems
            foreach(NewHorizonsBody body in bodies)
            {
                // stolen code from jam 5 base mod
                var dict = new Dictionary<string, object>();
                if (body.Config.extras is JObject jObject)
                {
                    dict = jObject.ToObject<Dictionary<string, object>>();
                }

                if (dict.TryGetValue("isCenterOfMiniSystem", out var isCenter) && isCenter is bool isCenterBool && isCenterBool)
                {
                    var systemAndBodies = new SystemAndBodies();
                    systemAndBodies.centerBody = body;
                    systemAndBodies.modName = body.Mod.ModHelper.Manifest.UniqueName;
                    systemAndBodies.childBodies = bodies.Where(x => x != body && Vector3.Distance(x.Object.transform.position, body.Object.transform.position) < 2500).ToList();
                    systems.Add(systemAndBodies);
                }
            }
            
            // create orbs based on their positions
        }
    }
}
