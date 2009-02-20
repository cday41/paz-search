using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PAZ_Dispersal
{
	/// <summary>
	/// Summary description for frmModifierInput.
	/// </summary>
	public class frmModifierInput : System.Windows.Forms.Form
	{
      private Modifier mTempMod;
      private int mHour;
      private DateTime mDate;
      private bool mValue;
      private System.Windows.Forms.PropertyGrid myGrid;
      private System.Windows.Forms.Button btnCancel;
      private System.Windows.Forms.Button btnOK;
      private System.Windows.Forms.DateTimePicker dtPicker;
      private System.Windows.Forms.Label lblCaption;
     

      public bool Value
		{
			get { return mValue; }
			set { mValue = value; }
		}

      public int Hour
		{
			get { return mHour; }
			set { mHour = value; }
		}

      public DateTime Date
		{
			get { return mDate; }
			set { mDate = value; }
		}

      public Modifier TempMod
		{
			get { return mTempMod; }
			set { mTempMod = value; 
               myGrid.SelectedObject = mTempMod;}
		}

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public frmModifierInput()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
         this.mValue = false;
			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(frmModifierInput));
			this.myGrid = new System.Windows.Forms.PropertyGrid();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.dtPicker = new System.Windows.Forms.DateTimePicker();
			this.lblCaption = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// myGrid
			// 
			this.myGrid.CommandsVisibleIfAvailable = true;
			this.myGrid.LargeButtons = false;
			this.myGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.myGrid.Location = new System.Drawing.Point(0, 0);
			this.myGrid.Name = "myGrid";
			this.myGrid.Size = new System.Drawing.Size(296, 240);
			this.myGrid.TabIndex = 0;
			this.myGrid.Text = "propertyGrid1";
			this.myGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.myGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(168, 288);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(96, 24);
			this.btnCancel.TabIndex = 1;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnOK
			// 
			this.btnOK.Location = new System.Drawing.Point(16, 288);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(96, 24);
			this.btnOK.TabIndex = 2;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// dtPicker
			// 
			this.dtPicker.Location = new System.Drawing.Point(24, 256);
			this.dtPicker.Name = "dtPicker";
			this.dtPicker.Size = new System.Drawing.Size(208, 20);
			this.dtPicker.TabIndex = 3;
			this.dtPicker.Visible = false;
			// 
			// lblCaption
			// 
			this.lblCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblCaption.Location = new System.Drawing.Point(16, 248);
			this.lblCaption.Name = "lblCaption";
			this.lblCaption.Size = new System.Drawing.Size(256, 32);
			this.lblCaption.TabIndex = 4;
			this.lblCaption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblCaption.Visible = false;
			// 
			// frmModifierInput
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 325);
			this.Controls.Add(this.lblCaption);
			this.Controls.Add(this.dtPicker);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.myGrid);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmModifierInput";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Input";
			this.Load += new System.EventHandler(this.frmModifierInput_Load);
			this.ResumeLayout(false);

		}
		#endregion

      private void frmModifierInput_Load(object sender, System.EventArgs e)
      {
         
      }

      public void setHour()
      {
         this.dtPicker.Visible = true;
         this.dtPicker.ShowUpDown = true;
         this.dtPicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
         this.Text = "Set the Hourly Modifiers";
      }
      public void setGender(string genderType)
      {
         if (genderType == "Male")
         {
            this.Text = "Modifiers for the male of the species";
            this.lblCaption.Text = "Male modifers";
            
         }
         else
         {
            this.Text = "Modifiers for the female of the species";
            this.lblCaption.Text = "Female modifers";
         }
            this.lblCaption.Visible = true;
      }

      public void setText(string inText)
      {
         this.Text = "Modifiers for " + inText;
         this.lblCaption.Text = inText;
         this.lblCaption.Visible = true;

      }

      
     
      
      private void btnCancel_Click(object sender, System.EventArgs e)
      {
         mTempMod = null;
         this.Close();
        
      }

      private void btnOK_Click(object sender, System.EventArgs e)
      {
         if(this.dtPicker.Format == DateTimePickerFormat.Time)
         {
            this.mHour = this.dtPicker.Value.Hour;
         }
         else
         {
            this.mDate = this.dtPicker.Value.Date;
         }
            this.mValue = true;
            this.Visible = false;

        
      }

      
   }
}
