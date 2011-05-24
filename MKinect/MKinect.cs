using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using xn;

namespace MKinect
{
    public class Kinect : IDisposable
    {
        private readonly string SAMPLE_XML_FILE = @"../../Data/SamplesConfig.xml";

        private Context context;
        private DepthGenerator depth;
        private ImageGenerator rawIm;
        private UserGenerator userGenerator;
        private SkeletonCapability skeletonCapbility;
        private PoseDetectionCapability poseDetectionCapability;
        private string calibPose;
        private Thread readerThread;
        private bool shouldRun;
        private Bitmap depthBitmap;
        private Bitmap rawImBitmap;
        private int[] histogram;

        private Dictionary<uint, Dictionary<SkeletonJoint, SkeletonJointPosition>> joints;

        private bool shouldDrawPixels = true;
        private bool shouldDrawBackground = true;
        private bool shouldPrintID = true;
        private bool shouldPrintState = true;
        private bool shouldDrawSkeleton = true;

        public Kinect()
        {
            try
            {
                this.SetupKinect();
            }
            catch (Exception)
            {
                MessageBox.Show("Kinect could not be started!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupKinect()
        {
            this.SkeletonUpdate += (u, s) => { };
            this.StatusUpdate += (u, s) => { };
            this.context = new Context(SAMPLE_XML_FILE);
            this.depth = context.FindExistingNode(NodeType.Depth) as DepthGenerator;
            if (this.depth == null)
            {
                throw new Exception("Viewer must have a depth node!");
            }
            this.rawIm = context.FindExistingNode(NodeType.Image) as ImageGenerator;

            this.userGenerator = new UserGenerator(this.context);
            this.skeletonCapbility = new SkeletonCapability(this.userGenerator);
            this.poseDetectionCapability = new PoseDetectionCapability(this.userGenerator);
            this.calibPose = this.skeletonCapbility.GetCalibrationPose();

            this.userGenerator.NewUser += new UserGenerator.NewUserHandler(userGenerator_NewUser);
            this.userGenerator.LostUser += new UserGenerator.LostUserHandler(userGenerator_LostUser);
            this.poseDetectionCapability.PoseDetected += new PoseDetectionCapability.PoseDetectedHandler(poseDetectionCapability_PoseDetected);
            this.skeletonCapbility.CalibrationEnd += new SkeletonCapability.CalibrationEndHandler(skeletonCapbility_CalibrationEnd);

            this.skeletonCapbility.SetSkeletonProfile(SkeletonProfile.All);
            this.joints = new Dictionary<uint, Dictionary<SkeletonJoint, SkeletonJointPosition>>();
            this.userGenerator.StartGenerating();

            this.histogram = new int[this.depth.GetDeviceMaxDepth()];

            MapOutputMode mapMode = this.depth.GetMapOutputMode();
            this.depthBitmap = new Bitmap((int)mapMode.nXRes, (int)mapMode.nYRes/*, System.Drawing.Imaging.PixelFormat.Format24bppRgb*/);
            this.rawImBitmap = new Bitmap((int)mapMode.nXRes, (int)mapMode.nYRes, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            this.shouldRun = true;
            this.readerThread = new Thread(ReaderThread);
            this.readerThread.Start();
        }

        public event Action<uint, Body.Skeleton> SkeletonUpdate;
        public event Action<uint, UserStatus> StatusUpdate;

        public Bitmap GetScene()
        {
            lock (this)
            {
                try
                {
                    if (this.depthBitmap != null) return this.depthBitmap.Clone() as Bitmap;
                    else return null;
                }
                catch (Exception) { return null; }
            }
        }

        public Bitmap GetCameraImage()
        {
            lock (this)
            {
                if (this.rawImBitmap != null) return this.rawImBitmap.Clone() as Bitmap;
                else return null;
            }
        }

        public void StopAndClose()
        {
            this.shouldRun = false;
            this.readerThread.Join();
        }

        public void Dispose()
        {
            depthBitmap.Dispose();
            rawImBitmap.Dispose();
            poseDetectionCapability.Dispose();
            skeletonCapbility.Dispose();
            userGenerator.Dispose();
            depth.Dispose();
            rawIm.Dispose();
            context.Dispose();
        }

        #region internals
        void skeletonCapbility_CalibrationEnd(ProductionNode node, uint id, bool success)
        {
            if (success)
            {
                this.skeletonCapbility.StartTracking(id);
                this.joints.Add(id, new Dictionary<SkeletonJoint, SkeletonJointPosition>());
            }
            else
            {
                this.poseDetectionCapability.StartPoseDetection(calibPose, id);
            }
        }

        void poseDetectionCapability_PoseDetected(ProductionNode node, string pose, uint id)
        {
            this.poseDetectionCapability.StopPoseDetection(id);
            this.skeletonCapbility.RequestCalibration(id, true);
        }

        void userGenerator_NewUser(ProductionNode node, uint id)
        {
            this.poseDetectionCapability.StartPoseDetection(this.calibPose, id);
        }

        void userGenerator_LostUser(ProductionNode node, uint id)
        {
            this.joints.Remove(id);
        }

        private unsafe void CalcHist(DepthMetaData depthMD)
        {
            // reset
            for (int i = 0; i < this.histogram.Length; ++i)
                this.histogram[i] = 0;

            ushort* pDepth = (ushort*)depthMD.DepthMapPtr.ToPointer();

            int points = 0;
            for (int y = 0; y < depthMD.YRes; ++y)
            {
                for (int x = 0; x < depthMD.XRes; ++x, ++pDepth)
                {
                    ushort depthVal = *pDepth;
                    if (depthVal != 0)
                    {
                        this.histogram[depthVal]++;
                        points++;
                    }
                }
            }

            for (int i = 1; i < this.histogram.Length; i++)
            {
                this.histogram[i] += this.histogram[i - 1];
            }

            if (points > 0)
            {
                for (int i = 1; i < this.histogram.Length; i++)
                {
                    this.histogram[i] = (int)(256 * (1.0f - (this.histogram[i] / (float)points)));
                }
            }
        }

        private Color[] colors = { Color.Red, Color.Blue, Color.ForestGreen, Color.Yellow, Color.Orange, Color.Purple, Color.White };
        private Color[] anticolors = { Color.Green, Color.Orange, Color.Red, Color.Purple, Color.Blue, Color.Yellow, Color.Black };
        private int ncolors = 6;

        private void GetJoint(uint user, SkeletonJoint joint)
        {
            SkeletonJointPosition pos = new SkeletonJointPosition();
            this.skeletonCapbility.GetSkeletonJointPosition(user, joint, ref pos);
            if (pos.position.Z == 0)
            {
                pos.fConfidence = 0;
            }
            else
            {
                pos.position = this.depth.ConvertRealWorldToProjective(pos.position);
            }
            this.joints[user][joint] = pos;
        }

        private void GetJoints(uint user)
        {
            GetJoint(user, SkeletonJoint.Head);
            GetJoint(user, SkeletonJoint.Neck);

            GetJoint(user, SkeletonJoint.LeftShoulder);
            GetJoint(user, SkeletonJoint.LeftElbow);
            GetJoint(user, SkeletonJoint.LeftHand);

            GetJoint(user, SkeletonJoint.RightShoulder);
            GetJoint(user, SkeletonJoint.RightElbow);
            GetJoint(user, SkeletonJoint.RightHand);

            GetJoint(user, SkeletonJoint.Torso);

            GetJoint(user, SkeletonJoint.LeftHip);
            GetJoint(user, SkeletonJoint.LeftKnee);
            GetJoint(user, SkeletonJoint.LeftFoot);

            GetJoint(user, SkeletonJoint.RightHip);
            GetJoint(user, SkeletonJoint.RightKnee);
            GetJoint(user, SkeletonJoint.RightFoot);
        }

        private void DrawLine(Graphics g, Color color, Dictionary<SkeletonJoint, SkeletonJointPosition> dict, SkeletonJoint j1, SkeletonJoint j2)
        {
            Point3D pos1 = dict[j1].position;
            Point3D pos2 = dict[j2].position;

            if (dict[j1].fConfidence == 0 || dict[j2].fConfidence == 0)
                return;

            g.DrawLine(new Pen(color),
                        new Point((int)pos1.X, (int)pos1.Y),
                        new Point((int)pos2.X, (int)pos2.Y));

        }

        private void HandleSkeleton(Graphics g, Color color, uint user)
        {
            GetJoints(user);
            Dictionary<SkeletonJoint, SkeletonJointPosition> dict = this.joints[user];
            this.DrawSkeleton(g, color, dict);
            this.NotifyUserWithSkeleton(user);
        }

        private void NotifyUserWithSkeleton(uint user)
        {
            this.SkeletonUpdate(user, new Body.Skeleton(this.joints[user]));
        }

        private void DrawSkeleton(Graphics g, Color color, Dictionary<SkeletonJoint, SkeletonJointPosition> dict)
        {
            DrawLine(g, color, dict, SkeletonJoint.Head, SkeletonJoint.Neck);

            DrawLine(g, color, dict, SkeletonJoint.LeftShoulder, SkeletonJoint.Torso);
            DrawLine(g, color, dict, SkeletonJoint.RightShoulder, SkeletonJoint.Torso);

            DrawLine(g, color, dict, SkeletonJoint.Neck, SkeletonJoint.LeftShoulder);
            DrawLine(g, color, dict, SkeletonJoint.LeftShoulder, SkeletonJoint.LeftElbow);
            DrawLine(g, color, dict, SkeletonJoint.LeftElbow, SkeletonJoint.LeftHand);

            DrawLine(g, color, dict, SkeletonJoint.Neck, SkeletonJoint.RightShoulder);
            DrawLine(g, color, dict, SkeletonJoint.RightShoulder, SkeletonJoint.RightElbow);
            DrawLine(g, color, dict, SkeletonJoint.RightElbow, SkeletonJoint.RightHand);

            DrawLine(g, color, dict, SkeletonJoint.LeftHip, SkeletonJoint.Torso);
            DrawLine(g, color, dict, SkeletonJoint.RightHip, SkeletonJoint.Torso);
            DrawLine(g, color, dict, SkeletonJoint.LeftHip, SkeletonJoint.RightHip);

            DrawLine(g, color, dict, SkeletonJoint.LeftHip, SkeletonJoint.LeftKnee);
            DrawLine(g, color, dict, SkeletonJoint.LeftKnee, SkeletonJoint.LeftFoot);

            DrawLine(g, color, dict, SkeletonJoint.RightHip, SkeletonJoint.RightKnee);
            DrawLine(g, color, dict, SkeletonJoint.RightKnee, SkeletonJoint.RightFoot);
        }

        private unsafe void ReaderThread()
        {
            DepthMetaData depthMD = new DepthMetaData();
            ImageMetaData rawImMD = new ImageMetaData();

            while (this.shouldRun)
            {
                try
                {
                    this.context.WaitOneUpdateAll(this.depth);
                }
                catch (Exception)
                {
                }

                this.depth.GetMetaData(depthMD);
                this.rawIm.GetMetaData(rawImMD);

                CalcHist(depthMD);

                lock (this)
                {
                    Rectangle rect = new Rectangle(0, 0, this.depthBitmap.Width, this.depthBitmap.Height);

                    this.GetRawImage(rawImMD, rect);
                    this.GetDepthImage(depthMD, rect);

                    Graphics g = Graphics.FromImage(this.depthBitmap);
                    uint[] users = this.userGenerator.GetUsers();
                    foreach (uint user in users)
                    {
                        this.DrawUserOverlay(g, user);
                    }
                    g.Dispose();
                }
            }
        }

        private unsafe void DrawUserOverlay(Graphics g, uint user)
        {
            if (this.shouldPrintID)
            {
                Point3D com = this.userGenerator.GetCoM(user);
                com = this.depth.ConvertRealWorldToProjective(com);

                string label = "";
                if (!this.shouldPrintState)
                {
                    label += user;
                    this.StatusUpdate(user, UserStatus.Detected);
                }
                else if (this.skeletonCapbility.IsTracking(user))
                {
                    label += user + " - Tracking";
                    this.StatusUpdate(user, UserStatus.Tracking);
                }
                else if (this.skeletonCapbility.IsCalibrating(user))
                {
                    label += user + " - Calibrating...";
                    this.StatusUpdate(user, UserStatus.Calibrating);
                }
                else
                {
                    label += user + " - Looking for pose";
                    this.StatusUpdate(user, UserStatus.LookingForPose);
                }

                g.DrawString(label, new Font("Arial", 6), new SolidBrush(anticolors[user % ncolors]), com.X, com.Y);

            }

            if (this.shouldDrawSkeleton && this.skeletonCapbility.IsTracking(user))
                //                        if (this.skeletonCapbility.IsTracking(user))
                HandleSkeleton(g, anticolors[user % ncolors], user);
        }

        private unsafe void GetDepthImage(DepthMetaData depthMD, Rectangle rect)
        {
            BitmapData data = this.depthBitmap.LockBits(rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            if (this.shouldDrawPixels)
            {
                ushort* pDepth = (ushort*)this.depth.GetDepthMapPtr().ToPointer();
                ushort* pLabels = (ushort*)this.userGenerator.GetUserPixels(0).SceneMapPtr.ToPointer();

                // set pixels
                for (int y = 0; y < depthMD.YRes; ++y)
                {
                    byte* pDest = (byte*)data.Scan0.ToPointer() + y * data.Stride;
                    for (int x = 0; x < depthMD.XRes; ++x, ++pDepth, ++pLabels, pDest += 3)
                    {
                        pDest[0] = pDest[1] = pDest[2] = 0;

                        ushort label = *pLabels;
                        if (this.shouldDrawBackground || *pLabels != 0)
                        {
                            Color labelColor = Color.White;
                            if (label != 0)
                            {
                                labelColor = colors[label % ncolors];
                            }

                            byte pixel = (byte)this.histogram[*pDepth];
                            pDest[0] = (byte)(pixel * (labelColor.B / 256.0));
                            pDest[1] = (byte)(pixel * (labelColor.G / 256.0));
                            pDest[2] = (byte)(pixel * (labelColor.R / 256.0));
                        }
                    }
                }
            }
            this.depthBitmap.UnlockBits(data);
        }

        private unsafe void GetRawImage(ImageMetaData rawImMD, Rectangle rect)
        {
            BitmapData rawImData = this.rawImBitmap.LockBits(rect, ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            byte* pSrc = (byte*)this.rawIm.GetImageMapPtr().ToPointer();
            for (int y = 0; y < rawImMD.YRes; ++y)
            {
                byte* pDest = (byte*)rawImData.Scan0.ToPointer() + y * rawImData.Stride;
                for (int x = 0; x < rawImMD.XRes; ++x, pSrc += 3, pDest += 3)
                {
                    pDest[0] = pSrc[2]; pDest[1] = pSrc[1]; pDest[2] = pSrc[0];
                }
            }
            this.rawImBitmap.UnlockBits(rawImData);
        }
        #endregion
    }
}
