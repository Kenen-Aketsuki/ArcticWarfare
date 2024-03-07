using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ArcticWarfare
{
    public static class AreaTool
    {
        public static Tilemap Terrain;//地形
        public static Tilemap Plant;//植被
        public static Tilemap Facilities;//设施
        public static Tilemap Areas;//区域显示
        public static TileBase MovZone;//移动范围单元格资源
        public static TileBase AtkZone;//攻击、技能范围单元格资源
        public static TileBase FocZone;//关注棋子标识
        //需要在程序初始化时赋值

        public static void FirstStart()
        {
            Terrain = GameObject.Find("BattleField").transform.GetChild(0).gameObject.GetComponent<Tilemap>();//地形
            Plant = GameObject.Find("BattleField").transform.GetChild(1).gameObject.GetComponent<Tilemap>();//植被
            Facilities = GameObject.Find("BattleField").transform.GetChild(2).gameObject.GetComponent<Tilemap>();//设施
            Areas = GameObject.Find("BattleField").transform.GetChild(3).gameObject.GetComponent<Tilemap>();//区域显示
        }

        public static short getMovePt(Vector3Int Sta,Vector3Int End,bool isOne)
        {
            if (Terrain.GetSprite(End) == null) return -1;
            if (isOne) return 1;
            Func<Vector3Int, short> GLV = Pos =>
             {
                 switch (Terrain.GetSprite(Pos).name)
                 {
                     case "GroundB1":
                         return 1;
                     case "GroundB2":
                         return 2;
                     case "GroundB3":
                         return 3;
                     case "GroundB4":
                         return 4;
                     case "GroundB5":
                         return 6;
                     case "Water":
                         return 10;
                     default:
                         return -1;
                 }
             };
            short Lvs = GLV(Sta);
            short Lve = GLV(End);
            short cst = 0;
            //地形判定
            if (Lve == 6 || Lve == -1 || Lvs == -1) return -1;
            if (Lve - Lvs == 0) cst += Lve;
            else if (Lve - Lvs == 1) cst += (short)(Lve + 1);
            else if (Lve - Lvs == -1) cst += (short)(Lve - 1);
            else
            {
                if (Lve == 10) cst += 10;
                else if (Lvs == 10 && Lve == 1) cst += 1;
                else return -1;
            }
            if (cst == 0) cst++;
            //植被、设施判定
            if (Plant.GetSprite(End) != null) cst += 1;
            if (Facilities.GetSprite(End) != null) cst += 1;

            return cst;
        }

        public  static List<Cell> getRoundCell(Cell sta,bool isOne)
        {
            Func<Vector3Int, Vector3Int, int> Dis = (x, y) => {
                return Mathf.Abs(x.x - y.x) + Mathf.Abs(x.y - y.y);
            };
            Vector3Int pos = sta.Pos;
            Vector3Int[] cells = new Vector3Int[6];
            List<Cell> Fcell = new List<Cell>();

            if (pos.y % 2 != 0)
            {
                cells[0] = pos + new Vector3Int(0, 1, 0); cells[1] = pos + new Vector3Int(1, 1, 0);
                cells[5] = pos + new Vector3Int(-1, 0, 0); cells[2] = pos + new Vector3Int(1, 0, 0);
                cells[4] = pos + new Vector3Int(0, -1, 0); cells[3] = pos + new Vector3Int(1, -1, 0);
            }
            else
            {
                cells[0] = pos + new Vector3Int(-1, 1, 0); cells[1] = pos + new Vector3Int(0, 1, 0);
                cells[5] = pos + new Vector3Int(-1, 0, 0); cells[2] = pos + new Vector3Int(1, 0, 0);
                cells[4] = pos + new Vector3Int(-1, -1, 0); cells[3] = pos + new Vector3Int(0, -1, 0);
            }

            for (int i = 0; i < cells.Length; i++)
            {
                if (AreaTool.getMovePt(pos, cells[i],isOne) == -1) continue;
                Fcell.Add(new Cell(sta.Cst + AreaTool.getMovePt(pos, cells[i],isOne), Dis(pos, cells[i]), cells[i],sta));
            }
            return Fcell;
        }

        public static List<Cell> getRoundCell(Cell sta)
        {
            Func<Vector3Int, Vector3Int, int> Dis = (x, y) => {
                return Mathf.Abs(x.x - y.x) + Mathf.Abs(x.y - y.y);
            };
            Vector3Int pos = sta.Pos;
            Vector3Int[] cells = new Vector3Int[6];
            List<Cell> Fcell = new List<Cell>();

            if (pos.y % 2 != 0)
            {
                //奇数
                cells[0] = pos + new Vector3Int(0, 1, 0); cells[1] = pos + new Vector3Int(1, 1, 0);
                cells[5] = pos + new Vector3Int(-1, 0, 0); cells[2] = pos + new Vector3Int(1, 0, 0);
                cells[4] = pos + new Vector3Int(0, -1, 0); cells[3] = pos + new Vector3Int(1, -1, 0);
            }
            else
            {
                //偶数
                cells[0] = pos + new Vector3Int(-1, 1, 0); cells[1] = pos + new Vector3Int(0, 1, 0);
                cells[5] = pos + new Vector3Int(-1, 0, 0); cells[2] = pos + new Vector3Int(1, 0, 0);
                cells[4] = pos + new Vector3Int(-1, -1, 0); cells[3] = pos + new Vector3Int(0, -1, 0);
            }

            for (int i = 0; i < cells.Length; i++)
            {
                Fcell.Add(new Cell(sta.Cst + 1, Dis(pos, cells[i]), cells[i], sta));
            }
            return Fcell;
        }

        static List<Cell> FindList = new List<Cell>();//打开队列
        static List<Cell> Marea = new List<Cell>();//区域

        static void FindTarArea(Vector3Int Sta, int Rag,bool isOne)//寻找指定区域
        {
            Func<List<Cell>, Cell, Cell> hasSamePos = (lst, cll) => {
                foreach (Cell c in lst)
                {
                    if (c.Pos == cll.Pos) return c;
                }
                return null;
            };

            Sta.z = 0;
            FindList.Add(new Cell(0, 0, Sta));

            for (; FindList.Count != 0;)
            {
                Cell Now = FindList[0];
                FindList.RemoveAt(0);
                List<Cell> Round = getRoundCell(Now, isOne);
                foreach (Cell c in Round)//把有效格子入队
                {
                    if (c.Cst > Rag || hasSamePos(Marea, c) != null) continue;
                    Cell Sam = hasSamePos(FindList, c);
                    if (Sam == null)
                    {
                        FindList.Add(c);
                    }
                    else
                    {
                        if (Sam.Cst > c.Cst)
                        {
                            FindList.Remove(Sam);
                            FindList.Add(c);
                        }
                    }
                }
                Marea.Add(Now);
            }

            
        }

        public static void setMoveArea(Vector3Int Sta,int Mov)
        {
            Areas.ClearAllTiles();
            Marea.Clear();
            FindTarArea(Sta, Mov, false);
            foreach (Cell Now in Marea)
            {
                Areas.SetTile(Now.Pos, MovZone);
            }
        }
        public static void setAttackArea(Vector3Int Sta, int Rag)
        {
            Areas.ClearAllTiles();
            Marea.Clear();
            FindTarArea(Sta, Rag, true);
            foreach (Cell Now in Marea)
            {
                Areas.SetTile(Now.Pos, AtkZone);
            }
        }

        public static List<Vector3Int> setTarArea(Vector3Int Sta, int Rage,bool isOne)
        {
            Marea.Clear();
            FindTarArea(Sta, Rage, isOne);
            List<Vector3Int> ResultArea = new List<Vector3Int>();
            foreach (Cell Now in Marea)
            {
                ResultArea.Add(Now.Pos);
            }
            return ResultArea;
        }

        public static void yee()//测试用方法
        {
            Debug.Log("我来了，就是测试成功了");
        }

        

        public static List<Cell> FindPath(Vector3Int startPoint, Vector3Int end)
        {
            List<Vector3Int> pathtiles = new List<Vector3Int>();

            List<Cell> openPathTiles = new List<Cell>();
            List<Cell> closedPathTiles = new List<Cell>();
            Func<Vector3Int, Vector3Int, int> Dis = (x, y) => {
                return Mathf.Abs(x.x - y.x) + Mathf.Abs(x.y - y.y);
            };
            Cell currentTile = new Cell(0, Dis(startPoint, end), startPoint, null);

            Cell endPoint = new Cell(0, 0, end, null);
            openPathTiles.Add(currentTile);
            while (openPathTiles.Count != 0)
            {
                openPathTiles = openPathTiles.OrderBy(x => x.F).ThenByDescending(x => x.Cst).ToList();
                currentTile = openPathTiles[0];
                openPathTiles.Remove(currentTile);
                closedPathTiles.Add(currentTile);
                int Cst = currentTile.Cst + 1;

                List<Vector3Int> temp1 = new List<Vector3Int>();
                foreach (Cell qw in closedPathTiles)
                {
                    temp1.Add(qw.Pos);
                }
                List<Vector3Int> temp2 = new List<Vector3Int>();
                foreach (Cell qw in openPathTiles)
                {
                    temp2.Add(qw.Pos);
                }

                if (temp1.Contains(endPoint.Pos))
                {
                    endPoint = currentTile;
                    break;
                }
                // 调查当前地格的每一块相邻的地格
                foreach (Cell adjacentTile in getRoundCell(currentTile))
                {
                    //判断是否可走，不能走跳出本次循环
                    if (temp1.Contains(adjacentTile.Pos))
                    {
                        continue;
                    }
                    if (!temp2.Contains(adjacentTile.Pos))
                    {
                        adjacentTile.StaDis = Dis(adjacentTile.Pos, endPoint.Pos);
                        openPathTiles.Add(adjacentTile);
                    }
                    else if (adjacentTile.F > adjacentTile.StaDis + Cst)
                    {
                        adjacentTile.Cst = Cst;
                    }

                }
            }
            List<Vector3Int> temp3 = new List<Vector3Int>();
            foreach (Cell qw in closedPathTiles)
            {
                temp3.Add(qw.Pos);
            }

            List<Cell> finalPathTiles = new List<Cell>();

            //回溯--设置最终路径。
            if (temp3.Contains(endPoint.Pos))
            {

                currentTile = endPoint;
                finalPathTiles.Add(currentTile);

                for (int i = endPoint.Cst - 1; i >= 0; i--)
                {

                    List<Vector3Int> temp4 = new List<Vector3Int>();
                    foreach (Cell t in getRoundCell(currentTile))
                    {
                        temp4.Add(t.Pos);
                    }
                    currentTile = closedPathTiles.Find(x => x.Cst == i && temp4.Contains(x.Pos));
                    finalPathTiles.Add(currentTile);
                }

                finalPathTiles.Reverse();

            }
            foreach (Cell temp in finalPathTiles)
            {
                pathtiles.Add(temp.Pos);
            }

            return finalPathTiles;

        }
    }

    public enum RoundState
    {
        WaitFocus,
        WaitAction,
        WaitAtkTarget,
        WaitSklTarget,
        WaitSupTarget,
        WaitMovDestination,
        WaitDeployPosition,
        WaitSupBoxPosition
        
    }

    public class Cell//单元格
    {
        public int Cst;//从起点到此的总花费
        public int StaDis;//到起点的距离
        public Vector3Int Pos;//此位置
        public Cell From=null;
        public int F => Cst + StaDis;

        public Cell(int cst, int stadis, Vector3Int pos,Cell from)
        {
            Cst = cst;
            StaDis = stadis;
            Pos = pos;
            From = from;
        }
        public Cell(int cst, int stadis, Vector3Int pos)
        {
            Cst = cst;
            StaDis = stadis;
            Pos = pos;
        }
    }
}
