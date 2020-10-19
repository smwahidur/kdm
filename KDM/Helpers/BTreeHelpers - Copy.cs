using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KDM.Models;
using Newtonsoft.Json;
using System.Web.Mvc;
using Serilog;

namespace KDM.Helpers
{
    public class BTreeHelpers
    {
        KDMDB db = new KDMDB();

        public NodeView NullNodeView()
        {
            var nullNodeView = new NodeView(Guid.NewGuid().ToString());
            nullNodeView.text.name = "XXXX";
            nullNodeView.text.title = "XXXX";
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
            if (u != null)
            {
                root.text.name = u.PlacementID;
                root.text.title = u.PlacementID;
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

                        if (leftRecord != null)
                        {
                            leftNodeView.text.name = leftRecord.PlacementID;
                            leftNodeView.text.title = leftRecord.PlacementID;
                            leftNodeView.text.desc = "Left Point: " + leftRecord.LeftPoint + " \n" + "Right Point: " + leftRecord.RightPoint;
                            leftNodeView.link.href = "/Member/TreeView?uID=" + leftRecord.PlacementID;
                        }
                        else
                        {
                            leftNodeView.text.name = left;
                            leftNodeView.text.title = left;
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

                        if (rightRecord != null)
                        {
                            rightNodeView.text.name = rightRecord.PlacementID;
                            rightNodeView.text.title = rightRecord.PlacementID;
                            rightNodeView.text.desc = "Left Point: " + rightRecord.LeftPoint + " \r\n" + "Right Point: " + rightRecord.RightPoint;
                            rightNodeView.link.href = "/Member/TreeView?uID=" + rightRecord.PlacementID;
                        }
                        else
                        {
                            rightNodeView.text.name = right;
                            rightNodeView.text.title = right;
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
                double newRightPoint = (double)parent.LeftPoint + pv;
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

        public void BinaryGenerationProcess()
        {
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
                            }
                            else
                            {
                                matchcount = (int)x.RightPoint / 500;
                                mTree.LeftPoint = (x.LeftPoint - (matchcount * 500)) > 10000 ? 10000 : x.LeftPoint - (matchcount * 500);
                            }


                            mTree.RightPoint = 0;
                        }
                        else if (x.LeftPoint == x.RightPoint)
                        {

                            if (x.RightPoint > 10000)
                            {
                                matchcount = 20;
                                mTree.RightPoint = 0;
                            }
                            else
                            {
                                matchcount = (int)x.RightPoint / 500;
                                mTree.RightPoint = x.RightPoint - (matchcount * 500);
                            }

                            mTree.LeftPoint = 0;
                        }
                        else
                        {
                            if (x.LeftPoint > 10000)
                            {
                                matchcount = 20;
                                mTree.RightPoint = 0;
                            }
                            else
                            {
                                matchcount = (int)x.LeftPoint / 500;
                                mTree.RightPoint = (x.RightPoint - (matchcount * 500)) > 10000 ? 10000 : x.RightPoint - (matchcount * 500);
                            }

                            mTree.LeftPoint = 0;
                        }


                        tbl_binary_matching_data bmD = new tbl_binary_matching_data();
                        bmD.PlacementID = x.PlacementID;
                        //bmD.BPCode = 
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

                    tr.Commit();
                    Log.Information("[M_0000010] PV BV Generation procession completed");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[M_0000010] [ERROR]");
                    tr.Rollback();
                }

            }
        }


    }

}