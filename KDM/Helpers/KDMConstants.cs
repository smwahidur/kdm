using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace KDM.Helpers
{
    public class KDMBonusConstants
    {
        #region BONUS CONSTANTS
        public static string Wallet = "WALLET";
        public static string Sponsor = "SPONSOR";
        public static string BinaryMatching = "BINARY_MATCHING";
        public static string Generation = "GENERATION";
        public static string MonthlyRoyalty = "MONTHLY_ROYALTY";
        public static string Performance = "PERFORMANCE";
        public static string Leadership = "LEADERSHIP";
        public static string Rank = "RANK";
        public static string RoyalClub = "ROYAL_CLUB";
        public static string ECommerce = "E_COMMERCE";
        public static string Dealer = "DEALER";
        public static string Stockist = "STOCKIST";
        #endregion
    }

    public class KDMFundFrom
    {
        public static string Member { get; set; } = "MEMBER";
    }

    public class KDMBonusUnitConstants
    {
        public static string WalletBonus = "WB";
        public static string Taka = "TAKA";
    }

    public class KDMEnvironmentConstants
    {
        public static string ApplicationPath = AppDomain.CurrentDomain.BaseDirectory;
        public static string DefaultUserPassword { get; set; } = "KDM@%_2020";
        public static string DefaultUserRole { get; set; } = "KDM_MEMBER";
        public static string KDMMemberRole { get; set; } = "KDM_MEMBER";
        public static int SessionTimeout { get; set; } = 10;

    }

    public class KDMOrderStatus
    {
        public static string Approved { get; set; } = "Approved";
        public static string Pending { get; set; } = "Pending";
        public static string Cancled { get; set; } = "Canceled";
    }

    public class KDMOptions
    {
        public static Dictionary<string, string> PaymentMethods = new Dictionary<string, string>()
        {
            {"MP","bKash/Rocket" },
            {"COD","Cash on Delivery" },
            {"WP","Wallet Payment" }
        };
    }
}