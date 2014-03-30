// #define CALIBRATION_ONLY

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;
using System.Web;
using System.Xml.Linq;

namespace DrawBot
{
   public partial class Form1 : Form
   {
      // Various constants
      const UInt32 blobsPerSecond = 50;
      const UInt32 blobLengthMillis = 1000 / blobsPerSecond;
      const UInt32 samplesPerSecond = 192000;
      const UInt32 numChannels = 2;
      const UInt32 bitsPerSample = 8;
      const float margin = 0.20F; // 20% margin

      // How long the arm should "linger" at each point
      const int firstPointTimeMs = 10000;
      const int anchorPointTimeMs = 100;
      const int intermediatePointTimeMs = 100;
      const int lastPointTimeMs = 10000;

      // Adjustments
      double baselineAngleAdjustment = 0.0; // Math.PI / 4 - 0.0855211333; // +0.0907571211 RS -0.0855211333 P
      double armAngleAdjustment = 0.0; // 0.0244346095; // +0.0314159265 RS +0.0244346095 P

      private class angleTimeTriplet
      {
         public float arm1angle, arm2angle;
         public int linger;
      }

      private class WAVblob
      {
         public UInt32 left, right; // left and right pulse length in microseconds
      }

      private float radiansToDegrees(float r)
      {
         return (float)(180 * r / Math.PI);
      }

      // Calculate the angle of arm 1 relative to its baseline and arm 2 relative to arm 1
      enum EncodingEnum {OK, OUT_OF_RANGE, TOO_FAR};
      private List<PointF> badTooFar = new List<PointF>();
      private List<PointF> badUnreachable = new List<PointF>();

      // Convert a point into an angle-angle-lingertime triple, given hublocation and arm lengths
      private EncodingEnum encodePoint(PointF pt, PointF hubLocation, float arm1Length, float arm2length, out angleTimeTriplet triple)
      {
         triple = new angleTimeTriplet();

         // First find the distance from the hub to the point
         double dx = pt.X - hubLocation.X;
         double dy = pt.Y - hubLocation.Y;
         double vectorLength = Math.Sqrt(dx * dx + dy * dy);

         // Since we now know the three sides of the triangle formed by the two arms and the vector,
         // calculate the angle between the vector and the first arm using law of cosines
         double angleArm1ToVector = Math.Acos((vectorLength * vectorLength + arm1Length * arm1Length - arm2length * arm2length) / (2 * vectorLength * arm1Length));
         
         // If the point is too far away to be reached by the fully-extended arms, the value passed to acos will be greater than 1 (< -1)
         if (double.IsNaN(angleArm1ToVector))
         {
            Console.WriteLine("Point too far: " + pt.X + "," + pt.Y);
            badTooFar.Add(pt);
            return EncodingEnum.TOO_FAR;
         }

         // Then determine the angle between the baseline and the vector
         double angleBaselineVector = dx == 0 ? Math.PI / 2 : Math.Atan(dy / (double)dx);
         if (angleBaselineVector < 0)
            angleBaselineVector += Math.PI;

         angleBaselineVector += baselineAngleAdjustment;

         // Now the angle between the arm and the baseline is simply the difference between these two angles
         triple.arm1angle = (float)(angleBaselineVector - angleArm1ToVector); 
         triple.arm2angle = (float)Math.Acos((arm1Length * arm1Length + arm2length * arm2length - vectorLength * vectorLength) / (2 * arm2length * arm1Length));
         triple.arm2angle += (float)armAngleAdjustment;
         triple.linger = intermediatePointTimeMs;

         // If the resulting angle would require the servo to move less than 0 degrees or more than 180, well, then that's unreachable too.
         if (triple.arm1angle < 0.0F || triple.arm1angle > Math.PI ||
             triple.arm2angle < 0.0F || triple.arm2angle > Math.PI)
         {
            Console.WriteLine("Angle out of range: arm1=" + radiansToDegrees((float)triple.arm1angle) + " arm2=" + radiansToDegrees((float)triple.arm2angle));
            badUnreachable.Add(pt);
            return EncodingEnum.OUT_OF_RANGE;
         }

#if false
         Console.WriteLine("Pt=" + pt.X + "," + pt.Y + " DX=" + (float)dx + " DY=" + (float)dy + " VectLen=" + (float)vectorLength + " Arm1-to-vect=" + radiansToDegrees((float)angleArm1ToVector) +
            " Base-to-Vect=" + radiansToDegrees((float)angleBaselineVector) + " arm1=" + radiansToDegrees((float)triple.arm1angle) + " arm2=" + radiansToDegrees((float)triple.arm2angle));
#endif
         return EncodingEnum.OK;
      }

