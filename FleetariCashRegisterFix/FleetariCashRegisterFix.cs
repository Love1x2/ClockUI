using MSCLoader;
using UnityEngine;

namespace FleetariCashRegisterFix
{
    public class FleetariCashRegisterFix : Mod
    {
        public override string ID => "FleetariCashRegisterFix";
        public override string Name => "FleetariCashRegisterFix";
        public override string Author => "Love1x2";
        public override string Version => "1.1";
        public override string Description => "Fixes the cash register at Fleetari's repair shop.";

        private SphereCollider fleetariRegister;

        private bool registerFixed = false;

        public override void ModSetup()
        {
            SetupFunction(Setup.Update, Mod_Update);
        }

        private void Mod_Update()
        {
            // Finds, resizes and offsets the cash register SphereCollider component.
            if (registerFixed == false && GameObject.Find("ShopCashRegister/Register") != null)
            {
                fleetariRegister = GameObject.Find("ShopCashRegister/Register").GetComponent<UnityEngine.SphereCollider>();
                fleetariRegister.radius = 50.0f;
                fleetariRegister.center = new Vector3(0.0f, 1.0f, 0.0f);
                registerFixed = true;
            }
        }
    }
}
