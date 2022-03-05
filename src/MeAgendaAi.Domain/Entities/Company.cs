using MeAgendaAi.Domains.Validators;
using MeAgendaAi.Domains.ValueObjects;

namespace MeAgendaAi.Domains.Entities
{
    /// <summary>
    /// Represent <c>Company</c> class.
    /// </summary>
    public class Company : User
    {
        /// <summary>
        /// Represents of Company name.
        /// </summary>
        public NameObject Name { get; set; } = default!;

        /// <summary>
        /// Represents the Company's National Register of Legal Entities
        /// </summary>
        public string CNPJ { get; protected set; } = default!;

        /// <summary>
        /// Represents the Company Description.
        /// </summary>
        public string Description { get; protected set; } = default!;

        /// <summary>
        /// Represents of Limit Cancellation Hours.
        /// </summary>
        public int LimitCancelHours { get; protected set; } = default!;

        protected Company()
        {
        }

        /// <summary>
        /// Build <c>Company</c> object.
        /// </summary>
        /// <param name="email">Email of company.</param>
        /// <param name="password">Password of company.</param>
        /// <param name="name">Name of company</param>
        /// <param name="cnpj">National Register of Legal Entities of company.</param>
        /// <param name="description">Description of company.</param>
        /// <param name="limitCancelHours">limit Cancel Hours of company.</param>
        public Company(string email, string password, string name, string cnpj, string description, int limitCancelHours) : base(email, password)
        {
            Name = new NameObject(name);
            CNPJ = cnpj;
            Description = description;
            LimitCancelHours = limitCancelHours;

            Validate();
        }

        /// <summary>
        /// Validate the Company class.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if a Company class is valid.
        ///     <c>false</c> if a Company class is invalid.
        /// </returns>
        public new bool Validate() => Validate(this, new CompanyValidator());
    }
}