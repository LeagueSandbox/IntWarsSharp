using GameServerCore.Enums;
using GameServerCore.Domain.GameObjects;
using static LeagueSandbox.GameServer.API.ApiFunctionManager;
using LeagueSandbox.GameServer.GameObjects.AttackableUnits.AI;
using GameServerCore.Domain;
using LeagueSandbox.GameServer.Scripting.CSharp;

namespace Spells
{
    public class YasuoDashWrapper : IGameScript
    {
        public static IAttackableUnit _target;
        public static IChampion _owner;
        public void OnActivate(IChampion owner)
        {
        }

        public void OnDeactivate(IChampion owner)
        {
        }

        public void OnStartCasting(IChampion owner, ISpell spell, IAttackableUnit target)
        {
            
        }

        public void OnFinishCasting(IChampion owner, ISpell spell, IAttackableUnit target)
        {
            _target = target;
            _owner = owner;
            var _hasbuff = ((ObjAiBase)target).HasBuffGameScriptActive("YasuoEBlock", "YasuoEBlock");

            if (!_hasbuff)
            {
                AddBuffGameScript("YasuoE", 1, spell, BuffType.COMBAT_ENCHANCER, owner, 0.395f - spell.Level *0.012f, true);            
                AddBuffGameScript("YasuoEBlock", 1, spell, BuffType.COMBAT_DEHANCER, (ObjAiBase)target, 11f - spell.Level * 1f, true);
            }
        }

        public void ApplyEffects(IChampion owner, IAttackableUnit target, ISpell spell, IProjectile projectile)
        {

        }
               
        public void OnUpdate(double diff)
        {
        }
    }
}
