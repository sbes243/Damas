using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using UnityEngine.UI;

public class Arbol : MonoBehaviour
{

    public static List<List<(int, int)>> moves = new List<List<(int, int)>>() { new List<(int, int)> { (-1, -1), (1, -1) }, new List<(int, int)> { (1, 1), (-1, 1) } };
    public static List<(int, int, int)> initialPos = new List<(int, int, int)>() {
            (1,7,0), (3,7, 0) , (5,7, 0) , ( 7,7,0),
            (0,6,0), (2,6, 0) , (4,6, 0) , (6,6, 0),
            (1,5,0), (3,5, 0) , (5,5, 0) , (7,5, 0),

            (0,2,1), (2,2, 1) , (4,2, 1) , (6,2, 1),
            (1,1,1), (3,1, 1) , (5,1, 1) , (7,1, 1),
            (0,0,1), (2,0, 1) , (4,0, 1) , (6,0, 1)
        };

    public Material rojot;
    public Material negrot;

    //game references
    public GameObject dama;
    public List<Material> equipos;
    public List<GameObject> lozas;
    public GameObject goscreen;


    Tablero tablero;
    Nodo initialstate;
    Assets.Scripts.Tree game;

    //a donde me puedo mover
    List<(int, int)> movimientoficha = new List<(int, int)>();
    GameObject fichaselected = null;
    private void Awake()
    {
        tablero = new Tablero(initialPos);
        initialstate = new Nodo(tablero);
        game = new Assets.Scripts.Tree(initialstate, equipos,false);
    }
    Ficha f;
    public void drawFichas(Tablero game)
    {
        GameObject[] objts = GameObject.FindGameObjectsWithTag("Ficha");
        foreach (GameObject g in objts) //dudoso
        {
            Destroy(g);
        }

        for (int j = 0; j < game.positions.Count; j++)
        {
            for (int i = (j % 2); i < game.positions.Count; i += 2)
            {
                f = game.getFicha(i, j);
                if (f != null)
                {
                    GameObject ficha = Instantiate(dama, new Vector3(f.pos.Item1, 0.01f, f.pos.Item2), new Quaternion(0, 0, 0, 0));
                    ficha.name = "Ficha";
                    ficha.GetComponent<MeshRenderer>().material = equipos[f.color];
                }
            }
        }
    }
    void Start()
    {     
        drawFichas(game.raiz.tablero);

    }
    (int, int) posant;
    bool myturn = true;
    bool inicialset = false;
    bool gameOver = false;

    void PintarTablero()
    {
        GameObject[] objts = GameObject.FindGameObjectsWithTag("Cuad");

        List<bool> colores = new List<bool>()
        {
            true, false, true, false, true, false, true, false,
            false, true, false, true, false, true, false, true,
            true, false, true, false, true, false, true, false,
            false, true, false, true, false, true, false, true,
            true, false, true, false, true, false, true, false,
            false, true, false, true, false, true, false, true,
            true, false, true, false, true, false, true, false,
            false, true, false, true, false, true, false, true
        };

        for (int i = 0; i < objts.Length; i++)
        {
            if (colores[i])
            {
                objts[i].transform.GetComponent<Renderer>().material = negrot;
            }
            else
            {
                objts[i].transform.GetComponent<Renderer>().material = rojot;
            }
        }
    }

    bool configurado = false;

    private void Update()
    {
        if (!gameOver)
        {
            if (game.raiz.hijos == null)
            {
                gameOver = !gameOver;
            }
            else
            {
                if (myturn)
                {
                    //Debug.Log("aea");
                    game.MinMax();
                    drawFichas(game.raiz.tablero); //dudoso
                    myturn = !myturn;
                }
                else
                {


                    if (Input.GetMouseButtonUp(0))
                    {

                        RaycastHit hitInfo = new RaycastHit();
                        bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);


                        if (hit && hitInfo.collider.name == "Ficha" )
                        {
                            PintarTablero();
                            Debug.Log("clicking ficha");
                            fichaselected = hitInfo.transform.gameObject;
                            posant = (int.Parse(fichaselected.transform.position.x.ToString()), int.Parse(fichaselected.transform.position.z.ToString()));
                            movimientoficha = game.raiz.checkFichamoves(game.raiz.tablero.getFicha(posant.Item1, posant.Item2));
                            GameObject[] objts = GameObject.FindGameObjectsWithTag("Cuad");
                            foreach (var o in objts)
                            {
                                for (int j = 0; j < movimientoficha.Count; j++)
                                {
                                    if (o.gameObject.transform.position.x == movimientoficha[j].Item1 &&
                                        o.gameObject.transform.position.z == movimientoficha[j].Item2)
                                    {
                                        o.GetComponent<MeshRenderer>().material.color = Color.white;
                                    }
                                }
                            }
                        }



                        if (fichaselected != null && hitInfo.collider.name != "Ficha")
                        {



                            if (hit)
                            {
                                int ia;
                                for (ia = 0; ia < movimientoficha.Count; ia++)
                                {
                                    if (movimientoficha[ia] == (int.Parse(hitInfo.transform.position.x.ToString()), int.Parse(hitInfo.transform.position.z.ToString())))
                                    {
                                        (int, int) movido = movimientoficha[ia];
                                        
                                        game.Enemy(movido, posant);
                                        Debug.Log("calculando movimiento");
                                       

                                        Vector3 val = new Vector3(hitInfo.transform.position.x, fichaselected.transform.position.y, hitInfo.transform.position.z);
                                        fichaselected.transform.position = val;
                                        drawFichas(game.raiz.tablero);
                                        PintarTablero();
                                        game.UpdateTree();
                                       

                                        myturn = !myturn;
                                        ia = movimientoficha.Count;


                                    }

                                }

                                //resetting data
                                movimientoficha.Clear();

                            }
                        }


                    }

                }

            }
        }
        else
        {
            //int v = game.raiz.calval(1);
            //if ( v> 0)
            //{
            //    goscreen.GetComponentInChildren<Text>().text += " Gana Azul";
            //}
            //else if(v==0)
            //{
            //    goscreen.GetComponentInChildren<Text>().text += "\n empate";
            //}
            //else
            //{
            //    goscreen.GetComponentInChildren<Text>().text += "\n Gana Rojo";
            //}
            goscreen.SetActive(true);
        }
    }

}
