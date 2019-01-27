using System;
using System.Drawing;
using System.Windows.Forms;

namespace DES {
    class CipherForm : Form {
        public NumericUpDown Input, Output, Key;
    
        //конструктор формы
        public CipherForm() {
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(165, 235);
            FormBorderStyle = FormBorderStyle.FixedDialog;
        
            Input = new NumericUpDown();
            Input.Size = new Size(60, 40);
            Input.Location = new Point(10, 10);
            Input.Maximum = 255;
            Controls.Add(Input);          
      
            Output = new NumericUpDown();
            Output.Location = new Point(10, 155);
            Output.Size = new Size(60, 40);
            Output.Maximum = 255;
            Controls.Add(Output);             
      
            Label shiftLb = new Label();
            shiftLb.Location = new Point(15, 40);
            shiftLb.Size = new Size(40, 40);
            shiftLb.TextAlign = ContentAlignment.MiddleCenter;
            shiftLb.Text = "Ключ:";
            Controls.Add(shiftLb);
      
            Key = new NumericUpDown();
            Key.Location = new Point(70, 50);
            Key.Size = new Size(60,40);
            Key.Maximum = 1023;
            Controls.Add(Key);
      
            Button encryptButton = new Button();
            encryptButton.Location = new Point(25, 80);
            encryptButton.Size = new Size(100,30);
            encryptButton.Text = "Зашифрувати";
            encryptButton.Click += EncryptClick;
            Controls.Add(encryptButton);
        }
        //ивенты нажатий на кнопки
        void EncryptClick(object sender, EventArgs e) {
            Output.Value = Calculus.Encrypt((int)Input.Value,(int)Key.Value);
        }
    }
}
