using MSCLoader;
using UnityEngine;

namespace FleetariCashRegisterFix
{
    public class FleetariCashRegisterFix : Mod
    {
        public override string ID => "FleetariCashRegisterFix";
        public override string Name => "FleetariCashRegisterFix";
        public override string Author => "Love1x2";
        public override string Version => "1.0";
        public override string Description => "Fixes the cash register at Fleetari's repair shop.";

        private SphereCollider fleetariRegister;

        public override void ModSetup()
        {
            SetupFunction(Setup.PostLoad, Mod_PostLoad);
        }

        private void Mod_PostLoad()
        {
            // Finds, resizes and offsets the cash register SphereCollider component.
            fleetariRegister = GameObject.Find("ShopCashRegister/Register").GetComponent<UnityEngine.SphereCollider>();
            fleetariRegister.radius = 50.0f;
            fleetariRegister.center = new Vector3(0.0f, 1.0f, 0.0f);
        }
    }
}
