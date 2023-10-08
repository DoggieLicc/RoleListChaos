using System.Collections.Generic;
using HarmonyLib;
using SML;
using System;
using TownieLib;
using UnityEngine;

using Server.Shared.State;
using Home.GameBrowser;


namespace GameBrowserListRandomizer
{

    [SML.Mod.SalemMod]
    public class GameBrowserListRandomizer
    {

        public void Start()
        {
            try
            {
                Harmony.CreateAndPatchAll(typeof(GameBrowserListRandomizer));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("common foxfox W");
        }
    }

    [HarmonyPatch(typeof(GameBrowserRoleDeck), "GetSortedRoleDeck")]
    class GetSortedRoleDeck_Patch 
    {
        public static bool Prefix(ref List<Role> __result)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)){
                return true;
            }

            List<Role> returnDeck = new List<Role>();

            System.Random rand = new System.Random();
            Array roles = Enum.GetValues(typeof(Role));
            int numRoles = ModSettings.GetInt("# of roles in list");

            for (int i = 0; i < numRoles; i++)
            {
                Role randRole = (Role)roles.GetValue(rand.Next(roles.Length));
                returnDeck.Add(randRole);
            }

            __result = returnDeck;

            return false;
        }
    }

}
