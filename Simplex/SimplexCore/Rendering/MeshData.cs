using System;
using System.Collections;
using System.Collections.Generic;

namespace Simplex.Core.Rendering
{
    public class MeshData : ISimplexUnique
    {
        private Guid uid = Guid.NewGuid();
        private Type dataType;

        private ArrayList data;
        private bool initialized = false;

        private string attributeName;

        public string AttributeName { get => attributeName; set => attributeName = value; }

        public Guid Uid { get => uid; }

        public bool Initialized { get => initialized; set => initialized = value; }
        public Type DataType { get => dataType; set => dataType = value; }
        public ArrayList Data { get => data; set => data = value; }
    }

    public class IntMeshData : MeshData
    {

        List<int> _data = new List<int>();

        public IntMeshData()
        {

        }


    }
}