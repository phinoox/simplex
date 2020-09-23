using System.Numerics;

namespace Simplex.Gui{
    
    public class BoundingBox2D{
            private Vector2 _min;
            private Vector2 _max;
            

            public BoundingBox2D(int minX,int minY,int maxX,int MaxY){
                _min = new Vector2(minX,minY);
                _max = new Vector2(maxX,MaxY);
            }

    }
}