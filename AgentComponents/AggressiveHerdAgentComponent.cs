using HuntableHerds.Extensions;
using HuntableHerds.Models;
using System;
using TaleWorlds.Core;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace HuntableHerds.AgentComponents {
    public class AggressiveHerdAgentComponent : HerdAgentComponent {
        private float _attackTimer = 0f;
        private float _aggroTimer = 0f;

        public AggressiveHerdAgentComponent(Agent agent) : base(agent) {
        }

        public override void HuntableAITick(float dt) {
            if (_attackTimer > 0f)
                _attackTimer -= dt;
            else if (_attackTimer < 0f)
                _attackTimer = 0f;

            if (Agent.CanSeeOtherAgent(Agent.Main, HerdBuildData.CurrentHerdBuildData.SightRange)) {
                _aggroTimer = 15f;
            }
            else {
                if (_aggroTimer > 0f)
                    _aggroTimer -= dt;
            }

            if (_aggroTimer > 0f) {
                TickIsAggroed(dt);
            }
            else if (_aggroTimer < 0f) {
                _aggroTimer = 0f;
            }
        }

        private void TickIsAggroed(float dt) {
            if (_attackTimer <= 1f)
                SetMoveToPosition(Agent.Main.Position.ToWorldPosition());

            if (_attackTimer == 0f && GetWithinAttackRangeOfPlayer() && Agent.CanSeeOtherAgent(Agent.Main))
                AttackPlayer();
        }

        private void AttackPlayer() {
            _attackTimer = 3f;
            SetMoveToPosition(Agent.Position.ToWorldPosition());

            Agent attacker = Agent;
            Agent victim = Agent.Main.HasMount ? Agent.Main.MountAgent : Agent.Main;
            // lazy block checking
            bool isBlocked = !Agent.Main.HasMount && Input.IsKeyDown(InputKey.RightMouseButton) && victim.CanSeeOtherAgent(attacker);
            int blockedDamageToPlayer = (int)Math.Round(HerdBuildData.CurrentHerdBuildData.DamageToPlayer * (isBlocked ? 0.25f : 1f));

            Blow blow = new Blow(attacker.Index);
            blow.DamageType = DamageTypes.Cut;
            blow.BoneIndex = victim.Monster.HeadLookDirectionBoneIndex;
            blow.Position = victim.Position;
            blow.Position.z = blow.Position.z + victim.GetEyeGlobalHeight();
            blow.BaseMagnitude = blockedDamageToPlayer;
            blow.WeaponRecord.FillAsMeleeBlow(null, null, -1, -1);
            blow.InflictedDamage = blockedDamageToPlayer;
            blow.SwingDirection = victim.LookDirection;
            blow.Direction = blow.SwingDirection;
            blow.DamageCalculated = true;

            sbyte mainHandItemBoneIndex = attacker.Monster.MainHandItemBoneIndex;
            AttackCollisionData attackCollisionDataForDebugPurpose = AttackCollisionData.GetAttackCollisionDataForDebugPurpose(isBlocked, false, false, true, false, false, false, false, false, false, false, false, isBlocked ? CombatCollisionResult.Blocked : CombatCollisionResult.StrikeAgent, -1, 0, 2, blow.BoneIndex, BoneBodyPartType.Head, mainHandItemBoneIndex, Agent.UsageDirection.AttackLeft, -1, CombatHitResultFlags.NormalHit, 0.5f, 1f, 0f, 0f, 0f, 0f, 0f, 0f, Vec3.Up, blow.Direction, blow.Position, Vec3.Zero, Vec3.Zero, victim.Velocity, Vec3.Up);

            victim.RegisterBlow(blow, attackCollisionDataForDebugPurpose);
        }

        private bool GetWithinAttackRangeOfPlayer() {
            if (Agent.Main.Position.Distance(this.Agent.Position) < HerdBuildData.CurrentHerdBuildData.HitboxRange)
                return true;
            return false;
        }
    }
}