      // Convert a line into a series of angle-angle-lingertime triples, interpolating new points if necessary
      private void encodeLine(PointF pt1, PointF pt2, PointF hubLocation, float arm1Length, float arm2Length, ref List<angleTimeTriplet> triples)
      {
         // Calculate the two endpoints
         angleTimeTriplet tripleStart;
         angleTimeTriplet tripleEnd;
         if (encodePoint(pt1, hubLocation, arm1Length, arm2Length, out tripleStart) == EncodingEnum.TOO_FAR ||
            encodePoint(pt2, hubLocation, arm1Length, arm2Length, out tripleEnd) == EncodingEnum.TOO_FAR)
         {
            Console.WriteLine("Cannot encode line.");
            return;
         }

         // If they are far apart, i.e. the arm angles are more than one degree different,
         // then interpolate additional points to ensure a straight line
         float d1 = radiansToDegrees(Math.Abs(tripleStart.arm1angle - tripleEnd.arm1angle));
         float d2 = radiansToDegrees(Math.Abs(tripleStart.arm2angle - tripleEnd.arm2angle));
         int steps = (int)Math.Max(d1, d2) + 1;
         for (int i = 1; i <= steps; ++i)
         {
            PointF ptInterpol = new PointF(pt1.X + i * (pt2.X - pt1.X) / steps, pt1.Y + i * (pt2.Y - pt1.Y) / steps);
            angleTimeTriplet newTriple;
            if (EncodingEnum.OK == encodePoint(ptInterpol, hubLocation, arm1Length, arm2Length, out newTriple))
            {
               newTriple.linger = i == steps ? anchorPointTimeMs : intermediatePointTimeMs;
               triples.Add(newTriple);
            }
         }
      }

      // Load an SVG and create a series of points.  (Note: requires special limited SVG)
      // ...and discover the dimension rectangle for the image
      private static bool loadSVG(string svgFile, out RectangleF imageDimensions, out List<PointF> points, ref string errorMessage)
      {
         XDocument xml;
         points = new List<PointF>();
         imageDimensions = new RectangleF();

         try
         {
            xml = XDocument.Load(svgFile);
         }
         catch (Exception e)
         {
            errorMessage = "Error opening file " + svgFile + ": " + e.Message;
            return false;
         }

         try
         {
            XAttribute width = xml.Root.Attribute("width");
            XAttribute height = xml.Root.Attribute("height");
            imageDimensions = new RectangleF(0.0F, 0.0F, float.Parse(width.Value), float.Parse(height.Value));
            XElement g1 = xml.Root.Element("{http://www.w3.org/2000/svg}g");
            XElement g2 = g1.Element("{http://www.w3.org/2000/svg}g");
            XElement path = g2.Element("{http://www.w3.org/2000/svg}path");
            string pointString = path.Attribute("d").Value;

            // At this point "pointString" should be a string beginning with "M" and containing a series of points in X Y form.
            if (pointString[0] != 'M')
               throw new Exception("Protocol error parsing SVG");

            pointString = pointString.Replace("  ", " ");

            char[] delimiterChars = { ' ',',' };
            string[] splits = pointString.Split(delimiterChars);

            for (int i = 1; i < splits.Count(); i += 2)
            {
               float x, y;
               if (float.TryParse(splits[i], out x) && float.TryParse(splits[i+1], out y))
                  points.Add(new PointF(x, y));
            }
         }
         catch (Exception e)
         {
            errorMessage = "Error parsing XML: " + e.Message;
            return false;
         }

         // This code calculates the bounding rectangle of the actual image
         float bottom = float.MaxValue, left = float.MaxValue, rwidth = 0F, rheight = 0F;
         foreach (PointF pt in points)
         {
            if (pt.X < left) left = pt.X;
            if (pt.Y < bottom) bottom = pt.Y;
         }
         foreach (PointF pt in points)
         {
            if (rwidth < pt.X - left) rwidth = pt.X - left;
            if (rheight < pt.Y - bottom) rheight = pt.Y - bottom;
         }

         // Shrink it a bit so that it has a bit of margin
         imageDimensions = new RectangleF(left - margin * rwidth, bottom - margin * rheight, (1.0F + 2 * margin) * rwidth, (1.0F + 2 * margin) * rheight);
         return true;
      }

