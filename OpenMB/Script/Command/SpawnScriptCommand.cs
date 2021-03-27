﻿using OpenMB.Game;
using OpenMB.Mods;
using Mogre;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenMB.Script.Command
{
	public class SpawnScriptCommand : ScriptCommand
	{
		private string[] commandArgs;
		public SpawnScriptCommand()
		{
			commandArgs = new string[] {
				"CharacterType",
				"CharacterID",
				"CharacterTeam",
				"SpawnVector"
			};
		}
		public override string[] CommandArgs
		{
			get
			{
				return commandArgs;
			}
		}

		public override string CommandName
		{
			get
			{
				return "spawn";
			}
		}

		public override ScriptCommandType CommandType
		{
			get
			{
				return ScriptCommandType.Line;
			}
		}

		public override void Execute(params object[] executeArgs)
		{
			string characterType = CommandArgs[0].StartsWith("%") ? Context.GetLocalValue(CommandArgs[0].Substring(1)).ToString() : CommandArgs[0];
			string characterID = CommandArgs[1].StartsWith("%") ? Context.GetLocalValue(CommandArgs[1].Substring(1)).ToString() : CommandArgs[1];
			string characterTeam = CommandArgs[2].StartsWith("%") ? Context.GetLocalValue(CommandArgs[2].Substring(1)).ToString() : CommandArgs[2];
			string vectorName = CommandArgs[3].StartsWith("%") ? Context.GetLocalValue(CommandArgs[3].Substring(1)).ToString() : CommandArgs[3];
			GameWorld world = executeArgs[0] as GameWorld;
			var vector = world.GlobalValueTable.GetRecord(vectorName);
			bool isBot = false;
			if (characterType == "player")
			{
				isBot = false;
			}
			else if (characterType == "bot")
			{
				isBot = true;
			}

			world.CreateCharacter(
				characterID,
				new Vector3(
					float.Parse(vector.NextNodes[0].Value),
					float.Parse(vector.NextNodes[1].Value),
					float.Parse(vector.NextNodes[2].Value)),
				characterTeam,
				isBot);
		}
	}
}
