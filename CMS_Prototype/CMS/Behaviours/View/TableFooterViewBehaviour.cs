using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using CMS.UI;

namespace CMS.Behaviours
{
    internal class TableFooterViewBehaviour : Behaviour, IViewBehaviour
    {
        public TableFooterViewBehaviour(UserDefinition currentUser) : base(currentUser) { }

        public UI.View Make(ViewDefinition definition, DAL.Models.DbSearchResponse ticketSet, View parentNode, Action<UI.View> viewAction)
        {
            var view = new UI.View()
            {
                ViewType = UI.ViewType.TableFooter,
                ParentNode = parentNode,
                Virtual = true
            };

            if (ticketSet != null && ticketSet.Total > 0)
            {
                var nodeList = new NodeList();
                Node specialNode = null;
                Node node = null;

                for (var i = 1; i <= ticketSet.NumberOfPages; i++)
                {
                    var button = new ButtonPageNumberControlBehaviour(CurrentUser).Make(null, ticketSet, view,
                        (b) => {
                            b.Props.Add("DisplayName", i.ToString());
                            b.Props.Add("IsCurrent", i == ticketSet.PageNumber);
                        });

                    var newNode = new Node(button);

                    if (node == null)
                    {
                        node = newNode;
                        nodeList.Head = node;
                        nodeList.Tail = node;
                    }
                    else
                    {
                        newNode.Prev = node;
                        node.Next = newNode;
                        node = newNode;
                        nodeList.Tail = node;
                    }

                    nodeList.Count++;

                    if (i == ticketSet.PageNumber)
                        specialNode = node;
                }

                var pageNumberButtons = new List<Node>();

                node = nodeList.Head;

                var distanceToHead = 0;
                var distanceToTail = 0;

                while (node != null)
                {
                    if (node == specialNode)
                    {
                        if (distanceToHead > 0)
                        {
                            pageNumberButtons.Add(nodeList.Head);
                            if (distanceToHead > 1)
                            {
                                if (distanceToHead > 2)
                                {
                                    if (distanceToHead > 3)
                                    {
                                        if (distanceToHead > 4)
                                        {
                                            pageNumberButtons.Add(new Node(new Control() { ControlType = ControlType.Label, Value = ".." }));
                                        }
                                        pageNumberButtons.Add(node.Prev.Prev.Prev);
                                    }
                                    pageNumberButtons.Add(node.Prev.Prev);
                                }
                                pageNumberButtons.Add(node.Prev);
                            }
                        }
                        pageNumberButtons.Add(node);
                        distanceToTail = nodeList.Count - 1 - distanceToHead;
                        if (distanceToTail > 0)
                        {
                            pageNumberButtons.Add(node.Next);
                            if (distanceToTail > 1)
                            {
                                pageNumberButtons.Add(node.Next.Next);
                                if (distanceToTail > 2)
                                {
                                    pageNumberButtons.Add(node.Next.Next.Next);
                                    if (distanceToTail > 3)
                                    {
                                        pageNumberButtons.Add(nodeList.Tail);
                                        if (distanceToTail > 4)
                                        {
                                            pageNumberButtons.Insert(pageNumberButtons.Count - 1, new Node(new Control() { ControlType = ControlType.Label, Value = ".." }));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    node = node.Next;
                    distanceToHead++;
                }

                view.Controls = pageNumberButtons.Count > 1 ? pageNumberButtons.Select(n => n.Control).ToList() : new List<Control>();
            }

            return view;
        }


        private class NodeList
        {
            internal Node Head;
            internal Node Tail;
            internal int Count;
        }

        private class Node
        {
            internal Node Prev;
            internal Node Next;
            internal Control Control;

            internal Node(Control control)
            {
                Control = control;
            }
        }
    }
}
