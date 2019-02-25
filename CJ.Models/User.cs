using CJ.Models.Entities;

namespace CJ.Models
{
    public class User : IEntity
    {
        public virtual int Id { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
    }
}
