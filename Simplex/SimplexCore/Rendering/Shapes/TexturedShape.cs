//
// TexturedShape.cs
//
// Copyright (C) 2018 OpenTK
//
// This software may be modified and distributed under the terms
// of the MIT license. See the LICENSE file for details.
//

using ObjectTK.Buffers;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Simplex.Core.Rendering
{
    /// <summary>
    /// Base class for creating textured shapes
    /// </summary>
    public abstract class TexturedShape
        : Shape
    {
        /// <summary>
        /// the texture coordinates the buffer gets filled with
        /// </summary>
        public Vector2[] TexCoords { get; protected set; }
        /// <summary>
        /// the texture coordinate buffer
        /// </summary>
        public Buffer<Vector2> TexCoordBuffer { get; protected set; }

        /// <summary>
        /// Initialiazes/Updates the buffers. Should be called after creation
        /// </summary>
        public override void UpdateBuffers()
        {
            base.UpdateBuffers();
            TexCoordBuffer = new Buffer<Vector2>();
            TexCoordBuffer.Init(BufferTarget.ArrayBuffer, TexCoords);
        }

        /// <summary>
        /// disposes the nested buffers
        /// </summary>
        /// <param name="manual"></param>
        protected override void Dispose(bool manual)
        {
            base.Dispose(manual);
            if (TexCoordBuffer != null) TexCoordBuffer.Dispose();
        }
    }
}