      // Scale the image to the page
      private void scalePoints(RectangleF imageSize, RectangleF pageSize, List<PointF> rawPoints, out List<PointF> scaledPoints)
      {
         scaledPoints = new List<PointF>();
         float scaleFactor = Math.Min(pageSize.Width / imageSize.Width, pageSize.Height / imageSize.Height);
         PointF imageCenter = new PointF(imageSize.Left + imageSize.Width / 2, imageSize.Top + imageSize.Height / 2);
         PointF pageCenter = new PointF(pageSize.Left + pageSize.Width / 2, pageSize.Top + pageSize.Height / 2);
         foreach (PointF rawPt in rawPoints)
         {
            float x = (rawPt.X - imageCenter.X) * scaleFactor + pageCenter.X;
            float y = (rawPt.Y - imageCenter.Y) * scaleFactor + pageCenter.Y;
            // Per Sean 3/24: rotate and mirror image left/right
            y = pageSize.Height - y;
            PointF scaledPt = new PointF(x, y);
            scaledPoints.Add(scaledPt);
         }
      }

      // Convert a list of x/y points to a list of angle-angle-lingertime triples
      void pointsToTriples(List<PointF> scaledPoints, float arm1Length, float arm2Length, PointF hubLocation, out List<angleTimeTriplet> triples)
      {
         triples = new List<angleTimeTriplet>();

#if CALIBRATION_ONLY
         angleTimeTriplet att = new angleTimeTriplet();
         att.arm1angle = att.arm2angle = (float)(Math.PI / 2); // 90 degree
         att.milliseconds = 10000;
         triples.Add(att);
         return;
#else
         // Add the first point to the list
         angleTimeTriplet triple;
         if (encodePoint(scaledPoints[0], hubLocation, arm1Length, arm2Length, out triple) == EncodingEnum.OK)
         {
            // The very first position the arms go to will allow a 10-second pause for inserting the pen
            triple.linger = firstPointTimeMs;
            triples.Add(triple);
         }

         // Then, generate lines between all the other point pairs and add them to the list
         for (int i = 0; i < scaledPoints.Count - 1; ++i)
            encodeLine(scaledPoints[i], scaledPoints[i + 1], hubLocation, arm1Length, arm2Length, ref triples);
#endif

         // The last point should remain a long time also--to allow pen removal
         angleTimeTriplet last = triples.Last();
         last.linger = lastPointTimeMs;
         triples.Add(last);
      }

