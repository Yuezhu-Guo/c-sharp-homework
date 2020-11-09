using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace class01_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        int a, b;
        double result;
        string op;
        int time = 50;
        bool isStart = false;
        int num = 5;
        int n = 5;
        int crctNum = 0;

        private void Form1_Load(object sender,EventArgs e)
        {
            btnNew_Click(sender, e);
            label5.Text = time.ToString();
        }

        private void btnScore_Click(object sender, EventArgs e)
        {
            string d = textBox1.Text;

            if (d == "")
            {
                d = "0";
            }
            double r = double.Parse(d);
            string sResult = " " + a + op + b + "=" + d + "";

            if(n > 0)
            {
                if (result == r)
                {
                    sResult += "√";
                    crctNum++;
                    
                }
                else
                {
                    sResult += "×";
                    
                }
                textBox2.Text = textBox2.Text + "\r\n" + sResult;
                n--;
                btnNew_Click(sender, e);
                textBox3.Text = n.ToString();
            }
            else  //达到指定数目则显示正确个数，得分，并停止倒计时
            {
                textBox2.Text = textBox2.Text + "\r\n" + "正确个数：" + crctNum +"\r\n" + "得分：" + ((crctNum * 1.0) / (num * 1.0)) * 100;
                timer1.Enabled = false;
                btnTime.Text = "Start";
            }
            

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            time --;
            label5.Text = time.ToString();
            if(time == 0) //超时则跳过该题并自动出题
            {
                time = 50; 
                btnNew_Click(sender, e);
            }
        }

        private void btnTime_Click(object sender, EventArgs e)
        {
            isStart = !isStart;
            if (isStart)
            {
                time = 50;
                label5.Text = time.ToString();
                timer1.Enabled = true;
                btnTime.Text = "Stop";
            }
            else
            {
                label5.Text = time.ToString();
                timer1.Enabled = false;
                btnTime.Text = "Start";
            }
        }

        Random rd = new Random();

        private void btnNew_Click(object sender, EventArgs e)
        {
            textBox3.Text = num.ToString();
            a = rd.Next(9) + 1;
            b = rd.Next(9) + 1;
            int c = rd.Next(4);
            switch (c)
            {
                case 0: op = "+"; result = a + b; break;
                case 1: op = "-"; result = a - b; break;
                case 2: op = "*"; result = a * b; break;
                case 3: op = "/"; result = (double)a / (double)b; break; //结果为Double型
            }
            label1.Text = a.ToString();
            label2.Text = b.ToString();
            label3.Text = op;
            textBox1.Text = "";
            time = 50;
        }

    }
}
