using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Core
{
	partial struct UnitMoveSystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			UnitMoveJob moveJob = new() { deltaTime = SystemAPI.Time.DeltaTime };
			moveJob.ScheduleParallel();

			/*foreach(var (localTR, unitMover, velocity) in SystemAPI.Query<
			RefRW<LocalTransform>,
			RefRO<UnitMover>,
			RefRW<PhysicsVelocity>>())
			{
				float3 moveDir = math.normalize(unitMover.ValueRO.targetPosition - localTR.ValueRO.Position);
				quaternion targetRot = quaternion.LookRotation(moveDir, math.up());

				localTR.ValueRW.Rotation = math.slerp(localTR.ValueRO.Rotation, targetRot, unitMover.ValueRO.rotationSpeed * SystemAPI.Time.DeltaTime);
				velocity.ValueRW.Linear = moveDir * unitMover.ValueRO.rotationSpeed;
				velocity.ValueRW.Angular = float3.zero;
			}*/
		}
	}

	[BurstCompile]
	public partial struct UnitMoveJob : IJobEntity
	{
		public float deltaTime;

		public void Execute(ref LocalTransform localTR, in UnitMover mover, ref PhysicsVelocity velocity)
		{
			float3 moveDir = math.normalize(mover.targetPosition - localTR.Position);
			quaternion targetRot = quaternion.LookRotation(moveDir, math.up());

			localTR.Rotation = math.slerp(localTR.Rotation, targetRot, mover.rotationSpeed * deltaTime);
			velocity.Linear = moveDir * mover.rotationSpeed;
			velocity.Angular = float3.zero;
		}
	}
}