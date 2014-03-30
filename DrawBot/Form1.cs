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
      const UInt32 blobsPerSecond = 50;
      const UInt32 blobLengthMillis = 1000 / blobsPerSecond;
      const UInt32 samplesPerSecond = 192000;
      const UInt32 numChannels = 2;
      const UInt32 bitsPerSample = 8;
      const double baselineAngleAdjustment = Math.PI / 4 - 0.0855211333; // +0.0907571211 RS -0.0855211333 P
      const double armAngleAdjustment = 0.0244346095; // +0.0314159265 RS +0.0244346095 P
      const float margin = 0.20F; // 15% margin
      const int firstPointTimeMs = 10000;
      const int anchorPointTimeMs = 50;
      const int intermediatePointTimeMs = 100; // 100;

      private class angleTimeTriplet
      {
         public float arm1angle, arm2angle;
         public int milliseconds;
      }

      private class blob
      {
         public UInt32[] pulseLengthMicros = new UInt32[2]; // left and right pulse length in microseconds
      }

      private float radiansToDegrees(float r)
      {
         return (float)(180 * r / Math.PI);
      }

      // Calculate the angle of arm 1 relative to its baseline and arm 2 relative to arm 1
      enum Encodings {OK, OUT_OF_RANGE, TOO_FAR};
      private List<PointF> badTooFar = new List<PointF>();
      private List<PointF> badUnreachable = new List<PointF>();

      private Encodings encodePoint(PointF pt, PointF hubLocation, float arm1Length, float arm2length, out angleTimeTriplet triple)
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
            return Encodings.TOO_FAR;
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
         triple.milliseconds = intermediatePointTimeMs;

         if (triple.arm1angle < 0.0F || triple.arm1angle > Math.PI ||
             triple.arm2angle < 0.0F || triple.arm2angle > Math.PI)
         {
            Console.WriteLine("Angle out of range: arm1=" + radiansToDegrees((float)triple.arm1angle) + " arm2=" + radiansToDegrees((float)triple.arm2angle));
            badUnreachable.Add(pt);
            return Encodings.OUT_OF_RANGE;
         }

#if false
         Console.WriteLine("Pt=" + pt.X + "," + pt.Y + " DX=" + (float)dx + " DY=" + (float)dy + " VectLen=" + (float)vectorLength + " Arm1-to-vect=" + radiansToDegrees((float)angleArm1ToVector) +
            " Base-to-Vect=" + radiansToDegrees((float)angleBaselineVector) + " arm1=" + radiansToDegrees((float)triple.arm1angle) + " arm2=" + radiansToDegrees((float)triple.arm2angle));
#endif
         return Encodings.OK;
      }

      private void encodeLine(PointF pt1, PointF pt2, PointF hubLocation, float arm1Length, float arm2Length, ref List<angleTimeTriplet> angles)
      {
         // Calculate the two endpoints
         angleTimeTriplet tripleStart;
         angleTimeTriplet tripleEnd;
         if (encodePoint(pt1, hubLocation, arm1Length, arm2Length, out tripleStart) == Encodings.TOO_FAR ||
            encodePoint(pt2, hubLocation, arm1Length, arm2Length, out tripleEnd) == Encodings.TOO_FAR)
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
            if (Encodings.OK == encodePoint(ptInterpol, hubLocation, arm1Length, arm2Length, out newTriple))
            {
               newTriple.milliseconds = i == steps ? anchorPointTimeMs : intermediatePointTimeMs;
               angles.Add(newTriple);
            }
         }
      }

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

#if true // This code calculates the bounding rectangle of the actual image
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
#endif
         return true;
      }

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
            PointF scaledPt = new PointF(x, y);
            scaledPoints.Add(scaledPt);
         }
      }

      void pointsToAngles(List<PointF> scaledPoints, float arm1Length, float arm2Length, PointF hubLocation, out List<angleTimeTriplet> triples)
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
         if (encodePoint(scaledPoints[0], hubLocation, arm1Length, arm2Length, out triple) == Encodings.OK)
         {
            // The very first position the arms go to will allow a 10-second pause for inserting the pen
            triple.milliseconds = firstPointTimeMs;
            triples.Add(triple);
         }

         // Then, generate lines between all the other point pairs and add them to the list
         for (int i = 0; i < scaledPoints.Count - 1; ++i)
            encodeLine(scaledPoints[i], scaledPoints[i + 1], hubLocation, arm1Length, arm2Length, ref triples);
