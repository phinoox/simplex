using OpenTK;
using Simplex.BVT;
using System;
using System.Collections.Generic;
using System.Linq;
using Simplex.Core.Rendering;
using Simplex.Core.Util;
using OpenTK.Mathematics;

namespace Simplex.Core.Scene
{
    /// <summary>
    /// Base class for a node in the 3D Scene
    /// </summary>
    public class SceneNode : IDisposable
    {

        #region Private Fields

        private bool _disposed = false;
        private BVTRoot _behavior = new BVTRoot();
        private List<SceneNode> _childNodes = new List<SceneNode>();
        private string _name;
        private SceneNode _parent;
        protected Quaternion _rotation = Quaternion.Identity;
        protected Vector3 _scale = new Vector3(1);
        protected Vector3 _translation = Vector3.Zero;
        protected BoundingBox _bounds = new BoundingBox();
        private Guid uid;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// default constructor,when inheriting override the oncreate function for easier handling
        /// </summary>
        public SceneNode()
        {
            onCreate();
        }

        #endregion Public Constructors

        #region Private Destructors

        /// <summary>
        /// calls the onDestroy method
        /// </summary>
        ~SceneNode()
        {
            if (!_disposed)
                Dispose();
        }

        #endregion Private Destructors

        #region Public Properties


        public bool AvoidRole { get; set; }

        public Quaternion Rotation { get => _rotation; set { _rotation = value; onRotate(); } }
        public Vector3 Scale { get => _scale; set => _scale = value; }
        public Vector3 Translation { get => _translation; set { _translation = value; onTranslate(); } }

        public Quaternion GlobalRotation
        {
            get
            {
                if (_parent == null)
                    return _rotation;
                else
                    return _rotation * _parent.GlobalRotation;
            }
            set
            {
                _rotation = value; onRotate();
            }
        }
        public Vector3 GlobalScale
        {
            get
            {
                if (_parent == null)
                    return _scale;
                else
                    return _scale * _parent.GlobalScale;
            }
            set
            {
                _scale = value;
            }
        }

        public Vector3 GlobalTranslation
        {
            get
            {
                if (_parent == null)
                    return _translation;
                else
                    return _translation + _parent.GlobalTranslation;
            }
            set
            {
                _translation = value; onTranslate();
            }
        }

        public virtual Vector3 Forward { get { return (_rotation * WorldDefaults.Forward).Normalized(); } }

        public virtual Vector3 Right
        {
            get
            {
                Vector3 right = (_rotation * WorldDefaults.Right).Normalized();
                return right;
            }
        }

        public virtual Vector3 Up { get { return (_rotation * WorldDefaults.Up).Normalized(); } }
        public string Name { get => _name; set => _name = value; }
        public BoundingBox Bounds { get => _bounds.Scaled(_scale); set => _bounds = value; }
        public SceneNode Parent { get => _parent; set => _parent = value; }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// gets called from the parent node when the node is added
        /// </summary>
        /// <param name="parent">the parent scene node</param>
        private void TreeEntered(SceneNode parent)
        {
            if (this._parent != null)
                onTreeMoved(this._parent, parent);
            this._parent = parent;
            if (this.uid == null)
                this.uid = Guid.NewGuid();
            onTreeEntered();
        }

        #endregion Private Methods

        #region Protected Methods

        /// <summary>
        /// gets called from the constructor when the object is created
        /// </summary>
        protected virtual void onCreate() { }

        /// <summary>
        /// gets called from the destructor
        /// </summary>
        protected virtual void onDestroy() { }

        protected virtual void onRotate()
        {
        }

        protected virtual void onScale()
        {
        }

        /// <summary>
        /// gets called every engine tick
        /// </summary>
        /// <param name="delta">the time in milliseconds since the last frame</param>
        protected virtual void onTick(float delta) { }

        protected virtual void onTranslate()
        {
        }

        /// <summary>
        /// gets called when the node is added to another node
        /// </summary>
        protected virtual void onTreeEntered()
        {
        }

        /// <summary>
        /// gets called when it gets removed from the parent
        /// </summary>
        protected virtual void onTreeExited()
        {
        }

