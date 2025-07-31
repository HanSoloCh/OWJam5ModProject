using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using NewHorizons;
using NewHorizons.Components.Orbital;
using NewHorizons.Components.SizeControllers;
using NewHorizons.Utility;
using NewHorizons.Utility.OWML;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OWJam5ModProject
{
    public class OWJam5ModProject : ModBehaviour
    {
        public delegate void ConfigurationChangedDelegate(IModConfig config);
        public ConfigurationChangedDelegate OnConfigurationChanged;

        public static OWJam5ModProject Instance;
        public INewHorizons NewHorizons;
        private List<GameObject> planetPivots = null;
        public bool BundlesBroken = false;

        public void Awake()
        {
            Instance = this;
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
        }

        public void Start()
        {

            // Starting here, you'll have access to OWML's mod helper.
            ModHelper.Console.WriteLine($"My mod {nameof(OWJam5ModProject)} is loaded!", MessageType.Success);

            // Get the New Horizons API and load configs
            NewHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            NewHorizons.LoadConfigs(this);

            new Harmony("2walker2.OWJam5ModProject").PatchAll(Assembly.GetExecutingAssembly());

            // Example of accessing game code.
            OnCompleteSceneLoad(OWScene.TitleScreen, OWScene.TitleScreen); // We start on title screen
            LoadManager.OnCompleteSceneLoad += OnCompleteSceneLoad;

            NewHorizons.GetStarSystemLoadedEvent().AddListener((string system) => { OnStarSystemLoaded(system); });
        }

        public void OnCompleteSceneLoad(OWScene previousScene, OWScene newScene)
        {
            if (newScene != OWScene.SolarSystem) return;
            ModHelper.Console.WriteLine("Loaded into solar system!", MessageType.Success);
        }

        public override void Configure(IModConfig config)
        {
            base.Configure(config);

            OnConfigurationChanged?.Invoke(config);
        }

        void OnStarSystemLoaded(string system)
        {
            if (system != "Jam5")
            {
                FinalRequirementManager.inJamSystem = false;
                return;
            }

            BundleBrokenMessage.BeginCheck();

            RotateSystem();
            FinalRequirementManager.inJamSystem = true;
            InitializeFunnels();
            //ReparentPlanets();
            NomaiWarpTransmitterSwapper.Apply();
            TurnWaterOff.Apply();
            ShadowFixer.Apply();
            FinalRequirementManager.Initialize();

            FindObjectOfType<ShipBody>().gameObject.AddComponent<ShipContactSensor>();
            FindObjectOfType<Signalscope>().gameObject.AddComponent<DeveloperCommentaryWarpController>();
            
            // for some reason station has lights enabled, so force this ourselves
            NewHorizons.GetPlanet("Walker_Jam5_Station").GetComponentInChildren<SectorLightsCullGroup>().SetShining(false);
        }

        /// <summary>
        /// Sets up funnels that activate based on planet proximity
        /// </summary>
        const string SAND_FUNNEL_PATH = "Walker_Jam5_Planet3Funnel_Body";
        const string SAND_SOURCE_PATH = "Walker_Jam5_Planet3_Body/Sector/Sand";
        const string SAND_TARGET_PATH = "Walker_Jam5_Planet2_Body/Sector/Sand";
        public const float SAND_DRAINED_HEIGHT = 137.5f * 2; // Sand sphere's scale is twice its radius
        const float SAND_FILLED_HEIGHT = 255 * 2;

        const string WATER_FUNNEL_NAME = "Walker_Jam5_Planet4Funnel_Body";
        const string WATER_SOURCE_PATH = "Walker_Jam5_Planet4_Body/Sector/Water";
        const string WATER_TARGET_PATH = "Walker_Jam5_Planet2_Body/Sector/Water";
        public const float WATER_DRAINED_HEIGHT = 137;
        public const float WATER_FILLED_HEIGHT = 259;
        const float WATER_FILLED_ADDITIONAL_HEIGHT = 10;
        void InitializeFunnels()
        {
            // Sand funnel
            GameObject sandFunnel = SearchUtilities.Find(SAND_FUNNEL_PATH);
            FunnelProximityActivator sandFunnelProximity = null;
            if (sandFunnel != null)
            {
                sandFunnelProximity = sandFunnel.AddComponent<FunnelProximityActivator>();
                GameObject sandSource = SearchUtilities.Find(SAND_SOURCE_PATH);
                GameObject sandTarget = SearchUtilities.Find(SAND_TARGET_PATH);
                sandFunnelProximity.Initialize(sandSource, SAND_DRAINED_HEIGHT, sandTarget, SAND_FILLED_HEIGHT);
            }

            // Water funnel
            GameObject waterFunnel = SearchUtilities.Find(WATER_FUNNEL_NAME);
            if (waterFunnel != null)
            {
                FunnelProximityActivator waterFunnelProximity = waterFunnel.AddComponent<FunnelProximityActivator>();
                GameObject waterSource = SearchUtilities.Find(WATER_SOURCE_PATH);
                GameObject waterTarget = SearchUtilities.Find(WATER_TARGET_PATH);
                waterFunnelProximity.Initialize(waterSource, WATER_DRAINED_HEIGHT, waterTarget, WATER_FILLED_HEIGHT, sandFunnelProximity, WATER_FILLED_ADDITIONAL_HEIGHT);
            }
        }

        /// <summary>
        /// rotate our system to face towards the station always, to catch the eye
        /// </summary>
        void RotateSystem()
        {
            // evil code. i dont care. its fast

            var stationTf = GameObject.Find("CentralStation_Body").transform;
            
            //Get the star transform
            Transform starTF = NewHorizons.GetPlanet("Walker_Jam5_Star").transform;

            foreach (NHAstroObject planet in FindObjectsOfType<NHAstroObject>())
            {
                //Exclude other mods, our star, and our platform
                if (planet.modUniqueName.Equals("2walker2.OWJam5ModProject")
                    && planet.transform != starTF 
                    && !planet._customName.Equals("Walker_Jam5_Platform"))
                {
                    planet.transform.parent = starTF; // parent
                }
            }

            var fromDir = starTF.up;
            var toDir = stationTf.position - starTF.position;
            starTF.Rotate(Vector3.up, Vector3.SignedAngle(fromDir.normalized, toDir.normalized, Vector3.up), relativeTo: Space.World);
            
            foreach (NHAstroObject planet in FindObjectsOfType<NHAstroObject>())
            {
                //Exclude other mods, our star, and our platform
                if (planet.modUniqueName.Equals("2walker2.OWJam5ModProject")
                    && planet.transform != starTF 
                    && !planet._customName.Equals("Walker_Jam5_Platform"))
                {
                    planet.transform.parent = null; // unparent
                }
            }
        }

        /**
         * Makes our planets all children of the sun
         * 
         * (should be fine since they're kinematic)
         */
        private void ReparentPlanets()
        {
            //Get the star transform
            Transform starTF = NewHorizons.GetPlanet("Walker_Jam5_Star").transform;

            //Find our planets and reparent them
            planetPivots = new List<GameObject>();
            foreach (NHAstroObject planet in FindObjectsOfType<NHAstroObject>())
            {
                //Exclude other mods, our star, and our platform
                if (planet.modUniqueName.Equals("2walker2.OWJam5ModProject") && planet.transform != starTF 
                    && !planet._customName.Equals("Walker_Jam5_Platform"))
                {
                    DebugLog("Found planet " + planet.name);
                    GameObject pivot = new GameObject(planet._customName + "_pivot");
                    pivot.transform.parent = starTF;
                    pivot.transform.localPosition = Vector3.zero;
                    planet.transform.SetParent(pivot.transform, true);
                    planetPivots.Add(pivot);
                }
            }
        }

        /**
         * Just some debug keybinds
         */
        private void Update()
        {
            if (Keyboard.current[Key.K].IsPressed())
            {
                DebugLog((NewHorizons.GetPlanet("Walker_Jam5_Star").transform.position 
                    - NewHorizons.GetPlanet("Walker_Jam5_Planet4").transform.position).magnitude.ToString());
            }
        }


        private void OnGui()
        {
            if (BundlesBroken)
            {
                GUI.Box(new Rect(10, 10, 400, 45), "Assetbundles not loaded for Heliostudy!\nThis was likely caused by the New Horizons \"Reload Config\" option.");
            }
        }
        /**
         * Print a string to the console
         */
        public static void DebugLog(string msg)
        {
            Instance.ModHelper.Console.WriteLine(msg);
        }
    }
}
