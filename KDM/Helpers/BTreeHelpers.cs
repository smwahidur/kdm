using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KDM.Models;
using Newtonsoft.Json;
using System.Web.Mvc;
using Serilog;
using Microsoft.AspNet.Identity;

namespace KDM.Helpers
{
    public class BTreeHelpers
    {
        KDMDB db = new KDMDB();

        public NodeView NullNodeView()
        {
            var nullNodeView = new NodeView(Guid.NewGuid().ToString());
            nullNodeView.text.name = "Name: XXXX";
            nullNodeView.text.title = "ID: XXXX";
            nullNodeView.link.href = "";

            return nullNodeView;
        }

        public string BuildMemberTreeBFS(NodeView root, Int64 level)
        {
            Queue<NodeView> q = new Queue<NodeView>();
            UrlHelper urlHelper = new UrlHelper();
            
            if (root == null)
                return "-1";
            
            var uID = root.HTMLid;
            var u = db.tbl_member_tree.Where(x => x.PlacementID == uID).FirstOrDefault();
            var member = db.tbl_members.Where(x => x.PlacementID == u.PlacementID).Select(x => new {x.MemberID, x.DistributorName }).FirstOrDefault();

            string name = "XXXX";
            string ID = "XXXX";

            if (member != null)
            {
                name = !String.IsNullOrWhiteSpace(member.DistributorName)?member.DistributorName:"XXXX";
                ID = !String.IsNullOrWhiteSpace(member.MemberID) ? member.MemberID : "XXXX";
            }
            
            if (u != null)
            {
                root.text.name = "Name: "+name;
                root.text.title = "ID: " + ID; //u.PlacementID;
                root.text.desc = "Left Point: " + u.LeftPoint + " \r\n" + "Right Point: " + u.RightPoint;
                root.link.href = "/Member/TreeView?uID=" + uID;

            }
            else
            {
                root = NullNodeView();
            }

            q.Enqueue(root);
            var cnt = 1;

            while (q.Count > 0)
            {
                var n = q.Dequeue();
                uID = n.HTMLid;
                var r = db.tbl_member_tree.Where(x => x.PlacementID == uID).FirstOrDefault();
                var children = new List<NodeView>();

                if (r != null)
                {
                    var left = r.LeftID;
                    var right = r.RightID;
                    
                    if (!String.IsNullOrWhiteSpace(left))
                    {
                        var leftNodeView = new NodeView(left);

                        var leftRecord = db.tbl_member_tree.Where(x => x.PlacementID == left).FirstOrDefault();
                        var leftMember = db.tbl_members.Where(x => x.PlacementID == left).Select(x=> new { x.MemberID, x.DistributorName }).FirstOrDefault();

                        string leftMemberName = "XXXX";
                        string leftMemberID = "XXXX";

                        if (leftMember != null)
                        {
                            leftMemberName = !String.IsNullOrWhiteSpace(leftMember.DistributorName) ? leftMember.DistributorName : "XXXX";
                            leftMemberID = !String.IsNullOrWhiteSpace(leftMember.MemberID) ? leftMember.MemberID : "XXXX";
                        }

                        if (leftRecord != null)
                        {
                            leftNodeView.text.name = "Name: "+leftMemberName; //leftRecord.PlacementID;
                            leftNodeView.text.title = "ID: " + leftMemberID; //leftRecord.PlacementID;
                            leftNodeView.text.desc = "Left Point: " + leftRecord.LeftPoint + " \n" + "Right Point: " + leftRecord.RightPoint;
                            leftNodeView.link.href = "/Member/TreeView?uID=" + leftRecord.PlacementID;
                        }
                        else
                        {
                            leftNodeView.text.name = "Name: " + leftMemberName;
                            leftNodeView.text.title = "ID: " + leftMemberID;
                            leftNodeView.text.desc = "Left Point: 0 \n Right Point: 0";
                            leftNodeView.link.href = "/Member/TreeView?uID=" + left;
                        }

                        q.Enqueue(leftNodeView);
                        children.Add(leftNodeView);
                    }
                    else
                    {
                        children.Add(NullNodeView());
                    }

                    if (!String.IsNullOrWhiteSpace(right))
                    {
                        var rightNodeView = new NodeView(right);

                        var rightRecord = db.tbl_member_tree.Where(x => x.PlacementID == right).FirstOrDefault();
                        var rightMember = db.tbl_members.Where(x => x.PlacementID == right).Select(x => new { x.MemberID, x.DistributorName } ).FirstOrDefault();

                        string rightMemberName = "XXXX";
                        string rightMemberID = "XXXX";

                        if (rightMember != null)
                        {
                            rightMemberName = !String.IsNullOrWhiteSpace(rightMember.DistributorName) ? rightMember.DistributorName : "XXXX";
                            rightMemberID = !String.IsNullOrWhiteSpace(rightMember.MemberID) ? rightMember.MemberID : "XXXX";
                        }

                        if (rightRecord != null)
                        {
                            rightNodeView.text.name = "Name: " + rightMemberName;
                            rightNodeView.text.title = "ID: "+ rightMemberID;
                            rightNodeView.text.desc = "Left Point: " + rightRecord.LeftPoint + " \r\n" + "Right Point: " + rightRecord.RightPoint;
                            rightNodeView.link.href = "/Member/TreeView?uID=" + rightRecord.PlacementID;
                        }
                        else
                        {
                            rightNodeView.text.name = "Name: " + rightMemberName;
                            rightNodeView.text.title = "ID: " + rightMemberID;
                            rightNodeView.text.desc = "Left Point: 0\n" + "Right Point: 0";
                            rightNodeView.link.href = "/Member/TreeView?uID=" + right;
                        }

                        q.Enqueue(rightNodeView);
                        children.Add(rightNodeView);
                    }
                    else
                    {
                        children.Add(NullNodeView());
                    }
                }
                else
                {
                    children.Add(NullNodeView());
                    children.Add(NullNodeView());
                }

                n.children = children;

                if (cnt == level)
                {
                    break;
                }
                cnt++;
            }

            return JsonConvert.SerializeObject(root);
        }

