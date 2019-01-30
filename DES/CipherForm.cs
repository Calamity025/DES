using System;
using System.Drawing;
using System.Windows.Forms;

namespace DES {
    class CipherForm : Form {
        public TextBox Input, Output;
        public NumericUpDown Key;
    
        //конструктор формы
        public CipherForm() {
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(285, 340);
            FormBorderStyle = FormBorderStyle.FixedDialog;
        
            Input = new TextBox();
            Input.Size = new Size(250, 100);
            Input.Location = new Point(10, 10);
            Input.Multiline = true;
            Input.Font = new Font(Input.Font.FontFamily, 14);
            Controls.Add(Input);
      
            Output = new TextBox();
            Output.Location = new Point(10, 185);
            Output.Size = new Size(250, 100);
            Output.Multiline = true;
            Output.Font = new Font(Output.Font.FontFamily, 14);
            Output.Text = "";
            Controls.Add(Output); 
      
            Label shiftLb = new Label();
            shiftLb.Location = new Point(15, 130);
            shiftLb.Size = new Size(60, 40);
            shiftLb.TextAlign = ContentAlignment.MiddleCenter;
            shiftLb.Text = "Здвиг для шифру:";
            Controls.Add(shiftLb);
      
            Key = new NumericUpDown();
            Key.Location = new Point(85, 140);
            Key.Value = 14;
            Key.Width = 50;
            Key.Maximum = 1023;
            Controls.Add(Key);
      
            Button encryptButton = new Button();
            encryptButton.Location = new Point(150, 115);
            encryptButton.Size = new Size(100,30);
            encryptButton.Text = "Зашифрувати";
            encryptButton.Click += EncryptClick;
            Controls.Add(encryptButton);
      
            Button decryptButton = new Button();
            decryptButton.Location = new Point(150, 150);
            decryptButton.Size = new Size(100,30);
            decryptButton.Text = "Дешифрувати";
            decryptButton.Click += DecryptClick;
            Controls.Add(decryptButton);
        }
        //ивенты нажатий на кнопки
        void DecryptClick(object sender, EventArgs e) {
        }
        void EncryptClick(object sender, EventArgs e) {
            string temp = "";
            for (int i = 0; i < Input.TextLength; i++) {
                temp += Convert.ToString(Calculus.Encrypt(Input.Text[i], (int) Key.Value));
            }
            Output.Text = temp;
        }
    }
}
