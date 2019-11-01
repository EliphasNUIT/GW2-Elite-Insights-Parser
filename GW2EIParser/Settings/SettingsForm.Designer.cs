namespace GW2EIParser.Setting
{
    partial class SettingsForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.lblSettingsInfoMsg = new System.Windows.Forms.Label();
            this.chkDefaultOutputLoc = new System.Windows.Forms.CheckBox();
            this.txtCustomSaveLoc = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btnFolderSelect = new System.Windows.Forms.Button();
            this.lblCustomSaveLoc = new System.Windows.Forms.Label();
            this.btnResetSkillList = new System.Windows.Forms.Button();
            this.btnResetTraitList = new System.Windows.Forms.Button();
            this.btnResetSpecList = new System.Windows.Forms.Button();
            this.chkOutputHtml = new System.Windows.Forms.CheckBox();
            this.chkPhaseParsing = new System.Windows.Forms.CheckBox();
            this.chkOneAtATime = new System.Windows.Forms.CheckBox();
            this.chkCombatReplay = new System.Windows.Forms.CheckBox();
            this.UploadDPSReports_checkbox = new System.Windows.Forms.CheckBox();
            this.UploadDRRH_check = new System.Windows.Forms.CheckBox();
            this.chkOutputJson = new System.Windows.Forms.CheckBox();
            this.chkIndentJSON = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkAnonymous = new System.Windows.Forms.CheckBox();
            this.chkHtmlExternalScripts = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabEISettings = new System.Windows.Forms.TabPage();
            this.chkAddPoVProf = new System.Windows.Forms.CheckBox();
            this.chkAddDuration = new System.Windows.Forms.CheckBox();
            this.chkAutoParse = new System.Windows.Forms.CheckBox();
            this.chkAutoAdd = new System.Windows.Forms.CheckBox();
            this.chkB_SkipFailedTries = new System.Windows.Forms.CheckBox();
            this.tabFormat = new System.Windows.Forms.TabPage();
            this.panelJson = new System.Windows.Forms.Panel();
            this.chkCompressJson = new System.Windows.Forms.CheckBox();
            this.panelHtml = new System.Windows.Forms.Panel();
            this.imgTheme = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.radioThemeLight = new System.Windows.Forms.RadioButton();
            this.radioThemeDark = new System.Windows.Forms.RadioButton();
            this.tabParseSettings = new System.Windows.Forms.TabPage();
            this.tabUpload = new System.Windows.Forms.TabPage();
            this.tabAPI = new System.Windows.Forms.TabPage();
            this.resetSkillLabel = new System.Windows.Forms.Label();
            this.resetTraitLabel = new System.Windows.Forms.Label();
            this.resetSpecLabel = new System.Windows.Forms.Label();
            this.cmdClose = new System.Windows.Forms.Button();
            this.dumpButton = new System.Windows.Forms.Button();
            this.loadButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabEISettings.SuspendLayout();
            this.tabFormat.SuspendLayout();
            this.panelJson.SuspendLayout();
            this.panelHtml.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgTheme)).BeginInit();
            this.panel1.SuspendLayout();
            this.tabParseSettings.SuspendLayout();
            this.tabUpload.SuspendLayout();
            this.tabAPI.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSettingsInfoMsg
            // 
            this.lblSettingsInfoMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSettingsInfoMsg.AutoSize = true;
            this.lblSettingsInfoMsg.Location = new System.Drawing.Point(9, 219);
            this.lblSettingsInfoMsg.Name = "lblSettingsInfoMsg";
            this.lblSettingsInfoMsg.Size = new System.Drawing.Size(251, 13);
            this.lblSettingsInfoMsg.TabIndex = 12;
            this.lblSettingsInfoMsg.Text = "*Changes will not alter files that are currently parsing";
            // 
            // chkDefaultOutputLoc
            // 
            this.chkDefaultOutputLoc.AutoSize = true;
            this.chkDefaultOutputLoc.Checked = true;
            this.chkDefaultOutputLoc.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDefaultOutputLoc.Location = new System.Drawing.Point(185, 19);
            this.chkDefaultOutputLoc.Name = "chkDefaultOutputLoc";
            this.chkDefaultOutputLoc.Size = new System.Drawing.Size(192, 17);
            this.chkDefaultOutputLoc.TabIndex = 14;
            this.chkDefaultOutputLoc.Text = "Save Output in same folder as evtc";
            this.chkDefaultOutputLoc.UseVisualStyleBackColor = true;
            this.chkDefaultOutputLoc.CheckedChanged += new System.EventHandler(this.DefaultOutputLocationCheckedChanged);
            // 
            // txtCustomSaveLoc
            // 
            this.txtCustomSaveLoc.Enabled = false;
            this.txtCustomSaveLoc.Location = new System.Drawing.Point(9, 141);
            this.txtCustomSaveLoc.Name = "txtCustomSaveLoc";
            this.txtCustomSaveLoc.Size = new System.Drawing.Size(370, 20);
            this.txtCustomSaveLoc.TabIndex = 15;
            this.txtCustomSaveLoc.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCustomSaveLoc.TextChanged += new System.EventHandler(this.CustomSaveLocationTextChanged);
            // 
            // btnFolderSelect
            // 
            this.btnFolderSelect.Location = new System.Drawing.Point(385, 141);
            this.btnFolderSelect.Name = "btnFolderSelect";
            this.btnFolderSelect.Size = new System.Drawing.Size(45, 20);
            this.btnFolderSelect.TabIndex = 16;
            this.btnFolderSelect.Text = "Select";
            this.btnFolderSelect.UseVisualStyleBackColor = true;
            this.btnFolderSelect.Click += new System.EventHandler(this.BtnFolderSelectClick);
            // 
            // lblCustomSaveLoc
            // 
            this.lblCustomSaveLoc.AutoSize = true;
            this.lblCustomSaveLoc.Location = new System.Drawing.Point(6, 125);
            this.lblCustomSaveLoc.Name = "lblCustomSaveLoc";
            this.lblCustomSaveLoc.Size = new System.Drawing.Size(98, 13);
            this.lblCustomSaveLoc.TabIndex = 17;
            this.lblCustomSaveLoc.Text = "Other output folder:";
            // 
            // btnResetSkillList
            // 
            this.btnResetSkillList.Location = new System.Drawing.Point(27, 132);
            this.btnResetSkillList.Name = "btnResetSkillList";
            this.btnResetSkillList.Size = new System.Drawing.Size(144, 23);
            this.btnResetSkillList.TabIndex = 24;
            this.btnResetSkillList.Text = "Reset Skill List ";
            this.btnResetSkillList.UseVisualStyleBackColor = true;
            this.btnResetSkillList.Click += new System.EventHandler(this.ResetSkillListClick);
            // 
            // btnResetTraitList
            // 
            this.btnResetTraitList.Location = new System.Drawing.Point(27, 78);
            this.btnResetTraitList.Name = "btnResetTraitList";
            this.btnResetTraitList.Size = new System.Drawing.Size(144, 23);
            this.btnResetTraitList.TabIndex = 24;
            this.btnResetTraitList.Text = "Reset Trait List ";
            this.btnResetTraitList.UseVisualStyleBackColor = true;
            this.btnResetTraitList.Click += new System.EventHandler(this.ResetTraitListClick);
            // 
            // btnResetSpecList
            // 
            this.btnResetSpecList.Location = new System.Drawing.Point(27, 27);
            this.btnResetSpecList.Name = "btnResetSpecList";
            this.btnResetSpecList.Size = new System.Drawing.Size(144, 23);
            this.btnResetSpecList.TabIndex = 25;
            this.btnResetSpecList.Text = "Reset Spec List";
            this.btnResetSpecList.UseVisualStyleBackColor = true;
            this.btnResetSpecList.Click += new System.EventHandler(this.ResetSpecListClick);
            // 
            // chkOutputHtml
            // 
            this.chkOutputHtml.AutoSize = true;
            this.chkOutputHtml.Checked = true;
            this.chkOutputHtml.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOutputHtml.Location = new System.Drawing.Point(12, 12);
            this.chkOutputHtml.Name = "chkOutputHtml";
            this.chkOutputHtml.Size = new System.Drawing.Size(105, 17);
            this.chkOutputHtml.TabIndex = 26;
            this.chkOutputHtml.Text = "Output as HTML";
            this.chkOutputHtml.UseVisualStyleBackColor = true;
            this.chkOutputHtml.CheckedChanged += new System.EventHandler(this.OuputHTMLCheckedChanged);
            // 
            // chkPhaseParsing
            // 
            this.chkPhaseParsing.AutoSize = true;
            this.chkPhaseParsing.Checked = true;
            this.chkPhaseParsing.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPhaseParsing.Location = new System.Drawing.Point(16, 14);
            this.chkPhaseParsing.Name = "chkPhaseParsing";
            this.chkPhaseParsing.Size = new System.Drawing.Size(91, 17);
            this.chkPhaseParsing.TabIndex = 30;
            this.chkPhaseParsing.Text = "Parse Phases";
            this.chkPhaseParsing.UseVisualStyleBackColor = true;
            this.chkPhaseParsing.CheckedChanged += new System.EventHandler(this.PhaseParsingCheckedChanged);
            // 
            // chkOneAtATime
            // 
            this.chkOneAtATime.AutoSize = true;
            this.chkOneAtATime.Location = new System.Drawing.Point(6, 19);
            this.chkOneAtATime.Name = "chkOneAtATime";
            this.chkOneAtATime.Size = new System.Drawing.Size(139, 17);
            this.chkOneAtATime.TabIndex = 29;
            this.chkOneAtATime.Text = "Parse logs one at a time";
            this.toolTip1.SetToolTip(this.chkOneAtATime, "Slower parsing but less ressource hungry. Keep default value if unsure.");
            this.chkOneAtATime.UseVisualStyleBackColor = true;
            this.chkOneAtATime.CheckedChanged += new System.EventHandler(this.ChkOneAtATimeCheckedChanged);
            // 
            // chkCombatReplay
            // 
            this.chkCombatReplay.AutoSize = true;
            this.chkCombatReplay.Checked = true;
            this.chkCombatReplay.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCombatReplay.Location = new System.Drawing.Point(16, 37);
            this.chkCombatReplay.Name = "chkCombatReplay";
            this.chkCombatReplay.Size = new System.Drawing.Size(143, 17);
            this.chkCombatReplay.TabIndex = 40;
            this.chkCombatReplay.Text = "Compute Combat Replay";
            this.chkCombatReplay.UseVisualStyleBackColor = true;
            this.chkCombatReplay.CheckedChanged += new System.EventHandler(this.ChkCombatReplayCheckedChanged);
            // 
            // UploadDPSReports_checkbox
            // 
            this.UploadDPSReports_checkbox.AutoSize = true;
            this.UploadDPSReports_checkbox.Location = new System.Drawing.Point(15, 13);
            this.UploadDPSReports_checkbox.Name = "UploadDPSReports_checkbox";
            this.UploadDPSReports_checkbox.Size = new System.Drawing.Size(196, 17);
            this.UploadDPSReports_checkbox.TabIndex = 43;
            this.UploadDPSReports_checkbox.Text = "Upload to DPSReports Elite Insights";
            this.UploadDPSReports_checkbox.UseVisualStyleBackColor = true;
            this.UploadDPSReports_checkbox.CheckedChanged += new System.EventHandler(this.UploadDPSReports_checkbox_CheckedChanged);
            // 
            // UploadDRRH_check
            // 
            this.UploadDRRH_check.AutoSize = true;
            this.UploadDRRH_check.Location = new System.Drawing.Point(15, 36);
            this.UploadDRRH_check.Name = "UploadDRRH_check";
            this.UploadDRRH_check.Size = new System.Drawing.Size(193, 17);
            this.UploadDRRH_check.TabIndex = 44;
            this.UploadDRRH_check.Text = "Upload to DPSReports RaidHeroes";
            this.UploadDRRH_check.UseVisualStyleBackColor = true;
            this.UploadDRRH_check.CheckedChanged += new System.EventHandler(this.UploadDRRH_check_CheckedChanged);
            // 
            // chkOutputJson
            // 
            this.chkOutputJson.AutoSize = true;
            this.chkOutputJson.Location = new System.Drawing.Point(12, 120);
            this.chkOutputJson.Name = "chkOutputJson";
            this.chkOutputJson.Size = new System.Drawing.Size(103, 17);
            this.chkOutputJson.TabIndex = 43;
            this.chkOutputJson.Text = "Output as JSON";
            this.chkOutputJson.UseVisualStyleBackColor = true;
            this.chkOutputJson.CheckedChanged += new System.EventHandler(this.OutputJSONCheckedChanged);
            // 
            // chkIndentJSON
            // 
            this.chkIndentJSON.AutoSize = true;
            this.chkIndentJSON.Location = new System.Drawing.Point(3, 3);
            this.chkIndentJSON.Name = "chkIndentJSON";
            this.chkIndentJSON.Size = new System.Drawing.Size(87, 17);
            this.chkIndentJSON.TabIndex = 44;
            this.chkIndentJSON.Text = "Indent JSON";
            this.chkIndentJSON.UseVisualStyleBackColor = true;
            this.chkIndentJSON.CheckedChanged += new System.EventHandler(this.ChkIndentJSONCheckedChanged);
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipTitle = "Setting description";
            // 
            // chkAnonymous
            // 
            this.chkAnonymous.AutoSize = true;
            this.chkAnonymous.Location = new System.Drawing.Point(185, 42);
            this.chkAnonymous.Name = "chkAnonymous";
            this.chkAnonymous.Size = new System.Drawing.Size(118, 17);
            this.chkAnonymous.TabIndex = 20;
            this.chkAnonymous.Text = "Anonymous Players";
            this.toolTip1.SetToolTip(this.chkAnonymous, "Replaces Players\' account names and character names by generic names");
            this.chkAnonymous.UseVisualStyleBackColor = true;
            this.chkAnonymous.CheckedChanged += new System.EventHandler(this.ChkAnonymous_CheckedChanged);
            // 
            // chkHtmlExternalScripts
            // 
            this.chkHtmlExternalScripts.AutoSize = true;
            this.chkHtmlExternalScripts.Location = new System.Drawing.Point(3, 8);
            this.chkHtmlExternalScripts.Name = "chkHtmlExternalScripts";
            this.chkHtmlExternalScripts.Size = new System.Drawing.Size(99, 17);
            this.chkHtmlExternalScripts.TabIndex = 46;
            this.chkHtmlExternalScripts.Text = "External Scripts";
            this.chkHtmlExternalScripts.UseVisualStyleBackColor = true;
            this.chkHtmlExternalScripts.CheckedChanged += new System.EventHandler(this.ChkHtmlExternalScripts_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabEISettings);
            this.tabControl1.Controls.Add(this.tabParseSettings);
            this.tabControl1.Controls.Add(this.tabFormat);
            this.tabControl1.Controls.Add(this.tabUpload);
            this.tabControl1.Controls.Add(this.tabAPI);
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(451, 204);
            this.tabControl1.TabIndex = 47;
            // 
            // tabPage1
            // 
            this.tabEISettings.Controls.Add(this.btnFolderSelect);
            this.tabEISettings.Controls.Add(this.txtCustomSaveLoc);
            this.tabEISettings.Controls.Add(this.lblCustomSaveLoc);
            this.tabEISettings.Controls.Add(this.chkAddPoVProf);
            this.tabEISettings.Controls.Add(this.chkOneAtATime);
            this.tabEISettings.Controls.Add(this.chkB_SkipFailedTries);
            this.tabEISettings.Controls.Add(this.chkAutoAdd);
            this.tabEISettings.Controls.Add(this.chkAddDuration);
            this.tabEISettings.Controls.Add(this.chkAutoParse);
            this.tabEISettings.Controls.Add(this.chkAnonymous);
            this.tabEISettings.Controls.Add(this.chkDefaultOutputLoc);
            this.tabEISettings.Location = new System.Drawing.Point(4, 22);
            this.tabEISettings.Name = "eiSettings";
            this.tabEISettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabEISettings.Size = new System.Drawing.Size(443, 178);
            this.tabEISettings.TabIndex = 0;
            this.tabEISettings.Text = "EI Settings";
            this.tabEISettings.UseVisualStyleBackColor = true;
            // 
            // chkAddPoVProf
            // 
            this.chkAddPoVProf.AutoSize = true;
            this.chkAddPoVProf.Location = new System.Drawing.Point(185, 88);
            this.chkAddPoVProf.Name = "chkAddPoVProf";
            this.chkAddPoVProf.Size = new System.Drawing.Size(176, 17);
            this.chkAddPoVProf.TabIndex = 18;
            this.chkAddPoVProf.Text = "Add PoV profession to file name";
            this.chkAddPoVProf.UseVisualStyleBackColor = true;
            this.chkAddPoVProf.CheckedChanged += new System.EventHandler(this.ChkAddPoVProf_CheckedChanged);
            // 
            // chkAddDuration
            // 
            this.chkAddDuration.AutoSize = true;
            this.chkAddDuration.Location = new System.Drawing.Point(185, 65);
            this.chkAddDuration.Name = "chkAddDuration";
            this.chkAddDuration.Size = new System.Drawing.Size(194, 17);
            this.chkAddDuration.TabIndex = 19;
            this.chkAddDuration.Text = "Add encounter duration to file name";
            this.chkAddDuration.UseVisualStyleBackColor = true;
            this.chkAddDuration.CheckedChanged += new System.EventHandler(this.ChkAddDuration_CheckedChanged);
            // 
            // chkAutoParse
            // 
            this.chkAutoParse.AutoSize = true;
            this.chkAutoParse.Location = new System.Drawing.Point(6, 88);
            this.chkAutoParse.Name = "chkAutoParse";
            this.chkAutoParse.Size = new System.Drawing.Size(171, 17);
            this.chkAutoParse.TabIndex = 39;
            this.chkAutoParse.Text = "Automatically parse added files";
            this.chkAutoParse.UseVisualStyleBackColor = true;
            this.chkAutoParse.CheckedChanged += new System.EventHandler(this.ChkAutoParse_CheckedChanged);
            // 
            // chkAutoAdd
            // 
            this.chkAutoAdd.AutoSize = true;
            this.chkAutoAdd.Location = new System.Drawing.Point(6, 65);
            this.chkAutoAdd.Name = "chkAutoAdd";
            this.chkAutoAdd.Size = new System.Drawing.Size(154, 17);
            this.chkAutoAdd.TabIndex = 40;
            this.chkAutoAdd.Text = "Automatically add new logs";
            this.chkAutoAdd.UseVisualStyleBackColor = true;
            this.chkAutoAdd.CheckedChanged += new System.EventHandler(this.ChkAutoAdd_CheckedChanged);
            // 
            // chkB_SkipFailedTries
            // 
            this.chkB_SkipFailedTries.AutoSize = true;
            this.chkB_SkipFailedTries.Location = new System.Drawing.Point(6, 42);
            this.chkB_SkipFailedTries.Name = "chkB_SkipFailedTries";
            this.chkB_SkipFailedTries.Size = new System.Drawing.Size(141, 17);
            this.chkB_SkipFailedTries.TabIndex = 38;
            this.chkB_SkipFailedTries.Text = "Skip generating log if fail";
            this.chkB_SkipFailedTries.UseVisualStyleBackColor = true;
            this.chkB_SkipFailedTries.CheckedChanged += new System.EventHandler(this.ChkB_SkipFailedTries_CheckedChanged);
            // 
            // formatPage
            // 
            this.tabFormat.Controls.Add(this.panelJson);
            this.tabFormat.Controls.Add(this.chkOutputJson);
            this.tabFormat.Controls.Add(this.panelHtml);
            this.tabFormat.Controls.Add(this.chkOutputHtml);
            this.tabFormat.Location = new System.Drawing.Point(4, 22);
            this.tabFormat.Name = "formatPage";
            this.tabFormat.Padding = new System.Windows.Forms.Padding(3);
            this.tabFormat.Size = new System.Drawing.Size(443, 178);
            this.tabFormat.TabIndex = 1;
            this.tabFormat.Text = "Formats";
            this.tabFormat.UseVisualStyleBackColor = true;
            // 
            // panelJson
            // 
            this.panelJson.Controls.Add(this.chkCompressJson);
            this.panelJson.Controls.Add(this.chkIndentJSON);
            this.panelJson.Location = new System.Drawing.Point(12, 143);
            this.panelJson.Name = "panelJson";
            this.panelJson.Size = new System.Drawing.Size(232, 29);
            this.panelJson.TabIndex = 45;
            // 
            // chkCompressJson
            // 
            this.chkCompressJson.AutoSize = true;
            this.chkCompressJson.Location = new System.Drawing.Point(111, 3);
            this.chkCompressJson.Name = "chkCompressJson";
            this.chkCompressJson.Size = new System.Drawing.Size(118, 17);
            this.chkCompressJson.TabIndex = 18;
            this.chkCompressJson.Text = "Compress Raw files";
            this.chkCompressJson.UseVisualStyleBackColor = true;
            this.chkCompressJson.CheckedChanged += new System.EventHandler(this.ChkCompressRaw_CheckedChanged);
            // 
            // panelHtml
            // 
            this.panelHtml.Controls.Add(this.imgTheme);
            this.panelHtml.Controls.Add(this.panel1);
            this.panelHtml.Controls.Add(this.chkHtmlExternalScripts);
            this.panelHtml.Location = new System.Drawing.Point(12, 35);
            this.panelHtml.Name = "panelHtml";
            this.panelHtml.Size = new System.Drawing.Size(323, 78);
            this.panelHtml.TabIndex = 54;
            // 
            // imgTheme
            // 
            this.imgTheme.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imgTheme.Location = new System.Drawing.Point(201, 8);
            this.imgTheme.Name = "imgTheme";
            this.imgTheme.Size = new System.Drawing.Size(94, 60);
            this.imgTheme.TabIndex = 49;
            this.imgTheme.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.radioThemeLight);
            this.panel1.Controls.Add(this.radioThemeDark);
            this.panel1.Location = new System.Drawing.Point(111, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(156, 60);
            this.panel1.TabIndex = 53;
            // 
            // radioThemeLight
            // 
            this.radioThemeLight.AutoSize = true;
            this.radioThemeLight.Location = new System.Drawing.Point(0, 0);
            this.radioThemeLight.Name = "radioThemeLight";
            this.radioThemeLight.Size = new System.Drawing.Size(84, 17);
            this.radioThemeLight.TabIndex = 47;
            this.radioThemeLight.TabStop = true;
            this.radioThemeLight.Text = "Light Theme";
            this.radioThemeLight.UseVisualStyleBackColor = true;
            this.radioThemeLight.CheckedChanged += new System.EventHandler(this.RadioThemeLight_CheckedChanged);
            // 
            // radioThemeDark
            // 
            this.radioThemeDark.AutoSize = true;
            this.radioThemeDark.Location = new System.Drawing.Point(0, 24);
            this.radioThemeDark.Name = "radioThemeDark";
            this.radioThemeDark.Size = new System.Drawing.Size(84, 17);
            this.radioThemeDark.TabIndex = 48;
            this.radioThemeDark.TabStop = true;
            this.radioThemeDark.Text = "Dark Theme";
            this.radioThemeDark.UseVisualStyleBackColor = true;
            this.radioThemeDark.CheckedChanged += new System.EventHandler(this.RadioThemeDark_CheckedChanged);
            // 
            // tabSettings
            // 
            this.tabParseSettings.Controls.Add(this.chkCombatReplay);
            this.tabParseSettings.Controls.Add(this.chkPhaseParsing);
            this.tabParseSettings.Location = new System.Drawing.Point(4, 22);
            this.tabParseSettings.Name = "tabParseSettings";
            this.tabParseSettings.Size = new System.Drawing.Size(443, 178);
            this.tabParseSettings.TabIndex = 3;
            this.tabParseSettings.Text = "Parse Settings";
            this.tabParseSettings.UseVisualStyleBackColor = true;
            // 
            // tabUpload
            // 
            this.tabUpload.Controls.Add(this.UploadDPSReports_checkbox);
            this.tabUpload.Controls.Add(this.UploadDRRH_check);
            this.tabUpload.Location = new System.Drawing.Point(4, 22);
            this.tabUpload.Name = "tabUpload";
            this.tabUpload.Size = new System.Drawing.Size(443, 178);
            this.tabUpload.TabIndex = 4;
            this.tabUpload.Text = "Upload";
            this.tabUpload.UseVisualStyleBackColor = true;
            // 
            // tabAPI
            // 
            this.tabAPI.Controls.Add(this.resetSkillLabel);
            this.tabAPI.Controls.Add(this.resetTraitLabel);
            this.tabAPI.Controls.Add(this.resetSpecLabel);
            this.tabAPI.Controls.Add(this.btnResetSpecList);
            this.tabAPI.Controls.Add(this.btnResetTraitList);
            this.tabAPI.Controls.Add(this.btnResetSkillList);
            this.tabAPI.Location = new System.Drawing.Point(4, 22);
            this.tabAPI.Name = "tabAPI";
            this.tabAPI.Size = new System.Drawing.Size(443, 178);
            this.tabAPI.TabIndex = 5;
            this.tabAPI.Text = "Maintenance";
            this.tabAPI.UseVisualStyleBackColor = true;
            // 
            // resetSkillLabel
            // 
            this.resetSkillLabel.AutoSize = true;
            this.resetSkillLabel.Location = new System.Drawing.Point(24, 116);
            this.resetSkillLabel.Name = "resetSkillLabel";
            this.resetSkillLabel.Size = new System.Drawing.Size(294, 13);
            this.resetSkillLabel.TabIndex = 27;
            this.resetSkillLabel.Text = "Resets the local skill list and loads all skills from the GW2 API";
            // 
            // resetTraitLabel
            // 
            this.resetTraitLabel.AutoSize = true;
            this.resetTraitLabel.Location = new System.Drawing.Point(24, 62);
            this.resetTraitLabel.Name = "resetTraitLabel";
            this.resetTraitLabel.Size = new System.Drawing.Size(289, 13);
            this.resetTraitLabel.TabIndex = 28;
            this.resetTraitLabel.Text = "Resets the local trait list and loads all trait from the GW2 API";
            // 
            // resetSpecLabel
            // 
            this.resetSpecLabel.AutoSize = true;
            this.resetSpecLabel.Location = new System.Drawing.Point(24, 11);
            this.resetSpecLabel.Name = "resetSpecLabel";
            this.resetSpecLabel.Size = new System.Drawing.Size(306, 13);
            this.resetSpecLabel.TabIndex = 26;
            this.resetSpecLabel.Text = "Resets the local spec list and loads all specs from the GW2 API";
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.Location = new System.Drawing.Point(384, 239);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(84, 24);
            this.cmdClose.TabIndex = 48;
            this.cmdClose.Text = "Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.CmdClose_Click);
            // 
            // dumpButton
            // 
            this.dumpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.dumpButton.Location = new System.Drawing.Point(294, 239);
            this.dumpButton.Name = "dumpButton";
            this.dumpButton.Size = new System.Drawing.Size(84, 24);
            this.dumpButton.TabIndex = 49;
            this.dumpButton.Text = "Save Settings";
            this.dumpButton.UseVisualStyleBackColor = true;
            this.dumpButton.Click += new System.EventHandler(this.SettingsDump_Click);
            // 
            // loadButton
            // 
            this.loadButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.loadButton.Location = new System.Drawing.Point(204, 239);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(84, 24);
            this.loadButton.TabIndex = 50;
            this.loadButton.Text = "Load Settings";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.SettingsLoad_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(475, 275);
            this.Controls.Add(this.loadButton);
            this.Controls.Add(this.dumpButton);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblSettingsInfoMsg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Parse settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsFormFormClosing);
            this.Load += new System.EventHandler(this.SettingsFormLoad);
            this.tabControl1.ResumeLayout(false);
            this.tabEISettings.ResumeLayout(false);
            this.tabEISettings.PerformLayout();
            this.tabFormat.ResumeLayout(false);
            this.tabFormat.PerformLayout();
            this.panelJson.ResumeLayout(false);
            this.panelJson.PerformLayout();
            this.panelHtml.ResumeLayout(false);
            this.panelHtml.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imgTheme)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabParseSettings.ResumeLayout(false);
            this.tabParseSettings.PerformLayout();
            this.tabUpload.ResumeLayout(false);
            this.tabUpload.PerformLayout();
            this.tabAPI.ResumeLayout(false);
            this.tabAPI.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblSettingsInfoMsg;
        private System.Windows.Forms.CheckBox chkDefaultOutputLoc;
        private System.Windows.Forms.TextBox txtCustomSaveLoc;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button btnFolderSelect;
        private System.Windows.Forms.Label lblCustomSaveLoc;
        private System.Windows.Forms.Button btnResetSkillList;
        private System.Windows.Forms.Button btnResetTraitList;
        private System.Windows.Forms.Button btnResetSpecList;
        private System.Windows.Forms.CheckBox chkOutputHtml;
        private System.Windows.Forms.CheckBox chkPhaseParsing;
        private System.Windows.Forms.CheckBox chkOneAtATime;
        private System.Windows.Forms.CheckBox chkCombatReplay;
        private System.Windows.Forms.CheckBox UploadDPSReports_checkbox;
        private System.Windows.Forms.CheckBox UploadDRRH_check;
        private System.Windows.Forms.CheckBox chkOutputJson;
        private System.Windows.Forms.CheckBox chkIndentJSON;
        private System.Windows.Forms.CheckBox chkCompressJson;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chkHtmlExternalScripts;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabEISettings;
        private System.Windows.Forms.TabPage tabFormat;
        private System.Windows.Forms.TabPage tabParseSettings;
        private System.Windows.Forms.TabPage tabUpload;
        private System.Windows.Forms.TabPage tabAPI;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton radioThemeLight;
        private System.Windows.Forms.RadioButton radioThemeDark;
        private System.Windows.Forms.Panel panelHtml;
        private System.Windows.Forms.Label resetSkillLabel;
        private System.Windows.Forms.Label resetTraitLabel;
        private System.Windows.Forms.Label resetSpecLabel;
        private System.Windows.Forms.PictureBox imgTheme;
        private System.Windows.Forms.CheckBox chkB_SkipFailedTries;
        private System.Windows.Forms.CheckBox chkAutoAdd;
        private System.Windows.Forms.CheckBox chkAutoParse;
        private System.Windows.Forms.CheckBox chkAddPoVProf;
        private System.Windows.Forms.CheckBox chkAddDuration;
        private System.Windows.Forms.Panel panelJson;
        private System.Windows.Forms.Button dumpButton;
        private System.Windows.Forms.Button loadButton;
        private System.Windows.Forms.CheckBox chkAnonymous;
    }
}

