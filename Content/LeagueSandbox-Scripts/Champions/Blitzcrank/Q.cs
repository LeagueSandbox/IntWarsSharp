using System.Numerics;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;

namespace Spells
{
    public class RocketGrab : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata => new SpellScriptMetadata()
        {
            // TODO
        };

        public void OnActivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnDeactivate(IObjAiBase owner, ISpell spell)
        {
        }

        public void OnStartCasting(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
            spell.SpellAnimation("SPELL1", owner);
        }

        public void OnFinishCasting(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
            var current = new Vector2(owner.Position.X, owner.Position.Y);
            var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
            var to = Vector2.Normalize(spellPos - current);
            var range = to * 925;
            var trueCoords = current + range;
            spell.AddProjectile("RocketGrabMissile", current, current, trueCoords);
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, ISpellMissile projectile)
        {
            var ap = owner.Stats.AbilityPower.Total;
            var damage = 25 + spell.CastInfo.SpellLevel * 55 + ap;
            target.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL, false);
            if (!target.IsDead)
            {
                AddParticleTarget(owner, "Blitzcrank_Grapplin_tar.troy", target, 1, "L_HAND");
                var current = new Vector2(owner.Position.X, owner.Position.Y);
                var spellPos = new Vector2(spell.CastInfo.TargetPosition.X, spell.CastInfo.TargetPosition.Z);
                var to = Vector2.Normalize(spellPos - current);
                var range = to * 50;
                var trueCoords = current + range;
                ForceMovement(target, "RUN", trueCoords, spell.SpellData.MissileSpeed, 0, 0, 0, movementOrdersFacing: ForceMovementOrdersFacing.KEEP_CURRENT_FACING);
            }

            projectile.SetToRemove();
        }

        public void OnUpdate(float diff)
        {
        }
    }
}