        /// <summary>
        /// gets called when the parent node changed
        /// </summary>
        /// <param name="oldParent"></param>
        /// <param name="newParent"></param>
        protected virtual void onTreeMoved(SceneNode oldParent, SceneNode newParent)
        {
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// adds a childnode
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(SceneNode child)
        {
            this._childNodes.Add(child);
            child.TreeEntered(this);
        }

        /// <summary>
        /// finds a child node by its guid
        /// </summary>
        /// <param name="guid">guid of the childnode</param>
        /// <returns>the childnode,null if non found</returns>
        public SceneNode FindChild(Guid guid)
        {
            return this._childNodes.Find(x => x.uid == guid);
        }

        /// <summary>
        /// tries to find a child by its name
        /// </summary>
        /// <param name="name">the name of the node</param>
        /// <returns>the node if found,else null</returns>
        public SceneNode FindChild(string name, bool recursive = true)
        {
            SceneNode node = this._childNodes.Find(x => x._name == name);
            if (node != null)
                return node;
            foreach (SceneNode child in _childNodes)
            {
                node = child.FindChild(name);
                if (node != null)
                    return node;
            }
            return node;
        }

        public List<T> FindChildByType<T>(bool recursive = true)
        {
            List<T> found = new List<T>();
            IEnumerable<T> nodeList = _childNodes.Where(x => x.GetType() == typeof(T)).Cast<T>();
            IEnumerable<T> own = nodeList as IEnumerable<T>;

            if (own != null)
                found.AddRange(own);

            if (!recursive)
                return found;

            foreach (SceneNode child in _childNodes)
            {
                found.AddRange(child.FindChildByType<T>());
            }

            return found;
        }

        /// <summary>
        /// returns thechildren of this node as array
        /// </summary>
        /// <returns></returns>
        public SceneNode[] GetChildren()
        {
            return _childNodes.ToArray();
        }

        /// <summary>
        /// gets called when it gets removed from the parent
        /// </summary>
        public void TreeExited()
        {
            onTreeExited();
        }

        /// <summary>
        /// gets called when the parent node changed
        /// </summary>
        /// <param name="newParent"></param>
        public void TreeMoved(SceneNode newParent)
        {
            SceneNode oldParent = this._parent;
            oldParent._childNodes.Remove(this);

            onTreeMoved(oldParent, newParent);
        }

        #endregion Public Methods

        public void ClearChildren()
        {
            foreach (SceneNode child in _childNodes)
            {
                child.Dispose();
            }
            _childNodes.Clear();
        }

        public virtual void CalculateBounds()
        {


            Vector3 minBounds = Bounds.Min;
            Vector3 maxBounds = Bounds.Max;
            bool first = true;
            foreach (SceneNode child in _childNodes)
            {
                child.CalculateBounds();
                Vector3 primMin = child.Bounds.Min;
                Vector3 primMax = child.Bounds.Max;
                if (first)
                {
                    minBounds = primMin;
                    maxBounds = primMax;
                    first = false;
                }
                if (primMin.X < minBounds.X)
                    minBounds.X = primMin.X;
                if (primMax.X > maxBounds.X)
                    maxBounds.X = primMax.X;

                if (primMin.Y < minBounds.Y)
                    minBounds.Y = primMin.Y;
                if (primMax.Y > maxBounds.Y)
                    maxBounds.Y = primMax.Y;

                if (primMin.Z < minBounds.Z)
                    minBounds.Z = primMin.Z;
                if (primMax.Z > maxBounds.Z)
                    maxBounds.Z = primMax.Z;
            }

            Bounds.Min = minBounds;
            Bounds.Max = maxBounds;
        }

        public virtual void Pitch(float angle, bool local = true)
        {
            float radAngle = MathHelper.DegreesToRadians(angle);
            //foreach (SceneNode child in _childNodes)
            //    child.RotateX(angle);

            Vector3 right = WorldDefaults.Right;
            if (local)
            {
                right = Right;
            }

            Quaternion rot = Quaternion.FromAxisAngle(right, radAngle).Normalized();
            // Rotation = (_rotation * rot).Normalized();
            Rotation = (rot * _rotation).Normalized();
        }



        public virtual void Yaw(float angle, bool local = true)
        {
            float radAngle = MathHelper.DegreesToRadians(angle);
            // foreach (SceneNode child in _childNodes)
            //     child.RotateY(angle);
            Vector3 up = WorldDefaults.Up;
            if (local)
                up = Up;
            Quaternion rot = Quaternion.FromAxisAngle(up, radAngle);
            // Rotation = (_rotation * rot).Normalized();
            //Rotation = (_rotation * rot).Normalized();
            Rotation = (rot * _rotation ).Normalized();
        }

        public virtual void Roll(float angle, bool local = true)
        {
            float radAngle = MathHelper.DegreesToRadians(angle);
            Vector3 forward = WorldDefaults.Forward;
            if (local)
                forward = Forward;
            Quaternion rot = Quaternion.FromAxisAngle(forward, radAngle);
            //Rotation = (_rotation * rot).Normalized();
            Rotation = (rot * _rotation).Normalized();
        }

        public void TranslateY(float distance)
        {
           Translation += new Vector3(0, distance, 0);
        }

        public void TranslateX(float distance)
        {
            Translation += new Vector3(distance, 0, 0);
        }

        public void TranslateZ(float distance)
        {
            Translation += new Vector3(0, 0, distance);
        }

        public virtual void LookAt(in Vector3 target)
        {
            Rotation = MathFuncs.Lookat(Translation, target);
            return;



            if ((target - _translation).LengthFast < 0.1f)
                return;
            Vector3 direction = (target - _translation).Normalized();
            Vector3 forward = Vector3.UnitZ;
            //compute rotation axis
            Vector3 rotAxis = Vector3.Cross(forward, direction);
            if (rotAxis.Length == 0)
                rotAxis = Vector3.UnitY;
            //making sure roataaxis is positive up
            // if(rotAxis.Y <0)
            //    rotAxis*=-1;
            //find the angle around rotation axis
            float dot = Vector3.Dot(direction, forward);
            float angle = MathF.Acos(dot);
            if (angle < 0.1f)
                return;

            // https://stackoverflow.com/questions/12435671/quaternion-lookat-function
            float s = MathF.Sin(angle * 0.5f);
            Vector3 u = rotAxis.Normalized();
            //convert axis angle to quaternion std::cosf(angle / 2), u.x() * s, u.y() * s, u.z() * s
            Rotation = new Quaternion(u.X * s, u.Y * s, u.Z * s, MathF.Cos(angle * .5f)); //Quaternion.FromAxisAngle(rotAxis, angle);

        }

        public virtual void HorizontalLookAt(in Vector3 target)
        {

        }

        public void Dispose()
        {
            foreach (SceneNode child in _childNodes)
            {
                child.Dispose();
            }
            onDestroy();
            GC.Collect();
            _disposed = true;
        }
    }
}