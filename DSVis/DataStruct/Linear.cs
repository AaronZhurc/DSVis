using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSVis.DataStruct {
    class Circular {
        Circular next;
        int mark;
        int name;
        public Circular() {
            this.name = 0;
            this.mark = 0;
            this.next = this;
        }
        public Circular(int name) {
            this.name = name;
            this.mark = 0;
            this.next = this;
        }
        public int Mark { get => mark; set => mark = value; }
        public int Name { get => name; set => name = value; }
        internal Circular Next { get => next; set => next = value; }
        public void AddNode(Circular next) {
            next.next = this.next;
            this.next = next;
        }
        public List<int> JosephusStart(int start,int gap,int num) {
            Circular p = this;
            while (start--!=0) {
                p = p.next;
            }
            return p.Josephus(gap,num);
        }
        public List<int> Josephus(int gap,int num) {
            Circular p = this;
            List<int> result=new List<int>();
            int count = 0;
            while (count!=num) {
                for(int c = 0; c < gap; c++) {
                    p = p.next;
                }
                while (p.mark == 1) {
                    p = p.next;
                }
                result.Add(p.name);
                p.mark = 1;
                count++;
            }
            return result;
        }
    }
    class ListArray {
        List<int> array;
        List<int> backup;
        List<int> sorted;
        List<int> sorting;
        int bloop, sloop;
        int swapa, swapb;
        bool over;

        public ListArray() {
            array = new List<int>();
            backup = new List<int>();
            sorted = new List<int>();
            sorting = new List<int>();
            bloop = 0;
            sloop = 0;
            over = false;
        }
        public List<int> Array { get => array; set => array = value; }
        public bool Over { get => over; set => over = value; }
        public List<int> Sorted { get => sorted; set => sorted = value; }
        public int Sloop { get => sloop; set => sloop = value; }
        public List<int> Sorting { get => sorting; set => sorting = value; }
        public int Bloop { get => bloop; set => bloop = value; }
        public int Swapa { get => swapa; set => swapa = value; }
        public int Swapb { get => swapb; set => swapb = value; }

        public int this[int index] { get => array[index]; set => array[index] = value; }

        public void setArray(List<int> value) {
            array = value;
            backup.Clear();
            sorted.Clear();
            sorting.Clear();
            for(int i = 0; i < value.Count; i++) {
                backup.Add(value[i]);
                sorted.Add(value[i]);
                sorting.Add(0);
            }
        }
        public void swap(int x,int y) {
            int temp;
            temp = array[x];
            array[x] = array[y];
            array[y] = temp;
            swapa = x;
            swapb = y;
        }
        //冒泡
        public void BubbleSort() {
            if (over == false) {
                if (sloop >= array.Count - 1 - bloop && bloop < array.Count) {
                    bloop++;
                    sloop = 0;
                }
                while (sloop + 1 < array.Count && array[sloop] <= array[sloop + 1]) {
                    sloop++;
                }
                if (sloop + 1 < array.Count && array[sloop] > array[sloop + 1]) {
                    swap(sloop, sloop + 1);
                    sloop++;
                }
                SortOver();
            }
        }
        //选择
        public void SelectionSort() {
            if (over == false) {
                if (sloop >= array.Count && bloop < array.Count - 1) {
                    bloop++;
                    sloop = bloop + 1;
                }
                int min = bloop;
                while (sloop < array.Count) {
                    if (array[sloop] < array[min]) {
                        min = sloop;
                    }
                    sloop++;
                }
                swap(min, bloop);
                SortOver();
            }
        }
        //插入
        public void InsertionSort() {
            if (over == false) {
                if (bloop < array.Count) {
                    bloop++;
                    sloop = bloop - 1;
                }
                int temp = array[bloop];
                while (sloop >= 0) {
                    if (array[sloop] > temp) {
                        array[sloop + 1] = array[sloop];
                        array[sloop] = temp;
                    } else {
                        break;
                    }
                    sloop--;
                }
                swapa = sloop + 1;
                SortOver();
            }
        } 
        //希尔
        public void ShellSort(int gap,int i) {
            if (over == false) {
                List<int> list = new List<int>();
                for (int j = i; j < array.Count; j += gap) {
                    list.Add(array[j]);
                }
                list.Sort();
                int k = 0;
                for (int j = i; j < array.Count; j += gap) {
                    array[j] = list[k++];
                }
                list.Clear();
                SortOver();
            }
        }
        //快排
        public bool QuickSort(int pivot, ref int high, ref int low, ref bool dir) {
            if (over == false) {
                if (low < high) {
                    if (dir == true) {//小的换到低端
                        while (low < high && array[high] >= pivot) {
                            high--;
                        }
                        array[low] = array[high];
                        if (low == high) {
                            array[high] = pivot;
                            SortOver();
                            return true;//本步排序执行结束
                        }
                        return false;//本步排序未执行结束
                    } else {//大的换到高端
                        while (low < high && array[low] <= pivot) {
                            low++;
                        }
                        array[high] = array[low];
                        if (low == high) {
                            array[low] = pivot;
                            SortOver();
                            return true;
                        }
                        return false;
                    }
                } else {
                    return true;
                }
            } else {
                return true;
            }
        }
        // 二路归并
        public void MergeSort(int count) {
            List<int> sub = new List<int>();
            for (int i = 0; i < array.Count; i += count) {
                sub.Clear();
                for(int j = i; j < i + count; j++) {
                    if (j >= array.Count) {
                        break;
                    }
                    sub.Add(array[j]);
                }
                sub.Sort();
                for (int j = i; j < i + count; j++) {
                    if (j >= array.Count) {
                        break;
                    }
                    array[j] = sub[j - i];
                }
            }
            SortOver();
        }
        public void SortOver() {
            int flag=0;
            for(int i = 0; i < array.Count; i++) {
                if (sorted[i] != array[i]) {
                    flag = 1;
                    sorting[i] = 0;
                } else
                    sorting[i] = 1;
            }
            if (flag == 0) {
                over = true;
            }
        }
        public void GetSorted() {
            sorted.Sort();
        }
        public void Clear() {
            for (int i = 0; i < array.Count; i++) {
                array[i]=backup[i];
                sorting[i] = 0;
            }
            sloop = 0;
            bloop = 0;
            swapa = -1;
            swapb = -1;
            over = false;
        }
    }
    class PivotList {
        private int pivot;
        private int piloc;
        private List<int> subList;

        public PivotList() {
            pivot = -1;
            piloc = -1;
            subList = new List<int>();
        }

        public int Pivot { get => pivot; set => pivot = value; }
        public int Piloc { get => piloc; set => piloc = value; }
        public List<int> SubList { get => subList; set => subList = value; }
    }
    class Maze {
        int[,] maze;
        int x, y;

        public int X { get => x; set => x = value; }
        public int Y { get => y; set => y = value; }

        public int this[int x,int y] {
            get => maze[x, y];
            set => maze[x, y] = value;
        }
        public Maze() {//全随机生成迷宫
            Random random = new Random();
            X = 2 * random.Next(3, 5) + 1;
            Y = 2 * random.Next(3, 5) + 1;
            CreateMaze(X, Y);
        }
        public Maze(int x,int y) {//指定行列生成迷宫
            this.X = x;
            this.Y = y;
            CreateMaze(x, y);
        }
        public Maze(int[,] maze,int x,int y) {//通过矩阵导入迷宫
            this.X = x;
            this.Y = y;
            this.maze = maze;
        }
        public void CreateMaze(int x,int y) {//生成迷宫 深度优先算法
            Random random = new Random();
            maze = new int[x + 2, y + 2];
            bool[,] visit = new bool[x + 2, y + 2];
            for (int i = 0; i < x + 2; i++) {
                for (int j = 0; j < y + 2; j++) {
                    maze[i, j] = 1;//1为墙，0为路径
                    visit[i, j] = false;//未访问
                }
            }

            int m_i;
            Stack<Point> stack = new Stack<Point>();
            Point p1=new Point();
            Point[] p = new Point[4];
            p1.X = 1;
            p1.Y = 1;
            visit[p1.X, p1.Y] = true;
            maze[p1.X, p1.Y] = 0;
            int num = 1;
            while (num < (x / 2 + 1) * (y / 2 + 1)) {
                p[0].X = p1.X;
                p[0].Y = p1.Y - 2;
                p[1].X = p1.X;
                p[1].Y = p1.Y + 2;
                p[2].X = p1.X - 2;
                p[2].Y = p1.Y;
                p[3].X = p1.X + 2;
                p[3].Y = p1.Y;
                int m_n;
                for (m_n = 0, m_i = 0; m_i < 4; m_i++) {
                    if (p[m_i].X >= 1 && p[m_i].X <= x && p[m_i].Y >= 1 && p[m_i].Y <= y) {
                        if (visit[p[m_i].X, p[m_i].Y] == false) {
                            m_n++;
                        }
                    }
                }
                if (m_n >= 1) {
                    do {
                        m_i = random.Next()%4;
                    } while (p[m_i].X < 1 || p[m_i].X > x || p[m_i].Y < 1 || p[m_i].Y > y || visit[p[m_i].X, p[m_i].Y] == true);
                    stack.Push(p1);
                    switch (m_i) {
                        case 0: 
                            maze[p1.X,p1.Y - 1] = 0; 
                            break;
                        case 1: 
                            maze[p1.X,p1.Y + 1] = 0; 
                            break;
                        case 2: 
                            maze[p1.X - 1,p1.Y] = 0; 
                            break;
                        case 3:
                            maze[p1.X + 1,p1.Y] = 0; 
                            break;
                    }
                    visit[p[m_i].X, p[m_i].Y] = true;
                    num++;
                    p1 = p[m_i];
                    maze[p1.X, p1.Y] = 0;
                } else {
                    if (stack.Count != 0) {
                        p1 = stack.Pop();
                    }
                }
            }
            maze[0, 1] = -1;
            maze[1, 1] = -1;
            maze[x + 1, y] = 0;
        }
    }
}
