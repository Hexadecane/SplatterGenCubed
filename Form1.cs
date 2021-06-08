using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;

namespace SplatterGenCubed {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();
        }

        // Data that will be output upon exporting:
        public Image DecalOutputData { get; set; }


        // Options form stuff:
        OptionsForm OF = new OptionsForm();


        // Functions for the dictionary entries go here:
        public static Func<Color, int, int> MamBloodRF = (Color originalBrushColor, int colorShift) => {
            return Math.Max(originalBrushColor.R + colorShift, 0);
        };
        public static Func<Color, int, int> MamBloodGF = (Color originalBrushColor, int colorShift) => {
            return Math.Max(originalBrushColor.G + rng.Next(-8, 4), 0);
        };
        public static Func<Color, int, int> MamBloodBF = (Color originalBrushColor, int colorShift) => {
            return Math.Max(originalBrushColor.G + rng.Next(-8, 4), 0);
        };
        public static Func<Color, int, int> MamBloodAF = (Color originalBrushColor, int colorShift) => {
            return Math.Min(originalBrushColor.A + rng.Next(0, 65), 255);
        };

        public static Func<Color, int, int> ComBloodRF = (Color originalBrushColor, int colorShift) => {
            return Math.Max(originalBrushColor.R + colorShift / 1, 0);
        };
        public static Func<Color, int, int> ComBloodGF = (Color originalBrushColor, int colorShift) => {
            return Math.Max(originalBrushColor.G + colorShift / 1, 0);
        };
        public static Func<Color, int, int> ComBloodBF = (Color originalBrushColor, int colorShift) => {
            return Math.Max(originalBrushColor.B + colorShift / 1, 0);
        };
        public static Func<Color, int, int> ComBloodAF = (Color originalBrushColor, int colorShift) => {
            return Math.Min(originalBrushColor.A + rng.Next(-32, 65), 255);
        };

        // Dictionary for storing liquid presets:
        public static Dictionary<string, LiquidPreset> LiquidPresets = new Dictionary<string, LiquidPreset> {
            {
                "MammalianBlood",
                new LiquidPreset("MammalianBlood", new double[]{359, 0.85, 0.166}, 192, rf:MamBloodRF, gf:MamBloodGF, bf:MamBloodBF, af:MamBloodAF)
            },
            {
                "CombineSynthBlood",
                new LiquidPreset("CombineSynthBlood", new double[]{70, 0.05, 0.675}, 255, rf:ComBloodRF, gf:ComBloodGF, bf:ComBloodBF, af:ComBloodAF)
            },
            {
                "XenianBlood",
                new LiquidPreset("XenianBlood", new double[]{60, 0.75, 0.65}, 128)
            },
            {
                "MotorOil",
                new LiquidPreset("MotorOil", new double[]{30, 0.25, 0.05}, 255)
            },
            {
                "Water",
                new LiquidPreset("Water", new double[]{0, 0.0, 0.25}, 96)
            },
        };

        public static LiquidPreset ActivePreset = LiquidPresets["MammalianBlood"];


        // Image settings:
        public Color bgColor = Color.FromArgb(255, 127, 127, 127);
        private int imgWidth;
        private int imgHeight;
        private int simWidth;
        private int simHeight;
        private int centerX;
        private int centerY;
        private int finalRes;
        private double maxDist;
        private double trueMaxDist;
        private double trueMaxDistLinear;


        // Random number generator:
        static public Random rng = new Random();


        // Useful structs and classes:
        public struct Droplet {
            public Vector3 Vector;
            public float X;
            public float Y;
            public float Z;
            public int Size;
        }
        //
        //


        // Member functions used for generation:
        public void InitializeImageSettings(int size, int fr) {
            imgWidth = size;
            imgHeight = size;
            simWidth = imgWidth / 2;
            simHeight = imgHeight / 2;
            centerX = imgWidth / 2;
            centerY = imgHeight / 2;
            finalRes = fr;
            maxDist = Math.Sqrt(Math.Pow(simWidth, 2) + Math.Pow(simHeight, 2));
            trueMaxDist = Math.Sqrt(Math.Pow(imgWidth, 2) + Math.Pow(imgHeight, 2));
            trueMaxDistLinear = imgWidth / 2.0d;
        }


        public void ResizeFormForImage(PictureBox thePB = null) {
            // Used for resizing the application form to the dimensions of the image.
            AppPictureBox.Width = AppPictureBox.Image.Width;
            AppPictureBox.Height = AppPictureBox.Image.Height;
            int newWidth = AppPictureBox.Width;
            int newHeight = AppPictureBox.Height;
            this.Size = new Size(newWidth + 40, newHeight + 107);
            AppPictureBox.Location = new Point(12, 27);
            SelectionPanel.Location = new Point(
                this.ClientSize.Width / 2 - SelectionPanel.Width / 2,
                newHeight + 27 + 6
            );
        }


        public static Bitmap ResizeImage(Image image, int width, int height) {
            // Used for resizing images, e.g. for supersampling anti-aliasing (SSAA).
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage)) {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes()) {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }


        public static Color HslToColor(double h, double s, double l, int a = 255) {
            // Convert an HSL value into an RGB Color object.

            int r, g, b;

            double p2;
            if (l <= 0.5) p2 = l * (1 + s);
            else p2 = l + s - l * s;

            double p1 = 2 * l - p2;
            double double_r, double_g, double_b;
            if (s == 0) {
                double_r = l;
                double_g = l;
                double_b = l;
            }
            else {
                double_r = QqhToRgb(p1, p2, h + 120);
                double_g = QqhToRgb(p1, p2, h);
                double_b = QqhToRgb(p1, p2, h - 120);
            }

            // Convert RGB to the 0 to 255 range.
            r = (int)(double_r * 255.0);
            g = (int)(double_g * 255.0);
            b = (int)(double_b * 255.0);

            return Color.FromArgb(a, r, g, b);
        }


        private static double QqhToRgb(double q1, double q2, double hue) {
            if (hue > 360) hue -= 360;
            else if (hue < 0) hue += 360;

            if (hue < 60) return q1 + (q2 - q1) * hue / 60;
            if (hue < 180) return q2;
            if (hue < 240) return q1 + (q2 - q1) * (240 - hue) / 60;
            return q1;
        }


        public static double DistanceCalc(int ax, int ay, int bx, int by) {
            // Finds the straight-line distance between two points.
            return Convert.ToInt32(Math.Sqrt(Math.Pow(by - ay, 2) + Math.Pow(bx - ax, 2)));
        }


        public static Point RotatedAbout(int ax, int ay, int bx, int by, double angle) {
            // Rotates point `A` about point `B` by `angle` radians clockwise.
            double radius = DistanceCalc(ax, ay, bx, by);
            angle += Math.Atan2(ay - by, ax - bx);
            int newX = Convert.ToInt32(Math.Round(bx + radius * Math.Cos(angle)));
            int newY = Convert.ToInt32(Math.Round(by + radius * Math.Sin(angle)));
            return new Point(newX, newY);
        }


        public static double DegToRad(double deg) {
            return deg * Math.PI / 180;
        }


        public static double RadToDeg(double rad) {
            return rad * 180 / Math.PI;
        }


        public static int FleckCoordMake(int x, double scale, int middle, double stretch, bool cos = true) {
            int shapemod = rng.Next(5, 8);  // Higher values = smoother.
            double s = scale * middle * stretch;
            double rx = DegToRad(x);
            if (cos) {
                return Convert.ToInt32(Math.Round(s * Math.Cos(rx) + rng.Next(-middle / shapemod, middle / shapemod)));
            }
            return Convert.ToInt32(Math.Round(s * Math.Sin(rx) + rng.Next(-middle / shapemod, middle / shapemod)));
        }


        public static Point[] MakeFleckVerts(double scale, int middle, double stretch) {
            // Makes the vertices for a liquid fleck.
            int vertCount = 24;  // Should be a factor of 360.
            Point[] res = new Point[vertCount];
            for (int i = 0; i < 360; i += 360 / vertCount) {
                int coordX = FleckCoordMake(i, scale, middle, stretch);
                int coordY = FleckCoordMake(i, scale, middle, 1, false);
                res[i / (360 / vertCount)] = new Point(coordX, coordY);
            }
            return res;
        }
        //
        //


        // Big important functions, probably used directly by buttons:
        private void GenerateSplatterDecal() {
            if (DecalOutputData != null) {
                DecalOutputData.Dispose();
            }

            // What resolution the splatter is initially rendered at, and what resolution the outputted image will be:
            InitializeImageSettings(2048, 256);

            SolidBrush layerBrush = new SolidBrush(HslToColor(ActivePreset.GetH(), ActivePreset.GetS(), ActivePreset.GetL(), ActivePreset.GetAlpha()));
            Bitmap imgBackground;

            using (Bitmap output = new Bitmap(imgWidth, imgHeight)) {
                using (Graphics g = Graphics.FromImage(output)) {
                    // Background color:
                    g.Clear(bgColor);

                    // We save this to a variable to paste the splatter over later:
                    imgBackground = (Bitmap)output.Clone();

                    // Make the background transparent now:
                    g.Clear(Color.FromArgb(0, 127, 127, 127));

                    // Lists for fleckmap, fleck sizes, hotspot points, and the direction vectors for the flecks:
                    List<Point> fleckMap = new List<Point>();
                    List<int> fleckMapFleckSize = new List<int>();
                    List<Point> hotspotPoints = new List<Point>();
                    List<Vector3> fleckMapVectors = new List<Vector3>();



                    //-
                    // Generate all the flecks to be rendered.

                    Droplet[] drops = new Droplet[rng.Next(4000, 6000 + 1)];

                    // The minimum and maximum droplet size:
                    int minSize = simWidth / (simWidth / 2) + 2;
                    int maxSize = Convert.ToInt32(Math.Round(maxDist) / (simWidth / (simWidth / 32)));
                    if (maxSize <= minSize) {
                        maxSize = minSize + 1;
                    }

                    // The initial X & Y offset for the splatter aiming direction:
                    double RandOffsetX = 0;
                    double RandOffsetY = 0;
                    // Creates the array the random splatter aiming directions stored in. Length is randomized.
                    // Shorter arrays create more obvious "islands" of flecks and a less chaotic looking splatter overall.
                    double[,] RandOffsets = new double[rng.Next(8, 32 + 1), 2];

                    // Whether to use changes to the splatter aiming vector or just keep it all centered:
                    bool useRandOffsets = true;

                    // Creates the random splatter aiming directions to choose from:
                    for (int j = 0; j < RandOffsets.GetLength(0); j++) {
                        RandOffsets[j, 0] = (rng.NextDouble() - 0.5) * rng.Next(64, 129);
                        RandOffsets[j, 1] = (rng.NextDouble() - 0.5) * rng.Next(64, 129);
                    }

                    // Creates the weights for the while loop below that pick the fleck size. Higher subsequent values = less likely to pick higher values?
                    double fleckWeightPow = rng.NextDouble() + 2.0d;
                    int[] fleckWeights = new int[maxSize].Select((x, i) => (int)Math.Pow(i, fleckWeightPow)).ToArray();
                    // Similar as above. Not sure how it works (yet).
                    double[] distanceWeights = new double[simWidth / 8].Select((x, i) => i + 2.0d).ToArray();
                    double maxDistanceWeight = Convert.ToDouble(distanceWeights.Length) / (rng.NextDouble() * 1.5 + 1.0);

                    // Making all the flecks (second half of drops array is used to fine mist):
                    for (int i = 0; i < drops.Length; i++) {
                        // Choosing a which random offset to use?
                        int randOffsetsIndex = Convert.ToInt32((i) / (drops.Length / Convert.ToDouble(RandOffsets.GetLength(0))));
                        if ((i) % (drops.Length / RandOffsets.GetLength(0)) == 0 && randOffsetsIndex != 0 && useRandOffsets) {
                            RandOffsetX = RandOffsets[randOffsetsIndex - 1, 0];
                            RandOffsetY = RandOffsets[randOffsetsIndex - 1, 1];
                        }

                        // Setting up the drops:
                        drops[i].X = centerX;
                        drops[i].Y = centerY;
                        drops[i].Z = 256;

                        int fleckSize = 0;

                        int angle = rng.Next(0, 361);  // Pick a random angle.
                        double distance = 0;

                        // Picks a distance. More likely to pick near the center.
                        while (distance == 0) {
                            for (int j = 0; j < maxDistanceWeight; j++) {
                                double curr = rng.NextDouble();
                                if (curr * distanceWeights[j] < 0.1) {
                                    distance = j + (rng.NextDouble() - 0.5);
                                    break;
                                }
                            }
                        }

                        // This is used to control which comparator to use in the while loop below for what size of flecks.
                        int fleckSizeComparator = (drops.Length < drops.Length / 2) ? fleckWeights.Length : maxSize / 5;

                        // Picks a fleck size. More likely to pick a smaller size.
                        while (fleckSize == 0) {
                            for (int j = minSize; j < fleckWeights.Length; j++) {
                                if (rng.Next(0, fleckWeights[j]) == 0) {
                                    fleckSize = j;
                                    break;
                                }
                            }
                        }

                        // What does this do? The larger the fleck size the less we reduce the distance?
                        if (drops.Length < drops.Length / 2) {
                            // Whatever it is, it only applies to the main flecks, not fine mist.
                            distance *= ((maxSize - fleckSize * (rng.NextDouble() * 0.5d + 0.5d)) / maxSize);
                        }

                        drops[i].Size = fleckSize;
                        drops[i].Vector.X = Convert.ToSingle(distance * Math.Cos(angle) + RandOffsetX);
                        drops[i].Vector.Y = Convert.ToSingle(distance * Math.Sin(angle) + RandOffsetY);
                        drops[i].Vector.Z = 32;
                    }
                    //-



                    // Simulate the drops as physical entities and move them by their vector until they collide with a simulated wall at Z = 0.
                    // Then add their data in the fleck/fleck property lists.
                    // This has a counter that decrements in the for loop when the droplets have their z coord value reach <= 0.
                    // Once the counter reaches zero, it's done.
                    int dropCount = drops.Length;
                    while (dropCount > 0) {
                        for (int i = 0; i < drops.Length; i++) {
                            drops[i].X += drops[i].Vector.X;
                            drops[i].Y += drops[i].Vector.Y;
                            drops[i].Z -= drops[i].Vector.Z;
                            if (drops[i].Z <= 0) {
                                fleckMap.Add(new Point(Convert.ToInt32(drops[i].X), Convert.ToInt32(drops[i].Y)));
                                fleckMapFleckSize.Add(drops[i].Size);
                                fleckMapVectors.Add(drops[i].Vector);
                                dropCount--;
                            }
                        }
                    }

                    // Find the the left/right/top/bottom-most fleck and create a bounding box to recenter the splatter:
                    int leftMost = 0, rightMost = 0, topMost = 0, bottomMost = 0;
                    foreach (Point i in fleckMap) {
                        int x = i.X;
                        int y = i.Y;
                        int fromX = x - centerX;
                        int fromY = y - centerY;
                        if (fromX < leftMost) {
                            leftMost = fromX;
                        }
                        else if (fromX > rightMost) {
                            rightMost = fromX;
                        }
                        if (fromY < topMost) {
                            topMost = fromY;
                        }
                        else if (fromY > bottomMost) {
                            bottomMost = fromY;
                        }
                    }
                    int diffX = (leftMost + rightMost) / -2;
                    int diffY = (topMost + bottomMost) / -2;
                    // Recenter...
                    for (int i = 0; i < fleckMap.Count; i++) {
                        int x = fleckMap[i].X;
                        int y = fleckMap[i].Y;

                        fleckMap[i] = new Point(x + diffX, y + diffY);

                    }

                    // Rescale the fleckmap so no flecks appear outside of the image:
                    double scaleFactor = 1;
                    int maxX = 0;
                    int maxY = 0;
                    foreach (Point i in fleckMap) {
                        int x = i.X;
                        int y = i.Y;

                        int distX = Math.Abs(x - centerX);
                        int distY = Math.Abs(y - centerY);
                        if (distX > maxX) {
                            maxX = distX;
                        }
                        if (distY > maxY) {
                            maxY = distY;
                        }
                    }
                    double maxFoundDist = Math.Sqrt(Math.Pow(maxX, 2) + Math.Pow(maxY, 2));
                    scaleFactor = maxDist / maxFoundDist;
                    // Apply rescaling...
                    for (int i = 0; i < fleckMap.Count; i++) {
                        int x = fleckMap[i].X;
                        int y = fleckMap[i].Y;

                        int fromX = x - centerX;
                        int fromY = y - centerY;
                        int newFromX = Convert.ToInt32(fromX * scaleFactor);
                        int newFromY = Convert.ToInt32(fromY * scaleFactor);
                        fleckMap[i] = new Point(centerX + newFromX, centerY + newFromY);
                    }


                    // Scales of the layers a droplet is made up of:
                    double[] layerScales = new double[3];
                    for (int i = 0; i < layerScales.Length; i++) {
                        layerScales[i] = 1.25 - ((i + 1) / 6.0) * 0.75;
                    }

                    Color originalBrushColor = layerBrush.Color;

                    // Fleck rendering code:
                    for (int i = 0; i < 1; i++) {
                        for (int p = 0; p < fleckMap.Count; p++) {

                            bool drawFleck = true;

                            int x = fleckMap[p].X;
                            int y = fleckMap[p].Y;

                            // Euclidean distance from center.
                            int distX = Math.Abs(x - centerX);
                            int distY = Math.Abs(y - centerY);
                            int distance = Convert.ToInt32(Math.Sqrt(Math.Pow(distX, 2) + Math.Pow(distY, 2))) + 1;

                            int fleckSize = fleckMapFleckSize[p];

                            int fleckMiddle = fleckSize / 2;
                            int xMid = x + fleckMiddle;
                            int yMid = y + fleckMiddle;

                            // Get angle of fleck + add a slight bit of randomization to it:
                            double angle = Math.Atan2(fleckMapVectors[p].Y, fleckMapVectors[p].X) + rng.Next(-5, 5 + 1) * Math.PI / 180;

                            double stretch = 1 + Math.Sqrt(Math.Pow(fleckMapVectors[p].Y, 2) + Math.Pow(fleckMapVectors[p].X, 2)) / rng.Next(100, 200 + 1);

                            Point[] Verts = MakeFleckVerts(layerScales[i], fleckMiddle, stretch)
                                .Select((coords) => RotatedAbout(coords.X + x, coords.Y + y, x, y, angle)).ToArray();

                            int rndBnd = rng.Next(10, 60);  // Random bound???

                            // Prevents flecks from being drawn partially outside the image.
                            foreach (Point pn in Verts) {
                                if (pn.X > imgWidth - rndBnd || pn.X < rndBnd || pn.Y > imgHeight - rndBnd || pn.Y < rndBnd) {
                                    drawFleck = false;
                                }
                            }

                            if (drawFleck) {
                                // Color randomization should be more subtle toward the outside of the image.
                                double colorShiftDistanceScaling = (trueMaxDistLinear - (distance * 0.9)) / trueMaxDistLinear;
                                int colorShift = Convert.ToInt32((rng.NextDouble() - 1) * 72.0 * colorShiftDistanceScaling);

                                // Get the preset's color values.
                                int A = ActivePreset.GetAlpha();
                                Color OG = HslToColor(ActivePreset.GetH(), ActivePreset.GetS(), ActivePreset.GetL());
                                int R = OG.R;
                                int G = OG.G;
                                int B = OG.B;

                                // Check if any functions are applied to the color channels to alter their value per fleck.
                                if (ActivePreset.AlphaFunc != null) {
                                    A = ActivePreset.AlphaFunc(originalBrushColor, colorShift);
                                }
                                if (ActivePreset.RedFunc != null) {
                                    R = ActivePreset.RedFunc(originalBrushColor, colorShift);
                                }
                                if (ActivePreset.GreenFunc != null) {
                                    G = ActivePreset.GreenFunc(originalBrushColor, colorShift);
                                }
                                if (ActivePreset.BlueFunc != null) {
                                    B = ActivePreset.BlueFunc(originalBrushColor, colorShift);
                                }

                                layerBrush.Color = Color.FromArgb(A, R, G, B);
                                g.FillPolygon(layerBrush, Verts);
                            }
                        }
                    }

                    Bitmap res = ResizeImage(output, finalRes, finalRes);

                    using (Graphics resG = Graphics.FromImage(res)) {
                        resG.CompositingMode = CompositingMode.SourceOver;

                        Bitmap prev = (Bitmap)res.Clone();
                        resG.DrawImage(imgBackground, 0, 0);
                        resG.DrawImage(prev, 0, 0);
                    }
                    AppPictureBox.Image = (Image)res.Clone();

                    ResizeFormForImage();

                    DecalOutputData = (Image)res.Clone();
                }

                layerBrush.Dispose();
            }
        }
        //
        //


        // Button member functions:
        private void GenerateButton_Click(object sender, EventArgs e) {
            GenerateSplatterDecal();
        }


        private void SaveButton_Click(object sender, EventArgs e) {
            SaveFileDialog sfd1 = new SaveFileDialog();
            sfd1.Filter = "PNG |*.png";
            sfd1.Title = "Save as .png";
            sfd1.ShowDialog();

            if (sfd1.FileName != "") {
                DecalOutputData.Save(sfd1.FileName);
            }
        }


        private void OptionsButton_Click(object sender, EventArgs e) {
            OF.Show();
        }


        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
            string MessageBoxText = "SplatterGenCubed - Alpha\n\nCopyright © 2021\nLicensed under GPLV3.\nCreated by Hxdce";
            MessageBox.Show(MessageBoxText, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        //
        //
    }


    // Class for liquid presets:
    public class LiquidPreset {

        public LiquidPreset(string name, double[] hsl, int a, Func<Color, int, int> rf = null, Func<Color, int, int> gf = null, Func<Color, int, int> bf = null, Func<Color, int, int> af = null) {
            Name = name;
            SetAllLiquidOptions(hsl, a, rf, gf, bf, af);
        }

        public void SetAllLiquidOptions(double[] newHSL, int newA, Func<Color, int, int> newRF = null, Func<Color, int, int> newGF = null, Func<Color, int, int> newBF = null, Func<Color, int, int> newAF = null) {
            SetHSL(newHSL);
            Alpha = newA;
            RedFunc = newRF;
            GreenFunc = newGF;
            BlueFunc = newBF;
            AlphaFunc = newAF;
        }


        // Getters & Setters for HSL and Alpha;
        public int GetH() {
            return Convert.ToInt32(HSL[0]);
        }
        public double GetS() {
            return HSL[1];
        }
        public double GetL() {
            return HSL[2];
        }
        public int GetAlpha() {
            return Alpha;
        }

        public void SetHSL(int H, double S, double L) {
            SetH(H);
            SetS(S);
            SetL(L);
        }
        public void SetHSL(double[] newHSL) {
            SetH(Convert.ToInt32(newHSL[0]));
            SetS(newHSL[1]);
            SetL(newHSL[2]);
        }
        public void SetH(int H) {
            // Clamp Hue to 0 - 360:
            HSL[0] = Math.Min(360, Math.Max(0, H));
        }
        public void SetS(double S) {
            // Clamp Saturation to 0 - 1:
            HSL[1] = Math.Min(1.0, Math.Max(0.0, S));
        }
        public void SetL(double L) {
            // Clamp Lightness to 0 - 1:
            HSL[2] = Math.Min(1.0, Math.Max(0.0, L));
        }


        // Brush alteration functions:
        public Func<Color, int, int> RedFunc;
        public Func<Color, int, int> GreenFunc;
        public Func<Color, int, int> BlueFunc;
        public Func<Color, int, int> AlphaFunc;
        // Public member variables:
        public string Name;

        // Private member variables:
        private double[] HSL = { 0, 0, 0 };
        private int Alpha;
    }
}
