using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinFormKalmanRadar {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();

            this.pictureBox1.Image = DrawPlot.getImage(line1, line2, 0, 0);

            UrgPort.processDel = new UrgPort.processData(this.processData);
            freshBoardDel = new freshBoard(this.freshBoardMethod);
        }

        private List<int> line1 = new List<int>();
        private List<int> line2 = new List<int>();


        private UrgPort urg;
        private bool openState = false;
        private void button1_Click(object sender, EventArgs e) {
            urg = UrgPort.getInstance(this.textBox1.Text);
            if (!openState) {
                openState = urg.openPort();
                this.button1.Text = "closePort";
            }
            else {
                openState = !urg.closePort();
                this.button1.Text = "openPort";
            }
        }

        public int num = 384;
        public KalmanSingle kalNum = new KalmanSingle();

        public int average = 0;
        private void processData(List<long> list) {
            if (list.Count < num + 1 || list[num] < 100) {
                return;
            }
            line1.Add((int)list[num]);
            line2.Add(kalNum.getDistance((int)list[num]));
            if (line1.Count < 10) {
                average += line1[line1.Count - 1];
            }
            if (line1.Count == 10) {
                average /= 10;
            }
             Console.WriteLine(line1[line1.Count - 1] + " " + line2[line2.Count - 1]);
            if (freshBoardDel != null) {
                this.Invoke(freshBoardDel, line1.Count / 100 * 100, 1770);
            }
        }

        public delegate void freshBoard(int start, int ave);
        public freshBoard freshBoardDel;
        int i = 0;
        public void freshBoardMethod(int start, int ave) {
            //Console.WriteLine(i++);
            //this.pictureBox1.Image = DrawPlot.addLineOnImage(this.pictureBox1.Image, line1, line2, start, ave, line1.Count - 1);
            this.pictureBox1.Image = DrawPlot.getImage(line1, line2, start, ave);
        }

    }
}
