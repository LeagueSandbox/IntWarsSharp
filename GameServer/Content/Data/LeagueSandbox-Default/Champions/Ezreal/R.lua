Vector2 = require 'Vector2' -- include 2d vector lib 

function onFinishCasting()
    local current = Vector2:new(getOwnerX(), getOwnerY())
    local to = (Vector2:new(getSpellToX(), getSpellToY()) - current):normalize()
    local range = to * 20000
    local trueCoords = current + range

    addProjectile("EzrealTrueshotBarrage", trueCoords.x, trueCoords.y)	
end

function applyEffects()
    local reduc = math.min(getNumberObjectsHit(), 7)
    local bonusAD = getOwner():GetStats().AttackDamage.Total - getOwner():GetStats().AttackDamage.BaseValue
    local AP = getOwner():GetStats().AbilityPower.Total*0.9
    local damage = 200+getSpellLevel()*150 + bonusAD + AP
	dealMagicalDamage(damage * (1 - reduc/10))
    -- TODO this can be fetched from projectile inibin "HitEffectName"
    addParticleTarget(getOwner(), "Ezreal_TrueShot_tar.troy", getTarget())
end
