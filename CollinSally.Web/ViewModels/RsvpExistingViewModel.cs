using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollinSally.Web.ViewModels
{
    public class RsvpExistingViewModel
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public string EmailAddress { get; set; }

        public bool Attending { get; set; }
    }
}
