using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2021aoc9
{
    class Program
    {
        class Domborzat
        {

            int[,] D;
            int N { get; }
            int M { get; }
            bool[,] gödör_e;
            int speed;
            bool debug;

            int[,] szín; // szín[x,y]==z   <=>   "(x,y) pont színe z".
            List<(int, int)> gödrök; // gödrök[n] == (i,j)    <=>    "az n-edik gödör (i,j) helyen van"
            List<int> méret; // méret[n] == m    <=>    "az n-edik gödörből kiinduló medence mérete m"
            public Domborzat(string path, int s = 0, bool debug=false)
            {
                speed = s;
                string[] input = System.IO.File.ReadAllLines(path);
                N = input.Length;
                M = input.First().Length;
                this.debug = debug;
                szín = new int[N, M];
                gödrök = new List<(int, int)>();
                méret = new List<int>();

                D = new int[N,M];
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < M; j++)
                        D[i,j] = int.Parse(input[i][j].ToString());
                
                gödör_e = new bool[N, M];
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < M; j++)
                    {
                        gödör_e[i, j] = D[i, j] < Szomszédai(i, j).Min(p => D[p.Item1, p.Item2]);
                        if (gödör_e[i, j])
                        {
                            gödrök.Add((i, j));
                            méret.Add(0);
                        }
                    }

            }

            public List<(int,int)> Szomszédai(int x, int y) 
            {
                List<(int, int)> szomszédok = new List<(int,int)>();
                if (0 <= x - 1)
                    szomszédok.Add((x - 1, y));
                if (x + 1 < N)
                    szomszédok.Add((x + 1, y));
                if (0 <= y - 1)
                    szomszédok.Add((x, y-1));
                if (y + 1 < M)
                    szomszédok.Add((x, y+1));
                return szomszédok;
            }

            public int SumRiskLVL()
            {
                int s=0;
                for (int i = 0; i < N; i++)
                    for (int j = 0; j < M; j++)
                        if (gödör_e[i,j])
                            s += 1 + D[i, j];
                return s;

            }

            private void MedenceSzínezés()
            {
                for (int i = 0; i < gödrök.Count; i++)
                {
                    (int x, int y) = gödrök[i];
                    if (szín[x,y]==0) // fehér
                        méret[i] = FloodFill(x, y);
                }
            }

            public int Három_legnagyobb_medence_szorzata() 
            {
                MedenceSzínezés();
                foreach (var item in méret)
                {
                    Console.Write($"{item}, ");
                }
                Diagnosztika();
                Console.WriteLine();
                return méret.Where(x => x != 0).OrderByDescending(x => x).Take(3).Aggregate(1, (current, item) => current * item);
            }

            private int FloodFill(int x0, int y0)
            {
                int méret = 0;
                Queue<(int, int)> tennivalók = new Queue<(int, int)>();
                tennivalók.Enqueue((x0, y0));

                while (0 < tennivalók.Count)
                {
                    (int i, int j) = tennivalók.Dequeue();
                    méret++;
                    szín[i, j] = 1; // fekete
                    foreach ((int,int) szomszéd in Szomszédai(i,j))
                    {
                        (int x, int y) = szomszéd;
                        if (szín[x, y] == 0 && D[x, y] < 9) // fehér
                        {
                            tennivalók.Enqueue(szomszéd);
                            szín[x, y] = -1; // szürke
                        }
                    }
                    if(debug)Diagnosztika();
                }
                //Diagnosztika();
                return méret;
            }

            private void Ki(int n, ConsoleColor szín)
            {
                Console.ForegroundColor = szín;
                Console.Write(n);
                Console.ForegroundColor = ConsoleColor.White;
            }
            public void Diagnosztika()
            {
                Console.Clear();
                for (int i = 0; i < N; i++)
                {
                    for (int j = 0; j < M; j++)
                        switch (szín[i, j])
                        {
                            case 1:
                                Ki(D[i, j], ConsoleColor.Green);
                                break;
                            case -1:
                                Ki(D[i, j], ConsoleColor.Red);
                                break;
                            default:
                                Ki(D[i, j], ConsoleColor.White);
                                break;
                        }
                    Console.WriteLine();
                }
                System.Threading.Thread.Sleep(speed);
            }

        }
        static void Main(string[] args)
        {
            Console.WriteLine("Indulhat?");
            Console.ReadKey();
            Domborzat d = new Domborzat("teszt.txt", 100, true);
            Console.WriteLine(d.SumRiskLVL());
            Console.WriteLine(d.Három_legnagyobb_medence_szorzata());
            Console.ReadLine();
        }
    }
}
