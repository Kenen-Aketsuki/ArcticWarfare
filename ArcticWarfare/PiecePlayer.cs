using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace ArcticWarfare
{ 

    public class PlayerBase
    {
        public string Side=null;
        public string pieceTag;
    }

    public class Griffin : PlayerBase
    {
        public Griffin()
        {
            this.Side = "Griffin";
            this.pieceTag = "GK_Doll";
        }

        public Vector3Int getRetritPos()
        {
            List<Vector3Int> retritPos = new List<Vector3Int>();
            for(int i = 6; i >= -58; i--)
            {
                for(int j = -24; j <= 75; j++)
                {
                    if (AreaTool.Facilities.GetTile(new Vector3Int(j, i, 0)) != null)
                    {
                        if (AreaTool.Facilities.GetTile(new Vector3Int(j, i, 0)).name == "Airdrome_SF") retritPos.Add(new Vector3Int(j, i, 0));
                    }
                }
            }
            return retritPos[UnityEngine.Random.Range(0, retritPos.Count)];
        }

        public Vector3Int getRescuPos()//获取M1887位置
        {
            List<Vector3Int> retritPos = new List<Vector3Int>();
            for (int i = 6; i >= -58; i--)
            {
                for (int j = -24; j <= 75; j++)
                {
                    if (AreaTool.Facilities.GetTile(new Vector3Int(j, i, 0)) != null)
                    {
                        if (AreaTool.Facilities.GetTile(new Vector3Int(j, i, 0)).name == "CommandNot_SF") retritPos.Add(new Vector3Int(j, i, 0));
                    }
                }
            }
            return retritPos[UnityEngine.Random.Range(0, retritPos.Count)];
        }
    }

    public class Sangvis : PlayerBase
    {
        int MaxLoad = 150;
        public int NowLoad = 0;
        public List<LoadPiece> Loding=new List<LoadPiece>();

        public Sangvis()
        {
            this.Side = "Sangvis";
            this.pieceTag = "SF_Doll";
        }

        public bool ifFullLoad(int L)//判断指挥网络在加入这个单位后是否过载 过载True 未过载False
        {
            if (NowLoad + L > MaxLoad)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AddLoadP(string Piece,int R)//添加加载中的棋子
        {
            Loding.Add(new LoadPiece(Piece,R));
        }
    }

    public class LoadPiece {
        public string PicInLoad;
        public int RoundToReady;
        public LoadPiece(string PIL,int RTR)
        {
            PicInLoad = PIL;
            RoundToReady = RTR;
        }
    }

    public class BasePiece:MonoBehaviour
    {
        public int MaxHP;//最大耐久
        public int BehP = 0;//行为点数,所有棋子开局获得5点
        public int MaxMov;//最大移动点数
        public int Mov;//移动点数
        public int HP;//当前耐久
        public int Dod;//闪避
        public int Acc;//命中
        public int Amr = 0;//护甲
        public int Atk;//伤害
        public int Rag;//射程
        public string Skl="N/A";//技能1
        public int MaxSkl;//技能1使用次数上限
        public int NowSkl;//技能1当前剩余使用次数
        public string ExSkl = "N/A";//技能2
        public int MaxESkl;//技能2使用次数上限
        public int NowESkl;//技能2当前剩余使用次数


        public int Stress = 0;//指挥网络负载，仅铁血
        public Vector3Int Pos;//棋子位置
        public List<BuffCell> BuffLST = new List<BuffCell>();//buff列表

        public void setNumber(int Mhp, int mpv, int dod, int acc, int atk, int rag,string skl,int M)
        {
            MaxHP = Mhp;
            MaxMov = mpv;
            Dod = dod;
            Acc = acc;
            Atk = atk;
            Rag = rag;
            Skl = skl;
            MaxSkl = M;
            NowSkl = M;
            ExSkl = "N/A";
            MaxESkl = 0;
            NowESkl = 0;
            Mov = MaxMov;
            HP = MaxHP;
        }

        public void setNumber(int Mhp, int mpv, int dod, int acc, int atk, int rag, string skl,int M1,string skl2, int M2)
        {
            MaxHP = Mhp;
            MaxMov = mpv;
            Dod = dod;
            Acc = acc;
            Atk = atk;
            Rag = rag;
            Skl = skl;
            MaxSkl = M1;
            NowSkl = M1;
            ExSkl = skl2;
            MaxESkl = M2;
            NowESkl = M2;
            Mov = MaxMov;
            HP = MaxHP;
        }

        public void setNumber(int Mhp, int mpv, int dod, int acc, int atk, int rag)
        {
            MaxHP = Mhp;
            MaxMov = mpv;
            Dod = dod;
            Acc = acc;
            Atk = atk;
            Rag = rag;
            Skl = "N/A";
            MaxSkl = 0;
            NowSkl = 0;
            ExSkl = "N/A";
            MaxESkl = 0;
            NowESkl = 0;
            Mov = MaxMov;
            HP = MaxHP;
        }
        
        public void setStress(int stress)//设置指挥网络负载
        {
            Stress = stress;
        }

        public void setAmor(int amr)
        {
            Amr= amr;
        }

        public bool HasBuff(BuffName Nam)
        {
            foreach (BuffCell cell in BuffLST)
            {
                if (cell.Name == Nam) return true;
            }
            return false;
        }

        public void AddBuff(BuffName Nam)
        {
            int LastTime;
            switch (Nam)
            {
                case BuffName.Badly_Injured:
                case BuffName.SF_Leader:
                case BuffName.Singel_Action:
                case BuffName.InFire:
                case BuffName.LightByFire:
                case BuffName.LightByFlare:
                    LastTime = 100;
                    break;
                case BuffName.Riders_Eye:
                case BuffName.Badly_Disabled:
                    LastTime = 3;
                    break;
                case BuffName.Cloud_Terrified:
                    LastTime = 5;
                    break;
                case BuffName.Globalcorrected:
                    LastTime = 6;
                    break;
                case BuffName.Disabled:
                    LastTime = 1;
                    break;
                default:
                    LastTime= 0;
                    break;
            }

            foreach (BuffCell cell in BuffLST)
            {
                if(cell.Name == Nam)
                {
                    cell.LastingTime = LastTime;
                    return;
                }
            }

            BuffLST.Add(new BuffCell(Nam, LastTime));

        }

        public void RemoveBuff(BuffName Nam)
        {
            for(int i = 0; i < BuffLST.Count; i++)
            {
                if(BuffLST[i].Name == Nam)
                {
                    BuffLST.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveBuff()
        {
            List<BuffName> list = new List<BuffName>();
            for(int j=0;j<100 ;j++ )
            {
                bool canOut = false;
                for (int i = 0; i < BuffLST.Count; i++)
                {
                    if (BuffLST[i].LastingTime <= 0)
                    {
                        BuffLST.RemoveAt(i);
                        break;
                    }
                    canOut = true;
                }
                if (canOut)
                {
                    break;
                }
            }
            
        }

        public bool CanCtrl()
        {
            if (HasBuff(BuffName.Disabled) || HasBuff(BuffName.Badly_Disabled))
            {
                return false;
            }

            if (HasBuff(BuffName.Singel_Action))
            {
                return true;
            }

            Vector3 GG = new Vector3(100, 1000, 0);
            Vector3 AC = new Vector3(100, 1000, 0); ;
            if (GameObject.Find("Gager") != null)
            {
                GG = GameObject.Find("Gager").transform.position;
            }
            if (GameObject.Find("Architect") != null)
            {
                AC = GameObject.Find("Architect").transform.position;
            }

            Vector3 PO = AreaTool.Terrain.CellToWorld(Pos);
            if (AreaTool.Facilities.GetTile(new Vector3Int(28, -27, 0)).name.Equals("HQ_GK"))
            {
                
                if (Math.Pow(GG.x - PO.x, 2) + Math.Pow(GG.y - PO.y, 2) <= 56.25) return true;
                if (Math.Pow(AC.x - PO.x, 2) + Math.Pow(AC.y - PO.y, 2) <= 56.25) return true;
                return false;
            }
            else
            {
                List<Vector3Int> RageOfCommand = AreaTool.setTarArea(Pos, 10, true);
                foreach (Vector3Int PS in RageOfCommand)
                {
                    if (AreaTool.Facilities.GetSprite(PS) != null)
                    {
                        if (AreaTool.Facilities.GetSprite(PS).name.Equals("CommandNot_SF")|| AreaTool.Facilities.GetSprite(PS).name.Equals("HQ_SF"))
                        {
                            return true;
                        }
                    }
                }

                if (Math.Pow(GG.x - PO.x, 2) + Math.Pow(GG.y - PO.y, 2) <= 56.25) return true;
                if (Math.Pow(AC.x - PO.x, 2) + Math.Pow(AC.y - PO.y, 2) <= 56.25) return true;

                return false;
            }
        }
    }
}

