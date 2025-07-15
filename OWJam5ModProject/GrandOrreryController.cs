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

namespace OWJam5ModProject
{
    public class GrandOrreryController : MonoBehaviour
    {
        public INewHorizons NewHorizons;
        string systemName = "Jam5";

        public class SystemAndBodies
        {
            public NewHorizonsBody centerBody;
            public List<NewHorizonsBody> childBodies = new();
        }
        public List<SystemAndBodies> systems = new();

        public void Start()
        {
            Apply(Main.BodyDict[systemName]);
        }

        public void CreateOrbs()
        {
            
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
                    systemAndBodies.childBodies = bodies.Where(x => x != body && Vector3.Distance(x.Object.transform.position, body.Object.transform.position) < 2500).ToList();
                    systems.Add(systemAndBodies);
                }
            }
            
            // create orbs based on their positions
        }
    }
}
