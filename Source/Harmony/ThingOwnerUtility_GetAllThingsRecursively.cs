﻿using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Verse;

namespace HumanResources
{
    //Excludes books from colony wealth calculation.
    [HarmonyPatch]
    public static class ThingOwnerUtility_GetAllThingsRecursively
    {
        static MethodBase TargetMethod()
        {
            MethodInfo target = typeof(ThingOwnerUtility).GetTypeInfo().GetDeclaredMethods("GetAllThingsRecursively").Where(x => x.IsGenericMethod).FirstOrDefault();
            return target.MakeGenericMethod(typeof(Thing));
        }

        public static void Postfix(ThingRequest request, List<Thing> outThings)
        {
            Func<Thing, bool> books = (Thing t) => t.Stuff != null && t.Stuff.IsWithinCategory(TechDefOf.Knowledge);
            if (request.group == ThingRequestGroup.HaulableEver && outThings.Any(books))
            {
                var preList = outThings;
                outThings.RemoveAll(new Predicate<Thing>(books));
            }
        }
    }
}