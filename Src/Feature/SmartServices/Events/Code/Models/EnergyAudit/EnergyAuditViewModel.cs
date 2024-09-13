using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace DEWAXP.Feature.Events.Models.EnergyAudit
{
	public class EnergyAuditViewModel
	{
		public EnergyAuditViewModel()
		{
			Buildings = new List<Building>();
		}

		public List<Building> Buildings { get; set; }

		public Building InitialBuilding
		{
			get
			{
				if (!Buildings.Any())
				{
					Buildings.Add(new Building());
				}
				return Buildings.First();
			}
		}
	}
}