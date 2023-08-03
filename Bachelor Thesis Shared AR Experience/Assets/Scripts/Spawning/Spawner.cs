using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using BachelorThesis;
using TMPro;
using UnityEngine.Serialization;

namespace Testing
{
	public class Spawner : MonoBehaviour, INetworkRunnerCallbacks
	{
		[SerializeField] private LatencyReductionController _latencyReductionController;
		[SerializeField] private int _cubiodsAmount;
		[SerializeField] private NetworkPrefabRef _vrUser;
		[SerializeField] private NetworkPrefabRef _androidUser;
		[SerializeField] private Transform _androidUserPos;
		[SerializeField] private Transform _vrUserPos;
		[SerializeField] public Transform SpawningPos;
		[SerializeField] private NetworkPrefabRef _cuboidPrefab;
		[SerializeField] private NetworkPrefabRef _cuboidWithoutPredictionPrefab;// Character to spawn for a joining player
		public Action<UserComponents> OnVrUserSpawned;

		private Dictionary<PlayerRef, NetworkObject> _spawnedPlayers = 
			new Dictionary<PlayerRef, NetworkObject>();
		private List<NetworkObject> _spawnedCuboids = 
			new List<NetworkObject>();
		private NetworkRunner _runner;
		private NetworkPrefabRef _objectPrefab;
		private KeyValuePair<Vector3, Quaternion> _serverTablePosRotation; 

		[SerializeField] private GestureDetector _gestureDetector;

		[SerializeField] private GameObject _table;
		private bool _onHandGesture = false;

		private void Awake()
		{
			_objectPrefab = _cuboidWithoutPredictionPrefab;
			_latencyReductionController.OnPredictionSwitchedOn += delegate { _objectPrefab = _cuboidPrefab; };
			SpawningPos = _table.transform.GetChild(0).transform;
		}

		private void Start()
		{
			StartGame(GameMode.Host);
			//_gestureDetector.OnHandGesture += PlaceTable;
		}

		private void SetBuildSettings()
		{
			/*
				XRGeneralSettingsPerBuildTarget buildTargetSettings = null;
				EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out buildTargetSettings);
				XRGeneralSettings settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.Android);
				//settings.InitManagerOnStart = true;
				XRPackageMetadataStore
					.RemoveLoader(settings.Manager, "Unity.XR.Oculus.OculusLoader", BuildTargetGroup.Android);
				//XRGeneralSettings.Instance.Manager.InitializeLoader();
				//XRGeneralSettings.Instance.Manager.DeinitializeLoader();
				
				XRGeneralSettingsPerBuildTarget buildTargetSettings = null;
				EditorBuildSettings.TryGetConfigObject(XRGeneralSettings.k_SettingsKey, out buildTargetSettings);
				XRGeneralSettings settings = buildTargetSettings.SettingsForBuildTarget(BuildTargetGroup.Android);      
				//XRPackageMetadataStore.RemoveLoader(settings.Manager, "Unity.XR.Oculus.OculusLoader", BuildTargetGroup.Android);
				XRPackageMetadataStore.AssignLoader(settings.Manager, "Unity.XR.Oculus.OculusLoader", BuildTargetGroup.Android);
				*/
		}

		async void StartGame(GameMode mode)
		{
			// Create the Fusion runner and let it know that we will be providing user input
			_runner = gameObject.AddComponent<NetworkRunner>();
			_runner.ProvideInput = true;
	    
			// Start or join (depends on gamemode) a session with a specific name
			await _runner.StartGame(new StartGameArgs()
			{
				GameMode = mode, 
				SessionName = "TestRoom", 
				Scene = SceneManager.GetActiveScene().buildIndex,
				SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
			});
		}
		
		public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
		{
			if (runner.IsServer)//is server = vr in current moment
			{
				// VR 
				if (_spawnedPlayers.Count == 0)
				{
					var networkPlayerObject = runner.Spawn(_vrUser, _vrUserPos.position, Quaternion.identity, player);
					_spawnedPlayers.Add(player, networkPlayerObject);
					var userComponents = networkPlayerObject.GetComponent<UserComponents>();
					userComponents.OvrCameraRig.SetActive(true);
					OnVrUserSpawned.Invoke(userComponents);
				}
				else
				{
					var networkPlayerObject =
						runner.Spawn(_androidUser, _androidUserPos.position, new Quaternion(0f,1f,0f,0f), player);
					_spawnedPlayers.Add(player, networkPlayerObject);
					networkPlayerObject.GetComponent<UserComponents>().Camera.enabled = false;
				}

				//SpawnBalls(runner, player, _cubiodsAmount, _objectPrefab);
			}
		}

		public void PlaceTable(Vector3 position, Quaternion rotation)
		{
			_table.transform.position = position;
			_table.transform.rotation = rotation;
		}

		private void SpawnBalls(NetworkRunner runner, 
			PlayerRef playerRef, int amountToSpawn, NetworkPrefabRef objectToSpawn)
		{
			for (int i = 0; i < amountToSpawn; i++)
			{
				if (SpawningPos != null)
				{
					NetworkObject networkCuboid = 
						runner.Spawn(objectToSpawn, SpawningPos.position, Quaternion.identity, playerRef);
					_spawnedCuboids.Add(networkCuboid);
					Debug.Log("Here is: "+_runner.IsServer+", "+SpawningPos.position);
				}
			}
		}

		public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
		{
			if (_spawnedPlayers.TryGetValue(player, out NetworkObject networkObject))
			{
				runner.Despawn(networkObject);
				_spawnedPlayers.Remove(player);
			}/*
			foreach (var cuboid in _spawnedCuboids)
			{
				runner.Despawn(cuboid);
				_spawnedCuboids.Remove(cuboid);
			}*/
		}

		public void OnInput(NetworkRunner runner, NetworkInput input)
		{
			var data = new NetworkInputData();
			if (Input.GetKeyDown(KeyCode.D))
			{
				SpawnBalls(runner, PlayerRef.None, _cubiodsAmount, _objectPrefab);
			}
			if (Input.GetKeyDown(KeyCode.T))
			{
				var amount = 1;
				SpawnBalls(runner, _spawnedPlayers.FirstOrDefault().Key, amount, _objectPrefab);
				data.SpawnedObjectsAmount = _spawnedCuboids.Count;
				//input.Set(data);
			}
/* // VR user script
			if (_onHandGesture)
			{
				data.ServerTablePos = _table.transform.position;
				data.ServerTableRotation = _table.transform.rotation;
				_onHandGesture = false;
			}*/
			
			input.Set(data);

		}
		
		

		public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
		public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
		public void OnConnectedToServer(NetworkRunner runner) { }
		public void OnDisconnectedFromServer(NetworkRunner runner) { }
		public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
		public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
		public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
		public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
		public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
		public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
		public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data) { }
		public void OnSceneLoadDone(NetworkRunner runner) { }
		public void OnSceneLoadStart(NetworkRunner runner) { }
	}
}
