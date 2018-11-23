using System;
using System.Text;

namespace AzureSearch.Infrastructure.Models
{
    public partial class People
    {
		// This implementation of ToString() is only for the purposes of the sample console application.
        // You can override ToString() in your own model class if you want, but you don't need to in order
        // to use the Azure Search .NET SDK.
        public override string ToString()
        {
            var builder = new StringBuilder();

            if (!String.IsNullOrEmpty(Id))
            {
                builder.AppendFormat("ID: {0}\t", Id);
            }

            if (!String.IsNullOrEmpty(FirstName))
            {
                builder.AppendFormat("First name: {0}\t", FirstName);
            }

            if (!String.IsNullOrEmpty(LastName))
            {
                builder.AppendFormat("Last name: {0}\t", LastName);
            }

            if (!String.IsNullOrEmpty(Email))
            {
                builder.AppendFormat("Email: {0}\t", Email);
            }

            if (!String.IsNullOrEmpty(Gender))
            {
                builder.AppendFormat("Gender: {0}\t", Gender);
            }

            if (!String.IsNullOrEmpty(IpAddress))
            {
                builder.AppendFormat("IP address: {0}\t", IpAddress);
            }

            return builder.ToString();
        }
    }
}