      // Convert list of angle-angle-lingertime triples to series of WAV file blobs
      private void triplesToBlobs(List<angleTimeTriplet> triples, int angle0Microseconds, int angle180Microseconds, out List<WAVblob> blobList)
      {
         blobList = new List<WAVblob>();
         foreach (angleTimeTriplet t in triples)
         {
            for (int i = t.linger; i > 0; i -= (int)blobLengthMillis)
            {
               WAVblob b = new WAVblob();
               b.left = (UInt32)(angle0Microseconds + (t.arm1angle / Math.PI) * (angle180Microseconds - angle0Microseconds));
               b.right = (UInt32)(angle0Microseconds + (t.arm2angle / Math.PI) * (angle180Microseconds - angle0Microseconds));
               blobList.Add(b);
            }
         }
      }

      // Convert a list of WAV file blobs to a WAV file.
      private bool blobsToWaveFile(List<WAVblob> blobList, string waveFileName, bool invertedWave, ref string errMessage)
      {
         UInt32 byteRate = samplesPerSecond * numChannels * bitsPerSample / 8;
         UInt32 blockAlign = numChannels * bitsPerSample / 8;
         UInt32 samplesPerMillisecond = samplesPerSecond / 1000;
         UInt32 dataChunkSize = (UInt32)blobList.Count * blobLengthMillis * samplesPerMillisecond * numChannels * bitsPerSample / 8;
         UInt32 samplesPerBlob = samplesPerSecond / blobsPerSecond;

         // Create the writer for data.
         FileStream fs = new FileStream(waveFileName, FileMode.Create);
         BinaryWriter w = new BinaryWriter(fs);

         // Write RIFF header
         w.Write('R'); w.Write('I'); w.Write('F'); w.Write('F');
         w.Write((UInt32)(36 + dataChunkSize));
         w.Write('W'); w.Write('A'); w.Write('V'); w.Write('E');

         // And the WAVE header
         w.Write('f'); w.Write('m'); w.Write('t'); w.Write(' ');
         w.Write((UInt32)16); // WAVE header size (remaining)
         w.Write((UInt16)1);  // PCM = 1
         w.Write((UInt16)numChannels);
         w.Write((UInt32)samplesPerSecond);
         w.Write((UInt32)byteRate);
         w.Write((UInt16)blockAlign);
         w.Write((UInt16)bitsPerSample);

         // The Data header
         w.Write('d'); w.Write('a'); w.Write('t'); w.Write('a');
         w.Write((UInt32)dataChunkSize);

         // And finally... the data
         // in the form CH0SAM0, CH1SAM0, CH0SAM1, CH1SAM1, etc.
         foreach (WAVblob b in blobList)
         {
            UInt32 highLeftSamples = b.left * samplesPerMillisecond / 1000;
            UInt32 highRightSamples = b.right * samplesPerMillisecond / 1000;
            for (UInt32 i = 0; i < samplesPerBlob; ++i)
            {
               byte hi = invertedWave ? byte.MinValue : byte.MaxValue;
               byte lo = invertedWave ? byte.MaxValue : byte.MinValue;
               w.Write(i < highLeftSamples ? hi : lo);
               w.Write(i < highRightSamples ? hi : lo);
            }
         }

         w.Close();
         fs.Close();
         return true;
      }

