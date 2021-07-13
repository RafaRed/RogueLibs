﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RogueLibsCore
{
	internal sealed partial class RogueLibsPlugin
	{
		public void PatchAgents()
		{
			// GetLastFiredBullet() extension
			Patcher.Postfix(typeof(Gun), nameof(Gun.spawnBullet),
				new Type[5] { typeof(bulletStatus), typeof(InvItem), typeof(int), typeof(bool), typeof(string) });





			Patcher.AnyErrors();
		}

		public static void Gun_spawnBullet(Gun __instance, Bullet __result)
		{
			LastFiredBulletHook hook = __instance.agent.GetHook<LastFiredBulletHook>() ?? __instance.agent.AddHook<LastFiredBulletHook>();
			hook.LastFiredBullet = __result;
		}
	}
	internal class LastFiredBulletHook : HookBase<Agent>
	{
		public Bullet LastFiredBullet { get; set; }
		protected override void Initialize() { }
	}
}
