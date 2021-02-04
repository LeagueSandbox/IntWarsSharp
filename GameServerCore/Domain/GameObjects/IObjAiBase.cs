﻿using GameServerCore.Domain.GameObjects.Spell;
using GameServerCore.Enums;
using LeagueSandbox.GameServer.Content;
using System.Collections.Generic;

namespace GameServerCore.Domain.GameObjects
{
    /// <summary>
    /// Base class for all moving, attackable, and attacking units.
    /// ObjAIBases normally follow these guidelines of functionality: Movement, Crowd Control, Inventory, Targeting, Attacking, and Spells.
    /// </summary>
    public interface IObjAiBase : IAttackableUnit
    {
        /// <summary>
        /// This AI's current auto attack spell.
        /// </summary>
        ISpell AutoAttackSpell { get; }
        /// <summary>
        /// This AI's current auto attack target. Null if no target.
        /// </summary>
        IAttackableUnit AutoAttackTarget { get; set; }
        /// <summary>
        /// Variable containing all data about the AI's current character such as base health, base mana, whether or not they are melee, base movespeed, per level stats, etc.
        /// </summary>
        /// TODO: Move to AttackableUnit as it relates to stats..
        ICharData CharData { get; }
        /// <summary>
        /// Whether or not this AI has finished an auto attack.
        /// </summary>
        bool HasAutoAttacked { get; set; }
        /// <summary>
        /// Whether or not this AI has made their first auto attack against their current target. Refreshes after untargeting or targeting another unit.
        /// </summary>
        bool HasMadeInitialAttack { get; set; }
        /// <summary>
        /// Variable housing all variables and functions related to this AI's Inventory, ex: Items.
        /// </summary>
        IInventoryManager Inventory { get; }
        /// <summary>
        /// Whether or not this AI is currently auto attacking.
        /// </summary>
        bool IsAttacking { get; }
        /// <summary>
        /// Whether or not this AI is currently casting a spell. *NOTE*: Not to be confused with channeling (which isn't implemented yet).
        /// </summary>
        bool IsCastingSpell { get; set; }
        /// <summary>
        /// Spell this unit will cast when in range of its target.
        /// Overrides auto attack spell casting.
        /// </summary>
        ISpell SpellToCast { get; }
        /// <summary>
        /// Whether or not this AI's auto attacks apply damage to their target immediately after their cast time ends.
        /// </summary>
        bool IsMelee { get; set; }
        /// <summary>
        /// Current order this AI is performing.
        /// </summary>
        /// TODO: Rework AI so this enum can be used fully.
        OrderType MoveOrder { get; }
        bool IsNextAutoCrit { get; }
        Dictionary<short, ISpell> Spells { get; }
        /// <summary>
        /// Unit this AI will auto attack when it is in auto attack range.
        /// </summary>
        IAttackableUnit TargetUnit { get; set;  }
        /// <summary>
        /// Unit this AI will dash to (assuming they are performing a targeted dash).
        /// </summary>
        IAttackableUnit DashTarget { get; }

        /// <summary>
        /// Forces this unit to stop moving by flushing its waypoints. Automatically networked.
        /// </summary>
        /// <param name="orderCause">OrderType that caused the unit to stop moving.</param>
        void StopMovement(OrderType orderCause = OrderType.Stop);
        /// <summary>
        /// Function called by this AI's auto attack projectile when it hits its target.
        /// </summary>
        void AutoAttackHit(IAttackableUnit target);
        /// <summary>
        /// Forces this AI unit to perform a dash which follows the specified AttackableUnit.
        /// </summary>
        /// <param name="target">Unit to follow.</param>
        /// <param name="dashSpeed">Constant speed that the unit will have during the dash.</param>
        /// <param name="animation">Internal name of the dash animation.</param>
        /// <param name="leapGravity">How much gravity the unit will experience when above the ground while dashing.</param>
        /// <param name="keepFacingLastDirection">Whether or not the unit should maintain the direction they were facing before dashing.</param>
        /// <param name="followTargetMaxDistance">Maximum distance the unit will follow the Target before stopping the dash or reaching to the Target.</param>
        /// <param name="backDistance">Unknown parameter.</param>
        /// <param name="travelTime">Total time the dash will follow the GameObject before stopping or reaching the Target.</param>
        /// TODO: Implement Dash class which houses these parameters, then have that as the only parameter to this function (and other Dash-based functions).
        void DashToTarget(IAttackableUnit target, float dashSpeed, string animation, float leapGravity, bool keepFacingLastDirection, float followTargetMaxDistance, float backDistance, float travelTime);
        /// <summary>
        /// Whether or not this AI is able to cast spells.
        /// </summary>
        bool CanCast();
        ISpell GetSpell(byte slot);
        ISpell GetSpell(string name);
        void SwapSpells(byte slot1, byte slot2);
        ISpell SetSpell(string name, byte slot, bool enabled = false);
        /// <summary>
        /// Removes the spell instance from the given slot (replaces it with an empty BaseSpell).
        /// </summary>
        /// <param name="slot">Byte slot of the spell to remove.</param>
        void RemoveSpell(byte slot);
        /// <summary>
        /// Sets this AI's current auto attack to their base auto attack.
        /// </summary>
        void ResetAutoAttackSpell();
        ISpell LevelUpSpell(byte slot);
        /// <summary>
        /// Sets this unit's auto attack spell that they will use when in range of their target (unless they are going to cast a spell first).
        /// </summary>
        /// <param name="newAutoAttackSpell">ISpell instance to set.</param>
        /// <param name="isReset">Whether or not setting this spell causes auto attacks to be reset (cooldown).</param>
        /// <returns>ISpell set.</returns>
        ISpell SetAutoAttackSpell(ISpell newAutoAttackSpell, bool isReset);
        /// <summary>
        /// Sets this unit's auto attack spell that they will use when in range of their target (unless they are going to cast a spell first).
        /// </summary>
        /// <param name="name">Internal name of the spell to set.</param>
        /// <param name="isReset">Whether or not setting this spell causes auto attacks to be reset (cooldown).</param>
        /// <returns>ISpell set.</returns>
        ISpell SetAutoAttackSpell(string newAutoAttackSpell, bool isReset);
        /// <summary>
        /// Sets the spell that this unit will cast when it gets in range of its target.
        /// Overrides auto attack spell casting.
        /// </summary>
        /// <param name="s"></param>
        void SetSpellToCast(ISpell s);
        /// <summary>
        /// Sets this AI's current target unit. This relates to both auto attacks as well as general spell targeting.
        /// </summary>
        /// <param name="target">Unit to target.</param>
        /// <param name="networked">Whether or not this change in target should be networked to clients.</param>
        void SetTargetUnit(IAttackableUnit target, bool networked = false);
        /// <summary>
        /// Sets this unit's move order to the given order type.
        /// </summary>
        /// <param name="order">OrderType to set.</param>
        void UpdateMoveOrder(OrderType order);
    }
}