      // Root level function: convert a specialize SVG file into a WAV file for a Sean Ragan DrawBot
      private bool SVGtoWAV(string inputSVG, RectangleF pageSize, PointF hubLocation, float arm1Length, float arm2Length, int angle0Microseconds, int angle180Microseconds, string waveFileName, bool invertedWave, ref string err, ref string diags)
      {
         // Calculated and loaded from image
         RectangleF imageSize;
         List<PointF> rawPoints;
         List<PointF> scaledPoints;
         List<angleTimeTriplet> triples;
         List<WAVblob> blobs;

         if (!loadSVG(inputSVG, out imageSize, out rawPoints, ref err))
            return false;
         scalePoints(imageSize, pageSize, rawPoints, out scaledPoints);
         pointsToTriples(scaledPoints, arm1Length, arm2Length, hubLocation, out triples);
         triplesToBlobs(triples, angle0Microseconds, angle180Microseconds, out blobs);
         if (!blobsToWaveFile(blobs, waveFileName, invertedWave, ref err))
            return false;

         diags += "The provided image has aspect " + imageSize.Width + " by " + imageSize.Height + Environment.NewLine;
         diags += "The provided paper has aspect " + pageSize.Width + " by " + pageSize.Height + Environment.NewLine;
         lvRawPoints.Items.Clear();
         lvScaledPoints.Items.Clear();
         lvAngles.Items.Clear();
         lvBlobs.Items.Clear();
         lvTooFar.Items.Clear();
         lvUnreachable.Items.Clear();

         diags += "There are " + rawPoints.Count + " raw points in the SVG:" + Environment.NewLine;
         for (int i = 0; i < rawPoints.Count; ++i)
         {
            diags += i + ".  " + rawPoints[i].X.ToString("F4") + ", " + rawPoints[i].Y.ToString("F4") + Environment.NewLine;
            ListViewItem item = lvRawPoints.Items.Add(i.ToString());
            item.SubItems.Add(rawPoints[i].X.ToString("F4"));
            item.SubItems.Add(rawPoints[i].Y.ToString("F4"));
         }

         diags += "There are " + scaledPoints.Count + " points, scaled to given paper size:" + Environment.NewLine;
         for (int i = 0; i < scaledPoints.Count; ++i)
         {
            diags += i + ".  " + scaledPoints[i].X.ToString("F4") + ", " + scaledPoints[i].Y.ToString("F4") + Environment.NewLine;
            ListViewItem item = lvScaledPoints.Items.Add(i.ToString());
            item.SubItems.Add(scaledPoints[i].X.ToString("F4"));
            item.SubItems.Add(scaledPoints[i].Y.ToString("F4"));
         }

         diags += "There are " + triples.Count + " angle triples:" + Environment.NewLine;
         for (int i = 0; i < triples.Count; ++i)
         {
            diags += i + ".  " + radiansToDegrees(triples[i].arm1angle).ToString("F4") + ", " +
               radiansToDegrees(triples[i].arm2angle).ToString("F4") + ", " + triples[i].linger + "ms" + Environment.NewLine;
            ListViewItem item = lvAngles.Items.Add(i.ToString());
            item.SubItems.Add(radiansToDegrees(triples[i].arm1angle).ToString("F4"));
            item.SubItems.Add(radiansToDegrees(triples[i].arm2angle).ToString("F4"));
         }

         diags += "There are " + blobs.Count + " blobs:" + Environment.NewLine;
         for (int i=0; i<blobs.Count; ++i)
         {
            diags += "   " + blobs[i].left + ", " + blobs[i].right + Environment.NewLine;
            ListViewItem item = lvBlobs.Items.Add(i.ToString());
            item.SubItems.Add(blobs[i].left.ToString());
            item.SubItems.Add(blobs[i].right.ToString());
         }         
         diags += "And the audio file should last about " + blobs.Count / blobsPerSecond + " seconds." + Environment.NewLine;

         diags += "There are " + badTooFar.Count + " points in the 'too far' list:" + Environment.NewLine;
         for (int i = 0; i < badTooFar.Count; ++i)
         {
            diags += "   " + badTooFar[i].X + ", " +badTooFar[i].Y + Environment.NewLine;
            ListViewItem item = lvTooFar.Items.Add(i.ToString());
            item.SubItems.Add(badTooFar[i].X.ToString());
            item.SubItems.Add(badTooFar[i].Y.ToString());
         }

         diags += "There are " + badUnreachable.Count + " points in the 'unreachable' list:" + Environment.NewLine;
         for (int i = 0; i < badUnreachable.Count; ++i)
         {
            diags += "   " + badUnreachable[i].X + ", " + badUnreachable[i].Y + Environment.NewLine;
            ListViewItem item = lvUnreachable.Items.Add(i.ToString());
            item.SubItems.Add(badUnreachable[i].X.ToString());
            item.SubItems.Add(badUnreachable[i].Y.ToString());
         }

         tbSpew.Text = diags;
         labRawPoints.Text = "Raw Points (" + rawPoints.Count + ")";
         labScaledPoints.Text = "Scaled Points (" + scaledPoints.Count + ")";
         labAngles.Text = "Angles (" + triples.Count + ")";
         labBlobs.Text = "WAV Blobs (" + blobs.Count + ")";
         labTooFar.Text = "Bad points (too far) (" + badTooFar.Count + ")";
         labUnreachable.Text = "Bad points (unreachable) (" + badUnreachable.Count + ")";
         badTooFar.Clear();
         badUnreachable.Clear();

         return true;
      }

