using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using MKinect.Body.Actions;

namespace MKinectUIExtensions.Trackers
{
    public class RollingDice : ModelVisual3D
    {
        private ReformBodyParts _bodyParts;
        private AxisAngleRotation3D _xRotation;
        private AxisAngleRotation3D _yRotation;
        private AxisAngleRotation3D _zRotation;

        public RollingDice()
            : base()
        {
            base.Content = CreateCubeGroup(5, 5, 5);
            this.SetupRotation();
        }

        private void SetupRotation()
        {
            this._xRotation = new AxisAngleRotation3D(new Vector3D(1, 0, 0), 0);
            this._yRotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), 0);
            this._zRotation = new AxisAngleRotation3D(new Vector3D(0, 0, 1), 0);
            this.GroupRotations();
        }

        private void GroupRotations()
        {
            this.Transform = new Transform3DGroup()
            {
                Children =
                {
                    new RotateTransform3D(this._xRotation),
                    new RotateTransform3D(this._yRotation),
                    new RotateTransform3D(this._zRotation),
                }
            };
        }

        public void StartTracking(ReformBodyParts bodyParts)
        {
            this._bodyParts = bodyParts;
            this._bodyParts.Rotate += RotateMe;
        }

        private void RotateMe(double x, double y, double z)
        {
            this.Dispatch(() =>
            {
                //this._xRotation.Angle = x;
                this._yRotation.Angle = y;
                this._zRotation.Angle = z;
            });
        }

        private void Dispatch(Action dispatch)
        {
            this.Dispatcher.BeginInvoke(dispatch);
        }

        private static Model3DGroup CreateCubeGroup(int width, int height, int depth)
        {
            Model3DGroup cube = new Model3DGroup();
            Point3D p0 = new Point3D(width / -2.0, height / -2.0, depth / -2.0);
            Point3D p1 = new Point3D(p0.X + width, p0.Y, p0.Z);
            Point3D p2 = new Point3D(p0.X + width, p0.Y, p0.Z + depth);
            Point3D p3 = new Point3D(p0.X, p0.Y, p0.Z + depth);
            Point3D p4 = new Point3D(p0.X, p0.Y + height, p0.Z);
            Point3D p5 = new Point3D(p0.X + width, p0.Y + height, p0.Z);
            Point3D p6 = new Point3D(p0.X + width, p0.Y + height, p0.Z + depth);
            Point3D p7 = new Point3D(p0.X, p0.Y + height, p0.Z + depth);
            //front side triangles
            cube.Children.Add(CreateTriangleModel(p3, p2, p6, Colors.Red));
            cube.Children.Add(CreateTriangleModel(p3, p6, p7, Colors.Red));
            //right side triangles
            cube.Children.Add(CreateTriangleModel(p2, p1, p5, Colors.Green));
            cube.Children.Add(CreateTriangleModel(p2, p5, p6, Colors.Green));
            //back side triangles
            cube.Children.Add(CreateTriangleModel(p1, p0, p4, Colors.Blue));
            cube.Children.Add(CreateTriangleModel(p1, p4, p5, Colors.Blue));
            //left side triangles
            cube.Children.Add(CreateTriangleModel(p0, p3, p7, Colors.Yellow));
            cube.Children.Add(CreateTriangleModel(p0, p7, p4, Colors.Yellow));
            //top side triangles
            cube.Children.Add(CreateTriangleModel(p7, p6, p5, Colors.Cyan));
            cube.Children.Add(CreateTriangleModel(p7, p5, p4, Colors.Cyan));
            //bottom side triangles
            cube.Children.Add(CreateTriangleModel(p2, p3, p0, Colors.Magenta));
            cube.Children.Add(CreateTriangleModel(p2, p0, p1, Colors.Magenta));

            return cube;
        }

        private static Model3DGroup CreateTriangleModel(Point3D p0, Point3D p1, Point3D p2, Color c)
        {
            MeshGeometry3D mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            Vector3D normal = CalculateNormal(p0, p1, p2);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            Material material = new DiffuseMaterial(
                new SolidColorBrush(c));
            GeometryModel3D model = new GeometryModel3D(
                mesh, material);
            Model3DGroup group = new Model3DGroup();
            group.Children.Add(model);
            return group;
        }

        private static Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            Vector3D v0 = new Vector3D(
                p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            Vector3D v1 = new Vector3D(
                p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }
    }
}
