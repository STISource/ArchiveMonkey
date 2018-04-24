﻿using System;
using System.Collections.Generic;
using DvApi32;

namespace ArchiveMonkey.Services
{
    public class DavidEmailFilter : IFilter
    {
        private EmailAttribute attribute;
        private EmailAttributeComparer comparer;
        private string value;

        public DavidEmailFilter(string filter)
        {
            if (!IsValidFilter(filter))
            {
                throw new Exception(string.Format("Invalid filter provided '{0}'", filter));
            }

            var elements = filter.Split(' ');

            this.attribute = (EmailAttribute)Enum.Parse(typeof(EmailAttribute), elements[0]);
            this.comparer = (EmailAttributeComparer)Enum.Parse(typeof(EmailAttributeComparer), elements[1]);
            this.value = elements[2].ToLower();
        }       

        public bool FilterApplies(object itemToArchive)
        {
            bool applies = false;
            var mailItem = (MailItem)itemToArchive;

            switch(this.attribute)
            {
                case EmailAttribute.RecipientList:
                    var recipients = new List<string>();
                    for(int i = 0; i < mailItem.Recipients.Count; i++)
                    {
                        recipients.Add(mailItem.Recipients.Item(i).EMail.ToLower());
                    }

                    for (int i = 0; i < mailItem.CC.Count; i++)
                    {
                        recipients.Add(mailItem.CC.Item(i).EMail.ToLower());
                    }

                    applies = this.comparer == EmailAttributeComparer.Contains ? recipients.Contains(this.value) : !recipients.Contains(this.value);
                    break;

                case EmailAttribute.Sender:

                    applies = this.comparer == EmailAttributeComparer.Is ? mailItem.From.EMail.ToLower() == this.value : mailItem.From.EMail.ToLower() != this.value;
                    break;
            }

            return applies;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", this.attribute.ToString(), this.comparer.ToString(), this.value);
        }

        public static bool IsValidFilter(string filter)
        {
            var elements = filter.Split(' ');
            EmailAttribute filterAttribute = EmailAttribute.RecipientList;
            EmailAttributeComparer filterComparer = EmailAttributeComparer.Contains;

            bool parsable = elements.Length == 3
                    && Enum.TryParse<EmailAttribute>(elements[0], out filterAttribute)
                    && Enum.TryParse<EmailAttributeComparer>(elements[1], out filterComparer);

            if(!parsable)
            {
                return false;
            }

            return (filterAttribute == EmailAttribute.RecipientList && (filterComparer == EmailAttributeComparer.Contains || filterComparer == EmailAttributeComparer.ContainsNot))
                || (filterAttribute == EmailAttribute.Sender && (filterComparer == EmailAttributeComparer.Is || filterComparer == EmailAttributeComparer.IsNot));
        }

        private enum EmailAttribute
        {
            RecipientList = 1,
            Sender = 2
        }

        private enum EmailAttributeComparer
        {
            Contains = 1,
            ContainsNot = 2,
            Is = 3,
            IsNot = 4
        }
    }
}
