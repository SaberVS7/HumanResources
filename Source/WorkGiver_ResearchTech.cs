﻿using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.AI;

namespace HumanResources
{
    class WorkGiver_ResearchTech : WorkGiver_Knowledge
    {
		public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
			IEnumerable<ResearchProjectDef> expertise = pawn.GetComp<CompKnowledge>().expertise.Keys;
			IEnumerable<ResearchProjectDef> available = DefDatabase<ResearchProjectDef>.AllDefsListForReading.Where(x => !x.IsFinished).Except(expertise);
			return !available.Any();
		}

		public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			//Log.Message(pawn + " is looking for a research job...");
			Building_WorkTable Desk = t as Building_WorkTable;
			if (Desk != null)
			{
				if (!CheckJobOnThing(pawn, t, forced) && RelevantBills(t/*, RecipeName*/).Count() > 0)
				{
					//Log.Message("...no job on desk.");
					return false;
				}
				List<ResearchProjectDef> studyMaterial = new List<ResearchProjectDef>();
				//Log.Message("...relevant bills: " + RelevantBills(Desk, RecipeName).Count);
				foreach (Bill bill in RelevantBills(Desk/*, RecipeName*/))
				{
					//Log.Message("...checking recipe: " + bill.recipe+", on bill "+bill.GetType());
					//Log.Message("...selected techs count: " + bill.SelectedTech().ToList().Count());
					studyMaterial.AddRange(bill.SelectedTech().Where(x => !x.IsFinished));
				}
				//Log.Message("...studyMaterial count is " + studyMaterial.Count());
				CompKnowledge techComp = pawn.GetComp<CompKnowledge>();
				techComp.AssignHomework(studyMaterial);
				//Log.Message("...homework count is " + techComp.HomeWork.Count());
				//if (techComp.HomeWork.Count() > 0) return true;
				if (studyMaterial.Intersect(techComp.HomeWork).Any()) return true;
				JobFailReason.Is("AlreadyKnowsThoseProjects".Translate(pawn), null);
				return false;
			}
			//Log.Message("case 4");
			return false;

		}

		public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
			IBillGiver billGiver = thing as IBillGiver;
			if (billGiver != null && ThingIsUsableBillGiver(thing) && billGiver.BillStack.AnyShouldDoNow && billGiver.UsableForBillsAfterFueling())
			{
				LocalTargetInfo target = thing;
				if (pawn.CanReserve(target, 1, -1, null, forced) && !thing.IsBurning() && !thing.IsForbidden(pawn))
				{
					billGiver.BillStack.RemoveIncompletableBills();
					foreach (Bill bill in RelevantBills(thing/*, RecipeName*/))
					{
						return new Job(DefDatabase<JobDef>.GetNamed(bill.recipe.defName), target)
						{
							bill = bill
						};
					}
				}
			}
			return null;
		}
	}
}
