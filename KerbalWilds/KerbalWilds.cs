﻿using HarmonyLib;
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
                if (system != "Kerbol System") return;

                var moho = NewHorizons.GetPlanet("Moho");
                moho.transform.Find("GravityWell").GetComponent<GravityVolume>()._alignmentPriority = 1;

                var duna = NewHorizons.GetPlanet("Duna");
                var ike = NewHorizons.GetPlanet("Ike");
                var align = duna.GetAddComponent<AlignWithTargetBody>();
                align.SetTargetBody(ike.GetAttachedOWRigidbody());
                align._localAlignmentAxis = new Vector3(0,0,-1);
                align._usePhysicsToRotate = true;

                var kerbin = NewHorizons.GetPlanet("Kerbin");
                kerbin.transform.Find("AmbientLight_TH").GetComponent<Light>().intensity = 1;
            });
        }
    }
}