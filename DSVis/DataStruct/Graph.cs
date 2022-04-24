using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace DSVis.DataStruct {
    public class Node<T> {//顶点
        private T data;
        private double x, y;
        private ArrayList edges = new ArrayList();
        public Node(T data, double x, double y) {
            this.data = data;
            this.x = x;
            this.y = y;
        }
        public Node() {
            this.x = -1;
            this.y = -1;
        }
        public double X { get => x; set => x = value; }
        public double Y { get => y; set => y = value; }
        public T Data { get => data; set => data = value; }
        public ArrayList Edges { get => edges; set => edges = value; }

        public bool FindNode(T data) {
            if (data.Equals(this.data)) {
                return true;
            } else {
                return false;
            }
        }
        public Node<T> GetNode(T data) {
            if (data.Equals(this.data)) {
                return this;
            } else {
                return null;
            }
        }
    }
    public class EdgeNode<T> {//图邻接表
        private int adjvertex;//邻接顶点
        private int mark;//标号
        private int weight;//权值
        private EdgeNode<T> next;//下一个邻接表结点
        public int Adjvertex { get => adjvertex; set => adjvertex = value; }
        public int Weight { get => weight; set => weight = value; }
        public EdgeNode<T> Next { get => next; set => next = value; }
        public int Mark { get => mark; set => mark = value; }

        public EdgeNode(int vex) {
            adjvertex = vex;
            next = null;
            weight = -1;
        }
    }
    public class VertexNode<T> {//图邻接表顶点
        private Node<T> data;//图的顶点
        private EdgeNode<T> firstedge;//邻接表的第一个结点
        private int indegree = 0;//入度
        private int delMark = 0;//删除标记
        private int earlytime = 0;
        private int latetime = 0;

        public int Indegree { get => indegree; set => indegree = value; }
        public Node<T> Data { get => data; set => data = value; }
        public EdgeNode<T> Firstedge { get => firstedge; set => firstedge = value; }
        public int DelMark { get => delMark; set => delMark = value; }
        public int Earlytime { get => earlytime; set => earlytime = value; }
        public int Latetime { get => latetime; set => latetime = value; }

        public VertexNode() {
            data = null;
            firstedge = null;
        }
        public VertexNode(Node<T> nd) {
            data = nd;
            firstedge = null;
        }
        public VertexNode(Node<T> nd, EdgeNode<T> en) {
            data = nd;
            firstedge = en;
        }   
    }
    public class ALGraph<T>{//无向图邻接表
        private VertexNode<T>[] adjlist;
        private int[] visited;
        private ArrayList VertexVisit=new ArrayList();
        private int edgeNum=0;
        private List<int> topoSort = new List<int>();

        public List<int> TopoSort { get => topoSort; set => topoSort = value; }

        public VertexNode<T> this[int index] { get => adjlist[index]; set => adjlist[index] = value; }
        public ALGraph(Node<T>[] nodes) {
            adjlist = new VertexNode<T>[nodes.Length];
            for (int i = 0; i < nodes.Length; i++) {
                adjlist[i] = new VertexNode<T>();
                adjlist[i].Data = nodes[i];
                adjlist[i].Firstedge = null;
            }
            visited = new int[adjlist.Length];
            for (int i = 0; i < visited.Length; i++){
                visited[i] = 0;
            }
        }
        public int GetEdgeNum() {
            return edgeNum;
        }
        public int GetVertexNum() {
            return adjlist.Length;
        }
        public bool IsEdge(Node<T> v1, Node<T> v2) {
            if (!IsVertex(v1) || !IsVertex(v2) || v1==v2 ) {
                return false;
            }
            EdgeNode<T> p = adjlist[GetIndex(v1)].Firstedge;
            while (p != null) {
                if (p.Adjvertex == GetIndex(v2)) {
                    return true;
                }
                p = p.Next;
            }
            return false;
        }
        public bool SetUdEdge(Node<T> v1, Node<T> v2) {
            if (IsEdge(v1, v2)) {
                return false;
            }
            EdgeNode<T> p = new EdgeNode<T>(GetIndex(v2));
            if (adjlist[GetIndex(v1)].Firstedge == null) {
                adjlist[GetIndex(v1)].Firstedge = p;
            } else {
                p.Next = adjlist[GetIndex(v1)].Firstedge;
                adjlist[GetIndex(v1)].Firstedge = p;
            }
            p = new EdgeNode<T>(GetIndex(v1));
            if (adjlist[GetIndex(v2)].Firstedge == null) {
                adjlist[GetIndex(v2)].Firstedge = p;
            } else {
                p.Next = adjlist[GetIndex(v2)].Firstedge;
                adjlist[GetIndex(v2)].Firstedge = p;
            }
            edgeNum++;
            return true;
        }
        public bool SetDEdge(Node<T> v1, Node<T> v2) {
            if (IsEdge(v1, v2)) {
                return false;
            }
            EdgeNode<T> p = new EdgeNode<T>(GetIndex(v2));
            if (adjlist[GetIndex(v1)].Firstedge == null) {
                adjlist[GetIndex(v1)].Firstedge = p;
            } else {
                p.Next = adjlist[GetIndex(v1)].Firstedge;
                adjlist[GetIndex(v1)].Firstedge = p;
            }
            edgeNum++;
            adjlist[GetIndex(v2)].Indegree++;
            return true;
        }
        public bool SetDEdge(Node<T> v1, Node<T> v2,int mark) {
            if (IsEdge(v1, v2)) {
                return false;
            }
            EdgeNode<T> p = new EdgeNode<T>(GetIndex(v2));
            p.Mark = mark;
            if (adjlist[GetIndex(v1)].Firstedge == null) {
                adjlist[GetIndex(v1)].Firstedge = p;
            } else {
                p.Next = adjlist[GetIndex(v1)].Firstedge;
                adjlist[GetIndex(v1)].Firstedge = p;
            }
            edgeNum++;
            adjlist[GetIndex(v2)].Indegree++;
            return true;
        }
        public bool IsVertex(Node<T> v) {
            foreach (VertexNode<T> vn in adjlist) {
                if (v.Equals(vn.Data))
                    return true;
            }
            return false;
        }
        public int GetIndex(Node<T> v) {
            int i;
            for (i = 0; i < adjlist.Length; i++) {
                if (adjlist[i].Data.Equals(v))
                    return i;
            }
            return i;
        }
        public void DFS() {
            for(int i = 0; i < visited.Length; i++) {
                if (visited[i] == 0) {
                    DFStraverse(i);
                }
            }
        }
        public void DFStraverse(int i) {
            visited[i] = 1;
            VertexVisit.Add(i);
            EdgeNode<T> p = adjlist[i].Firstedge;
            while (p != null) {
                if (visited[p.Adjvertex] == 0) {
                    DFStraverse(p.Adjvertex);                
                }
                p = p.Next;
            }
        }
        public void BFS() {
            for (int i = 0; i < visited.Length; i++) {
                if (visited[i] == 0) {
                    BFStraverse(i); 
                }
            }
        }
        public void BFStraverse(int i) {
            VertexVisit.Add(i);
            visited[i] = 1;
            Queue<int> queue = new Queue<int>(visited.Length);
            queue.Enqueue(i);
            while (queue.Count!=0) {
                int k = queue.Dequeue();
                EdgeNode<T> p = adjlist[k].Firstedge;
                while (p != null) {
                    if (visited[p.Adjvertex] == 0) {
                        visited[p.Adjvertex] = 1;
                        VertexVisit.Add(p.Adjvertex);
                        queue.Enqueue(p.Adjvertex);
                    }
                    p = p.Next;
                }
            }
        }
        public ArrayList GetVisited(int i) {
            if (i == 0) {
                DFS();
                return VertexVisit;
            } else {
                BFS();
                return VertexVisit;
            }
        }
        public void DelGraph() {
            for (int i = 0; i < visited.Length; i++) {
                visited[i] = 0;
            }
            VertexVisit.Clear();
            for (int i = 0; i < adjlist.Length; i++) {
                adjlist[i] = new VertexNode<T>();
            }
        }
        public void InitVisited() {
            for (int i = 0; i < visited.Length; i++) {
                visited[i] = 0;
                adjlist[i].DelMark = 0;
            }
            VertexVisit.Clear();
            InitIndegree();
        }
        public int getTopoStart() {
            UpdateTopoStart();
            for(int i = 0; i < visited.Length; i++) {
                if (adjlist[i].Indegree == 0 && adjlist[i].DelMark == 0) {
                    adjlist[i].DelMark = 1;
                    return i;
                }
            }
            return -1;
        }

        public void UpdateTopoStart() {
            for(int i = 0; i < visited.Length; i++) {
                if (adjlist[i].Indegree == 0 && adjlist[i].DelMark != 0 && !VertexVisit.Contains(i)) {
                    VertexVisit.Add(i);
                    EdgeNode<T> edge = adjlist[i].Firstedge;
                    while (edge != null) {
                        adjlist[edge.Adjvertex].Indegree--;
                        edge = edge.Next;
                    }
                }
            }
        }
        public void InitIndegree() {
            for (int i = 0; i < visited.Length; i++) {
                EdgeNode<T> edge = adjlist[i].Firstedge;
                while (edge != null) {
                    adjlist[edge.Adjvertex].Indegree++;
                    edge = edge.Next;
                }
            }
        }
        public void SetDEdgeWeight(Node<T> v1,Node<T> v2,int weight) {
            EdgeNode<T> e = adjlist[GetIndex(v1)].Firstedge;
            while (e != null && e.Adjvertex != GetIndex(v2)) {
                e = e.Next;
            }
            if (e != null && e.Adjvertex == GetIndex(v2)) {
                e.Weight = weight;
            }
        }
        public void getTopoSort() {
            for(int i = 0; i < adjlist.Length; i++) {
                topoSort.Add(getTopoStart());
            }
        }
        public void GetEarlyTime(int n) {
            adjlist[n].Earlytime = 0;
            int max = 0;
            for(int i = 0; i < adjlist.Length; i++) {
                EdgeNode<T> e = adjlist[i].Firstedge;
                while (e!=null&& e.Adjvertex != n) {
                    e = e.Next;
                }
                if (e != null) {
                    if (e.Weight + adjlist[i].Earlytime > max) {
                        max = e.Weight + adjlist[i].Earlytime;
                    }
                }
            }
            adjlist[n].Earlytime = max;
        }
        public void GetLateTime(int n) {
            int max = 0;
            for (int i = 0; i < adjlist.Length; i++) {
                if (adjlist[i].Firstedge == null && max < adjlist[i].Earlytime) {
                    max = adjlist[i].Earlytime;
                }
            }
            adjlist[n].Latetime = max;
            int min = max;
            EdgeNode<T> e = adjlist[n].Firstedge;
            while (e != null) {
                if (adjlist[e.Adjvertex].Earlytime - e.Weight < min) {
                    min = adjlist[e.Adjvertex].Earlytime - e.Weight;
                }
                e = e.Next;
            }
            adjlist[n].Latetime = min;
        }
    }
    public class ENode<T>: IComparable<ENode<T>> {//边
        private int weight;
        private Node<T> v1;
        private Node<T> v2;
        private int index;
        private int mark;
        public int Weight { get => weight; set => weight = value; }
        public Node<T> V1 { get => v1; set => v1 = value; }
        public Node<T> V2 { get => v2; set => v2 = value; }
        public int Index { get => index; set => index = value; }
        public int Mark { get => mark; set => mark = value; }

        public ENode() {
            this.v1 = null;
            this.v2 = null;
            this.weight = -1;
            this.index = -1;
            this.mark = 0;
        }
        public ENode(Node<T> v1, Node<T> v2,int index) {
            this.v1 = v1;
            this.v2 = v2;
            this.weight = -1;
            this.index = index;
            this.mark = 0;
        }
        public ENode(Node<T> v1, Node<T> v2,int weight,int index){
            this.v1=v1;
            this.v2=v2;
            this.weight = weight;
            this.index = index;
            this.mark = 0;
        }
        public int findEdge(int v1,int v2) {
            if ((this.V1.Data.Equals(v1) && this.V2.Data.Equals(v2))||(this.V1.Data.Equals(v2) && this.V2.Data.Equals(v1))) {
                return this.Index;
            } else {
                return -1;
            }
        }
        int IComparable<ENode<T>>.CompareTo(ENode<T> other) {
            int result;
            if (this.weight == other.Weight) {
                result = 0;
            } else if(this.weight >= other.Weight) {
                result = 1;
            } else {
                result = -1;
            }
            return result;
        }
    }
    public class ELGraph<T> {//边邻接表
        private int edgecount;
        Node<T>[] vertexs;
        ENode<T>[] edges;
        private ArrayList EdgeVisit = new ArrayList();
        public Node<T>[] Vertexs { get => vertexs; set => vertexs = value; }
        public ENode<T>[] Edges { get => edges; set => edges = value; }
        public ELGraph(Node<T>[] nodes) {
            vertexs = new Node<T>[nodes.Length];
            for (int i = 0; i < nodes.Length; i++) {
                vertexs[i] = new Node<T>(nodes[i].Data,nodes[i].X,nodes[i].Y);
            }
            edges = new ENode<T>[nodes.Length*nodes.Length];
            edgecount = 0;
        }
        public int GetVertexNum() {
            return Vertexs.Length;
        }
        public int GetEdgeNum() {
            int edgenum = 0;
            try {
                for (int i = 0; edges[i] != null; i++) {
                    edgenum++;
                }
            } catch {
            } 
            return edgenum;
        }
        public bool IsEdge(Node<T> v1, Node<T> v2) {
            if (!IsVertex(v1) || !IsVertex(v2) || v1 == v2) {
                return false;
            }
            foreach (ENode<T> en in edges) {
                if ((en.V1 == v1 && en.V2 == v2) || (en.V1 == v2 && en.V2 == v1))
                    return false;
            }
            return true;
        }
        public bool SetEdge(Node<T> v1, Node<T> v2) {
            if (IsEdge(v1, v2) == true) { 
                    return false;
            }
            ENode<T> e = new ENode<T>(v1, v2, edgecount);
            vertexs[(int)(object)v1.Data].Edges.Add(e);
            vertexs[(int)(object)v2.Data].Edges.Add(e);
            edges[edgecount] = e;
            edgecount++;
            return true;
        }
        public bool SetEdge(Node<T> v1, Node<T> v2, int weight) {
            if (IsEdge(v1, v2) == true) {
                return false;
            }
            ENode<T> e = new ENode<T>(v1, v2, edgecount);
            e.Weight = weight;
            vertexs[(int)(object)v1.Data].Edges.Add(e);
            vertexs[(int)(object)v2.Data].Edges.Add(e);
            edges[edgecount] = e;
            edgecount++;
            return true;
        }
        public bool SetWeight(int no,int weight) {
            if (no<0||no>edges.Length) {
                return false;
            }
            edges[no].Weight = weight;
            return true;
        }
        public bool IsVertex(Node<T> v) {
            foreach (Node<T> vn in vertexs) {
                if (v.Equals(vn))
                    return true;
            }
            return false;
        }
        public int GetVertexIndex(Node<T> v) {
            int i;
            for (i = 0; i < vertexs.Length; i++) {
                if (vertexs[i].Equals(v))
                    return i;
            }
            return i;
        }
        public int GetEdgeIndex(ENode<T> e) {
            int i;
            for (i = 0; i < this.GetEdgeNum(); i++) {
                if (edges[i].Equals(e))
                    return edges[i].Index;
            }
            return i;
        }
        public void DelGraph() {
            for (int i = 0; i < vertexs.Length; i++) {
                vertexs[i] = new Node<T>();  
            }
            for (int i = 0; i < this.GetEdgeNum(); i++) {
                edges[i] = new ENode<T>();
            }
            EdgeVisit.Clear();
            for (int i = 0; i < this.GetEdgeNum(); i++) {
                edges[i].Mark = 0;
            }
        }
        public void Prim() {
            int i, j, k;
            int add = 0;
            //int[] recV = new int[this.GetEdgeNum()];
            List<ENode<T>> recE = new List<ENode<T>>();
            List<Node<T>> recV = new List<Node<T>>();
            recV.Add(vertexs[0]);

            int CheckDup(ENode<T> e, int n) {
                int l;
                int flag1 = 0, flag2 = 0;
                if (e.Mark == 1)
                    return 1;
                for (l = 0; l < n; l++) {
                    if (e.V1.Data.Equals(recV[l].Data)) {
                        flag1++;
                        break;
                    }
                }
                for (l = 0; l < n; l++) {
                    if (e.V2.Data.Equals(recV[l].Data)) {
                        flag2++;
                        break;
                    }
                }
                if (flag1 == 1 && flag2 == 1)
                    return 1;
                else if (flag1 == 0 && flag2 != 0) {
                    add = (int)(object)e.V1.Data;
                    return 0;
                } else if (flag1 != 0 && flag2 == 0) {
                    add = (int)(object)e.V2.Data;
                    return 0;
                } else
                    return 0;
            }

            for (i = 0; i < vertexs.Length; i++) {
                recE.Clear();
                for (j = 0; j < i; j++) {
                    Node<T> v = recV[j] as Node<T>;
                    for (k = 0; k < vertexs[(int)(object)v.Data].Edges.Count; k++) {
                        ENode<T> e = vertexs[(int)(object)v.Data].Edges[k] as ENode<T>;
                        recE.Add(edges[e.Index]);
                    }
                }
                recE.Sort();
                foreach(Object o in recE){
                    ENode<T> e = o as ENode<T>;
                    if (CheckDup(e, recV.Count) == 0 && e.Mark != 1) {
                        e.Mark = 1;
                        EdgeVisit.Add(e.Index);
                        recV.Add(vertexs[add]);
                        break;
                    }
                }
            }
        }
        public void Kruskal() {
            int i, j = 0, k = 0, s1, s2;
            int[] f=new int[edges.Length];
            for (i = 0; i < edges.Length; i++) {
                f[i] = i;
            }
            List<ENode<T>> list = new List<ENode<T>>();
            for (i = 0; i < this.GetEdgeNum(); i++) {
                list.Add(edges[i]);
            }
            list.Sort();
            while (k < vertexs.Length - 1) {
                s1 = f[(int)(object)list[j].V1.Data];
                s2 = f[(int)(object)list[j].V2.Data];
                if (!s1.Equals(s2)) {
                    EdgeVisit.Add(list[j].Index);
                    k++;
                    for (i = 0; i < list.Count; i++) {
                        if (f[i] == s2)
                            f[i] = s1;
                    }
                }
                j++;
            }
        }
        public ArrayList GetVisited(int i) {
            if (i == 0) {
                Prim();
            } else {
                Kruskal();
            }
            return EdgeVisit;
        }
        public void InitVisited() {
            EdgeVisit.Clear();
            for(int i = 0; i < this.GetEdgeNum(); i++) {
                edges[i].Mark = 0;
            }
        }
    }
    public class DirAdjMatrix<T> {//有向图矩阵
        private Node<T>[] vertexs;
        private int[,] arcs;
        private int edgenum;
        private int vertexnum;
        private const int MaxVertex = 30;
        private const int INF = 30000;
        private int[] p = new int[30];
        private int[] d = new int[30];
        private ArrayList EdgeVisit = new ArrayList();
        public int Edgenum { get => edgenum; set => edgenum = value; }
        public int[] P { get => p; set => p = value; }
        public int[] D { get => d; set => d = value; }

        public DirAdjMatrix() {
            vertexs = new Node<T>[MaxVertex];
            arcs = new int[MaxVertex, MaxVertex];
            for(int i = 0; i < MaxVertex; i++) {
                for(int j = 0; j < MaxVertex; j++) {
                    arcs[i, j] = INF;
                }
            }
            edgenum = 0;
            vertexnum = 0;
        }
        public Node<T> GetNode(int index) {
            return vertexs[index];
        }
        public void SetNode(int index,Node<T> v) {
            vertexs[index] = v;
            vertexnum++;
        }
        public int GetMatrix(int index1,int index2) {
            return arcs[index1, index2];
        }
        public void SetMatrix(int index1,int index2,int v) {
            arcs[index1, index2] = v;
        }
        public int GetVertexNum() {
            return vertexnum;
        }
        public int GetEdgeNum() {
            return edgenum;
        }
        public bool IsVertex(Node<T> v) {
            foreach(Node<T> n in vertexs) {
                if (v.Equals(n)) {
                    return true;
                }
            }
            return false;
        }
        public int GetIndex(Node<T> v) {
            int i;
            for (i = 0; i < vertexs.Length; i++) {
                if (vertexs[i].Equals(v)) {
                    return i;
                }
            }
            return i;
        }
        public bool SetEdge(Node<T> v1,Node<T> v2) {
            if (!IsVertex(v1) || !IsVertex(v2)) {
                return false;
            }
            edgenum++;
            return true;
        }
        public bool SetWeight(Node<T> v1,Node<T> v2,int v) {
            if (!IsVertex(v1) || !IsVertex(v2)) {
                return false;
            }
            arcs[GetIndex(v1), GetIndex(v2)] = v;
            return true;
        }
        public bool IsEdge(Node<T> v1,Node<T> v2) {
            if (!IsVertex(v1) || !IsVertex(v2)) {
                return false;
            }
            if (arcs[GetIndex(v1), GetIndex(v2)] != int.MaxValue) {
                return true;
            } else {
                return false;
            }
        }
        public ArrayList Dijkstra12N(int v0) {
            int i, w, v;
            int min;
            bool[] final=new bool[MaxVertex];
            for (v = 0; v <= vertexnum - 1; v++) {
                final[v] = false;
                d[v] = arcs[v0, v];
                p[v] = -1;
                if (d[v] < INF)
                    p[v] = v0;
            }
            d[v0] = 0;
            final[v0] = true;
            for (i = 1; i <= vertexnum; i++) {
                v = -1;
                min = INF;
                for (w = 0; w <= vertexnum - 1; w++) {
                    if (!final[w] && (d[w] < min)) {
                        v = w;
                        min = d[w];
                    }
                }
                if (v == -1) {
                    break;
                }
                final[v] = true;
                for (w = 0; w <= vertexnum - 1; w++) {
                    if (!final[w] && (min + arcs[v, w] < d[w])) {
                        d[w] = min + arcs[v, w];
                        p[w] = v;
                    }
                }
            }
            for (v = 0; v <= vertexnum - 1; v++) {
                if (p[v] == -1)
                    continue;
                EdgeVisit.Add(-1);
                EdgeVisit.Add(v);
                i = v;
                while (p[i] != -1) {
                    EdgeVisit.Add(p[i]);
                    i = p[i];
                }
            }
            return EdgeVisit;
        }
        public ArrayList Dijkstra(int v0) {
            ArrayList VertMark = new ArrayList();
            VertMark.Add(v0);
            int min;
            int v1 = -1, v2 = -1;
            for (int i = 0; i < vertexnum; i++) {
                min = INF;
                foreach (int m in VertMark) {
                    for (int j = 0; j < vertexnum; j++) {
                        if (arcs[m, j] < min && !VertMark.Contains(j) && !EdgeVisit.Contains(new Tuple<int, int>(m, j))) {
                            min = arcs[m, j];
                            v1 = m;
                            v2 = j;
                        }
                    }
                }
                if (!VertMark.Contains(v2) && !EdgeVisit.Contains(new Tuple<int, int>(v1, v2)) && v2!=-1) {
                    EdgeVisit.Add(new Tuple<int, int>(v1, v2));
                    VertMark.Add(v2);
                }
            }
            return EdgeVisit;
        }
        public void DelGraph() {
            for (int i = 0; i < vertexs.Length; i++) {
                vertexs[i] = new Node<T>();
            }
            for (int i = 0; i < 30; i++) {
                for(int j = 0; j < 30; j++) {
                    arcs[i,j] = INF;
                }
            }
            edgenum = 0;
            vertexnum = 0;
            InitVisited();
        }
        public void InitVisited() {
            for(int i = 0; i < MaxVertex; i++) {
                d[i] = -1;
                p[i] = -1;
            }
            EdgeVisit.Clear();
        }
    }


}
