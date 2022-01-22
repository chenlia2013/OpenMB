﻿using Mogre;
using OpenMB.Forms;
using OpenMB.Forms.Controller;
using OpenMB.Mods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OpenMB.Core
{
	public class Game
	{
		private Argument gameArgument;
		public Game(string[] args)
		{
			gameArgument = new Argument(args);
			if (args != null)
			{
				foreach (string arg in args)
				{
					string newArg = arg.Trim().Replace(" ", null);//Remove Space
					string[] tokens = newArg.Split('=');
					if (tokens.Length == 2)
					{
						gameArgument.AddArg(tokens[0], tokens[1]);
					}
				}
			}
		}

		public void Run()
		{
			var root = new Root();

			string modArg = gameArgument.GetArgValue("Engine.Mod");
			var mods = ModManager.Instance.InstalledMods.Where(o => o.Value.MetaData.DisplayInChooser).ToList();
			
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			if (string.IsNullOrEmpty(modArg))
			{
				if (mods.Count == 0)
				{
					MessageBox.Show("No module found, app will exit now!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				else
				{
					modArg = mods.First().Key;
				}
			}
			frmConfigureController controller = new frmConfigureController(new frmConfigure(modArg));
			controller.form.ShowDialog();
		}
	}
}
