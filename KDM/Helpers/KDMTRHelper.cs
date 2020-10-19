using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Serilog;

namespace KDM.Helpers
{
    public class KDMTRHelper
    {
        KDMDB db = new KDMDB();

        // METHOD ID: M_0000001
        public bool GiveWalletBonus(string MemberID)
        {
            bool ret = false;
            double walletBonusAmount = 0;
            double walletBonusNewAmount = 0;
            double walletBonusAmountLeft = 0;
            int totalNoOfPV = 0;
            Int64 totalPVUsed = 0;

            using (var tr = db.Database.BeginTransaction())
            {
                try
                {
                    var memberBonusMaster = db.tbl_member_bonus_master.Where(x => x.MemberID == MemberID).FirstOrDefault();
                    var memberBonusTemp = db.tbl_member_bonus_temp.Where(x => x.MemberID == MemberID).FirstOrDefault();


                    if(memberBonusMaster==null)
                    {
                        memberBonusMaster = new tbl_member_bonus_master() { MemberID = MemberID };
                        db.tbl_member_bonus_master.Add(memberBonusMaster);
                        db.SaveChanges();
                    }
                    
                    if (memberBonusMaster != null && memberBonusTemp != null)
                    {
                        var currentTempWalletBonus = memberBonusTemp.WalleBonusBalance;

                        if (currentTempWalletBonus >= KDMBonusSlabs.PVForWalletBonus)
                        {
                            totalNoOfPV = (int)(currentTempWalletBonus / KDMBonusSlabs.PVForWalletBonus);
                            totalPVUsed = totalNoOfPV * KDMBonusSlabs.PVForWalletBonus;
                            walletBonusAmount = totalNoOfPV * KDMBonusSlabs.WalletBonus;
                            walletBonusNewAmount = (double)memberBonusMaster.WalletBonus + walletBonusAmount;
                            walletBonusAmountLeft = (double)(currentTempWalletBonus % KDMBonusSlabs.PVForWalletBonus);

                            #region Member Bonus History Create
                            tbl_member_bonus_history tbl = new tbl_member_bonus_history();
                            tbl.MemberID = MemberID;
                            tbl.BonusSource = KDMBonusConstants.Wallet;
                            tbl.BonusAmount = Convert.ToDecimal(walletBonusAmount);
                            tbl.BonusTotalAmount = Convert.ToDecimal(walletBonusNewAmount);
                            tbl.BonusUnitAmount = Convert.ToDecimal(KDMBonusSlabs.WalletBonus);
                            tbl.PVTotal = totalPVUsed;
                            tbl.PVUnit = KDMBonusSlabs.PVForWalletBonus;
                            tbl.BonusUnit = KDMBonusUnitConstants.WalletBonus;
                            tbl.BonusDate = DateTime.Now;
                            db.tbl_member_bonus_history.Add(tbl);
                            #endregion

                            memberBonusTemp.WalleBonusBalance = Convert.ToDecimal(walletBonusAmountLeft);

                            memberBonusMaster.WalletBonus = Convert.ToDecimal(walletBonusNewAmount);

                            db.SaveChanges();
                            tr.Commit();

                            ret = true;
                        }
                    }
                }
                catch(Exception ex)
                {
                    Log.Error(ex, "[M_0000001] [ERROR]");
                    tr.Rollback();
                }
                
            }

            return ret;
        }


