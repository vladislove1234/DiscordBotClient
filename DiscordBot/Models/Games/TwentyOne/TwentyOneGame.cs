using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using DiscordBot.Models.Games.Abstractions;
using DiscordBot.Models.Games.Enums;
using Game = DiscordBot.Models.Games.Abstractions.Game; 

namespace DiscordBot.Models.Games.TwentyOne
{
    public class TwentyOneGame : Game
    {
        public IUserMessage MainMessage { get; private set; }
        public ITextChannel MainChannel { get; private set; }
        private Player _startUser;
        private Player _joinedUser;
        private Player _currentMoveUser;
        public Deck GameDeck;
        public TwentyOneGame(DiscordSocketClient client, ITextChannel channel, IUser startUser) : base(client, channel, startUser)
        {
            _startUser = new Player(startUser);
            MainChannel = channel;
            State = Enums.GameState.WaitForPlayers;
            MainChannel.SendMessageAsync($"{_startUser.User.Username} wants to play 21! Type join to enter!");
            GameDeck = new Deck();
        }
        protected override async Task OnMessageRecieved(SocketMessage arg)
        {
            if (State == Enums.GameState.WaitForPlayers)
            {
                if (arg.Author.Username != _startUser.User.Username && arg.Content.ToLower() == "join")
                {
                    _joinedUser = new Player(arg.Author);
                    await MainChannel.SendMessageAsync($"{_joinedUser.User.Username} joined the game. Good Luck!");
                    StartGame();
                    await UpdateMessage();
                    await arg.DeleteAsync();
                }
            }
            else
            {
                if (arg.Author.Username == _currentMoveUser.User.Username)
                {
                    switch (arg.Content.ToLower())
                    {
                        case "take":
                            _currentMoveUser.AddCard(GameDeck.TakeFirst());
                            Task.Run(async () => await UpdateMessage());
                            CheckScores();
                            break;
                        case "stop":
                            Task.Run(async () => await UpdateMessage());
                            ChangeMove();
                            break;
                        default:
                            await MainChannel.SendMessageAsync("Wrong command");
                            break;
                    }
                    await arg.DeleteAsync();
                }
            }
        }
        private void ChangeMove()
        {
            if (_startUser == _currentMoveUser)
            {
                _startUser.Stop();
                _currentMoveUser = _joinedUser;
            }
            else if (_currentMoveUser == _joinedUser)
            {
                _joinedUser.Stop();
                State = Enums.GameState.End;
                CheckScores();
            }
        }
        private void StartGame()
        {
            _currentMoveUser = _startUser;
            _currentMoveUser.AddCards(GameDeck.TakeCards(2));
            _joinedUser.AddCards(GameDeck.TakeCards(2));
            State = Enums.GameState.Play;
            CheckScores();
        }
        private void CheckScores()
        {
            if (_currentMoveUser.Score > 21)
            {
                _currentMoveUser.Stop();
                ChangeMove();
            }
            //else if (_startUser.Score > 21 && _joinedUser.Score > 21)
            //  EndGame("Tie! Both of players have more than 21 points");
            else if (_startUser.Score == 21 && _joinedUser.Score == 21)
                EndGame("Tie! Both of players have 21 points");
            else if (_startUser.Score == 21)
                EndGame(_startUser, $"{_startUser.User.Username} has a blackjack!");
            else if (_joinedUser.Score == 21)
                EndGame(_joinedUser, $"{_joinedUser.User.Username} has a blackjack!");
            else if (_joinedUser.Stopped && _startUser.Stopped)
            {
                if (_startUser.Score > 21)
                        EndGame(_joinedUser, $"{ _startUser.User.Username} has more than 21 points.");
                    else if (_joinedUser.Score > 21)
                        EndGame(_startUser, $"{_joinedUser.User.Username} has more than 21 points.");
                    else if (_startUser.Score == _joinedUser.Score)
                        EndGame("Tie! Both of players have same points");
                    else if (_startUser.Score > _joinedUser.Score)
                        EndGame(_startUser, $"{_startUser.User.Username} has more points ({_startUser.Score})!");
                    else EndGame(_joinedUser, $"{_joinedUser.User.Username} has more points ({_joinedUser.Score})!");
            }
        }
        private void EndGame(Player winner, string message)
        {
            Task.Run(async () => await MainChannel.SendMessageAsync($"The winner is {winner.User.Username} with {winner.Score} points \n" + message));
            Task.Run(async () => await MainMessage.DeleteAsync());
            State = Enums.GameState.End;
        }
        private void EndGame(string message)
        {
            Task.Run(async() => await MainChannel.SendMessageAsync(message));
            Task.Run(async () => await MainMessage.DeleteAsync());
            State = GameState.End;
        }
        private async Task UpdateMessage()
        {
            if (MainMessage == null)
            {
                MainMessage = (IUserMessage)await MainChannel.SendMessageAsync(GenerateMeesage());
            }
            else
            {
                await MainMessage.ModifyAsync(x => x.Content = GenerateMeesage());
            }
        }
        private string GenerateMeesage()
        {
            string message = string.Empty;
            message += "Players: \n";
            message += $"1. {_startUser.User.Username}  Cards: ";
            foreach (var card in _startUser.Cards)
            {
                message += " :black_joker: ";
            }
            message += "\n";
            message += $"2. {_joinedUser.User.Username}  Cards: ";
            foreach (var card in _joinedUser.Cards)
            {
                message += " :black_joker: ";
            }
            message += "\n";
            message += $"Current move of {_currentMoveUser.User.Username}";
            return message;
        }
    }
}
