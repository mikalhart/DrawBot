namespace DrawBot
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
         this.label1 = new System.Windows.Forms.Label();
         this.tbsvgFile = new System.Windows.Forms.TextBox();
         this.btnBrowseSVG = new System.Windows.Forms.Button();
         this.label2 = new System.Windows.Forms.Label();
         this.tbPageWidth = new System.Windows.Forms.TextBox();
         this.label3 = new System.Windows.Forms.Label();
         this.label4 = new System.Windows.Forms.Label();
         this.tbPageHeight = new System.Windows.Forms.TextBox();
         this.label5 = new System.Windows.Forms.Label();
         this.tbHubY = new System.Windows.Forms.TextBox();
         this.label6 = new System.Windows.Forms.Label();
         this.tbHubX = new System.Windows.Forms.TextBox();
         this.label7 = new System.Windows.Forms.Label();
         this.tbArm2Length = new System.Windows.Forms.TextBox();
         this.label9 = new System.Windows.Forms.Label();
         this.tbArm1Length = new System.Windows.Forms.TextBox();
         this.label10 = new System.Windows.Forms.Label();
         this.tbms180 = new System.Windows.Forms.TextBox();
         this.label8 = new System.Windows.Forms.Label();
         this.tbms0 = new System.Windows.Forms.TextBox();
         this.label11 = new System.Windows.Forms.Label();
         this.btnBrowseWav = new System.Windows.Forms.Button();
         this.tbWavFile = new System.Windows.Forms.TextBox();
         this.label12 = new System.Windows.Forms.Label();
         this.cbInvertWave = new System.Windows.Forms.CheckBox();
         this.btnGo = new System.Windows.Forms.Button();
         this.tbSpew = new System.Windows.Forms.TextBox();
         this.lvRawPoints = new System.Windows.Forms.ListView();
         this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.lvScaledPoints = new System.Windows.Forms.ListView();
         this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.lvAngles = new System.Windows.Forms.ListView();
         this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.lvBlobs = new System.Windows.Forms.ListView();
         this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.labRawPoints = new System.Windows.Forms.Label();
         this.labScaledPoints = new System.Windows.Forms.Label();
         this.labAngles = new System.Windows.Forms.Label();
         this.labBlobs = new System.Windows.Forms.Label();
         this.labTooFar = new System.Windows.Forms.Label();
         this.lvTooFar = new System.Windows.Forms.ListView();
         this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.labUnreachable = new System.Windows.Forms.Label();
         this.lvUnreachable = new System.Windows.Forms.ListView();
         this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.columnHeader18 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
         this.label13 = new System.Windows.Forms.Label();
         this.label14 = new System.Windows.Forms.Label();
         this.tbBaselineAdj = new System.Windows.Forms.TextBox();
         this.tbArmAdj = new System.Windows.Forms.TextBox();
         this.SuspendLayout();
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(24, 34);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(56, 13);
         this.label1.TabIndex = 0;
         this.label1.Text = "Input SVG";
         // 
         // tbsvgFile
         // 
         this.tbsvgFile.Location = new System.Drawing.Point(87, 34);
         this.tbsvgFile.Name = "tbsvgFile";
         this.tbsvgFile.Size = new System.Drawing.Size(309, 20);
         this.tbsvgFile.TabIndex = 1;
         this.tbsvgFile.TextChanged += new System.EventHandler(this.tbsvgFile_TextChanged);
         // 
         // btnBrowseSVG
         // 
         this.btnBrowseSVG.Location = new System.Drawing.Point(321, 74);
         this.btnBrowseSVG.Name = "btnBrowseSVG";
         this.btnBrowseSVG.Size = new System.Drawing.Size(75, 23);
         this.btnBrowseSVG.TabIndex = 2;
         this.btnBrowseSVG.Text = "&Browse";
         this.btnBrowseSVG.UseVisualStyleBackColor = true;
         this.btnBrowseSVG.Click += new System.EventHandler(this.btnBrowseSVG_Click);
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(24, 72);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(59, 13);
         this.label2.TabIndex = 3;
         this.label2.Text = "Paper size:";
         // 
         // tbPageWidth
         // 
         this.tbPageWidth.Location = new System.Drawing.Point(89, 72);
         this.tbPageWidth.Name = "tbPageWidth";
         this.tbPageWidth.Size = new System.Drawing.Size(41, 20);
         this.tbPageWidth.TabIndex = 4;
         this.tbPageWidth.Text = "5";
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(136, 79);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(32, 13);
         this.label3.TabIndex = 5;
         this.label3.Text = "width";
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(221, 79);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(36, 13);
         this.label4.TabIndex = 7;
         this.label4.Text = "height";
         // 
         // tbPageHeight
         // 
         this.tbPageHeight.Location = new System.Drawing.Point(174, 72);
         this.tbPageHeight.Name = "tbPageHeight";
         this.tbPageHeight.Size = new System.Drawing.Size(41, 20);
         this.tbPageHeight.TabIndex = 6;
         this.tbPageHeight.Text = "7";
         // 
         // label5
         // 
         this.label5.AutoSize = true;
         this.label5.Location = new System.Drawing.Point(229, 113);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(14, 13);
         this.label5.TabIndex = 12;
         this.label5.Text = "Y";
         // 
         // tbHubY
         // 
         this.tbHubY.Location = new System.Drawing.Point(174, 106);
         this.tbHubY.Name = "tbHubY";
         this.tbHubY.Size = new System.Drawing.Size(49, 20);
         this.tbHubY.TabIndex = 11;
         this.tbHubY.Text = "-0.8125";
         // 
         // label6
         // 
         this.label6.AutoSize = true;
         this.label6.Location = new System.Drawing.Point(136, 113);
         this.label6.Name = "label6";
         this.label6.Size = new System.Drawing.Size(14, 13);
         this.label6.TabIndex = 10;
         this.label6.Text = "X";
         // 
         // tbHubX
         // 
         this.tbHubX.Location = new System.Drawing.Point(89, 106);
         this.tbHubX.Name = "tbHubX";
         this.tbHubX.Size = new System.Drawing.Size(41, 20);
         this.tbHubX.TabIndex = 9;
         this.tbHubX.Text = "0.4375";
         // 
         // label7
         // 
         this.label7.AutoSize = true;
         this.label7.Location = new System.Drawing.Point(24, 106);
         this.label7.Name = "label7";
         this.label7.Size = new System.Drawing.Size(67, 13);
         this.label7.TabIndex = 8;
         this.label7.Text = "Hub location";
         // 
         // tbArm2Length
         // 
         this.tbArm2Length.Location = new System.Drawing.Point(174, 143);
         this.tbArm2Length.Name = "tbArm2Length";
         this.tbArm2Length.Size = new System.Drawing.Size(41, 20);
         this.tbArm2Length.TabIndex = 16;
         this.tbArm2Length.Text = "4";
         // 
         // label9
         // 
         this.label9.AutoSize = true;
         this.label9.Location = new System.Drawing.Point(136, 146);
         this.label9.Name = "label9";
         this.label9.Size = new System.Drawing.Size(34, 13);
         this.label9.TabIndex = 15;
         this.label9.Text = "Arm 2";
         // 
         // tbArm1Length
         // 
         this.tbArm1Length.Location = new System.Drawing.Point(89, 143);
         this.tbArm1Length.Name = "tbArm1Length";
         this.tbArm1Length.Size = new System.Drawing.Size(41, 20);
         this.tbArm1Length.TabIndex = 14;
         this.tbArm1Length.Text = "4";
         // 
         // label10
         // 
         this.label10.AutoSize = true;
         this.label10.Location = new System.Drawing.Point(24, 143);
         this.label10.Name = "label10";
         this.label10.Size = new System.Drawing.Size(55, 13);
         this.label10.TabIndex = 13;
         this.label10.Text = "Arm 1 Len";
         // 
         // tbms180
         // 
         this.tbms180.Location = new System.Drawing.Point(204, 179);
         this.tbms180.Name = "tbms180";
         this.tbms180.Size = new System.Drawing.Size(41, 20);
         this.tbms180.TabIndex = 20;
         this.tbms180.Text = "2400";
         // 
         // label8
         // 
         this.label8.AutoSize = true;
         this.label8.Location = new System.Drawing.Point(136, 182);
         this.label8.Name = "label8";
         this.label8.Size = new System.Drawing.Size(62, 13);
         this.label8.TabIndex = 19;
         this.label8.Text = "uS 180 deg";
         // 
         // tbms0
         // 
         this.tbms0.Location = new System.Drawing.Point(89, 179);
         this.tbms0.Name = "tbms0";
         this.tbms0.Size = new System.Drawing.Size(41, 20);
         this.tbms0.TabIndex = 18;
         this.tbms0.Text = "600";
         // 
         // label11
         // 
         this.label11.AutoSize = true;
         this.label11.Location = new System.Drawing.Point(24, 179);
         this.label11.Name = "label11";
         this.label11.Size = new System.Drawing.Size(50, 13);
         this.label11.TabIndex = 17;
         this.label11.Text = "uS 0 deg";
         // 
         // btnBrowseWav
         // 
         this.btnBrowseWav.Location = new System.Drawing.Point(321, 277);
         this.btnBrowseWav.Name = "btnBrowseWav";
         this.btnBrowseWav.Size = new System.Drawing.Size(75, 23);
         this.btnBrowseWav.TabIndex = 23;
         this.btnBrowseWav.Text = "B&rowse";
         this.btnBrowseWav.UseVisualStyleBackColor = true;
         this.btnBrowseWav.Click += new System.EventHandler(this.btnBrowseWav_Click);
         // 
         // tbWavFile
         // 
         this.tbWavFile.Location = new System.Drawing.Point(87, 250);
         this.tbWavFile.Name = "tbWavFile";
         this.tbWavFile.Size = new System.Drawing.Size(309, 20);
         this.tbWavFile.TabIndex = 22;
         // 
         // label12
         // 
         this.label12.AutoSize = true;
         this.label12.Location = new System.Drawing.Point(18, 250);
         this.label12.Name = "label12";
         this.label12.Size = new System.Drawing.Size(67, 13);
         this.label12.TabIndex = 21;
         this.label12.Text = "Output WAV";
         // 
         // cbInvertWave
         // 
         this.cbInvertWave.AutoSize = true;
         this.cbInvertWave.Location = new System.Drawing.Point(27, 297);
         this.cbInvertWave.Name = "cbInvertWave";
         this.cbInvertWave.Size = new System.Drawing.Size(85, 17);
         this.cbInvertWave.TabIndex = 24;
         this.cbInvertWave.Text = "&Invert Wave";
         this.cbInvertWave.UseVisualStyleBackColor = true;
         // 
         // btnGo
         // 
         this.btnGo.Location = new System.Drawing.Point(123, 293);
         this.btnGo.Name = "btnGo";
         this.btnGo.Size = new System.Drawing.Size(75, 23);
         this.btnGo.TabIndex = 25;
         this.btnGo.Text = "&Go!";
         this.btnGo.UseVisualStyleBackColor = true;
         this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
         // 
         // tbSpew
         // 
         this.tbSpew.Location = new System.Drawing.Point(12, 335);
         this.tbSpew.Multiline = true;
         this.tbSpew.Name = "tbSpew";
         this.tbSpew.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.tbSpew.Size = new System.Drawing.Size(363, 152);
         this.tbSpew.TabIndex = 26;
         // 
         // lvRawPoints
         // 
         this.lvRawPoints.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
         this.lvRawPoints.Location = new System.Drawing.Point(418, 34);
         this.lvRawPoints.Name = "lvRawPoints";
         this.lvRawPoints.Size = new System.Drawing.Size(211, 184);
         this.lvRawPoints.TabIndex = 27;
         this.lvRawPoints.UseCompatibleStateImageBehavior = false;
         this.lvRawPoints.View = System.Windows.Forms.View.Details;
         // 
         // columnHeader1
         // 
         this.columnHeader1.Text = "index";
         this.columnHeader1.Width = 46;
         // 
         // columnHeader2
         // 
         this.columnHeader2.Text = "X";
         this.columnHeader2.Width = 68;
         // 
         // columnHeader3
         // 
         this.columnHeader3.Text = "Y";
         this.columnHeader3.Width = 68;
         // 
         // lvScaledPoints
         // 
         this.lvScaledPoints.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
         this.lvScaledPoints.Location = new System.Drawing.Point(638, 34);
         this.lvScaledPoints.Name = "lvScaledPoints";
         this.lvScaledPoints.Size = new System.Drawing.Size(211, 184);
         this.lvScaledPoints.TabIndex = 28;
         this.lvScaledPoints.UseCompatibleStateImageBehavior = false;
         this.lvScaledPoints.View = System.Windows.Forms.View.Details;
         // 
         // columnHeader4
         // 
         this.columnHeader4.Text = "index";
         this.columnHeader4.Width = 46;
         // 
         // columnHeader5
         // 
         this.columnHeader5.Text = "X";
         this.columnHeader5.Width = 68;
         // 
         // columnHeader6
         // 
         this.columnHeader6.Text = "Y";
         this.columnHeader6.Width = 68;
         // 
         // lvAngles
         // 
         this.lvAngles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
         this.lvAngles.Location = new System.Drawing.Point(418, 256);
         this.lvAngles.Name = "lvAngles";
         this.lvAngles.Size = new System.Drawing.Size(211, 194);
         this.lvAngles.TabIndex = 29;
         this.lvAngles.UseCompatibleStateImageBehavior = false;
         this.lvAngles.View = System.Windows.Forms.View.Details;
         // 
         // columnHeader7
         // 
         this.columnHeader7.Text = "index";
         this.columnHeader7.Width = 46;
         // 
         // columnHeader8
         // 
         this.columnHeader8.Text = "arm1";
         this.columnHeader8.Width = 68;
         // 
         // columnHeader9
         // 
         this.columnHeader9.Text = "arm2";
         this.columnHeader9.Width = 68;
         // 
         // lvBlobs
         // 
         this.lvBlobs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12});
         this.lvBlobs.Location = new System.Drawing.Point(638, 256);
         this.lvBlobs.Name = "lvBlobs";
         this.lvBlobs.Size = new System.Drawing.Size(211, 194);
         this.lvBlobs.TabIndex = 30;
         this.lvBlobs.UseCompatibleStateImageBehavior = false;
         this.lvBlobs.View = System.Windows.Forms.View.Details;
         // 
         // columnHeader10
         // 
         this.columnHeader10.Text = "index";
         this.columnHeader10.Width = 46;
         // 
         // columnHeader11
         // 
         this.columnHeader11.Text = "left chan";
         this.columnHeader11.Width = 68;
         // 
         // columnHeader12
         // 
         this.columnHeader12.Text = "right chan";
         this.columnHeader12.Width = 68;
         // 
         // labRawPoints
         // 
         this.labRawPoints.AutoSize = true;
         this.labRawPoints.Location = new System.Drawing.Point(415, 18);
         this.labRawPoints.Name = "labRawPoints";
         this.labRawPoints.Size = new System.Drawing.Size(61, 13);
         this.labRawPoints.TabIndex = 31;
         this.labRawPoints.Text = "Raw Points";
         // 
         // labScaledPoints
         // 
         this.labScaledPoints.AutoSize = true;
         this.labScaledPoints.Location = new System.Drawing.Point(635, 18);
         this.labScaledPoints.Name = "labScaledPoints";
         this.labScaledPoints.Size = new System.Drawing.Size(72, 13);
         this.labScaledPoints.TabIndex = 32;
         this.labScaledPoints.Text = "Scaled Points";
         // 
         // labAngles
         // 
         this.labAngles.AutoSize = true;
         this.labAngles.Location = new System.Drawing.Point(415, 240);
         this.labAngles.Name = "labAngles";
         this.labAngles.Size = new System.Drawing.Size(39, 13);
         this.labAngles.TabIndex = 33;
         this.labAngles.Text = "Angles";
         // 
         // labBlobs
         // 
         this.labBlobs.AutoSize = true;
         this.labBlobs.Location = new System.Drawing.Point(635, 240);
         this.labBlobs.Name = "labBlobs";
         this.labBlobs.Size = new System.Drawing.Size(61, 13);
         this.labBlobs.TabIndex = 34;
         this.labBlobs.Text = "WAV Blobs";
         // 
         // labTooFar
         // 
         this.labTooFar.AutoSize = true;
         this.labTooFar.Location = new System.Drawing.Point(857, 18);
         this.labTooFar.Name = "labTooFar";
         this.labTooFar.Size = new System.Drawing.Size(96, 13);
         this.labTooFar.TabIndex = 36;
         this.labTooFar.Text = "Bad points (too far)";
         // 
         // lvTooFar
         // 
         this.lvTooFar.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader13,
            this.columnHeader14,
            this.columnHeader15});
         this.lvTooFar.Location = new System.Drawing.Point(860, 34);
         this.lvTooFar.Name = "lvTooFar";
         this.lvTooFar.Size = new System.Drawing.Size(211, 184);
         this.lvTooFar.TabIndex = 35;
         this.lvTooFar.UseCompatibleStateImageBehavior = false;
         this.lvTooFar.View = System.Windows.Forms.View.Details;
         // 
         // columnHeader13
         // 
         this.columnHeader13.Text = "index";
         this.columnHeader13.Width = 46;
         // 
         // columnHeader14
         // 
         this.columnHeader14.Text = "X";
         this.columnHeader14.Width = 68;
         // 
         // columnHeader15
         // 
         this.columnHeader15.Text = "Y";
         this.columnHeader15.Width = 68;
         // 
         // labUnreachable
         // 
         this.labUnreachable.AutoSize = true;
         this.labUnreachable.Location = new System.Drawing.Point(857, 240);
         this.labUnreachable.Name = "labUnreachable";
         this.labUnreachable.Size = new System.Drawing.Size(125, 13);
         this.labUnreachable.TabIndex = 38;
         this.labUnreachable.Text = "Bad points (unreachable)";
         // 
         // lvUnreachable
         // 
         this.lvUnreachable.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader16,
            this.columnHeader17,
            this.columnHeader18});
         this.lvUnreachable.Location = new System.Drawing.Point(860, 256);
         this.lvUnreachable.Name = "lvUnreachable";
         this.lvUnreachable.Size = new System.Drawing.Size(211, 194);
         this.lvUnreachable.TabIndex = 37;
         this.lvUnreachable.UseCompatibleStateImageBehavior = false;
         this.lvUnreachable.View = System.Windows.Forms.View.Details;
         // 
         // columnHeader16
         // 
         this.columnHeader16.Text = "index";
         this.columnHeader16.Width = 46;
         // 
         // columnHeader17
         // 
         this.columnHeader17.Text = "X";
         this.columnHeader17.Width = 68;
         // 
         // columnHeader18
         // 
         this.columnHeader18.Text = "Y";
         this.columnHeader18.Width = 68;
         // 
         // label13
         // 
         this.label13.AutoSize = true;
         this.label13.Location = new System.Drawing.Point(32, 212);
         this.label13.Name = "label13";
         this.label13.Size = new System.Drawing.Size(65, 13);
         this.label13.TabIndex = 39;
         this.label13.Text = "Baseline Adj";
         // 
         // label14
         // 
         this.label14.AutoSize = true;
         this.label14.Location = new System.Drawing.Point(200, 212);
         this.label14.Name = "label14";
         this.label14.Size = new System.Drawing.Size(43, 13);
         this.label14.TabIndex = 40;
         this.label14.Text = "Arm Adj";
         // 
         // tbBaselineAdj
         // 
         this.tbBaselineAdj.Location = new System.Drawing.Point(104, 212);
         this.tbBaselineAdj.Name = "tbBaselineAdj";
         this.tbBaselineAdj.Size = new System.Drawing.Size(90, 20);
         this.tbBaselineAdj.TabIndex = 41;
         this.tbBaselineAdj.Text = "0";
         // 
         // tbArmAdj
         // 
         this.tbArmAdj.Location = new System.Drawing.Point(249, 212);
         this.tbArmAdj.Name = "tbArmAdj";
         this.tbArmAdj.Size = new System.Drawing.Size(90, 20);
         this.tbArmAdj.TabIndex = 42;
         this.tbArmAdj.Text = "0";
         // 
         // Form1
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(1083, 501);
         this.Controls.Add(this.tbArmAdj);
         this.Controls.Add(this.tbBaselineAdj);
         this.Controls.Add(this.label14);
         this.Controls.Add(this.label13);
         this.Controls.Add(this.labUnreachable);
         this.Controls.Add(this.lvUnreachable);
         this.Controls.Add(this.labTooFar);
         this.Controls.Add(this.lvTooFar);
         this.Controls.Add(this.labBlobs);
         this.Controls.Add(this.labAngles);
         this.Controls.Add(this.labScaledPoints);
         this.Controls.Add(this.labRawPoints);
         this.Controls.Add(this.lvBlobs);
         this.Controls.Add(this.lvAngles);
         this.Controls.Add(this.lvScaledPoints);
         this.Controls.Add(this.lvRawPoints);
         this.Controls.Add(this.tbSpew);
         this.Controls.Add(this.btnGo);
         this.Controls.Add(this.cbInvertWave);
         this.Controls.Add(this.btnBrowseWav);
         this.Controls.Add(this.tbWavFile);
         this.Controls.Add(this.label12);
         this.Controls.Add(this.tbms180);
         this.Controls.Add(this.label8);
         this.Controls.Add(this.tbms0);
         this.Controls.Add(this.label11);
         this.Controls.Add(this.tbArm2Length);
         this.Controls.Add(this.label9);
         this.Controls.Add(this.tbArm1Length);
         this.Controls.Add(this.label10);
         this.Controls.Add(this.label5);
         this.Controls.Add(this.tbHubY);
         this.Controls.Add(this.label6);
         this.Controls.Add(this.tbHubX);
         this.Controls.Add(this.label7);
         this.Controls.Add(this.label4);
         this.Controls.Add(this.tbPageHeight);
         this.Controls.Add(this.label3);
         this.Controls.Add(this.tbPageWidth);
         this.Controls.Add(this.label2);
         this.Controls.Add(this.btnBrowseSVG);
         this.Controls.Add(this.tbsvgFile);
         this.Controls.Add(this.label1);
         this.Name = "Form1";
         this.Text = "DrawBot WAVE synthesizer";
         this.Load += new System.EventHandler(this.Form1_Load);
         this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox tbsvgFile;
      private System.Windows.Forms.Button btnBrowseSVG;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.TextBox tbPageWidth;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.TextBox tbPageHeight;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.TextBox tbHubY;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.TextBox tbHubX;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.TextBox tbArm2Length;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.TextBox tbArm1Length;
      private System.Windows.Forms.Label label10;
      private System.Windows.Forms.TextBox tbms180;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.TextBox tbms0;
      private System.Windows.Forms.Label label11;
      private System.Windows.Forms.Button btnBrowseWav;
      private System.Windows.Forms.TextBox tbWavFile;
      private System.Windows.Forms.Label label12;
      private System.Windows.Forms.CheckBox cbInvertWave;
      private System.Windows.Forms.Button btnGo;
      private System.Windows.Forms.TextBox tbSpew;
      private System.Windows.Forms.ListView lvRawPoints;
      private System.Windows.Forms.ColumnHeader columnHeader1;
      private System.Windows.Forms.ColumnHeader columnHeader2;
      private System.Windows.Forms.ColumnHeader columnHeader3;
      private System.Windows.Forms.ListView lvScaledPoints;
      private System.Windows.Forms.ColumnHeader columnHeader4;
      private System.Windows.Forms.ColumnHeader columnHeader5;
      private System.Windows.Forms.ColumnHeader columnHeader6;
      private System.Windows.Forms.ListView lvAngles;
      private System.Windows.Forms.ColumnHeader columnHeader7;
      private System.Windows.Forms.ColumnHeader columnHeader8;
      private System.Windows.Forms.ColumnHeader columnHeader9;
      private System.Windows.Forms.ListView lvBlobs;
      private System.Windows.Forms.ColumnHeader columnHeader10;
      private System.Windows.Forms.ColumnHeader columnHeader11;
      private System.Windows.Forms.ColumnHeader columnHeader12;
      private System.Windows.Forms.Label labRawPoints;
      private System.Windows.Forms.Label labScaledPoints;
      private System.Windows.Forms.Label labAngles;
      private System.Windows.Forms.Label labBlobs;
      private System.Windows.Forms.Label labTooFar;
      private System.Windows.Forms.ListView lvTooFar;
      private System.Windows.Forms.ColumnHeader columnHeader13;
      private System.Windows.Forms.ColumnHeader columnHeader14;
      private System.Windows.Forms.ColumnHeader columnHeader15;
      private System.Windows.Forms.Label labUnreachable;
      private System.Windows.Forms.ListView lvUnreachable;
      private System.Windows.Forms.ColumnHeader columnHeader16;
      private System.Windows.Forms.ColumnHeader columnHeader17;
      private System.Windows.Forms.ColumnHeader columnHeader18;
      private System.Windows.Forms.Label label13;
      private System.Windows.Forms.Label label14;
      private System.Windows.Forms.TextBox tbBaselineAdj;
      private System.Windows.Forms.TextBox tbArmAdj;
   }
}

