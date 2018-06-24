using System;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Eto.Serialization.Xaml;

namespace PlaylistGenerator.UI
{
    public class MainForm : Form
	{	
		public MainForm()
		{
            XamlReader.Load(this);
            DataContext = new MainFormViewModel();
            var l = new Label();
        }

		protected void HandleAbout(object sender, EventArgs e)
		{
			new AboutDialog().ShowDialog(this);
		}

		protected void HandleQuit(object sender, EventArgs e)
		{
			Application.Instance.Quit();
		}
	}
}
