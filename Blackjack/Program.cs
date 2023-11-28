// Lists for storing player's and dealer's cards
List<int> playerHand = new List<int>();
List<int> dealerHand = new List<int>();

// Value of player's and dealer's hand
int playerHandValue;
int dealerHandValue = 0;

// Values for testing game state conditions
bool dealerTurn = false;
bool stand = false;
bool blackjack = false;
bool bust = false;

// Starting money
int money = 200;

// Menu selector starting empty for menu loop
string selector = "";

Console.WriteLine("Welcome to Blackjack.\n");

// Game class variable with starting money
Game game = new Game(200);

Console.WriteLine($"You are given ${money} for free, use it wisely.");

// Loop until player trying to quit the game
while (selector.ToLower() != "q")
{
    // Printing out menu and taking palyer's input 
    Console.WriteLine();
    Console.WriteLine("What would you like to do:");
    Console.WriteLine("p - play");
    Console.WriteLine("m - money");
    Console.WriteLine("q - quit");
    Console.Write(": ");
    selector = Console.ReadLine();

    // Loop if the player's input is invalid
    while (selector.ToLower() != "q" && selector.ToLower() != "p" && selector.ToLower() != "m")
    {
        Console.WriteLine("\nPlease enter p, m or q.");
        Console.Write(": ");
        selector = Console.ReadLine();
    }

    // Check player's input and act appropriately
    switch (selector)
    {
        case "p":
            // Deal 2 cards each to player and dealer
            playerHand.Add(game.DealCard());
            dealerHand.Add(game.DealCard());
            playerHand.Add(game.DealCard());
            dealerHand.Add(game.DealCard());

            // Display the cards and calculate the value of player's hand
            game.DisplayCards(playerHand, dealerHand, dealerTurn);
            playerHandValue = game.CheckHand(playerHand);

            // Check if the player has 21 at this point for blackjack
            if (game.MaxHand(playerHandValue, dealerTurn))
            {
                // Switch turn
                dealerTurn = true;
                Console.WriteLine("Blackjack!!");

                // Display all cards and calculate the value of dealer's hand to see if can match blackjack
                // If not the player win if yes then it's a draw
                game.DisplayCards(playerHand, dealerHand, dealerTurn);
                dealerHandValue = game.CheckHand(dealerHand);
                if (dealerHandValue < 21)
                {
                    Console.WriteLine("You Win!!");
                }
                else
                {
                    Console.WriteLine("It's a push Blackjack to Blackjack!!");
                }
            }

            // If player doesn't have blackjack game proceed as normal
            else
            {
                // Loop if player choose to hit, or their hand value is less than 21
                while (!stand && !bust && !game.MaxHand(playerHandValue, dealerTurn))
                {
                    // Printing out options and taking player's input 
                    Console.WriteLine("What would you like to do:");
                    Console.WriteLine("h - hit");
                    Console.WriteLine("s - stand");
                    Console.Write(": ");
                    string gameSelector = Console.ReadLine();

                    // Loop if the palyer's input is invalid
                    while (gameSelector.ToLower() != "h" && gameSelector.ToLower() != "s")
                    {
                        Console.WriteLine("\nPlease enter h or s.");
                        Console.Write(": ");
                        gameSelector = Console.ReadLine();
                    }

                    // Check player's input and act appropriately
                    // If player hit then deal a card, re-display cards, and calculate new hand value
                    // If player stand then set stand to true for loop conditon
                    switch (gameSelector)
                    {
                        case "h":
                            playerHand.Add(game.DealCard());
                            game.DisplayCards(playerHand, dealerHand, dealerTurn);
                            playerHandValue = game.CheckHand(playerHand);
                            break;
                        case "s":
                            stand = true;
                            break;
                    }

                    // Check if the player bust
                    if (playerHandValue > 21)
                    {
                        Console.WriteLine("You bust.");
                        Console.WriteLine("You lose.");
                        bust = true;
                    }
                }

                // Check if the player went over 21 to see if the game should continue
                if (!bust)
                {
                    // Change the turn to dealer's
                    dealerTurn = true;

                    // Re-displaying the cards since one of the dealer's cards started off hidden
                    // Calculating the value of the dealer's hand
                    game.DisplayCards(playerHand, dealerHand, dealerTurn);
                    dealerHandValue = game.CheckHand(dealerHand);

                    // Check if the dealer got blackjack
                    // At this point if yes the dealer automatically win
                    if (dealerHandValue == 21)
                    {
                        blackjack = true;
                        Console.WriteLine("Dealer Blackjack.");
                        Console.WriteLine("You Lose.");
                    }

                    // If not then dealer continue to hit until their hand value is over 16
                    // Display the cards as healer draw them and assigning new hand value for dealer each time
                    else
                    {
                        // Check to see if dealer bust or go over 16
                        while (!bust && !game.MaxHand(dealerHandValue, dealerTurn))
                        {
                            dealerHand.Add(game.DealCard());
                            game.DisplayCards(playerHand, dealerHand, dealerTurn);
                            dealerHandValue = game.CheckHand(dealerHand);

                            // Check if the dealer bust
                            if (dealerHandValue > 21)
                            {
                                Console.WriteLine("Dealer bust.");
                                Console.WriteLine("You Win!!");
                                bust = true;
                            }
                        }
                    }
                }

                // Check the game state if both players are still playing and no one had blackjack prior
                // Conpare the hand values and display message saying who win the game or if it's a draw
                if (!bust && !blackjack)
                {
                    if (playerHandValue > dealerHandValue)
                    {
                        Console.WriteLine($"You win, {playerHandValue} to {dealerHandValue}!!");
                    }
                    else if (playerHandValue < dealerHandValue)
                    {
                        Console.WriteLine($"You lose, {dealerHandValue} to {playerHandValue}.");
                    }
                    else
                    {
                        Console.WriteLine($"It's a push, {dealerHandValue} to {playerHandValue}!");
                    }
                }
            }
            break;
        // Display how much money the player has left
        case "m":
            Console.WriteLine($"\nYou have ${game.Money} left.");
            break;
    }

    // Reset the hand arrays so both players has no cards
    // Shuffle deck so all cards can be drawn again
    playerHand.Clear();
    dealerHand.Clear();
    game.ShuffleDeck();

    // Reset conditions for the game state
    dealerTurn = false;
    stand = false;
    blackjack = false;
    bust = false;
}

