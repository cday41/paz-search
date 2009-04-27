using System;
using System.Windows.Forms;
using System.Drawing;

namespace inputBox
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
   public class InputBox : System.Windows.Forms.Form
   {
      private System.Windows.Forms.TextBox txtInput;
      private System.Windows.Forms.Label lblMessage;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.Button btnCancel;
      private string _message;
      private string _title;
      private int _width;
      private int _height;
      private System.ComponentModel.Container components = null;

      private InputBox(string sMessage, string sTitle)
      {
         _message = sMessage;
         _title = sTitle;
         InitializeComponent();
      }

      protected override void Dispose( bool disposing )
      {
         if( disposing )
         {
            if(components != null)
            {
               components.Dispose();
            }
         } base.Dispose( disposing );
      }

      private void InitializeComponent()
      {
         this.txtInput = new System.Windows.Forms.TextBox();
         this.lblMessage = new System.Windows.Forms.Label();
         this.btnOK = new System.Windows.Forms.Button();
         this.btnCancel = new System.Windows.Forms.Button();
         this.SuspendLayout();

         //Calculate the width and height of the label based on the size of the message
         if (_message.Length > 100)
         {
            //First determine the number of new line characters (/n) that might be in the string
            int numNewLines = getNumNewLines(_message);
            _width = 600;	//Max width of 600 for really long strings
            //Calculate the height of the label
            _height = Convert.ToInt32(this.lblMessage.Font.Height * ((_message.Length / 100) + numNewLines));
         }
         else
         {
            //Shorter strings will result in a single line only as wide as needed.
            _width = 200 + _message.Length * 3;
            _height = this.lblMessage.Font.Height;
         }	
         //
         // label
         //
         this.lblMessage.Location = new System.Drawing.Point(25,25);
         this.lblMessage.Name = "lblMessage";
         this.lblMessage.Width = _width;
         this.lblMessage.Height = _height;
         this.lblMessage.Text = _message;
			
         //
         // buttons
         //
         this.btnOK.Name = "btnOK";
         this.btnOK.Location = new System.Drawing.Point(_width - this.btnOK.Width - this.btnCancel.Width - 50, _height + 100);
         this.btnOK.Text = "OK";
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);

         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Location = new System.Drawing.Point(_width - this.btnCancel.Width - 25, _height + 100);
         this.btnCancel.Text = "Cancel";
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

         // 
         // text box
         // 
         this.txtInput.Location = new System.Drawing.Point(25, _height + 50);
         this.txtInput.Name = "txtInput";
         this.txtInput.Size = new System.Drawing.Size(_width, 20);
         this.txtInput.TabIndex = 0;
         this.txtInput.Text = "";
         this.txtInput.Width = _width;	//width should be same as label width
         // 
         // InputBox
         // 
         Rectangle r1 = Screen.GetWorkingArea(this);
         this.ClientSize = new System.Drawing.Size(_width + 50, _height + 150);
         this.ControlBox = false;
         this.Controls.AddRange(new System.Windows.Forms.Control[] {this.txtInput,
                                                                      this.btnOK,
                                                                      this.btnCancel,
                                                                      this.lblMessage});
         this.DesktopLocation = new Point((r1.Width/2)-(this.Width/2), (r1.Height/2)-(this.Height/2));
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Name = "InputBox";
         this.Text = _title;
         this.AcceptButton = this.btnOK;
         this.ResumeLayout(false);
      }

      private int getNumNewLines(string s1)
      {
         string pattern = "\n";
         System.Text.RegularExpressions.MatchCollection matches = System.Text.RegularExpressions.Regex.Matches(s1, pattern);
         return matches.Count;
      }

      private void btnOK_Click(object sender, System.EventArgs e)
      {
         this.Close();
      }
      private void btnCancel_Click(object sender, System.EventArgs e)
      {
         this.txtInput.Text = "-1";
         this.Close();
      }
      public static string ShowInputBox(string sMessage, string sTitle)
      {
         InputBox box = new InputBox(sMessage, sTitle);
         box.ShowDialog();
         return box.txtInput.Text;
      }	
   }
}
