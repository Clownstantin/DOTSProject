using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Core
{
	public class UnitSelectionManager : MonoBehaviour
	{
		private void Update()
		{
			if(Input.GetMouseButtonDown(1))
			{
				Vector3 mousePos = MouseWorldPosition.Instance.GetPosition();
				EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
				EntityQuery query = new EntityQueryBuilder(Allocator.Temp).WithAll<UnitMover>().Build(entityManager);
				NativeArray<UnitMover> moverArray = query.ToComponentDataArray<UnitMover>(Allocator.Temp);

				for(int i = 0; i < moverArray.Length; i++)
				{
					UnitMover unitMover = moverArray[i];
					unitMover.targetPosition = mousePos;
					moverArray[i] = unitMover;
				}

				query.CopyFromComponentDataArray(moverArray);
			}
		}
	}
}