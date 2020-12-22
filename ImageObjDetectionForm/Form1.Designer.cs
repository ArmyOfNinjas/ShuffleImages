namespace ImageObjDetectionForm
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.picImage = new System.Windows.Forms.PictureBox();
			this.btnBrowse = new System.Windows.Forms.Button();
			this.btnDetect = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.picImage)).BeginInit();
			this.SuspendLayout();
			// 
			// picImage
			// 
			this.picImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.picImage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.picImage.Location = new System.Drawing.Point(0, 0);
			this.picImage.Name = "picImage";
			this.picImage.Size = new System.Drawing.Size(685, 789);
			this.picImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
			this.picImage.TabIndex = 0;
			this.picImage.TabStop = false;
			// 
			// btnBrowse
			// 
			this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnBrowse.Location = new System.Drawing.Point(150, 723);
			this.btnBrowse.Name = "btnBrowse";
			this.btnBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnBrowse.TabIndex = 1;
			this.btnBrowse.Text = "Browse";
			this.btnBrowse.UseVisualStyleBackColor = true;
			this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
			// 
			// btnDetect
			// 
			this.btnDetect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnDetect.Location = new System.Drawing.Point(271, 723);
			this.btnDetect.Name = "btnDetect";
			this.btnDetect.Size = new System.Drawing.Size(75, 23);
			this.btnDetect.TabIndex = 2;
			this.btnDetect.Text = "Detect";
			this.btnDetect.UseVisualStyleBackColor = true;
			this.btnDetect.Click += new System.EventHandler(this.btnDetect_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(685, 789);
			this.Controls.Add(this.btnDetect);
			this.Controls.Add(this.btnBrowse);
			this.Controls.Add(this.picImage);
			this.Name = "Form1";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(this.picImage)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox picImage;
		private System.Windows.Forms.Button btnBrowse;
		private System.Windows.Forms.Button btnDetect;
	}
}

