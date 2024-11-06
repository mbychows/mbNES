namespace mbNES
{
    partial class DebugWindow
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
            this.memoryContentsRichTextBox = new System.Windows.Forms.RichTextBox();
            this.displayMemoryButton = new System.Windows.Forms.Button();
            this.logRichTextBox = new System.Windows.Forms.RichTextBox();
            this.refreshLogButton = new System.Windows.Forms.Button();
            this.registersTextBox = new System.Windows.Forms.RichTextBox();
            this.refreshRegistersButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // memoryContentsRichTextBox
            // 
            this.memoryContentsRichTextBox.Font = new System.Drawing.Font("Courier New", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.memoryContentsRichTextBox.Location = new System.Drawing.Point(12, 251);
            this.memoryContentsRichTextBox.Name = "memoryContentsRichTextBox";
            this.memoryContentsRichTextBox.Size = new System.Drawing.Size(776, 309);
            this.memoryContentsRichTextBox.TabIndex = 1;
            this.memoryContentsRichTextBox.Text = "";
            // 
            // displayMemoryButton
            // 
            this.displayMemoryButton.Location = new System.Drawing.Point(12, 566);
            this.displayMemoryButton.Name = "displayMemoryButton";
            this.displayMemoryButton.Size = new System.Drawing.Size(120, 25);
            this.displayMemoryButton.TabIndex = 2;
            this.displayMemoryButton.Text = "Display Memory";
            this.displayMemoryButton.UseVisualStyleBackColor = true;
            this.displayMemoryButton.Click += new System.EventHandler(this.displayMemoryButton_Click);
            // 
            // logRichTextBox
            // 
            this.logRichTextBox.Font = new System.Drawing.Font("Courier New", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.logRichTextBox.Location = new System.Drawing.Point(12, 13);
            this.logRichTextBox.Name = "logRichTextBox";
            this.logRichTextBox.Size = new System.Drawing.Size(776, 172);
            this.logRichTextBox.TabIndex = 3;
            this.logRichTextBox.Text = "";
            // 
            // refreshLogButton
            // 
            this.refreshLogButton.Location = new System.Drawing.Point(138, 566);
            this.refreshLogButton.Name = "refreshLogButton";
            this.refreshLogButton.Size = new System.Drawing.Size(120, 25);
            this.refreshLogButton.TabIndex = 4;
            this.refreshLogButton.Text = "Refresh Log";
            this.refreshLogButton.UseVisualStyleBackColor = true;
            this.refreshLogButton.Click += new System.EventHandler(this.refreshLogButton_Click);
            // 
            // registersTextBox
            // 
            this.registersTextBox.AutoWordSelection = true;
            this.registersTextBox.Font = new System.Drawing.Font("Courier New", 10.18868F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.registersTextBox.Location = new System.Drawing.Point(12, 197);
            this.registersTextBox.Name = "registersTextBox";
            this.registersTextBox.Size = new System.Drawing.Size(776, 38);
            this.registersTextBox.TabIndex = 5;
            this.registersTextBox.Text = "";
            // 
            // refreshRegistersButton
            // 
            this.refreshRegistersButton.Location = new System.Drawing.Point(265, 567);
            this.refreshRegistersButton.Name = "refreshRegistersButton";
            this.refreshRegistersButton.Size = new System.Drawing.Size(120, 25);
            this.refreshRegistersButton.TabIndex = 6;
            this.refreshRegistersButton.Text = "Refresh Registers";
            this.refreshRegistersButton.UseVisualStyleBackColor = true;
            this.refreshRegistersButton.Click += new System.EventHandler(this.refreshRegistersButton_Click);
            // 
            // DebugWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 601);
            this.Controls.Add(this.refreshRegistersButton);
            this.Controls.Add(this.registersTextBox);
            this.Controls.Add(this.refreshLogButton);
            this.Controls.Add(this.logRichTextBox);
            this.Controls.Add(this.displayMemoryButton);
            this.Controls.Add(this.memoryContentsRichTextBox);
            this.Name = "DebugWindow";
            this.Text = "Debug";
            this.Load += new System.EventHandler(this.DebugWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox memoryContentsRichTextBox;
        private System.Windows.Forms.Button displayMemoryButton;
        private System.Windows.Forms.RichTextBox logRichTextBox;
        private System.Windows.Forms.Button refreshLogButton;
        private System.Windows.Forms.RichTextBox registersTextBox;
        private System.Windows.Forms.Button refreshRegistersButton;
    }
}