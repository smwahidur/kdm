using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KDM.Helpers
{
    public class KDMBonusSlabs
    {
        public static double WalletBonus { get; set; }
        public static Int64 PVForWalletBonus { get; set; }
        public static double SponsorBonus { get; set; } 
        public static Int64 PVForSponsorBonus { get; set; }
        public static double BinaryMatchingBonus { get; set; }
        public static Int64 PVForBinaryMatchingBonus { get; set; }
        public static float GenerationBonus { get; set; }
        public static Int64 GenerationBonusLayer { get; set; }
        public static double MonthlyRoyaltyBonus { get; set; }
        public static Int64 PVForMonthlyRoyaltyBonus { get; set; }
        public static float PerformanceBonusPercentage { get; set; }
        public static float LeadershipBonusPercentage { get; set; }
        public static double RankIncentiveBonus { get; set; }
        public static float RoyalClubBonusPercentage { get; set; }
        public static float ECommerceBonusPercentage { get; set; }
        public static double DealerCommission { get; set; }
        public static Int64 PVForDealerCommision { get; set; }
        public static double StockistCommission { get; set; }
        public static Int64 PVForStockistCommision { get; set; }
        
        
    }

    public class KDMSettings
    {
        static KDMDB db = new KDMDB();
        public static bool LoadSettings()
        {
            var settings = db.tbl_kdm_settings_master.FirstOrDefault();
            if(settings!=null)
            {

                KDMBonusSlabs.WalletBonus =(double) settings.WalletBonus;
                KDMBonusSlabs.PVForWalletBonus = (Int64)settings.PVForWalletBonus;
                KDMBonusSlabs.SponsorBonus = (double)settings.SponsorBonus;
                KDMBonusSlabs.PVForSponsorBonus = (Int64)settings.PVForSponsorBonus;
                KDMBonusSlabs.BinaryMatchingBonus = (double)settings.BinaryMatchingBonus;
                KDMBonusSlabs.PVForBinaryMatchingBonus = (Int64)settings.PVForBinaryMatchingBonus;
                KDMBonusSlabs.GenerationBonus = (float)settings.GenerationBonus;
                KDMBonusSlabs.GenerationBonusLayer = (Int64)settings.GenerationBonusLayer;
                KDMBonusSlabs.MonthlyRoyaltyBonus = (double)settings.MonthlyRoyaltyBonus;
                KDMBonusSlabs.PVForMonthlyRoyaltyBonus = (Int64)settings.PVForMonthlyRoyaltyBonus;
                KDMBonusSlabs.PerformanceBonusPercentage = (float)settings.PerformanceBonusPercentage;
                KDMBonusSlabs.LeadershipBonusPercentage = (float)settings.LeadershipBonusPercentage;
                KDMBonusSlabs.RankIncentiveBonus = (double)settings.RankIncentiveBonus;
                KDMBonusSlabs.RoyalClubBonusPercentage = (float)settings.RoyalClubBonusPercentage;
                KDMBonusSlabs.ECommerceBonusPercentage = (float)settings.ECommerceBonusPercentage;
                KDMBonusSlabs.DealerCommission = (float)settings.DealerCommission;
                KDMBonusSlabs.PVForDealerCommision = (Int64)settings.PVForDealerCommision;
                KDMBonusSlabs.StockistCommission = (double)settings.StockistCommission;
                KDMBonusSlabs.PVForStockistCommision = (Int64)settings.PVForStockistCommision;

                return true;
            }

            return false;
        }

        public static string GetNextMemberID(KDMDB trDB)
        {
            string nextID = "-1";
            var settings = trDB.tbl_kdm_settings_master.Where(x => x.ID == 1).FirstOrDefault();
            if(settings!=null)
            {
                var id = settings.NextMemberID;
                settings.NextMemberID = id + 1;
                nextID = "K"+id.ToString().PadLeft(6, '0');
            }

            return nextID;
        }
    }

    
}