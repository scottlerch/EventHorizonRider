using EventHorizonRider.Core;
using System;
using System.Linq;
using System.Windows.Forms;

namespace EventHorizonRider.Windows;

public partial class DevelopmentToolsForm : Form
{
    private readonly MainGame _mainGame;

    public DevelopmentToolsForm(MainGame game)
    {
        _mainGame = game;
        _mainGame.Initialized += OnGameInitialized;
        InitializeComponent();
    }

    private void OnGameInitialized(object sender, System.EventArgs e)
    {
        propertyGrid.SelectedObject = _mainGame.GameContext;
        UpdateTreeView();

        for (var i = 1; i <= _mainGame.GameContext.Levels.NumberOfLevels; i++)
        {
            var levelNumber = i;
            var menuItem = new ToolStripMenuItem("Level " + i);
            menuItem.Click += (s, a) =>
            {
                _mainGame.GameContext.Root.OverrideLevel = levelNumber;
            };
            menuItem.ShortcutKeys = (Keys.D0 + i) | Keys.Control;
            levelsMenu.DropDownItems.Add(menuItem);
        }
    }

    private void UpdateTreeView()
    {
        treeView.BeginUpdate();
        treeView.Nodes.Clear();

        var root = CreateNode("MainGame", _mainGame);
        var gameContextNode = CreateNode("GameContext", _mainGame.GameContext);

        var levels = new TreeNode("Levels");
        root.Nodes.Add(levels);

        var levelNumber = 1;
        foreach (var level in _mainGame.GameContext.Levels.Levels)
        {
            levels.Nodes.Add(CreateNode("Level " + levelNumber, level));
            levelNumber++;
        }

        root.Nodes.Add(gameContextNode);

        treeView.Nodes.Add(root);

        treeView.EndUpdate();

        treeView.ExpandAll();
    }

    private static TreeNode CreateNode(string name, object obj)
    {
        var node = new TreeNode(name) { Tag = obj };

        var properties = obj.GetType().GetProperties();

        foreach (var property in properties)
        {
            if (property.PropertyType.IsClass &&
                property.PropertyType.FullName.StartsWith("EventHorizon") &&
                property.GetValue(obj) != null)
            {
                node.Nodes.Add(CreateNode(property.Name, property.GetValue(obj)));
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

    private void OnAfterNodeSelected(object sender, TreeViewEventArgs e) => e.Node.BackColor = System.Drawing.Color.Yellow;

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

    private void OnLoad(object sender, EventArgs e)
    {

    }
}
