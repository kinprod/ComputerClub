using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

internal class Program
{
    static void Main(string[] args)
    {
        ComputerClub computerClub = new ComputerClub(8);
        computerClub.Work();
    }
}

class ComputerClub
{
    private int _money = 0;
    private List<Computer> _computers = new List<Computer>();
    private Queue<Client> _clients = new Queue<Client>();

    public ComputerClub(int computerCount)
    {
        Random random = new Random();

        for (int i = 0; i < computerCount; i++)
        {
            _computers.Add(new Computer(random.Next(5, 15)));
        }

        CreateNewClients(25, random);
    }

    public void CreateNewClients(int count, Random random)
    {
        for (int i = 0; i < count; i++)
        {
            _clients.Enqueue(new Client(random.Next(100, 251), random));
        }
    }

    public void Work()
    {
        while (_clients.Count > 0)
        {
            Client newClient = _clients.Dequeue();
            Console.WriteLine($"Баланс компьютерного клуба {_money} руб. Ждем нового клиента");
            Console.WriteLine($"У вас новый клиетн, и он хочет купить {newClient.DesiredMinutes} минут.");

            ShowAllComputersState();

            Console.Write("\nВы предлагаете ему компьютер под номером: ");
            string userInput = Console.ReadLine();
            if (int.TryParse(userInput, out int computerNumber))
            {
                computerNumber--;

                if( computerNumber >= 0 && computerNumber < _computers.Count)
                {
                    if (_computers[computerNumber].IsTaken)
                    {
                        Console.WriteLine("Вы пытаетесь посадить клиента за занятый компьютер, он ушел.");
                    }
                    else
                    {
                        if (newClient.CheckSolvency(_computers[computerNumber]))
                        {
                            Console.WriteLine("Клиент сел за компьютер " + computerNumber + 1);
                            _computers[computerNumber].BecomeTaken(newClient);
                        }
                        else
                        {
                            Console.WriteLine("У клиента не достаточно денег, он ушел.");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Вы сами не знаете за какой компьютер посадить клиента, и он ушел.");
                }
            }
            else
            {
                CreateNewClients(1, new Random());
                Console.WriteLine("Неверный ввод! Попробуйте ещё раз.");
            }

            Console.WriteLine("Чтобы перейти в следующему клиенту нажмите любую клавишу.");
            Console.ReadKey();
            Console.Clear();
            SpendOneMinute();
        }
    }

    private void ShowAllComputersState()
    {
        Console.WriteLine("Информация о всех компьютерах:\n");
        for (int i = 0; i < _computers.Count; i++)
        {
            Console.Write(i + 1 + " - ");
            _computers[i].ShowState();
        }
    }

    private void SpendOneMinute()
    {
        foreach (var computer in _computers)
        {
            computer.SpendOneMinute();
        }
    }
}

class Computer
{
    private Client _client;
    private int _minetsRemaining;

    public bool IsTaken
    {
        get
        {
            return _minetsRemaining > 0;
        }
    }

    public int PricePerMinutes { get; private set; }

    public Computer(int pricePerMinutes)
    {
        PricePerMinutes = pricePerMinutes;
    }

    public void BecomeTaken(Client client)
    {
        _client = client;
        _minetsRemaining = client.DesiredMinutes;
    }

    public void BecomeEmpty()
    {
        _client = null;
    }

    public void SpendOneMinute()
    {
        _minetsRemaining--;
    }

    public void ShowState()
    {
        if (IsTaken)
            Console.WriteLine($"Компьютер занят, осталось минут: {_minetsRemaining}");
        else
            Console.WriteLine($"Компьютер свободен, цена за минуту: {PricePerMinutes}");
    }
}

class Client
{
    private int _money;
    private int _moneyToPay;

    public int DesiredMinutes { get; private set; }

    public Client(int money, Random random)
    {
        _money = money;
        DesiredMinutes = random.Next(10, 30);
    }

    public bool CheckSolvency(Computer computer)
    {
        _moneyToPay = DesiredMinutes * computer.PricePerMinutes;
        
        if (_money >= _moneyToPay)
        {
            return true;
        }
        else
        {
            _moneyToPay = 0;
            return false;
        }
    }

    public int Pay()
    {
        _money -= _moneyToPay;
        return _moneyToPay;
    }
}