﻿using UnityEngine;

namespace PaladinMod.States
{
    public class CastHeal : BaseCastSpellState
    {
        public override void OnEnter()
        {
            this.baseDuration = 0.3f;
            this.muzzleflashEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashCroco");
            this.projectilePrefab = Modules.Projectiles.heal;

            base.OnEnter();
        }
    }
}