        public bool IsLeaf(Node node)
        {
            if (String.IsNullOrWhiteSpace(node.LeftID) && String.IsNullOrWhiteSpace(node.RightID))
            {
                return true;
            }

            return false;
        }

        public bool IsRoot(Node node)
        {
            var root = db.tbl_member_tree.Where(x => x.PlacementID == node.ID && x.IsRoot == true).FirstOrDefault();
            if (root != null)
            {
                return true;
            }

            return false;
        }

        public Node FindParent(Node node)
        {
            var parent = db.tbl_member_tree.Where(x => x.LeftID == node.ID || x.RightID == node.ID).Select(x => new Node()
            {

                ID = x.PlacementID,
                LeftID = x.LeftID,
                RightID = x.RightID,
                IsRoot = x.IsRoot ?? false

            }).FirstOrDefault();

            if (parent != null)
            {

                if (parent.IsRoot)
                {
                    return new Node() { ID = parent.ID, LeftID = String.Empty, RightID = String.Empty, IsRoot = true };
                }

                return parent;
            }

            return null;
        }


        #region PV Calculation 

        public Node FindParentAndUpdatePoint(Node node, double pv = 0, double bv = 0, bool WithBV = false)
        {
            var parent = db.tbl_member_tree.Where(x => x.LeftID == node.ID || x.RightID == node.ID).FirstOrDefault();

            if (parent != null)
            {
                tbl_member_tree_history history = new tbl_member_tree_history();
                history.MemberID = parent.PlacementID;
                history.BLeftPoint = (int)parent.LeftPoint;
                history.BRightPoint = (int)parent.RightPoint;
                history.BOwnPoint = (int)parent.PV;

                if (WithBV)
                {
                    history.BBVPoint = (int)parent.TotalBV;
                }

                double newLeftPoint = (double)parent.LeftPoint + pv;
                double newRightPoint = (double)parent.RightPoint + pv;
                double newTotalBV = 0;

                if (WithBV)
                    newTotalBV = (double)parent.TotalBV + bv;

                if (parent.LeftID == node.ID)
                {
                    parent.LeftPoint = newLeftPoint;
                    history.ALeftPoint = (int)newLeftPoint;
                }
                else if (parent.RightID == node.ID)
                {
                    parent.RightPoint = newRightPoint;
                    history.ARightPoint = (int)newRightPoint;
                }

                if (WithBV)
                    parent.TotalBV = newTotalBV;

                // history entry
                if (WithBV)
                    history.ABVPoint = (int)newTotalBV;

                history.AOwnPoint = 0;
                history.ProcessDate = DateTime.Now;
                db.tbl_member_tree_history.Add(history);
                // end history entry
                db.SaveChanges();

                if (parent.IsRoot == true)
                {
                    return new Node() { ID = parent.PlacementID, LeftID = String.Empty, RightID = String.Empty, IsRoot = true };
                }

                return new Node() { ID = parent.PlacementID, LeftID = parent.LeftID, RightID = parent.RightID, IsRoot = false };
            }

            return null;
        }

