using OpenTK;
using ObjectTK.Tools.Shapes;

namespace Simplex.Core.Rendering{
   

   public class BoundingBox{
   private Vector3 _min;
   private Vector3 _max;

        private Cube _cube;
        public Vector3 Max { get => _max; set => _max = value; }
        public Vector3 Min { get => _min; set => _min = value; }

        public void Init(){
            
        }
    }

}