﻿using System.Collections.Generic;
using System.Numerics;

namespace GameServerCore.Domain.GameObjects.Spell.Missile
{
    public interface ISpellCircleMissile : IObjMissile // TODO: Change to ISpellMissile for spells rework.
    {
        /// <summary>
        /// Number of objects this projectile has hit since it was created.
        /// </summary>
        List<IGameObject> ObjectsHit { get; }
        /// <summary>
        /// Position this projectile is moving towards. Projectile is destroyed once it reaches this destination. Equals Vector2.Zero if TargetUnit is not null.
        /// </summary>
        Vector2 Destination { get; }

        /// <summary>
        /// Whether or not this projectile has a destination; if it is a valid projectile.
        /// </summary>
        /// <returns>True/False.</returns>
        bool HasDestination();
    }
}