      public Form1()
      {
         InitializeComponent();
      }

      private void Form1_Load(object sender, EventArgs e)
      {
         Graphics g = this.CreateGraphics();
         Pen pen = new Pen(Color.FromArgb(255, 0, 0, 0));
         g.DrawLine(pen, 20, 10, 300, 100);
      }

      private int X, Y;
      private void Form1_MouseDown(object sender, MouseEventArgs e)
      {
         X = e.X; Y = e.Y;
      }

      private void btnBrowseSVG_Click(object sender, EventArgs e)
      {
         OpenFileDialog dlg = new OpenFileDialog();
         dlg.Filter = "SVG file (*.svg)|*.svg|All files (*.*)|*.*";
         dlg.Title = "Open an SVG image file";
         if (dlg.ShowDialog(this) == DialogResult.OK)
            this.tbsvgFile.Text = dlg.FileName;
      }

      private void btnBrowseWav_Click(object sender, EventArgs e)
      {
         SaveFileDialog dlg = new SaveFileDialog();
         dlg.Filter = "WAV file|*.wav";
         dlg.Title = "Save a WAV File";
         if (dlg.ShowDialog(this) == DialogResult.OK)
            this.tbWavFile.Text = dlg.FileName;
      }

      private void btnGo_Click(object sender, EventArgs e)
      {
         Cursor.Current = Cursors.WaitCursor;
         tbSpew.Clear();
         try
         {
            string err = "";
            string diags = "";
            baselineAngleAdjustment = Math.PI / 4 + float.Parse(this.tbBaselineAdj.Text);
            armAngleAdjustment = float.Parse(this.tbArmAdj.Text);
            if (!SVGtoWAV(tbsvgFile.Text,
               new RectangleF(0, 0, float.Parse(tbPageWidth.Text), float.Parse(tbPageHeight.Text)),
               new PointF(float.Parse(tbHubX.Text), float.Parse(tbHubY.Text)),
               float.Parse(tbArm1Length.Text),
               float.Parse(tbArm2Length.Text),
               int.Parse(tbms0.Text),
               int.Parse(tbms180.Text),
               tbWavFile.Text,
               cbInvertWave.Checked,
               ref err,
               ref diags))
            {
               Cursor.Current = Cursors.Default;
               MessageBox.Show(this, err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
               Cursor.Current = Cursors.Default;
               MessageBox.Show(this, "File " + tbWavFile.Text + " generated!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
         }
         catch (Exception exc)
         {
            Cursor.Current = Cursors.Default;
            MessageBox.Show(this, exc.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
         Cursor.Current = Cursors.Default;
      }

      private void tbsvgFile_TextChanged(object sender, EventArgs e)
      {
         if (!tbWavFile.Modified)
         {
            if (tbsvgFile.Text.EndsWith(".svg"))
               tbWavFile.Text = tbsvgFile.Text.Substring(0, tbsvgFile.Text.Length - 4) + ".wav";
            else
               tbWavFile.Text = tbsvgFile.Text;
         }
      }
   }
}
