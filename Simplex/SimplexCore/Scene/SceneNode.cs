using OpenTK;
using Simplex.BVT;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simplex.Core.Scene
{
    /// <summary>
    /// Base class for a node in the 3D Scene
    /// </summary>
    public class SceneNode
    {

        #region Private Fields

        private BVTRoot behavior = new BVTRoot();
        private List<SceneNode> childNodes = new List<SceneNode>();
        private string name;
        private SceneNode parent;
        private Quaternion rotation = Quaternion.Identity;
        private Vector3 scale = new Vector3(1);
        private Vector3 translation = Vector3.Zero;
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
            onDestroy();
        }

        #endregion Private Destructors

        #region Public Properties

        public Quaternion Rotation { get => rotation; set => rotation = value; }
        public Vector3 Scale { get => scale; set => scale = value; }
        public Vector3 Translation { get => translation; set => translation = value; }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// gets called from the parent node when the node is added
        /// </summary>
        /// <param name="parent">the parent scene node</param>
        private void TreeEntered(SceneNode parent)
        {
            if (this.parent != null)
                onTreeMoved(this.parent, parent);
            this.parent = parent;
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
            this.childNodes.Add(child);
            child.TreeEntered(this);
        }

        /// <summary>
        /// finds a child node by its guid
        /// </summary>
        /// <param name="guid">guid of the childnode</param>
        /// <returns>the childnode,null if non found</returns>
        public SceneNode FindChild(Guid guid)
        {
            return this.childNodes.Find(x => x.uid == guid);
        }

        /// <summary>
        /// tries to find a child by its name
        /// </summary>
        /// <param name="name">the name of the node</param>
        /// <returns>the node if found,else null</returns>
        public SceneNode FindChild(string name)
        {
            return this.childNodes.Find(x => x.name == name);
        }

        public List<T> FindChildByType<T>(bool recursive = true)
        {
            List<T> found = new List<T>();
            List<T> own = childNodes.Where(x => x.GetType() == typeof(T)).ToList() as List<T>;
            if (own != null)
                found.AddRange(own);

            if (!recursive)
                return found;

            foreach (SceneNode child in childNodes)
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
            return childNodes.ToArray();
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
            SceneNode oldParent = this.parent;
            oldParent.childNodes.Remove(this);

            onTreeMoved(oldParent, newParent);
        }

        #endregion Public Methods

    }
}