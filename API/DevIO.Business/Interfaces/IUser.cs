using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace DevIO.Business.Interfaces
{
    public interface IUser
    {
        string Name { get; }
        public Guid GetUserId();
        string GetUserEmail();
        bool IsAuthenticated();
        bool IsInRole(string role);
        IEnumerable<Claim> GetClaimsIdentity();
    }
}
