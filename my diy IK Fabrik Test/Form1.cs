using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Windows.Media.Media3D;

namespace my_diy_IK_Fabrik_Test
{
    public partial class Form1 : Form
    {
        private IKLink _base = null;
        private IKLink _base2 = null;
        private Pen _pen = Pens.Black;
        private int[] _rotations = new int[2];

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            float len1x = 50; float len1y = 10;
            float len2x = 40; float len2y = 40;

            double rad120 = 120 * Math.PI / 180;

            _base = new IKLink(new Vector3D(this.Size.Width / 2, this.Size.Height / 2, 0));
            _base.Rotation = new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            var newEl = new IKLink(len1x, len1y, 0);
            _base.Children.Add(newEl);
            newEl.Children.Add(new IKLink(len2x, len2y, 0));

            _base2 = new IKLink(new Vector3D(this.Size.Width / 2, this.Size.Height / 2, 0));
            _base2.Rotation = new Matrix3D(Math.Cos(rad120), 0, -Math.Sin(rad120), 0, 1, 0, 0, 0, Math.Sin(rad120), 0, Math.Cos(rad120), 0, 0, 0, 0, 1);
            newEl = new IKLink(len1x, len1y, 0);
            _base2.Children.Add(newEl);
            newEl.Children.Add(new IKLink(len2x, len2y, 0));

        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (_base == null) return;

            foreach (var b in new IKLink[] { _base, _base2 })
            {
                //if (_base == b) continue;
                if (_base2 == b) continue;
                var currVec = Vector3D.Multiply(b.Children[0].Vector, b.Rotation);

                var currPos = new Point3D(b.Vector.X, b.Vector.Y, b.Vector.Z);

                var rot = b.Children[0].Rotation;

                var destPos = Vector3D.Multiply(currVec, rot);

                Debug.WriteLine("currVec {0},   DestPos {1},    Rotation {2}", currVec, destPos, rot);

                var endPos = new Point3D(destPos.X + currPos.X, destPos.Y + currPos.Y, destPos.Z + currPos.Z);
                e.Graphics.DrawLine(_pen, (float)currPos.X, (float)currPos.Y, (float)endPos.X, (float)endPos.Y);
                currPos = endPos;

                currVec = b.Children[0].Children[0].Vector;
                rot.Append(b.Children[0].Children[0].Rotation);
                destPos = currVec * rot;

                endPos = new Point3D(destPos.X + currPos.X, destPos.Y + currPos.Y, destPos.Z + currPos.Z);
                e.Graphics.DrawLine(_pen, (float)currPos.X, (float)currPos.Y, (float)endPos.X, (float)endPos.Y);


            }


        }


        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            int grad;
            int delta = e.Delta / 40;

            IKLink el = null;
            IKLink el2 = null;
            if (!Control.ModifierKeys.HasFlag(Keys.Shift))
            {
                el = _base.Children[0];
                el2 = _base2.Children[0];
                grad = _rotations[0] + delta;
                _rotations[0] += delta;
            }
            else
            {
                el = _base.Children[0].Children[0];
                el2 = _base2.Children[0].Children[0];
                grad = _rotations[1] + delta;
                _rotations[1] += delta;
            }

            float bogen = (float)(grad * Math.PI / 180);
            Debug.WriteLine("Grad: {0} TotalGrad: {1} bogen: {2}", delta, grad, bogen);

            //el.Rotation = new Vector3((float)Math.Cos(bogen), (float)Math.Sin(bogen), 1);
            // el.Rotation = new Matrix3D(Math.Cos(bogen), Math.Sin(bogen),-Math.Sin(bogen));
            el.Rotation = new Matrix3D(Math.Cos(bogen), Math.Sin(bogen), 0, 0, -Math.Sin(bogen), Math.Cos(bogen), 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);

            //https://math.stackexchange.com/questions/142821/matrix-for-rotation-around-a-vector
            Vector3D rotVec1 = new Vector3D(1, 0, 0);

            //to extend 3x3 to 4x4
            float dummy0 = 0;
            float dummy1 = 1;

            var W = new Matrix3D(0, rotVec1.Z, -rotVec1.Y, dummy0, -rotVec1.Z, 0, rotVec1.X, dummy0, rotVec1.Y, -rotVec1.X, 0, dummy0, dummy0, dummy0, dummy0, dummy1);
            var W2 = Matrix3D.Multiply(W, W);
            var sin2 = Math.Sin(bogen) * Math.Sin(bogen);

            var rot = Matrix3D.Identity + Matrix3D.Sc( Math.Sin(bogen) , W) + 2 * sin2 * W ^ 2;

            el2.Rotation = new Matrix3D(Math.Cos(bogen), Math.Sin(bogen), 0, 0, -Math.Sin(bogen), Math.Cos(bogen), 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            this.Invalidate();
            // Debug.WriteLine("{0:n4}   {0:n4}",el.Rotation.X,el.Rotation.Y);
            //Debug.WriteLine(el.Rotation);
        }
    }
}