// Class for all game's functions
class Game
{
    // For displaying money, created with instance creation
    private int _money;

    // Random variable for randoming card
    private Random _rand;

    // Constant array for holding card ranks and suits used for printing
    private readonly char[] _suits = new char[] { 's', 'h', 'd', 'c' };
    private readonly char[] _ranks = new char[] { 'a', '2', '3', '4', '5', '6', '7',
                                                  '8', '9', 't', 'j', 'q', 'k'};
    // To record which card has already been drawn (if not the value will be 0)
    private int[,] _cards = new int[4, 13];

    // Constructor that set money from parameter and initialise the rand variable on instance creation
    public Game(int money)
    {
        _money = money;
        _rand = new Random();
    }

    // Method for dealing card
    // Random the card from 0 to 51
    // Check if the value has already been randomised using the _cards array
    // If yes loop, if not then tell array the value is now drawn
    // Return the value + 1 since empty hand will be 0 in the array so lowest hand is 1
    public int DealCard()
    {
        int card = _rand.Next(52);
        while (_cards[card / 13, card % 13] > 0)
        {
            card = _rand.Next(52);
        }
        _cards[card / 13, card % 13]++;

        return card;
    }

    // Method to check hand value given player
    // Take in the hand list and current turn
    public int CheckHand(List<int> hand)
    {
        // Value for holding current hand value and amount of aces
        int handValue = 0;
        int aces = 0;

        // Check each card in the hand list and add appropriate value to the hand value
        // When card is an ace add 1 to aces and 11 to hand value to be subtracted later
        foreach (int card in hand)
        {
            if ((card % 13) > 9)
            {
                handValue += 10;
            }
            else if ((card % 13) == 0)
            {
                aces++;
                handValue += 11;
            }
            else
            {
                handValue += ((card % 13) + 1);
            }
        }

        // If hand value exceeded 21 while holding aces then subtracting 10
        // (So instead of aces being 11 it becomes 1)
        // Do this until hand is 21 or less
        while (aces > 0 && handValue > 21)
        {
            handValue -= 10;
            aces--;
        }

        // Return the final hand value
        return handValue;
    }

    // Method for displaying the cards in player's and dealer's hand
    // Take in the lists for both hands and the current turn
    public void DisplayCards(List<int> playerHand, List<int> dealerHand, bool dealerTurn)
    {
        // If it isn't the dealer's turn then display one dealer's card and hide the other
        // If it is the dealer's turn then display all the cards in the dealer's hand
        Console.WriteLine();
        if (dealerTurn)
        {
            Console.Write("Dealer's cards: ");
            foreach (int card in dealerHand)
            {
                Console.Write($"{_ranks[card % 13]}{_suits[card / 13]} ");
            }
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine($"Dealer's cards: {_ranks[dealerHand[0] % 13]}{_suits[dealerHand[0] / 13]} **");
        }

        // Display all the cards in the player's hand
        Console.WriteLine();
        Console.Write("Player's cards: ");
        foreach (int card in playerHand)
        {
            Console.Write($"{_ranks[card % 13]}{_suits[card / 13]} ");
        }
        Console.WriteLine("\n");
    }

    // Get property for money to be called when user select to look at how much money left
    public int Money
    {
        get
        {
            return _money;
        }
    }

    // Method for checking if player / dealer has reached the max amount to stop them hitting
    // For player the value is 21 and dealer value being 16
    public bool MaxHand(int handValue, bool dealerTurn) 
    {
        if ((handValue == 21 && !dealerTurn) || (handValue > 16 && dealerTurn))
        {
            return true;
        }
        return false;
    }

    // Method for resetting all the values in the _cards array to 0
    // This is so each round is played with all cards shuffled so everything can be drawn again
    public void ShuffleDeck()
    {
        for (int suit = 0; suit < _suits.Length; suit++)
        {
            for (int rank = 0; rank < _ranks.Length; rank++)
            {
                _cards[suit, rank] = 0;
            }
        }
    }
}