using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace Y86
{
    public partial class frmtaby86 : Form
    {
        ListViewGroup F = new ListViewGroup();
        ListViewGroup D = new ListViewGroup();
        ListViewGroup E = new ListViewGroup();
        ListViewGroup M = new ListViewGroup();
        ListViewGroup W = new ListViewGroup();
        ListViewGroup R = new ListViewGroup();
        ListViewGroup S = new ListViewGroup();
        ListViewGroup INS = new ListViewGroup();
        public frmtaby86()
        {
            try
            {
                InitializeComponent();                
                radionotime.Checked = true;
                tmr.Enabled = false;
                F.Header = "Fetch";
                F.HeaderAlignment = HorizontalAlignment.Left;
                D.Header = "Decode";
                D.HeaderAlignment = HorizontalAlignment.Left;
                E.Header = "Execute";
                E.HeaderAlignment = HorizontalAlignment.Left;
                M.Header = "Memory";
                M.HeaderAlignment = HorizontalAlignment.Left;
                W.Header = "Write Back";
                W.HeaderAlignment = HorizontalAlignment.Left;
                S.Header = "Status";
                S.HeaderAlignment = HorizontalAlignment.Left;
                R.Header = "Register";
                R.HeaderAlignment = HorizontalAlignment.Left;
                INS.Header = "Instruction";
                INS.HeaderAlignment = HorizontalAlignment.Left;
                lstview.Groups.Add(F);
                lstview.Groups.Add(D);
                lstview.Groups.Add(E);
                lstview.Groups.Add(M);
                lstview.Groups.Add(W);
                lstview.Groups.Add(S);
                lstview.Groups.Add(R);
                lstview.Groups.Add(INS);
            }
            catch(System.Exception err)
            {
                return;
            }            
        }
        const int INTMIN = -2147483648;
        const int LINENUM = 31;
        const int MAXINSTRUCTIONNUMBER=11111;
        int lastrow = INTMIN;
        //save all instructions read from file
        string[] instruction=new string[MAXINSTRUCTIONNUMBER];
        //reg
        string[] reg = { "%eax", "%ecx", "%edx", "%ebx", "%esp", "%ebp", "%esi", "%edi" ,"","","","","","","","RNONE"}; //register
        //state
        int F_icode,F_ifun, instr_valid,imem_error,F_valC,F_valP,F_PC,F_predPC,F_rA,F_rB;
        int D_stat, D_icode, D_ifun, D_rA, D_rB, D_valC, D_valP,d_srcA,d_srcB,d_valA,d_valB,d_dstE,d_dstM;
        int E_stat, E_icode, E_ifun, E_valC, E_valA, E_valB, E_dstE, E_dstM, E_srcA, E_srcB, e_valE,aluA, aluB, alufun, set_CC, ZF, SF, OF, e_Cnd, e_dstE;
        int M_stat, M_icode, M_ifun, M_Cnd, M_valE, M_valA, M_dstE, M_dstM,m_valM,mem_addr,mem_read,mem_write,dmem_error;
        int W_stat, W_icode, W_valE, W_valM, W_dstE, W_dstM;
        int F_stall, D_stall, D_bubble, E_bubble,M_bubble,W_stall;
        //currentcycle
        int cycle = 0;
        //HCL
        const int INOP = 0;
        const int IHALT = 1;
        const int IRRMOVL = 2;
        const int IIRMOVL = 3;
        const int IRMMOVL = 4;
        const int IMRMOVL = 5;
        const int IOPL = 6;        
        const int IJXX = 7;        
        const int ICALL = 8;
        const int IRET = 9;
        const int IPUSHL=0xA;
        const int IPOPL = 0xB;
        const int FNONE = 0;        
        const int RNONE = 0xF;
        const int ALUADD = 0;
        const int SAOK = 1;
        const int SADR = 2;
        const int SINS = 3;
        const int SHLT = 4;
        const int SBUB = 5;
        const int SSTA = 6;
        const int REAX = 0;
        const int RECX = 1;
        const int REDX = 2;
        const int REBX = 3;
        const int RESP = 4;
        const int REBP = 5;
        const int RESI = 6;
        const int REDI = 7;
        int[] rreg=new int[20];  //real value of registers;
        //totalinstructionnumber
        int totalinstru = 0; //total instruction 
        int totalmem = 0; //size of array mapmem
        //address->row row start from index 1    maps to the first instruction
        int[] addrrow = new int[MAXINSTRUCTIONNUMBER]; 
        int[] address = new int[MAXINSTRUCTIONNUMBER];
        int[] icode = new int[MAXINSTRUCTIONNUMBER];
        int[] ifun = new int[MAXINSTRUCTIONNUMBER];
        int[] rA = new int[MAXINSTRUCTIONNUMBER];
        int[] rB = new int[MAXINSTRUCTIONNUMBER];
        int[] valC = new int[MAXINSTRUCTIONNUMBER];
        int[] valP = new int[MAXINSTRUCTIONNUMBER];//save origin instruction     
        //memory
        const int maxhash = 20000;        
        int[] rmem = new int[maxhash+1]; //real memory
        int[,] mapmem = new int[maxhash+1,2];  //if address>maxhash   save hear       
        const int INF = 0x7FFFFFFF;        
        int nowinstru = 0;
        int F_instru = INTMIN, D_instru = INTMIN, E_instru = INTMIN, M_instru = INTMIN, W_instru = INTMIN;  //instruction of each stage
        int[] instrulong={1,1,2,6,6,6,2,5,5,1,2,2};
        string[] state = { "", "SAOK", "SADR", "SINS", "SHLT", "SBUB","SSTA" };
        string[] alufunction = { "+", "-", "&", "^" };
        int finish = 0;
        int reset = 0;
        int[,] cycins = new int[MAXINSTRUCTIONNUMBER,5]; //0 F 1 D 2 E 3 M 4 W        
        char[,] stagecyc = new char[5555, 5555]; //save stage F D E M W
        string[] bel = new string[5555]; //save instruction ID and its content 
        int firstins = 0; //first cycle to be valid in the FDEMW diagram
        //Fetchsave
        int[] F_predPCsave = new int[MAXINSTRUCTIONNUMBER];
        int[] F_PCsave = new int[MAXINSTRUCTIONNUMBER];
        int[] F_icodesave = new int[MAXINSTRUCTIONNUMBER];
        int[] F_ifunsave = new int[MAXINSTRUCTIONNUMBER];
        int[] instr_validsave = new int[MAXINSTRUCTIONNUMBER];
        int[] imem_errorsave = new int[MAXINSTRUCTIONNUMBER];
        int[] F_valCsave = new int[MAXINSTRUCTIONNUMBER];
        int[] F_valPsave = new int[MAXINSTRUCTIONNUMBER];
        int[] F_rAsave = new int[MAXINSTRUCTIONNUMBER];
        int[] F_rBsave = new int[MAXINSTRUCTIONNUMBER];
        //Decodesave
        int[] D_icodesave = new int[MAXINSTRUCTIONNUMBER];
        int[] D_ifunsave = new int[MAXINSTRUCTIONNUMBER];
        int[] D_valCsave = new int[MAXINSTRUCTIONNUMBER];
        int[] D_valPsave = new int[MAXINSTRUCTIONNUMBER];
        int[] d_valAsave = new int[MAXINSTRUCTIONNUMBER];
        int[] d_valBsave = new int[MAXINSTRUCTIONNUMBER];
        int[] d_srcAsave = new int[MAXINSTRUCTIONNUMBER];
        int[] d_srcBsave = new int[MAXINSTRUCTIONNUMBER];
        int[] d_dstEsave = new int[MAXINSTRUCTIONNUMBER];
        int[] d_dstMsave = new int[MAXINSTRUCTIONNUMBER];
        int[] D_rAsave = new int[MAXINSTRUCTIONNUMBER];
        int[] D_rBsave = new int[MAXINSTRUCTIONNUMBER];
        //Executesave
        int[] E_icodesave = new int[MAXINSTRUCTIONNUMBER];
        int[] E_ifunsave = new int[MAXINSTRUCTIONNUMBER];
        int[] E_valCsave = new int[MAXINSTRUCTIONNUMBER];
        int[] E_valAsave = new int[MAXINSTRUCTIONNUMBER];
        int[] E_valBsave = new int[MAXINSTRUCTIONNUMBER];
        int[] e_dstEsave = new int[MAXINSTRUCTIONNUMBER];
        int[] E_dstEsave = new int[MAXINSTRUCTIONNUMBER];
        int[] E_dstMsave = new int[MAXINSTRUCTIONNUMBER];
        int[] E_srcAsave = new int[MAXINSTRUCTIONNUMBER];
        int[] E_srcBsave = new int[MAXINSTRUCTIONNUMBER];
        int[] e_valEsave = new int[MAXINSTRUCTIONNUMBER];
        int[] aluAsave = new int[MAXINSTRUCTIONNUMBER];
        int[] aluBsave = new int[MAXINSTRUCTIONNUMBER];
        int[] alufunsave = new int[MAXINSTRUCTIONNUMBER];
        int[] set_CCsave = new int[MAXINSTRUCTIONNUMBER];
        int[] ZFsave = new int[MAXINSTRUCTIONNUMBER];
        int[] SFsave = new int[MAXINSTRUCTIONNUMBER];
        int[] OFsave = new int[MAXINSTRUCTIONNUMBER];
        int[] e_Cndsave = new int[MAXINSTRUCTIONNUMBER];
        //Memorysave
        int[] M_icodesave = new int[MAXINSTRUCTIONNUMBER];
        int[] M_ifunsave = new int[MAXINSTRUCTIONNUMBER];
        int[] M_Cndsave = new int[MAXINSTRUCTIONNUMBER];
        int[] M_valEsave = new int[MAXINSTRUCTIONNUMBER];
        int[] M_valAsave = new int[MAXINSTRUCTIONNUMBER];
        int[] M_dstEsave = new int[MAXINSTRUCTIONNUMBER];
        int[] M_dstMsave = new int[MAXINSTRUCTIONNUMBER];
        int[] m_valMsave = new int[MAXINSTRUCTIONNUMBER];
        int[] mem_addrsave = new int[MAXINSTRUCTIONNUMBER];
        int[] mem_readsave = new int[MAXINSTRUCTIONNUMBER];
        int[] mem_writesave = new int[MAXINSTRUCTIONNUMBER];
        int[] dmem_errorsave = new int[MAXINSTRUCTIONNUMBER];
        //Writesave
        int[] W_icodesave = new int[MAXINSTRUCTIONNUMBER];
        int[] W_valEsave = new int[MAXINSTRUCTIONNUMBER];
        int[] W_valMsave = new int[MAXINSTRUCTIONNUMBER];
        int[] W_dstEsave = new int[MAXINSTRUCTIONNUMBER];
        int[] W_dstMsave = new int[MAXINSTRUCTIONNUMBER];
        //instructionsave
        int[] F_instrusave = new int[MAXINSTRUCTIONNUMBER];
        int[] D_instrusave = new int[MAXINSTRUCTIONNUMBER];
        int[] E_instrusave = new int[MAXINSTRUCTIONNUMBER];
        int[] M_instrusave = new int[MAXINSTRUCTIONNUMBER];
        int[] W_instrusave = new int[MAXINSTRUCTIONNUMBER];
        //status save
        int[] F_statsave = new int[MAXINSTRUCTIONNUMBER];
        int[] D_statsave = new int[MAXINSTRUCTIONNUMBER];
        int[] E_statsave = new int[MAXINSTRUCTIONNUMBER];
        int[] M_statsave = new int[MAXINSTRUCTIONNUMBER];
        int[] W_statsave = new int[MAXINSTRUCTIONNUMBER];
        //register save
        int[,] regsave = new int[MAXINSTRUCTIONNUMBER, 8];
        //slt
        int[] slt = new int[MAXINSTRUCTIONNUMBER];
        bool todo;
        bool pause = false;
        bool clean = true;
        int notclean=0;
        private void initstate()
        {            
            try
            {
                if (clean==true) for (int i = 0; i < totalinstru; i++) slt[i] = 0; //not break point
                for (int i = 0; i < maxhash; i++) rmem[i] = 0;
                firstins = -1;
                for (int i = 0; i < 5555; i++)
                {
                    bel[i] = "";
                    for (int j = 0; j < 5555; j++)
                        stagecyc[i, j] = ' ';
                }
                TextBox[,] txtstage = {{txtfive1,txtfive2,txtfive3,txtfive4,txtfive5,txtfive6},
                                  {txtfour1,txtfour2,txtfour3,txtfour4,txtfour5,txtfour6},
                                  {txtthree1,txtthree2,txtthree3,txtthree4,txtthree5,txtthree6},
                                  {txttwo1,txttwo2,txttwo3,txttwo4,txttwo5,txttwo6},
                                  {txtone1,txtone2,txtone3,txtone4,txtone5,txtone6},
                                  {txtzero1,txtzero2,txtzero3,txtzero4,txtzero5,txtzero6}};
                Label[] lblinstruction = { lblins0, lblins1, lblins2, lblins3, lblins4, lblins5 };
                for (int i = 0; i <= cycle; i++)
                {
                    for (int j = 0; j < 5; j++)
                        cycins[i, j] = INTMIN;
                }
                for (int i = 0; i < 6; i++)
                {
                    txtstage[i, 0].Text = txtstage[i, 1].Text = "F";
                    txtstage[i, 2].Text = "D";
                    txtstage[i, 3].Text = "E";
                    txtstage[i, 4].Text = "M";
                    txtstage[i, 5].Text = "W";
                    for (int j = 0; j < 6; j++) txtstage[i, j].Visible = true;
                    lblinstruction[i].Visible = true;
                    lblinstruction[i].Text = "ins" + (5 - i).ToString();
                }
                F_icode = F_ifun = instr_valid = imem_error = F_valC = F_valP = F_PC = F_predPC = F_rA = F_rB = INTMIN;
                D_icode = D_ifun = D_valC = D_valP = d_srcA = d_srcB = d_valA = d_valB = d_dstE = d_dstM = INTMIN;
                D_rA = D_rB = RNONE;
                E_icode = E_ifun = E_valC = E_valA = E_valB = E_dstE = E_dstM = E_srcA = E_srcB = e_valE = aluA = aluB = alufun = set_CC = ZF = SF = OF = e_Cnd = e_dstE = INTMIN;
                M_icode = M_ifun = M_Cnd = M_valE = M_valA = M_dstE = M_dstM = m_valM = mem_addr = mem_read = mem_write = dmem_error = INTMIN;
                W_icode = W_valE = W_valM = W_dstE = W_dstM = INTMIN;
                F_stall = D_stall = D_bubble = E_bubble = M_bubble = W_stall = 0;
                D_stat = E_stat = M_stat = W_stat = SAOK;
                F_instru = D_instru = E_instru = M_instru = W_instru = INTMIN;
                SF = OF = 0;
                ZF = 1;
                finish = 0;
                if (totalinstru != 0 && reset == 0)
                {
                    for (int i = 0; i < totalinstru; i++)
                    {
                        addrrow[address[i]] = 0;
                    }//clear origin map
                }
                if (reset == 0) totalinstru = 0;
                totalmem = 0;
                cycle = 0;
            }
            catch(System.Exception err)
            {
                return;
            }            
        }
        private string handlehex(int x)
        {
            try
            {
                if (x == INTMIN) return "NULL";
                return "0x" + x.ToString("x").PadLeft(8, '0');
            }
            catch (System.Exception err)
            {
                return "NULL";
            }               
        }
        private string handledecimal(int x)
        {
            try
            {
                if (x == INTMIN) return "NULL";
                return x.ToString();
            }
            catch (System.Exception err)
            {
                return "NULL";
            }               
        }
        private string handlehex2(int x)
        {
            try
            {
                if (x == INTMIN) return "NULL";
                return "0x" + x.ToString("x");
            }
            catch (System.Exception err)
            {
                return "NULL";
            }               
        }
        private string handlehex3(int x)
        {
            try
            {
                if (x == 1) return "true"; else return "false";
            }
            catch(System.Exception err)
            {
                return "NULL";
            }            
        }
        private string handleinstruction(int x)
        {
            try
            {
                if (x == INTMIN) return "NULL";
                return instruction[x];
            }
            catch (System.Exception err)
            {
                return "NULL";
            }

        }
        private int judgeempty(char c)
        {
            try
            {
                if (c.Equals(' ')) return INTMIN;
                return Convert.ToInt32(c.ToString(), 16);
            }
            catch(System.Exception err)
            {
                return INTMIN; 
            }
            
        }
        private string littleendian(string s1)
        {
            try
            {
                string s2 = "";
                s2 = s2 + s1.Substring(6, 2);
                s2 = s2 + s1.Substring(4, 2);
                s2 = s2 + s1.Substring(2, 2);
                s2 = s2 + s1.Substring(0, 2);
                s2 = s2.ToUpper();
                return s2;
            }
            catch(System.Exception err)
            {
                return "";
            }

        }
        int operate(int valB,int valA,int fun)
        {
            try
            {
                if (fun == 0) return valB + valA;
                else if (fun == 1) return valB - valA;
                else if (fun == 2) return valB & valA;
                else if (fun == 3) return valB ^ valA;
                return 0;
            }
            catch(System.Exception err)
            {
                return 0;
            }

        }
        private string process(string s1)
        {
            try
            {
                string s2 = "";
                int i = 0;
                while (i < s1.Length)
                {
                    s2 = s2 + s1[i];
                    int j = i;
                    while ((s1[i].Equals(' ') || s1[i].Equals('\t')) && j + 1 < s1.Length && (s1[j + 1].Equals(' ') || s1[j + 1].Equals('\t'))) j++;
                    i = j + 1;
                }
                //s1 = s1.Trim();
                for (i = 0; i < s1.Length; i++)
                {
                    if (s1[i].Equals('|')) return s1.Substring(i + 1);
                }
                return s1;

            }
            catch(System.Exception err)
            {
                return "";
            }
        }
        private void translateinstruction()
        {            
            try
            {
                for (int i = 0; i < totalinstru; i++)
                {
                    icode[i] = INTMIN; ifun[i] = INTMIN; rA[i] = INTMIN; rB[i] = INTMIN; valC[i] = INTMIN; valP[i] = INTMIN;//before continue;
                    int j;
                    for (j = 0; j < instruction[i].Length && instruction[i][j].Equals(' '); j++) ;
                    if (j == instruction[i].Length) continue;
                    if (instruction[i][j].Equals('|')) continue;
                    address[i] = Convert.ToInt32(instruction[i].Substring(j, 5), 16);
                    addrrow[address[i]] = i + 1;   //  attention! i+1
                    //st+7                
                    if (instruction[i][j + 7].Equals(' ')) continue; //0x000: 
                    icode[i] = Convert.ToInt32(instruction[i].Substring(j + 7, 1), 16);
                    ifun[i] = instruction[i][j + 8] - '0';
                    if (i > 0) valP[i - 1] = address[i];
                    if (icode[i] >= 2 && icode[i] <= 6 || (icode[i] >= 10))
                    {
                        rA[i] = judgeempty(instruction[i][j + 9]);
                        rB[i] = judgeempty(instruction[i][j + 10]);
                    }//rrmovl irmovl rmmovl mrmovl cmovXX pushl popl                
                    if (icode[i] == 7 || icode[i] == 8) valC[i] = Convert.ToInt32(littleendian(instruction[i].Substring(j + 9, 8)), 16);
                    //jXX call
                    if (icode[i] >= 3 && icode[i] <= 5) valC[i] = Convert.ToInt32(littleendian(instruction[i].Substring(j + 11, 8)), 16);
                    //irmovl rmmovl mrmovl 
                }
                totalmem = 0;
            }
            catch(System.Exception err)
            {
                return;
            }
        }
        private void lst_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            try
            {
                e.ItemHeight = 17;        
            }
            catch (System.Exception err)
            {
                return;
            }
            
        }

        private void lst_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.DrawBackground();
                e.DrawFocusRectangle();
                int cnt = lst.SelectedIndices.Count;
                e.Graphics.DrawString(lst.Items[e.Index].ToString(), e.Font, new SolidBrush(Color.Blue), e.Bounds);
            }
            catch(System.Exception err)
            {
                return;
            }
        }

        private void rch_VScroll(object sender, EventArgs e)
        {
            try
            {
                Point p = rch.Location;
                int first = rch.GetCharIndexFromPosition(p);
                int fstline = rch.GetLineFromCharIndex(first);
                for (int i = 0; i < LINENUM; i++)
                {
                    lst.Items[i] = " " + (fstline + i - 10).ToString();
                    lststage.Items[i] = "";
                    if (slt[fstline + i - 11] == 1) lst.SelectedIndices.Add(i);
                    else lst.SelectedIndices.Remove(i);
                    if (W_instru == fstline + i - 11) lststage.Items[i] = lststage.Items[i] + " W";
                    if (M_instru == fstline + i - 11) lststage.Items[i] = lststage.Items[i] + " M";
                    if (E_instru == fstline + i - 11) lststage.Items[i] = lststage.Items[i] + " E";
                    if (D_instru == fstline + i - 11) lststage.Items[i] = lststage.Items[i] + " D";
                    if (F_instru == fstline + i - 11) lststage.Items[i] = lststage.Items[i] + " F";
                }            
            }
            catch(System.Exception err)
            {
                return;
            }
        }

        private int checkinput(string input)
        {
            try
            {
                input = input.Trim();
                if (input[0].Equals('|')) return 0;
                input = input.Substring(7);
                if (input.Substring(0, 13).Equals("00           ")) return 1;
                if (input.Substring(0, 13).Equals("10           ")) return 1;
                if (input[0].Equals('2') && input[1] - '0' >= 0 && input[1] - '6' <= 0 && (input[2] - '0' >= 0 && input[2] - '7' <= 0) && (input[3] - '0' >= 0 && input[3] - '7' <= 0) && input.Substring(4, 9).Equals("         ")) return 1;//rrmov cmov
                if (input.Substring(0, 2).Equals("30") && input[2].Equals('F') && (input[3] - '0' >= 0 && input[3] - '7' <= 0))
                {
                    for (int i = 4; i <= 11; i++) if (input[i].Equals(' ')) return 0;
                    if (input[12].Equals(' ')) return 1;
                    return 0;
                }//irmov
                if ((input.Substring(0, 2).Equals("40") || input.Substring(0, 2).Equals("50")) && (input[2] - '0' >= 0 && input[2] - '7' <= 0) && (input[3] - '0' >= 0 && input[3] - '7' <= 0))
                {
                    for (int i = 4; i <= 11; i++) if (input[i].Equals(' ')) return 0;
                    return 1;
                }//rmmov mrmov
                if (input[0].Equals('6') && input[1] - '0' >= 0 && input[1] - '3' <= 0 && input[2] - '0' >= 0 && input[2] - '7' <= 0 && input[3] - '0' >= 0 && input[3] - '7' <= 0 && input.Substring(4, 9).Equals("         ")) return 1; //OPL
                if (input[0].Equals('7') && input[1] - '0' >= 0 && input[1] - '6' <= 0)
                {
                    for (int i = 2; i <= 9; i++) if (input[i].Equals(' ')) return 0;
                    if (input.Substring(10, 3).Equals("   ")) return 1;
                    return 0;
                }//JXX
                if (input[0].Equals('8') && input[1].Equals('0'))
                {
                    for (int i = 2; i <= 9; i++) if (input[i].Equals(' ')) return 0;
                    if (input.Substring(10, 3).Equals("   ")) return 1;
                    return 0;
                }//call
                if (input.Substring(0, 13).Equals("90           ")) return 1;
                if ((input[0].Equals('A') || input[0].Equals('B')) && input[1].Equals('0') && input[2] - '0' >= 0 && input[2] - '7' <= 0 && input[3].Equals('F') && input.Substring(4, 9).Equals("         ")) return 1; //push pop            
                return 0;

            }
            catch(System.Exception err)
            {
                return 0;
            }
        }
        private void btnopen_Click(object sender, EventArgs e)
        {            
            try
            {
                for (int i = 0; i < LINENUM; i++)
                {
                    lst.Items[i] = " " + (i + 1).ToString();
                }
                rch.Text = "";
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "所有文件(*.*)|*.*";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    initstate();
                    try
                    {
                        string filename = openFileDialog1.FileName;
                        try
                        {
                            int id = 0;
                            System.IO.StreamReader reader = new System.IO.StreamReader(filename, System.Text.Encoding.Default);
                            string input;
                            while ((input = reader.ReadLine()) != null)
                            {
                                instruction[id++] = input;
                                int legal = checkinput(input);
                                if (legal == 0) { id--; continue; }
                                int st = 0;
                                for (st = 0; st < input.Length && !input[st].Equals('|'); st++) ;
                                if (st == input.Length) { id--; continue; }
                                rch.Text = rch.Text + input.Substring(st + 1).PadRight(82, ' ') + "\n";
                            }
                            totalinstru = id;
                            for (int i = 0; i < id; i++)
                            {
                                int len = rch.Lines[i].Length;
                                int st = rch.GetFirstCharIndexFromLine(i);
                                int j = st, tmp = 0;
                                while (j - st < len && (rch.Lines[i][j - st].Equals(' ') || rch.Lines[i][j-st].Equals('\t'))) j++;
                                if (j - st == len) continue;                                
                                tmp = j - st;
                                st = j;
                                string two = rch.Lines[i].Substring(tmp, Math.Min(2, len - tmp));
                                string three = rch.Lines[i].Substring(tmp, Math.Min(3, len - tmp));
                                string four = rch.Lines[i].Substring(tmp, Math.Min(4, len - tmp));
                                string five = rch.Lines[i].Substring(tmp, Math.Min(5, len - tmp));
                                string six = rch.Lines[i].Substring(tmp, Math.Min(6, len - tmp));
                                if (four.Equals("halt"))
                                {
                                    rch.Select(st, 4);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (three.Equals("nop"))
                                {
                                    rch.Select(st, 3);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (six.Equals("rrmovl"))
                                {
                                    rch.Select(st, 6);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (six.Equals("irmovl"))
                                {
                                    rch.Select(st, 6);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (six.Equals("rmmovl"))
                                {
                                    rch.Select(st, 6);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (six.Equals("mrmovl"))
                                {
                                    rch.Select(st, 6);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (four.Equals("addl"))
                                {
                                    rch.Select(st, 4);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (four.Equals("subl"))
                                {
                                    rch.Select(st, 4);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (four.Equals("andl"))
                                {
                                    rch.Select(st, 4);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (four.Equals("xorl"))
                                {
                                    rch.Select(st, 4);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (three.Equals("jmp")) //jmp 0x
                                {
                                    rch.Select(st, 3);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (three.Equals("jle")) //jmp 0x
                                {
                                    rch.Select(st, 3);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (two.Equals("jl")) //jmp 0x
                                {
                                    rch.Select(st, 2);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (two.Equals("je")) //jmp 0x
                                {
                                    rch.Select(st, 2);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (three.Equals("jne")) //jmp 0x
                                {
                                    rch.Select(st, 3);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (three.Equals("jge")) //jmp 0x
                                {
                                    rch.Select(st, 3);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (two.Equals("jg")) //jmp 0x
                                {
                                    rch.Select(st, 2);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (six.Equals("cmovle"))
                                {
                                    rch.Select(st, 6);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (five.Equals("cmovl"))
                                {
                                    rch.Select(st, 5);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (five.Equals("cmove"))
                                {
                                    rch.Select(st, 5);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (six.Equals("cmovne"))
                                {
                                    rch.Select(st, 6);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (six.Equals("cmovge"))
                                {
                                    rch.Select(st, 5);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (five.Equals("cmovg"))
                                {
                                    rch.Select(st, 5);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (four.Equals("call")) //jmp 0x
                                {
                                    rch.Select(st, 4);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (three.Equals("ret"))
                                {
                                    rch.Select(st, 3);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (five.Equals("pushl"))
                                {
                                    rch.Select(st, 5);
                                    rch.SelectionColor = Color.Blue;
                                }
                                else if (four.Equals("popl"))
                                {
                                    rch.Select(st, 5);
                                    rch.SelectionColor = Color.Blue;
                                }
                                rch.Select(0, 0);
                                for (int k = tmp; k < len - 3; k++)
                                {
                                    for (int REGID = 0; REGID < 8; REGID++)
                                    {
                                        if (rch.Lines[i].Substring(k, 4).Equals(reg[REGID]))
                                        {
                                            rch.Select(k - tmp + st, 4);
                                            rch.SelectionColor = Color.Green;
                                            rch.Select(0, 0);
                                            break;
                                        }
                                    }

                                    if (rch.Lines[i][k] - '0' >= 0 && rch.Lines[i][k] - '0' <= 9 || (rch.Lines[i][k].Equals('x') && k != tmp && rch.Lines[i][k - 1].Equals('0')) || rch.Lines[i][k].Equals('-') || (rch.Lines[i][k] - 'A' >= 0 && rch.Lines[i][k] - 'F' <= 0))
                                    {
                                        rch.Select(k - tmp + st, 1);
                                        rch.SelectionColor = Color.Red;
                                        rch.Select(0, 0);
                                    }
                                }
                                rch.Select(0, 0);
                            }
                            reader.Close();
                        }
                        catch (System.IO.FileNotFoundException err2)
                        {
                            MessageBox.Show(err2.ToString());
                            btngo.Enabled = btnforward.Enabled = btnbackward.Enabled = btnrunall.Enabled = false; todo = false;
                            btnpause.Enabled = btnresume.Enabled = btnstop.Enabled = false;
                            rch.Text = "";
                            return;
                        }
                    }
                    catch (System.IndexOutOfRangeException erroutrange)
                    {
                        MessageBox.Show("不是标准的输入文件", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btngo.Enabled = btnforward.Enabled = btnbackward.Enabled = btnrunall.Enabled = false; todo = false;
                        btnpause.Enabled = btnresume.Enabled = btnstop.Enabled = false;
                        rch.Text = "";
                        return;
                    }
                    btngo.Enabled = btnforward.Enabled = btnbackward.Enabled = btnrunall.Enabled = true; todo = false;
                    btnpause.Enabled = btnresume.Enabled = btnstop.Enabled = true;
                    nowinstru = 0;
                    translateinstruction(); //translate instruction (code)                       
                }
                else
                {
                    btngo.Enabled = btnforward.Enabled = btnbackward.Enabled = btnrunall.Enabled = false; todo = false;
                    btnpause.Enabled = btnresume.Enabled = btnstop.Enabled = false;
                }
                /*rmem[20] = 0xd;
                rmem[24] = 0xc0;
                rmem[28] = 0xb00;
                rmem[32] = 0xa000;*/
            }
            catch(System.Exception err)
            {
                return;
            }
        }

        private void rch_Click(object sender, EventArgs e)
        {
            try
            {
                if (rch.Text.Equals(""))
                {
                    return;
                }

                int ind = rch.SelectionStart;
                int row = rch.GetLineFromCharIndex(ind);
                int fst = rch.GetFirstCharIndexFromLine(row);
                if (lastrow != INTMIN)
                {
                    rch.Select(rch.GetFirstCharIndexFromLine(lastrow), rch.Lines[lastrow].Length);
                    rch.SelectionBackColor = Color.White;
                }
                rch.Select(fst, rch.Lines[row].Length);
                rch.SelectionBackColor = Color.FromArgb(255, 255, 128);
                rch.Select(ind, 0);
                lastrow = row;
            }
            catch (System.IndexOutOfRangeException err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        private void btnstep_Click(object sender, EventArgs e)
        {
            try
            {
                if (finish == 1) return; //finish
                if (cycle == 0 && icode[nowinstru] == INTMIN)
                {
                    nowinstru++;
                    return;     //no use
                }
                cycle++;//currentcycle++
                txtcurrentcycle.Text = cycle.ToString();
                if (cycle >= 5) //W<-M 
                {
                    if (M_stat != SBUB && M_bubble != 1 && W_stall != 1 && M_stat != SADR && M_stat != SHLT) //no dmem error but bubble to E and to M and to W e.g. ret ok
                    {
                        W_instru = M_instru;
                        W_stat = M_stat; W_icode = M_icode; W_valE = M_valE; W_valM = m_valM; W_dstE = M_dstE; W_dstM = M_dstM;
                        if (dmem_error == 1)
                        {
                            W_stat = SADR; //out of range                             
                            W_icode = W_valE = W_valM = W_dstE = W_dstM = INTMIN;
                            W_stall = 1;
                            M_bubble = 1;
                        }
                        if (W_stat == SAOK)
                        {
                            if (W_icode == IOPL || W_icode == IRRMOVL || W_icode == IIRMOVL || W_icode == IPUSHL || W_icode == IPOPL || W_icode == ICALL || W_icode == IRET)
                                rreg[W_dstE] = W_valE;
                            if (W_icode == IMRMOVL || W_icode == IPOPL)
                                rreg[W_dstM] = W_valM;
                        }//write to registers;
                    }
                    else if (M_bubble == 1 || M_stat == SBUB) //E bubble or dmem error bubble into M W
                    {
                        W_instru = INTMIN;
                        W_stat = SBUB;
                        W_icode = W_valE = W_valM = W_dstE = W_dstM = INTMIN;
                    }
                    else if (W_stall == 1)
                    {
                        W_stat = SSTA; //stall write stage prevent modification of condition codes caused by the following instructions
                    }
                    else if (M_stat == SADR)   //address
                    {
                        W_instru = INTMIN;
                        W_stat = SADR;
                        W_icode = W_valE = W_valM = W_dstE = W_dstM = INTMIN;
                    }
                    else if (M_stat == SHLT)   //halt
                    {
                        W_instru = INTMIN;
                        W_stat = SHLT;
                        W_icode = M_icode; W_valE = M_valE; W_valM = m_valM; W_dstE = M_dstE; W_dstM = M_dstM;
                    }
                }

                //save last M_icode;
                int lastM_icode = M_icode, lastM_Cnd = M_Cnd;
                if (cycle >= 4)//M<-E
                {
                    if (E_bubble != 1)
                    {
                        M_instru = E_instru;
                        M_stat = E_stat; M_icode = E_icode; M_Cnd = e_Cnd; M_valE = e_valE; M_valA = E_valA; M_dstE = e_dstE; M_dstM = E_dstM;
                        m_valM = INTMIN; dmem_error = INTMIN;
                        mem_addr = INTMIN;
                        mem_read = mem_write = INTMIN;
                        if (E_stat == SAOK && M_bubble != 1 && M_Cnd != 0) //normal jne 0x7F24 dmemerror
                        {
                            int needmemory = 1;
                            if (M_icode == IRMMOVL || M_icode == IMRMOVL || M_icode == IPUSHL || M_icode == ICALL) //RM MR PUSH CALL
                            {
                                mem_addr = M_valE;
                            }
                            else if (M_icode == IPOPL || M_icode == IRET) //POP RET
                            {
                                mem_addr = M_valA;
                            }
                            else needmemory = 0;
                            if (needmemory == 1 && M_icode != IOPL && M_icode != IRRMOVL && M_icode != IIRMOVL && M_icode != IJXX) //!=OP RR IR JXX no memory operation
                            {
                                if (mem_addr < Convert.ToInt32(txtminmem.Text) || mem_addr > Convert.ToInt32(txtmaxmem.Text))
                                {
                                    dmem_error = 1;
                                } //address out of range                            
                                if (dmem_error != 1)
                                {
                                    if (M_icode == IMRMOVL || M_icode == IPOPL || M_icode == IRET) //MR POP RET  
                                    //read memory
                                    {
                                        txtM_optab.Text = "Read";
                                        mem_read = 1;
                                        if (mem_addr <= maxhash)
                                        {
                                            m_valM = rmem[mem_addr]; //real memory                                         
                                        }
                                        else
                                        {
                                            for (int i = 0; i < totalmem; i++)
                                            {
                                                if (mapmem[i, 0] == mem_addr)
                                                {
                                                    m_valM = mapmem[i, 1];
                                                }
                                            }
                                        }
                                        txtm_valMtab.Text = m_valM.ToString();
                                    }
                                    else
                                    //write to memory
                                    {
                                        txtM_optab.Text = "Write";
                                        txtm_valMtab.Text = "NULL";
                                        mem_write = 1;
                                        if (mem_addr <= maxhash)
                                        {
                                            rmem[mem_addr] = M_valA; //real memory 
                                        }
                                        else
                                        {
                                            int flag = 0;
                                            for (int i = 0; i < totalmem; i++)
                                            {
                                                if (mapmem[i, 0] == mem_addr)
                                                {
                                                    mapmem[i, 1] = M_valA;
                                                    flag = 1;
                                                }
                                            }
                                            if (flag == 0)
                                            {
                                                mapmem[++totalmem, 0] = mem_addr;
                                                mapmem[totalmem, 1] = M_valA;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    txtM_optab.Text = "NONE";
                                }
                            }
                        }
                    }
                    else
                    {
                        txtM_optab.Text = "NONE";
                        txtm_valMtab.Text = "NULL";
                        M_instru = INTMIN;
                        M_stat = SBUB;
                        if (E_stat != SBUB) M_stat = E_stat;
                        M_icode = M_Cnd = M_valE = M_valA = M_dstE = M_dstM = INTMIN;
                        m_valM = INTMIN; dmem_error = INTMIN;
                        mem_read = mem_write = INTMIN;
                        if (E_stat == SHLT)
                        {
                            M_instru = E_instru;
                            M_stat = E_stat; M_icode = E_icode; M_Cnd = e_Cnd; M_valE = e_valE; M_valA = E_valA; M_dstE = e_dstE; M_dstM = E_dstM;
                        }//SHLT
                    }
                }

                //-------------------------------------------------------------------------------------

                //t-1  save last E_icode
                int lastE_icode = E_icode, laste_Cnd = e_Cnd, lastE_dstM = E_dstM;
                if (cycle >= 3) //E<-D
                {
                    if ((E_icode == IJXX && e_Cnd == 0) ||
                         ((E_icode == IMRMOVL || E_icode == IPOPL)/*in*/  && (E_dstM == d_srcA || E_dstM == d_srcB)))
                    {
                        E_bubble = 1;
                    }
                    else
                    {
                        E_bubble = 0;
                    }
                    if (E_bubble == 0 && (D_stat == SAOK || D_stat == SSTA)) //not bubble
                    {
                        E_instru = D_instru;
                        E_stat = SAOK;
                        if (D_stat != SSTA) E_stat = D_stat;
                        E_icode = D_icode; E_ifun = D_ifun; E_valC = D_valC; E_valA = d_valA; E_valB = d_valB; E_dstE = d_dstE; E_dstM = d_dstM; E_srcA = d_srcA; E_srcB = d_srcB;
                        if (E_icode == IIRMOVL || E_icode == IRMMOVL || E_icode == IMRMOVL)
                        {
                            aluA = E_valC;
                        }
                        else if (E_icode == IRRMOVL || E_icode == IOPL)
                        {
                            aluA = E_valA;
                        }
                        else if (E_icode == IRET || E_icode == IPOPL)
                        {
                            aluA = 4;
                        }
                        else if (E_icode == IPUSHL || E_icode == ICALL)
                        {
                            aluA = -4;
                        }
                        //alu B
                        if (E_icode == IRRMOVL || E_icode == IIRMOVL)
                        {
                            aluB = 0;
                        }
                        else if (E_icode == IRMMOVL || E_icode == IMRMOVL || E_icode == IOPL || E_icode == ICALL || E_icode == IPUSHL || E_icode == IRET || E_icode == IPOPL)
                        {
                            aluB = E_valB;
                        }
                        if (E_icode == IOPL) alufun = E_ifun; else alufun = 0; //default:ALUADD
                        if (E_icode == IJXX && E_ifun == 0) alufun = INTMIN; //no need to operate
                        e_valE = operate(aluB, aluA, alufun);
                        if (E_icode == IOPL && W_stall != 1 && W_stat != SADR && W_stat != SINS && W_stat != SHLT && M_stat != SADR && M_stat != SINS && M_stat != SHLT && dmem_error != 1)
                        {
                            set_CC = 1;
                        }
                        else
                        {
                            set_CC = 0;
                        }//only IOPL will change the condition code 
                        if (set_CC == 1)
                        {
                            if (e_valE == 0) ZF = 1; else ZF = 0;
                            if (e_valE < 0) SF = 1; else SF = 0;
                            if ((E_valA < 0 == E_valB < 0) && (e_valE < 0 != E_valA < 0)) OF = 1; else OF = 0;//overflow flag
                        }//set condition code
                        e_Cnd = 1;
                        if ((E_icode == IJXX && E_ifun > 0) || (E_icode == IRRMOVL && E_ifun > 0)) //JXX (not JMP) cmovXX
                        {
                            if (E_ifun == 1) //less or equal
                            {
                                if (SF == 1 || ZF == 1) e_Cnd = 1; else e_Cnd = 0;
                            }
                            else if (E_ifun == 2) //less
                            {
                                if (SF == 1) e_Cnd = 1; else e_Cnd = 0;
                            }
                            else if (E_ifun == 3) //equal
                            {
                                if (ZF == 1) e_Cnd = 1; else e_Cnd = 0;
                            }
                            else if (E_ifun == 4) //not equal
                            {
                                if (ZF == 0) e_Cnd = 1; else e_Cnd = 0;
                            }
                            else if (E_ifun == 5) //greater or equal
                            {
                                if (ZF == 1 || (ZF == 0 && SF == 0)) //=0 >0
                                {
                                    e_Cnd = 1;
                                }
                                else
                                {
                                    e_Cnd = 0;
                                }
                            }
                            else //greater
                            {
                                if (ZF == 0 && SF == 0) e_Cnd = 1;
                                else e_Cnd = 0;
                            }
                        }
                        if (e_Cnd == 0) e_dstE = RNONE; else e_dstE = E_dstE;
                    }
                    else
                    {
                        E_instru = INTMIN;
                        E_stat = SBUB;
                        if (D_stat != SBUB && D_stat != SSTA) E_stat = D_stat; //SADR SHLT
                        E_icode = E_ifun = E_valC = E_valA = E_valB = e_dstE = E_dstE = e_valE = E_dstM = E_srcA = E_srcB = e_Cnd = aluA = aluB = alufun = INTMIN;
                        if (D_stat == SHLT)
                        {
                            E_instru = D_instru;
                            E_stat = D_stat; E_icode = D_icode; E_ifun = D_ifun; E_valC = D_valC; E_valA = d_valA; E_valB = d_valB; E_dstE = d_dstE; E_dstM = d_dstM; E_srcA = d_srcA; E_srcB = d_srcB;
                        }//halt save origin instruction
                    }
                }

                txte_Cndtab.Text = e_Cnd.ToString();
                //-------------------------------------------------------------------------------------


                int lastD_icode = D_icode, lastd_srcA = d_srcA, lastd_srcB = d_srcB;
                if (cycle >= 2) //D<-F
                {
                    int loaduse = 0;
                    if ((lastE_icode == IMRMOVL || lastE_icode == IPOPL) && (lastE_dstM == d_srcA || lastE_dstM == d_srcB)) loaduse = 1;
                    if ((lastE_icode == IJXX && laste_Cnd == 0) || (loaduse == 0 && (D_icode == IRET || lastE_icode == IRET || lastM_icode == IRET))) //not condition for a load use hazard
                    {
                        D_bubble = 1;
                    }
                    else
                    {
                        D_bubble = 0;
                    }
                    if (loaduse == 1) D_stall = 1; else D_stall = 0;
                    if (D_stall == 1)//stall LOAD USE
                    {
                        D_stat = SSTA;
                        D_stall = 0; //recover D_stall
                    }
                    else if (D_bubble == 1 || F_icode == 1)//D bubble or nop
                    {
                        D_instru = INTMIN;
                        D_stat = SBUB;
                        D_icode = D_ifun = D_rA = D_rB = D_valC = D_valP = d_valA = d_valB = d_srcA = d_srcB = d_dstE = d_dstM = INTMIN;
                    }
                    else if (F_icode == 0) //halt
                    {
                        D_instru = F_instru;
                        D_stat = SHLT;
                        D_icode = F_icode; D_ifun = F_ifun; D_rA = F_rA; D_rB = F_rB; D_valC = F_valC; D_valP = F_valP;
                    }
                    else if (imem_error == 1) //address exception wrong instruction
                    {
                        imem_error = 0;
                        D_instru = INTMIN;
                        D_stat = SADR;
                        D_icode = D_ifun = D_rA = D_rB = D_valC = D_valP = d_valA = d_valB = d_srcA = d_srcB = d_dstE = d_dstM = INTMIN;
                    }
                    else
                    {
                        D_instru = F_instru;
                        D_stat = SAOK;
                        D_icode = F_icode; D_ifun = F_ifun; D_rA = F_rA; D_rB = F_rB; D_valC = F_valC; D_valP = F_valP;
                    }
                    if (D_stat == SAOK || D_stat == SSTA)
                    {
                        //srcA
                        if (D_icode == IRRMOVL || D_icode == IRMMOVL || D_icode == IOPL || D_icode == IPUSHL)
                        {
                            d_srcA = D_rA;
                        }
                        else if (D_icode == IPOPL || D_icode == IRET)
                        {
                            d_srcA = RESP;
                        }
                        else d_srcA = RNONE;
                        //srcB
                        if (D_icode == IOPL || D_icode == IRMMOVL || D_icode == IMRMOVL)
                        {
                            d_srcB = D_rB;
                        }
                        else if (D_icode == IPUSHL || D_icode == IPOPL || D_icode == ICALL || D_icode == IRET)
                        {
                            d_srcB = RESP;
                        }
                        else d_srcB = RNONE;
                        //dstE
                        if (D_icode == IRRMOVL || D_icode == IIRMOVL || D_icode == IOPL)
                        {
                            d_dstE = D_rB;
                        }
                        else if (D_icode == IPUSHL || D_icode == IPOPL || D_icode == ICALL || D_icode == IRET)
                        {
                            d_dstE = RESP;
                        }
                        else d_dstE = RNONE;
                        //dstM
                        if (D_icode == IMRMOVL || D_icode == IPOPL)
                        {
                            d_dstM = D_rA;
                        }
                        else
                        {
                            d_dstM = RNONE;
                        }
                        //d_valA only RR OP RM MR PUSH POP RET CALL JXX
                        if (D_icode == IRRMOVL || D_icode == IOPL || D_icode == IRMMOVL || D_icode == IMRMOVL || D_icode == IPUSHL || D_icode == IPOPL || D_icode == IRET || D_icode == ICALL || D_icode == IJXX)
                        {
                            if (D_icode == ICALL || D_icode == IJXX) d_valA = D_valP;
                            else if (d_srcA == e_dstE) d_valA = e_valE;
                            else if (d_srcA == M_dstM) d_valA = m_valM;
                            else if (d_srcA == M_dstE) d_valA = M_valE;
                            else if (d_srcA == W_dstM) d_valA = W_valM;
                            else if (d_srcA == W_dstE) d_valA = W_valE;
                            else d_valA = rreg[d_srcA];
                        }
                        //d_valB; RM MR OP CALL  PUSH POP CALL RET
                        if (D_icode == IRMMOVL || D_icode == IMRMOVL || D_icode == IOPL || D_icode == ICALL || D_icode == IPUSHL || D_icode == IPOPL || D_icode == IRET)
                        {
                            if (d_srcB == e_dstE) d_valB = e_valE;
                            else if (d_srcB == M_dstM) d_valB = m_valM;
                            else if (d_srcB == M_dstE) d_valB = M_valE;
                            else if (d_srcB == W_dstM) d_valB = W_valM;
                            else if (d_srcB == W_dstE) d_valB = W_valE;
                            else d_valB = rreg[d_srcB];
                        }
                    }
                    //finish forwarding
                }

                //-------------------------------------------------------------------------------------

                if (((lastE_icode == IMRMOVL || lastE_icode == IPOPL) && (lastE_dstM == lastd_srcA || lastE_dstM == lastd_srcB)) || //load use
                    (lastD_icode == IRET || lastE_icode == IRET)) F_stall = 1; // elinimate lastM_icode load use
                else F_stall = 0;
                if (F_stall != 1) //not stall F 
                {
                    if (cycle >= 2) //F<-W
                    {
                        if (F_icode == IJXX || F_icode == ICALL) F_predPC = F_valC;
                        else F_predPC = F_valP;
                        if (M_icode == IJXX && M_Cnd == 0) F_PC = M_valA; //not taken branch next instruction valP
                        else if (W_icode == IRET) F_PC = W_valM; //current time
                        else F_PC = F_predPC;
                    }
                    else F_PC = address[nowinstru];
                    int index = 0;
                    if (F_PC != INTMIN) index = addrrow[F_PC];    //should be >=1 after add one
                    F_instru = index - 1; //delete one
                    if (F_instru == -1) F_instru = INTMIN;
                    F_icode = F_ifun = F_rA = F_rB = F_valC = F_valP = INTMIN; //init
                    imem_error = 0;
                    if (index == 0) //no this address
                    {
                        imem_error = 1;
                        if (M_bubble == 1) finish = 1; // dmem_error 
                    }
                    else
                    {
                        F_icode = icode[index - 1]; F_ifun = ifun[index - 1]; F_rA = rA[index - 1]; F_rB = rB[index - 1]; F_valC = valC[index - 1]; F_valP = valP[index - 1];
                    }
                }
                //save state to arry
                F_predPCsave[cycle] = F_predPC; F_PCsave[cycle] = F_PC; F_icodesave[cycle] = F_icode; F_ifunsave[cycle] = F_ifun; instr_validsave[cycle] = instr_valid; imem_errorsave[cycle] = imem_error; F_valCsave[cycle] = F_valC; F_valPsave[cycle] = F_valP; ; F_rAsave[cycle] = F_rA; F_rBsave[cycle] = F_rB;
                D_icodesave[cycle] = D_icode; D_ifunsave[cycle] = D_icode; D_valCsave[cycle] = D_valC; D_valPsave[cycle] = D_valP; d_valAsave[cycle] = d_valA; d_valBsave[cycle] = d_valB; d_srcAsave[cycle] = d_srcA; d_srcBsave[cycle] = d_srcB; d_dstEsave[cycle] = d_dstE; d_dstMsave[cycle] = d_dstM;
                D_rAsave[cycle] = D_rA; D_rBsave[cycle] = D_rB;
                E_icodesave[cycle] = E_icode; E_ifunsave[cycle] = E_ifun; E_valCsave[cycle] = E_valC; E_valAsave[cycle] = E_valA; E_valBsave[cycle] = E_valB; e_dstEsave[cycle] = e_dstE; E_dstEsave[cycle] = E_dstE;
                E_dstMsave[cycle] = E_dstM; E_srcAsave[cycle] = E_srcA; E_srcBsave[cycle] = E_srcB; e_valEsave[cycle] = e_valE; aluAsave[cycle] = aluA; aluBsave[cycle] = aluB; alufunsave[cycle] = alufun; set_CCsave[cycle] = set_CC; ZFsave[cycle] = ZF; SFsave[cycle] = SF; OFsave[cycle] = OF; e_Cndsave[cycle] = e_Cnd;
                M_icodesave[cycle] = M_icode; M_ifunsave[cycle] = M_ifun; M_Cndsave[cycle] = M_Cnd; M_valEsave[cycle] = M_valE; M_valAsave[cycle] = M_valA; M_dstEsave[cycle] = M_dstE; M_dstMsave[cycle] = M_dstM; m_valMsave[cycle] = m_valM; mem_addrsave[cycle] = mem_addr; mem_readsave[cycle] = mem_read; mem_writesave[cycle] = mem_write; dmem_errorsave[cycle] = dmem_error;
                W_icodesave[cycle] = W_icode; W_valEsave[cycle] = W_valE; W_valMsave[cycle] = W_valM; W_dstEsave[cycle] = W_dstE; W_dstMsave[cycle] = W_dstM;
                F_instrusave[cycle] = F_instru; D_instrusave[cycle] = D_instru; E_instrusave[cycle] = E_instru; M_instrusave[cycle] = M_instru; W_instrusave[cycle] = W_instru;
                D_statsave[cycle] = D_stat; E_statsave[cycle] = E_stat; M_statsave[cycle] = M_stat; W_statsave[cycle] = W_stat;
                for (int i = 0; i < 8; i++) regsave[cycle, i] = rreg[i];




                //Fetch PC
                if (imem_error == 0) lblF_PC.Text = txtrealPCtab.Text = F_PC.ToString(); else lblF_PC.Text = txtrealPCtab.Text = "NULL";
                if (F_predPC != INTMIN) lblpredPC.Text = txtpredPCtab.Text = F_predPC.ToString(); else lblpredPC.Text = txtpredPCtab.Text = "NULL";
                if (M_stat == SAOK && cycle >= 4)
                {
                    lblM_valAPC.Text = txtM_valAtab.Text = M_valA.ToString();
                    if (cycle >= 5) lblW_valMPC.Text = W_valM.ToString(); else lblW_valMPC.Text = "NULL";
                }
                else
                {
                    lblM_valAPC.Text = lblW_valMPC.Text = "NULL";
                }
                if (F_instru != INTMIN && cycle >= 1) lblF_instru.Text = txtF_instrutab.Text = rch.Lines[F_instru].Trim(); else lblF_instru.Text = txtF_instrutab.Text = "NULL";
                if (D_instru != INTMIN && D_stat != SBUB && cycle >= 2) lblD_instru.Text = txtD_instrutab.Text = rch.Lines[D_instru].Trim(); else lblD_instru.Text = txtD_instrutab.Text = "NULL";
                if (E_instru != INTMIN && E_stat != SBUB && cycle >= 3) lblE_instru.Text = txtE_instrutab.Text = rch.Lines[E_instru].Trim(); else lblE_instru.Text = txtE_instrutab.Text = "NULL";
                if (M_instru != INTMIN && M_stat != SBUB && cycle >= 4 && M_bubble != 1) lblM_instru.Text = txtM_instrutab.Text = rch.Lines[M_instru].Trim(); else lblM_instru.Text = txtM_instrutab.Text = "NULL";
                //M_bubble dmem_error M_stat==SBUB because of bubble injected into execute stage
                if (W_instru != INTMIN && W_stat != SBUB && cycle >= 5) lblW_instru.Text = txtW_instrutab.Text = rch.Lines[W_instru].Trim(); else lblW_instru.Text = txtW_instrutab.Text = "NULL";



                //Decode            
                if (D_stat != SBUB && cycle >= 2)
                {
                    lblD_icode.Text = txtD_icodetab.Text = D_icode.ToString(); lblD_ifun.Text = txtD_ifuntab.Text = D_ifun.ToString(); lblD_valP.Text = txtD_valPtab.Text = D_valP.ToString();
                    if (D_rA != INTMIN) lblD_rA.Text = txtD_rAtab.Text = reg[D_rA]; else lblD_rA.Text = txtD_rAtab.Text = "RNONE";
                    if (D_rB != INTMIN) lblD_rB.Text = txtD_rBtab.Text = reg[D_rB]; else lblD_rB.Text = txtD_rBtab.Text = "RNONE";
                    if (D_valC != INTMIN) lblD_valC.Text = txtD_valCtab.Text = D_valC.ToString(); else lblD_valC.Text = txtD_valCtab.Text = "NULL";
                    if (D_icode == IOPL || D_icode == IRMMOVL || D_icode == IRRMOVL || D_icode == IMRMOVL || D_icode == IPUSHL) //select+forward
                    {
                        lble_valEfwd.Text = e_valE.ToString(); lblm_valMfwd.Text = m_valM.ToString(); lblM_valEfwd.Text = M_valE.ToString(); lblW_valMfwd.Text = W_valM.ToString(); lblW_valEfwd.Text = W_valE.ToString();
                    }
                    else
                    {
                        lble_valEfwd.Text = lblm_valMfwd.Text = lblM_valEfwd.Text = lblW_valMfwd.Text = lblW_valEfwd.Text = "NULL";
                    }
                }
                else
                {
                    lblD_icode.Text = lblD_ifun.Text = lblD_rA.Text = lblD_rB.Text = lblD_valC.Text = lblD_valP.Text = lblD_rA.Text = lblD_rB.Text = "NULL";
                    txtD_icodetab.Text = txtD_ifuntab.Text = txtD_rAtab.Text = txtD_rBtab.Text = txtD_valCtab.Text = txtD_valPtab.Text = txtD_rAtab.Text = txtD_rBtab.Text = "NULL";
                    lble_valEfwd.Text = lblm_valMfwd.Text = lblM_valEfwd.Text = lblW_valMfwd.Text = lblW_valEfwd.Text = "NULL"; //no forward since decode not implement
                }
                if (D_rA != RNONE && D_rA != INTMIN && (D_stat == SAOK || D_stat == SSTA) && cycle >= 2) lblselARrA.Text = rreg[D_rA].ToString(); else lblselARrA.Text = "RNONE";
                if (D_rB != RNONE && D_rB != INTMIN && (D_stat == SAOK || D_stat == SSTA) && cycle >= 2) lblselBRrB.Text = rreg[D_rB].ToString(); else lblselBRrB.Text = "RNONE";
                if (D_valP != INTMIN && (D_stat == SAOK || D_stat == SSTA) && cycle >= 2) lblselAvalP.Text = D_valP.ToString(); else lblselAvalP.Text = "NULL";
                lblD_stat.Text = txtD_stattab.Text = state[D_stat];


                //Execute
                if (E_stat != SBUB && cycle >= 3)
                {
                    if (E_dstE != INTMIN) lblE_dstE.Text = txtE_dstEtab.Text = reg[E_dstE]; else lblE_dstE.Text = txtE_dstEtab.Text = "RNONE";
                    if (E_dstM != INTMIN) lblE_dstM.Text = txtE_dstMtab.Text = reg[E_dstM]; else lblE_dstM.Text = txtE_dstMtab.Text = "RNONE";
                    if (E_srcA != INTMIN) lblE_srcA.Text = txtE_srcAtab.Text = reg[E_srcA]; else lblE_srcA.Text = txtE_srcAtab.Text = "RNONE";
                    if (E_srcB != INTMIN) lblE_srcB.Text = txtE_srcBtab.Text = reg[E_srcB]; else lblE_srcB.Text = txtE_srcBtab.Text = "RNONE";
                    lblE_icode.Text = txtE_icodetab.Text = E_icode.ToString(); lblE_ifun.Text = txtE_ifuntab.Text = E_ifun.ToString();
                    lblE_valC.Text = txtE_valCtab.Text = E_valC.ToString();
                    if (E_icode == IRRMOVL || E_icode == IOPL || E_icode == IRMMOVL || E_icode == IMRMOVL || E_icode == IPUSHL || E_icode == IPOPL || E_icode == IRET || E_icode == ICALL || E_icode == IJXX)
                    {
                        lblE_valA.Text = txtE_valAtab.Text = E_valA.ToString();
                    }
                    else
                    {
                        lblE_valA.Text = txtE_valAtab.Text = "NULL";
                    }//no need to show E_valA
                    if (E_icode == IRMMOVL || E_icode == IMRMOVL || E_icode == IOPL || E_icode == ICALL || E_icode == IPUSHL || E_icode == IPOPL || E_icode == IRET)
                    {
                        lblE_valB.Text = txtE_valBtab.Text = E_valB.ToString();
                    }
                    else
                    {
                        lblE_valB.Text = txtE_valBtab.Text = "NULL";
                    }//no need to show E_valB

                    if (alufun != INTMIN)
                    {
                        lblALUA.Text = txtaluAtab.Text = aluA.ToString(); lblALUB.Text = txtaluBtab.Text = aluB.ToString(); lblALUfun.Text = lblalufuntab.Text = alufunction[alufun];
                        lblsetCC.Text = txtsetCCtab.Text = set_CC.ToString();
                        lblZFreal.Text = txtZFtab.Text = ZF.ToString(); lblSFreal.Text = txtSFtab.Text = SF.ToString(); lblOFreal.Text = txtOFtab.Text = OF.ToString();
                    }
                    else
                    {
                        lblALUA.Text = lblALUB.Text = lblALUfun.Text = "NULL";
                        txtaluAtab.Text = txtaluBtab.Text = lblalufuntab.Text = "NULL";
                        lblsetCC.Text = "0";
                        txtsetCCtab.Text = "0";
                    }
                }
                else
                {
                    lblE_dstE.Text = lblE_dstM.Text = lblE_srcA.Text = lblE_srcB.Text = "NULL";
                    txtE_dstEtab.Text = txtE_dstMtab.Text = txtE_srcAtab.Text = txtE_srcBtab.Text = "NULL";
                    lblE_icode.Text = lblE_ifun.Text = lblE_valC.Text = lblE_valA.Text = lblE_valB.Text = lblALUA.Text = lblALUB.Text = lblALUfun.Text = lblsetCC.Text = "NULL";
                    txtE_icodetab.Text = txtE_ifuntab.Text = txtE_valCtab.Text = txtE_valAtab.Text = txtE_valBtab.Text = txtaluAtab.Text = txtaluBtab.Text = lblalufuntab.Text = txtsetCCtab.Text = "NULL";
                }
                lblE_stat.Text = txtE_stattab.Text = state[E_stat];


                //Memory
                if (M_bubble != 1 && M_stat != SBUB && M_stat == SAOK && cycle >= 4)
                {
                    if (M_dstE != INTMIN) lblM_dstE.Text = txtM_dstEtab.Text = reg[M_dstE]; else lblM_dstE.Text = txtM_dstEtab.Text = "RNONE";
                    if (M_dstM != INTMIN) lblM_dstM.Text = txtM_dstMtab.Text = reg[M_dstM]; else lblM_dstM.Text = txtM_dstMtab.Text = "RNONE";
                    lblM_icode.Text = txtM_icodetab.Text = M_icode.ToString(); lblM_Cnd.Text = txtM_Cndtab.Text = M_Cnd.ToString(); lblM_valE.Text = txtM_valEtab.Text = M_valE.ToString(); lblM_addr.Text = txtmemaddrtab.Text = mem_addr.ToString();
                }
                else
                {
                    lblM_icode.Text = lblM_Cnd.Text = lblM_valE.Text = lblM_addr.Text = "NULL";
                    txtM_icodetab.Text = txtM_Cndtab.Text = txtM_valEtab.Text = txtmemaddrtab.Text = "NULL";
                    lblM_dstE.Text = lblM_dstM.Text = "RNONE";
                    txtM_dstEtab.Text = txtM_dstMtab.Text = "RNONE";
                }
                lblM_stat.Text = txtM_stattab.Text = state[M_stat];


                //Write
                if (W_stat != SBUB && cycle >= 5)
                {
                    if (W_dstE != INTMIN) lblW_dstE.Text = txtW_dstEtab.Text = reg[W_dstE]; else lblW_dstE.Text = txtW_dstEtab.Text = "RNONE";
                    if (W_dstM != INTMIN) lblW_dstM.Text = txtW_dstMtab.Text = reg[W_dstM]; else lblW_dstM.Text = txtW_dstMtab.Text = "RNONE";
                    lblW_icode.Text = txtW_icodetab.Text = W_icode.ToString(); lblW_valE.Text = txtW_valEtab.Text = W_valE.ToString(); lblW_valM.Text = txtW_valMtab.Text = W_valM.ToString();
                }
                else
                {
                    lblW_icode.Text = lblW_valE.Text = lblW_valM.Text = "NULL";
                    txtW_icodetab.Text = txtW_valEtab.Text = txtW_valMtab.Text = "NULL";
                    lblW_dstE.Text = lblW_dstM.Text = "RNONE";
                    txtW_dstEtab.Text = txtW_dstMtab.Text = "RNONE";
                }
                lblW_stat.Text = txtW_stattab.Text = state[W_stat];

                if (W_stat == SADR || W_stat == SHLT)
                {
                    finish = 1;
                    MessageBox.Show(state[W_stat], "运行完毕！", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                lbleaxreal.Text = txteaxtab.Text = rreg[0].ToString();
                lblecxreal.Text = txtecxtab.Text = rreg[1].ToString();
                lbledxreal.Text = txtedxtab.Text = rreg[2].ToString();
                lblebxreal.Text = txtebxtab.Text = rreg[3].ToString();
                lblespreal.Text = txtesptab.Text = rreg[4].ToString();
                lblebpreal.Text = txtebptab.Text = rreg[5].ToString();
                lblesireal.Text = txtesitab.Text = rreg[6].ToString();
                lbledireal.Text = txteditab.Text = rreg[7].ToString();

                txte_valEtab.Text = e_valE.ToString();
                if (alufun == INTMIN)
                {
                    txtaluAtab.Text = lblALUA.Text = "NULLL";
                    txtaluBtab.Text = lblALUB.Text = "NULL";
                    txte_valEtab.Text = lble_valEfwd.Text = "NULL";
                }
                if (M_valA == INTMIN) txtM_valAtab.Text = lblM_valAPC.Text = "NULL";
                if (M_valE == INTMIN) txtM_valEtab.Text = lblM_valEfwd.Text = lblM_valE.Text = "NULL";
                if (m_valM == INTMIN) txtm_valMtab.Text = lblm_valMlbl.Text = lblm_valMfwd.Text = "NULL";
                if (mem_addr == INTMIN) txtmemaddrtab.Text = lblM_addr.Text = "NULL";
                if (W_valE == INTMIN) txtW_valEtab.Text = lblW_valE.Text = lblW_valEfwd.Text = "NULL";
                if (W_valM == INTMIN) txtW_valMtab.Text = lblW_valM.Text = lblW_valMfwd.Text = lblW_valMPC.Text = "NULL";
                if (E_valA == INTMIN) txtE_valAtab.Text = lblE_valA.Text = "NULL";
                if (E_valB == INTMIN) txtE_valBtab.Text = lblE_valB.Text = "NULL";
                if (E_valC == INTMIN) txtE_valCtab.Text = lblE_valC.Text = "NULL";
                if (D_valC == INTMIN) txtD_valCtab.Text = lblD_valC.Text = "NULL";
                if (D_valP == INTMIN) txtD_valPtab.Text = lblD_valP.Text = "NULL";
                if (D_icode == INTMIN) txtD_icodetab.Text = lblD_icode.Text = "NULL";
                if (D_ifun == INTMIN) txtD_ifuntab.Text = lblD_ifun.Text = "NULL";
                if (E_icode == INTMIN) txtE_icodetab.Text = lblE_icode.Text = "NULL";
                if (E_ifun == INTMIN) txtE_ifuntab.Text = lblE_ifun.Text = "NULL";
                if (M_icode == INTMIN) txtM_icodetab.Text = lblM_icode.Text = "NULL";
                if (W_icode == INTMIN) txtW_icodetab.Text = lblW_icode.Text = "NULL";
                if (e_Cnd == INTMIN) txte_Cndtab.Text = "NULL";
                if (M_Cnd == INTMIN) txtM_Cndtab.Text = lblM_Cnd.Text = "NULL";
                if (finish == 1)
                {
                    btngo.Enabled = btnforward.Enabled = btnbackward.Enabled = btnrunall.Enabled = false; todo = false;
                    btnpause.Enabled = btnresume.Enabled = btnstop.Enabled = false;
                }

                if (W_instru != INTMIN)
                {
                    int p = rch.GetFirstCharIndexFromLine(W_instru);
                    int rolltorow = Math.Max(0, W_instru - 15);
                    rch.Select(rch.GetFirstCharIndexFromLine(rolltorow), 0);
                    rch.ScrollToCaret();
                    rch.Select(p, 0);
                    rch_Click(sender, e);
                }
                //-------------------------------------------------------------------------------------
                cycins[cycle, 0] = F_instru;
                cycins[cycle, 1] = D_instru;
                cycins[cycle, 2] = E_instru;
                cycins[cycle, 3] = M_instru;
                cycins[cycle, 4] = W_instru;
                TextBox[,] txtstage = {{txtfive1,txtfive2,txtfive3,txtfive4,txtfive5,txtfive6},
                                  {txtfour1,txtfour2,txtfour3,txtfour4,txtfour5,txtfour6},
                                  {txtthree1,txtthree2,txtthree3,txtthree4,txtthree5,txtthree6},
                                  {txttwo1,txttwo2,txttwo3,txttwo4,txttwo5,txttwo6},
                                  {txtone1,txtone2,txtone3,txtone4,txtone5,txtone6},
                                  {txtzero1,txtzero2,txtzero3,txtzero4,txtzero5,txtzero6}};
                Label[] lblinstruction = { lblins0, lblins1, lblins2, lblins3, lblins4, lblins5 };
                if (W_instru != INTMIN && cycle >= 10)
                {
                    if (firstins == -1) firstins = cycle;
                    lblcycle0.Text = cycle.ToString();
                    lblcycle1.Text = (cycle - 1).ToString();
                    lblcycle2.Text = (cycle - 2).ToString();
                    lblcycle3.Text = (cycle - 3).ToString();
                    lblcycle4.Text = (cycle - 4).ToString();
                    lblcycle5.Text = (cycle - 5).ToString();
                    lblcycle6.Text = (cycle - 6).ToString();
                    lblcycle7.Text = (cycle - 7).ToString();
                    lblcycle8.Text = (cycle - 8).ToString();
                    lblcycle9.Text = (cycle - 9).ToString();
                    lblcycle10.Text = (cycle - 10).ToString();
                    int i = 0;
                    while (i < 6)
                    {
                        txtstage[i, 0].Text = "F"; txtstage[i, 1].Text = "F"; txtstage[i, 2].Text = "D"; txtstage[i, 3].Text = "E"; txtstage[i, 4].Text = "M"; txtstage[i, 5].Text = "W";
                        if (cycins[cycle - i, 4] != INTMIN)  //W
                        {
                            if (cycins[cycle - i, 4] == cycins[cycle - i - 5, 0]) //F W 
                            {
                                if (cycins[cycle - i - 5, 0] == cycins[cycle - i - 4, 0]) //FFD
                                {
                                    txtstage[i, 0].Text = "F"; txtstage[i, 1].Text = "F";
                                    for (int j = 0; j < 6; j++) { txtstage[i, j].Visible = true; txtstage[i, j].BackColor = Color.FromArgb(128, 255, 255); }
                                    txtstage[i, 1].BackColor = Color.FromArgb(128, 128, 255);
                                    lblinstruction[i].Text = process(instruction[cycins[cycle - i, 4]]);
                                    lblinstruction[i].Visible = true;
                                    if (i + 1 < 6)
                                    {
                                        txtstage[i + 1, 0].Text = "F"; txtstage[i + 1, 1].Text = "D";
                                        for (int j = 0; j < 6; j++) { txtstage[i + 1, j].Visible = true; txtstage[i + 1, j].BackColor = Color.FromArgb(128, 255, 255); }
                                        txtstage[i + 1, 2].BackColor = Color.FromArgb(128, 128, 255);
                                        lblinstruction[i + 1].Text = process(instruction[cycins[cycle - i - 1, 4]]);
                                        lblinstruction[i + 1].Visible = true;
                                    }
                                    i += 2;
                                    if (i < 6)
                                    {
                                        for (int j = 0; j < 6; j++) { txtstage[i, j].Visible = false; txtstage[i, j].BackColor = Color.FromArgb(128, 255, 255); }
                                        lblinstruction[i].Visible = false;
                                        i++;
                                    }
                                    continue;
                                }
                                else //FDD
                                {
                                    txtstage[i, 0].Text = "F"; txtstage[i, 1].Text = "D";
                                    for (int j = 0; j < 6; j++) { txtstage[i, j].Visible = true; txtstage[i, j].BackColor = Color.FromArgb(128, 255, 255); }
                                    txtstage[i, 2].BackColor = Color.FromArgb(128, 128, 255);
                                    lblinstruction[i].Visible = true;
                                    lblinstruction[i].Text = process(instruction[cycins[cycle - i, 4]]);
                                    i++;
                                    if (i < 6)
                                    {
                                        for (int j = 0; j < 6; j++) { txtstage[i, j].Visible = false; txtstage[i, j].BackColor = Color.FromArgb(128, 255, 255); }
                                        lblinstruction[i].Visible = false;
                                        i++;
                                    }
                                    continue;
                                }
                            }
                        }
                        for (int j = 4; j >= 0; j--)
                        {
                            if (cycins[cycle - i - (4 - j), j] != INTMIN) { txtstage[i, j + 1].Visible = true; txtstage[i, j].BackColor = Color.FromArgb(128, 255, 255); }
                            else txtstage[i, j + 1].Visible = false;
                        }
                        txtstage[i, 0].Visible = false;
                        if (cycins[cycle - i - 4, 0] == INTMIN) lblinstruction[i].Visible = false;
                        else { lblinstruction[i].Visible = true; lblinstruction[i].Text = process(instruction[cycins[cycle - i - 4, 0]]); }
                        i++;
                    }

                    for (int u = 0; u < 6; u++)
                    {
                        for (int v = 0; v < 6; v++)
                        {
                            if (txtstage[u, 5 - v].Visible == true)
                            {
                                stagecyc[cycle - firstins + 5 - u, cycle - u - v - (firstins - 10)] = txtstage[u, 5 - v].Text[0];
                                bel[cycle - firstins + 5 - u] = lblinstruction[u].Text;
                            }
                            else
                            {
                                stagecyc[cycle - firstins + 5 - u, cycle - u - v - (firstins - 10)] = ' ';
                            }
                        }
                    }
                }
                rch_VScroll(sender, e);
            }
            catch(System.Exception err)
            {
                return;
            }
        }
        private void btnrunall_Click(object sender, EventArgs e)
        {            
            try
            {
                btnset_Click(sender, e);
                pause = false;
                if (tmr.Enabled==true)
                {
                    todo = true;
                }
                else
                {
                    while (finish == 0)
                    {                        
                        btnstep_Click(sender, e);               
                        if (checkpause() == 1) return; //pause
                    }
                }                
            }
            catch(System.Exception err)
            {
                return;
            }                           
        }

        private void btnreadmem_Click(object sender, EventArgs e)
        {
            try
            {
                int mem_addr;
                if (txtsetmemaddr.Text.Substring(0,2).Equals("0x")) mem_addr=Convert.ToInt32(txtsetmemaddr.Text,16); else mem_addr = Convert.ToInt32(txtsetmemaddr.Text);
                if (mem_addr <= maxhash)
                {
                    txtsetvalue.Text = rmem[mem_addr].ToString();
                }
                else
                {
                    for (int i = 0; i < totalmem; i++)
                    {
                        if (mapmem[i, 0] == mem_addr)
                        {
                            txtsetvalue.Text = mapmem[i, 1].ToString();
                            break;
                        }
                    }
                }
            }
            catch(System.Exception err)
            {
                return;
            }
           
        }

        private void btnwritemem_Click(object sender, EventArgs e)
        {
            try
            {
                int mem_addr;
                if (txtsetmemaddr.Text.Substring(0, 2).Equals("0x")) mem_addr = Convert.ToInt32(txtsetmemaddr.Text, 16); else mem_addr = Convert.ToInt32(txtsetmemaddr.Text);                
                int value = Convert.ToInt32(txtsetvalue.Text);
                if (mem_addr <= maxhash)
                {
                    rmem[mem_addr] = value; //real memory 
                }
                else
                {
                    int flag = 0;
                    for (int i = 0; i < totalmem; i++)
                    {
                        if (mapmem[i, 0] == mem_addr)
                        {
                            mapmem[i, 1] = value;
                            flag = 1;
                        }
                    }
                    if (flag == 0)
                    {
                        mapmem[++totalmem, 0] = mem_addr;
                        mapmem[totalmem, 1] = value;
                    }
                }
            }
            catch(System.Exception err)
            {
                return;
            }
            
        }

        private void btnset_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < txtset.Lines.Count(); i++)
                {
                    int mem_addr = 0, value = 0;
                    for (int j = 0; j < txtset.Lines[i].Length; j++)
                    {
                        if (txtset.Lines[i][j].Equals(' '))
                        {
                            string s1 = "", s2 = "";
                            s1 = txtset.Lines[i].Substring(0, j);
                            s2 = txtset.Lines[i].Substring(j + 1);
                            if (s1.Substring(0, 2).Equals("0x"))
                            {
                                mem_addr = Convert.ToInt32(s1, 16);
                            }
                            else
                            {
                                mem_addr = Convert.ToInt32(s1);
                            }
                            if (s2.Substring(0, 2).Equals("0x"))
                            {
                                value = Convert.ToInt32(s2, 16);
                            }
                            else
                            {
                                value = Convert.ToInt32(s2);
                            }
                            //get mem_addr and value write to memory
                            if (mem_addr <= maxhash)
                            {
                                rmem[mem_addr] = value; //real memory 
                            }
                            else
                            {
                                int flag = 0;
                                for (int k = 0; k < totalmem; k++)
                                {
                                    if (mapmem[k, 0] == mem_addr)
                                    {
                                        mapmem[k, 1] = value;
                                        flag = 1;
                                    }
                                }
                                if (flag == 0)
                                {
                                    mapmem[++totalmem, 0] = mem_addr;
                                    mapmem[totalmem, 1] = value;
                                }
                            }
                        }
                    }
                }
            }
            catch(System.Exception err)
            {
                return;
            }
        }

        private void btnreset_Click(object sender, EventArgs e)
        {
            try
            {
                pause = false;
                reset = 1;
                if (notclean == 1) { clean = false; notclean = 0; }
                initstate();
                reset = 0;
                btngo.Enabled = btnforward.Enabled = btnbackward.Enabled = btnrunall.Enabled = true;
                btnpause.Enabled = btnresume.Enabled = btnstop.Enabled = true;
                todo = false;
                nowinstru = 0;
                datagridstage.Columns.Clear();
                datagridstage.Rows.Clear();
                lblins0.Text = "ins0";
                lblins1.Text = "ins1";
                lblins2.Text = "ins2";
                lblins3.Text = "ins3";
                lblins4.Text = "ins4";
                lblins5.Text = "ins5";
                TextBox[,] txtstage = {{txtfive1,txtfive2,txtfive3,txtfive4,txtfive5,txtfive6},
                                  {txtfour1,txtfour2,txtfour3,txtfour4,txtfour5,txtfour6},
                                  {txtthree1,txtthree2,txtthree3,txtthree4,txtthree5,txtthree6},
                                  {txttwo1,txttwo2,txttwo3,txttwo4,txttwo5,txttwo6},
                                  {txtone1,txtone2,txtone3,txtone4,txtone5,txtone6},
                                  {txtzero1,txtzero2,txtzero3,txtzero4,txtzero5,txtzero6}};
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        txtstage[i, j].BackColor = Color.FromArgb(128, 255, 255);
                    }
                    txtstage[i, 0].Text = "F"; txtstage[i, 1].Text = "F"; txtstage[i, 2].Text = "D"; txtstage[i, 3].Text = "E"; txtstage[i, 4].Text = "M"; txtstage[i, 5].Text = "W";
                }            
            }
            catch(System.Exception err)
            {
                return;
            }                        
        }

        private int checkpause()
        {
            try
            {
                //if (slt[F_instru] == 1) return 1;
                Point p = rch.Location;
                int first = rch.GetCharIndexFromPosition(p);
                int fstline = rch.GetLineFromCharIndex(first);
                int cnt = lst.SelectedItems.Count;
                for (int i = 0; i < cnt; i++)
                {
                    int id = lst.SelectedIndices[i];                    
                    if (F_instru == fstline + id - 11) return 1;
                }
                return 0;
            }
            catch (System.Exception err)
            {
                return 1;
            }
            
        }

        private void btngo_Click(object sender, EventArgs e)
        {
            try
            {
                notclean = 1;                
                btnreset_Click(sender, e);
                btnset_Click(sender, e);
                for (int i = 1; i <= Convert.ToInt32(txtruncycle.Text); i++)
                {
                    btnstep_Click(sender, e);
                    if (checkpause() == 1) return; //pause
                }
            }
            catch(System.Exception err)
            {
                return;
            }
            
        }

        private void btnforward_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 1; i <= Convert.ToInt32(txtforward.Text);i++ )
                {
                    btnstep_Click(sender, e);
                    if (checkpause() == 1) return; //pause
                }   
            }
            catch(System.Exception err)
            {
                return;
            }                         
        }

        private void btnbackward_Click(object sender, EventArgs e)
        {
            try
            {
                notclean = 1;
                int times = cycle - Convert.ToInt32(txtbackward.Text);
                btnreset_Click(sender, e);
                btnset_Click(sender, e);
                for (int i = 1; i <= times; i++)
                {
                    btnstep_Click(sender, e);
                    if (checkpause() == 1) return; //pause
                }   
            }
            catch(System.Exception err)
            {
                return;
            }
        }

        private void lststage_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                e.DrawBackground();
                e.DrawFocusRectangle();
                e.Graphics.DrawString(lststage.Items[e.Index].ToString(), e.Font, new SolidBrush(Color.Black), e.Bounds);
            }
            catch (System.Exception err)
            {
                return;
            }            
        }

        private void lststage_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            try
            {
                e.ItemHeight = 17;        
            }
            catch(System.Exception err)
            {
                return;
            }
            
        }



        private void rch_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                rch_VScroll(sender, e);
            }
            catch(System.Exception err)
            {
                return;
            }            
        }

        private void btnshowallstage_Click(object sender,EventArgs e)
        {
            try
            {
                datagridstage.Columns.Clear();
                datagridstage.Rows.Clear();
                for (int i = 0; i < cycle - firstins + 6; i++)
                {
                    datagridstage.Columns.Add("11", (i + firstins - 5).ToString());
                }
                for (int i = 0; i < cycle - firstins + 6; i++)
                {
                    datagridstage.Rows.Add();
                    datagridstage.Rows[i].HeaderCell.Value = (i+1).ToString()+" "+bel[i];
                    for (int j = i; j < Math.Min(cycle - firstins + 6,i+6); j++)
                    {
                        datagridstage.Rows[i].Cells[j].Value = stagecyc[i, j];
                        datagridstage.Rows[i].Cells[j].Style.BackColor = Color.Lime;
                        if (j - 1 >= i && stagecyc[i, j] == stagecyc[i, j - 1] && (stagecyc[i, j].Equals('F') || stagecyc[i, j].Equals('D'))) datagridstage.Rows[i].Cells[j].Style.BackColor = Color.Orange;
                    }
                }
            }
            catch ( System.Exception err)
            {
                return;
            }

        }

        private void btnsavetofile_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "所有文件(*.*)|*.*";
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string filename = saveFileDialog1.FileName;

                    using (StreamWriter sw = File.CreateText(filename))
                    {
                        for (int i = 0; i < cycle; i++)
                        {
                            //Fetch
                            sw.WriteLine("Cycle_" + i.ToString());
                            sw.WriteLine("--------------------");
                            sw.WriteLine("FETCH:");
                            sw.WriteLine("	F_predPC 	= " + handlehex(F_predPCsave[i + 1]));
                            //Decode
                            sw.WriteLine("");
                            sw.WriteLine("DECODE:");
                            sw.WriteLine("    D_icode     = " + handlehex2(D_icodesave[i + 1]));
                            sw.WriteLine("    D_ifun      = " + handlehex2(D_ifunsave[i + 1]));
                            sw.WriteLine("    D_rA        = " + handlehex2(D_rAsave[i + 1]));
                            sw.WriteLine("    D_rB        = " + handlehex2(D_rBsave[i + 1]));
                            sw.WriteLine("    D_valC      = " + handlehex(D_valCsave[i + 1]));
                            sw.WriteLine("    D_valP      = " + handlehex(D_valPsave[i + 1]));
                            //Execute
                            sw.WriteLine("");
                            sw.WriteLine("EXECUTE:");
                            sw.WriteLine("    E_icode     = " + handlehex2(E_icodesave[i + 1]));
                            sw.WriteLine("    E_ifun      = " + handlehex2(E_ifunsave[i + 1]));
                            sw.WriteLine("    E_valC      = " + handlehex(E_valCsave[i + 1]));
                            sw.WriteLine("    E_valA      = " + handlehex(E_valAsave[i + 1]));
                            sw.WriteLine("    E_valB      = " + handlehex(E_valBsave[i + 1]));
                            sw.WriteLine("    E_dstE      = " + handlehex2(E_dstEsave[i + 1]));
                            sw.WriteLine("    E_dstM      = " + handlehex2(E_dstMsave[i + 1]));
                            sw.WriteLine("    E_srcA      = " + handlehex2(E_srcAsave[i + 1]));
                            sw.WriteLine("    E_srcB      = " + handlehex2(E_srcBsave[i + 1]));
                            //Memory
                            sw.WriteLine("");
                            sw.WriteLine("MEMORY:");
                            sw.WriteLine("    M_icode     = " + handlehex2(M_icodesave[i + 1]));
                            sw.WriteLine("    M_Bch       = " + handlehex3(M_Cndsave[i + 1]));
                            sw.WriteLine("    M_valE      = " + handlehex(M_valEsave[i + 1]));
                            sw.WriteLine("    M_valA      = " + handlehex(M_valAsave[i + 1]));
                            sw.WriteLine("    M_dstE      = " + handlehex2(M_dstEsave[i + 1]));
                            sw.WriteLine("    M_dstM      = " + handlehex2(M_dstMsave[i + 1]));
                            //Write
                            sw.WriteLine("");
                            sw.WriteLine("WRITE BACK:");
                            sw.WriteLine("    W_icode     = " + handlehex2(W_icodesave[i + 1]));
                            sw.WriteLine("    W_valE      = " + handlehex(W_valEsave[i + 1]));
                            sw.WriteLine("    W_valM      = " + handlehex(W_valMsave[i + 1]));
                            sw.WriteLine("    W_dstE      = " + handlehex2(W_dstEsave[i + 1]));
                            sw.WriteLine("    eW_dstM      = " + handlehex2(W_dstMsave[i + 1]));
                            sw.WriteLine("");
                        }
                        sw.Close();
                    }
                }
            }
            catch(System.Exception err)
            {
                return;
            }
           
        }
        
        private void doit() //show on listview
        {
            try
            {
                lstview.Items.Clear();
                int currentcycle = int.Parse(txtrunto.Text);
                ListViewItem lvi = new ListViewItem();
                //Fetch
                lvi.Text = "F_predPC";                
                lvi.SubItems.Add(handlehex(F_predPCsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(F_predPCsave[currentcycle]));                
                lvi.Group = F;
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = F;
                lvi.Text = "F_PC";
                lvi.SubItems.Add(handlehex(F_PCsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(F_PCsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = F;
                lvi.Text = "F_icode";
                lvi.SubItems.Add(handlehex2(F_icodesave[currentcycle]));
                lvi.SubItems.Add(handledecimal(F_icodesave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = F;
                lvi.Text = "F_ifun";
                lvi.SubItems.Add(handlehex2(F_ifunsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(F_ifunsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = F;
                lvi.Text = "F_valC";
                lvi.SubItems.Add(handlehex(F_valCsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(F_valCsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = F;
                lvi.Text = "F_valP";
                lvi.SubItems.Add(handlehex(F_valPsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(F_valPsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = F;
                lvi.Text = "imem_error";
                lvi.SubItems.Add(handlehex3(imem_errorsave[currentcycle]));
                lvi.SubItems.Add(handlehex3(imem_errorsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = F;
                lvi.Text = "F_rA";
                lvi.SubItems.Add(handlehex2(F_rAsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(F_rAsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = F;
                lvi.Text = "F_rB";
                lvi.SubItems.Add(handlehex2(F_rBsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(F_rBsave[currentcycle]));
                lstview.Items.Add(lvi);
                //Decode
                lvi = new ListViewItem();
                lvi.Group = D;
                lvi.Text = "D_icode";
                lvi.SubItems.Add(handlehex2(D_icodesave[currentcycle]));
                lvi.SubItems.Add(handledecimal(D_icodesave[currentcycle]));                
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = D;
                lvi.Text = "D_ifun";
                lvi.SubItems.Add(handlehex2(D_ifunsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(D_ifunsave[currentcycle]));                
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = D;
                lvi.Text = "D_rA";
                lvi.SubItems.Add(handlehex2(D_rAsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(D_rAsave[currentcycle]));                
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = D;
                lvi.Text = "D_rB";
                lvi.SubItems.Add(handlehex2(D_rBsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(D_rBsave[currentcycle]));                
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = D;
                lvi.Text = "D_valC";
                lvi.SubItems.Add(handlehex(D_valCsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(D_valCsave[currentcycle]));                
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = D;
                lvi.Text = "D_valP";
                lvi.SubItems.Add(handlehex(D_valPsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(D_valPsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = D;
                lvi.Text = "d_valA";
                lvi.SubItems.Add(handlehex(d_valAsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(d_valAsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = D;
                lvi.Text = "d_valB";
                lvi.SubItems.Add(handlehex(d_valBsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(d_valBsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = D;
                lvi.Text = "d_srcA";
                lvi.SubItems.Add(handlehex2(d_srcAsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(d_srcAsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = D;
                lvi.Text = "d_srcB";
                lvi.SubItems.Add(handlehex2(d_srcBsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(d_srcBsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = D;
                lvi.Text = "d_dstE";
                lvi.SubItems.Add(handlehex2(d_dstEsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(d_dstEsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = D;
                lvi.Text = "d_dstM";
                lvi.SubItems.Add(handlehex2(d_dstMsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(d_dstMsave[currentcycle]));
                lstview.Items.Add(lvi);
                //Execute
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "E_icode";
                lvi.SubItems.Add(handlehex2(E_icodesave[currentcycle]));
                lvi.SubItems.Add(handledecimal(E_icodesave[currentcycle]));
                lvi.Group = E;
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "E_ifun";
                lvi.SubItems.Add(handlehex2(E_ifunsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(E_ifunsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "E_valC";
                lvi.SubItems.Add(handlehex(E_valCsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(E_valCsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "E_valA";
                lvi.SubItems.Add(handlehex(E_valAsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(E_valAsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "E_dstE";
                lvi.SubItems.Add(handlehex2(E_dstEsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(E_dstEsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "e_dstE";
                lvi.SubItems.Add(handlehex2(e_dstEsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(e_dstEsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "E_dstM";
                lvi.SubItems.Add(handlehex2(E_dstMsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(E_dstMsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "E_srcA";
                lvi.SubItems.Add(handlehex2(E_srcAsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(E_srcAsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "E_srcB";
                lvi.SubItems.Add(handlehex2(E_srcBsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(E_srcBsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "e_Cnd";
                lvi.SubItems.Add(handlehex3(e_Cndsave[currentcycle]));
                lvi.SubItems.Add(handlehex3(e_Cndsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "aluA";
                lvi.SubItems.Add(handlehex(aluAsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(aluAsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "aluB";
                lvi.SubItems.Add(handlehex(aluBsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(aluBsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "alufun";
                lvi.SubItems.Add(handlehex(alufunsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(alufunsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "set_CC";
                lvi.SubItems.Add(handlehex2(set_CCsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(set_CCsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "ZF";
                lvi.SubItems.Add(handlehex2(ZFsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(ZFsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "SF";
                lvi.SubItems.Add(handlehex2(SFsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(SFsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = E;
                lvi.Text = "OF";
                lvi.SubItems.Add(handlehex2(OFsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(OFsave[currentcycle]));
                lstview.Items.Add(lvi);
                //Memory
                lvi = new ListViewItem();
                lvi.Group = M;
                lvi.Text = "M_icode";
                lvi.SubItems.Add(handlehex2(M_icodesave[currentcycle]));
                lvi.SubItems.Add(handledecimal(M_icodesave[currentcycle]));
                lvi.Group = M;
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = M;
                lvi.Text = "M_Cnd";
                lvi.SubItems.Add(handlehex3(M_Cndsave[currentcycle]));
                lvi.SubItems.Add(handlehex3(M_Cndsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = M;
                lvi.Text = "M_valE";
                lvi.SubItems.Add(handlehex(M_valEsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(M_valEsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = M;
                lvi.Text = "M_valA";
                lvi.SubItems.Add(handlehex(M_valAsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(M_valAsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = M;
                lvi.Text = "M_dstE";
                lvi.SubItems.Add(handlehex2(M_dstEsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(M_dstEsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = M;
                lvi.Text = "M_dstM";
                lvi.SubItems.Add(handlehex2(M_dstMsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(M_dstMsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = M;
                lvi.Text = "m_valM";
                lvi.SubItems.Add(handlehex(m_valMsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(m_valMsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = M;
                lvi.Text = "mem_addr";
                lvi.SubItems.Add(handlehex(mem_addrsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(mem_addrsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = M;
                lvi.Text = "mem_read";
                lvi.SubItems.Add(handlehex3(mem_readsave[currentcycle]));
                lvi.SubItems.Add(handlehex3(mem_readsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = M;
                lvi.Text = "mem_write";
                lvi.SubItems.Add(handlehex3(mem_writesave[currentcycle]));
                lvi.SubItems.Add(handlehex3(mem_writesave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = M;
                lvi.Text = "dmem_error";
                lvi.SubItems.Add(handlehex3(dmem_errorsave[currentcycle]));
                lvi.SubItems.Add(handlehex3(dmem_errorsave[currentcycle]));
                lstview.Items.Add(lvi);
                //Write Back
                lvi = new ListViewItem();
                lvi.Group = W;
                lvi.Text = "W_icode";
                lvi.SubItems.Add(handlehex2(W_icodesave[currentcycle]));
                lvi.SubItems.Add(handledecimal(W_icodesave[currentcycle]));
                lvi.Group = W;
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = W;
                lvi.Text = "W_valE";
                lvi.SubItems.Add(handlehex(W_valEsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(W_valEsave[currentcycle]));                
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = W;
                lvi.Text = "W_valM";
                lvi.SubItems.Add(handlehex(W_valMsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(W_valMsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = W;
                lvi.Text = "W_dstE";
                lvi.SubItems.Add(handlehex2(W_dstEsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(W_dstEsave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = W;
                lvi.Text = "W_dstM";
                lvi.SubItems.Add(handlehex2(W_dstMsave[currentcycle]));
                lvi.SubItems.Add(handledecimal(W_dstMsave[currentcycle]));
                lstview.Items.Add(lvi);
                //Status
                lvi = new ListViewItem();
                lvi.Group = S;
                lvi.Text = "F_stat";
                lvi.SubItems.Add(state[F_statsave[currentcycle]]);
                lvi.SubItems.Add(state[F_statsave[currentcycle]]);
                lvi.Group = S;
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = S;
                lvi.Text = "D_stat";
                lvi.SubItems.Add(state[D_statsave[currentcycle]]);
                lvi.SubItems.Add(state[D_statsave[currentcycle]]);                
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = S;
                lvi.Text = "E_stat";
                lvi.SubItems.Add(state[E_statsave[currentcycle]]);
                lvi.SubItems.Add(state[E_statsave[currentcycle]]);                
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = S;
                lvi.Text = "M_stat";
                lvi.SubItems.Add(state[M_statsave[currentcycle]]);
                lvi.SubItems.Add(state[M_statsave[currentcycle]]);                
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = S;
                lvi.Text = "W_stat";
                lvi.SubItems.Add(state[W_statsave[currentcycle]]);
                lvi.SubItems.Add(state[W_statsave[currentcycle]]);                
                lstview.Items.Add(lvi);       
                //Register
                lvi = new ListViewItem();
                lvi.Group = R;
                lvi.Text = "%eax";
                lvi.SubItems.Add(handlehex(regsave[currentcycle,0]));
                lvi.SubItems.Add(handledecimal(regsave[currentcycle, 0]));
                lvi.Group = R;
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = R;
                lvi.Text = "%ecx";
                lvi.SubItems.Add(handlehex(regsave[currentcycle, 1]));
                lvi.SubItems.Add(handledecimal(regsave[currentcycle, 1]));                
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = R;
                lvi.Text = "%edx";
                lvi.SubItems.Add(handlehex(regsave[currentcycle, 2]));
                lvi.SubItems.Add(handledecimal(regsave[currentcycle, 2]));                
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = R;
                lvi.Text = "%ebx";
                lvi.SubItems.Add(handlehex(regsave[currentcycle, 3]));
                lvi.SubItems.Add(handledecimal(regsave[currentcycle, 3]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = R;
                lvi.Text = "%esp";
                lvi.SubItems.Add(handlehex(regsave[currentcycle, 4]));
                lvi.SubItems.Add(handledecimal(regsave[currentcycle, 4]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = R;
                lvi.Text = "%ebp";
                lvi.SubItems.Add(handlehex(regsave[currentcycle, 5]));
                lvi.SubItems.Add(handledecimal(regsave[currentcycle, 5]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = R;
                lvi.Text = "%esi";
                lvi.SubItems.Add(handlehex(regsave[currentcycle, 6]));
                lvi.SubItems.Add(handledecimal(regsave[currentcycle, 6]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = R;
                lvi.Text = "%edi";
                lvi.SubItems.Add(handlehex(regsave[currentcycle, 7]));
                lvi.SubItems.Add(handledecimal(regsave[currentcycle, 7]));
                lstview.Items.Add(lvi);       
                //Instruction
                lvi = new ListViewItem();
                lvi.Group = INS;
                lvi.Text = "F_instruction";
                lvi.SubItems.Add(handleinstruction(F_instrusave[currentcycle]));
                lvi.SubItems.Add(handleinstruction(F_instrusave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = INS;
                lvi.Text = "D_instruction";
                lvi.SubItems.Add(handleinstruction(D_instrusave[currentcycle]));
                lvi.SubItems.Add(handleinstruction(D_instrusave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = INS;
                lvi.Text = "E_instruction";
                lvi.SubItems.Add(handleinstruction(E_instrusave[currentcycle]));
                lvi.SubItems.Add(handleinstruction(E_instrusave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = INS;
                lvi.Text = "M_instruction";
                lvi.SubItems.Add(handleinstruction(M_instrusave[currentcycle]));
                lvi.SubItems.Add(handleinstruction(M_instrusave[currentcycle]));
                lstview.Items.Add(lvi);
                lvi = new ListViewItem();
                lvi.Group = INS;
                lvi.Text = "W_instruction";
                lvi.SubItems.Add(handleinstruction(W_instrusave[currentcycle]));
                lvi.SubItems.Add(handleinstruction(W_instrusave[currentcycle]));
                lstview.Items.Add(lvi);    
            }
            catch(System.Exception err)
            {
                return;
            }
        }

        

        private void txtrunto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                doit();
            }
        }

        private void btnnext_Click(object sender, EventArgs e)
        {
            try
            {
                txtrunto.Text = (int.Parse(txtrunto.Text) + 1).ToString();
                doit();
            }
            catch(System.Exception err)
            {
                return;
            }
            
        }

        private void btnprev_Click(object sender, EventArgs e)
        {
            try
            {
                txtrunto.Text = (int.Parse(txtrunto.Text) - 1).ToString();
                doit();
            }
            catch (System.Exception err)
            {
                return;
            }
        }

        private void btnfirst_Click(object sender, EventArgs e)
        {
            try
            {
                txtrunto.Text = "1";
                doit();
            }
            catch(System.Exception err)
            {
                return;
            }            
        }

        private void btnlast_Click(object sender, EventArgs e)
        {
            try
            {
                txtrunto.Text = cycle.ToString();
                doit();
            }
            catch(System.Exception err)
            {
                return;
            }
        }



        int memmin, memmax;
        int flag = 0;
        private void btnshowallmemory_Click(object sender, EventArgs e)
        {
            try
            {
                if (flag == 1)
                {
                    lblarrow.Visible = true; flag = 0;
                }
                else
                {
                    if (txtshowmemmin.Text.Substring(0, 2).Equals("0x")) memmin = Convert.ToInt32(txtshowmemmin.Text, 16); else memmin = Convert.ToInt32(txtshowmemmin.Text);
                    if (txtshowmemmax.Text.Substring(0, 2).Equals("0x")) memmax = Convert.ToInt32(txtshowmemmax.Text, 16); else memmax = Convert.ToInt32(txtshowmemmax.Text);
                    lblarrow.Visible = false;
                }
                lstmemory.Items.Clear();
                ListViewItem lvi = new ListViewItem();
                while (memmin <= memmax && (memmax - memmin) % 4 != 0) { memmin += 1; }
                for (int i = memmin; i <=memmax; i+=4)
                {
                    lvi = new ListViewItem();
                    lvi.Text = handlehex(i);
                    lvi.SubItems.Add(handledecimal(i));
                    lvi.SubItems.Add(handlehex(rmem[i]));
                    lvi.SubItems.Add(handledecimal(rmem[i]));
                    lstmemory.Items.Add(lvi);
                }            
            }
            catch(System.Exception err)
            {
                return;
            }            
        }

        private void btnshowstack_Click(object sender, EventArgs e)
        {
            try
            {
                memmin = Math.Max(0, rreg[4] - 162); memmax = rreg[4];
                flag = 1;
                btnshowallmemory_Click(sender, e);
            }
            catch(System.Exception err)
            {
                return;
            }
            
        }

        private void lst_Click(object sender, EventArgs e)
        {
            try
            {
                Point p = rch.Location;
                int first = rch.GetCharIndexFromPosition(p);
                int fstline = rch.GetLineFromCharIndex(first);
                for (int i = 0; i < LINENUM; i++)
                {
                    int instruid = fstline + i - 11;
                    if (lst.SelectedIndices.Contains(i))
                    {
                        slt[instruid] = 1;
                    }
                    else
                    {
                        slt[instruid] = 0;
                    }
                }   
            }
            catch(System.Exception err)
            {
                return;
            }             
        }

        const double minHZ = 0.01;
        const double maxHZ = 1000;
        private void hscr_Scroll(object sender, ScrollEventArgs e)
        {            
            
            
        }

        private void radiotime_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radiotime.Checked == true) tmr.Enabled = true; else tmr.Enabled = false;
            }
            catch(System.Exception err)
            {
                return;
            }            
        }

        private void tmr_Tick(object sender, EventArgs e)
        {
            try
            {
                if (todo==true)
                {
                    if (finish == 1) { todo = false; return;  }
                    if (pause == true) { return; }
                    btnstep_Click(sender, e);
                    if (checkpause() == 1)
                    {
                        pause = true;
                        return;
                    }                        
                }
            }
            catch(System.Exception err)
            {
                return;
            }
            
        }


        private void radionotime_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radionotime.Checked == true) tmr.Enabled = false; else tmr.Enabled = true;
            }
            catch (System.Exception err)
            {
                return;
            }   
        }

        private void hscr_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                if (hscr.Value != 0)
                {
                    double HZ = Convert.ToDouble(hscr.Value) / hscr.Maximum * (maxHZ - minHZ) + minHZ;
                    txtinterval.Text = HZ.ToString(); //HZ
                    tmr.Interval = Convert.ToInt32(1.0 / HZ * 1000.0);
                    
                }

            }
            catch (System.Exception err)
            {
                return;
            }
        }

        private void txtinterval_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Enter)
                {
                    double t = (Convert.ToDouble(txtinterval.Text) - minHZ) / (maxHZ - minHZ);
                    hscr.Value = Convert.ToInt32(t * hscr.Maximum);
                }
            }
            catch (System.Exception err)
            {
                return;
            }
        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnpause_Click(object sender, EventArgs e)
        {
            pause = true;
        }

        private void btnresume_Click(object sender, EventArgs e)
        {
            pause = false;
        }

        private void btnstop_Click(object sender, EventArgs e)
        {
            pause = true; 
            btnreset_Click(sender, e);
        }
        
    }
};