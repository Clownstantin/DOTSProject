using Unity.Entities;
using UnityEngine;

namespace Core
{
	public class UnitAuthoring : MonoBehaviour
	{
		public class Baker : Baker<UnitAuthoring>
		{
			public override void Bake(UnitAuthoring authoring)
			{
				Entity entity = GetEntity(TransformUsageFlags.Dynamic);
				AddComponent(entity, new Unit());
			}
		}
	}

	public struct Unit : IComponentData { }
}