#endif
      }

      private void anglesToBlobs(List<angleTimeTriplet> triples, int angle0Microseconds, int angle180Microseconds, out List<blob> blobList)
      {
         blobList = new List<blob>();
         foreach (angleTimeTriplet t in triples)
         {
            for (int i = t.milliseconds; i > 0; i -= (int)blobLengthMillis)
            {
               blob b = new blob();
               b.pulseLengthMicros[0] = (UInt32)(angle0Microseconds + (t.arm1angle / Math.PI) * (angle180Microseconds - angle0Microseconds));
               b.pulseLengthMicros[1] = (UInt32)(angle0Microseconds + (t.arm2angle / Math.PI) * (angle180Microseconds - angle0Microseconds));
               blobList.Add(b);
            }
         }
      }

      private bool blobsToWave(List<blob> blobList, string waveFileName, bool invertedWave, ref string errMessage)
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
         foreach (blob b in blobList)
         {
            UInt32 highLeftSamples = b.pulseLengthMicros[0] * samplesPerMillisecond / 1000;
            UInt32 highRightSamples = b.pulseLengthMicros[1] * samplesPerMillisecond / 1000;
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

      private bool SVGtoWAV(string inputSVG, RectangleF pageSize, PointF hubLocation, float arm1Length, float arm2Length, int angle0Microseconds, int angle180Microseconds, string waveFileName, bool invertedWave, ref string err, ref string diags)
      {
         // Calculated and loaded from image
         RectangleF imageSize;
         List<PointF> rawPoints;
         List<PointF> scaledPoints;
         List<angleTimeTriplet> triples;
         List<blob> blobs;

         if (!loadSVG(inputSVG, out imageSize, out rawPoints, ref err))
            return false;
         scalePoints(imageSize, pageSize, rawPoints, out scaledPoints);
         pointsToAngles(scaledPoints, arm1Length, arm2Length, hubLocation, out triples);
         anglesToBlobs(triples, angle0Microseconds, angle180Microseconds, out blobs);
         if (!blobsToWave(blobs, waveFileName, invertedWave, ref err))
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
               radiansToDegrees(triples[i].arm2angle).ToString("F4") + ", " + triples[i].milliseconds + "ms" + Environment.NewLine;
            ListViewItem item = lvAngles.Items.Add(i.ToString());
            item.SubItems.Add(radiansToDegrees(triples[i].arm1angle).ToString("F4"));
            item.SubItems.Add(radiansToDegrees(triples[i].arm2angle).ToString("F4"));
         }

         diags += "There are " + blobs.Count + " blobs:" + Environment.NewLine;
         for (int i=0; i<blobs.Count; ++i)
         {
            diags += "   " + blobs[i].pulseLengthMicros[0] + ", " + blobs[i].pulseLengthMicros[1] + Environment.NewLine;
            ListViewItem item = lvBlobs.Items.Add(i.ToString());
            item.SubItems.Add(blobs[i].pulseLengthMicros[0].ToString());
            item.SubItems.Add(blobs[i].pulseLengthMicros[1].ToString());
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

#if false
         // Inputs
         string inputSVG = @"C:\Users\mnhart\Google Drive\Projects\DrawBot\svg\01 - circle.svg";
         RectangleF pageSize = new RectangleF(0F, 0F, 11F, 8.5F);
         PointF hubLocation = new PointF(5.5F, -2F);
         float arm1Length = 6.2F;
         float arm2Length = 6.5F;
         int angle0Microseconds = 600;
         int angle180Microseconds = 2400;
         string waveFileName = "Happiness.sundial.wav";
         bool invertedWave = false;
         string err = "";

         if (!SVGtoWAV(inputSVG, pageSize, hubLocation, arm1Length, arm2Length, angle0Microseconds, angle180Microseconds, waveFileName, invertedWave, ref err))
            MessageBox.Show("Error: " + err);
#endif
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

#if false
      // Convert a point into a pair of angles plus minimum time to linger there
      angleTimeTriplet pointToAngles(PointF p, PointF hub, int armLength)
      {
         angleTimeTriplet triple = new angleTimeTriplet();

         double dx = p.X - hub.X;
         double dy = p.Y - hub.Y;
         double vector = Math.Sqrt(dx * dx + dy * dy);
         double theta = dx == 0 ? Math.PI / 2 : Math.Atan(dy / (double)dx);
         if (theta < 0)
            theta += Math.PI;
         double theta1 = Math.Acos(vector / (2 * armLength));
         triple.arm1angle = theta + theta1;
         triple.arm2angle = 2 * theta1;
         triple.milliseconds = 20;

         return triple;
      }

      // Take a line defined by two endpoints and transform it into a series of angle+time triples
      // such that no consecutive points differ by more than 1 degree
      void EncodeLine(PointF pt1, PointF pt2, ref List<angleTimeTriplet> triples, PointF hub, int armLength)
      {
         angleTimeTriplet trip1 = pointToAngles(pt1, hub, armLength);
         angleTimeTriplet trip2 = pointToAngles(pt2, hub, armLength);

         if (Math.Abs(trip1.arm1angle - trip2.arm1angle) > Math.PI / 180.0 || Math.Abs(trip1.arm2angle - trip2.arm2angle) > Math.PI / 180.0) // 1 degree max
         {
            PointF mid = new PointF((pt2.X + pt1.X) / 2, (pt2.Y + pt1.Y) / 2);
            EncodeLine(pt1, mid, ref triples, hub, armLength);
            EncodeLine(mid, pt2, ref triples, hub, armLength);
         }
         else
         {
            triples.Add(trip2);
         }
      }
#endif


      private void Form1_MouseUp(object sender, MouseEventArgs e)
      {
#if false
         // We assume the page's lower left corner is (0, 0)
         // The size of the page is given just as a sanity check 
         // to make sure no line goes outside that boundary
         RectangleF pageSize = new RectangleF(0.0F, 0.0F, 8.5F, 11F);
         PointF hubLocation = new PointF(pageSize.Width / 2, -200);
         int armLength = 700;
         int angle0Microseconds = 750;
         int angle180Microseconds = 2250;
         int msPerDegreeSpeed = 3;
         bool inverted = false;

         // Step 0: Given the constraints, create an initial list of points
         List<Point> inputPoints = new List<Point>();
#if false // 5 points on side
         for (int i=1; i<5; ++i)
            inputPoints.Add(new Point(700, 100*i));
#elif true // circle
         int radius = 100;
         for (double t=0; t<2*Math.PI; t += 0.01)
            inputPoints.Add(new Point((int)(pageSize.Width / 2 + 200 + radius * Math.Cos(t)), (int)(pageSize.Height / 2 + radius * Math.Sin(t))));
#else // big diagonal
         inputPoints.Add(new Point(pageSize.Width - 1, 0));
         inputPoints.Add(new Point(0, pageSize.Height - 1));
#endif
         Console.WriteLine("There are " + inputPoints.Count + " points in the list.");

         // Step 1: Given a list of points, create a list of angle-time triplets
         List<angleTimeTriplet> triples = new List<angleTimeTriplet>();
         
         // First of all, an initial point that lasts 10 seconds
         angleTimeTriplet triple = pointToAngles(inputPoints[0], hubLocation, armLength);
         triple.milliseconds = 10000;
         triples.Add(triple);

         // Draw the point list and calculate triples, extrapolating if necessary
         Graphics g = this.CreateGraphics();
         Pen red = new Pen(Color.FromArgb(255, 255, 0, 0));
         Pen black = new Pen(Color.FromArgb(255, 0, 0, 0));
         for (int i = 0; i < inputPoints.Count - 1; ++i)
         {
            EncodeLine(inputPoints[i], inputPoints[i + 1], ref triples, hubLocation, armLength);
            g.DrawLine(red, inputPoints[i].X, pageSize.Height - inputPoints[i].Y, inputPoints[i + 1].X, pageSize.Height - inputPoints[i + 1].Y);
         }

         // Lastly, make the last point last 10 seconds also
         triple = pointToAngles(inputPoints.Last(), hubLocation, armLength);
         triple.milliseconds = 10000;
         triples.Add(triple);

         Console.WriteLine("There are " + triples.Count + " triples in the list.");

         // Step 2: Draw all the angle-time triplets with proper timing
         foreach (angleTimeTriplet t in triples)
         {
            // Draw a line at length L and angle AP from 0,0
            double arm1endX = armLength * Math.Cos(t.arm1angle) + hubLocation.X;
            double arm1endY = armLength * Math.Sin(t.arm1angle) + hubLocation.Y;
            g.DrawLine(black, hubLocation.X, pageSize.Height - hubLocation.Y, (int)arm1endX, pageSize.Height - (int)arm1endY);

            // Draw a line at length L and angle -A from newx, newy
            double arm2endX = arm1endX + armLength * Math.Cos(t.arm1angle - t.arm2angle);
            double arm2endY = arm1endY + armLength * Math.Sin(t.arm1angle - t.arm2angle);
            g.DrawLine(black, (int)arm1endX, pageSize.Height - (int)arm1endY, (int)arm2endX, pageSize.Height - (int)arm2endY);

            System.Threading.Thread.Sleep(t.milliseconds);
         }

         // Step 3: Given a list of angle-time triplets, create a list of 192Khz "blobs".
         // A "blob" is a 50ms chunk of data
         List<blob> bloblist = new List<blob>();
         UInt32 samplesPerSecond = 192000;
         UInt32 blobsPerSecond = 50;
         UInt32 blobLengthMs = 1000 / blobsPerSecond;
         foreach (angleTimeTriplet t in triples)
         {
            for (int i = t.milliseconds; i > 0; i -= (int)blobLengthMs)
            {
               blob b = new blob();
               b.pulseLengthMicros[0] = (UInt32)(angle0Microseconds + (t.arm1angle / Math.PI) * (angle180Microseconds - angle0Microseconds));
               b.pulseLengthMicros[1] = (UInt32)(angle0Microseconds + (t.arm2angle / Math.PI) * (angle180Microseconds - angle0Microseconds));
               Console.WriteLine(b.pulseLengthMicros[0] + " " + b.pulseLengthMicros[1]);
               bloblist.Add(b);
            }
         }
         Console.WriteLine("There are " + bloblist.Count + " blobs in the list.");
         Console.WriteLine("And the audio file should last " + bloblist.Count / blobsPerSecond + " seconds.");

         // Step 4: translate the blob list into a 192KHz .WAV file
         FileStream fs = new FileStream("circle.wav", FileMode.Create);
         UInt32 numChannels = 2;
         UInt32 bitsPerSample = 8;
         UInt32 byteRate = samplesPerSecond * numChannels * bitsPerSample / 8;
         UInt32 blockAlign = numChannels * bitsPerSample / 8;
         UInt32 samplesPerMillisecond = samplesPerSecond / 1000;
         UInt32 dataChunkSize = (UInt32)bloblist.Count * blobLengthMs * samplesPerMillisecond * numChannels * bitsPerSample / 8;
         UInt32 samplesPerBlob = samplesPerSecond / blobsPerSecond;

         // Create the writer for data.
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
         foreach(blob b in bloblist)
         {
            UInt32 highLeftSamples = b.pulseLengthMicros[0] * samplesPerMillisecond / 1000;
            UInt32 highRightSamples = b.pulseLengthMicros[1] * samplesPerMillisecond / 1000;
            for (UInt32 i=0; i<samplesPerBlob; ++i)
            {
               byte hi = inverted ? byte.MinValue : byte.MaxValue;
               byte lo = inverted ? byte.MaxValue : byte.MinValue;
               w.Write(i < highLeftSamples ? hi : lo);
               w.Write(i < highRightSamples ? hi : lo);
            }
         }

         w.Close();
         fs.Close();
#endif
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

      private void tbms0_TextChanged(object sender, EventArgs e)
      {

      }
   }
}