        // METHOD ID: M_0000002
        public bool GiveSponsorBonus(KDMDB kdmDB, string MemberID)
        {
            bool ret = false;
            double sponsorBonusAmount = 0;
            double sponsorBonusPVLeft = 0;
            int totalNoOfPV = 0;
            Int64 totalPVUsed = 0;

            //using (var tr = kdmDB.Database.BeginTransaction())
            //{
                try
                {
                    var sponsorID = kdmDB.tbl_members.Where(x => x.PlacementID == MemberID).Select(x => x.SponsorID).FirstOrDefault();

                sponsorID = sponsorID.Trim();

                    if(sponsorID==null)
                    {
                        Log.Warning("[M_0000002] Sponsor ID not found for member id "+MemberID);
                        return false;
                    }
                     
                    var sponsorBonusMaster = kdmDB.tbl_member_bonus_master.Where(x => x.MemberID == sponsorID).FirstOrDefault();
                    var memberBonusTemp = kdmDB.tbl_member_bonus_temp.Where(x => x.MemberID == MemberID).FirstOrDefault();
                    
                    if (sponsorBonusMaster == null)
                    {
                        sponsorBonusMaster = new tbl_member_bonus_master() { 
                            MemberID = sponsorID,
                            WalletBonus=0,
                            SponsorBonus=0,
                            BinaryMatchingBonus=0,
                            GenerationBonus=0,
                            MonthlyRoyalityBonus=0,
                            PerformanceBonus=0,
                            LeadershipBonus=0,
                            RankIncentive=0,
                            RoyalClubBonus=0,
                            ECommerceBonus=0,
                            UpdateDate=DateTime.Now
                        };
                        kdmDB.tbl_member_bonus_master.Add(sponsorBonusMaster);
                        kdmDB.SaveChanges();
                    }

                    if (sponsorBonusMaster != null && memberBonusTemp != null)
                    {
                        var currentTempSponsorBonus = memberBonusTemp.SponsorBonusBalance;

                        if (currentTempSponsorBonus >= KDMBonusSlabs.PVForSponsorBonus)
                        {
                            totalNoOfPV = (int)(currentTempSponsorBonus / KDMBonusSlabs.PVForSponsorBonus);
                            totalPVUsed = totalNoOfPV * KDMBonusSlabs.PVForSponsorBonus;
                            sponsorBonusAmount = totalNoOfPV * KDMBonusSlabs.SponsorBonus;
                            sponsorBonusPVLeft = (double)(currentTempSponsorBonus-totalPVUsed);

                            #region Member Bonus History Create
                            tbl_member_bonus_history tbl = new tbl_member_bonus_history();
                            tbl.MemberID = sponsorID.Trim();
                            tbl.FromMemberID = MemberID.Trim();
                            tbl.BonusSource = KDMBonusConstants.Sponsor;
                        tbl.PreviousAmount = sponsorBonusMaster.SponsorBonus;
                            tbl.BonusTotalAmount = Convert.ToDecimal(sponsorBonusAmount);
                            tbl.BonusLeftPV = Convert.ToDecimal(sponsorBonusPVLeft);
                        tbl.BonusLeftBV = 0;
                            tbl.BonusUnitAmount = Convert.ToDecimal(KDMBonusSlabs.SponsorBonus);
                            tbl.PVTotal = totalPVUsed;
                            tbl.PVUnit = KDMBonusSlabs.PVForSponsorBonus;
                        tbl.BVTotal = 0;
                            tbl.BonusUnit = KDMBonusUnitConstants.Taka;
                            kdmDB.tbl_member_bonus_history.Add(tbl);
                            #endregion

                            memberBonusTemp.SponsorBonusBalance = Convert.ToDecimal(sponsorBonusPVLeft);

                            sponsorBonusMaster.SponsorBonus = Convert.ToDecimal((double)sponsorBonusMaster.SponsorBonus + sponsorBonusAmount);

                            kdmDB.SaveChanges();
                           // tr.Commit();

                            ret = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[M_0000002] [ERROR]");
                   // tr.Rollback();
                }

            //}

            return ret;
        }

        // METHOD ID: M_0000003
        public bool UpdateFundMonthlyRoyalityBonusByMemberSource(string MemberID)
        {
            bool ret = false;
            double monthlyRoyalityBonusAmount = 0;
            double monthlyRoyalityBonusPVLeft = 0;
            double monthlyRoyalityBonusNewAmount = 0;

            int totalNoOfPV = 0;
            Int64 totalPVUsed = 0;

            string fundName = "MonthlyRoyaltyBonus";

            using (var tr = db.Database.BeginTransaction())
            {
                try
                {
                    var fundMaster = db.tbl_company_funds_master.Where(x => x.FundName == fundName).FirstOrDefault();
                    var memberBonusTemp = db.tbl_member_bonus_temp.Where(x => x.MemberID == MemberID).FirstOrDefault();

                    if (fundMaster == null)
                    {
                        fundMaster = new tbl_company_funds_master() { FundName= "MonthlyRoyaltyBonus" };
                        db.tbl_company_funds_master.Add(fundMaster);
                        db.SaveChanges();
                    }

                    if (fundMaster != null && memberBonusTemp != null)
                    {
                        var currentTempMonthlyRoyalityBonus = memberBonusTemp.MonthlyRoyalityBonusBalance;

                        if (currentTempMonthlyRoyalityBonus >= KDMBonusSlabs.PVForMonthlyRoyaltyBonus)
                        {
                            totalNoOfPV = (int)(currentTempMonthlyRoyalityBonus / KDMBonusSlabs.PVForMonthlyRoyaltyBonus);
                            totalPVUsed = totalNoOfPV * KDMBonusSlabs.PVForMonthlyRoyaltyBonus;
                            monthlyRoyalityBonusAmount = totalNoOfPV * KDMBonusSlabs.MonthlyRoyaltyBonus;
                            monthlyRoyalityBonusPVLeft = (double)(currentTempMonthlyRoyalityBonus - totalPVUsed);

                            monthlyRoyalityBonusNewAmount = (double)fundMaster.FundAmount + monthlyRoyalityBonusAmount;

                            #region Member Bonus History Create
                            tbl_company_funds_history tbl = new tbl_company_funds_history();
                            tbl.FundName = KDMBonusConstants.MonthlyRoyalty;
                            tbl.FundAmount = Convert.ToDecimal(monthlyRoyalityBonusAmount);
                            tbl.FundTotalAmount = Convert.ToDecimal(monthlyRoyalityBonusNewAmount);
                            tbl.FundUnitAmount = KDMBonusSlabs.MonthlyRoyaltyBonus.ToString();
                            tbl.PVUsed = totalPVUsed;
                            tbl.PVUnit = KDMBonusSlabs.PVForMonthlyRoyaltyBonus;
                            tbl.FundFrom = KDMFundFrom.Member;
                            tbl.FundFromID = MemberID.ToString();
                            db.tbl_company_funds_history.Add(tbl);
                            #endregion

                            memberBonusTemp.MonthlyRoyalityBonusBalance = Convert.ToDecimal(monthlyRoyalityBonusPVLeft);

                            fundMaster.FundAmount = Convert.ToDecimal(monthlyRoyalityBonusNewAmount);

                            db.SaveChanges();
                            tr.Commit();

                            ret = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[M_0000003] [ERROR]");
                    tr.Rollback();
                }

            }

            return ret;
        }

        // METHOD ID: M_0000004
        public bool UpdateFundPerformanceBonusByMemberSource(string MemberID)
        {
            bool ret = false;
            double performanceBonusAmount = 0;
            double performanceBonusNewAmount = 0;
            string fundName = "PerformanceBonus";

            using (var tr = db.Database.BeginTransaction())
            {
                try
                {
                    var fundMaster = db.tbl_company_funds_master.Where(x => x.FundName == fundName).FirstOrDefault();
                    var memberBonusTemp = db.tbl_member_bonus_temp.Where(x => x.MemberID == MemberID).FirstOrDefault();

                    if (fundMaster == null)
                    {
                        fundMaster = new tbl_company_funds_master() { FundName = fundName };
                        db.tbl_company_funds_master.Add(fundMaster);
                        db.SaveChanges();
                    }

                    if (fundMaster != null && memberBonusTemp != null)
                    {
                        var currentTempPerformanceBonus = memberBonusTemp.PerformanceBonusBalance;

                        if (currentTempPerformanceBonus > 0 )
                        {
                            performanceBonusAmount = (double)currentTempPerformanceBonus * (KDMBonusSlabs.PerformanceBonusPercentage/(float)100);
                            performanceBonusNewAmount = (double)fundMaster.FundAmount + performanceBonusAmount;

                            #region Member Bonus History Create
                            tbl_company_funds_history tbl = new tbl_company_funds_history();
                            tbl.FundName = KDMBonusConstants.Performance;
                            tbl.FundAmount = Convert.ToDecimal(performanceBonusAmount);
                            tbl.FundTotalAmount = Convert.ToDecimal(performanceBonusNewAmount);
                            tbl.FundUnitAmount = KDMBonusSlabs.PerformanceBonusPercentage+"%";
                            tbl.PVUsed = 0;
                            tbl.PVUnit = 0;
                            tbl.FundFrom = KDMFundFrom.Member;
                            tbl.FundFromID = MemberID.ToString();
                            db.tbl_company_funds_history.Add(tbl);
                            #endregion

                            memberBonusTemp.PerformanceBonusBalance = 0;

                            fundMaster.FundAmount = Convert.ToDecimal(performanceBonusNewAmount);

                            db.SaveChanges();
                            tr.Commit();

                            ret = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[M_0000004] [ERROR]");
                    tr.Rollback();
                }

            }

            return ret;
        }

        // METHOD ID: M_0000005
        public bool UpdateFundLadershipBonusByMemberSource(string MemberID)
        {
            bool ret = false;
            double leadershipBonusAmount = 0;
            double leadershipBonusNewAmount = 0;
            string fundName = "LeadershipBonus";

            using (var tr = db.Database.BeginTransaction())
            {
                try
                {
                    var fundMaster = db.tbl_company_funds_master.Where(x => x.FundName == fundName).FirstOrDefault();
                    var memberBonusTemp = db.tbl_member_bonus_temp.Where(x => x.MemberID == MemberID).FirstOrDefault();

                    if (fundMaster == null)
                    {
                        fundMaster = new tbl_company_funds_master() { FundName = fundName };
                        db.tbl_company_funds_master.Add(fundMaster);
                        db.SaveChanges();
                    }

                    if (fundMaster != null && memberBonusTemp != null)
                    {
                        var currentTempLeadershipBonus = memberBonusTemp.LeadershipBonusBalance;

                        if (currentTempLeadershipBonus > 0)
                        {
                            leadershipBonusAmount = (double)currentTempLeadershipBonus * (KDMBonusSlabs.LeadershipBonusPercentage / (float)100);
                            leadershipBonusNewAmount = (double)fundMaster.FundAmount + leadershipBonusAmount;

                            #region Member Bonus History Create
                            tbl_company_funds_history tbl = new tbl_company_funds_history();
                            tbl.FundName = KDMBonusConstants.Leadership;
                            tbl.FundAmount = Convert.ToDecimal(leadershipBonusAmount);
                            tbl.FundTotalAmount = Convert.ToDecimal(leadershipBonusNewAmount);
                            tbl.FundUnitAmount = KDMBonusSlabs.LeadershipBonusPercentage + "%";
                            tbl.PVUsed = 0;
                            tbl.PVUnit = 0;
                            tbl.FundFrom = KDMFundFrom.Member;
                            tbl.FundFromID = MemberID.ToString();
                            db.tbl_company_funds_history.Add(tbl);
                            #endregion

                            memberBonusTemp.LeadershipBonusBalance = 0;

                            fundMaster.FundAmount = Convert.ToDecimal(leadershipBonusNewAmount);

                            db.SaveChanges();
                            tr.Commit();

                            ret = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[M_0000005] [ERROR]");
                    tr.Rollback();
                }

            }

            return ret;
        }

        // METHOD ID: M_0000006
        public bool UpdateFundRoyalClubBonusByMemberSource(string MemberID)
        {
            bool ret = false;
            double royalClubBonusAmount = 0;
            double royalClubBonusNewAmount = 0;
            string fundName = "RoyalClubBonus";

            using (var tr = db.Database.BeginTransaction())
            {
                try
                {
                    var fundMaster = db.tbl_company_funds_master.Where(x => x.FundName == fundName).FirstOrDefault();
                    var memberBonusTemp = db.tbl_member_bonus_temp.Where(x => x.MemberID == MemberID).FirstOrDefault();

                    if (fundMaster == null)
                    {
                        fundMaster = new tbl_company_funds_master() { FundName = fundName };
                        db.tbl_company_funds_master.Add(fundMaster);
                        db.SaveChanges();
                    }

                    if (fundMaster != null && memberBonusTemp != null)
                    {
                        var currentTempRoyalClubBonus = memberBonusTemp.RoyalClubBonusBalance;

                        if (currentTempRoyalClubBonus > 0)
                        {
                            royalClubBonusAmount = (double)currentTempRoyalClubBonus * (KDMBonusSlabs.RoyalClubBonusPercentage / (float)100);
                            royalClubBonusNewAmount = (double)fundMaster.FundAmount + royalClubBonusAmount;

                            #region Member Bonus History Create
                            tbl_company_funds_history tbl = new tbl_company_funds_history();
                            tbl.FundName = KDMBonusConstants.RoyalClub;
                            tbl.FundAmount = Convert.ToDecimal(royalClubBonusAmount);
                            tbl.FundTotalAmount = Convert.ToDecimal(royalClubBonusNewAmount);
                            tbl.FundUnitAmount = KDMBonusSlabs.RoyalClubBonusPercentage + "%";
                            tbl.PVUsed = 0;
                            tbl.PVUnit = 0;
                            tbl.FundFrom = KDMFundFrom.Member;
                            tbl.FundFromID = MemberID.ToString();
                            db.tbl_company_funds_history.Add(tbl);
                            #endregion

                            memberBonusTemp.RoyalClubBonusBalance = 0;

                            fundMaster.FundAmount = Convert.ToDecimal(royalClubBonusNewAmount);

                            db.SaveChanges();
                            tr.Commit();

                            ret = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[M_0000006] [ERROR]");
                    tr.Rollback();
                }

            }

            return ret;
        }

        // METHOD ID: M_0000007
        public bool UpdateFundECommerceBonusByMemberSource(string MemberID)
        {
            bool ret = false;
            double ecommerceBonusAmount = 0;
            double ecommerceBonusNewAmount = 0;
            string fundName = "ECommerceBonus";

            using (var tr = db.Database.BeginTransaction())
            {
                try
                {
                    var fundMaster = db.tbl_company_funds_master.Where(x => x.FundName == fundName).FirstOrDefault();
                    var memberBonusTemp = db.tbl_member_bonus_temp.Where(x => x.MemberID == MemberID).FirstOrDefault();

                    if (fundMaster == null)
                    {
                        fundMaster = new tbl_company_funds_master() { FundName = fundName };
                        db.tbl_company_funds_master.Add(fundMaster);
                        db.SaveChanges();
                    }

                    if (fundMaster != null && memberBonusTemp != null)
                    {
                        var currentTempECommerceBonus = memberBonusTemp.ECommerceBonusBalance;

                        if (currentTempECommerceBonus > 0)
                        {
                            ecommerceBonusAmount = (double)currentTempECommerceBonus * (KDMBonusSlabs.RoyalClubBonusPercentage / (float)100);
                            ecommerceBonusNewAmount = (double)fundMaster.FundAmount + ecommerceBonusAmount;

                            #region Member Bonus History Create
                            tbl_company_funds_history tbl = new tbl_company_funds_history();
                            tbl.FundName = KDMBonusConstants.ECommerce;
                            tbl.FundAmount = Convert.ToDecimal(ecommerceBonusAmount);
                            tbl.FundTotalAmount = Convert.ToDecimal(ecommerceBonusNewAmount);
                            tbl.FundUnitAmount = KDMBonusSlabs.ECommerceBonusPercentage + "%";
                            tbl.PVUsed = 0;
                            tbl.PVUnit = 0;
                            tbl.FundFrom = KDMFundFrom.Member;
                            tbl.FundFromID = MemberID.ToString();
                            db.tbl_company_funds_history.Add(tbl);
                            #endregion

                            memberBonusTemp.ECommerceBonusBalance = 0;

                            fundMaster.FundAmount = Convert.ToDecimal(ecommerceBonusNewAmount);

                            db.SaveChanges();
                            tr.Commit();

                            ret = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[M_0000007] [ERROR]");
                    tr.Rollback();
                }

            }

            return ret;
        }

        // METHOD ID: M_0000008
        public bool GiveDealerCommission(string DealerID)
        {
            bool ret = false;
            double dealerCommissionAmount = 0;
            double dealerCommissionNewAmount = 0;
            double dealerCommissionPVLeft = 0;
            int totalNoOfPV = 0;
            Int64 totalPVUsed = 0;

            using (var tr = db.Database.BeginTransaction())
            {
                try
                {
                    var dealerBonusMaster = db.tbl_dealer_stockist_commission_master.Where(x => x.DealerID == DealerID).FirstOrDefault();
                    var memberBonusTemp = db.tbl_member_bonus_temp.Where(x => x.MemberID == DealerID).FirstOrDefault();

                    if (dealerBonusMaster == null)
                    {
                        dealerBonusMaster = new tbl_dealer_stockist_commission_master() { DealerID = DealerID };
                        db.tbl_dealer_stockist_commission_master.Add(dealerBonusMaster);
                        db.SaveChanges();
                    }

                    if (dealerBonusMaster != null && memberBonusTemp != null)
                    {
                        var currentTempDealerBonus = memberBonusTemp.DealerBonusBalance;

                        if (currentTempDealerBonus >= KDMBonusSlabs.PVForDealerCommision)
                        {
                            totalNoOfPV = (int)(currentTempDealerBonus / KDMBonusSlabs.PVForDealerCommision);
                            totalPVUsed = totalNoOfPV * KDMBonusSlabs.PVForDealerCommision;
                            dealerCommissionAmount = totalNoOfPV * KDMBonusSlabs.DealerCommission;
                            dealerCommissionNewAmount = (double)dealerBonusMaster.DealerCommission + dealerCommissionAmount;
                            dealerCommissionPVLeft = (double)(currentTempDealerBonus % KDMBonusSlabs.PVForDealerCommision);

                            #region Member Bonus History Create
                            tbl_member_bonus_history tbl = new tbl_member_bonus_history();
                            tbl.MemberID = DealerID;
                            tbl.BonusSource = KDMBonusConstants.Dealer;
                            tbl.BonusAmount = Convert.ToDecimal(dealerCommissionAmount);
                            tbl.BonusTotalAmount= Convert.ToDecimal(dealerCommissionNewAmount);
                            tbl.BonusUnitAmount = Convert.ToDecimal(KDMBonusSlabs.DealerCommission);
                            tbl.PVTotal = totalPVUsed;
                            tbl.PVUnit = KDMBonusSlabs.PVForDealerCommision;
                            tbl.BonusUnit = KDMBonusUnitConstants.Taka;
                            db.tbl_member_bonus_history.Add(tbl);
                            #endregion

                            memberBonusTemp.DealerBonusBalance = Convert.ToDecimal(dealerCommissionPVLeft);

                            dealerBonusMaster.DealerCommission = Convert.ToDecimal(dealerCommissionNewAmount);

                            db.SaveChanges();
                            tr.Commit();

                            ret = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[M_0000001] [ERROR]");
                    tr.Rollback();
                }

            }

            return ret;
        }


        // METHOD ID: M_0000009
        public bool GiveStockistCommission(string StockistID)
        {
            bool ret = false;
            double stockistCommissionAmount = 0;
            double stockistCommissionNewAmount = 0;
            double stockistCommissionPVLeft = 0;
            int totalNoOfPV = 0;
            Int64 totalPVUsed = 0;

            using (var tr = db.Database.BeginTransaction())
            {
                try
                {
                    var stockistBonusMaster = db.tbl_dealer_stockist_commission_master.Where(x => x.DealerID == StockistID).FirstOrDefault();
                    var memberBonusTemp = db.tbl_member_bonus_temp.Where(x => x.MemberID == StockistID).FirstOrDefault();

                    if (stockistBonusMaster == null)
                    {
                        stockistBonusMaster = new tbl_dealer_stockist_commission_master() { DealerID = StockistID };
                        db.tbl_dealer_stockist_commission_master.Add(stockistBonusMaster);
                        db.SaveChanges();
                    }

                    if (stockistBonusMaster != null && memberBonusTemp != null)
                    {
                        var currentTempStockistBonus = memberBonusTemp.StockistBonusBalance;

                        if (currentTempStockistBonus >= KDMBonusSlabs.PVForStockistCommision)
                        {
                            totalNoOfPV = (int)(currentTempStockistBonus / KDMBonusSlabs.PVForStockistCommision);
                            totalPVUsed = totalNoOfPV * KDMBonusSlabs.PVForStockistCommision;
                            stockistCommissionAmount = totalNoOfPV * KDMBonusSlabs.StockistCommission;
                            stockistCommissionNewAmount = (double)stockistBonusMaster.StockistCommission + stockistCommissionAmount;
                            stockistCommissionPVLeft = (double)(currentTempStockistBonus % KDMBonusSlabs.PVForStockistCommision);

                            #region Member Bonus History Create
                            tbl_member_bonus_history tbl = new tbl_member_bonus_history();
                            tbl.MemberID = StockistID;
                            tbl.BonusSource = KDMBonusConstants.Stockist;
                            tbl.BonusAmount = Convert.ToDecimal(stockistCommissionAmount);
                            tbl.BonusTotalAmount = Convert.ToDecimal(stockistCommissionNewAmount);
                            tbl.BonusUnitAmount = Convert.ToDecimal(KDMBonusSlabs.StockistCommission);
                            tbl.PVTotal = totalPVUsed;
                            tbl.PVUnit = KDMBonusSlabs.PVForDealerCommision;
                            tbl.BonusUnit = KDMBonusUnitConstants.Taka;
                            db.tbl_member_bonus_history.Add(tbl);
                            #endregion

                            memberBonusTemp.StockistBonusBalance = Convert.ToDecimal(stockistCommissionPVLeft);

                            stockistBonusMaster.StockistCommission = Convert.ToDecimal(stockistCommissionNewAmount);

                            db.SaveChanges();
                            tr.Commit();

                            ret = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[M_0000009] [ERROR]");
                    tr.Rollback();
                }

            }

            return ret;
        }

        //METHOD ID: M_0000014
        public bool UpdateMemberBonusTemp(KDMDB kdmDB, string MemberID, string BonusType, double Amount)
        {
            try
            {
                MemberID = MemberID.Trim();
                tbl_member_bonus_temp bonusTemp = null;
                bonusTemp = kdmDB.tbl_member_bonus_temp.Where(x => x.MemberID == MemberID).FirstOrDefault();

                if (bonusTemp == null)
                {
                    bonusTemp = new tbl_member_bonus_temp()
                    {
                        MemberID = MemberID,
                        WalleBonusBalance = 0,
                        SponsorBonusBalance = 0,
                        BinaryMatchingBalance = 0,
                        GenerationBonusBalance = 0,
                        MonthlyRoyalityBonusBalance = 0,
                        PerformanceBonusBalance = 0,
                        LeadershipBonusBalance = 0,
                        RankBonusBalance = 0,
                        RoyalClubBonusBalance = 0,
                        ECommerceBonusBalance = 0,
                        DealerBonusBalance = 0,
                        StockistBonusBalance = 0
                    };

                    kdmDB.tbl_member_bonus_temp.Add(bonusTemp);

                    kdmDB.SaveChanges();

                }
                    

                if(bonusTemp!=null)
                {
                    switch(BonusType)
                    {
                        case "WALLET":
                            bonusTemp.WalleBonusBalance = bonusTemp.WalleBonusBalance + Convert.ToDecimal(Amount);
                            break;
                        case "SPONSOR":
                            bonusTemp.SponsorBonusBalance = bonusTemp.SponsorBonusBalance + Convert.ToDecimal(Amount);
                            kdmDB.Entry(bonusTemp).State = System.Data.Entity.EntityState.Modified;
                            break;
                        case "MONTHLY_ROYALTY":
                            bonusTemp.MonthlyRoyalityBonusBalance = bonusTemp.MonthlyRoyalityBonusBalance + Convert.ToDecimal(Amount);
                            break;
                        case "PERFORMANCE":
                            bonusTemp.PerformanceBonusBalance = bonusTemp.PerformanceBonusBalance + Convert.ToDecimal(Amount);
                            break;
                        case "LEADERSHIP":
                            bonusTemp.LeadershipBonusBalance = bonusTemp.LeadershipBonusBalance + Convert.ToDecimal(Amount);
                            break;
                        case "RANK":
                            bonusTemp.RankBonusBalance = bonusTemp.RankBonusBalance + Convert.ToDecimal(Amount);
                            break;
                        case "ROYAL_CLUB":
                            bonusTemp.RoyalClubBonusBalance = bonusTemp.RoyalClubBonusBalance + Convert.ToDecimal(Amount);
                            break;
                        case "E_COMMERCE":
                            bonusTemp.ECommerceBonusBalance = bonusTemp.ECommerceBonusBalance + Convert.ToDecimal(Amount);
                            break;
                        case "BINARY_MATCHING":

                            var memberTreeForBinaryMatching = kdmDB.tbl_member_tree.Where(x => x.PlacementID == MemberID).FirstOrDefault();
                            memberTreeForBinaryMatching.PV = memberTreeForBinaryMatching.PV + Amount;

                            break;
                        case "GENERATION":

                            var memberTreeGeneration = kdmDB.tbl_member_tree.Where(x => x.PlacementID == MemberID).FirstOrDefault();
                            memberTreeGeneration.BV = memberTreeGeneration.BV + Amount;

                            break;

                    }

                    kdmDB.SaveChanges();

                    return true;
                }
            }
            catch(Exception ex)
            {
                Log.Error(ex, "[M_0000014] [ERROR]");
            }

            return false;

        }
    }
}