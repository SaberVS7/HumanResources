﻿using RimWorld;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace HumanResources
{
    class JobDriver_PlayWithWeapon : JobDriver_WatchBuilding
    {
        protected int ticksSpentAlready = 0;

        protected override void WatchTickAction()
        {
            ticksSpentAlready++;

            //pawn posture
            Verb verbToUse = pawn.TryGetAttackVerb(pawn.equipment?.Primary, true);
            LocalTargetInfo target = pawn.jobs.curJob.GetTarget(TargetIndex.A);
            pawn.stances.SetStance(new Stance_Warmup(1, target, verbToUse));

            //sound:
            if (verbToUse.verbProps != null && verbToUse.verbProps.warmupTime > 0)
            {
                if ((ticksSpentAlready % verbToUse.verbProps.AdjustedFullCycleTime(verbToUse, pawn).SecondsToTicks()) == 0)
                {
                    if (verbToUse.verbProps.soundCast != null)
                    {
                        verbToUse.verbProps.soundCast.PlayOneShot(new TargetInfo(base.pawn.Position, base.pawn.Map, false));
                    }
                    if (verbToUse.verbProps.soundCastTail != null)
                    {
                        verbToUse.verbProps.soundCastTail.PlayOneShotOnCamera(base.pawn.Map);
                    }
                }
            }
            base.WatchTickAction();
        }
    }
}
