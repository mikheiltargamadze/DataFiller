using Entities.Models;
using ViewModels.AutoMapepr;

namespace Infrastructure.BackgroundWorks
{
    public class Person : BaseDto<Person, PersonEntity>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
}
