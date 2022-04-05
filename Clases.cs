using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    //valores estaticos
    
    public class Ficha
    {
        public int color;
        public (int, int) pos;
        //public GameObject repre;
        public Ficha()
        {
            color = 0;
            pos = (0, 0);
        }
        public Ficha(Ficha f)
        {
            color = f.color;
            pos = f.pos;
            //repre = f.repre;
        }
        public Ficha((int, int) pos, int color)
        {
            this.color = color;
            this.pos = pos;
        }

    }

    public class Tablero
    {
        public List<List<Ficha>> positions = new List<List<Ficha>>();
        public Tablero()
        {
            fillblank();
        }
        public Tablero(Tablero t)
        {

            foreach (List<Ficha> f in t.positions)
            {
                List<Ficha> nf = new List<Ficha>();
                foreach (Ficha fi in f)
                {
                    if (fi == null)
                    {
                        nf.Add(null);
                    }
                    else
                    {
                        nf.Add(new Ficha(fi));
                    }

                }
                positions.Add(nf);
            }
        }
        public Tablero(List<(int, int, int)> fpos)
        {
            fillblank();
            foreach ((int, int, int) ficha in fpos)
            {

                positions[ficha.Item2][ficha.Item1] = new Ficha((ficha.Item1, ficha.Item2), ficha.Item3);
            }
        }
        void fillblank()
        {
            for (int i = 0; i < 8; i++)
            {
                positions.Add(new List<Ficha>(8) { null, null, null, null, null, null, null, null });
            }
        }
        public Ficha getFicha(int x, int y)
        {
            return positions[y][x];
        }

    }

    public class Nodo
    {
        public Tablero tablero;
        public List<Nodo> hijos = null;
        public int turno;
        //~Nodo()
        //{
        //    // Debug.Log("mori");
        //    for (int i = 0; i < hijos.Count; i++)
        //    {
        //        hijos[i] = null;
        //    }
        //}
        public void setTurno(int t)
        {
            turno = t;
        }
        public Nodo(Tablero t)
        {
            tablero = t;

        }
        public Nodo(Tablero t,int turno)
        {
            tablero = t;
            this.turno = turno;
        }
        public Nodo(ref Tablero g)
        {
            tablero = g;
        }
        bool inboard(int a)
        {
            if (a < 0) return false;
            if (a >= tablero.positions.Count) return false;
            return true;
        }
        public List<(int, int)> checkFichamoves(Ficha f)
        {
            List<(int, int)> nuevasPosiciones = new List<(int, int)>();

            foreach ((int, int) move in Arbol.moves[f.color])
            {
                int z = f.pos.Item2 + move.Item2;
                int x = f.pos.Item1 + move.Item1;
                if (inboard(z) && inboard(x))
                {
                    Tablero nt = new Tablero(tablero);
                    if (tablero.positions[z][x] == null)
                    {

                        (nt.positions[f.pos.Item2][f.pos.Item1], nt.positions[z][x]) = (nt.positions[z][x], nt.positions[f.pos.Item2][f.pos.Item1]);
                        nt.positions[z][x].pos = (x, z);

                        nuevasPosiciones.Add((x, z));

                    }
                    else if (tablero.positions[z][x].color != f.color && inboard(z + move.Item2) && inboard(x + move.Item1) && tablero.positions[z + move.Item2][x + move.Item1] == null)
                    {

                        nt.positions[z][x] = null;
                        z += move.Item2;
                        x += move.Item1;
                        (nt.positions[f.pos.Item2][f.pos.Item1], nt.positions[z][x]) = (nt.positions[z][x], nt.positions[f.pos.Item2][f.pos.Item1]);
                        nt.positions[z][x].pos = (x, z);
                        nuevasPosiciones.Add((x, z));
                    }

                }

                
            }


            return nuevasPosiciones;
        }
        public List<Nodo> checkmoves(int color)
        {
            List<Nodo> nuevasPosiciones = new List<Nodo>();

            for (int j = 0; j < tablero.positions.Count; j++)
            {
                for (int i = (j % 2); i < tablero.positions.Count; i += 2)
                {
                    Ficha f = tablero.positions[j][i];
                    if (f != null && f.color == color)
                    {
                        foreach ((int, int) move in Arbol.moves[f.color])
                        {
                            int z = f.pos.Item2 + move.Item2;
                            int x = f.pos.Item1 + move.Item1;
                            if (inboard(z) && inboard(x))
                            {
                                Tablero nt = new Tablero(tablero);
                                if (nt.positions[z][x] == null)
                                {

                                    (nt.positions[f.pos.Item2][f.pos.Item1], nt.positions[z][x]) = (nt.positions[z][x], nt.positions[f.pos.Item2][f.pos.Item1]);
                                    nt.positions[z][x].pos = (x, z);

                                    nuevasPosiciones.Add(new Nodo(nt,color));

                                }
                                else if (nt.positions[z][x].color != f.color && inboard(z + move.Item2) && inboard(x + move.Item1) && nt.positions[z + move.Item2][x + move.Item1] == null)
                                {

                                    nt.positions[z][x] = null;
                                    z += move.Item2;
                                    x += move.Item1;
                                    (nt.positions[f.pos.Item2][f.pos.Item1], nt.positions[z][x]) = (nt.positions[z][x], nt.positions[f.pos.Item2][f.pos.Item1]);
                                    nt.positions[z][x].pos = (x, z);
                                    nuevasPosiciones.Add(new Nodo(nt,color));
                                }

                            }

                        }
                    }
                }
            }
            return (nuevasPosiciones.Count == 0) ? null : nuevasPosiciones;
        }
        public int calval(int starter)
        {
            int n0 = 0;
            int n1 = 0;
            for (int j = 0; j < tablero.positions.Count; j++)
            {
                for (int i = (j % 2); i < tablero.positions.Count; i += 2)
                {
                    if (tablero.positions[j][i] != null)
                    {
                        if (tablero.positions[j][i].color == 0) n0++;
                        else n1++;
                    }
                }
            }
            int sol = n0 - n1;
            return (starter == 0) ? sol : -1 * sol;
        }

    }
    //turnos false es azul---true es rojo
    public class Tree
    {
        public int maxprof = 4;
        public Nodo raiz;
        public List<Material> equipos;
        //game setup
        
        public Tree(Nodo n, List<Material> equipos,bool j)
        {
            raiz = n;
            this.equipos = equipos;
            raiz.setTurno((j) ? 0 : 1);
            createnextmoves(raiz);
        }
        public void createnextmoves(Nodo n,int prof = 1)
        {
            
            if (prof > maxprof) return;
            if (n.hijos == null)
            {
                n.hijos = n.checkmoves((n.turno==0)?1:0);
            }
            //n.hijos = n.checkmoves((inicia)?1:0);
            //inicia = !inicia;
            if (n.hijos == null) return;
            for (int i = 0; i < n.hijos.Count; i++)
            {
                createnextmoves(n.hijos[i],prof+1);
            }


        }
        

        public int minMaxRecursive(Nodo x, ref int index, bool condition = true)
        { //1 es maximizador, 0 es minimizadorcalva

            if (x.hijos == null) return x.calval(0);

            List<int> vals = new List<int>();
            for (int i = 0; i < x.hijos.Count; i++)
            {
                vals.Add(minMaxRecursive(x.hijos[i], ref index, (!condition)));
            }

            if (condition)
            {
                int e = 0;
                int maxi = vals[0];
                Debug.Log(maxi);
                for (int i = 1; i < vals.Count; i++)
                {
                    if (vals[i] > maxi)
                    {
                        maxi = vals[i];
                        e = i;
                    }
                }
                index = e;
                //Debug.Log(index);
                return maxi;
            }
            else
            {
                int e = 0;
                int mini = vals[0];
                for (int i = 1; i < vals.Count; i++)
                {
                    if (vals[i] < mini)
                    {
                        mini = vals[i];
                        e = i;
                    }
                }
                index = e;

                return mini;
            }
        }
        private void dfs(Nodo a, int b = 0, bool turno = false)
        {
            if (a == null) return;
            if (b > maxprof) return;

        }

        public void MinMax()
        {
            int pos = 0;
            int val =minMaxRecursive(raiz, ref pos);
            Debug.Log("--------------------------------");
            Debug.Log(pos);
            Debug.Log(val);
            if (raiz.hijos != null)
            {
                Nodo save = raiz.hijos[pos];
                raiz.hijos.RemoveAt(pos);
                raiz = null;
                raiz = save;
            }
            else
            {
                raiz = null;
            }
        }

        public void Enemy((int, int) b, (int, int) c)
        {
            int pos = findenemymove(b, c);
            if (raiz.hijos != null)
            {
                Nodo save = raiz.hijos[pos];
                raiz.hijos.RemoveAt(pos);
                raiz = null;
                raiz = save;
                
            }
            else
            {
                raiz = null;
            }
            GC.Collect();
        }

        public void UpdateTree()
        {

            createnextmoves(raiz);

        }

        private int findenemymove((int,int) a, (int, int) b)
        {

            for (int i = 0; i < raiz.hijos.Count; i++)
            {
                if (raiz.hijos[i].tablero.positions[a.Item2][a.Item1] != null && raiz.hijos[i].tablero.positions[b.Item2][b.Item1] == null)
                {
                    return i;
                }
                //if (c.Item1 < 0)
                //{
                //    if (raiz.hijos[i].tablero.positions[a.pos.Item2][a.pos.Item1] != null && raiz.hijos[i].tablero.positions[a.pos.Item2][a.pos.Item1].color == a.color && raiz.hijos[i].tablero.positions[b.Item2][b.Item1] == null)
                //    {
                //        Debug.Log("found");
                //        return i;
                //    }

                //}
                //else
                //{

                //    if (raiz.hijos[i].tablero.positions[a.pos.Item2][a.pos.Item1] != null && raiz.hijos[i].tablero.positions[a.pos.Item2][a.pos.Item1].color == a.color  && raiz.hijos[i].tablero.positions[b.Item2][b.Item1] == null && raiz.hijos[i].tablero.positions[c.Item2][c.Item1] == null)
                //    {
                //        Debug.Log("found");
                //        return i;
                //    }
                //}

            }
            Debug.Log("not found");
            return 0;
        }
    }

}
