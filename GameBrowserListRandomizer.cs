using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HarmonyLib;
using SML;
using System;
using TownieLib;
using UnityEngine;

using Server.Shared.State;
using Home.GameBrowser;
using BetterTOS2;

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

    [HarmonyPatch(typeof(RoleDeckBuilder), "GetSortedRoleDeckSlots")]
    class GetSortedRoleDeckSlots_Patch 
    {
        public static bool Prefix(ref List<RoleDeckSlot> __result)
        {
            GamePhase gamePhase = Pepper.GetGamePhase();

            if (!ModSettings.GetBool("Randomize In-Game") && (gamePhase == GamePhase.PICK_NAMES || gamePhase == GamePhase.PLAY || gamePhase == GamePhase.CASUAL_VOTE)) return true;

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) return true;

            List<RoleDeckSlot> returnDeck = new List<RoleDeckSlot>();
            
            System.Random rand = new System.Random();
            Array roles = Enum.GetValues(typeof(Role));
            Array factions = getFactions();
            int numRoles = ModSettings.GetInt("# of roles in list");

            for (int i = 0; i < numRoles; i++)
            {
                Role randRole1 = (Role)roles.GetValue(rand.Next(roles.Length));
                while (!checkRole(randRole1)) randRole1 = (Role)roles.GetValue(rand.Next(roles.Length));
                Role randRole2 = (Role)roles.GetValue(rand.Next(roles.Length));
                while (!checkRole(randRole2)) randRole2 = (Role)roles.GetValue(rand.Next(roles.Length));
                FactionType randFaction1 = (FactionType)factions.GetValue(rand.Next(factions.Length));
                while (!checkFaction(randFaction1)) randFaction1 = (FactionType)factions.GetValue(rand.Next(factions.Length));
                FactionType randFaction2 = (FactionType)factions.GetValue(rand.Next(factions.Length));
                while (!checkFaction(randFaction2)) randFaction2 = (FactionType)factions.GetValue(rand.Next(factions.Length));

                returnDeck.Add(new RoleDeckSlot(randRole1, randRole2, randFaction1, randFaction2));
            }

            __result = returnDeck;

            return false;
        }

        private static bool checkRole(Role role)
        {
            int roleNum = (int)role;
            if (BTOSInfo.IS_MODDED)
            {
                if (btosDisallowedRoles.Contains(roleNum)) return false;
                if ((int)RolePlus.ROLE_COUNT <= roleNum && roleNum <= 99) return false;
                if (122 <= roleNum && roleNum <= 249) return false;
                return true;
            }
            if ((int)Role.ROLE_COUNT <= roleNum && roleNum <= 99) return false;
            if (118 <= roleNum && roleNum <= 200) return false;
            if (218 <= roleNum && roleNum <= 240) return false;
            if (242 <= roleNum && roleNum <= 249) return false;
            return true;
        }

        private static bool checkFaction(FactionType faction)
        {
            int factionNum = (int)faction;
            if (BTOSInfo.IS_MODDED)
            {
                if ((int)FactionTypePlus.FACTION_COUNT <= factionNum) return false;
                return true;
            }
            if (14 <= factionNum) return false;
            return true;
        }

        private static Array getFactions()
        {
            if (BTOSInfo.IS_MODDED)
            {
                return typeof(FactionTypePlus)
                    .GetFields(BindingFlags.Public | BindingFlags.Static)
                    .Where(f => f.FieldType == typeof(FactionType))
                    .Select(f => (FactionType)f.GetValue(null))
                    .ToArray();
            }
            return Enum.GetValues(typeof(FactionType));
        }

        private static List<int> btosDisallowedRoles = new List<int>(){0, 249, 250, 252, 253, 254, 255};
    }

}
