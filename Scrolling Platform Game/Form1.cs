using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Scrolling_Platform_Game
{
    public partial class Form1 : Form
    {
        bool goLeft = false; //boolean care controleaza deplasarea la stanga 
        bool goRight = false; //boolean care controleaza deplasarea la dreapta 
        bool jumping = false; //boolean care testeaza daca jucatorul sare 
        bool hasKey = false; //boolean care testeaza daca jucatorul are cheia 

        int jumpSpeed = 10; //integer care seteaza viteza la salt
        int force = 8; //forta cu care sare 
        int score = 0; //scorul jucatorului

        int playSpeed = 18; //viteza de deplasare a jucatorului
        int backLeft = 20; //viteza cu care se misca fundalul in spate 

        public Form1()
        {
            InitializeComponent();
        }

        private void mainGameTimer(object sender, EventArgs e) //eventul cronometrului, functioneaza ca un while, dar reintra in secventa de cod dupa un interval de timp (20 de milisecunde aici)
        {
            player.Top += jumpSpeed; //in functie de valoarea variabilei jumpSpeed, determinam daca jucatorul sare sau cade 

            player.Refresh(); //dam refresh pentru a actualiza pictureBox-ul cu jucatorul

            if (jumping && force < 0)
            {
                jumping = false; //daca forta saltului a ajuns la 0 inseamna ca nu mai sare 
            }

            if (jumping) //daca e in salt 
            {
                jumpSpeed = -12; //setam la -12, numarul de pixeli cu care jucatorul sare
                force -= 1; //forta scade cu cate 1 astfel incat saltul sa se apropie treptat de limita 
            }
            else //daca nu e in salt 
            {
                jumpSpeed = 12; //cade cu 12 pixeli/tick
            }

            if (goLeft && player.Left > 100)
            {
                player.Left -= playSpeed; //daca se deplaseaza la stanga, deplasam pictureboxul, dar nu mai in stanga de marginea stanga a ecranului
                player.Image = Properties.Resources.player_stanga; //schimbam imaginea daca jucatorul se intoarce spre stanga 
            }

            if (goRight && player.Left + player.Width + 100 < this.ClientSize.Width)
            {
                player.Left += playSpeed; //daca se deplaseaza la dreapta, deplasam pictureboxul, dar nu mai in dreapta de marginea dreapta a ecranului
                player.Image = Properties.Resources.player; //schimbam imaginea daca jucatorul se intoarce spre dreapta
            }

            if (goRight && background.Left > -1352)
            {
                background.Left -= backLeft; //daca am ajuns cu backgroundul la marginea din dreapta, il deplasam catre stanga 

                foreach (Control x in this.Controls)
                {
                    if (x is PictureBox && (x.Tag.ToString() == "platform" || x.Tag.ToString() == "coin" || x.Tag.ToString() == "door" || x.Tag.ToString() == "key"))
                    {
                        x.Left -= backLeft; //in acest caz deplasam si toate obiectele catre stanga 
                    }
                }
            }

            if (goLeft && background.Left < 2)
            {
                background.Left += backLeft; //daca am ajuns cu backgroundul la marginea din stanga, il deplasam catre dreapta

                foreach (Control x in this.Controls)
                {
                    if (x is PictureBox && (x.Tag.ToString() == "platform" || x.Tag.ToString() == "coin" || x.Tag.ToString() == "door" || x.Tag.ToString() == "key"))
                    {
                        x.Left += backLeft; //in acest caz deplasam si toate obiectele catre dreapta 
                    }
                }
            }

            //in urmatorul for verificam interactiunea jucatorului cu diverse alte picturebox-uri (platforme, banuti)
            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && x.Tag.ToString() == "platform")
                {
                    //daca player se intersecteaza cu o platforma si nu sare, il plasam deasupra platformei
                    if (player.Bounds.IntersectsWith(x.Bounds) && !jumping)
                    {
                        force = 8;
                        player.Top = x.Top - player.Height;
                        jumpSpeed = 0;
                    }
                }

                if (x is PictureBox && x.Tag.ToString() == "coin")
                {
                    //daca player se intersecteaza cu un banut, banutul dispare deoarece este colectat si scorul creste
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        score++;
                        lbScore.Text = score.ToString(); //afisam scorul
                    }
                }
            }

            //Daca jucatorul se intesecteaza cu usa si are cheia, nivelul este complet, oprim timer-ul astfel incat controalele sa nu se mai deplaseze si afisam mesaj
            if (player.Bounds.IntersectsWith(door.Bounds) && hasKey)
            {
                door.Image = Properties.Resources.door_open;
                gameTimer.Stop();
                MessageBox.Show("Level complete!");
            }

            //Daca jucatorul se intersecteaza cu cheia, o scoatem si setam variabila hasKey pe true
            if (player.Bounds.IntersectsWith(key.Bounds))
            {
                this.Controls.Remove(key);
                hasKey = true;
            }

            //daca jucatorul iese prin partea de jos a ecranului a murit
            if (player.Top + player.Height > this.ClientSize.Height +60)
            {
                gameTimer.Stop();
                MessageBox.Show("You died!");
            }
        }

        private void keyisup(object sender, KeyEventArgs e) //daca jucatorul a ridicat degetul de pe o anumita tasta 
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = false; //daca nu mai apasa sageata stanga, variabila de deplasare la stanga e false => jucatorul nu se va deplasa la stanga
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = false; //daca nu mai apasa sageata dreapta, variabila de deplasare la dreapta e false => jucatorul nu se va deplasa la dreapta
            }

            if (jumping)
            {
                jumping = false; //cand tastele nu mai sunt apasate, setam variabila de salt la false ca sa poata sari din nou
            }
        }

        private void keyisdown(object sender, KeyEventArgs e) //daca jucatorul a apasat o anumita tasta 
        {
            if (e.KeyCode == Keys.Left)
            {
                goLeft = true; //daca apasa sageata stanga, variabila de deplasare la stanga e true => jucatorul se va deplasa la stanga
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = true; //daca apasa sageata dreapta, variabila de deplasare la dreapta e true => jucatorul se va deplasa la dreapta
            }

            if (e.KeyCode == Keys.Space && !jumping)
            {
                jumping = true; //daca nu era deja in salt si apasa spatiu, variabila de salt devine true => incepe saltul
            }
        }
    }
}
//Sursa: https://www.mooict.com/c-tutorial-create-a-side-scrolling-platform-game-in-visual-studio/
//Pentru a adauga event-uri la orice control (fereastra, pictureBox, timer etc), se selecteaza controlul in designer, in fereastra de proprietati a acestuia se da click pe fulgerul din partea de sus
//Se cauta in lista eventul dorit, se scrie numele functiei care va fi apelata, se apasa Enter si functia se adauga in cod 
