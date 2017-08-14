using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace my_diy_IK_Fabrik_Test
{
    public class IKLink
    {
        private Vector3D _Vector = new Vector3D(0,0,0);
        private List<IKLink> _childen = new List<IKLink>();

        public IKLink(Vector3D vec)
        {
            _Vector = vec;
        }

        public IKLink(float x, float y, float z)
        {
            _Vector = new Vector3D(x, y, z);
        }

        public List<IKLink> Children { get { return _childen; } }

        public Vector3D Vector { get { return _Vector; } }

        public Matrix3D Rotation { get; set; } 
    }
}
