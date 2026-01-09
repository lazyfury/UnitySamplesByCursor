using System;
using System.Linq;
using UnityEngine;
using WarStrategyEngine.Core;
using WarStrategyEngine.Core.Implementations;
using WarStrategyEngine.Core.Interfaces;

namespace WarStrategyEngine.Examples
{
	/// <summary>
	/// 游戏引擎使用示例
	/// </summary>
	public partial class GameEngineExample : MonoBehaviour
	{
		[Header("UI 设置")]
		[Tooltip("用于显示输出的文本组件")]
		public TMPro.TextMeshProUGUI outputText;

		[Header("游戏设置")]
		[Tooltip("地图宽度")]
		public int mapWidth = 20;

		[Tooltip("地图高度")]
		public int mapHeight = 20;

		[Header("运行时信息")]
		[Tooltip("游戏引擎实例")]
		private IGameEngine gameEngine;

		[Tooltip("输出日志内容")]
		private System.Text.StringBuilder logBuilder = new System.Text.StringBuilder();

		[Header("地图渲染")]
		[Tooltip("瓦片渲染器")]
		public TileRenderer tileRenderer;
		
		[Tooltip("单位渲染器")]
		public UnitRenderer unitRenderer;

		[Header("摄像机设置")]
		[Tooltip("要跟随的摄像机（如果为空则使用主摄像机）")]
		public Camera followCamera;
		
		[Tooltip("是否启用摄像机跟随")]
		public bool enableCameraFollow = true;
		
		[Tooltip("摄像机跟随速度（0=立即跟随，越大越慢）")]
		[Range(0f, 10f)]
		public float cameraFollowSpeed = 5f;
		
		[Tooltip("摄像机Z轴偏移（2D摄像机应该保持这个值）")]
		public float cameraZOffset = -10f;


		void Start()
		{
			// 初始化输出
			if (outputText != null)
			{
				outputText.text = "";
			}

			// 初始化摄像机
			if (followCamera == null)
			{
				followCamera = Camera.main;
			}

			// 运行示例
			RunExample();
		}

