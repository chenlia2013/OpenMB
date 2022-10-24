﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace io.mod
{
	public class ModioModFile
	{
		public int id { get; set; }
		public int mod_id { get; set; }
		public int date_added { get; set; }
		public int date_scanned { get; set; }
		public int virus_status { get; set; }
		public int virus_positive { get; set; }
		public string virustotal_hash { get; set; }
		public int filesize { get; set; }
		public ModioModFileHash file_hash { get; set; }
		public string filename { get; set; }
		public string version { get; set; }
		public string changelog { get; set; }
		public string metadata_blob { get; set; }
		public ModioModFileDownload download { get; set; }
	}
}
