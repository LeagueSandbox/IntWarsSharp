using System.Linq;
using GameServerCore;
using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.Scripting.CSharp;
using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Domain.GameObjects.Spell.Missile;

namespace Spells
{
    public class FallenOne : ISpellScript
    {
        public ISpellScriptMetadata ScriptMetadata { get; private set; } = new SpellScriptMetadata()
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
            foreach (var enemyTarget in GetChampionsInRange(owner.Position, 20000, true)
                .Where(x => x.Team == CustomConvert.GetEnemyTeam(owner.Team)))
            {
                AddParticleTarget(owner, "KarthusFallenOne", enemyTarget);
            }
        }

        public void OnFinishCasting(IObjAiBase owner, ISpell spell, IAttackableUnit target)
        {
            var ap = owner.Stats.AbilityPower.Total;
            var damage = 100 + spell.CastInfo.SpellLevel * 150 + ap * 0.6f;
            foreach (var enemyTarget in GetChampionsInRange(owner.Position, 20000, true)
                .Where(x => x.Team == CustomConvert.GetEnemyTeam(owner.Team)))
            {
                enemyTarget.TakeDamage(owner, damage, DamageType.DAMAGE_TYPE_MAGICAL, DamageSource.DAMAGE_SOURCE_SPELL,
                    false);
            }
        }

        public void ApplyEffects(IObjAiBase owner, IAttackableUnit target, ISpell spell, ISpellMissile projectile)
        {
        }

        public void OnUpdate(float diff)
        {
        }
    }
}
