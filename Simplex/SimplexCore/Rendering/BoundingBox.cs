using OpenTK;
using ObjectTK.Tools.Shapes;

namespace Simplex.Core.Rendering{
   

   public class BoundingBox{
   private Vector3 _min = new Vector3(-0.5f);
   private Vector3 _max = new Vector3(0.5f);

        private Cube _cube;
        public Vector3 Max { get => _max; set => _max = value; }
        public Vector3 Min { get => _min; set => _min = value; }

        public Vector3 Center { get {return _min + (_max - _min) * 0.5f; }}

        public void Init(){
            
        }

        public BoundingBox Scaled(Vector3 scale){
            BoundingBox box = new BoundingBox();
            box.Max = _max * scale;
            box.Min = _min * scale;
            return box;
        }
    }

}