﻿using AAEmu.Game.Core.Managers.UnitManagers;
using AAEmu.Game.Models.Game.DoodadObj;
using AAEmu.Game.Models.Game.Skills;
using AAEmu.Game.Models.Game.Units;

namespace AAEmu.Game.Models.Game.World.Interactions
{
    public class Use : IWorldInteraction
    {

        //This Use is triggered by interacting with a doodad that does not have a relational FuncKey to a funcTemplate 
        public void Execute(Unit caster, SkillCaster casterCaster, BaseUnit target, SkillCastTarget targetCaster, uint skillId)
        {
            if (target is Doodad doodad)
            {
                var func = DoodadManager.Instance.GetFunc(doodad.FuncGroupId, skillId);
                if (func == null)
                    return;
                func.Use(caster, doodad, skillId);
            }
        }
    }
}
