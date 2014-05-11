using EventHorizonRider.Core;
using System;
using System.Linq;
using System.Windows.Forms;

namespace EventHorizonRider.Windows
{
    public partial class DevelopmentToolsForm : Form
    {
        private readonly MainGame mainGame;

        public DevelopmentToolsForm(MainGame game)
        {
            mainGame = game;
            mainGame.Initialized += OnGameInitialized;
            InitializeComponent();
        }

        private void OnGameInitialized(object sender, System.EventArgs e)
        {
            propertyGrid.SelectedObject = mainGame.GameContext;
            UpdateTreeView();
        }

        private void UpdateTreeView()
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();

            var root = CreateNode(mainGame);
            var gameContextNode = CreateNode(mainGame.GameContext);
            var levelsNode = CreateNode(mainGame.GameContext.LevelCollection);

            foreach (var level in mainGame.GameContext.LevelCollection.Levels)
            {
                levelsNode.Nodes.Add(CreateNode(level));
            }

            var playerData = CreateNode(mainGame.GameContext.PlayerData);
            var gameState = CreateNode(mainGame.GameContext.GameState);
            var componentsNode = CreateGameComponentsNode(mainGame.GameContext.Root);

            gameContextNode.Nodes.Add(levelsNode);
            gameContextNode.Nodes.Add(playerData);
            gameContextNode.Nodes.Add(gameState);
            gameContextNode.Nodes.Add(componentsNode);

            root.Nodes.Add(gameContextNode);

            treeView.Nodes.Add(root);

            treeView.EndUpdate();

            treeView.ExpandAll();
        }

        private TreeNode CreateNode(object obj)
        {
            var node = new TreeNode(obj.ToString().Split(new [] {'.'}).Last()) {Tag = obj };

            var properties = obj.GetType().GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType.IsClass)
                {
                    node.Nodes.Add(new TreeNode(property.PropertyType.ToString().Split(new[] {'.'}).Last())
                    {
                        Tag = property.GetValue(obj)
                    });
                }
            }

            return node;
        }

        private TreeNode CreateGameComponentsNode(ComponentBase component)
        {
            var node = CreateNode(component);

            if (!component.ChildrenIsEmpty)
            {
                foreach (var child in component.Children)
                {
                    node.Nodes.Add(CreateGameComponentsNode(child));
                }
            }

            return node;
        }

        private void OnNodeClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (propertyGrid.SelectedObject != e.Node.Tag)
            {
                propertyGrid.SelectedObject = e.Node.Tag;
            }
        }

        private void OnTimer(object sender, EventArgs e)
        {
        }

        private void SelectNode(TreeNodeCollection nodes, object tag)
        {
            foreach (var node in nodes.Cast<TreeNode>())
            {
                if (node.Tag == tag)
                {
                    treeView.SelectedNode = node;
                    return;
                }

                SelectNodeRecursive(node, tag);
            }
        }

        private void SelectNodeRecursive(TreeNode node, object tag)
        {
            if (node.Tag == tag)
            {
                treeView.SelectedNode = node;
                return;
            }

            foreach (var child in node.Nodes.Cast<TreeNode>())
            {
                SelectNodeRecursive(child, tag);
            }
        }

        private void OnAfterNodeSelected(object sender, TreeViewEventArgs e)
        {
            e.Node.BackColor = System.Drawing.Color.Yellow;
        }

        private void OnBeforeNodeSelected(object sender, TreeViewCancelEventArgs e)
        {
            if (treeView.SelectedNode != null)
            {
                treeView.SelectedNode.BackColor = System.Drawing.Color.White;
            }
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            object currentObj = null;

            if (treeView.SelectedNode != null)
            {
                currentObj = treeView.SelectedNode.Tag;
            }

            UpdateTreeView();

            SelectNode(treeView.Nodes, currentObj);
        }
    }
}
