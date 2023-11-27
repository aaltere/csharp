int[] playerCards = new int[11];
int[] dealerCards = new int[11];

bool dealerTurn = false;
bool stand = false;
bool blackjack = false;

int money = 200;

int currentPlayerCard = 0;
int currentDealerCard = 0;
int playerHandValue;
int dealerHandValue = 0;

string selector = "";

Console.WriteLine("Welcome to Blackjack.\n");

Game game = new Game(200);

Console.WriteLine($"You are given ${money} for free, use it wisely.");

while (selector.ToLower() != "q")
{
    Console.WriteLine();
    Console.WriteLine("What would you like to do:");
    Console.WriteLine("p - play");
    Console.WriteLine("m - money");
    Console.WriteLine("q - quit");
    Console.Write(": ");
    selector = Console.ReadLine();

    while (selector.ToLower() != "q" && selector.ToLower() != "p" && selector.ToLower() != "m")
    {
        Console.WriteLine("\nPlease enter p, m or q.");
        Console.Write(": ");
        selector = Console.ReadLine();
    }

    switch (selector)
    {
        case "p":
            game.ShuffleDeck();
            playerCards[currentPlayerCard++] = game.DealCard();
            playerCards[currentPlayerCard++] = game.DealCard();
            dealerCards[currentDealerCard++] = game.DealCard();
            dealerCards[currentDealerCard++] = game.DealCard();
            game.DisplayCards(playerCards, dealerCards, dealerTurn);
            playerHandValue = game.CheckHand(playerCards, dealerTurn);
            if (game.MaxHand(playerHandValue, dealerTurn))
            {
                dealerTurn = true;
                blackjack = true;
                Console.WriteLine("Blackjack!!");
                game.DisplayCards(playerCards, dealerCards, dealerTurn);
                dealerHandValue = game.CheckHand(dealerCards, dealerTurn);
                if (dealerHandValue < 21)
                {
                    Console.WriteLine("You Win!!");
                }
                else
                {
                    Console.WriteLine("It's a push Blackjack to Blackjack!!");
                }
            }
            else
            {

                while (!stand && !game.Busted && !game.MaxHand(playerHandValue, dealerTurn))
                {
                    Console.WriteLine("What would you like to do:");
                    Console.WriteLine("h - hit");
                    Console.WriteLine("s - stand");
                    Console.Write(": ");
                    string gameSelector = Console.ReadLine();
                    while (gameSelector.ToLower() != "h" && gameSelector.ToLower() != "s")
                    {
                        Console.WriteLine("\nPlease enter h or s.");
                        Console.Write(": ");
                        gameSelector = Console.ReadLine();
                    }
                    switch (gameSelector)
                    {
                        case "h":
                            playerCards[currentPlayerCard++] = game.DealCard();
                            game.DisplayCards(playerCards, dealerCards, dealerTurn);
                            playerHandValue = game.CheckHand(playerCards, dealerTurn);
                            break;
                        case "s":
                            stand = true;
                            break;
                    }
                }
                if (!game.Busted)
                {
                    dealerTurn = true;

                    game.DisplayCards(playerCards, dealerCards, dealerTurn);
                    dealerHandValue = game.CheckHand(dealerCards, dealerTurn);
                    if (dealerHandValue == 21)
                    {
                        blackjack = true;
                        Console.WriteLine("Dealer Blackjack.");
                        Console.WriteLine("You Lose.");
                    }
                    else
                    {
                        while (!game.Busted && !game.MaxHand(dealerHandValue, dealerTurn))
                        {
                            dealerCards[currentDealerCard++] = game.DealCard();
                            game.DisplayCards(playerCards, dealerCards, dealerTurn);
                            dealerHandValue = game.CheckHand(dealerCards, dealerTurn);
                        }
                    }
                }

                if (!game.Busted && !blackjack)
                {
                    if (playerHandValue > dealerHandValue)
                    {
                        Console.WriteLine($"You Win {playerHandValue} to {dealerHandValue}!!");
                    }
                    else if (playerHandValue < dealerHandValue)
                    {
                        Console.WriteLine($"You Lose {dealerHandValue} to {playerHandValue}.");
                    }
                    else
                    {
                        Console.WriteLine($"It's a push {dealerHandValue} to {playerHandValue}!");
                    }
                }
            }
            break;
        case "m":
            Console.WriteLine($"\nYou have ${game.Money} left.");
            break;
    }

    for (int card = 0; card < 11;  card++)
    {
        playerCards[card] = 0;
        dealerCards[card] = 0;
    }

    currentPlayerCard = 0;
    currentDealerCard = 0;

    dealerTurn = false;
    stand = false;
}

class Game
{
    private int _money;

    private Random _rand;

    private readonly char[] _suits = new char[] { 's', 'h', 'd', 'c' };
    private readonly char[] _ranks = new char[] { 'a', '2', '3', '4', '5', '6', '7',
                                                  '8', '9', 't', 'j', 'q', 'k'};
    private int[,] _cards = new int[4, 13];

    public Game(int money)
    {
        _money = money;
        _rand = new Random();
    }

    public int DealCard()
    {
        int card = _rand.Next(52);
        while (_cards[card / 13, card % 13] > 0)
        {
            card = _rand.Next(52);
        }
        _cards[card / 13, card % 13]++;

        return card + 1;
    }

    public int CheckHand(int[] hand, bool dealerTurn)
    {
        int handValue = 0;
        int aces = 0;

        Busted = false;

        foreach (int card in hand)
        {
            if (card > 0)
            {
                if (((card - 1) % 13) > 9)
                {
                    handValue += 10;
                }
                else if (((card - 1) % 13) == 0)
                {
                    aces++;
                    handValue += 11;
                }
                else
                {
                    handValue += (((card - 1) % 13) + 1);
                }
            }
        }

        while (aces > 0 && handValue > 21)
        {
            handValue -= 10;
            aces--;
        }

        if (handValue > 21)
        {
            if (dealerTurn)
            {
                Console.WriteLine("Dealer bust.");
                Console.WriteLine("You Win!!");
            }
            else
            {
                Console.WriteLine("You bust.");
                Console.WriteLine("You lose.");
            }
            Busted = true;

            return 0;
        }

        return handValue;
    }

    public void DisplayCards(int[] playerCards, int[] dealerCards, bool dealerTurn)
    {
        Console.WriteLine();
        if (dealerTurn)
        {
            Console.Write("Dealer's cards: ");
            foreach (int dealerCard in dealerCards)
            {
                if (dealerCard != 0)
                { 
                    Console.Write($"{_ranks[(dealerCard - 1) % 13]}{_suits[(dealerCard - 1) / 13]} ");
                }
            }
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine($"Dealer's cards: {_ranks[(dealerCards[0] - 1) % 13]}{_suits[(dealerCards[0] - 1) / 13]} **");
        }

        Console.WriteLine();
        Console.Write("Player's cards: ");
        foreach (int playerCard in playerCards)
        {
            if (playerCard != 0)
            {
                Console.Write($"{_ranks[(playerCard - 1) % 13]}{_suits[(playerCard - 1) / 13]} ");
            }
        }
        Console.WriteLine("\n");
    }

    public int Money
    {
        get
        {
            return _money;
        }
    }

    public bool Busted { private set; get; }

    public bool MaxHand(int handValue, bool dealerTurn) 
    {
        if ((handValue == 21 && !dealerTurn) || (handValue > 16 && dealerTurn))
        {
            return true;
        }
        return false;
    }

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