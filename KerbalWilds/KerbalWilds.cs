using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using System.Reflection;
using NewHorizons.Handlers;
using UnityEngine;

namespace KerbalWilds
{
    public class KerbalWilds : ModBehaviour
    {
        public static KerbalWilds Instance;
        public INewHorizons NewHorizons;
        public bool InSystem;

        public void Awake()
        {
            Instance = this;
            // You won't be able to access OWML's mod helper in Awake.
            // So you probably don't want to do anything here.
            // Use Start() instead.
        }

        public void Start()
        {


            // Get the New Horizons API and load configs
            NewHorizons = ModHelper.Interaction.TryGetModApi<INewHorizons>("xen.NewHorizons");
            NewHorizons.LoadConfigs(this);

            new Harmony("Trifid.KerbalWilds").PatchAll(Assembly.GetExecutingAssembly());

            NewHorizons.GetStarSystemLoadedEvent().AddListener(system =>
            {
                if (system != "Kerbol System")
                {
                    InSystem = false;
                    return;
                }
                InSystem = true;

                var moho = NewHorizons.GetPlanet("Moho");
                var eve = NewHorizons.GetPlanet("Eve");
                var gilly = NewHorizons.GetPlanet("Gilly");
                var kerbin = NewHorizons.GetPlanet("Kerbin");
                var mun = NewHorizons.GetPlanet("The Mun");
                var minmus = NewHorizons.GetPlanet("Minmus");
                var duna = NewHorizons.GetPlanet("Duna");
                var ike = NewHorizons.GetPlanet("Ike");
                var dres = NewHorizons.GetPlanet("Dres");
                var jool = NewHorizons.GetPlanet("Jool");
                var laythe = NewHorizons.GetPlanet("Laythe");
                var vall = NewHorizons.GetPlanet("Vall");
                var tylo = NewHorizons.GetPlanet("Tylo");
                var bop = NewHorizons.GetPlanet("Bop");
                var pol = NewHorizons.GetPlanet("Pol");
                var eeloo = NewHorizons.GetPlanet("Eeloo");

                moho.transform.Find("GravityWell").GetComponent<GravityVolume>()._alignmentPriority = 1;

                kerbin.transform.Find("Sector/AmbientLight_TH").GetComponent<Light>().intensity = 1;
                kerbin.transform.Find("Sector/DayAmbience").GetComponent<AudioSource>().spatialBlend = 0;
                
                var dunaAlign = duna.GetAddComponent<AlignWithTargetBody>();
                dunaAlign.SetTargetBody(ike.GetAttachedOWRigidbody());
                dunaAlign._localAlignmentAxis = new Vector3(0,0,-1);
                dunaAlign._usePhysicsToRotate = true;


            });
        }
    }
}