using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;


namespace CasinoDiscordBot
{
    public class Bot
    {
        DiscordClient client;
        CommandService commands;
        BlackJack bj = new BlackJack();
        string message;

        public Bot()
        {
            // Initialize DiscordClient
            client = new DiscordClient(input =>
            {
                input.LogLevel = LogSeverity.Info;
                input.LogHandler = Log;
                input.AppName = "Casino Bot";
                input.AppVersion = "0.01";
            });

            // Associate Commands to Client
            client.UsingCommands(input =>
            {
                input.PrefixChar = '.';
                input.AllowMentionPrefix = true;
            });

            commands = client.GetService<CommandService>();

////////////////// Commands //////////////////////////////////////////////////////////////////
            // --- PLAY--- (Begin the game)
            commands.CreateCommand("play")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    bj = new BlackJack();
                    message = "Welcome to BlackJack \nYour Chips : " + bj.getChips()
                        + "\nHow much will you bet .min ( 5 ) or .max ( 100 )?";
                    await (e.Channel.SendMessage(message));
                });

            // --- DRAW --- (Draw a card)
            commands.CreateCommand("draw")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    // if the hand has been split I want to draw to my player hand first
                    // but only if I have not called to hold my player hand first

                    bj.determinePlay("draw");

                    // display the player hand regardless of whether or not it has been held
                    // if you drew a card and you lost as a results end the game
                    if(!bj.isHandSplit() && !bj.playerHold)
                    {
                        // if the player hand value is greater than 21 they lose
                        // get the value of the split hand
                        if (bj.getCurrentHandValue(false) > 21)
                        {
                            // you lost so remove the chips from your pot
                            bj.adjustPlayerChips(2);
                            message = "You lost \nYour Chips : " + bj.getChips()
                                + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                                + "\tHand Value : " + bj.getCurrentHandValue(false)
                                + "\nDealer Hand : " + displayHand(bj.getCompHand())
                                + "\nWould you like me to .deal again? or .quit?";
                            await (e.Channel.SendMessage(message));
                        }
                        else
                        {
                            // if you haven't lost show the split hand value
                            message = "Your Chips: " + bj.getChips()
                                + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                                + "\tHand Value : " + bj.getCurrentHandValue(false);
                            await (e.Channel.SendMessage(message));
                        }
                    }

                    // if the hand has been spplit and I haven't already held my player hand
                    // I want to play to that hand first
                    if(bj.isHandSplit() && !bj.playerHold)
                    {
                        // if the player hand value is greater than 21 they lose
                        // get the value of the split hand
                        if (bj.getCurrentHandValue(false) > 21)
                        {
                            // you lost so remove the chips from your pot
                            bj.adjustPlayerChips(2);
                            bj.playerHold = true;
                            message = "You lost \nYour Chips : " + bj.getChips()
                                + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                                + "\tHand Value : " + bj.getCurrentHandValue(false);
                            await (e.Channel.SendMessage(message));
                        }
                        else
                        {
                            // if you haven't lost show the split hand value
                            message = "Your Chips: " + bj.getChips()
                                + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                                + "\tHand Value : " + bj.getCurrentHandValue(false);
                            await (e.Channel.SendMessage(message));
                        }
                    }

                    // if the hand has been split and I have already held on my player hand its time to play to 
                    // the split hand
                    if (bj.isHandSplit() && bj.playerHold)
                    {
                        // if the player hand value is greater than 21 they lose
                        // get the value of the split hand
                        if (bj.getCurrentHandValue(true) > 21)
                        {
                            // you lost so remove the chips from your pot
                            bj.adjustPlayerChips(2);
                            bj.splitHold = true;
                            message = "You lost \nYour Chips : " + bj.getChips()
                                + "\nYour Split Hand : " + displayHand(bj.getSplitHand())
                                + "\tSplit Hand Value : " + bj.getCurrentHandValue(true);
                            await (e.Channel.SendMessage(message));
                        }
                        else
                        {
                            // if you haven't lost show the split hand value
                            message = "Your Chips: " + bj.getChips()
                                + "\nYour Split Hand : " + displayHand(bj.getSplitHand())
                                + "\tSplit Hand Value : " + bj.getCurrentHandValue(true);
                            await (e.Channel.SendMessage(message));
                        }
                    }






                    // display the split hand only if there is one
                    // if you drew a card and you lost as a result end the game


                    // display the dealer hand regardless of what happened above





