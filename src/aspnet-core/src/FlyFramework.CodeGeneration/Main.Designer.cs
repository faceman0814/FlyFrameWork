using System.Windows.Forms;

namespace FlyFramework
{
    partial class Main
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
            entityTable = new DataGridView();
            nameComboBoxColumn = new DataGridViewTextBoxColumn();
            typeComboBoxColumn = new DataGridViewComboBoxColumn();
            comboBox = new ComboBox();
            lengthTextBoxColumn = new DataGridViewTextBoxColumn();
            descriptTextBoxColumn = new DataGridViewTextBoxColumn();
            label1 = new Label();
            entityName = new TextBox();
            button1 = new Button();
            label2 = new Label();
            label3 = new Label();
            projectPath = new TextBox();
            label4 = new Label();
            entityDescription = new TextBox();
            ((System.ComponentModel.ISupportInitialize)entityTable).BeginInit();
            SuspendLayout();
            // 
            // entityTable
            // 
            entityTable.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            entityTable.Columns.AddRange(new DataGridViewColumn[] { nameComboBoxColumn, typeComboBoxColumn, lengthTextBoxColumn, descriptTextBoxColumn });
            entityTable.Location = new Point(0, 0);
            entityTable.Name = "entityTable";
            entityTable.RowHeadersWidth = 51;
            entityTable.Size = new Size(954, 284);
            entityTable.TabIndex = 0;
            // 
            // nameComboBoxColumn
            // 
            nameComboBoxColumn.HeaderText = "字段名称";
            nameComboBoxColumn.MinimumWidth = 6;
            nameComboBoxColumn.Name = "nameComboBoxColumn";
            nameComboBoxColumn.Width = 225;
            // 
            // typeComboBoxColumn
            // 
            typeComboBoxColumn.DataSource = comboBox.Items;
            typeComboBoxColumn.HeaderText = "字段类型";
            typeComboBoxColumn.MinimumWidth = 6;
            typeComboBoxColumn.Name = "typeComboBoxColumn";
            typeComboBoxColumn.Width = 225;
            // 
            // comboBox
            // 
            comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Items.AddRange(new object[] { "int", "string", "bool", "char", "DateTime", "Decimal", "float" });
            comboBox.Location = new Point(0, 0);
            comboBox.Name = "comboBox";
            comboBox.Size = new Size(121, 28);
            comboBox.TabIndex = 0;
            // 
            // lengthTextBoxColumn
            // 
            lengthTextBoxColumn.HeaderText = "字段长度";
            lengthTextBoxColumn.MinimumWidth = 6;
            lengthTextBoxColumn.Name = "lengthTextBoxColumn";
            lengthTextBoxColumn.Width = 225;
            // 
            // descriptTextBoxColumn
            // 
            descriptTextBoxColumn.HeaderText = "字段描述";
            descriptTextBoxColumn.MinimumWidth = 6;
            descriptTextBoxColumn.Name = "descriptTextBoxColumn";
            descriptTextBoxColumn.Width = 225;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = SystemColors.ControlText;
            label1.Location = new Point(23, 318);
            label1.Name = "label1";
            label1.Size = new Size(84, 20);
            label1.TabIndex = 1;
            label1.Text = "实体名称：";
            // 
            // entityName
            // 
            entityName.Location = new Point(102, 315);
            entityName.Name = "entityName";
            entityName.Size = new Size(192, 27);
            entityName.TabIndex = 2;
            // 
            // button1
            // 
            button1.Location = new Point(23, 434);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 3;
            button1.Text = "生成代码";
            button1.UseVisualStyleBackColor = true;
            button1.Click += CodeGeneration;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(28, 375);
            label2.Name = "label2";
            label2.Size = new Size(0, 20);
            label2.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(23, 395);
            label3.Name = "label3";
            label3.Size = new Size(114, 20);
            label3.TabIndex = 5;
            label3.Text = "目标生成地址：\r\n";
            // 
            // projectPath
            // 
            projectPath.Location = new Point(133, 392);
            projectPath.Name = "projectPath";
            projectPath.Size = new Size(604, 27);
            projectPath.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = SystemColors.ControlText;
            label4.Location = new Point(23, 355);
            label4.Name = "label4";
            label4.Size = new Size(84, 20);
            label4.TabIndex = 7;
            label4.Text = "实体描述：";
            // 
            // entityDescription
            // 
            entityDescription.Location = new Point(102, 352);
            entityDescription.Name = "entityDescription";
            entityDescription.Size = new Size(192, 27);
            entityDescription.TabIndex = 8;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(956, 589);
            Controls.Add(entityDescription);
            Controls.Add(label4);
            Controls.Add(projectPath);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(button1);
            Controls.Add(entityName);
            Controls.Add(label1);
            Controls.Add(entityTable);
            Name = "Main";
            Text = "FlyFrameWork框架代码生成器";
            Load += Main_Load;
            ((System.ComponentModel.ISupportInitialize)entityTable).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private DataGridView entityTable;
        private ComboBox comboBox;
        private DataGridViewTextBoxColumn nameComboBoxColumn;
        private DataGridViewComboBoxColumn typeComboBoxColumn;
        private DataGridViewTextBoxColumn lengthTextBoxColumn;
        private DataGridViewTextBoxColumn descriptTextBoxColumn;
        private Label label1;
        private TextBox entityName;
        private Button button1;
        private Label label2;
        private Label label3;
        private TextBox projectPath;
        private Label label4;
        private TextBox entityDescription;
    }
}