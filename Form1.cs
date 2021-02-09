using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace _016_01_칼라영상처리2
{
    public partial class Form1 : Form
    {
        //////////////////////////////////
        //      전역 변수
        //////////////////////////////////
        static byte[,,] inImage = null, outImage = null;
        static int inH = 0, inW = 0, outH = 0, outW = 0;
        static string fileName, openFileName;
        static Bitmap paper, bitmap;
        const int RGB = 3, RR = 0, GG = 1, BB = 2;
        static int mouseYN = 0;
        static int down_x = 0, down_y = 0, up_x = 0, up_y = 0;
        static string[] tmpFiles = new string[500];
        static int tmpIndex = 0;

        //////////////////////////////////
        //      이벤트 함수
        //////////////////////////////////
        public Form1()
        {
            InitializeComponent();
            toolStripStatusLabel1.Text = " ";
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.O:
                        open_Image();
                        break;
                    case Keys.S:
                        save_Image();
                        break;
                    case Keys.Z:
                        restoreTempFile();
                        break;
                }
            }
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (mouseYN != 0)
            {
                down_x = e.Y;
                down_y = e.X;
            }
        }
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (mouseYN != 0)
            {
                up_x = e.Y;
                up_y = e.X;

                if (up_x < down_x)
                {
                    int tmp = up_x;
                    up_x = down_x;
                    down_x = tmp;
                }
                if (up_y < down_y)
                {
                    int tmp = up_y;
                    up_y = down_y;
                    down_y = tmp;
                }
                switch (mouseYN)
                {
                    case 1:
                        bright_Image();
                        break;
                    case 2:
                        color_Reverse_Image();
                        break;
                    case 3:
                        bw_Image();
                        break;
                }

                mouseYN = 0;
            }
        }
        private void 열기CtrlOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            open_Image();
        }
        private void 저장CtrlSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            save_Image();
        }
        private void 되돌리기CtrlZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            restoreTempFile();
        }
        private void 도움말ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 미완성
        }
        private void 원본ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openEqual_Image();
        }
        private void 전체지정ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bright_Image();
        }
        private void 마우스로지정ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            mouseYN = 1;
        }
        private void 배확대ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            scaleUp_Image();
        }
        private void 배축소ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            scaleDown_Image();
        }
        private void 전체지정ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            bw_Image();
        }
        private void 마우스로지정ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            mouseYN = 3;
        }
        private void 좌우반전ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            rl_Reverse_Image();
        }
        private void 상하반전ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ud_Reverse_Image();
        }
        private void 전체지정ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            color_Reverse_Image();
        }
        private void 마우스로지정ToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            mouseYN = 2;
        }
        private void 이동ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            move_Image();
        }
        private void 회전ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            rotate_Image();
        }
        private void 경계선검출ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            edge_Image();
        }
        private void 약하게ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mosaic1_Image();
        }
        private void 강하게ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mosaic2_Image();
        }
        private void 엠보싱ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            emboss_Image();
        }
        private void 블러링ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            blurr_Image();
        }
        private void 스트레칭ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            stretch_Image();
        }
        private void 엔드인탐색ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            endin_Image();
        }
        private void 평활화ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            hisequal_Image();
        }

        //////////////////////////////////
        //      공통 함수
        //////////////////////////////////
        void open_Image()
        {       // 이미지 불러오기
            label1.Visible = false;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "칼라 필터|*.png; *.jpg; *.bmp; *.tif";
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            fileName = ofd.FileName;
            openFileName = ofd.FileName;
            bitmap = new Bitmap(openFileName);

            inW = bitmap.Height;
            inH = bitmap.Width;
            inImage = new byte[RGB, inH, inW];
            for (int i = 0; i < inH; i++)
            {
                for (int k = 0; k < inW; k++)
                {
                    Color c = bitmap.GetPixel(i, k);
                    inImage[RR, i, k] = c.R;
                    inImage[GG, i, k] = c.G;
                    inImage[BB, i, k] = c.B;
                }
            }
            equal_Image();
        }
        void save_Image()
        {       // 이미지 저장
            if (outImage == null)
            {
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.DefaultExt = "";
            sfd.Filter = "PNG File(*.png) | *.png";
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string saveFileName = sfd.FileName;
            Bitmap image = new Bitmap(outH, outW);

            for (int i = 0; i < outH; i++)
            {
                for (int k = 0; k < outW; k++)
                {
                    Color c;
                    int r, g, b;
                    r = outImage[RR, i, k];
                    g = outImage[GG, i, k];
                    b = outImage[BB, i, k];
                    c = Color.FromArgb(r, g, b);
                    image.SetPixel(i, k, c);
                }
            }
            image.Save(saveFileName, ImageFormat.Png);
            toolStripStatusLabel1.Text = saveFileName + "으로 저장됨";
        }
        void saveTempFile()
        {       // inImage를 디스크에 저장 후, outImage를 inImage에 저장
            string saveFileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".tmp";
            Bitmap image = new Bitmap(inH, inW);

            for (int i = 0; i < inH; i++)
            {
                for (int k = 0; k < inW; k++)
                {
                    int r = inImage[RR, i, k];
                    int g = inImage[GG, i, k];
                    int b = inImage[BB, i, k];
                    Color c = Color.FromArgb(r, g, b);
                    image.SetPixel(i, k, c);
                }
            }
            image.Save(saveFileName, System.Drawing.Imaging.ImageFormat.Png);
            tmpFiles[tmpIndex++] = saveFileName;

            inH = outH;
            inW = outW;
            inImage = new byte[RGB, inH, inW];
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        inImage[rgb, i, k] = outImage[rgb, i, k];
                    }
                }
            }
        }
        void restoreTempFile()
        {       // 디스크의 최근 파일을 inImage에 복사
            if (tmpIndex <= 0)
            {
                return;
            }       
            fileName = tmpFiles[--tmpIndex];
            bitmap = new Bitmap(fileName);
            inW = bitmap.Height;
            inH = bitmap.Width;
            inImage = new byte[RGB, inH, inW];

            for (int i = 0; i < inH; i++)
            {
                for (int k = 0; k < inW; k++)
                {
                    Color c = bitmap.GetPixel(i, k);
                    inImage[RR, i, k] = c.R;
                    inImage[GG, i, k] = c.G;
                    inImage[BB, i, k] = c.B;
                }
            }
            equal_Image();
            //System.IO.File.Delete(fileName); // 임시파일 삭제 
        }

        void display_Image()
        {       // 이미지 출력
            paper = new Bitmap(outH, outW);
            pictureBox1.Size = new Size(outH, outW);
            this.Size = new Size(outH + 150, outW + 110);

            Color pen;
            for (int i = 0; i < outH; i++)
            {
                for (int k = 0; k < outW; k++)
                {
                    byte r = outImage[RR, i, k];
                    byte g = outImage[GG, i, k];
                    byte b = outImage[BB, i, k];
                    pen = Color.FromArgb(r, g, b);
                    paper.SetPixel(i, k, pen);
                }
            }
            pictureBox1.Image = paper;
            toolStripStatusLabel1.Text = "(" + outH.ToString() + "x" + outW.ToString() + ")  " + fileName;
        }
        double getValue(string label_Text, int max, int min)
        {       // 서브폼의 입력값 반환
            subForm01 sf01 = new subForm01(label_Text, max, min);
            if (sf01.ShowDialog() == DialogResult.Cancel)
            {
                return 0;
            }
            double value = (double)sf01.nud.Value;
            return value;
        }

        //////////////////////////////////
        //      영상처리 함수
        //////////////////////////////////
        void equal_Image()
        {       // outImage 출력
            if (inImage == null)
            {
                return;
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        outImage[rgb, i, k] = inImage[rgb, i, k];
                    }
                }
            }
            display_Image();
        }
        void openEqual_Image()
        {       // 원본 출력
            if (inImage == null)
            {
                return;
            }

            bitmap = new Bitmap(openFileName);
            inW = bitmap.Height;
            inH = bitmap.Width;
            inImage = new byte[RGB, inH, inW];
            for (int i = 0; i < inH; i++)
            {
                for (int k = 0; k < inW; k++)
                {
                    Color c = bitmap.GetPixel(i, k);
                    inImage[RR, i, k] = c.R;
                    inImage[GG, i, k] = c.G;
                    inImage[BB, i, k] = c.B;
                }
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        outImage[rgb, i, k] = inImage[rgb, i, k];
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
        void bright_Image()
        {       // 밝기 조절
            if (inImage == null)
            {
                return;
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            if (up_y > outH || up_x > outW)
            {
                return;
            }
            if (mouseYN == 0)
            {
                down_x = 0;
                down_y = 0;
                up_x = outW;
                up_y = outH;
            }

            int bright = (int)getValue("밝기를 입력하세요. (+:밝게/-:어둡게)", 255, -255);

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        if ((down_x <= k && k <= up_x) && (down_y <= i && i <= up_y))
                        {
                            if ((inImage[rgb, i, k] + bright) > 255)
                            {
                                outImage[rgb, i, k] = 255;
                            }
                            else if ((inImage[rgb, i, k] + bright) < 0)
                            {
                                outImage[rgb, i, k] = 0;
                            }
                            else
                            {
                                outImage[rgb, i, k] = (byte)(inImage[rgb, i, k] + bright);
                            }
                        }
                        else
                        {
                            outImage[rgb, i, k] = inImage[rgb, i, k];
                        }
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
        void bw_Image()
        {       // 흑백 출력 (픽셀의 평균값 기준)
            if (inImage == null)
            {
                return;
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            if (up_y > outH || up_x > outW)
            {
                return;
            }
            if (mouseYN == 0)
            {
                down_x = 0;
                down_y = 0;
                up_x = outW;
                up_y = outH;
            }

            int sum = 0;
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        sum += inImage[rgb, i, k];
                    }
                }
            }
            double avg = (double)sum / (outH * outW * RGB);
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        double rgb_avg = (inImage[RR, i, k] + inImage[GG, i, k] + inImage[BB, i, k]) / RGB;
                        if ((down_x <= k && k <= up_x) && (down_y <= i && i <= up_y))
                        {
                            if (rgb_avg < avg)
                            {
                                outImage[rgb, i, k] = 0;
                            }
                            else
                            {
                                outImage[rgb, i, k] = 255;
                            }
                        }
                        else
                        {
                            outImage[rgb, i, k] = inImage[rgb, i, k];
                        }
                    }
                }
            }
            saveTempFile();
            display_Image();
        }
        void color_Reverse_Image()
        {       // 색상 반전 출력
            if (inImage == null)
            {
                return;
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            if (up_y > outH || up_x > outW)
            {
                return;
            }
            if (mouseYN == 0)
            {
                down_x = 0;
                down_y = 0;
                up_x = outW;
                up_y = outH;
            }

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        if ((down_x <= k && k <= up_x) && (down_y <= i && i <= up_y))
                        {
                            outImage[rgb, i, k] = (byte)(255 - inImage[rgb, i, k]);
                        }
                        else
                        {
                            outImage[rgb, i, k] = inImage[rgb, i, k];
                        }
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
        void rl_Reverse_Image()
        {       // 좌우 반전 출력
            if (inImage == null)
            {
                return;
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        outImage[rgb, i, k] = inImage[rgb, outH - i - 1, k];
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
        void ud_Reverse_Image()
        {       // 상하 반전 출력
            if (inImage == null)
            {
                return;
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        outImage[rgb, i, k] = inImage[rgb, i, outW - k - 1];
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
        void scaleUp_Image()
        {       // 크기 조절(확대)
            if (inImage == null)
            {
                return;
            }

            outH = inH * 2;
            outW = inW * 2;
            outImage = new byte[RGB, outH, outW];

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        outImage[rgb, i, k] = inImage[rgb, i / 2, k / 2];
                    }
                }
            }
            display_Image();
            saveTempFile();
        }

        void scaleDown_Image()
        {       // 크기 조절(축소)
            if (inImage == null)
            {
                return;
            }

            outH = inH / 2;
            outW = inW / 2;
            outImage = new byte[RGB, outH, outW];

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        outImage[rgb, i, k] = inImage[rgb, i * 2, k * 2];
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
        void move_Image()
        {       // 이동 출력
            if (inImage == null)
            {
                return;
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            int y = (int)getValue("이동 방향과 거리를 입력하세요. (+:우/-:좌)", outW, -outW);
            int x = (int)getValue("이동 방향과 거리를 입력하세요. (+:하/-:상)", outH, -outH);

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        outImage[rgb, i, k] = 0;
                    }
                }
            }

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    if ((i + y) < 0 || (i + y) >= outH)
                    {
                        continue;
                    }
                    for (int k = 0; k < outW; k++)
                    {
                        if ((k + x) < 0 || (k + x) >= outW)
                        {
                            continue;
                        }
                        outImage[rgb, i + y, k + x] = inImage[rgb, i, k];
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
        void rotate_Image()
        {       // 회전 출력
            if (inImage == null)
            {
                return;
            }

            double deg = (double)getValue("회전할 각도를 입력하세요. (시계방향)", 360, 0);
            double PI = 3.141592;
            double rad = -deg * PI / 180.0;

            outH = (int)(Math.Abs(inW * Math.Sin(rad)) + Math.Abs(inH * Math.Cos(rad)));
            outW = (int)(Math.Abs(inH * Math.Sin(rad)) + Math.Abs(inW * Math.Cos(rad)));
            if (outH < inH)
            {
                outH = inH;
            }
            if (outW < inW)
            {
                outW = inW;
            }
            outImage = new byte[RGB, outH, outW];

            int new_x, new_y;
            int center_x = outW / 2, center_y = outH / 2;

            byte[,,] rot_Image = new byte[RGB, outH, outW];
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = center_y - (inH / 2), a = 0; i < center_y + (inH / 2); i++, a++)
                {
                    for (int k = center_x - (inW / 2), b = 0; k < center_x + (inW / 2); k++, b++)
                    {
                        rot_Image[rgb, i, k] = inImage[rgb, a, b];
                    }
                }
            }

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        new_x = (int)((i - center_y) * Math.Sin(rad) + (k - center_x) * Math.Cos(rad) + center_x);
                        new_y = (int)((i - center_y) * Math.Cos(rad) - (k - center_x) * Math.Sin(rad) + center_y);
                        if (new_y < 0 || new_y >= outH)
                        {
                            outImage[rgb, i, k] = 0;
                        }
                        else if (new_x < 0 || new_x >= outW)
                        {
                            outImage[rgb, i, k] = 0;
                        }
                        else
                        {
                            outImage[rgb, i, k] = rot_Image[rgb, new_y, new_x];
                        }
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
        void mosaic1_Image()
        {       // 모자이크 출력(약하게)
            if (inImage == null)
            {
                return;
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            int mosaic = 8;

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i += mosaic)
                {
                    for (int k = 0; k < outW; k += mosaic)
                    {
                        if ((i + mosaic) > outH || (k + mosaic) > outW)
                        {
                            for (int a = 0; a < (outH % mosaic); a++)
                            {
                                for (int b = 0; b < (outW % mosaic); b++)
                                {
                                    outImage[rgb, i + a, k + b] = inImage[rgb, i + a, k + b];
                                }
                            }
                        }
                        else
                        {
                            for (int a = 0; a < mosaic; a++)
                            {
                                for (int b = 0; b < mosaic; b++)
                                {
                                    outImage[rgb, i + a, k + b] = inImage[rgb, i + (mosaic / 2), k + (mosaic / 2)];
                                }
                            }
                        }
                    }
                }
            }
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = (outH - mosaic); i < outH; i++)
                {
                    for (int k = (outW - mosaic); k < outW; k++)
                    {
                        outImage[rgb, i, k] = inImage[rgb, i, k];
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
        void mosaic2_Image()
        {       // 모자이크 출력(강하게)
            if (inImage == null)
            {
                return;
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            int mosaic = 16;

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i += mosaic)
                {
                    for (int k = 0; k < outW; k += mosaic)
                    {
                        if ((i + mosaic) > outH || (k + mosaic) > outW)
                        {
                            for (int a = 0; a < (outH % mosaic); a++)
                            {
                                for (int b = 0; b < (outW % mosaic); b++)
                                {
                                    outImage[rgb, i + a, k + b] = inImage[rgb, i + a, k + b];
                                }
                            }
                        }
                        else
                        {
                            for (int a = 0; a < mosaic; a++)
                            {
                                for (int b = 0; b < mosaic; b++)
                                {
                                    outImage[rgb, i + a, k + b] = inImage[rgb, i + (mosaic / 2), k + (mosaic / 2)];
                                }
                            }
                        }
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
        void emboss_Image()
        {       // 엠보싱 출력
            if (inImage == null)
            {
                return;
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            const int MSIZE = 3;
            double[,] mask = { { -1.0, 0.0, 0.0 }, { 0.0, 0.0, 0.0 }, { 0.0, 0.0, 1.0 } };

            double[,,] tmp_Input = new double[RGB, inH + 2, inW + 2];
            double[,,] tmp_Output = new double[RGB, outH, outW];

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < inH + 2; i++)
                {
                    for (int k = 0; k < inW + 2; k++)
                    {
                        tmp_Input[rgb, i, k] = 127.0;
                    }
                }
            }
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < inH; i++)
                {
                    for (int k = 0; k < inW; k++)
                    {
                        tmp_Input[rgb, i + 1, k + 1] = inImage[rgb, i, k];
                    }
                }
            }

            double sum = 0.0;
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < inH; i++)
                {
                    for (int k = 0; k < inW; k++)
                    {
                        for (int a = 0; a < MSIZE; a++)
                        {
                            for (int b = 0; b < MSIZE; b++)
                            {
                                sum += tmp_Input[rgb, i + a, k + b] * mask[a, b];
                            }
                        }
                        tmp_Output[rgb, i, k] = sum + 127;
                        sum = 0.0;
                    }
                }
            }

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        if (tmp_Output[rgb, i, k] > 255)
                        {
                            tmp_Output[rgb, i, k] = 255;
                        }
                        else if (tmp_Output[rgb, i, k] < 0)
                        {
                            tmp_Output[rgb, i, k] = 0;
                        }
                        outImage[rgb, i, k] = (byte)tmp_Output[rgb, i, k];
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
        void blurr_Image()
        {       // 블러링 출력
            if (inImage == null)
            {
                return;
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            const int MSIZE = 3;
            double[,] mask = { { 1 / 9.0, 1 / 9.0, 1 / 9.0 }, { 1 / 9.0, 1 / 9.0, 1 / 9.0 }, { 1 / 9.0, 1 / 9.0, 1 / 9.0 } };

            double[,,] tmp_Input = new double[RGB, inH + 2, inW + 2];
            double[,,] tmp_Output = new double[RGB, outH, outW];

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < inH + 2; i++)
                {
                    for (int k = 0; k < inW + 2; k++)
                    {
                        tmp_Input[rgb, i, k] = 127.0;
                    }
                }
            }
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < inH; i++)
                {
                    for (int k = 0; k < inW; k++)
                    {
                        tmp_Input[rgb, i + 1, k + 1] = inImage[rgb, i, k];
                    }
                }
            }

            double sum = 0.0;
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < inH; i++)
                {
                    for (int k = 0; k < inW; k++)
                    {
                        for (int a = 0; a < MSIZE; a++)
                        {
                            for (int b = 0; b < MSIZE; b++)
                            {
                                sum += tmp_Input[rgb, i + a, k + b] * mask[a, b];
                            }
                        }
                        tmp_Output[rgb, i, k] = sum;
                        sum = 0.0;
                    }
                }
            }

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        if (tmp_Output[rgb, i, k] > 255)
                        {
                            tmp_Output[rgb, i, k] = 255;
                        }
                        else if (tmp_Output[rgb, i, k] < 0)
                        {
                            tmp_Output[rgb, i, k] = 0;
                        }
                        outImage[rgb, i, k] = (byte)tmp_Output[rgb, i, k];
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
        void edge_Image()
        {       // 경계선 검출(sobel mask 이용)
            if (inImage == null)
            {
                return;
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            int[,] mask_x = new int[3, 3] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] mask_y = new int[3, 3] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        outImage[rgb, i, k] = 0;
                    }
                }
            }
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 1; i < outH - 1; i++)
                {
                    for (int k = 1; k < outW - 1; k++)
                    {
                        float sum_x = 0, sum_y = 0;

                        for (int n = 0; n < 3; n++)
                        {
                            for (int m = 0; m < 3; m++)
                            {
                                sum_x += mask_x[n, m] * inImage[rgb, i + n - 1, k + m - 1];
                                sum_y += mask_y[n, m] * inImage[rgb, i + n - 1, k + m - 1];
                            }
                        }
                        double mag = Math.Sqrt(sum_x * sum_x + sum_y * sum_y);
                        if (mag > 255)
                        {
                            mag = 255;
                        }
                        else if (mag < 0)
                        {
                            mag = 0;
                        }
                        outImage[rgb, i, k] = (byte)mag;
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
        void stretch_Image()
        {       // 선명하게 출력
            if (inImage == null)
            {
                return;
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            byte min_num = inImage[0, 0, 0], max_num = inImage[0, 0, 0];
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < inH; i++)
                {
                    for (int k = 0; k < inW; k++)
                    {
                        if (min_num > inImage[rgb, i, k])
                        {
                            min_num = inImage[rgb, i, k];
                        }
                        if (max_num < inImage[rgb, i, k])
                        {
                            max_num = inImage[rgb, i, k];
                        }
                    }
                }
            }

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        outImage[rgb, i, k] = (byte)((double)(inImage[rgb, i, k] - min_num) / (max_num - min_num) * 255);
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
        void endin_Image()
        {       // 강제로 선명하게 출력
            if (inImage == null)
            {
                return;
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            byte min_num = inImage[0, 0, 0], max_num = inImage[0, 0, 0];
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < inH; i++)
                {
                    for (int k = 0; k < inW; k++)
                    {
                        if (min_num > inImage[rgb, i, k])
                        {
                            min_num = inImage[rgb, i, k];
                        }
                        if (max_num < inImage[rgb, i, k])
                        {
                            max_num = inImage[rgb, i, k];
                        }
                    }
                }
            }
            min_num += 50;
            max_num -= 50;
            for (int rgb = 0; rgb < RGB; rgb++)
            {
                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        double value = (double)(inImage[rgb, i, k] - min_num) / (max_num - min_num) * 255;
                        if (value > 255)
                        {
                            value = 255;
                        }
                        else if (value < 0)
                        {
                            value = 0;
                        }
                        outImage[rgb, i, k] = (byte)value;
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
        void hisequal_Image()
        {       // 평활화 출력
            if (inImage == null)
            {
                return;
            }

            outH = inH;
            outW = inW;
            outImage = new byte[RGB, outH, outW];

            for (int rgb = 0; rgb < RGB; rgb++)
            {
                long sum = 0;
                long[] hist = new long[256];
                long[] sum_hist = new long[256];
                double[] normal_hist = new double[256];
                for (int i = 0; i < 256; i++)
                {
                    hist[i] = 0;
                }
                for (int i = 0; i < inH; i++)
                {
                    for (int k = 0; k < inW; k++)
                    {
                        hist[inImage[rgb, i, k]]++;
                    }
                }
                for (int i = 0; i < 256; i++)
                {
                    sum += hist[i];
                    sum_hist[i] = sum;
                }
                for (int i = 0; i < 256; i++)
                {
                    normal_hist[i] = ((double)sum_hist[i] / (inH * inW)) * 255.0;
                }

                for (int i = 0; i < outH; i++)
                {
                    for (int k = 0; k < outW; k++)
                    {
                        outImage[rgb, i, k] = (byte)normal_hist[inImage[rgb, i, k]];
                    }
                }
            }
            display_Image();
            saveTempFile();
        }
    }
}