        public void TravarseLeafToRoot(Node node, double pv = 0, double bv = 0, bool WithBV = false)
        {
            if (WithBV)
                bv = Math.Floor((bv * (KDMBonusSlabs.GenerationBonus / 100)) / KDMBonusSlabs.GenerationBonusLayer);
            else
                bv = 0;

            var parent = FindParentAndUpdatePoint(node, pv, bv, WithBV);
            var parentCount = 1;
            //var maxBVNode = KDMBonusSlabs.GenerationBonusLayer;

            while (parent != null)
            {
                if (parent.IsRoot)
                    break;

                if (parentCount == KDMBonusSlabs.GenerationBonusLayer)
                    bv = 0;

                parent = FindParentAndUpdatePoint(parent, pv, bv, WithBV);
                parentCount++;
            }
        }

        // Method ID: M_0000010
        public void ProcessBinaryPVBV(bool WithBV = false)
        {
            List<tbl_member_tree> selectedMembers = new List<tbl_member_tree>();

            if (WithBV)
                selectedMembers = db.tbl_member_tree.Where(x => x.PV > 0 || x.BV > 0).ToList();
            else
                selectedMembers = db.tbl_member_tree.Where(x => x.PV > 0).ToList();

            using (var tr = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var member in selectedMembers)
                    {

                        Node node = new Node();
                        node.ID = member.PlacementID;
                        node.LeftID = member.LeftID;
                        node.RightID = member.RightID;

                        double pv = (double)member.PV;
                        double bv = (double)member.BV;

                        TravarseLeafToRoot(node, pv, bv, WithBV);


                        //before history

                        member.PV = 0;

                        if (WithBV)
                            member.BV = 0;

                        //after history

                        db.SaveChanges();
                    }

                    tr.Commit();
                    if (WithBV)
                        Log.Information("[M_0000010] PV BV Binary process completed");
                    else
                        Log.Information("[M_0000010] PV Binary process completed");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[M_0000010] [ERROR]");
                    tr.Rollback();
                }

            }
        }

        #endregion

        #region Generation Bonus Processor

        public Node FindParentAndUpdatePointBV(Node node, double bv = 0)
        {
            var parent = db.tbl_member_tree.Where(x => x.LeftID == node.ID || x.RightID == node.ID).FirstOrDefault();

            if (parent != null)
            {
                tbl_member_tree_history history = new tbl_member_tree_history();
                history.MemberID = parent.PlacementID;
                history.BLeftPoint = (int)parent.LeftPoint;
                history.BRightPoint = (int)parent.RightPoint;
                history.BOwnPoint = (int)parent.PV;

                history.BBVPoint = (int)parent.TotalBV;



                double newTotalBV = (double)parent.TotalBV + bv;


                history.ALeftPoint = (int)parent.LeftPoint;
                history.ARightPoint = (int)parent.RightPoint;

                parent.TotalBV = newTotalBV;

                // history entry

                history.ABVPoint = (int)newTotalBV;

                history.AOwnPoint = (int)parent.PV;
                history.ProcessDate = DateTime.Now;
                db.tbl_member_tree_history.Add(history);
                // end history entry
                db.SaveChanges();

                if (parent.IsRoot == true)
                {
                    return new Node() { ID = parent.PlacementID, LeftID = String.Empty, RightID = String.Empty, IsRoot = true };
                }

                return new Node() { ID = parent.PlacementID, LeftID = parent.LeftID, RightID = parent.RightID, IsRoot = false };
            }

            return null;
        }

        public void TravarseLeafToRootBV(Node node, double bv = 0)
        {
            double[] GBForParents ={ 5, 5, 5, 5, 5, 4, 4, 4, 4, 3, 3, 3, 2, 2, 1};

            double GBForOwn = 10;
            double GBTotalForParents = (double)GBForParents.Sum();
            double GBDistributed = GBForOwn + GBTotalForParents;

            double calculatedOwnGenerationBonus = bv * (GBForOwn / 100);
            double calculatedLeftGenerationBonus = bv * (GBDistributed / 100);
            double[] calculatedGenerationBonusForParents = GBForParents.Select(x => bv * (x / 100)).ToArray();
            //double calculated
            //bv = Math.Floor((bv * (KDMBonusSlabs.GenerationBonus / 100)) / KDMBonusSlabs.GenerationBonusLayer);
            
            var parent = FindParentAndUpdatePointBV(node, calculatedOwnGenerationBonus);
            var parentCount = 1;
            //var maxBVNode = KDMBonusSlabs.GenerationBonusLayer;

            while (parent != null)
            {
                if (parent.IsRoot)
                    break;

                if (parentCount == KDMBonusSlabs.GenerationBonusLayer)
                    bv = calculatedLeftGenerationBonus;

                
                parent = FindParentAndUpdatePoint(parent, calculatedGenerationBonusForParents[parentCount]);
                parentCount++;
            }
        }

        // Method ID: M_0000010
        public void ProcessBinaryBV()
        {
            List<tbl_member_tree> selectedMembers = new List<tbl_member_tree>();

            selectedMembers = db.tbl_member_tree.Where(x => x.BV > 0).ToList();


            using (var tr = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var member in selectedMembers)
                    {

                        Node node = new Node();
                        node.ID = member.PlacementID;
                        node.LeftID = member.LeftID;
                        node.RightID = member.RightID;
                        
                        double bv = (double)member.BV;

                        TravarseLeafToRootBV(node, bv);


                        //before history

                        member.BV = 0;

                        //after history

                        db.SaveChanges();
                    }

                    tr.Commit();

                    Log.Information("[M_0000010] PV BV Binary process completed");

                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[M_0000010] [ERROR]");
                    tr.Rollback();
                }

            }
        }

        #endregion


        public void BinaryMatchingnProcess(string BPCode)
        {
            

            #region Set max point 10000

            var memberListforall = db.tbl_member_tree.Where(y => y.LeftPoint > 10000 || y.RightPoint > 10000).ToList();

            foreach (var m in memberListforall)
            {
                if (m.LeftPoint > 10000)
                {
                    m.LeftPoint = 10000;
                }
                if (m.RightPoint > 10000)
                {
                    m.RightPoint = 10000;
                }
            }

            db.SaveChanges();

            #endregion

            #region Calculate matching and Update Left and Right Points in Tree
            var memberList = db.tbl_member_tree.Where(x => x.LeftPoint >= 500 && x.RightPoint >= 500).ToList();

            using (var tr = db.Database.BeginTransaction())
            {
                try
                {
                    memberList.ForEach(x =>
                    {
                        int matchcount = 0;
                        double? bLeftPoint = x.LeftPoint;
                        double? bRightPoint = x.RightPoint;
                        tbl_member_tree mTree = x;

                        if (x.LeftPoint > x.RightPoint)
                        {
                            if (x.RightPoint > 10000)
                            {
                                matchcount = 20;
                                mTree.LeftPoint = 0;
                                mTree.RightPoint = 0;
                            }
                            else
                            {
                                matchcount = (int)x.RightPoint / 500;
                                mTree.LeftPoint = (x.LeftPoint - (matchcount * 500)) > 10000 ? 10000 : x.LeftPoint - (matchcount * 500);
                                mTree.RightPoint = x.RightPoint - (matchcount * 500);
                            }
                        }
                        else if (x.LeftPoint == x.RightPoint)
                        {

                            if (x.RightPoint > 10000)
                            {
                                matchcount = 20;
                                mTree.RightPoint = 0;
                                mTree.LeftPoint = 0;
                            }
                            else
                            {
                                matchcount = (int)x.RightPoint / 500;
                                mTree.RightPoint = x.RightPoint - (matchcount * 500);
                                mTree.LeftPoint = x.LeftPoint - (matchcount * 500);
                            }
                        }
                        else
                        {
                            if (x.LeftPoint > 10000)
                            {
                                matchcount = 20;
                                mTree.RightPoint = 0;
                                mTree.LeftPoint = 0;
                            }
                            else
                            {
                                matchcount = (int)x.LeftPoint / 500;
                                mTree.RightPoint = (x.RightPoint - (matchcount * 500)) > 10000 ? 10000 : x.RightPoint - (matchcount * 500);
                                mTree.LeftPoint = x.LeftPoint - (matchcount*500);
                            }  
                        }

                        
                        

                        tbl_binary_matching_data bmD = new tbl_binary_matching_data();
                        bmD.PlacementID = x.PlacementID;
                        bmD.BPCode = BPCode;
                        //bmD.ProcessDate = dateParameter;
                        //bmD.InputerID = User.Identity.Name;
                        //bmD.AuthorizerID
                        bmD.BLeftPoint = bLeftPoint;
                        bmD.BRightPoint = bRightPoint;
                        bmD.MatchingCount = matchcount;
                        bmD.ALeftPoint = mTree.LeftPoint;
                        bmD.ARightPoint = mTree.RightPoint;
                        bmD.PostingDate = DateTime.Now.Date;

                        db.Entry(mTree).State = System.Data.Entity.EntityState.Modified;
                        db.tbl_binary_matching_data.Add(bmD);
                        db.SaveChanges();

                    });

                    string user = HttpContext.Current.User.Identity.GetUserName();
                    DateTime processDate = DateTime.Now.Date;
                    DateTime processTime = DateTime.Now;
                    db.tbl_process_code.Add(new tbl_process_code()
                    {
                        ProcessNo=BPCode,
                        ProcessBy=user,
                        ProcessDate=processDate,
                        ProcessTime=processTime,
                        PostingDate=processDate
                    });

                    db.SaveChanges();

                    tr.Commit();
                    Log.Information("[M_0000010] Matching Process Completed");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[M_0000010] [ERROR]");
                    tr.Rollback();
                }

            }

            #endregion
        }

        public void MatchingToMemberAccount(string ProcessNo)
        {
            int PVPoint = Convert.ToInt32(db.tbl_pv_point.Select(s => s.PVPoint).FirstOrDefault());
            var mdata = db.tbl_binary_matching_data.Where(x => x.BPCode == ProcessNo && x.SentToAccount != 1).ToList();
            foreach (var item in mdata)
            {
                tbl_member_account_data tbl = new tbl_member_account_data();
                tbl.trSerialNo = tbl.trSerialNo + 1;
                tbl.MemberID = item.PlacementID;
                tbl.PurposeCode = Convert.ToInt32("1");
                tbl.CreditAmount = Convert.ToDecimal(item.MatchingCount * PVPoint*(0.90));
                tbl.Balance = tbl.Balance + tbl.CreditAmount;
                db.tbl_member_account_data.Add(tbl);

                tbl_tax_account_data tbltax = new tbl_tax_account_data();
                tbltax.trSerialNo = tbltax.trSerialNo + 1;
                tbltax.ForAccount= item.PlacementID;
                tbltax.PurposeCode= Convert.ToInt32("1");
                tbltax.CreditAmount = Convert.ToDecimal(item.MatchingCount * PVPoint * (0.10));
                tbltax.Balance = tbltax.Balance + tbltax.CreditAmount;
                db.tbl_tax_account_data.Add(tbltax);
               
            }

            db.SaveChanges();
        }


    }

}