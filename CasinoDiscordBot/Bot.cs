﻿using System;
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
            client = new DiscordClient(input =>
            {
                input.LogLevel = LogSeverity.Info;
                input.LogHandler = Log;
                input.AppName = "Casino Bot";
                input.AppVersion = "0.01";
            });

            client.UsingCommands(input =>
            {
                input.PrefixChar = '.';
                input.AllowMentionPrefix = true;
            });

            commands = client.GetService<CommandService>();

            // Commands
            commands.CreateCommand("play")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    message = @"Welcome to BlackJack \nYour Chips : " + bj.getChips()
                        + "\nHow much will you bet .min ( 5 ) or .max ( 100 )?";
                    await (e.Channel.SendMessage(message));
                });

            commands.CreateCommand("draw")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    
                    bj.determinePlay("draw");

                    if(bj.getCurrentHandValue(false) > 21)
                    {
                        message = @"You lost \nYour Chips : " + bj.getChips() 
                            + "\nYour Hand : " + displayHand(bj.getPlayerHand()) 
                            + "\tHand Value : " + bj.getCurrentHandValue(false) 
                            + "Dealer Hand : " + displayHand(bj.getCompHand()) 
                            + "\tComp Hand Value : " + getCurrentHandValue(bj.getCompHand()) 
                            + "\nWould you like me to .deal again? or .quit?";
                        bj.adjustPlayerChips(2);
                        await (e.Channel.SendMessage(message));
                    }
                    else
                    {
                        message = @"Your Chips: " + bj.getChips()
                            + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false);
                        await (e.Channel.SendMessage(message));
                    }
                    if(bj.isHandSplit())
                    {
                        if (bj.getCurrentHandValue(true) > 21)
                        {
                            bj.adjustPlayerChips(2);
                            message = @"You lost \nYour Chips : " + bj.getChips()
                                + "\nYour Split Hand : " + displayHand(bj.getPlayerHand())
                                + "\tSplit Hand Value : " + bj.getCurrentHandValue(true)
                                + "Dealer Hand : " + displayHand(bj.getCompHand())
                                + "\nWould you like me to .deal again? or .quit?";
                            await (e.Channel.SendMessage(message));
                        }
                        else
                        {
                            message = @"Your Chips: " + bj.getChips() 
                                + "\nYour Split Hand : " + displayHand(bj.getSplitHand()) 
                                + "\tSplit Hand Value : " + bj.getCurrentHandValue(true);
                            await (e.Channel.SendMessage(message));
                        }
                    }
                    if(!(bj.getCurrentHandValue(false) > 21) && !(bj.getCurrentHandValue(true) > 21))
                    {
                        message = @"Dealer Hand : ? " + displayCard(bj.getCompHand()[1]) 
                            + "\nWhat will you do?" + "\n.draw, .double down, or .hold";
                        await (e.Channel.SendMessage(message));
                    }
                });

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
                        message = @"You Won \nYour Chips : " + bj.getChips()
                            + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false)
                            + "Dealer Hand : " + displayHand(bj.getCompHand())
                            + "\tComp Hand Value : " + getCurrentHandValue(bj.getCompHand())
                            + "\nWould you like me to .deal again? or .quit?";
                        await (e.Channel.SendMessage(message));
                    }
                    else
                    {
                        message = @"Your Chips : " + bj.getChips()
                            + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false)
                            + "Dealer Hand : ? " + displayCard(bj.getCompHand()[1])
                            + "\nWhat will you do?" + "\n.draw, .split, .double down, or .hold";
                        await (e.Channel.SendMessage(message));
                    }
                });

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
                        message = @"You Won \nYour Chips : " + bj.getChips()
                            + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false)
                            + "Dealer Hand : " + displayHand(bj.getCompHand())
                            + "\tComp Hand Value : " + getCurrentHandValue(bj.getCompHand())
                            + "\nWould you like me to .deal again? or .quit?";
                        await (e.Channel.SendMessage(message));
                    }
                    else
                    {
                        message = @"Your Chips : " + bj.getChips()
                            + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false)
                            + "Dealer Hand : ? " + displayCard(bj.getCompHand()[1])
                            + "\nWhat will you do?" + "\n.draw, .split, .double down, or .hold";
                        await (e.Channel.SendMessage(message));
                    }
                });

            commands.CreateCommand("split")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    bj.determinePlay("split");
                    message = @"Your Chips : " + bj.getChips() + "\nYour Hand : "
                        + displayHand(bj.getPlayerHand()) + "\tHand Value : "
                        + bj.getCurrentHandValue(false)
                        + "Your Split Hand : " + displayHand(bj.getSplitHand())
                        + "\tSplit Hand Value : " + bj.getCurrentHandValue(true)
                        + "Dealer Hand : ? " + displayCard(bj.getCompHand()[1])
                        + "\nWhat will you do?" + "\n.draw, .double down, or .hold";
                    await (e.Channel.SendMessage(message));
                });

            commands.CreateCommand("double down")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    bj.determinePlay("double down");
                    message = @"Your Chips : " + bj.getChips()
                        + "Your Hand : " + displayHand(bj.getPlayerHand())
                        + "\tHand Value : " + bj.getCurrentHandValue(false);
                    await (e.Channel.SendMessage(message));
                    if (bj.isHandSplit())
                    {
                        message = @"Your Split Hand : " + displayHand(bj.getSplitHand())
                            + "\tSplit Hand Value : " + bj.getCurrentHandValue(true);
                        await (e.Channel.SendMessage(message));
                    }
                    await (e.Channel.SendMessage("Dealer Hand : ? " + displayCard(bj.getCompHand()[1]) + "\nWhat will you do?" + "\n.draw or .hold"));
                });

            commands.CreateCommand("hold")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    bj.determinePlay("hold");
                    if(bj.result() == 1)
                    {
                        bj.adjustPlayerChips(1);
                        message = @"You won! \nYour Chips : " + bj.getChips()
                            + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false);
                        await (e.Channel.SendMessage(message));
                    }
                    else if(bj.result() == 2)
                    {
                        bj.adjustPlayerChips(2);
                        message = @"You lost \nYour Chips : " + bj.getChips()
                            + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false);
                        await (e.Channel.SendMessage(message));
                    }
                    else
                    {
                        bj.adjustPlayerChips(3);
                        message = @"You tied \nYour Chips : " + bj.getChips()
                            + "\nYour Hand : " + displayHand(bj.getPlayerHand())
                            + "\tHand Value : " + bj.getCurrentHandValue(false);
                        await (e.Channel.SendMessage(message));
                    }

                    if (bj.isHandSplit())
                    {
                        if (bj.result() == 1)
                        {
                            bj.adjustPlayerChips(1);
                            message = @"You won! \nYour Chips : " + bj.getChips()
                                + "\nYour Second Hand: " + displayHand(bj.getSplitHand())
                                + "\tSplit Hand Value : " + bj.getCurrentHandValue(true);
                            await (e.Channel.SendMessage(message));
                        }
                        else if (bj.result() == 2)
                        {
                            bj.adjustPlayerChips(2);
                            message = @"You lost \nYour Chips : " + bj.getChips()
                                + "\nYour Second Hand: " + displayHand(bj.getSplitHand())
                                + "\tSplit Hand Value : " + bj.getCurrentHandValue(true);
                            await (e.Channel.SendMessage(message));
                        }
                        else
                        {
                            bj.adjustPlayerChips(3);
                            message = @"You tied \nYour Chips : " + bj.getChips()
                                + "\nYour Second Hand: " + displayHand(bj.getSplitHand())
                                + "\tSplit Hand Value : " + bj.getCurrentHandValue(true);
                            await (e.Channel.SendMessage(message));
                        }
                    }
                    else
                    {
                        message = "Dealer Hand : " + displayHand(bj.getCompHand())
                            + "\tComp Hand Value : " + getCurrentHandValue(bj.getCompHand())
                            + "\nWould you like me to .deal again? or .quit?";
                        await (e.Channel.SendMessage(message));
                    }
                });

            commands.CreateCommand("deal")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    bj.determinePlay("deal");
                    await (e.Channel.SendMessage("\nYour Chips : " + bj.getChips() + "\nHow much will you bet .min ( 5 ) or .max ( 100 )?"));
                });

            commands.CreateCommand("quit")
                .Parameter("user", ParameterType.Unparsed)
                .Do(async (e) =>
                {
                    await (e.Channel.SendMessage("Thanks for playing!"));
                    bj = new BlackJack();
                });

            // Call this last
            client.ExecuteAndWait(async () =>
            {
                await (client.Connect("MzA3OTAzNDYwMjgyNDY2MzA0.C-ZFSQ.weJRxtNtOjTnzbIDCXZfD6wKXyk", TokenType.Bot));
            });
        }

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
