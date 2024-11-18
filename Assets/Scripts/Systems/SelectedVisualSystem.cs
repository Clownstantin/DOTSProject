using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Core
{
	partial struct SelectedVisualSystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			foreach(var selected in SystemAPI.Query<RefRO<Selected>>())
			{
				var localTR = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
				localTR.ValueRW.Scale = selected.ValueRO.showScale;
			}

			foreach(var selected in SystemAPI.Query<RefRO<Selected>>().WithDisabled<Selected>())
			{
				var localTR = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
				localTR.ValueRW.Scale = 0;
			}
		}
	}
}
