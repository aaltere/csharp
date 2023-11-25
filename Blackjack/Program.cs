int[] playerCards = new int[8];
int[] dealerCards = new int[8];
bool dealerTurn = false;
bool stand = false;
int money = 200;

int currentPlayerCard = 0;
int currentDealerCard = 0;

Console.WriteLine("Welcome to Blackjack.\n");

Game game = new Game(200);

Console.WriteLine($"You are given ${money} for free, use it wisely.\n");

Console.WriteLine("What would you like to do:");
Console.WriteLine("p - play");
Console.WriteLine("m - money");
Console.WriteLine("q - quit");
Console.Write(": ");
string selector = Console.ReadLine();

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
        game.CheckHand(playerCards);

        while (!stand && !game.IsGameOver() && !game.Is21())
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
                    game.CheckHand(playerCards);
                    break;
                case "s":
                    stand = true;
                    break;
            }
        }
        break;
    case "m":
        game.DisplayMoney();
        break;
}

class Game
{
    private int _money;

    private bool _gameOver = false;
    private bool _is21 = false;

    private Random _rand = new Random();

    private readonly char[] _suits = new char[] { 's', 'h', 'd', 'c' };
    private readonly char[] _ranks = new char[] { 'a', '2', '3', '4', '5', '6', '7',
                                                  '8', '9', 't', 'j', 'q', 'k'};
    private int[,] _cards = new int[4, 13];

    public Game(int money)
    {
        _money = money;
    }

    public int DealCard()
    {
        int card = _rand.Next(52);
        if (_cards[card / 13, card % 13] > 0)
        {
            card = _rand.Next(52);
        }
        _cards[card / 13, card % 13]++;

        return card + 1;
    }

    public void CheckHand(int[] hand)
    {
        int handValue = 0;
        int aces = 0;

        foreach (int card in hand)
        {
            if (card > 0)
            {
                if (((card - 1) % 13) + 1 > 10)
                {
                    handValue += 10;
                }
                else if (((card - 1) % 13) == 0)
                {
                    aces++;
                    if ((handValue + 11) > 21)
                    {
                        handValue++;
                    }
                    else
                    {
                        handValue += 11;
                    }
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
            _gameOver = true;
        }
        else if (handValue == 21)
        {
            _is21 = true;
        }

        Console.WriteLine(handValue + "\n");
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

    public void DisplayMoney()
    {
        Console.WriteLine($"\nYou have ${_money} left.");
    }

    public bool IsGameOver() { return _gameOver; }
    public bool Is21() { return _is21;  }

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