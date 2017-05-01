using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CasinoDiscordBot
{
    //TODO: If you get a black jack you immediately win!!! fill in logic for that

    public class BlackJack
    {
        int[] playerHand { get; set; }
        int[] splitHand { get; set; }
        int[] computerHand { get; set; }
        int[] deck;
        int[] newDeck;
        int playerChips { get; set; }
        int currentBet { get; set; }
        Random rand = new Random(DateTime.Now.Millisecond);
        public bool playerHold = false;
        public bool splitHold = false;
        bool handSplit = false;
        bool isPlayerHand = true;
        bool drewCard = false;
        const int MAX_BET = 500;
        const int MIN_BET = 5;
        const int MAX_NUM_OF_EACH_CARD = 20;
        int playerHandIterator = 2;
        int splitHandIterator = 2;
        int compHandIterator = 2;
        private static int NUM_ACE, NUM_TWO, NUM_THREE, NUM_FOUR, NUM_FIVE, NUM_SIX, NUM_SEVEN, NUM_EIGHT, NUM_NINE, NUM_TEN, NUM_JACK, NUM_QUEEN, NUM_KING = MAX_NUM_OF_EACH_CARD;

        public BlackJack()
        {
            playerHand = new int[21];
            computerHand = new int[21];
            splitHand = new int[21];
            deck = new int[260];
            playerChips = 500;
            currentBet = 0;
            beginGame();
        }

        public bool didDrawCard()
        {
            return drewCard;
        }

        public bool isHandSplit()
        {
            return handSplit;
        }

        public void splitPlayerHand()
        {
            handSplit = true;
        }

        public bool HandIsFinished()
        {
            bool finished = false;
            if (isHandSplit() && playerHold && splitHold)
            {
                finished = true;
            }
            if(!isHandSplit() && playerHold)
            {
                finished = true;
            }
            return finished;
        }

        public int[] getPlayerHand()
        {
            return playerHand;
        }

        public int[] getSplitHand()
        {
            return splitHand;
        }

        public int[] getCompHand()
        {
            return computerHand;
        }

        public int getChips()
        {
            return playerChips;
        }

        public int getCurrentBet()
        {
            return currentBet;
        }

        public int getCurrentHandValue(bool isSplitHand)
        {

            // Assume that an ace is eleven unless the value of the hand exceeds 21
            // All face cards equal ten
            int temp = 0;
            // if the players hand has been split
            if(isSplitHand)
            {
                int splitHandAces = 0;
                for (int i = 0; i < splitHand.Length; i++)
                {
                    // if the card is not a face card or an ace
                    if(splitHand[i] < 11 && splitHand[i] > 1)
                    {
                        temp += splitHand[i];
                    }
                    // if the card is a face card
                    else if(splitHand[i] > 10)
                    {
                        temp += 10;
                    }
                    // if the card is an ace
                    else if(splitHand[i] == 1)
                    {
                        temp += 11;
                        splitHandAces++;
                    }
                }
                // if the hand has aces in it and the value of the hand exceeds 21
                // remove ten points for every ace as long as the hand exceeds 21
                for(int i = 0; i < splitHandAces; i++)
                {
                    if(temp > 21)
                    {
                        temp -= 10;
                    }
                }
            }
            // If the players hand has not been split
            else
            {
                int handAces = 0;
                for (int i = 0; i < playerHand.Length; i++)
                {
                    // if the card is not a face card or an ace
                    if (playerHand[i] < 11 && playerHand[i] > 1)
                    {
                        temp += playerHand[i];
                    }
                    // if the card is a face card
                    else if (playerHand[i] > 10)
                    {
                        temp += 10;
                    }
                    // if the card is an ace
                    else if (playerHand[i] == 1)
                    {
                        temp += 11;
                        handAces++;
                    }
                }
                // if the hand has aces in it and the value of the hand exceeds 21
                // remove ten points for every ace as long as the hand exceeds 21
                for (int i = 0; i < handAces; i++)
                {
                    if (temp > 21)
                    {
                        temp -= 10;
                    }
                }
            }
            return temp;
        }

        private void beginGame()
        {
            shuffle();

            drawHand(true);
            drawHand(false);
        }

        public void reDeal()
        {
            playerHand = new int[21];
            computerHand = new int[21];
            splitHand = new int[21];
            currentBet = 0;
            playerHold = false;
            handSplit = false;

            drawHand(true);
            drawHand(false);
        }

        private void drawHand(bool player)
        {
            if(player)
            {
                playerHand[0] = drawCard();
                playerHand[1] = drawCard();
            }
            if (!player)
            {
                computerHand[0] = drawCard();
                computerHand[1] = drawCard();
            }
        }

        private void shuffle()
        {
            // randomize the deck
            for (int i = 0; i < deck.Length; i++)
            {
                deck[i] = setCard(getRandom());
            }
        }

        private int drawCard()
        {
            // Draw the top card off the deck
            int returnCard = deck[0];

            // the deck now has one less card in it
            newDeck = new int[deck.Length - 1];

            // take all cards but the first card in the deck and place them in the new deck
            for (int i = 1; i < deck.Length; i++)
            {
                newDeck[i - 1] = deck[i];
            }

            // set the deck to the new size
            deck = new int[newDeck.Length];

            // place all the cards back in the deck
            for (int i = 0; i < newDeck.Length; i++)
            {
                deck[i] = newDeck[i];
            }

            // return the top card
            return returnCard;
        }

        private int determineWinner(bool split)
        {
            int playerHandValue = 0;
            int splitHandValue = 0;
            int compHandValue = 0;

            bool playerWin = false;
            bool splitWin = false;
            // if the hand has been split get the current split hand value
            if (isHandSplit())
            {
                splitHandValue = getCurrentHandValue(true);
            }
            // if it hasn't get the current hand value
            else
            {
                playerHandValue = getCurrentHandValue(false);
            }

            int handAces = 0;
            for (int i = 0; i < computerHand.Length; i++)
            {
                // if the card is not a face card or an ace
                if (computerHand[i] < 11 && computerHand[i] > 1)
                {
                    compHandValue += computerHand[i];
                }
                // if the card is a face card
                else if (computerHand[i] > 10)
                {
                    compHandValue += 10;
                }
                // if the card is an ace
                else if (computerHand[i] == 1)
                {
                    compHandValue += 11;
                    handAces++;
                }
            }
            // if the hand has aces in it and the value of the hand exceeds 21
            // remove ten points for every ace as long as the hand exceeds 21
            for (int i = 0; i < handAces; i++)
            {
                if (compHandValue > 21)
                {
                    compHandValue -= 10;
                }
            }
            
            int ptemp = 0;
            int stemp = 0;

            // if both hands are equal
            if (playerHandValue == compHandValue)
            {
                ptemp = 3;
            }

            if (splitHandValue == compHandValue)
            {
                stemp = 3;
            }

            // player hand is higher than computer and player hand is less than or equal to 21 -- the player wins
            if(playerHandValue > compHandValue && playerHandValue <= 21)
            {
                ptemp = 1;
                playerWin = true;
            }
            // split hand is higher than computer and split hand is less than or equal to 21 -- the split hand wins
            if (splitHandValue > compHandValue && splitHandValue <= 21)
            {
                stemp = 1;
                splitWin = true;
            }
            // if comp hand is higher than player, and comp hand is less than or equal to 21 == comp wins
            if ((compHandValue > playerHandValue && compHandValue <= 21))
            {
                ptemp = 2;
                playerWin = false;
            }
            // if comp hand is higher than sp[lit, and comp hand is less than or equal to 21 == comp wins
            if ((compHandValue > splitHandValue && compHandValue <= 21))
            {
                stemp = 2;
                splitWin = false;
            }
            // if player hand is greater than 21 -- player loses
            if (playerHandValue > 21)
            {
                ptemp = 2;
                playerWin = false;
            }
            // if split hand is greater than 21 -- split loses
            if (splitHandValue > 21)
            {
                stemp = 2;
                splitWin = false;
            }
            // if comp hand is greater than 21 and player hand is less than or equal to 21 -- player wins
            if (compHandValue > 21 && playerHandValue <= 21)
            {
                ptemp = 1;
                playerWin = true;
            }
            // if comp hand is greater than 21 and split hand is less than or equal to 21 -- split wins
            if (compHandValue > 21 && splitHandValue <= 21)
            {
                stemp = 1;
                splitWin = true;
            }

            int temp = 0;
            if (split)
            {
                temp = stemp;
            }
            else
            {
                temp = ptemp;
            }
            
            return temp;
        }

        public void determinePlay(string decision)
        {
            // pass the decision of the player through call the respective method
            switch (decision)
            {
                case "draw":
                    // if you haven't split your hand
                    if(!isHandSplit())
                    {
                        playerHand[playerHandIterator] = drawCard();
                        playerHandIterator++;
                        drewCard = true;
                    }
                    // if you have split your hand
                    else
                    {
                        // if this is your split hand
                        if(!isPlayerHand)
                        {
                            splitHand[splitHandIterator] = drawCard();
                            splitHandIterator++;
                            drewCard = true;
                        }
                        // if this is your player hand
                        else
                        {
                            playerHand[playerHandIterator] = drawCard();
                            playerHandIterator++;
                            drewCard = true;
                        }
                    }
                    break;
                case "split":
                    split();
                    break;
                case "hold":
                    hold();
                    break;
                case "double down":
                    doubleDown();
                    break;
                case "deal":
                    reDeal();
                    break;
                default:
                    Console.WriteLine("Not a valid option please choose either draw, double down, split or hold");
                    break;
            }
        }

        public int result(bool split)
        {
            int temp = determineWinner(split);

            return temp;
        }

        public void adjustPlayerChips(int result)
        {
            if (result == 1)
            {
                playerChips += (int)(Math.Round(currentBet + (currentBet * 1.5)));
            }
            if (result == 2)
            {
                playerChips -= currentBet;
            }
            if (result == 3)
            {
                //playerChips += currentBet;
            }
        }

        public void bet(int amount)
        {
            // set the current bet and take those chips away from the player until the round ends
            if(amount <= MAX_BET && amount >= 5 && playerChips > 0)
            {
                currentBet = amount;
            }
            else if(playerChips > 0)
            {
                currentBet = 5;
            }
        }

        private void doubleDown()
        {
            if(!drewCard)
            {
                currentBet *= 2;
                playerHand[playerHandIterator] = drawCard();
            }
        }

        private void split()
        {
            if(!handSplit)
            {
                splitPlayerHand();
                handSplit = true;
                // split the hand into two hands 
                splitHand[0] = playerHand[1];
                int temp = playerHand[0];
                playerHand = new int[21];
                playerHand[0] = temp;

                splitHand[1] = drawCard();
                playerHand[1] = drawCard();
            }
        }

        private void hold()
        {
            // if this is the split hand then hold that hand
            if (isHandSplit())
            {
                splitHold = true;
            }
            // if this is the player hand
            else 
            {
                playerHold = true;
            }
            // if both hands have been held play the computer hand
            if (playerHold && splitHold)
            {
                playComputerHand();
            }
        }

        public void playComputerHand()
        {
            int compHandValue = 0;
            int aces = 0;

            for (int i = 0; i < computerHand.Length; i++)
            {
                if(computerHand[i] > 1 && computerHand[i] < 11)
                {
                    compHandValue += computerHand[i];
                }
                else if(computerHand[i] > 10)
                {
                    compHandValue += 10;
                }
                else if(computerHand[i] == 1)
                {
                    aces++;
                    compHandValue += 11;
                }
            }

            for (int i = 0; i < aces; i++)
            {
                if (compHandValue > 21)
                {
                    compHandValue -= 10;
                }
            }

            if (compHandValue < 17)
            {
                computerHand[compHandIterator] = drawCard();
                compHandIterator++;
                playComputerHand();
            }
        }

        private int setCard(int num)
        {
            switch (num)
            {
                case 1:
                    if (NUM_ACE > 0 && NUM_ACE <= 20)
                    {
                        NUM_ACE -= 1;
                    }
                    else
                    {
                        getRandom();
                    }
                    break;
                case 2:
                    if (NUM_TWO > 0 && NUM_TWO <= 20)
                    {
                        NUM_TWO -= 1;
                    }
                    else
                    {
                        getRandom();
                    }
                    break;
                case 3:
                    if (NUM_THREE > 0 && NUM_THREE <= 20)
                    {
                        NUM_THREE -= 1;
                    }
                    else
                    {
                        getRandom();
                    }
                    break;
                case 4:
                    if (NUM_FOUR > 0 && NUM_FOUR <= 20)
                    {
                        NUM_FOUR -= 1;
                    }
                    else
                    {
                        getRandom();
                    }
                    break;
                case 5:
                    if (NUM_FIVE > 0 && NUM_FIVE <= 20)
                    {
                        NUM_FIVE -= 1;
                    }
                    else
                    {
                        getRandom();
                    }
                    break;
                case 6:
                    if (NUM_SIX > 0 && NUM_SIX <= 20)
                    {
                        NUM_SIX -= 1;
                    }
                    else
                    {
                        getRandom();
                    }
                    break;
                case 7:
                    if (NUM_SEVEN > 0 && NUM_SEVEN <= 20)
                    {
                        NUM_SEVEN -= 1;
                    }
                    else
                    {
                        getRandom();
                    }
                    break;
                case 8:
                    if (NUM_EIGHT > 0 && NUM_EIGHT <= 20)
                    {
                        NUM_EIGHT -= 1;
                    }
                    else
                    {
                        getRandom();
                    }
                    break;
                case 9:
                    if (NUM_NINE > 0 && NUM_NINE <= 20)
                    {
                        NUM_NINE -= 1;
                    }
                    else
                    {
                        getRandom();
                    }
                    break;
                case 10:
                    if (NUM_TEN > 0 && NUM_TEN <= 20)
                    {
                        NUM_TEN -= 1;
                    }
                    else
                    {
                        getRandom();
                    }
                    break;
                case 11:
                    if (NUM_JACK > 0 && NUM_JACK <= 20)
                    {
                        NUM_JACK -= 1;
                    }
                    else
                    {
                        getRandom();
                    }
                    break;
                case 12:
                    if (NUM_QUEEN > 0 && NUM_QUEEN <= 20)
                    {
                        NUM_QUEEN -= 1;
                    }
                    else
                    {
                        getRandom();
                    }
                    break;
                case 13:
                    if (NUM_KING > 0 && NUM_KING <= 20)
                    {
                        NUM_KING -= 1;
                    }
                    else
                    {
                        getRandom();
                    }
                    break;
            }
            return num;
        }

        private int getRandom()
        {
            return rand.Next(1, 14);
        }

        public void buyIn()
        {
            playerChips += 500;
        }
    }
}
