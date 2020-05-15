using Simplex.BVT;
using System;
using System.Collections.Generic;
using System.Text;

namespace Simplex.Core.Scene
{
    public class SceneNode
    {
        List<SceneNode> childNodes = new List<SceneNode>();
        BVTRoot behavior = new BVTRoot();
        Transformation transform = new Transformation();
        SceneNode parent;
        Guid uid;
        string name;

        public SceneNode() {

            onCreate();
        }

         ~SceneNode()
        {
            onDestroy();
        }

        public void AddChild(SceneNode child)
        {
            this.childNodes.Add(child);
            child.TreeEntered(this);
        }

        private void TreeEntered(SceneNode parent)
        {
            if (this.parent != null)
                onTreeMoved(this.parent, parent);
            this.parent = parent;
            if(this.uid==null)
               this.uid = Guid.NewGuid();
            onTreeEntered();
        }

        public SceneNode FindChild(Guid cuid)
        {
            return this.childNodes.Find(x => x.uid == cuid);
        }

        public SceneNode FindChild(string name)
        {
            return this.childNodes.Find(x => x.name == name);
        }

        public SceneNode[] GetChildren()
        {
            return childNodes.ToArray();
        }

        protected virtual void onTreeEntered()
        {
            
        }

        public void TreeMoved(SceneNode newParent)
        {
            SceneNode oldParent = this.parent;
            oldParent.childNodes.Remove(this);

            onTreeMoved(oldParent,newParent);
        }

        protected virtual void onTreeMoved(SceneNode oldParent, SceneNode newParent)
        {
            
        }

        public void TreeExited()
        {
            onTreeExited();
        }

        protected virtual void onTreeExited()
        {
        }

        protected virtual void onCreate() { }
        protected virtual void onDestroy() { }
        protected virtual void onTick(float delta) { }
    }
}