                    // If the hand has been split
                    if (bj.isHandSplit())
                    {
                        // get the value of the split hand
                        if (bj.getCurrentHandValue(true) > 21)
                        {
                            // you lost so remove the chips from your pot
                            bj.adjustPlayerChips(2);
                            message = "You lost \nYour Chips : " + bj.getChips()
                                + "\nYour Split Hand : " + displayHand(bj.getPlayerHand())
                                + "\tSplit Hand Value : " + bj.getCurrentHandValue(true)
                                + "\nDealer Hand : " + displayHand(bj.getCompHand())
                                + "\nWould you like me to .deal again? or .quit?";
                            await (e.Channel.SendMessage(message));
                        }
                        else
                        {
                            // if you haven't lost show the split hand value
                            message = "Your Chips: " + bj.getChips() 
                                + "\nYour Split Hand : " + displayHand(bj.getSplitHand()) 
                                + "\tSplit Hand Value : " + bj.getCurrentHandValue(true);
                            await (e.Channel.SendMessage(message));
                        }
                    }
                    // get the player hand current value
                    else if (bj.getCurrentHandValue(false) > 21)
                    {
                        // you lost so remove the chips from your pot
                        message = "You lost \nYour Chips : " + bj.getChips()
                            + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false)
                            + "\nDealer Hand : " + displayHand(bj.getCompHand())
                            + "\tComp Hand Value : " + getCurrentHandValue(bj.getCompHand())
                            + "\nWould you like me to .deal again? or .quit?";
                        bj.adjustPlayerChips(2);
                        await (e.Channel.SendMessage(message));
                    }
                    else
                    {
                        // other wise show the player hand value
                        message = "Your Chips: " + bj.getChips()
                            + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false);
                        await (e.Channel.SendMessage(message));
                        message = "\nDealer Hand : ? " + displayCard(bj.getCompHand()[1])
                            + "\nWhat will you do? .draw, .double down, or .hold";
                        await (e.Channel.SendMessage(message));
                    }
                });

            // --- MIN --- (Bet the Minimum amount of chips)
            commands.CreateCommand("min")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    if(bj.getChips() >= 5)
                    {
                        bj.bet(5);
                    }
                    else
                    {
                        bj.buyIn();
                        bj.bet(5);
                        await (e.Channel.SendMessage("A gracious donor has made a contribution to your addiction.."));
                    }

                    if (bj.getCurrentHandValue(false) == 21)
                    {
                        bj.adjustPlayerChips(1);
                        message = "You Won \nYour Chips : " + bj.getChips()
                            + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false)
                            + "\nDealer Hand : " + displayHand(bj.getCompHand())
                            + "\tComp Hand Value : " + getCurrentHandValue(bj.getCompHand())
                            + "\nWould you like me to .deal again? or .quit?";
                        await (e.Channel.SendMessage(message));
                    }
                    else
                    {
                        message = "Your Chips : " + bj.getChips()
                            + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false)
                            + "\nDealer Hand : ? " + displayCard(bj.getCompHand()[1])
                            + "\nWhat will you do? .draw, .split, .double down, or .hold";
                        await (e.Channel.SendMessage(message));
                    }
                });
            
            // --- MAX --- (Bet the Maximum amount of chips)
            commands.CreateCommand("max")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    if (bj.getChips() >= 100)
                    {
                        bj.bet(100);
                    }
                    else
                    {
                        bj.buyIn();
                        bj.bet(100);
                        await (e.Channel.SendMessage("A gracious donor has made a contribution to your addiction.."));
                    }
                    if (bj.getCurrentHandValue(false) == 21)
                    {
                        bj.adjustPlayerChips(1);
                        message = "You Won \nYour Chips : " + bj.getChips()
                            + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false)
                            + "\nDealer Hand : " + displayHand(bj.getCompHand())
                            + "\tComp Hand Value : " + getCurrentHandValue(bj.getCompHand())
                            + "\nWould you like me to .deal again? or .quit?";
                        await (e.Channel.SendMessage(message));
                    }
                    else
                    {
                        message = "Your Chips : " + bj.getChips()
                            + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false)
                            + "\nDealer Hand : ? " + displayCard(bj.getCompHand()[1])
                            + "\nWhat will you do? .draw, .split, .double down, or .hold";
                        await (e.Channel.SendMessage(message));
                    }
                });

            // --- SPLIT --- (Split the current hand into two hands)
            commands.CreateCommand("split")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    bj.determinePlay("split");
                    message = "Your Chips : " + bj.getChips() + "\nYour Hand : "
                        + displayHand(bj.getPlayerHand()) + "\tHand Value : "
                        + bj.getCurrentHandValue(false)
                        + "\nYour Split Hand : " + displayHand(bj.getSplitHand())
                        + "\tSplit Hand Value : " + bj.getCurrentHandValue(true)
                        + "\nDealer Hand : ? " + displayCard(bj.getCompHand()[1])
                        + "\nWhat will you do? .draw, .double down, or .hold";
                    await (e.Channel.SendMessage(message));
                });

            // --- DOUBLE DOWN --- (Double your bet for this hand (can only be done before the frist draw))
            commands.CreateCommand("double down")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    // if you haven't already drawn a card
                    if(!bj.didDrawCard())
                    {
                        await (e.Channel.SendMessage("Cannot double down, please select a different option"));
                        message = "Your Chips : " + bj.getChips()
                            + "Your Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false);
                        await (e.Channel.SendMessage(message));
                        if (bj.isHandSplit())
                        {
                            message = "\nYour Split Hand : " + displayHand(bj.getSplitHand())
                                + "\tSplit Hand Value : " + bj.getCurrentHandValue(true);
                            await (e.Channel.SendMessage(message));
                        }
                        await (e.Channel.SendMessage("\nDealer Hand : ? " + displayCard(bj.getCompHand()[1]) + "\nWhat will you do? .draw or .hold"));
                    }
                    // if you haven't already drawn a card
                    else
                    {
                        bj.determinePlay("double down");
                        message = "Your Chips : " + bj.getChips()
                            + "Your Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false);
                        await (e.Channel.SendMessage(message));
                        if (bj.isHandSplit())
                        {
                            message = "\nYour Split Hand : " + displayHand(bj.getSplitHand())
                                + "\tSplit Hand Value : " + bj.getCurrentHandValue(true);
                            await (e.Channel.SendMessage(message));
                        }
                        await (e.Channel.SendMessage("\nDealer Hand : ? " + displayCard(bj.getCompHand()[1]) + "\nWhat will you do? .draw or .hold"));
                    }
                    
                });

            //TODO: In the following command, logic needs to be put in place that will allow for holding on either hand

            // --- HOLD --- (Hold the current hand and see how you fare against the Computers hand)
            commands.CreateCommand("hold")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    // if this is the player hand and there is no split hand
                    if(!bj.playerHold && !bj.isHandSplit())
                    {
                        bj.playerHold = true;

                        bj.determinePlay("hold");
                        message = "Your Chips : " + bj.getChips()
                                  + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                                  + "\tHand Value : " + bj.getCurrentHandValue(false);
                        await (e.Channel.SendMessage(message));

                        message = "\nDealer Hand : " + displayHand(bj.getCompHand())
                                + "\tComp Hand Value : " + getCurrentHandValue(bj.getCompHand());
                        await (e.Channel.SendMessage(message));

                        // get the result of the hand
                        if (bj.result(false) == 1)
                        {
                            bj.adjustPlayerChips(1);
                            message = "\nYou won! \nYour Chips : " + bj.getChips()
                                + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                                + "\tHand Value : " + bj.getCurrentHandValue(false)
                                + "\nWould you like me to .deal again? or .quit?";
                            await (e.Channel.SendMessage(message));
                        }
                        else if (bj.result(false) == 2)
                        {
                            bj.adjustPlayerChips(2);
                            message = "\nYou lost \nYour Chips : " + bj.getChips()
                                + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                                + "\tHand Value : " + bj.getCurrentHandValue(false)
                                + "\nWould you like me to .deal again? or .quit?";
                            await (e.Channel.SendMessage(message));
                        }
                        else
                        {
                            bj.adjustPlayerChips(3);
                            message = "\nYou tied \nYour Chips : " + bj.getChips()
                                + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                                + "\tHand Value : " + bj.getCurrentHandValue(false)
                                + "\nWould you like me to .deal again? or .quit?";
                            await (e.Channel.SendMessage(message));
                        }
                    }

                    // if this is the player hand and there is a split hand
                    if (bj.isHandSplit() && !bj.splitHold && !bj.playerHold)
                    {
                        bj.playerHold = true;
                        // hold the split hand
                        bj.determinePlay("hold");
                        message = "\nYour Chips : " + bj.getChips()
                                  + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                                  + "\tHand Value : " + bj.getCurrentHandValue(false);
                        await (e.Channel.SendMessage(message));

                        message = "\nYour Second Hand: " + displayHand(bj.getSplitHand())
                                + "\tSplit Hand Value : " + bj.getCurrentHandValue(true);
                        await (e.Channel.SendMessage(message));

                        message = "\nDealer Hand : ? " + displayCard(bj.getCompHand()[1])
                                    + "\nWhat will you do? .draw, .double down, or .hold";
                        await (e.Channel.SendMessage(message));
                    }

                    // if this is the split hand that you are holding
                    if (bj.isHandSplit() && !bj.splitHold)
                    {
                        bj.splitHold = true;
                        // Get the results of the hands
                        //(PLAYER HAND)
                        if (bj.result(false) == 1)
                        {
                            bj.adjustPlayerChips(1);
                            message = "\nYou won! \nYour Chips : " + bj.getChips()
                                + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                                + "\tHand Value : " + bj.getCurrentHandValue(false);
                            await (e.Channel.SendMessage(message));
                        }
                        else if (bj.result(false) == 2)
                        {
                            bj.adjustPlayerChips(2);
                            message = "\nYou lost \nYour Chips : " + bj.getChips()
                                + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                                + "\tHand Value : " + bj.getCurrentHandValue(false);
                            await (e.Channel.SendMessage(message));
                        }
                        else
                        {
                            bj.adjustPlayerChips(3);
                            message = "\nYou tied \nYour Chips : " + bj.getChips()
                                + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                                + "\tHand Value : " + bj.getCurrentHandValue(false);
                            await (e.Channel.SendMessage(message));
                        }

                        //(SPLIT HAND)
                        if (bj.result(true) == 1)
                        {
                            bj.adjustPlayerChips(1);
                            message = "\nYou won! \nYour Chips : " + bj.getChips()
                                + "\nYour Second Hand : " + displayHand(bj.getSplitHand())
                                + "\tSecond Hand Value : " + bj.getCurrentHandValue(true);
                            await (e.Channel.SendMessage(message));
                        }
                        else if (bj.result(true) == 2)
                        {
                            bj.adjustPlayerChips(2);
                            message = "\nYou lost \nYour Chips : " + bj.getChips()
                                + "\nYour Second Hand : " + displayHand(bj.getSplitHand())
                                + "\tSecond Hand Value : " + bj.getCurrentHandValue(true);
                            await (e.Channel.SendMessage(message));
                        }
                        else
                        {
                            bj.adjustPlayerChips(3);
                            message = "\nYou tied \nYour Chips : " + bj.getChips()
                                + "\nYour Second Hand : " + displayHand(bj.getSplitHand())
                                + "\tSecond Hand Value : " + bj.getCurrentHandValue(true);
                            await (e.Channel.SendMessage(message));
                        }
                    }
                    
                });

            // --- DEAL --- (Deal another hand)
            commands.CreateCommand("deal")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    bj.determinePlay("deal");
                    await (e.Channel.SendMessage("\nYour Chips : " + bj.getChips() + "\nHow much will you bet .min ( 5 ) or .max ( 100 )?"));
                });

            // --- QUIT --- (Quit the game and forfeit your winnings)
            commands.CreateCommand("quit")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await (e.Channel.SendMessage("Thanks for playing!"));
                    bj = new BlackJack();
                });

            // Call this last
            // Execute the client (with all associated commands)
            client.ExecuteAndWait(async () =>
            {
                await (client.Connect("MzA3OTAzNDYwMjgyNDY2MzA0.C-ZFSQ.weJRxtNtOjTnzbIDCXZfD6wKXyk", TokenType.Bot));
            });
        }

        //TODO: Move all of these helper functions into the Blackjack class where they belong
        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message.ToString());
        }

        private string displayHand(int[] playerHand)
        {
            string temp = " ";
            for(int i = 0; i < playerHand.Length; i++)
            {
                if(playerHand[i] < 11 && playerHand[i] > 1)
                {
                    temp += playerHand[i] + " ";
                }
                if (playerHand[i] == 1)
                {
                    temp += "A ";
                }
                if (playerHand[i] == 11)
                {
                    temp += "J ";
                }
                if (playerHand[i] == 12)
                {
                    temp += "Q ";
                }
                if (playerHand[i] == 13)
                {
                    temp += "K ";
                }
            }
            return temp;
        }

        private string displayCard(int card)
        {
            string temp = "";
            if (card < 11 && card > 1)
            {
                temp += card + " ";
            }
            if (card == 1)
            {
                temp += "A ";
            }
            if (card == 11)
            {
                temp += "J ";
            }
            if (card == 12)
            {
                temp += "Q ";
            }
            if (card == 13)
            {
                temp += "K ";
            }
            return temp;
        }

        public int getCurrentHandValue(int[] playerHand)
        {
            // Assume that an ace is eleven unless the value of the hand exceeds 21
            // All face cards equal ten
            int temp = 0;
            // if the players hand has been split

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

            return temp;
        }
    }
}
