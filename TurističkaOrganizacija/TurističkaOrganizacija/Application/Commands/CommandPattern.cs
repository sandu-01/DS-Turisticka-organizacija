using System;
using TurističkaOrganizacija.Domain;

namespace TurističkaOrganizacija.Application.Commands
{
    /// <summary>
    /// Command pattern for encapsulating operations
    /// Allows for undo/redo functionality and operation queuing
    /// </summary>
    public interface ICommand
    {
        void Execute();
        void Undo();
        bool CanUndo { get; }
    }

    /// <summary>
    /// Command for adding a new client
    /// </summary>
    public class AddClientCommand : ICommand
    {
        private readonly Client _client;
        private readonly string _passportPlaintext;
        private readonly ClientService _clientService;
        private int _createdClientId;

        public AddClientCommand(Client client, string passportPlaintext, ClientService clientService)
        {
            _client = client;
            _passportPlaintext = passportPlaintext;
            _clientService = clientService;
        }

        public bool CanUndo => _createdClientId > 0;

        public void Execute()
        {
            _clientService.Create(_client, _passportPlaintext);
            _createdClientId = _client.Id;
        }

        public void Undo()
        {
            if (CanUndo)
            {
                _clientService.Delete(_createdClientId);
            }
        }
    }

    /// <summary>
    /// Command for updating a client
    /// </summary>
    public class UpdateClientCommand : ICommand
    {
        private readonly Client _originalClient;
        private readonly Client _updatedClient;
        private readonly string _passportPlaintext;
        private readonly ClientService _clientService;

        public UpdateClientCommand(Client originalClient, Client updatedClient, string passportPlaintext, ClientService clientService)
        {
            _originalClient = originalClient;
            _updatedClient = updatedClient;
            _passportPlaintext = passportPlaintext;
            _clientService = clientService;
        }

        public bool CanUndo => true;

        public void Execute()
        {
            _clientService.Update(_updatedClient, _passportPlaintext);
        }

        public void Undo()
        {
            _clientService.Update(_originalClient, _originalClient.PassportNumber.ToString());
        }
    }

    /// <summary>
    /// Command for deleting a client
    /// </summary>
    public class DeleteClientCommand : ICommand
    {
        private readonly Client _client;
        private readonly ClientService _clientService;

        public DeleteClientCommand(Client client, ClientService clientService)
        {
            _client = client;
            _clientService = clientService;
        }

        public bool CanUndo => true;

        public void Execute()
        {
            _clientService.Delete(_client.Id);
        }

        public void Undo()
        {
            // Note: This is simplified - in real scenario you'd need to store more data
            _clientService.Create(_client, _client.PassportNumber.ToString());
        }
    }

    /// <summary>
    /// Command for making a reservation
    /// </summary>
    public class MakeReservationCommand : ICommand
    {
        private readonly int _clientId;
        private readonly TravelPackage _package;
        private readonly int _passengerCount;
        private readonly decimal _totalPrice;
        private readonly ReservationService _reservationService;

        public MakeReservationCommand(int clientId, TravelPackage package, int passengerCount, decimal totalPrice, ReservationService reservationService)
        {
            _clientId = clientId;
            _package = package;
            _passengerCount = passengerCount;
            _totalPrice = totalPrice;
            _reservationService = reservationService;
        }

        public bool CanUndo => true;

        public void Execute()
        {
            _reservationService.Reserve(_clientId, _package, _passengerCount, _totalPrice);
        }

        public void Undo()
        {
            _reservationService.Cancel(_clientId, _package.Id);
        }
    }

    /// <summary>
    /// Command invoker - manages command execution and undo/redo
    /// </summary>
    public class CommandInvoker
    {
        private readonly System.Collections.Generic.Stack<ICommand> _undoStack = new System.Collections.Generic.Stack<ICommand>();
        private readonly System.Collections.Generic.Stack<ICommand> _redoStack = new System.Collections.Generic.Stack<ICommand>();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _undoStack.Push(command);
            _redoStack.Clear(); // Clear redo stack when new command is executed
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                var command = _undoStack.Pop();
                if (command.CanUndo)
                {
                    command.Undo();
                    _redoStack.Push(command);
                }
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _undoStack.Push(command);
            }
        }

        public bool CanUndo => _undoStack.Count > 0 && _undoStack.Peek().CanUndo;
        public bool CanRedo => _redoStack.Count > 0;
    }
}
