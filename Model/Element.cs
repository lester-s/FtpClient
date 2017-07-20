using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FtpClient.Model
{
	public class Element
	{
		public string Name { get; set; }
		public int Progress { get; set; }
		public ElementTypes ElementType { get; set; }
	}

	public enum ElementTypes
	{
		File,
		Folder
	}
}