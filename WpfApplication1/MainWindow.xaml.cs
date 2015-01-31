using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    enum Mask : short
    {
        SPADE,
        HEART,
        CLUB,
        DIAMOND
    }

    public partial class MainWindow : Window
    {
        const int DECKSIZE = 52;
        ArrayList drawPile = new ArrayList();
        ArrayList p1Cards = new ArrayList();
        ArrayList p2Cards = new ArrayList();
        Random rnd = new Random();
        int selectedCard = 0;
        int superIndicator = 0;


        public MainWindow()
        {
            InitializeComponent();
            for (int i = 0; i < 52; ++i)
            {
                drawPile.Add(i);
            }
        }

        private int dealCard(ArrayList drawPile)
        {
            int result = -1;
            if (drawPile.Count==0) Environment.Exit(0);
            result = rnd.Next(0, drawPile.Count);
            drawPile.Remove(result);
            return result;
        }

        private void updateCard(ArrayList cards)
        {
            Card1.Content = toFace((int)cards[0]);
            Card2.Content = toFace((int)cards[1]);
            Card3.Content = toFace((int)cards[2]);
            Card4.Content = toFace((int)cards[3]);
            if (cards.Count > 4)
            {
                Card5.Content = toFace((int)cards[4]);
            }
            else
            {
                Card5.Content = "";
            }
        }
        private string toFace(int index)
        {
            int cardNum = index / 4 + 1;
            string face;
            switch (cardNum)
            {
                case 11:
                    face = "J";
                    break;
                case 12:
                    face = "Q";
                    break;
                case 13:
                    face = "K";
                    break;
                case 1:
                    face = "A";
                    break;
                default:
                    face = cardNum.ToString();
                    break;
            }
            int cardMask = cardNum % 4;
            face += (cardMask==0)?"s":(cardMask==1)?"h":(cardMask==2)?"c":"d";
            return face;
        }

        private void StartButton1_Click(object sender, RoutedEventArgs e)
        {
            p1Cards.Add(dealCard(drawPile));
            p1Cards.Add(dealCard(drawPile));
            p1Cards.Add(dealCard(drawPile));
            p1Cards.Add(dealCard(drawPile));
            p1Cards.Add(dealCard(drawPile));

            p2Cards.Add(dealCard(drawPile));
            p2Cards.Add(dealCard(drawPile));
            p2Cards.Add(dealCard(drawPile));
            p2Cards.Add(dealCard(drawPile));
            p2Cards.Add(dealCard(drawPile));

            SuperButton.Content = "Declare";
            updateCard(p1Cards);

        }
        private void MaskCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DeclareButton_Click(object sender, RoutedEventArgs e)
        {
            if (superIndicator == 0)
            {
                bool success = false;
                if (MaskCombo.SelectedIndex == 0 && NumberCombo.SelectedIndex == 0)
                {
                    InstructionLabel.Content = "Please at least declare a restriction";
                }
                else if (MaskCombo.SelectedIndex == 0)
                {   //number declared
                    foreach (int card in p2Cards)
                    {
                        if (card / 4 + 1 == NumberCombo.SelectedIndex)
                        {
                            success = true;
                        }
                    }
                }
                else if (NumberCombo.SelectedIndex == 0)
                {   //mask declared
                    foreach (int card in p2Cards)
                    {
                        if (card % 4 + 1 == MaskCombo.SelectedIndex)
                        {
                            success = true;
                        }
                    }
                }
                else
                {
                    int declared = MaskCombo.SelectedIndex + (NumberCombo.SelectedIndex - 1) * 4;
                    foreach (int card in p2Cards)
                    {
                        if (card == declared)
                        {
                            success = true;
                        }
                    }
                }

                if (success)
                {
                    //exchange
                    SuperButton.Content.Equals("Give");
                    superIndicator = 1;
                }
                else
                {
                    SuperButton.Content.Equals("Discard");
                    superIndicator = 3;
                }
            }
            else if (superIndicator == 1)
            {
                int p1Card = (int)p1Cards[selectedCard];
                int p2Selected = rnd.Next(0, p2Cards.Count);
                p1Cards[selectedCard] = p2Cards[p2Selected];
                p2Cards[p2Selected] = p1Card;
                if (p1Cards.Count == 4)
                {
                    p1Cards.Add(dealCard(drawPile));
                }
                updateCard(p1Cards);
            }
            else if (superIndicator ==2)
            {
                int p1Card = (int)p1Cards[selectedCard];
                int p2Selected = rnd.Next(0, p2Cards.Count);
                p1Cards[selectedCard] = p2Cards[p2Selected];
                p2Cards[p2Selected] = p1Card;
                if (p1Cards.Count == 4)
                {
                    p1Cards.Add(dealCard(drawPile));
                }
                updateCard(p1Cards);
            }
            else if (superIndicator==3)
            {
                p1Cards.Remove(selectedCard);
                p2Turn();
            }
        }

        //simple p2 ai
        private void p2Turn()
        {
            SuperButton.Content = "Declare";
            superIndicator = 0;
            if (p2Cards.Count == 4)
            {
                p2Cards.Add(dealCard(drawPile));
            }
            bool success = false;
            int maskSel,numSel;
            do{
            maskSel = rnd.Next(0, 5);
            numSel = rnd.Next(0, 14);
            }while (maskSel == 0 && numSel == 0);
            
            if (maskSel == 0)
            {   //number declared
                foreach (int card in p1Cards)
                {
                    if (card / 4 + 1 == numSel)
                    {
                        success = true;
                    }
                }
            }
            else if (numSel == 0)
            {   //mask declared
                foreach (int card in p1Cards)
                {
                    if (card % 4 + 1 == maskSel)
                    {
                        success = true;
                    }
                }
            }
            else
            {
                int declared = maskSel + (numSel - 1) * 4;
                foreach (int card in p1Cards)
                {
                    if (card == declared)
                    {
                        success = true;
                    }
                }
            }

            if (success)
            {
                //give
                SuperButton.Content = "Choose";
                superIndicator = 2;
            }
            else
            {
                //discard
                p2Cards.Remove(rnd.Next(0,p2Cards.Count));
                if (p1Cards.Count == 4)
                {
                    p1Cards.Add(dealCard(drawPile));
                }
                updateCard(p1Cards);
            }

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Card1_Click(object sender, RoutedEventArgs e)
        {
            selectedCard = 0;
        }

        private void Card2_Click(object sender, RoutedEventArgs e)
        {
            selectedCard = 1;
        }
        private void Card3_Click(object sender, RoutedEventArgs e)
        {
            selectedCard = 2;
        }
        private void Card4_Click(object sender, RoutedEventArgs e)
        {
            selectedCard = 3;
        }
        private void Card5_Click(object sender, RoutedEventArgs e)
        {
            selectedCard = 4;
        }

        private void Card1_Copy_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
