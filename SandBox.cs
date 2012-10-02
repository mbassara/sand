using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.CompilerServices;

namespace Sand
{
    class SandBox
    {
        private int size;
        private int pixelSize;

        private bool[,] box;
        private Graphics img;

        private Point fallingPoint;
        public Point FallingPoint
        {
            get { return fallingPoint; }
            set
            {
                if (value.X / pixelSize < size && value.Y / pixelSize < size &&
                    value.X / pixelSize >= 0 && value.Y / pixelSize >= 0)
                    fallingPoint = new Point(value.X / pixelSize, value.Y / pixelSize);
            }
        }

        private bool isRunning;
        public bool IsRunning
        {
            get { return isRunning; }
            set { isRunning = value; }
        }

        private bool isPaused;
        public bool IsPaused
        {
            get { return isPaused; }
            set { isPaused = value; }
        }

        private int delay;
        public int Delay
        {
            get { return delay; }
            set { delay = value; }
        }

        private static Dictionary<Graphics, SandBox> instances = new System.Collections.Generic.Dictionary<Graphics,SandBox>();

        public static SandBox GetInstance(Graphics img, int size, int pixelSize, int delay)
        {
            if (!instances.ContainsKey(img))
                instances.Add(img, new SandBox(img, size, pixelSize, delay));

            return instances[img];
        }

        private SandBox(Graphics img, int size, int pixelSize, int delay)
        {
            this.size = size / pixelSize;
            this.pixelSize = pixelSize;
            this.img = img;
            this.img.Clear(Color.White);
            this.delay = delay;

            isRunning = false;
            isPaused = true;
            fallingPoint = new Point(size / 2, 0);
            box = new bool[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    box[i, j] = false;

            for (int i = 0; i < size; i++)
                box[size-1, i] = true;
        }

        public void Reset()
        {
            Reset(size * pixelSize /*size in pixels*/, pixelSize, delay);
        }

        public void Reset(int size, int pixelSize, int delay)
        {
            isRunning = false;
            isPaused = true;

            this.size = size / pixelSize;
            this.pixelSize = pixelSize;
            this.img.Clear(Color.White);

            fallingPoint = new Point(size / 2, 0);
            box = new bool[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    box[i, j] = false;

            for (int i = 0; i < size; i++)
                box[size - 1, i] = true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Run(int iterations)
        {
            System.Diagnostics.Debug.WriteLine("Run start, Thread: " + System.Threading.Thread.CurrentThread.Name);
            if (isRunning)
                return;

            img.Clear(Color.White);

            isRunning = true;
            for (int i = 0; i < iterations && isRunning; i++)
            {
                Iterate();

                if(delay > 0)
                    System.Threading.Thread.Sleep(delay);
            }
            isRunning = false;
            System.Diagnostics.Debug.WriteLine("Run end, Thread: " + System.Threading.Thread.CurrentThread.Name);
        }

        private void MakeGrain(int x, int y)
        {
            box[x, y] = true;
        }


        private void Iterate()
        {
            if (!isPaused)
                MakeGrain(fallingPoint.Y, fallingPoint.X);
            bool[,] before = DuplicateBox();

            for (int i = 0; i + 1 < size && isRunning; i += 2)
                for (int j = 0; j + 1 < size && isRunning; j += 2)
                    NextState(ref box[i, j], ref box[i, j + 1], ref box[i + 1, j], ref box[i + 1, j + 1]);

            for (int i = 1; i + 1 < size && isRunning; i += 2)
                for (int j = 1; j + 1 < size && isRunning; j += 2)
                    NextState(ref box[i, j], ref box[i, j + 1], ref box[i + 1, j], ref box[i + 1, j + 1]);

            for (int i = 0; i + 1 < size && isRunning; i++)
                for (int j = 0; j + 1 < size && isRunning; j++)
                    if (box[i, j] != before[i, j] && isRunning)
                    {
                        if (box[i, j])
                            img.FillRectangle(Brushes.Black, j * pixelSize, i * pixelSize, pixelSize, pixelSize);
                        else
                            img.FillRectangle(Brushes.White, j * pixelSize, i * pixelSize, pixelSize, pixelSize);
                    }
        }

        private bool[,] DuplicateBox()
        {
            bool[,] newBox = new bool[size, size];

            for (int i = 0; i < size && isRunning; i++)
                for (int j = 0; j < size && isRunning; j++)
                    newBox[i, j] = box[i, j];
            return newBox;
        }

        private void NextState(ref bool ul,
                                ref bool ur,
                                ref bool bl,
                                ref bool br)
        {
            if (!ul && !ur && !bl && !br)
                return; // empty

            bool t = true, f = false;

            if (ul && !ur &&
                !bl && !br)
            { // 1
                ul = f; ur = f;
                bl = t; br = f;
            }
            else if (!ul && ur &&
                     !bl && !br)
            { // 2
                ul = f; ur = f;
                bl = f; br = t;
            }
            else if (ul && !ur &&
                     bl && !br)
            { // 3
                ul = f; ur = f;
                bl = t; br = t;
            }
            else if (!ul && ur &&
                     !bl && br)
            { // 4
                ul = f; ur = f;
                bl = t; br = t;
            }
            else if (ul && ur &&
                     bl && !br)
            { // 5
                ul = t; ur = f;
                bl = t; br = t;
            }
            else if (ul && ur &&
                     !bl && br)
            { // 6
                ul = f; ur = t;
                bl = t; br = t;
            }
            else if (!ul && ur &&
                     bl && !br)
            { // 7
                ul = f; ur = f;
                bl = t; br = t;
            }
            else if (ul && !ur &&
                     !bl && br)
            { // 8
                ul = f; ur = f;
                bl = t; br = t;
            }
            else if (ul && ur &&
                     !bl && !br)
            { // 9 block
                if (new Random().Next() % 2 == 0)
                {
                    ul = f; ur = f;
                    bl = t; br = t;
                }
                else
                {
                    ul = t; ur = t;
                    bl = f; br = f;
                }
            }
        }
    }
}
