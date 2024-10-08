using EntryControl.POS.Core.Interfaces.Services;
using EntryControl.POS.Domain.Entities;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EntryControl.POS.App
{
    public partial class MainForm : MaterialForm
    {
        private void InitializeComponent()
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.Blue600, Primary.Blue700,
                Primary.Blue200, Accent.LightBlue200,
                TextShade.WHITE);

            tabControl = new MaterialTabControl();
            tabControlPage = new TabPage();
            txtType = new MaterialComboBox();
            txtAmmount = new MaterialTextBox();
            txtDescription = new MaterialTextBox();
            btnAdd = new MaterialButton();
            tabViewPage = new TabPage();
            btnAll = new MaterialButton();
            btnSynchronized = new MaterialButton();
            btnDirty = new MaterialButton();
            listViewEntries = new MaterialListView();
            tabSelector = new MaterialTabSelector();
            statusBar = new StatusStrip();
            statusLabel = new ToolStripStatusLabel();
            connectionStatusLabel = new ToolStripStatusLabel();
            tabControl.SuspendLayout();
            tabControlPage.SuspendLayout();
            tabViewPage.SuspendLayout();
            statusBar.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl
            // 
            tabControl.Controls.Add(tabControlPage);
            tabControl.Controls.Add(tabViewPage);
            tabControl.Depth = 0;
            tabControl.Dock = DockStyle.Fill;
            tabControl.Location = new Point(3, 64);
            tabControl.MouseState = MouseState.HOVER;
            tabControl.Multiline = true;
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(674, 511);
            tabControl.TabIndex = 0;
            // 
            // tabControlPage
            // 
            tabControlPage.Controls.Add(txtType);
            tabControlPage.Controls.Add(txtAmmount);
            tabControlPage.Controls.Add(txtDescription);
            tabControlPage.Controls.Add(btnAdd);
            tabControlPage.Location = new Point(4, 24);
            tabControlPage.Name = "tabControlPage";
            tabControlPage.Padding = new Padding(3);
            tabControlPage.Size = new Size(666, 483);
            tabControlPage.TabIndex = 0;
            tabControlPage.Text = "Entry";
            tabControlPage.UseVisualStyleBackColor = true;
            // 
            // txtType
            // 
            txtType.AutoResize = false;
            txtType.BackColor = Color.FromArgb(255, 255, 255);
            txtType.Depth = 0;
            txtType.DrawMode = DrawMode.OwnerDrawVariable;
            txtType.DropDownHeight = 174;
            txtType.DropDownStyle = ComboBoxStyle.DropDownList;
            txtType.DropDownWidth = 121;
            txtType.Font = new Font("Roboto Medium", 14F, FontStyle.Bold, GraphicsUnit.Pixel);
            txtType.ForeColor = Color.FromArgb(222, 0, 0, 0);
            txtType.Hint = "Type";
            txtType.IntegralHeight = false;
            txtType.ItemHeight = 43;
            txtType.Items.AddRange(new object[] { "Debit", "Credit" });
            txtType.Location = new Point(20, 80);
            txtType.MaxDropDownItems = 4;
            txtType.MouseState = MouseState.OUT;
            txtType.Name = "txtType";
            txtType.Size = new Size(200, 49);
            txtType.StartIndex = 0;
            txtType.TabIndex = 0;
            // 
            // txtAmmount
            // 
            txtAmmount.AnimateReadOnly = false;
            txtAmmount.BorderStyle = BorderStyle.None;
            txtAmmount.Depth = 0;
            txtAmmount.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtAmmount.Hint = "Amount";
            txtAmmount.LeadingIcon = null;
            txtAmmount.Location = new Point(20, 160);
            txtAmmount.MaxLength = 32767;
            txtAmmount.MouseState = MouseState.OUT;
            txtAmmount.Multiline = false;
            txtAmmount.Name = "txtAmmount";
            txtAmmount.Size = new Size(200, 50);
            txtAmmount.TabIndex = 1;
            txtAmmount.Text = "";
            txtAmmount.TrailingIcon = null;
            // 
            // txtDescription
            // 
            txtDescription.AnimateReadOnly = false;
            txtDescription.BorderStyle = BorderStyle.None;
            txtDescription.Depth = 0;
            txtDescription.Font = new Font("Roboto", 16F, FontStyle.Regular, GraphicsUnit.Pixel);
            txtDescription.Hint = "Description";
            txtDescription.LeadingIcon = null;
            txtDescription.Location = new Point(20, 240);
            txtDescription.MaxLength = 32767;
            txtDescription.MouseState = MouseState.OUT;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(200, 50);
            txtDescription.TabIndex = 2;
            txtDescription.Text = "";
            txtDescription.TrailingIcon = null;
            // 
            // btnAdd
            // 
            btnAdd.AutoSize = false;
            btnAdd.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnAdd.Density = MaterialButton.MaterialButtonDensity.Default;
            btnAdd.Depth = 0;
            btnAdd.HighEmphasis = true;
            btnAdd.Icon = null;
            btnAdd.Location = new Point(20, 350);
            btnAdd.Margin = new Padding(4, 6, 4, 6);
            btnAdd.MouseState = MouseState.HOVER;
            btnAdd.Name = "btnAdd";
            btnAdd.NoAccentTextColor = Color.Empty;
            btnAdd.Size = new Size(200, 36);
            btnAdd.TabIndex = 3;
            btnAdd.Text = "Add";
            btnAdd.Type = MaterialButton.MaterialButtonType.Contained;
            btnAdd.UseAccentColor = false;
            btnAdd.UseVisualStyleBackColor = true;
            btnAdd.Click += AddEntry_Click;
            // 
            // tabViewPage
            // 
            tabViewPage.Controls.Add(btnAll);
            tabViewPage.Controls.Add(btnSynchronized);
            tabViewPage.Controls.Add(btnDirty);
            tabViewPage.Controls.Add(listViewEntries);
            tabViewPage.Location = new Point(4, 24);
            tabViewPage.Name = "tabViewPage";
            tabViewPage.Padding = new Padding(3);
            tabViewPage.Size = new Size(672, 522);
            tabViewPage.TabIndex = 1;
            tabViewPage.Text = "List";
            tabViewPage.UseVisualStyleBackColor = true;
            // 
            // btnAll
            // 
            btnAll.AutoSize = false;
            btnAll.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnAll.Density = MaterialButton.MaterialButtonDensity.Default;
            btnAll.Depth = 0;
            btnAll.HighEmphasis = true;
            btnAll.Icon = null;
            btnAll.Location = new Point(20, 60);
            btnAll.Margin = new Padding(4, 6, 4, 6);
            btnAll.MouseState = MouseState.HOVER;
            btnAll.Name = "btnAll";
            btnAll.NoAccentTextColor = Color.Empty;
            btnAll.Size = new Size(100, 36);
            btnAll.TabIndex = 0;
            btnAll.Text = "All";
            btnAll.Type = MaterialButton.MaterialButtonType.Contained;
            btnAll.UseAccentColor = false;
            btnAll.UseVisualStyleBackColor = true;
            btnAll.Click += FilterAll_Click;
            // 
            // btnSynchronized
            // 
            btnSynchronized.AutoSize = false;
            btnSynchronized.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnSynchronized.Density = MaterialButton.MaterialButtonDensity.Default;
            btnSynchronized.Depth = 0;
            btnSynchronized.HighEmphasis = true;
            btnSynchronized.Icon = null;
            btnSynchronized.Location = new Point(140, 60);
            btnSynchronized.Margin = new Padding(4, 6, 4, 6);
            btnSynchronized.MouseState = MouseState.HOVER;
            btnSynchronized.Name = "btnSynchronized";
            btnSynchronized.NoAccentTextColor = Color.Empty;
            btnSynchronized.Size = new Size(120, 36);
            btnSynchronized.TabIndex = 1;
            btnSynchronized.Text = "Synchronized";
            btnSynchronized.Type = MaterialButton.MaterialButtonType.Contained;
            btnSynchronized.UseAccentColor = false;
            btnSynchronized.UseVisualStyleBackColor = true;
            btnSynchronized.Click += FilterSynchronized_Click;
            // 
            // btnDirty
            // 
            btnDirty.AutoSize = false;
            btnDirty.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            btnDirty.Density = MaterialButton.MaterialButtonDensity.Default;
            btnDirty.Depth = 0;
            btnDirty.HighEmphasis = true;
            btnDirty.Icon = null;
            btnDirty.Location = new Point(280, 60);
            btnDirty.Margin = new Padding(4, 6, 4, 6);
            btnDirty.MouseState = MouseState.HOVER;
            btnDirty.Name = "btnDirty";
            btnDirty.NoAccentTextColor = Color.Empty;
            btnDirty.Size = new Size(150, 36);
            btnDirty.TabIndex = 2;
            btnDirty.Text = "Not Synchronized";
            btnDirty.Type = MaterialButton.MaterialButtonType.Contained;
            btnDirty.UseAccentColor = false;
            btnDirty.UseVisualStyleBackColor = true;
            btnDirty.Click += FilterDirty_Click;
            // 
            // listViewEntries
            // 
            listViewEntries.AutoSizeTable = false;
            listViewEntries.BackColor = Color.FromArgb(255, 255, 255);
            listViewEntries.BorderStyle = BorderStyle.None;
            listViewEntries.Depth = 0;
            listViewEntries.FullRowSelect = true;
            listViewEntries.Location = new Point(20, 100);
            listViewEntries.MinimumSize = new Size(200, 100);
            listViewEntries.MouseLocation = new Point(-1, -1);
            listViewEntries.MouseState = MouseState.OUT;
            listViewEntries.Name = "listViewEntries";
            listViewEntries.OwnerDraw = true;
            listViewEntries.Size = new Size(630, 400);
            listViewEntries.TabIndex = 3;
            listViewEntries.UseCompatibleStateImageBehavior = false;
            listViewEntries.View = View.Details;
            listViewEntries.FullRowSelect = true;
            listViewEntries.GridLines = true;
            listViewEntries.HideSelection = false;
            listViewEntries.Columns.Add("ID", 50);
            listViewEntries.Columns.Add("Type", 100);
            listViewEntries.Columns.Add("Amount", 120);
            listViewEntries.Columns.Add("Description", 150);
            listViewEntries.Columns.Add("Date", 100);
            listViewEntries.Columns.Add("Synchronized", 120);

            // 
            // tabSelector
            // 
            tabSelector.BaseTabControl = tabControl;
            tabSelector.CharacterCasing = MaterialTabSelector.CustomCharacterCasing.Normal;
            tabSelector.Depth = 0;
            tabSelector.Dock = DockStyle.Top;
            tabSelector.Font = new Font("Roboto", 14F, FontStyle.Regular, GraphicsUnit.Pixel);
            tabSelector.Location = new Point(3, 64);
            tabSelector.MouseState = MouseState.HOVER;
            tabSelector.Name = "tabSelector";
            tabSelector.Size = new Size(674, 48);
            tabSelector.TabIndex = 1;
            tabSelector.Text = "tabSelector";
            // 
            // statusBar
            // 
            statusBar.Items.AddRange(new ToolStripItem[] { statusLabel, connectionStatusLabel });
            statusBar.Location = new Point(3, 575);
            statusBar.Name = "statusBar";
            statusBar.Size = new Size(674, 22);
            statusBar.TabIndex = 2;
            // 
            // statusLabel
            // 
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new Size(110, 17);
            statusLabel.Text = "Connection Status: ";
            // 
            // connectionStatusLabel
            // 
            connectionStatusLabel.BackColor = Color.Red;
            connectionStatusLabel.Name = "connectionStatusLabel";
            connectionStatusLabel.Size = new Size(10, 17);
            connectionStatusLabel.Text = " ";
            // 
            // MainForm
            // 
            ClientSize = new Size(680, 600);
            Controls.Add(tabSelector);
            Controls.Add(tabControl);
            Controls.Add(statusBar);
            Name = "MainForm";
            Text = "Entry Management";
            tabControl.ResumeLayout(false);
            tabControlPage.ResumeLayout(false);
            tabViewPage.ResumeLayout(false);
            statusBar.ResumeLayout(false);
            statusBar.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private MaterialSkin.Controls.MaterialTabControl tabControl;
        private System.Windows.Forms.TabPage tabControlPage;
        private System.Windows.Forms.TabPage tabViewPage;
        private System.Windows.Forms.StatusStrip statusBar;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel connectionStatusLabel;
        private MaterialSkin.Controls.MaterialComboBox txtType;
        private MaterialSkin.Controls.MaterialTextBox txtAmmount;
        private MaterialSkin.Controls.MaterialTextBox txtDescription;
        private MaterialSkin.Controls.MaterialButton btnAdd;
        private MaterialSkin.Controls.MaterialButton btnAll;
        private MaterialSkin.Controls.MaterialButton btnSynchronized;
        private MaterialSkin.Controls.MaterialButton btnDirty;
        private MaterialSkin.Controls.MaterialListView listViewEntries;

    }
}
