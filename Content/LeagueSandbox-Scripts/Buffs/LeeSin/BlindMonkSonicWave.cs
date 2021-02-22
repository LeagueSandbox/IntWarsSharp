﻿using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using LeagueSandbox.GameServer.GameObjects.Stats;
using LeagueSandbox.GameServer.Scripting.CSharp;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using GameServerCore.Domain.GameObjects.Spell;

namespace BlindMonkSonicWave
{
    internal class BlindMonkSonicWave : IBuffGameScript
    {
        public BuffType BuffType => BuffType.INTERNAL;
        public BuffAddType BuffAddType => BuffAddType.REPLACE_EXISTING;
        public bool IsHidden => false;
        public int MaxStacks => 1;

        public IStatsModifier StatsModifier { get; private set; }

        ISpell originSpell;
        IBuff thisBuff;

        public void OnActivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
            originSpell = ownerSpell;
            thisBuff = buff;
        }

        public void OnDeactivate(IAttackableUnit unit, IBuff buff, ISpell ownerSpell)
        {
        }

        public void OnUpdate(float diff)
        {
            if (thisBuff == null || originSpell == null || thisBuff.Elapsed())
            {
                return;
            }

            var owner = originSpell.CastInfo.Owner;
            var target = thisBuff.TargetUnit;
            if (owner.IsDashing && owner.IsCollidingWith(target))
            {
                owner.SetDashingState(false);
                var ad = owner.Stats.AttackDamage.Total * 1.0f;
                var damage = 50 + (originSpell.CastInfo.SpellLevel * 30) + ad + (0.08f * (target.Stats.HealthPoints.Total - target.Stats.CurrentHealth));
                target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_PHYSICAL, DamageSource.DAMAGE_SOURCE_REACTIVE, false);
                AddParticleTarget(owner, "GlobalHit_Yellow_tar.troy", target, 1);

                thisBuff.DeactivateBuff();
            }
        }
    }
}