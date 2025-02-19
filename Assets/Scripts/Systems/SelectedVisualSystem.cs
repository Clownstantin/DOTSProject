using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Core
{
	[UpdateInGroup(typeof(LateSimulationSystemGroup)), UpdateBefore(typeof(ResetEventsSystem))]
	partial struct SelectedVisualSystem : ISystem
	{
		[BurstCompile]
		public void OnUpdate(ref SystemState state)
		{
			foreach(var selected in SystemAPI.Query<RefRO<Selected>>().WithPresent<Selected>())
				if(selected.ValueRO.onSelected)
				{
					var localTR = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
					localTR.ValueRW.Scale = selected.ValueRO.showScale;
				}
				else if(selected.ValueRO.onDeselected)
				{
					var localTR = SystemAPI.GetComponentRW<LocalTransform>(selected.ValueRO.visualEntity);
					localTR.ValueRW.Scale = 0;
				}
		}
	}
}
