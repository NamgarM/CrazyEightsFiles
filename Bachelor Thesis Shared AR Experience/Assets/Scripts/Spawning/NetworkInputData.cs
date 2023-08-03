using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace BachelorThesis
{
	enum TouchingTrigger
	{
		Touched = 0,
	}
	public struct NetworkInputData : INetworkInput
	{
		public NetworkButtons TocuhingTrigger;
		public int SpawnedObjectsAmount;
		public const byte OnTablePlacing = 0x01;
		public Vector3 ServerTablePos;
		public Quaternion ServerTableRotation;
		public Vector3 ServerSpawningPos;
		public Quaternion ServerSpawningRotation;
	}
}