		/// <summary>
		/// 运行游戏引擎示例
		/// </summary>
		public void RunExample()
		{
			LogOutput("=== 战争策略游戏引擎示例 ===\n");

			// 1. 创建游戏引擎
			gameEngine = new GameEngine();

			// 2. 初始化游戏
			gameEngine.Initialize(mapWidth, mapHeight);
			LogOutput($"游戏引擎已初始化，地图大小: {mapWidth}x{mapHeight}\n");

			// 2.5. 绘制地图瓦片
			DrawTiles();
			
			// 2.6. 绘制单位
			DrawUnits();

			// 3. 获取地图信息
			var map = gameEngine.GetMap();
			LogOutput($"地图宽度: {map.Width}, 高度: {map.Height}\n");

			// 4. 获取所有玩家
			LogOutput("玩家列表:\n");
			foreach (var player in gameEngine.GetPlayers())
			{
				LogOutput($"  - {player.Name} (ID: {player.Id}, 阵营: {player.Faction}, 人类: {player.IsHuman})\n");
				LogOutput($"    资源: 金币={player.Resources.GetResource(ResourceType.Gold)}, " +
								$"食物={player.Resources.GetResource(ResourceType.Food)}\n");
			}

			// 5. 获取所有单位
			LogOutput("\n单位列表:\n");
			foreach (var unit in gameEngine.GetUnits())
			{
				var unitOwner = gameEngine.GetPlayers().FirstOrDefault(p => p.Id == unit.OwnerId);
				LogOutput($"  - {unit.Type} (ID: {unit.Id}, 所有者: {unitOwner?.Name}, " +
								$"位置: {unit.Position}, 生命值: {unit.Stats.CurrentHealth}/{unit.Stats.MaxHealth})\n");
			}

			// 6. 演示单位移动
			var firstUnit = gameEngine.GetUnits().FirstOrDefault();
			if (firstUnit != null)
			{
				var currentPos = firstUnit.Position;
				var newPos = new Position(currentPos.X + 1, currentPos.Y);

				LogOutput($"\n尝试移动单位 {firstUnit.Id} 从 {currentPos} 到 {newPos}\n");
				bool moved = firstUnit.MoveTo(newPos);
				LogOutput($"移动结果: {(moved ? "成功" : "失败")}\n");
				LogOutput($"当前位置: {firstUnit.Position}\n");
			}

			// 7. 演示战斗
			var units = gameEngine.GetUnits().ToList();
			if (units.Count >= 2)
			{
				var attacker = units[0];
				var defender = units[1];

				// 确保它们是不同玩家的单位
				if (attacker.OwnerId == defender.OwnerId)
				{
					defender = units.FirstOrDefault(u => u.OwnerId != attacker.OwnerId);
				}

				if (defender != null && attacker.OwnerId != defender.OwnerId)
				{
					LogOutput("\n战斗演示:\n");
					LogOutput($"  攻击者: {attacker.Type} (生命值: {attacker.Stats.CurrentHealth}, 攻击力: {attacker.Stats.Attack})\n");
					LogOutput($"  防御者: {defender.Type} (生命值: {defender.Stats.CurrentHealth}, 防御力: {defender.Stats.Defense})\n");

					var result = attacker.Attack(defender);
					LogOutput($"  攻击结果: {(result.Success ? "成功" : "失败")}\n");
					LogOutput($"  造成伤害: {result.Damage}\n");
					LogOutput($"  防御者剩余生命值: {defender.Stats.CurrentHealth}\n");
					LogOutput($"  是否击杀: {result.TargetKilled}\n");
					if (result.CounterAttacked)
					{
						LogOutput($"  反击伤害: {result.CounterDamage}\n");
						LogOutput($"  攻击者剩余生命值: {attacker.Stats.CurrentHealth}\n");
					}
				}
			}

			// 8. 演示路径查找
			if (units.Count > 0)
			{
				var unit = units[0];
				var start = unit.Position;
				var goal = new Position(start.X + 5, start.Y + 5);

				LogOutput("\n路径查找演示:\n");
				LogOutput($"  起点: {start}\n");
				LogOutput($"  终点: {goal}\n");

				var path = map.FindPath(start, goal, unit);
				LogOutput($"  找到路径，长度: {path.Count}\n");
				if (path.Count > 0)
				{
					LogOutput($"  路径: {string.Join(" -> ", path.Take(5))}...\n");
				}
			}

			// 9. 演示资源管理
			var currentPlayer = gameEngine.GetPlayers().FirstOrDefault();
			if (currentPlayer != null)
			{
				LogOutput("\n资源管理演示:\n");
				LogOutput($"  当前金币: {currentPlayer.Resources.GetResource(ResourceType.Gold)}\n");

				currentPlayer.Resources.AddResource(ResourceType.Gold, 500);
				LogOutput($"  增加500金币后: {currentPlayer.Resources.GetResource(ResourceType.Gold)}\n");

				bool canAfford = currentPlayer.Resources.ConsumeResource(ResourceType.Gold, 200);
				LogOutput($"  消耗200金币: {(canAfford ? "成功" : "失败")}\n");
				LogOutput($"  剩余金币: {currentPlayer.Resources.GetResource(ResourceType.Gold)}\n");
			}

			// 10. 更新游戏状态
			LogOutput("\n更新游戏状态...\n");
			gameEngine.Update(0.1f);
			LogOutput($"当前游戏状态: {gameEngine.State}\n");

			LogOutput("\n示例运行完成！\n");
		}

