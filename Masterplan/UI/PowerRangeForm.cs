﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Policy;
using System.Windows.Forms;

using Masterplan.Data;
using Masterplan.Tools.Generators;

namespace Masterplan.UI
{
	partial class PowerRangeForm : Form
	{
		public PowerRangeForm(CreaturePower power)
		{
			InitializeComponent();

			RangeBox.Items.Add("Melee");
			RangeBox.Items.Add("Melee Touch");
			RangeBox.Items.Add("Melee Weapon");
			RangeBox.Items.Add("Melee N");
			RangeBox.Items.Add("Reach N");
			RangeBox.Items.Add("Ranged N");
			RangeBox.Items.Add("Close Blast N");
			RangeBox.Items.Add("Close Burst N");
			RangeBox.Items.Add("Area Burst N within N");
			RangeBox.Items.Add("Personal");

			TargetBox.Items.Add("(one creature)");
            TargetBox.Items.Add("(creatures in blast)");
			TargetBox.Items.Add("(creatures in burst)");
			TargetBox.Items.Add("(enemies in blast)");
			TargetBox.Items.Add("(enemies in burst)");

			if (power.Range != "" || power.Range is null) { CurrentRangeLbl.Text = power.Range; }
			else { CurrentRangeLbl.Text = "<Current range is not set>"; }
			
			RangeBox.Text = power.Range;
        }
        public string PowerRange
        {
            get { 
				if (RangeBox.Text == "" && TargetBox.Text == "") 
				{
					return RangeBox.Text;
				}
				else { return RangeBox.Text + " " + TargetBox.Text; }
				}
        }
        private void OKBtn_Click(object sender, EventArgs e)
        {
        }
	}
}
