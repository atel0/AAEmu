﻿using System;
using System.Collections.Generic;
using System.Linq;
using AAEmu.Game.Core.Managers;
using AAEmu.Game.Models.Game.AI.V2.Params;
using AAEmu.Game.Models.Game.Skills;

namespace AAEmu.Game.Models.Game.AI.v2.Behaviors
{
    public class AlmightyAttackBehavior : BaseCombatBehavior
    {
        private AlmightyNpcAiParams _aiParams;
        private Queue<AiSkill> _skillQueue;

        public override void Enter()
        {
            Ai.Owner.InterruptSkills();
            _aiParams = Ai.Owner.Template.AiParams as AlmightyNpcAiParams;
            _skillQueue = new Queue<AiSkill>();
        }

        public override void Tick(TimeSpan delta)
        {
            if (_aiParams == null)
                return;

            var target = Ai.Owner.CurrentTarget;
            if (target == null)
                return; // Technically, the aggro code should take us out of this state very soon.

            if (CanStrafe && !IsUsingSkill)
                MoveInRange(target, Ai.Owner.Template.AttackStartRangeScale * 4, 5.4f * (delta.Milliseconds / 1000.0f));

            if (!CanUseSkill)
                return;

            _strafeDuringDelay = false;
            #region Pick a skill
            
            if(_skillQueue.Count == 0)
            {
                if (!RefreshSkillQueue())
                    return;
            }

            var selectedSkill = _skillQueue.Dequeue();
            if (selectedSkill == null)
                return;
            var skillTemplate = SkillManager.Instance.GetSkillTemplate(selectedSkill.SkillId);
            if (skillTemplate != null)
            {
                var targetDist = Ai.Owner.GetDistanceTo(Ai.Owner.CurrentTarget);
                if (targetDist >= skillTemplate.MinRange && targetDist <= skillTemplate.MaxRange)
                {
                    Ai.Owner.StopMovement();
                    UseSkill(new Skill(skillTemplate), target, selectedSkill.Delay);
                    _strafeDuringDelay = selectedSkill.Strafe;
                }
            }
            // If skill list is empty, get Base skill
            #endregion
        }

        public override void Exit()
        {
        }

        private bool RefreshSkillQueue()
        {
            var availableSkills = RequestAvailableSkillList();

            if(availableSkills.Count > 0)
            {
                var selectedSkillList = availableSkills.RandomElementByWeight(s => s.Dice);

                foreach(var skill in selectedSkillList.Skills)
                {
                    _skillQueue.Enqueue(skill);
                }

                return _skillQueue.Count > 0;
            }
            else
            {
                if(Ai.Owner.Template.BaseSkillId != 0)
                {
                    _skillQueue.Enqueue(new AiSkill
                    {
                        SkillId = (uint)Ai.Owner.Template.BaseSkillId,
                        Strafe = Ai.Owner.Template.BaseSkillStrafe,
                        Delay = Ai.Owner.Template.BaseSkillDelay
                    });
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private List<AiSkillList> RequestAvailableSkillList()
        {
            int healthRatio = (int)(((float)Ai.Owner.Hp / Ai.Owner.MaxHp) * 100);

            var baseList = _aiParams.AiSkillLists.AsEnumerable();

            baseList = baseList.Where(s => s.HealthRangeMin <= healthRatio && healthRatio <= s.HealthRangeMax);
            baseList = baseList.Where(s => s.Skills.All(skill => !Ai.Owner.Cooldowns.CheckCooldown(skill.SkillId)));

            return baseList.ToList();
        }
    }
}
