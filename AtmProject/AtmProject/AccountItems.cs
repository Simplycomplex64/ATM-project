using System;
using System.Collections;

namespace AtmProject
{
    internal class AccountItems
    {
        
        public int account_id;
        private long account_balance;
        private string account_type;
        private Boolean account_enabled;
        private int overdraft;

        public AccountItems(int account_id, long account_balance, string account_type, bool account_enabled, int overdraft)
        {
            this.account_id = account_id;
            this.account_balance = account_balance;
            this.account_type = account_type;
            this.account_enabled = account_enabled;
            this.overdraft = overdraft;
        }
    }
}