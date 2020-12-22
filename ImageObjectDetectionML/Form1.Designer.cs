namespace ImageObjectDetectionML
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
			this.btnSelectImage = new System.Windows.Forms.Button();
			this.FileWatcher = new System.IO.FileSystemWatcher();
			this.FileDialog = new System.Windows.Forms.OpenFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.FileWatcher)).BeginInit();
			this.SuspendLayout();
			// 
			// btnSelectImage
			// 
			this.btnSelectImage.Location = new System.Drawing.Point(275, 274);
			this.btnSelectImage.Name = "btnSelectImage";
			this.btnSelectImage.Size = new System.Drawing.Size(94, 23);
			this.btnSelectImage.TabIndex = 0;
			this.btnSelectImage.Text = "Select Image";
			this.btnSelectImage.UseVisualStyleBackColor = true;
			this.btnSelectImage.Click += new System.EventHandler(this.btnSelectImage_Click);
			// 
			// FileWatcher
			// 
			this.FileWatcher.EnableRaisingEvents = true;
			this.FileWatcher.SynchronizingObject = this;
			// 
			// FileDialog
			// 
			this.FileDialog.FileName = "FileName";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(656, 454);
			this.Controls.Add(this.btnSelectImage);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.FileWatcher)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnSelectImage;
		private System.IO.FileSystemWatcher FileWatcher;
		private System.Windows.Forms.OpenFileDialog FileDialog;
	}
}

