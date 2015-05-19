﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ContractConfigurator
{
    /// <summary>
    /// Class for representing a biome where science can be done.
    /// </summary>
    public class Biome
    {
        private static List<string> kscBiomes = null;

        public static IEnumerable<string> KSCBiomes
        {
            get
            {
                if (HighLogic.CurrentGame == null)
                {
                    return Enumerable.Empty<string>();
                }

                return kscBiomes = kscBiomes ?? UnityEngine.Object.FindObjectsOfType<Collider>()
                    .Where(x => x.gameObject.layer == 15)
                    .Select(x => x.gameObject.tag)
                    .Where(x => x != "Untagged")
                    .Where(x => !x.Contains("KSC_Runway_Light"))
                    .Where(x => !x.Contains("KSC_Pad_Flag_Pole"))
                    .Where(x => !x.Contains("Ladder"))
                    .Select(x => Vessel.GetLandedAtString(x))
                    .Select(x => x.Replace(" ", ""))
                    .Distinct()
                    .ToList();
            }
        }

        public CelestialBody body;
        public string biome;

        public Biome(CelestialBody body, string biome)
        {
            this.body = body;
            this.biome = biome;
        }

        public override string ToString()
        {
            if (biome == "KSC")
            {
                return biome;
            }

            return (body == null ? "" : IsKSC() ? "KSC's " : (body.theName + "'s ")) + PrintBiomeName(biome);
        }

        public bool IsKSC()
        {
            if (body == null || body.BiomeMap == null || !body.isHomeWorld)
            {
                return false;
            }

            return !body.BiomeMap.Attributes.Any(attr => attr.name.Replace(" ", string.Empty) == biome);
        }

        public static string PrintBiomeName(string biome)
        {
            return Regex.Replace(biome, @"([A-Z&]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1");
        }
    }
}
