using System.Collections.Generic;
using LeagueHistory.Core.JsonObjects;

namespace LeagueHistory.Models
{
    public class LookupViewModel
    {
        public List<LookupResponse> Responses
        {
            get;
            set;
        } = new List<LookupResponse>();
    }
}