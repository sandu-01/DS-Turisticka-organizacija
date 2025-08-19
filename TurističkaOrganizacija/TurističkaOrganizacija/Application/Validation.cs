using System;
using System.Text.RegularExpressions;
using TurističkaOrganizacija.Domain;

namespace TurističkaOrganizacija.Application
{
    public interface IValidationRule
    {
        IValidationRule SetNext(IValidationRule next);
        void Validate(Client client);
    }

    public abstract class ValidationRuleBase : IValidationRule
    {
        private IValidationRule _next;
        public IValidationRule SetNext(IValidationRule next)
        {
            _next = next; return next;
        }
        public void Validate(Client client)
        {
            DoValidate(client);
            _next?.Validate(client);
        }
        protected abstract void DoValidate(Client client);
    }

    public class RequiredFieldsRule : ValidationRuleBase
    {
        protected override void DoValidate(Client client)
        {
            if (string.IsNullOrWhiteSpace(client.FirstName) || string.IsNullOrWhiteSpace(client.LastName))
                throw new ArgumentException("Ime i prezime su obavezni.");
        }
    }

    public class EmailFormatRule : ValidationRuleBase
    {
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
        protected override void DoValidate(Client client)
        {
            if (string.IsNullOrWhiteSpace(client.Email) || !EmailRegex.IsMatch(client.Email))
                throw new ArgumentException("Email nije ispravan.");
        }
    }

    public class UniquePassportRule : ValidationRuleBase
    {
        private readonly IClientRepository _repo;
        private readonly int? _excludeId;
        public UniquePassportRule(IClientRepository repo, int? excludeId)
        {
            _repo = repo; _excludeId = excludeId;
        }
        protected override void DoValidate(Client client)
        {
            if (client.PassportNumber <= 0) throw new ArgumentException("Pasoš nije ispravan.");
            if (_repo.ExistsPassport(client.PassportNumber, _excludeId))
                throw new ArgumentException("Klijent sa istim brojem pasoša već postoji.");
        }
    }
}


