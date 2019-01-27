using System.Drawing;
using System.Windows.Forms;

namespace DES {
	internal class Program {
		public static void Main(string[] args) {
			CipherForm start = new CipherForm();
			start.Text = "DES";
			start.BackColor = Color.LightGray;
			Application.Run(start);
		}
	}
}