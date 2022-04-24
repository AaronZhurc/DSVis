using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSVis.DataStruct {
    public class HNode : IComparable<HNode>, ICloneable {
        char name;
        int weight,mark;
        HNode lchild, rchild, parent;
        string code;
        double x, y;

        public char Name { get => name; set => name = value; }
        public int Weight { get => weight; set => weight = value; }
        public HNode LChild { get => lchild; set => lchild = value; }
        public HNode RChild { get => rchild; set => rchild = value; }
        public HNode Parent { get => parent; set => parent = value; }
        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public string Code { get => code; set => code = value; }
        public int Mark { get => mark; set => mark = value; }

        public HNode() {
            this.name = '\0';
            this.weight = -1;
            this.lchild = null;
            this.rchild = null;
            this.parent = null;
            this.code = "\0";
            this.x = -1;
            this.y = -1;
            this.mark = 0;
        }
        public HNode(char name, int weight, double x = 0, double y = 0) {
            this.name = name;
            this.weight = weight;
            this.lchild = null;
            this.rchild = null;
            this.parent = null;
            this.code = "\0";
            this.x = x;
            this.y = y;
            this.mark = 0;
        }
        public int CompareTo(HNode other) {
            int result;
            if (this.weight == other.Weight) {
                result = 0;
            } else if (this.weight >= other.Weight) {
                result = 1;
            } else {
                result = -1;
            }
            return result;
        }

        public object Clone() {
            return this.MemberwiseClone();
        }

        public bool FindNode(char name) {
            if (name == this.name) {
                return true;
            } else {
                return false;
            }
        }

        public bool TestLoop(char name) {
            bool p=false, l=false, r=false;
            if (name!=this.name) {
                if (this.parent == null && this.lchild == null && this.rchild == null) {
                    return false;
                }
                if (this.parent != null) {
                    p=this.parent.TestLoop(name);
                }
                if (this.lchild != null) {
                    l=this.lchild.TestLoop(name);
                }
                if (this.rchild != null) {
                    r=this.rchild.TestLoop(name);
                }
                return p || l || r;
            } else {
                return true;
            }
        }
    }
    public class HTree {
        private List<HNode> data;
        private List<HNode> oridata;
        private ArrayList code;
        private int length;
        public HNode this[int index] {
            get => (HNode)data[index];
            set => data[index] = value;
        }
        public int Length { get => length; set => length = value; }
        public ArrayList Code { get => code; set => code = value; }

        public HTree() {
            data = new List<HNode>();
            oridata = new List<HNode>();
            code = new ArrayList();
            length = 0;
        }
        public bool IsLeaf(HNode node) {
            foreach(HNode n in oridata) {
                if (n.Equals(node)) {
                    return true;
                }
            }
            return false;
        }
        public bool SetLeaf(HNode node) {
            if (IsLeaf(node) == true) {
                return false;
            }
            data.Add(node);
            oridata.Add(node.Clone() as HNode);
            return true;
        }
        public int GetLeafNum() {
            return data.Count;
        }
        public void Sort() => data.Sort();
        public void Huffman() {
            int i = 0;
            if (data.Count >= 2) {
                HNode pa = new HNode();
                pa.Weight = data[0].Weight + data[1].Weight;
                pa.LChild = data[0];
                pa.RChild = data[1];
                pa.X = (data[0].X + data[1].X) / 2;
                pa.Y = data[0].Y;
                data[0].Y += 0.08;
                data[0].Parent = pa;
                SetNewY(data[0]);
                data[1].Y += 0.08;
                data[1].Parent = pa;
                SetNewY(data[1]);
                data.RemoveAt(0);
                data.RemoveAt(0);
                data.Add(pa);
                foreach(HNode node in data) {
                    i = SetNewX(node, i);
                }
            }
        }
        public void SetNewY(HNode node) {
            if (node.LChild != null) {
                node.LChild.Y += 0.1;
                SetNewY(node.LChild);
            }
            if (node.RChild != null) {
                node.RChild.Y += 0.1;
                SetNewY(node.RChild);
            }
        }
        public int SetNewX(HNode node, int i) {
            if (node.LChild == null && node.RChild == null) {
                node.X = 0.16 + 0.08 * i;
                return ++i;
            }
            i = SetNewX(node.LChild, i);
            i = SetNewX(node.RChild, i);
            node.X = (node.LChild.X + node.RChild.X) / 2;
            return i;
        }
        public void BackUp() {
            data.Clear();
            foreach(HNode node in oridata) {
                data.Add(node.Clone() as HNode);
            }
        }
        public void SetCode(HNode node) {
            if (node.LChild != null && node.RChild != null) {
                node.LChild.Code = node.Code + "0";
                node.RChild.Code = node.Code + "1";
                SetCode(node.LChild);
                SetCode(node.RChild);
            } else {
                code.Add(node);
            }
        }
    }
    public class TNode<T>{
        T name;
        int mark,height;
        TNode<T> parent,lchild,rchild;
        int ltag, rtag;
        private ArrayList vertexVisit = new ArrayList();
        double x, y;

        public TNode<T> Parent { get => parent; set => parent = value; }
        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public int Mark { get => mark; set => mark = value; }
        public TNode<T> Lchild { get => lchild; set => lchild = value; }
        public TNode<T> Rchild { get => rchild; set => rchild = value; }
        public T Name { get => name; set => name = value; }
        public int Height { get => height; set => height = value; }
        public ArrayList VertexVisit { get => vertexVisit; set => vertexVisit = value; }
        public int Ltag { get => ltag; set => ltag = value; }
        public int Rtag { get => rtag; set => rtag = value; }

        public TNode() {
            this.parent = null;
            this.lchild = null;
            this.rchild = null;
            this.x = -1;
            this.y = -1;
            this.ltag = 0;
            this.rtag = 0;
            this.mark = 0;
            this.Height = 0;
        }
        public TNode(T name, double x = 0, double y = 0) {
            this.name = name;
            this.parent = null;
            this.lchild = null;
            this.rchild = null;
            this.ltag = 0;
            this.rtag = 0;
            this.x = x;
            this.y = y;
            this.mark = 0;
            this.Height = 0;
        }
        public bool FindNode(T name) {
            if (name.Equals(this.name)) {
                return true;
            } else {
                return false;
            }
        }

        public bool TestLoopE(T name) {
            bool p = false, l = false, r = false;
            if (!name.ToString().Equals(this.name.ToString())) {
                if (this.parent == null && this.lchild == null && this.rchild == null) {
                    return false;
                }
                if (this.parent != null) {
                    p = this.parent.TestLoop(this.name, name);
                }
                if (this.lchild != null) {
                    l = this.lchild.TestLoop(this.name, name);
                }
                if (this.rchild != null) {
                    r = this.rchild.TestLoop(this.name, name);
                }
                return p || l || r;
            } else {
                return true;
            }
        }

        public bool TestLoop(T from,T name) {
            bool p = false, l = false, r = false;
            if (!name.ToString().Equals(this.name.ToString())) {
                if (this.parent == null && this.lchild == null && this.rchild == null && !this.name.ToString().Equals(from.ToString())) {
                    return false;
                }
                if (this.parent != null && !this.parent.name.ToString().Equals(from.ToString())) {
                    p = this.parent.TestLoop(this.name, name);
                }
                if (this.lchild != null && !this.lchild.name.ToString().Equals(from.ToString())) {
                    l = this.lchild.TestLoop(this.name, name);
                }
                if (this.rchild != null && !this.rchild.name.ToString().Equals(from.ToString())) {
                    r = this.rchild.TestLoop(this.name, name);
                }
                return p || l || r;
            } else {
                return true;
            }
        }

        public bool SetEdge(TNode<T> child) {
            if (this == child) {
                return false;
            } else if (child.y >= this.y) {
                if (child.x < this.x && this.lchild == null) {
                    this.lchild = child;
                    this.ltag = 0;
                    child.parent = this;
                    return true;
                } else if (child.x >= this.x && this.rchild == null) {
                    this.rchild = child;
                    this.rtag = 0;
                    child.parent = this;
                    return true;
                } else if (child.x < this.x && this.lchild != null) {
                    if (child.x < this.lchild.x) {
                        this.rchild = this.lchild;
                        this.lchild = child;
                        this.ltag = 0;
                        child.parent = this;
                        return true;
                    } else {
                        this.rchild = child;
                        this.rtag = 0;
                        child.parent = this;
                        return true;
                    }
                } else if (child.x >= this.x && this.rchild != null) {
                    if (child.x > this.rchild.x) {
                        this.lchild = this.rchild;
                        this.rchild = child;
                        this.rtag = 0;
                        child.parent = this;
                        return true;
                    } else {
                        this.lchild = child;
                        this.ltag = 0;
                        child.parent = this;
                        return true;
                    }
                } else {
                    this.rchild = child;
                    this.rtag = 0;
                    child.parent = this;
                    return true;
                }
            } else {
                return child.SetEdge(this);
            }
        }
        public ArrayList GetVisited(int i) {
            if (i == 0) {
                PreOrder(this);
                return VertexVisit;
            } else if (i == 1) {
                InOrder(this);
                return VertexVisit;
            } else {
                PostOrder(this);
                return VertexVisit;
            }
        }
        public void PreOrder(TNode<T> t) {
            if (t!=null) {
                VertexVisit.Add(char.Parse(t.name.ToString()) - 'A');
                if (t.ltag == 0) {
                    PreOrder(t.lchild);
                }
                if (t.rtag == 0) {
                    PreOrder(t.rchild);
                }
            }
        }
        public void InOrder(TNode<T> t) {
            if (t != null) {
                if (t.ltag == 0) {
                    InOrder(t.lchild);
                }
                VertexVisit.Add(char.Parse(t.name.ToString()) - 'A');
                if (t.rtag == 0) {
                    InOrder(t.rchild);
                }
            }
        }

        public void InThread(ref TNode<T> p, ref TNode<T> pre) {
            if (p != null) {
                if (p.ltag == 0) {
                    InThread(ref p.lchild, ref pre);
                }
                if (p.lchild == null) {
                    p.lchild = pre;
                    p.ltag = 1;
                }
                if (pre != null && pre.rchild == null) {
                    pre.rchild = p;
                    pre.rtag = 1;
                }
                pre = p;
                if (p.rtag == 0) {
                    InThread(ref p.rchild, ref pre);
                }
            }
        }
        public void PreThread(ref TNode<T> p, ref TNode<T> pre) {
            if (p != null) {
                if (p.lchild == null) {
                    p.lchild = pre;
                    p.ltag = 1;
                }
                if (pre != null && pre.rchild == null) {
                    pre.rchild = p;
                    pre.rtag = 1;
                }
                pre = p;
                if (p.ltag == 0) {
                    PreThread(ref p.lchild, ref pre);
                }
                if (p.rtag == 0) {
                    PreThread(ref p.rchild, ref pre);
                }
            }
        }
        public void PostThread(ref TNode<T> p, ref TNode<T> pre) {
            if (p != null) {
                if (p.ltag == 0) {
                    PostThread(ref p.lchild, ref pre);
                }
                if (p.rtag == 0) {
                    PostThread(ref p.rchild, ref pre);
                }
                if (p.lchild == null) {
                    p.lchild = pre;
                    p.ltag = 1;
                }
                if (pre != null && pre.rchild == null) {
                    pre.rchild = p;
                    pre.rtag = 1;
                }
                pre = p;
            }
        }
        public void CreateThread(ref TNode<T> t,int type) {
            TNode<T> pre = null;
            if (type == 1) {
                PreThread(ref t, ref pre);
            } else if (type == 2) {
                InThread(ref t, ref pre);
            } else if (type == 3) {
                PostThread(ref t, ref pre);
            }
        }
        public void ClearThread() {
            Queue<TNode<T>> queue = new Queue<TNode<T>>();
            queue.Enqueue(this);
            while (queue.Count != 0) {
                int size = queue.Count;
                for (int i = 0; i < size; i++) {
                    TNode<T> poll = queue.Dequeue();
                    if (poll.ltag == 1) {
                        poll.ltag = 0;
                        poll.lchild = null;
                    }
                    if (poll.rtag == 1) {
                        poll.rtag = 0;
                        poll.rchild = null;
                    }
                    if (poll.lchild != null) {
                        queue.Enqueue(poll.lchild);
                    }
                    if (poll.rchild != null) {
                        queue.Enqueue(poll.rchild);
                    }
                }
            }
        }
        public void PostOrder(TNode<T> t) {
            if (t != null) {
                if (t.ltag == 0) {
                    PostOrder(t.lchild);
                }
                if (t.rtag == 0) {
                    PostOrder(t.rchild);
                }
                VertexVisit.Add(char.Parse(t.name.ToString()) - 'A');
            }
        }
        public void LevelOrder() {
            VertexVisit.Clear();
            Queue<TNode<T>> queue = new Queue<TNode<T>>();
            queue.Enqueue(this);
            while (queue.Count != 0) {
                int size = queue.Count;
                for(int i = 0; i < size; i++) {
                    TNode<T> poll = queue.Dequeue();
                    VertexVisit.Add(poll);
                    if (poll.lchild != null) {
                        queue.Enqueue(poll.lchild);
                    }
                    if (poll.rchild != null) {
                        queue.Enqueue(poll.rchild);
                    }
                }
            }
        }
        public int Depth() {
            int depth = 1;
            TNode<T> t = this;
            while (t.Parent!=null) {
                t = t.Parent;
                depth++;
            }
            return depth;
        }
        public void GetBfE() {
            GetHeight(this);
            GetBf(this);
        }
        public void GetBf(TNode<T> t) {
            if (t != null) {
                GetBf(t.lchild);
                GetBf(t.rchild);
                if (t.lchild == null) {
                    if (t.rchild != null) {
                        t.mark = - t.rchild.Height;
                    } else {
                        t.mark = 0;
                    }
                } else {
                    if (t.rchild == null) {
                        t.mark = t.lchild.Height;
                    } else {
                        t.mark = t.lchild.Height - t.rchild.Height;
                    }
                }
            }
        }
        public void GetHeight(TNode<T> t) {
            if (t != null) {
                GetHeight(t.lchild);
                GetHeight(t.rchild);
                if (t.lchild == null) {
                    if (t.rchild == null) {
                        t.Height = 1;
                    } else {
                        t.Height = t.rchild.Height + 1;
                    }
                } else {
                    if (t.rchild == null) {
                        t.Height = t.lchild.Height + 1;
                    } else {
                        if (t.lchild.Height >= t.rchild.Height) {
                            t.Height = t.lchild.Height + 1;
                        } else {
                            t.Height = t.rchild.Height + 1;
                        }
                    }
                }
            }
        }
        public TNode<T> getUnbalancedE() {
            return getUnbalanced(this);
        }
        public TNode<T> getUnbalanced(TNode<T> t) {
            TNode<T> lt, rt;
            if (t != null) {
                lt=getUnbalanced(t.lchild);
                rt=getUnbalanced(t.rchild);
                if (lt != null) {
                    return lt;
                }
                if (rt != null) {
                    return rt;
                }
                if (t.mark == 2 || t.mark == -2) {
                    return t;
                }
                return null;
            } else {
                return null;
            }
        }

        public bool IsMaxHeap() {
            bool flag = true;
            Queue<TNode<T>> queue = new Queue<TNode<T>>();
            queue.Enqueue(this);
            while (queue.Count != 0) {
                int size = queue.Count;
                for (int i = 0; i < size; i++) {
                    TNode<T> poll = queue.Dequeue();
                    if (poll.lchild != null && poll.rchild != null) {
                        if (int.Parse(poll.lchild.Name.ToString()) <= int.Parse(poll.Name.ToString()) && int.Parse(poll.rchild.Name.ToString()) <= int.Parse(poll.Name.ToString())) {
                            flag &= true;
                        } else {
                            flag &= false;
                        }
                    } else if (poll.lchild != null && poll.rchild == null) {
                        if (int.Parse(poll.lchild.Name.ToString()) <= int.Parse(poll.Name.ToString())) {
                            flag &= true;
                        } else {
                            flag &= false;
                        }
                    } else {
                        flag &= true;
                    }
                    if (poll.lchild != null) {
                        queue.Enqueue(poll.lchild);
                    }
                    if (poll.rchild != null) {
                        queue.Enqueue(poll.rchild);
                    }
                }
            }
            return flag;
        }
        public bool IsMinHeap() {
            bool flag = true;
            Queue<TNode<T>> queue = new Queue<TNode<T>>();
            queue.Enqueue(this);
            while (queue.Count != 0) {
                int size = queue.Count;
                for (int i = 0; i < size; i++) {
                    TNode<T> poll = queue.Dequeue();
                    if (poll.lchild != null && poll.rchild != null) {
                        if (int.Parse(poll.lchild.Name.ToString()) >= int.Parse(poll.Name.ToString()) && int.Parse(poll.lchild.Name.ToString()) >= int.Parse(poll.Name.ToString())) {
                            flag &= true;
                        } else {
                            flag &= false;
                        }
                    } else if (poll.lchild != null && poll.rchild == null) {
                        if (int.Parse(poll.lchild.Name.ToString()) >= int.Parse(poll.Name.ToString())) {
                            flag &= true;
                        } else {
                            flag &= false;
                        }
                    } else {
                        flag &= true;
                    }
                    if (poll.lchild != null) {
                        queue.Enqueue(poll.lchild);
                    }
                    if (poll.rchild != null) {
                        queue.Enqueue(poll.rchild);
                    }
                }
            }
            return flag;
        }
    }
}
