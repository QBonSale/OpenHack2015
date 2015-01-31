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
            result = (int)drawPile[rnd.Next(0, drawPile.Count)];
            drawPile.Remove(result);
            InstructionLabel.Content += "\nA card is drawed";
            return result;
        }

        private void updateCard()
        {
            Card1.Content = toFace((int)p1Cards[0]);
            Card2.Content = toFace((int)p1Cards[1]);
            Card3.Content = toFace((int)p1Cards[2]);
            Card4.Content = toFace((int)p1Cards[3]);
            if (p1Cards.Count > 4)
            {
                Card5.Content = toFace((int)p1Cards[4]);
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
            int cardMask = index % 4;
            face += (cardMask==0)?"s":(cardMask==1)?"h":(cardMask==2)?"c":"d";
            return face;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
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
            //InstructionLabel.Content = "Please select a combination to declare";

            updateCard();

        }
        private void MaskCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void SuperButton_Click(object sender, RoutedEventArgs e)
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
                    InstructionLabel.Content = "Hit! Please select a card to exchange.";
                    SuperButton.Content = "Give";
                    superIndicator = 1;
                }
                else
                {
                    InstructionLabel.Content = "No Hit! Please select a card to discard.";
                    SuperButton.Content = "Discard";
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
                updateCard();
                SuperButton.Content = "Declare";
                superIndicator = 0;
                p2Turn();
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
                updateCard();
                SuperButton.Content = "Declare";
                superIndicator = 0;
            }
            else if (superIndicator==3)
            {
                p1Cards.RemoveAt(selectedCard);
                updateCard();
                p2Turn();
            }
        }

        //simple p2 ai
        private void p2Turn()
        {
            SuperButton.Content = "Declare";
            InstructionLabel.Content = "Enemy's turn";
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
            InstructionLabel.Content += "\nEnemy declared a " + ((maskSel == 0) ? "Not Specified" : (maskSel == 1) ? "Spade" : (maskSel == 2) ? "Heart" : (maskSel == 3) ? "Club" : "Diamond") + "\n with a " +
                ((numSel == 0) ? "Not Specified" : (numSel == 1) ? "A" : (numSel == 13) ? "K" : (numSel == 12) ? "Q" : (numSel == 11) ? "J" : numSel.ToString());
            if (success)
            {
                //give
                InstructionLabel.Content += "\nHit! Please select a card to exchange.";
                SuperButton.Content = "Choose";
                superIndicator = 2;
            }
            else
            {
                //discard
                InstructionLabel.Content += "\nNoHit! Enemy select a card to discard .";
                p2Cards.RemoveAt(rnd.Next(0,p2Cards.Count));
                if (p1Cards.Count == 4)
                {
                    p1Cards.Add(dealCard(drawPile));
                }
                updateCard();
            }

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Card1_Click(object sender, RoutedEventArgs e)
        {
            selectedCard = 0;
            InstructionLabel.Content = "You just selected " + toFace((int)p1Cards[selectedCard]);
        }

        private void Card2_Click(object sender, RoutedEventArgs e)
        {
            selectedCard = 1;
            InstructionLabel.Content = "You just selected " + toFace((int)p1Cards[selectedCard]);
        }
        private void Card3_Click(object sender, RoutedEventArgs e)
        {
            selectedCard = 2;
            InstructionLabel.Content = "You just selected " + toFace((int)p1Cards[selectedCard]);

        }
        private void Card4_Click(object sender, RoutedEventArgs e)
        {
            selectedCard = 3;
            InstructionLabel.Content = "You just selected " + toFace((int)p1Cards[selectedCard]);

        }
        private void Card5_Click(object sender, RoutedEventArgs e)
        {
            selectedCard = 4;
            InstructionLabel.Content = "You just selected " + toFace((int)p1Cards[selectedCard]);

        }
    }
}