		/// <summary>
		/// 绘制地图瓦片
		/// </summary>
		private void DrawTiles()
		{
			if (gameEngine == null)
				return;

			var map = gameEngine.GetMap();
			if (map == null)
				return;

			// 如果已有TileRenderer组件，使用它
			if (tileRenderer == null)
			{
				tileRenderer = GetComponent<TileRenderer>();

				// 如果没有，创建一个
				if (tileRenderer == null)
				{
					tileRenderer = gameObject.AddComponent<TileRenderer>();
				}
			}

			// 设置地图并绘制
			if (tileRenderer != null)
			{
				tileRenderer.SetMap(map);
				LogOutput("地图瓦片已绘制\n");
			}
		}
		
		/// <summary>
		/// 绘制单位
		/// </summary>
		private void DrawUnits()
		{
			if (gameEngine == null)
				return;

			// 如果已有UnitRenderer组件，使用它
			if (unitRenderer == null)
			{
				unitRenderer = GetComponent<UnitRenderer>();

				// 如果没有，创建一个
				if (unitRenderer == null)
				{
					unitRenderer = gameObject.AddComponent<UnitRenderer>();
				}
			}

			// 设置游戏引擎并绘制
			if (unitRenderer != null)
			{
				unitRenderer.SetGameEngine(gameEngine, tileRenderer);
				LogOutput("单位已绘制\n");
			}
		}

		/// <summary>
		/// 输出日志到控制台和UI
		/// </summary>
		private void LogOutput(string message)
		{
			// 输出到控制台
			Debug.Log(message.TrimEnd('\n'));

			// 输出到UI
			if (outputText != null)
			{
				logBuilder.Append(message);
				outputText.text = logBuilder.ToString();
			}
		}

		/// <summary>
		/// 更新游戏引擎（可在Update中调用）
		/// </summary>
		void Update()
		{
			if (gameEngine != null)
			{
				gameEngine.Update(Time.deltaTime);
				
				// 更新单位位置（如果单位移动了）
				if (unitRenderer != null)
				{
					UpdateUnitPositions();
				}
			}
			
			// 更新摄像机跟随
			if (enableCameraFollow)
			{
				UpdateCameraFollow();
			}
		}
		
		/// <summary>
		/// 更新所有单位的位置
		/// </summary>
		private void UpdateUnitPositions()
		{
			if (unitRenderer == null || gameEngine == null)
				return;
			
			foreach (var unit in gameEngine.GetUnits())
			{
				unitRenderer.UpdateUnitPosition(unit);
			}
		}
		
		/// <summary>
		/// 更新摄像机跟随玩家位置
		/// </summary>
		private void UpdateCameraFollow()
		{
			if (followCamera == null || gameEngine == null)
				return;
			
			// 获取当前玩家
			var currentPlayer = gameEngine.GetCurrentPlayer();
			if (currentPlayer == null)
				return;
			
			// 获取当前玩家的第一个存活单位
			var firstUnit = currentPlayer.Units.FirstOrDefault();
			if (firstUnit == null)
				return;
			
			// 计算目标位置（Unity 2D坐标系）
			// 需要将地图坐标转换为Unity世界坐标，与TileRenderer保持一致
			float tileSize = 1f;
			if (tileRenderer != null)
			{
				tileSize = tileRenderer.tileSize;
			}
			
			// 计算单位所在瓦片的中心位置（与TileRenderer中的计算方式一致）
			Vector3 targetPosition = new Vector3(
				firstUnit.Position.X * tileSize + tileSize * 0.5f,
				firstUnit.Position.Y * tileSize + tileSize * 0.5f,
				cameraZOffset
			);
			
			// 平滑跟随或立即跟随
			if (cameraFollowSpeed > 0f)
			{
				// 使用Lerp进行平滑跟随
				followCamera.transform.position = Vector3.Lerp(
					followCamera.transform.position,
					targetPosition,
					Time.deltaTime * cameraFollowSpeed
				);
			}
			else
			{
				// 立即跟随
				followCamera.transform.position = targetPosition;
			}
		}

		/// <summary>
		/// 清理资源
		/// </summary>
		void OnDestroy()
		{
			gameEngine = null;
			logBuilder?.Clear();
		}
	}
}
